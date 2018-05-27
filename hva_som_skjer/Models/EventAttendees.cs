using hva_som_skjer.Models;

namespace hva_som_skjer.models
{
    public class EventAttendees
    {
        public int Id { get; set; }
        public string Attendee { get; set; }

        public int EventId { get; set; }
        public Event Event { get; set; }
    }
}