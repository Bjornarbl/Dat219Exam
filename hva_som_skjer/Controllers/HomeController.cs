using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using hva_som_skjer.Models;

namespace hva_som_skjer.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Events()
        {
            ViewData["Message"] = "Liste over arangementer.";

            return View();
        }
        
        public IActionResult Clubs()
        {
            ViewData["Message"] = "List over clubs.";

            return View();
        }

        public IActionResult Calendar()
        {
            ViewData["Message"] = "Calendar of subscribed events.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
