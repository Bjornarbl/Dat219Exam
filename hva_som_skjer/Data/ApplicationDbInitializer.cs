using System.Collections.Generic;
using hva_som_skjer.Models;
using Microsoft.AspNetCore.Identity;

namespace hva_som_skjer.Data
{
    public static class ApplicationDbInitializer
    {
        public static void Initialize(ApplicationDbContext context, UserManager<ApplicationUser> um)
        {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

                context.Clubs.AddRange(new List<ClubModel>{
                new ClubModel("Tufte IL", 
                "Sport", 
                "Fotball", 
                "Per", 
                "Grimstad", 
                "www.test.no", 
                "post@test.no", 
                23131231, 
                1993,
                "images/tufte.gif"),
                new ClubModel("Øl, Viser og Dram", 
                "Kunst og Kultur", 
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
                "Håndball", 
                "Espen", 
                "Kråka, Blubox kjelleren, Jon Lilletunsvei 9, 4877 Grimstad", 
                "www.ro.no", 
                "post@ro.no", 
                321321321, 
                2002,
                "images/roa.png"
                ),
            });

        

            var user = new ApplicationUser{UserName="email@email.com", Email = "email@email.com"};
            um.CreateAsync(user, "Password1.").Wait();
            context.SaveChanges();


        }
    }
}