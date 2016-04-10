using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace Wewy.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public string Hometown { get; set; }

        public string Nickname { get; set; }

        // Last known time zone offset in minutes.
        // For GMT+2 it would be 120.
        public int TimezoneOffsetMinutes { get; set; }

        public double Longitude { get; set; }

        public double Latitude { get; set; }

        public string City { get; set; }

        public string Country { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }

        [InverseProperty("Members")]
        public virtual List<Group> Groups { get; set; }

        [InverseProperty("Admin")]
        public virtual List<Group> GroupsAdministering { get; set; }
        
        [InverseProperty("Creator")]
        public virtual List<Status> CreatedStatuses { get; set; }  
    
        [InverseProperty("Visitor")]
        public virtual List<LastGroupVisit> LastGroupVisits { get; set; }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public DbSet<Group> Groups { get; set; }

        public DbSet<Status> Status { get; set; }

        public DbSet<LastGroupVisit> LastGroupVisits { get; set; }
    }
}