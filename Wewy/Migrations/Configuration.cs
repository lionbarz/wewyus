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

            UserTimeZone laTzD = new UserTimeZone()
            {
                Name = "America/Los_Angeles",
                Offset = -7
            };

            UserTimeZone beirutTz = new UserTimeZone()
            {
                Name = "Asia/Beirut",
                Offset = 2
            };

            UserTimeZone chicagoTz = new UserTimeZone
            {
                Name = "America/Chicago",
                Offset = -6
            };

            UserTimeZone tokyoTz = new UserTimeZone
            {
                Name = "Asia/Tokyo",
                Offset = 9
            };

            UserTimeZone seoulTz = new UserTimeZone
            {
                Name = "Asia/Seoul",
                Offset = 9
            };

            context.UserTimeZones.AddOrUpdate(
                p => p.Name,
                laTzD,
                beirutTz,
                chicagoTz,
                tokyoTz,
                seoulTz);

            context.SaveChanges();

            context.Cities.AddOrUpdate(
                p => p.Name,
                new City()
                {
                    Name = "Austin",
                    UserTimeZoneId = chicagoTz.UserTimeZoneId
                },
                new City()
                {
                    Name = "Beirut",
                    UserTimeZoneId = beirutTz.UserTimeZoneId
                },
                new City()
                {
                    Name = "Los Angeles",
                    UserTimeZoneId = laTzD.UserTimeZoneId
                },
                new City()
                {
                    Name = "San Francisco",
                    UserTimeZoneId = laTzD.UserTimeZoneId
                },
                new City()
                {
                    Name = "Seattle",
                    UserTimeZoneId = laTzD.UserTimeZoneId
                },
                new City()
                {
                    Name = "Seoul",
                    UserTimeZoneId = chicagoTz.UserTimeZoneId
                },
                new City()
                {
                    Name = "Tokyo",
                    UserTimeZoneId = chicagoTz.UserTimeZoneId
                },
                new City()
                {
                    Name = "Tripoli",
                    UserTimeZoneId = beirutTz.UserTimeZoneId
                });
        }
    }
}
