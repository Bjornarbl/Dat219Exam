using System.Collections.Generic;

namespace hva_som_skjer.Models
{
    public class SubscrtiptionViewModel
    {
        public List<CommentModel> Comments{get;set;}
        public List<NewsModel> News{get;set;}

        public ApplicationUser User {get; set;}
    }
}