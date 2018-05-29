namespace hva_som_skjer.Models
{
    public class Attendee
    {
        public int Id { get; set; }

        public ApplicationUser User { get; set; }

        public int EventId { get; set; }
        public Event Event { get; set; }

        public bool Attending { get; set; }

    }
}