using System.ComponentModel.DataAnnotations;

namespace hva_som_skjer.Models
{
    public class ClubModel
    {
        public ClubModel () { }

        public int Id { get; set; }

        [Required]
        [Display(Name="Navn")]
        public string Name { get; set; }

        [Required]
        [Display(Name="Kategori")]
        public string Category { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        [Display(Name="Beskrivelse")]
        public string Description { get; set; }

        [Display(Name="Kontaktperson")]
        public string Contact { get; set; }

        [Display(Name="Adresse")]
        public string Adress { get; set; }

        [DataType(DataType.Url)]
        [Display(Name="Nettsted")]
        public string Website { get; set; }

        [DataType(DataType.EmailAddress)]
        [Display(Name="E-post")]
        public string Email { get; set; }

        [DataType(DataType.PhoneNumber)]
        [Display(Name="Telefon")]
        public string Phone { get; set; }

        [Display(Name="Stiftet")]
        public int Founded { get; set; }
    }
}