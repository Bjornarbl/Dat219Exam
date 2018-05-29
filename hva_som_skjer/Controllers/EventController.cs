using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using hva_som_skjer.Data;
using hva_som_skjer.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace hva_som_skjer.Controllers
{
    public class EventController : Controller
    {
        private ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _um;

        public EventController(ApplicationDbContext db, UserManager<ApplicationUser> um)
        {
            _db = db;
            _um = um;
        }

        // GET: Event/key?ALl
        public async Task<IActionResult> Index(int? key)
        {
            if (key != null)
            {
                var evts = await _db.Events.Where(s => s.ClubId == key).ToListAsync();                
                return View(evts.OrderBy(x => x.StartDate));
            }
   
            var events = await _db.Events.ToListAsync();  
                      
            return View(events.OrderBy(x => x.StartDate));
        }

        // GET: Event
        public async Task<IActionResult> Calendar()
        {
            var events = await _db.Events.ToListAsync();

            return View(events.OrderBy(x => x.StartDate));
        }

        // GET: Event/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _db.Events.Include(x => x.Club).ThenInclude(c => c.Admins).SingleOrDefaultAsync(m => m.Id == id);
            
            if (@event == null)
            {
                return NotFound();
            }

            // The club that created the event
            var club = _db.Clubs.SingleOrDefault(c => c.Id == @event.ClubId);

            if (club == null)
            {
                return NotFound();   
            }

            // List of all Admins in club
            var admins = _db.Admins.Where(a => a.ClubModel.Id == club.Id).Include(x => x.User).ToList();

            if (!admins.Any())
            {
                return NotFound();
            }

            var vm = new EventViewModel();

            vm.Event = @event;
            vm.Club = club;
            vm.Admins = admins;

            return View(vm);        
        }

        // GET: Event/Create/1
        [Authorize]
        public IActionResult Create(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ViewData["clubEvent"] = id;

            // Check if user is admin 
            var admins = _db.Admins.Include(x => x.User).Where(m => m.ClubModel.Id == id).ToList();
            var userName = _um.GetUserName(User);

            if(!admins.Any(a => a.User.UserName == userName))
            {
                return RedirectToAction(nameof(Index));
            }

            return View();
        }

        // POST: Event/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,ClubId,Content,StartDate,StartTime,EndTime,Location")] Event @event, IFormFile file)
        {
            if (ModelState.IsValid)
            {
                if (file != null)
                {
                    string filename = string.Format(@"{0}.png", Guid.NewGuid());
                    string filePath = "/images/events/"+filename;

                    var localPath = Directory.GetCurrentDirectory();
                    localPath += "/wwwroot/" + filePath;

                    if (file.Length > 0)
                    {
                        using (var stream = new FileStream(localPath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }
                    }
                    @event.ImagePath = filePath;
                }
                else 
                {
                    @event.ImagePath = "/images/events/EventDefault.PNG";
                }

                var club = await _db.Clubs.SingleOrDefaultAsync(m => m.Id == @event.ClubId);

                if (club == null)
                {
                    return NotFound();
                }
                
                @event.Club = club;

                club.Events.Add(@event);
                _db.Add(@event);
            
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Details), new { Id = @event.Id });
            }
            return View();
        }

        // GET: Event/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _db.Events.SingleOrDefaultAsync(m => m.Id == id);
            
            if (@event == null)
            {
                return NotFound();
            }

            // Check if user is admin 
            var admins = _db.Admins.Include(x => x.User).Where(m => m.ClubModel.Id == @event.ClubId).ToList();
            var userName = _um.GetUserName(User);

            if(!admins.Any(a => a.User.UserName == userName))
            {
                return RedirectToAction(nameof(Index));
            }

            return View(@event);
        }

        // POST: Event/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ClubId,Title,Content,StartDate,StartTime,EndTime,Location,ImagePath")] Event @event, IFormFile file)
        {
            if (id != @event.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (file != null)
                {
                    string filename = string.Format(@"{0}.png", Guid.NewGuid());
                    string filePath = "/images/events/"+filename;
                    string oldpath = @event.ImagePath;

                    var localPath = Directory.GetCurrentDirectory();
                    localPath += "/wwwroot/" + filePath;

                    if (file.Length > 0)
                    {
                        using (var stream = new FileStream(localPath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }
                    }
                    @event.ImagePath = filePath;

                    if(oldpath != "/images/events/EventDefault.PNG")
                    {
                    string oldpicture = localPath + "/wwwroot/" + oldpath;
                    System.IO.File.Delete(oldpicture);
                    }
                }

                var club = await _db.Clubs.SingleOrDefaultAsync(m => m.Id == @event.ClubId);

                if (club == null)
                {
                    return NotFound();
                }
                
                @event.Club = club;

                try
                {
                    _db.Update(@event);
                    await _db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventExists(@event.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Details), new { Id = @event.Id });
            }
            return View(@event);
        }

        // GET: Event/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _db.Events.SingleOrDefaultAsync(m => m.Id == id);

            if (@event == null)
            {
                return NotFound();
            }

            // Check if user is admin 
            var admins = _db.Admins.Include(x => x.User).Where(m => m.ClubModel.Id == @event.ClubId).ToList();
            var userName = _um.GetUserName(User);

            if(!admins.Any(a => a.User.UserName == userName))
            {
                return RedirectToAction(nameof(Index));
            }

            return View(@event);
        }

        // POST: Event/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var @event = await _db.Events.SingleOrDefaultAsync(m => m.Id == id);
            _db.Events.Remove(@event);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EventExists(int id)
        {
            return _db.Events.Any(e => e.Id == id);
        }
    }
}
