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

        public CommentModel(string content, string author)
        {
            Content = content;
            Posted = DateTime.UtcNow;
        }

        public int Id { get; set; }


        [Display(Name="Innhold")]
        public string Content { get; set; }

        [Display(Name="Publisert")]
        [DataType(DataType.Date)]
        public DateTime Posted { get; set; }
    }
}