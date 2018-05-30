using System.Collections.Generic;
using hva_som_skjer.Models;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Identity;

namespace hva_som_skjer.Data
{
    public static class ApplicationDbInitializer
    {
        public static void Initialize(ApplicationDbContext context, UserManager<ApplicationUser> um, bool IsDevelopment)
        {
            if (!IsDevelopment)
            {
                context.Database.EnsureCreated();
                return;
            }

            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            /* 
                context.Clubs.AddRange(new List<ClubModel>{
                new ClubModel("Tufte IL", 
                "Sport", 
                "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.", 
                "Per", 
                "Grimstad", 
                "www.test.no", 
                "post@test.no", 
                23131231, 
                1993,
                "images/tufte.gif"),
                new ClubModel("Øl, Viser og Dram", 
                "Utdanning", 
                "Studentforeningen Øl, Viser & Dram har som overordnet mål at foreningens medlemmer skal ha det mest mulig moro. Gjennom diverse arrangement skal foreningen også spre glede blandt studentmassen i Grimstad.", 
                "Anna", 
                "Kråka, Blubox kjelleren, Jon Lilletunsvei 9, 4877 Grimstad", 
                "www.ovd.no", 
                "post@ovd.no", 
                79727787, 
                1999,
                "images/ovd.png"),
                new ClubModel("Rø", 
                "Sport", 
                "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.", 
                "Espen", 
                "Kråka, Blubox kjelleren, Jon Lilletunsvei 9, 4877 Grimstad", 
                "www.ro.no", 
                "post@ro.no", 
                321321321, 
                2002,
                "images/roa.png"
                ),
            });
            */

            var user = new ApplicationUser{UserName="email@email.com", Email = "email@email.com"};
            user.ProfilePicture ="../../images/ProfilePictures/tempProfile.png";
            um.CreateAsync(user, "Password1.").Wait();

            var json_clubs = (List<ClubModel>)JsonConvert.DeserializeObject(
                System.IO.File.ReadAllText("Data/clubs.json"),
                typeof(List<ClubModel>)
            );

            foreach(var element in json_clubs)
            {
                element.BannerImage = "../../images/BannerPictures/defaultBanner.png";

                Admin ClubAdmin = new Admin();
                ClubAdmin.User= user;
                ClubAdmin.ClubModel = element;
                
                element.Admins.Add(ClubAdmin);
            }

            context.Clubs.AddRange(json_clubs); 

            // Event dummies
            //var json_events = (List<Event>)JsonConvert.DeserializeObject(
            //    System.IO.File.ReadAllText("Data/events.json"),
            //    typeof(List<Event>)
            //);
            //context.Events.AddRange(json_events); 

            context.SaveChanges();
        }
    }
}