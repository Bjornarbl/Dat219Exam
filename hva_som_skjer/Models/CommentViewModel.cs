using System.Collections.Generic;

namespace hva_som_skjer.Models
{
    public class CommentViewModel
    {
        public CommentViewModel(NewsModel news, List<CommentModel> comment, ApplicationUser user)
        {
            this.NewsModel = news;
            this.CommentModel = comment;
            this.User = user;
        }

        public NewsModel NewsModel{get; set;}
        public List<CommentModel> CommentModel {get; set;}
        public ApplicationUser User{get; set;}
    }
}