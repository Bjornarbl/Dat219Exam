using System.Collections.Generic;

namespace hva_som_skjer.Models
{
    public class CommentViewModel
    {
        public CommentViewModel(NewsModel news, List<CommentModel> comment, List<ApplicationUser> users, ApplicationUser currentuser)
        {
            this.NewsModel = news;
            this.CommentModel = comment;
            this.Users = users;
            this.CurrentUser = currentuser;
        }

        public NewsModel NewsModel{get; set;}
        public List<CommentModel> CommentModel {get; set;}
        public List<ApplicationUser> Users{get; set;}
        public ApplicationUser CurrentUser{get;set;}
    }
}