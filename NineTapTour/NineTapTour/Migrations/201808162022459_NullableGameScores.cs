namespace NineTapTour.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NullableGameScores : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.FinalizeTemps", "Game1", c => c.Int());
            AlterColumn("dbo.FinalizeTemps", "Game2", c => c.Int());
            AlterColumn("dbo.FinalizeTemps", "Game3", c => c.Int());
            AlterColumn("dbo.FinalizeTemps", "Game4", c => c.Int());
            AlterColumn("dbo.PlayerHistories", "Game1", c => c.Int());
            AlterColumn("dbo.PlayerHistories", "Game2", c => c.Int());
            AlterColumn("dbo.PlayerHistories", "Game3", c => c.Int());
            AlterColumn("dbo.PlayerHistories", "Game4", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.PlayerHistories", "Game4", c => c.Int(nullable: false));
            AlterColumn("dbo.PlayerHistories", "Game3", c => c.Int(nullable: false));
            AlterColumn("dbo.PlayerHistories", "Game2", c => c.Int(nullable: false));
            AlterColumn("dbo.PlayerHistories", "Game1", c => c.Int(nullable: false));
            AlterColumn("dbo.FinalizeTemps", "Game4", c => c.Int(nullable: false));
            AlterColumn("dbo.FinalizeTemps", "Game3", c => c.Int(nullable: false));
            AlterColumn("dbo.FinalizeTemps", "Game2", c => c.Int(nullable: false));
            AlterColumn("dbo.FinalizeTemps", "Game1", c => c.Int(nullable: false));
        }
    }
}
