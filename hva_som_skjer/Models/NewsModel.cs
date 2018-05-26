using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace hva_som_skjer.Models
{
    public class NewsModel 
    {
        public NewsModel() 
        {
            Posted = DateTime.UtcNow;
        }

        public NewsModel(string title, string content)
        {
            Content = content;
            Posted = DateTime.UtcNow;
        }

        public int Id { get; set; }

        [Display(Name="Tittel")]
        public string Title { get; set; }

        [Display(Name="Innhold")]
        public string Content { get; set; }

        [Display(Name="Publisert")]
        [DataType(DataType.Date)]
        public DateTime Posted { get; set; }

        public List<CommentModel> Comments { get; set; }
    }
}