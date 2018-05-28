using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using hva_som_skjer.Models;
using hva_som_skjer.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace hva_som_skjer.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _um;

        public HomeController(ApplicationDbContext db ,UserManager<ApplicationUser> um)
        {
            _db = db;
            _um = um;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _um.GetUserAsync(User);
            SubscrtiptionViewModel vm = new SubscrtiptionViewModel();

            if(User.Identity.IsAuthenticated && !( user == null))
            {
                
                var sub= await _db.Subscriptions.ToListAsync();
                var relevantsub = sub.Where(Subscription => Subscription.user == user);

                var Clubs = await _db.Clubs.ToListAsync();
                var relevantClubs = new List<ClubModel>();
                foreach(var s in relevantsub)
                {
                    foreach(var t in Clubs)
                    {
                        if(s.club == t)
                        {
                            relevantClubs.Add(t);
                        }
                    }
                }
                
                var news = await _db.News.OrderByDescending(NewsModel => NewsModel.Id).ToListAsync();
                var relevantNews = new List<NewsModel>();                
                foreach(var t in news)
                {
                    foreach(var s in relevantClubs)
                    {
                        if(t.Club == s)
                        {
                            relevantNews.Add(t);
                        }
                    }
                }
                

                var comments = await _db.Comments.ToListAsync();
                var relevantComments = new List<CommentModel>();

                foreach(var s in comments)
                {
                    foreach(var t in relevantNews)
                    {
                        if(s.news == t)
                        {
                            relevantComments.Add(s);
                        }
                    }
                } 

                vm.Comments = relevantComments;
                vm.News = relevantNews;                
                vm.User = user;
                
            }
            return View(vm);    
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
