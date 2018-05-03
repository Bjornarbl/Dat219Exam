using System.Collections.Generic;
using hva_som_skjer.Models;

namespace hva_som_skjer.Data
{
    public static class ApplicationDbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            context.Clubs.Add(new ClubModel(
                "Øl, Viser og Dram", 
                "Kunst og Kultur", 
                "Studentforeningen Øl, Viser & Dram har som overordnet mål at foreningens medlemmer skal ha det mest mulig moro. Gjennom diverse arrangement skal foreningen også spre glede blandt studentmassen i Grimstad.", 
                "Anna", 
                "Kråka, Blubox kjelleren, Jon Lilletunsvei 9, 4877 Grimstad", 
                "www.ovd.no", 
                "post@ovd.no", 
                79727787, 
                1999));

            context.SaveChanges();
        }
    }
}