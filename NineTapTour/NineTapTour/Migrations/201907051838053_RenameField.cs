namespace NineTapTour.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenameField : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PlayerHistories", "AverageForEntry", c => c.Double(nullable: false));
            DropColumn("dbo.PlayerHistories", "AverageForGame");
        }
        
        public override void Down()
        {
            AddColumn("dbo.PlayerHistories", "AverageForGame", c => c.Double(nullable: false));
            DropColumn("dbo.PlayerHistories", "AverageForEntry");
        }
    }
}
