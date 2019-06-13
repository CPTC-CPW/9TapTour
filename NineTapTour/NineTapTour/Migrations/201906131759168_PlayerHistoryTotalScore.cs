namespace NineTapTour.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PlayerHistoryTotalScore : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.PlayerHistories", "TotalScore");
        }
        
        public override void Down()
        {
            AddColumn("dbo.PlayerHistories", "TotalScore", c => c.Int(nullable: false));
        }
    }
}
