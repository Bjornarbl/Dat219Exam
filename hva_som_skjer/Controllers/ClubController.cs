using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using hva_som_skjer.Models;
using hva_som_skjer.Data;
using Microsoft.EntityFrameworkCore;

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

        public async Task<IActionResult> CreateClub()
        {
            return View();
        }

        public async Task<IActionResult> Clubs() {
            return View(await _db.Clubs.ToListAsync());
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
