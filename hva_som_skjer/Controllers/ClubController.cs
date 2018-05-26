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

            var club = await _db.Clubs.SingleOrDefaultAsync(m => m.Id == id);

            if (club == null)
            {
                return NotFound();
            }

            return View(club);
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

        [HttpPost]
        public async Task<IActionResult> UploadLogo(List<IFormFile> files, int Id)
        {
            string filename = string.Format(@"{0}.png", Guid.NewGuid());
            var localPath = Directory.GetCurrentDirectory();
            string filePath = "\\data\\LogoPictures\\"+filename;
            string wholePath = localPath+filePath;

            var club = await _db.Clubs.SingleOrDefaultAsync(m => m.Id == Id);
            
            if (files[0].Length > 0)
            {
                
                var oldPicture = club.Image;
                club.Image = wholePath;
                _db.Clubs.Update(club);
                _db.SaveChanges();

                using (var stream = new FileStream(wholePath, FileMode.Create))
                {
                    await files[0].CopyToAsync(stream);
                }
                //TODO: delete old profile picture. Problem is Access to path * is denied
            }    
            
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> UploadBanner(List<IFormFile> files, int Id)
        {
            
            
            if (files[0].Length > 0)
            {
                string filename = string.Format(@"{0}.png", Guid.NewGuid());
                var club = await _db.Clubs.SingleOrDefaultAsync(m => m.Id == Id);                
                var oldPicture = club.BannerImage;
                club.BannerImage = "../../../Data/BannerPictures/"+filename;
                _db.Clubs.Update(club);
                _db.SaveChanges();


            var localPath = Directory.GetCurrentDirectory();
            string filePath = "\\data\\BannerPictures\\"+filename;
            string wholePath = localPath+filePath;
                using (var stream = new FileStream(wholePath, FileMode.Create))
                {
                    await files[0].CopyToAsync(stream);
                }
                //TODO: delete old profile picture. Problem is Access to path * is denied
            }else
            {
                return RedirectToAction(nameof(Index));
            }    
            
            return RedirectToAction(nameof(Index));
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

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Generate(ClubModel club)
        {
            var localPath = Directory.GetCurrentDirectory();

            club.Image = "../../images/LogoPictures/tempLogo.png";
            club.BannerImage = "../../images/BannerPictures/defaultBanner.png";

            var user = await _um.GetUserAsync(User);
            club.InitializeLists(user);

            
             
            _db.Clubs.Add(club);
            _db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
