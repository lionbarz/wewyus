namespace Wewy.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModifiedDate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Status", "DateModifiedUtc", c => c.DateTime());
            AddColumn("dbo.Status", "DateModifiedCreator", c => c.DateTime());
            AddColumn("dbo.Status", "DateModifiedLover", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Status", "DateModifiedLover");
            DropColumn("dbo.Status", "DateModifiedCreator");
            DropColumn("dbo.Status", "DateModifiedUtc");
        }
    }
}
