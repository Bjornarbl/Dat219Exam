using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using hva_som_skjer.Data;
using hva_som_skjer.Models;
using System;
using System.Collections.Generic;

namespace ViewComponents {

    public class ClubListViewComponent {

        private ApplicationDbContext _db;

        public ClubListViewComponent(ApplicationDbContext db) {
            this._db = db;
        }

        public async Task<IViewComponentResult> InvokeAsync(int clubCount = 10)
        {
            var articles = await _db.Clubs.Take(clubCount).ToListAsync(); // Fetch data
            return View(articles); // Send data to the view
        }

        private IViewComponentResult View(List<ClubModel> articles)
        {
            throw new NotImplementedException();
        }
    }
}