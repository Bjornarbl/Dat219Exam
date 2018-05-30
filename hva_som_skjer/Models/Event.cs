using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using hva_som_skjer.Models;

namespace hva_som_skjer.Models
{
    public class Event 
    {
        public Event() 
        {
            Created = DateTime.UtcNow;
        }

        public int Id { get; set; }

        public int ClubId { get; set; }
        public ClubModel Club { get; set; }

        public List<Attendee> Attendees { get; set; }

        [StringLength(60, MinimumLength = 3)]
        [Display(Name="Tittel*")]
        [Required]
        public string Title { get; set; }

        [Display(Name="Detaljer")]
        public string Content { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name="Dato*")]
        public DateTime StartDate { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
        [Display(Name="Tidspunkt*")]
        public DateTime StartTime { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
        [Display(Name="Sluttidspunkt*")]
        public DateTime EndTime { get; set; }

        [Display(Name="Sted")]
        public string Location { get; set; }

        [DataType(DataType.Date)]
        public DateTime Created { get; set; }

        public string ImagePath { get; set; }
    }
}