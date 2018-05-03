using System;
using System.ComponentModel.DataAnnotations;

namespace hva_som_skjer.Models
{
    public class NewsModel 
    {
        public NewsModel() 
        {
            Posted = DateTime.UtcNow;
        }

        public NewsModel(string title, string content, string club)
        {
            Content = content;
            Posted = DateTime.UtcNow;
            Club = club;
        }

        public int Id { get; set; }

        public string Club { get; set; }

        [Display(Name="Tittel")]
        public string Title { get; set; }

        [Display(Name="Innhold")]
        public string Content { get; set; }

        [Display(Name="Publisert")]
        [DataType(DataType.Date)]
        public DateTime Posted { get; set; }
    }
}