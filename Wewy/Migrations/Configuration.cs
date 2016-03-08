namespace Wewy.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using Models;

    internal sealed class Configuration : DbMigrationsConfiguration<Wewy.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(Wewy.Models.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            UserTimeZone laTz = new UserTimeZone()
            {
                Name = "America/Los_Angeles",
                Offset = -8
            };

            UserTimeZone beirutTz = new UserTimeZone()
            {
                Name = "Asia/Beirut",
                Offset = 2
            };

            UserTimeZone austinTz = new UserTimeZone
            {
                Name = "America/Houston",
                Offset = -6
            };

            context.UserTimeZones.AddOrUpdate(
                p => p.Name,
                laTz,
                beirutTz,
                austinTz);

            context.SaveChanges();

            context.Cities.AddOrUpdate(
                p => p.Name,
                new City()
                {
                    Name = "Los Angeles",
                    UserTimeZoneId = laTz.UserTimeZoneId
                },
                new City()
                {
                    Name = "Seattle",
                    UserTimeZoneId = laTz.UserTimeZoneId
                },
                new City()
                {
                    Name = "Beirut",
                    UserTimeZoneId = beirutTz.UserTimeZoneId
                },
                new City()
                {
                    Name = "Tripoli",
                    UserTimeZoneId = beirutTz.UserTimeZoneId
                },
                new City()
                {
                    Name = "Austin",
                    UserTimeZoneId = austinTz.UserTimeZoneId
                });
        }
    }
}
