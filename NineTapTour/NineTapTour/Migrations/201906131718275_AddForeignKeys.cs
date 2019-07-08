namespace NineTapTour.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddForeignKeys : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.Games", "gameRegionID");
            AddForeignKey("dbo.Games", "gameRegionID", "dbo.NineTapRegions", "NineTapRegionID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Games", "gameRegionID", "dbo.NineTapRegions");
            DropIndex("dbo.Games", new[] { "gameRegionID" });
        }
    }
}
