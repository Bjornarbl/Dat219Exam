using System.Collections.Generic;

namespace hva_som_skjer.Models
{
    public class NewsViewModel
    {
        public List<NewsModel> NewsModel{get; set;}
        public List<CommentModel> CommentModel {get; set;}
        public ClubModel Club{get; set;}
        public ApplicationUser User{get; set;}
        public bool isAdmin{get; set;}
    }
}