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

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //

            //UserTimeZone tz = new UserTimeZone()
            //{
            //    Name = "America/Los_Angeles",
            //    Offset = -8
            //};

            //context.UserTimeZones.AddOrUpdate(tz);
            //context.Cities.AddOrUpdate(new City() { Name = "Seattle", UserTimeZone = tz, });

            //tz = new UserTimeZone()
            //{
            //    Name = "Asia/Beirut",
            //    Offset = 2
            //};

            //context.UserTimeZones.AddOrUpdate(tz);
            //context.Cities.AddOrUpdate(new City() { Name = "Beirut", UserTimeZone = tz, });
        }
    }
}
