using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using static System.Net.Mime.MediaTypeNames;

namespace hva_som_skjer.Models
{   
    public class ClubModel
    {
        public ClubModel()
        {
            Admins = new List<Admin>();
            News = new Collection<NewsModel>();
        }

        //denne brukes til json filen til å populere siden
        public ClubModel(string name,   string category, string description, string contact,
                         string adress, string website,  string email,       string phone, int founded, string image) 
        { 
            this.Name = name;
            this.Category = category;
            this.Description = description;
            this.Contact = contact;
            this.Adress = adress;
            this.Website = website;
            this.Email = email;
            this.Phone = phone;
            this.Founded = founded;
            this.Image = image;

        }

        //TODO: List for admin users, news, comments and events.
        [Key]
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
        public int? Founded { get; set; }

        public string Image {get; set; }

        public string BannerImage {get; set; }

        public ICollection<Admin> Admins{get; set;}

        public ICollection<NewsModel> News{get; set;}

    }

    public class Admin
    {
        public Admin(){}

        [Key]
        public int ClubAdminId{get; set;}

        public ApplicationUser admin{get; set;}


    }

    public class NewsPost
    {

        [Key]
        public int ClubAdminId{get; set;}

        public ApplicationUser admin{get; set;}

        public ClubModel ClubModel{get; set;}        
    }
}
