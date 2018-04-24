using System;
using System.ComponentModel.DataAnnotations;

namespace hva_som_skjer.Models
{
    public class CommentModel 
    {
        public CommentModel() 
        {
            Posted = DateTime.UtcNow;
        }

        public CommentModel(string title, string summary, string content, ApplicationUser user)
        {
            Content = content;
            Posted = DateTime.UtcNow;
            User = user.Id;
        }

        public int Id { get; set; }

        public string User { get; set; }

        [Display(Name="Innhold")]
        public string Content { get; set; }

        [DataType(DataType.Date)]
        public DateTime Posted { get; set; }
    }
}