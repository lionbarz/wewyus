namespace Wewy.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveCityId : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.StatusViews", "CityId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.StatusViews", "CityId", c => c.Int(nullable: false));
        }
    }
}
