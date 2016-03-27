namespace Wewy.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class GroupAdmins : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Groups", "AdminId", c => c.String(maxLength: 128));
            CreateIndex("dbo.Groups", "AdminId");
            AddForeignKey("dbo.Groups", "AdminId", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Groups", "AdminId", "dbo.AspNetUsers");
            DropIndex("dbo.Groups", new[] { "AdminId" });
            DropColumn("dbo.Groups", "AdminId");
        }
    }
}
