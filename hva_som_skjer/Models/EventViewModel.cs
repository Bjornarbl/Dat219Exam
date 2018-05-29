using System.Collections.Generic;

namespace hva_som_skjer.Models
{
    public class EventViewModel
    {
        public Event Event { get; set; }

        public ClubModel Club{ get; set; }

        public List<Admin> Admins { get; set; }
    }
}