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

namespace hva_som_skjer.Controllers
{
    public class ClubController : Controller
    {
        private ApplicationDbContext _db;

        public ClubController(ApplicationDbContext db)
        {
            _db = db;
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


        [HttpPost]
        public IActionResult Generate(ClubModel club)
        {
            var localPath = Directory.GetCurrentDirectory();

            club.Image = "images/tempLogo.png";
            club.BannerImage = "images/tempLogo.png";

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
