using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace hva_som_skjer.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public string ProfilePicture { get; set; }
        public List<Subscription> subscriptions{get; set;}

    }

    public class Subscription
    {      
        public int Id{get; set;}
        public ApplicationUser user{get; set;}
        public ClubModel club {get; set;}
    }
}
