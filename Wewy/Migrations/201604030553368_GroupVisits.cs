namespace Wewy.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class GroupVisits : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.LastGroupVisits",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        VisitTimeUtc = c.DateTime(nullable: false),
                        VisitorId = c.String(maxLength: 128),
                        GroupId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Groups", t => t.GroupId)
                .ForeignKey("dbo.AspNetUsers", t => t.VisitorId)
                .Index(t => t.VisitorId)
                .Index(t => t.GroupId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.LastGroupVisits", "VisitorId", "dbo.AspNetUsers");
            DropForeignKey("dbo.LastGroupVisits", "GroupId", "dbo.Groups");
            DropIndex("dbo.LastGroupVisits", new[] { "GroupId" });
            DropIndex("dbo.LastGroupVisits", new[] { "VisitorId" });
            DropTable("dbo.LastGroupVisits");
        }
    }
}
