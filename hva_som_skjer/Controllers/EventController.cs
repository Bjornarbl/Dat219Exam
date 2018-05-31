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
            var vm = new EventViewModel();

            if (key != null)
            {
                var evts = await _db.Events.Where(s => s.ClubId == key).OrderBy(x => x.StartDate).ToListAsync();  
                var upcommingEvts = evts.Where(m => m.StartDate > DateTime.UtcNow).ToList();
                var archivedEvts = evts.Where(m => m.StartDate < DateTime.UtcNow).ToList();

                vm.Events = upcommingEvts;
                vm.ArchivedEvents = archivedEvts;

                return View(vm);
            }
   
            var events = await _db.Events.OrderBy(x => x.StartDate).ToListAsync();  
            var upcommingEvents = events.Where(m => m.StartDate > DateTime.UtcNow).ToList();
            var archivedEvents = events.Where(m => m.StartDate < DateTime.UtcNow).ToList();

            vm.Events = upcommingEvents;
            vm.ArchivedEvents = archivedEvents;
                      
            return View(vm);
        }

        // GET: Event
        public async Task<IActionResult> Calendar()
        {
            var events = new List<Event>();

            var userName = _um.GetUserName(User);

            // All deltagelse for brukeren
            var userEvents = await _db.Attendees.Include(x => x.Event).Where(m => m.User.UserName == userName).ToArrayAsync();

            foreach (var m in userEvents)
            {
                events.Add(m.Event);
            }

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


            var userName = _um.GetUserName(User);
            var eventAttendes = _db.Attendees.Include(x => x.User).Where(m => m.EventId == @event.Id);
            
            foreach(var a in eventAttendes)
            {
                if(a.User.UserName == userName)
                {
                    vm.Attending = true;
                }
            }

            vm.Event = @event;
            vm.Club = club;
            vm.Admins = admins;
            vm.numAttendees = eventAttendes.Count();

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
                    @event.ImagePath = "/images/events/event-default.png";
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
                    string imagePath = "/images/events/" + filename;
                    string oldImagePath = @event.ImagePath;
                    var localPath = Directory.GetCurrentDirectory();

                    var filePath = localPath + "/wwwroot/" + imagePath;
                    string oldFilePath = localPath + "/wwwroot/" + oldImagePath;

                    if (file.Length > 0)
                    {
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }
                    }
                    @event.ImagePath = imagePath;

                    if(oldImagePath != "/images/events/event-default.png")
                    {
                        System.IO.File.Delete(oldFilePath);
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
            
            var oldpath = @event.ImagePath;
            var localPath = Directory.GetCurrentDirectory();
            string oldpicture = localPath + "/wwwroot/" + oldpath;

            if(oldpath != "/images/events/event-default.png")
            {
                System.IO.File.Delete(oldpicture);
            }

            _db.Events.Remove(@event);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Going(int eventId)
        {
            var @event = await _db.Events.SingleOrDefaultAsync(m => m.Id == eventId);
            var user = await _um.GetUserAsync(User);

            var attendee = new Attendee();
            attendee.Event = @event;
            attendee.EventId = eventId;
            attendee.User = user;

            _db.Attendees.Add(attendee);
            await _db.SaveChangesAsync();

            return RedirectToAction("Details", new{ID = eventId});
        }

        [HttpPost]
        public async Task<IActionResult> NotGoing(int eventId)
        {
            var @event = await _db.Events.SingleOrDefaultAsync(m => m.Id == eventId);
            var user = await _um.GetUserAsync(User);

            var attendees = await _db.Attendees.ToListAsync();
            var eventAttendees = attendees.Where(m => m.EventId == @event.Id);

            foreach (var a in eventAttendees)
            {
                if (a.User.UserName == user.UserName)
                {
                   _db.Attendees.Attach(a);
                   _db.Attendees.Remove(a);
                   _db.SaveChanges();
                }
            }

            return RedirectToAction("Details", new{ID = eventId});
        }

        private bool EventExists(int id)
        {
            return _db.Events.Any(e => e.Id == id);
        }
    }
}
