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

            var vm = new NewsViewModel();

            vm.NewsModel = _db.News.OrderByDescending(NewsModel => NewsModel.Id).ToList();
            vm.CommentModel =_db.Comments.ToList();

            vm.Club = clubtemp;
            vm.User = await _um.GetUserAsync(User);

            return View(vm);
        }

        public async Task<IActionResult> Search(string name) 
        {
            if (name == null)
            {
                return NotFound();
            }

            var clubs = _db.Clubs.Where(s => s.Name.ToLower().Contains(name.ToLower()) || 
            s.Category.ToLower().Contains(name.ToLower()) ||
            s.Description.ToLower().Contains(name.ToLower()) 
            
            );
            if (!clubs.Any())
            {
                return NotFound();
            }

            return View(clubs.ToList());
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

        public async Task<IActionResult> CreateClub()
        {
            return View();
        }

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

            return View(club);
        }
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

            return View(club);
        }

        [HttpPost]
        public async Task<IActionResult> UploadLogo(List<IFormFile> files, int Id)
        {
            var club = await _db.Clubs.SingleOrDefaultAsync(m => m.Id == Id);
            
            try
            {
                 string filename = string.Format(@"{0}.png", Guid.NewGuid());
                
                var oldPicture = club.Image;
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
            }catch{ return RedirectToAction("EditClub",new{ID = club.Id});}    
            
            return RedirectToAction("Club",new{ID = club.Id});
        }

        [HttpPost]
        public async Task<IActionResult> UploadBanner(List<IFormFile> files, int Id)
        {
            var club = await _db.Clubs.SingleOrDefaultAsync(m => m.Id == Id);
            
            try
            {
                string filename = string.Format(@"{0}.png", Guid.NewGuid());
                                
                var oldPicture = club.BannerImage;
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

        [HttpPost]
        public async Task<IActionResult> AddComment(CommentModel comment)
        {
            
            var news = await _db.News.SingleOrDefaultAsync(m => m.Id == comment.NewsId);

            var club = await _db.Clubs.SingleOrDefaultAsync(m => m.Id == news.clubId);

            var user = await _um.GetUserAsync(User);
            comment.Author = user.UserName;
            comment.AuthorPicture = user.ProfilePicture;
            comment.news = news;
        

            _db.Comments.Add(comment);
            _db.SaveChanges();

            return RedirectToAction("Club",new{ID = club.Id});
        }


        [HttpPost]
        public async Task<IActionResult> Generate(ClubModel club)
        {
            Admin ClubAdmin = new Admin();

            club.Admins.Add(ClubAdmin);
            _db.Clubs.Add(club);


            club.Image = "../../images/LogoPictures/tempLogo.png";
            club.BannerImage = "../../images/BannerPictures/defaultBanner.png";

            var user = await _um.GetUserAsync(User);
            
            ClubAdmin.ClubModel = club;
            ClubAdmin.User = user;

            //_db.Clubs.Update(club);
            //_db.Admins.Update(ClubAdmin);
            _db.SaveChanges();

            return RedirectToAction("Club",new{ID = club.Id});
        }



        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
