namespace NineTapTour.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddForeignKeys : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.Games", "gameRegionID");
            CreateIndex("dbo.Participants", "ParticipantRegionID");
            CreateIndex("dbo.Tournaments", "TourneyRegion");
            CreateIndex("dbo.PlayerHistories", "GameID");
            CreateIndex("dbo.PlayerHistories", "regionID");
            AddForeignKey("dbo.Games", "gameRegionID", "dbo.NineTapRegions", "NineTapRegionID", cascadeDelete: true);
            AddForeignKey("dbo.Participants", "ParticipantRegionID", "dbo.NineTapRegions", "NineTapRegionID", cascadeDelete: true);
            AddForeignKey("dbo.Tournaments", "TourneyRegion", "dbo.NineTapRegions", "NineTapRegionID", cascadeDelete: true);
            AddForeignKey("dbo.PlayerHistories", "GameID", "dbo.Games", "Id", cascadeDelete: true);
            AddForeignKey("dbo.PlayerHistories", "regionID", "dbo.NineTapRegions", "NineTapRegionID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PlayerHistories", "regionID", "dbo.NineTapRegions");
            DropForeignKey("dbo.PlayerHistories", "GameID", "dbo.Games");
            DropForeignKey("dbo.Tournaments", "TourneyRegion", "dbo.NineTapRegions");
            DropForeignKey("dbo.Participants", "ParticipantRegionID", "dbo.NineTapRegions");
            DropForeignKey("dbo.Games", "gameRegionID", "dbo.NineTapRegions");
            DropIndex("dbo.PlayerHistories", new[] { "regionID" });
            DropIndex("dbo.PlayerHistories", new[] { "GameID" });
            DropIndex("dbo.Tournaments", new[] { "TourneyRegion" });
            DropIndex("dbo.Participants", new[] { "ParticipantRegionID" });
            DropIndex("dbo.Games", new[] { "gameRegionID" });
        }
    }
}
