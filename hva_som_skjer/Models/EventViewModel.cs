using System.Collections.Generic;

namespace hva_som_skjer.Models
{
    public class EventViewModel
    {
        public List<Event> Events { get; set; }
        public List<Event> ArchivedEvents { get; set; }
        public Event Event { get; set; }

        public ClubModel Club{ get; set; }

        public List<Admin> Admins { get; set; }

        public bool Attending { get; set; }

        public int numAttendees { get; set; }
    }
}