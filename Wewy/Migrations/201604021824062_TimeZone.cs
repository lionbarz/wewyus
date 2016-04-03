namespace Wewy.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TimeZone : DbMigration
    {
        public override void Up()
        {
            RenameColumn("dbo.UserTimeZones", "Name", "JavascriptName");
            AddColumn("dbo.UserTimeZones", "WindowsRegistryName", c => c.String());
            DropColumn("dbo.UserTimeZones", "Offset");
        }
        
        public override void Down()
        {
            AddColumn("dbo.UserTimeZones", "Offset", c => c.Int(nullable: false));
            DropColumn("dbo.UserTimeZones", "WindowsRegistryName");
            RenameColumn("dbo.UserTimeZones", "JavascriptName", "Name");
        }
    }
}
