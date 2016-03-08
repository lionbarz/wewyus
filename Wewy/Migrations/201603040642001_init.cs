namespace Wewy.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init : DbMigration
    {
        public override void Up()
        {
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
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Hometown = c.String(),
                        Nickname = c.String(),
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
                        DateCreatedLocal = c.DateTime(nullable: false),
                        DateModifiedUtc = c.DateTime(),
                        DateModifiedLocal = c.DateTime(),
                        CreatorId = c.String(maxLength: 128),
                        GroupId = c.Int(nullable: false),
                        CreatorCityId = c.Int(nullable: false),
                        Text = c.String(),
                    })
                .PrimaryKey(t => t.StatusId)
                .ForeignKey("dbo.Groups", t => t.GroupId)
                .ForeignKey("dbo.AspNetUsers", t => t.CreatorId)
                .ForeignKey("dbo.Cities", t => t.CreatorCityId)
                .Index(t => t.CreatorId)
                .Index(t => t.GroupId)
                .Index(t => t.CreatorCityId);
            
            CreateTable(
                "dbo.Groups",
                c => new
                    {
                        GroupId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.GroupId);
            
            CreateTable(
                "dbo.StatusViews",
                c => new
                    {
                        StatusViewId = c.Int(nullable: false, identity: true),
                        StatusId = c.Int(nullable: false),
                        ViewerId = c.String(maxLength: 128),
                        CityId = c.Int(nullable: false),
                        LocalTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.StatusViewId)
                .ForeignKey("dbo.Cities", t => t.CityId)
                .ForeignKey("dbo.Status", t => t.StatusId)
                .ForeignKey("dbo.AspNetUsers", t => t.ViewerId)
                .Index(t => t.StatusId)
                .Index(t => t.ViewerId)
                .Index(t => t.CityId);
            
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
                "dbo.UserTimeZones",
                c => new
                    {
                        UserTimeZoneId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Offset = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.UserTimeZoneId);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.ApplicationUserGroups",
                c => new
                    {
                        ApplicationUser_Id = c.String(nullable: false, maxLength: 128),
                        Group_GroupId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ApplicationUser_Id, t.Group_GroupId })
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUser_Id, cascadeDelete: true)
                .ForeignKey("dbo.Groups", t => t.Group_GroupId, cascadeDelete: true)
                .Index(t => t.ApplicationUser_Id)
                .Index(t => t.Group_GroupId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.Cities", "UserTimeZoneId", "dbo.UserTimeZones");
            DropForeignKey("dbo.Status", "CreatorCityId", "dbo.Cities");
            DropForeignKey("dbo.AspNetUsers", "CurrentCityId", "dbo.Cities");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.ApplicationUserGroups", "Group_GroupId", "dbo.Groups");
            DropForeignKey("dbo.ApplicationUserGroups", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Status", "CreatorId", "dbo.AspNetUsers");
            DropForeignKey("dbo.StatusViews", "ViewerId", "dbo.AspNetUsers");
            DropForeignKey("dbo.StatusViews", "StatusId", "dbo.Status");
            DropForeignKey("dbo.StatusViews", "CityId", "dbo.Cities");
            DropForeignKey("dbo.Status", "GroupId", "dbo.Groups");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.ApplicationUserGroups", new[] { "Group_GroupId" });
            DropIndex("dbo.ApplicationUserGroups", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.StatusViews", new[] { "CityId" });
            DropIndex("dbo.StatusViews", new[] { "ViewerId" });
            DropIndex("dbo.StatusViews", new[] { "StatusId" });
            DropIndex("dbo.Status", new[] { "CreatorCityId" });
            DropIndex("dbo.Status", new[] { "GroupId" });
            DropIndex("dbo.Status", new[] { "CreatorId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AspNetUsers", new[] { "CurrentCityId" });
            DropIndex("dbo.Cities", new[] { "UserTimeZoneId" });
            DropTable("dbo.ApplicationUserGroups");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.UserTimeZones");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.StatusViews");
            DropTable("dbo.Groups");
            DropTable("dbo.Status");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.Cities");
        }
    }
}
