namespace Wewy.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Relationships",
                c => new
                    {
                        RelationshipId = c.Int(nullable: false, identity: true),
                        FirstId = c.String(maxLength: 128),
                        SecondId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.RelationshipId)
                .ForeignKey("dbo.AspNetUsers", t => t.FirstId)
                .ForeignKey("dbo.AspNetUsers", t => t.SecondId)
                .Index(t => t.FirstId)
                .Index(t => t.SecondId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Hometown = c.String(),
                        CurrentCityId = c.Int(nullable: false),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Cities", t => t.CurrentCityId)
                .Index(t => t.CurrentCityId)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Status",
                c => new
                    {
                        StatusId = c.Int(nullable: false, identity: true),
                        DateCreatedUtc = c.DateTime(nullable: false),
                        DateCreatedCreator = c.DateTime(nullable: false),
                        DateCreatedLover = c.DateTime(nullable: false),
                        CreatorId = c.String(maxLength: 128),
                        RelationshipId = c.Int(nullable: false),
                        Text = c.String(),
                        CreatorCityId = c.Int(nullable: false),
                        LoverCityId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.StatusId)
                .ForeignKey("dbo.Cities", t => t.CreatorCityId)
                .ForeignKey("dbo.Cities", t => t.LoverCityId)
                .ForeignKey("dbo.Relationships", t => t.RelationshipId)
                .ForeignKey("dbo.AspNetUsers", t => t.CreatorId)
                .Index(t => t.CreatorId)
                .Index(t => t.RelationshipId)
                .Index(t => t.CreatorCityId)
                .Index(t => t.LoverCityId);
            
            CreateTable(
                "dbo.Cities",
                c => new
                    {
                        CityId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        UserTimeZoneId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.CityId)
                .ForeignKey("dbo.UserTimeZones", t => t.UserTimeZoneId)
                .Index(t => t.UserTimeZoneId);
            
            CreateTable(
                "dbo.UserTimeZones",
                c => new
                    {
                        UserTimeZoneId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Offset = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.UserTimeZoneId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Relationships", "SecondId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Relationships", "FirstId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Status", "CreatorId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Status", "RelationshipId", "dbo.Relationships");
            DropForeignKey("dbo.Cities", "UserTimeZoneId", "dbo.UserTimeZones");
            DropForeignKey("dbo.AspNetUsers", "CurrentCityId", "dbo.Cities");
            DropForeignKey("dbo.Status", "LoverCityId", "dbo.Cities");
            DropForeignKey("dbo.Status", "CreatorCityId", "dbo.Cities");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.Cities", new[] { "UserTimeZoneId" });
            DropIndex("dbo.Status", new[] { "LoverCityId" });
            DropIndex("dbo.Status", new[] { "CreatorCityId" });
            DropIndex("dbo.Status", new[] { "RelationshipId" });
            DropIndex("dbo.Status", new[] { "CreatorId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AspNetUsers", new[] { "CurrentCityId" });
            DropIndex("dbo.Relationships", new[] { "SecondId" });
            DropIndex("dbo.Relationships", new[] { "FirstId" });
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.UserTimeZones");
            DropTable("dbo.Cities");
            DropTable("dbo.Status");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.Relationships");
        }
    }
}
