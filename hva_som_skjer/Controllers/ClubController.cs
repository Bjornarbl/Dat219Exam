using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using hva_som_skjer.Models;
using hva_som_skjer.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace hva_som_skjer.Controllers
{
    public class ClubController : Controller
    {
        private ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _um;


        public ClubController(ApplicationDbContext db ,UserManager<ApplicationUser> um)
        {
            _db = db;
            _um = um;
            
        }

        public async Task<IActionResult> Index() {
            return View(await _db.Clubs.ToListAsync());
        }
        
        public async Task<IActionResult> Club(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var clubtemp = await _db.Clubs.SingleOrDefaultAsync(m => m.Id == id);

            if (clubtemp == null)
            {
                return NotFound();
            }

            var vm = new Models.NewsViewModel();

            vm.NewsModel = _db.News.OrderByDescending(NewsModel => NewsModel.Id).ToList();
            vm.CommentModel =_db.Comments.ToList();

            var user = await _um.GetUserAsync(User);
            vm.Club = clubtemp;
            vm.Users = _db.Users.ToList();
            vm.CurrentUser = user;
            vm.isAdmin = false;
            if(User.Identity.IsAuthenticated && !(user == null))
            {
                var admins = await _db.Admins.ToListAsync();
                var relevantAdmins = admins.Where(Admin => Admin.User == user);
                
                foreach(var s in relevantAdmins)
                {
                    if(s.ClubModel == clubtemp)
                    {
                        vm.isAdmin = true;
                    }
                }

                var subscriptions = await _db.Subscriptions.ToListAsync();
                var relevantSub = subscriptions.Where(Subscription => Subscription.user == user);

                foreach(var s in relevantSub)
                {
                    if(s.club == clubtemp)
                    {
                        vm.isFollowing = true;
                    }
                }

            }
            return View(vm);
        }

        public async Task<IActionResult> Search(ClubViewModel vm) 
        {
            if (vm == null)
            {
                vm = new ClubViewModel();
                vm.Limit = 10;
            }

            // Do not perform a search if the search is invalid
            if (!ModelState.IsValid)
                return BadRequest();

            // Perform search, one filter at a time
            var results = _db.Clubs.AsQueryable();

            if (vm.Search != null)
                results = results.Where(p => p.Name.ToLower().Contains(vm.Search.ToLower()) ||
                p.Category.ToLower().Contains(vm.Search.ToLower())
                || p.Founded.Equals(vm.Search)
                );
            
            if(vm.Type == "Category" && vm.OrderBy == "A")
                results = results.OrderBy(p=> p.Category);
            
            if(vm.Type == "Category" && vm.OrderBy == "Z")
                results = results.OrderByDescending(p => p.Category);

            if(vm.Type == "Founded" && vm.OrderBy == "A")
                results = results.OrderByDescending(p => p.Founded);

            if(vm.Type == "Founded" && vm.OrderBy == "Z")
                results = results.OrderBy(p=> p.Founded);
                

            if(vm.Type == "Name" && vm.OrderBy == "A")
                results = results.OrderBy(p=> p.Name);

            if(vm.Type == "Name" && vm.OrderBy == "Z") 
                results = results.OrderByDescending(p => p.Name);

            if(vm.Limit <= 0) {
                vm.Limit = 1;
            }
            // This is what actually fetches the data
            vm.Results = results.ToList();
            
            // Send result to view
            return View(vm);
        } 

        public async Task<IActionResult> Clubs(string category) 
        {
            if (category == null)
            {
                return NotFound();
            }

            var clubs = _db.Clubs.Where(s => s.Category.Contains(category));
              if (clubs == null)
            {
                return NotFound();
            }

            return View(clubs.ToList());
        }

        [Authorize]
        public async Task<IActionResult> CreateClub()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> EditClub(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var club = await _db.Clubs.SingleOrDefaultAsync(m => m.Id == id);

            if (club == null)
            {
                return NotFound();
            }

            if(User.Identity.IsAuthenticated )
            {
                var user = await _um.GetUserAsync(User);
                if(!(user == null))
                {
                    bool isAdmin = false;
                    var admins = await _db.Admins.ToListAsync();
                    var relevantAdmins = admins.Where(Admin => Admin.User == user);             
                    foreach(var s in relevantAdmins)
                    {
                        if(s.ClubModel == club)
                        {
                            isAdmin = true;
                        }
                    }
                    if(isAdmin)
                    {
                        return View(club);
                    }else
                    {
                        return RedirectToAction("Club",new{ID = club.Id});
                    }
                }    
            }
            return RedirectToAction("Club",new{ID = club.Id});
        }
        [Authorize]
        public async Task<IActionResult> AddNews(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var club = await _db.Clubs.SingleOrDefaultAsync(m => m.Id == id);

            if (club == null)
            {
                return NotFound();
            }

            if(User.Identity.IsAuthenticated )
            {
                var user = await _um.GetUserAsync(User);
                if(!(user == null))
                {
                    bool isAdmin = false;
                    var admins = await _db.Admins.ToListAsync();
                    var relevantAdmins = admins.Where(Admin => Admin.User == user);             
                    foreach(var s in relevantAdmins)
                    {
                        if(s.ClubModel == club)
                        {
                            isAdmin = true;
                        }
                    }
                    if(isAdmin)
                    {
                        return View(club);
                    }else
                    {
                        return RedirectToAction("Club",new{ID = club.Id});
                    }
                }    
            }
            return RedirectToAction("Club",new{ID = club.Id});
        }

        [HttpPost]
        public async Task<IActionResult> UploadLogo(List<IFormFile> files, int Id)
        {
            var club = await _db.Clubs.SingleOrDefaultAsync(m => m.Id == Id);
            
            try
            {
                var oldPicture = club.Image;
                string filename = string.Format(@"{0}.png", Guid.NewGuid());
                club.Image = "../../images/LogoPictures/"+filename;
                _db.Clubs.Update(club);
                _db.SaveChanges();

                var localPath = Directory.GetCurrentDirectory();
                string filePath = "\\wwwroot\\images\\LogoPictures\\"+filename;
                string wholePath = localPath+filePath;
                using (var stream = new FileStream(wholePath, FileMode.Create))
                {
                    await files[0].CopyToAsync(stream);
                }
                //TODO: delete old profile picture. Problem is Access to path * is denied
                if(oldPicture != "../../images/LogoPictures/tempLogo.png")
                {
                    char[] MyChar = {'.', '.','/' };
                    string tempName = oldPicture.TrimStart(MyChar);
                    tempName = "../hva_som_skjer/wwwroot/"+tempName;
                    System.IO.File.Delete(tempName);
                }
            }catch{ return RedirectToAction("EditClub",new{ID = club.Id});}    
            
            return RedirectToAction("Club",new{ID = club.Id});
        }

        [HttpPost]
        public async Task<IActionResult> UploadBanner(List<IFormFile> files, int Id)
        {
            var club = await _db.Clubs.SingleOrDefaultAsync(m => m.Id == Id);
            
            try
            {
                var oldPicture = club.BannerImage;
                string filename = string.Format(@"{0}.png", Guid.NewGuid());
                club.BannerImage = "../../images/BannerPictures/"+filename;
                _db.Clubs.Update(club);
                _db.SaveChanges();


                var localPath = Directory.GetCurrentDirectory();
                string filePath = "\\wwwroot\\images\\BannerPictures\\"+filename;
                string wholePath = localPath+filePath;
                using (var stream = new FileStream(wholePath, FileMode.Create))
                {
                    await files[0].CopyToAsync(stream);
                }
            //TODO: delete old profile picture. Problem is Access to path * is denied
            if(oldPicture != "../../images/BannerPictures/defaultBanner.png")
                {
                    char[] MyChar = {'.', '.','/' };
                    string tempName = oldPicture.TrimStart(MyChar);
                    tempName = "../hva_som_skjer/wwwroot/"+tempName;
                    System.IO.File.Delete(tempName);
                }
            }catch
            {
                return RedirectToAction(nameof(Index));
            }    
            
            return RedirectToAction("Club",new{ID = club.Id});
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ClubModel Club, int Id)
        {
            var club = await _db.Clubs.SingleOrDefaultAsync(m => m.Id == Id);
            club.Name = Club.Name;
            if(Club.Category != "Empty"){club.Category = Club.Category;}
            club.Description = Club.Description;
            club.Contact     = Club.Contact;
            club.Adress      = Club.Adress;
            if(Club.Website != ""){club.Website = Club.Website;}
            if(Club.Email   != ""){club.Email   = Club.Email;}
            club.Phone       = Club.Phone;
            club.Founded     = Club.Founded;

            _db.Clubs.Update(club);
            _db.SaveChanges();

            return RedirectToAction("Club",new{ID = club.Id});
        }

        [HttpPost]
        public async Task<IActionResult> AddNewsMethod(NewsModel news, int ClubId)
        {
            _db.Add(news);
            
            var club = await _db.Clubs.SingleOrDefaultAsync(m => m.Id == ClubId);
            var user = await _um.GetUserAsync(User);
            news.Club = club;
            news.poster = user.UserName;

            _db.SaveChanges();

            return RedirectToAction("Club",new{ID = club.Id});
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddComment(CommentModel comment, string returnURL)
        {
            
            var news = await _db.News.SingleOrDefaultAsync(m => m.Id == comment.NewsId);

            var club = await _db.Clubs.SingleOrDefaultAsync(m => m.Id == news.clubId);

            var user = await _um.GetUserAsync(User);
            comment.Author = user;
            comment.news = news;
        

            _db.Comments.Add(comment);
            _db.SaveChanges();

            return RedirectToAction("Club",new{ID = club.Id});
        }

        [HttpPost]
        public async Task<IActionResult> Subscribe(int clubId)
        {
            var user = await _um.GetUserAsync(User);
            var club = await _db.Clubs.SingleOrDefaultAsync(m => m.Id == clubId);

            Subscription subscription = new Subscription();
            subscription.club = club;
            subscription.user = user;  

            _db.Add(subscription);
            _db.SaveChanges();

            return RedirectToAction("Club",new{ID = club.Id});
        }

        [HttpPost]
        public async Task<IActionResult> Unsubscribe(int clubId)
        {
            var club = await _db.Clubs.SingleOrDefaultAsync(m => m.Id == clubId);
            var user = await _um.GetUserAsync(User);

            var subscriptions = await _db.Subscriptions.ToListAsync();
            var relevantSub = subscriptions.Where(Subscription => Subscription.user == user);

            foreach(var s in relevantSub)
            {
                if(s.club == club)
                {
                   _db.Subscriptions.Attach(s);
                   _db.Subscriptions.Remove(s);
                   _db.SaveChanges();
                }
            }
            return RedirectToAction("Club",new{ID = clubId});
        }

        [HttpPost]
        public async Task<IActionResult> Generate(ClubModel club, IFormFile logo, IFormFile banner)
        {
            Admin ClubAdmin = new Admin();

            club.Admins.Add(ClubAdmin);
            _db.Clubs.Add(club);

            if(logo == null)
            {
                club.Image = "../../images/LogoPictures/tempLogo.png";
            }else
            {
                try
                {
                    string filename = string.Format(@"{0}.png", Guid.NewGuid());
                    club.Image = "../../images/LogoPictures/"+filename;
                    _db.SaveChanges();

                    var localPath = Directory.GetCurrentDirectory();
                    string filePath = "\\wwwroot\\images\\LogoPictures\\"+filename;
                    string wholePath = localPath+filePath;
                    using (var stream = new FileStream(wholePath, FileMode.Create))
                    {
                        await logo.CopyToAsync(stream);
                    }
                
                }catch{ club.Image = "../../images/LogoPictures/tempLogo.png";}  
            }
            if(banner == null)
            {
                club.BannerImage = "../../images/BannerPictures/defaultBanner.png";
            }else
            {
                try
                {
                    string filename = string.Format(@"{0}.png", Guid.NewGuid());
                    club.BannerImage = "../../images/BannerPictures/"+filename;
                    _db.SaveChanges();

                    var localPath = Directory.GetCurrentDirectory();
                    string filePath = "\\wwwroot\\images\\BannerPictures\\"+filename;
                    string wholePath = localPath+filePath;
                    using (var stream = new FileStream(wholePath, FileMode.Create))
                    {
                        await banner.CopyToAsync(stream);
                    }
                
                }catch{ club.BannerImage = "../../images/BannerPictures/defaultBanner.png";}
            }
            
            var user = await _um.GetUserAsync(User);
            
            ClubAdmin.ClubModel = club;
            ClubAdmin.User = user;
            _db.SaveChanges();

            return RedirectToAction("Club",new{ID = club.Id});
        }

        [HttpPost]
        public async Task<IActionResult> AddAdmin(string Name, int Id)
        {
            var club = await _db.Clubs.SingleOrDefaultAsync(m => m.Id == Id);
            var user = await _db.Users.ToListAsync();
            var selectedUser = user.Where(ApplicationUser => ApplicationUser.UserName == Name);
            if(selectedUser != null)
            {
                Admin admin = new Admin();
                admin.User = selectedUser.FirstOrDefault();
                admin.ClubModel = club;

                _db.Add(admin);
                _db.SaveChanges();
            }
            return RedirectToAction("Club",new{ID = club.Id});
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
