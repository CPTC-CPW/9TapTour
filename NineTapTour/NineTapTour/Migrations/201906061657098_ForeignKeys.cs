namespace NineTapTour.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ForeignKeys : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Participants", "NineTapRegion_NineTapRegionID", c => c.Int());
            AddColumn("dbo.Tournaments", "NineTapRegion_NineTapRegionID", c => c.Int());
            CreateIndex("dbo.Games", "gameRegionID");
            CreateIndex("dbo.Participants", "NineTapRegion_NineTapRegionID");
            CreateIndex("dbo.Tournaments", "NineTapRegion_NineTapRegionID");
            AddForeignKey("dbo.Games", "gameRegionID", "dbo.NineTapRegions", "NineTapRegionID", cascadeDelete: true);
            AddForeignKey("dbo.Participants", "NineTapRegion_NineTapRegionID", "dbo.NineTapRegions", "NineTapRegionID");
            AddForeignKey("dbo.Tournaments", "NineTapRegion_NineTapRegionID", "dbo.NineTapRegions", "NineTapRegionID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Tournaments", "NineTapRegion_NineTapRegionID", "dbo.NineTapRegions");
            DropForeignKey("dbo.Participants", "NineTapRegion_NineTapRegionID", "dbo.NineTapRegions");
            DropForeignKey("dbo.Games", "gameRegionID", "dbo.NineTapRegions");
            DropIndex("dbo.Tournaments", new[] { "NineTapRegion_NineTapRegionID" });
            DropIndex("dbo.Participants", new[] { "NineTapRegion_NineTapRegionID" });
            DropIndex("dbo.Games", new[] { "gameRegionID" });
            DropColumn("dbo.Tournaments", "NineTapRegion_NineTapRegionID");
            DropColumn("dbo.Participants", "NineTapRegion_NineTapRegionID");
        }
    }
}
