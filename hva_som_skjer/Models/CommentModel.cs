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

        public CommentModel(string content, ApplicationUser author)
        {
            Content = content;
            Posted = DateTime.UtcNow;
            Author = author;
        }

        public int Id { get; set; }

        public ApplicationUser Author { get; set; }

        [Display(Name="Innhold")]
        public string Content { get; set; }

        [Display(Name="Publisert")]
        [DataType(DataType.Date)]
        public DateTime Posted { get; set; }
    }
}