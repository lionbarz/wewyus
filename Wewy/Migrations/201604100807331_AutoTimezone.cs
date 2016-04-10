namespace Wewy.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AutoTimezone : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.StatusViews", "CityId", "dbo.Cities");
            DropForeignKey("dbo.AspNetUsers", "CurrentCityId", "dbo.Cities");
            DropForeignKey("dbo.Status", "CreatorCityId", "dbo.Cities");
            DropForeignKey("dbo.Cities", "UserTimeZoneId", "dbo.UserTimeZones");
            DropIndex("dbo.Cities", new[] { "UserTimeZoneId" });
            DropIndex("dbo.AspNetUsers", new[] { "CurrentCityId" });
            DropIndex("dbo.Status", new[] { "CreatorCityId" });
            DropIndex("dbo.StatusViews", new[] { "CityId" });
            AddColumn("dbo.AspNetUsers", "TimezoneOffsetMinutes", c => c.Int(nullable: false));
            AddColumn("dbo.AspNetUsers", "Longitude", c => c.Double(nullable: false));
            AddColumn("dbo.AspNetUsers", "Latitude", c => c.Double(nullable: false));
            AddColumn("dbo.AspNetUsers", "City", c => c.String());
            AddColumn("dbo.AspNetUsers", "Country", c => c.String());
            AddColumn("dbo.Status", "City", c => c.String());
            AddColumn("dbo.Status", "Country", c => c.String());
            AddColumn("dbo.Status", "Latitude", c => c.Double(nullable: false));
            AddColumn("dbo.Status", "Longitude", c => c.Double(nullable: false));
            AddColumn("dbo.StatusViews", "City", c => c.String());
            AddColumn("dbo.StatusViews", "Country", c => c.String());
            AddColumn("dbo.StatusViews", "Latitude", c => c.Double(nullable: false));
            AddColumn("dbo.StatusViews", "Longitude", c => c.Double(nullable: false));
            DropColumn("dbo.AspNetUsers", "CurrentCityId");
            DropColumn("dbo.Status", "CreatorCityId");
            DropTable("dbo.Cities");
            DropTable("dbo.UserTimeZones");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.UserTimeZones",
                c => new
                    {
                        UserTimeZoneId = c.Int(nullable: false, identity: true),
                        JavascriptName = c.String(),
                        WindowsRegistryName = c.String(),
                    })
                .PrimaryKey(t => t.UserTimeZoneId);
            
            CreateTable(
                "dbo.Cities",
                c => new
                    {
                        CityId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        UserTimeZoneId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.CityId);
            
            AddColumn("dbo.Status", "CreatorCityId", c => c.Int(nullable: false));
            AddColumn("dbo.AspNetUsers", "CurrentCityId", c => c.Int(nullable: false));
            DropColumn("dbo.StatusViews", "Longitude");
            DropColumn("dbo.StatusViews", "Latitude");
            DropColumn("dbo.StatusViews", "Country");
            DropColumn("dbo.StatusViews", "City");
            DropColumn("dbo.Status", "Longitude");
            DropColumn("dbo.Status", "Latitude");
            DropColumn("dbo.Status", "Country");
            DropColumn("dbo.Status", "City");
            DropColumn("dbo.AspNetUsers", "Country");
            DropColumn("dbo.AspNetUsers", "City");
            DropColumn("dbo.AspNetUsers", "Latitude");
            DropColumn("dbo.AspNetUsers", "Longitude");
            DropColumn("dbo.AspNetUsers", "TimezoneOffsetMinutes");
            CreateIndex("dbo.StatusViews", "CityId");
            CreateIndex("dbo.Status", "CreatorCityId");
            CreateIndex("dbo.AspNetUsers", "CurrentCityId");
            CreateIndex("dbo.Cities", "UserTimeZoneId");
            AddForeignKey("dbo.Cities", "UserTimeZoneId", "dbo.UserTimeZones", "UserTimeZoneId");
            AddForeignKey("dbo.Status", "CreatorCityId", "dbo.Cities", "CityId");
            AddForeignKey("dbo.AspNetUsers", "CurrentCityId", "dbo.Cities", "CityId");
            AddForeignKey("dbo.StatusViews", "CityId", "dbo.Cities", "CityId");
        }
    }
}
