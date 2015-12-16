namespace NineTapTour.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Game123arenullable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Games", "Game1", c => c.Int());
            AlterColumn("dbo.Games", "Game2", c => c.Int());
            AlterColumn("dbo.Games", "Game3", c => c.Int());
            AlterColumn("dbo.Games", "Game4", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Games", "Game4", c => c.Int(nullable: false));
            AlterColumn("dbo.Games", "Game3", c => c.Int(nullable: false));
            AlterColumn("dbo.Games", "Game2", c => c.Int(nullable: false));
            AlterColumn("dbo.Games", "Game1", c => c.Int(nullable: false));
        }
    }
}
