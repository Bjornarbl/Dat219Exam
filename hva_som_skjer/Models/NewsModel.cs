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
            Comments = new List<CommentModel>();
        }

        public NewsModel(string title, string content, ClubModel club)
        {
            this.Title = title;
            this.Content = content;
            this.Posted = DateTime.UtcNow;
            this.Comments = new List<CommentModel>();
            this.Club = club;
        }

        public int Id { get; set; }

        [Display(Name="Tittel")]
        public string Title { get; set; }

        [Display(Name="Innhold")]
        public string Content { get; set; }

        [Display(Name="Publisert")]
        [DataType(DataType.Date)]
        public DateTime Posted { get; set; }

        [Display(Name="Poster")]
        public string poster { get; set; }

        public string Image {get; set; }

        
        public int clubId {get; set;}
        public ClubModel Club {get; set;}
        


        public List<CommentModel> Comments { get; set; }
    }
}