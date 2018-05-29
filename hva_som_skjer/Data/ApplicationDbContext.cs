using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using hva_som_skjer.Models;

namespace hva_som_skjer.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }

        public DbSet<Models.ClubModel> Clubs { get; set; } 
        public DbSet<Models.NewsModel> News { get; set; }
        public DbSet<Models.CommentModel> Comments { get; set; }
        public DbSet<Models.Admin> Admins { get; set; } 
        public DbSet<Models.Event> Events { get; set; }
        public DbSet<Models.Subscription> Subscriptions { get; set; } 
        public DbSet<Models.Attendee> Attendees { get; set; }
    }
}
