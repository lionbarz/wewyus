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
                JavascriptName = "America/Los_Angeles",
                WindowsRegistryName = "Pacific Standard Time"
            };

            UserTimeZone beirutTz = new UserTimeZone()
            {
                JavascriptName = "Asia/Beirut",
                WindowsRegistryName = "Jordan Standard Time"
            };

            UserTimeZone chicagoTz = new UserTimeZone
            {
                JavascriptName = "America/Chicago",
                WindowsRegistryName = "Central Standard Time"
            };

            UserTimeZone tokyoTz = new UserTimeZone
            {
                JavascriptName = "Asia/Tokyo",
                WindowsRegistryName = "Tokyo Standard Time"
            };

            UserTimeZone seoulTz = new UserTimeZone
            {
                JavascriptName = "Asia/Seoul",
                WindowsRegistryName = "Korea Standard Time"
            };

            context.UserTimeZones.AddOrUpdate(
                p => p.JavascriptName,
                laTz,
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
                    UserTimeZoneId = laTz.UserTimeZoneId
                },
                new City()
                {
                    Name = "San Francisco",
                    UserTimeZoneId = laTz.UserTimeZoneId
                },
                new City()
                {
                    Name = "Seattle",
                    UserTimeZoneId = laTz.UserTimeZoneId
                },
                new City()
                {
                    Name = "Seoul",
                    UserTimeZoneId = seoulTz.UserTimeZoneId
                },
                new City()
                {
                    Name = "Tokyo",
                    UserTimeZoneId = tokyoTz.UserTimeZoneId
                },
                new City()
                {
                    Name = "Tripoli",
                    UserTimeZoneId = beirutTz.UserTimeZoneId
                });
        }
    }
}
