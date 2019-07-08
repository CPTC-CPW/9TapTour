namespace NineTapTour.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddForeignKeys : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.FinalizeTemps", "TournamentID");
            CreateIndex("dbo.FinalizeTemps", "GameId");
            CreateIndex("dbo.FinalizeTemps", "MemberId");
            CreateIndex("dbo.FinalizeTemps", "FinalizeRegionID");
            CreateIndex("dbo.Games", "gameRegionID");
            CreateIndex("dbo.Tournaments", "TourneyRegion");
            CreateIndex("dbo.Participants", "ParticipantRegionID");
            CreateIndex("dbo.PlayerHistories", "GameID");
            CreateIndex("dbo.PlayerHistories", "regionID");
            AddForeignKey("dbo.Games", "gameRegionID", "dbo.NineTapRegions", "NineTapRegionID", cascadeDelete: true);
            AddForeignKey("dbo.FinalizeTemps", "GameId", "dbo.Games", "Id", cascadeDelete: true);
            AddForeignKey("dbo.FinalizeTemps", "MemberId", "dbo.Members", "Id", cascadeDelete: true);
            AddForeignKey("dbo.FinalizeTemps", "FinalizeRegionID", "dbo.NineTapRegions", "NineTapRegionID", cascadeDelete: true);
            AddForeignKey("dbo.Tournaments", "TourneyRegion", "dbo.NineTapRegions", "NineTapRegionID", cascadeDelete: true);
            AddForeignKey("dbo.Participants", "ParticipantRegionID", "dbo.NineTapRegions", "NineTapRegionID", cascadeDelete: true);
            AddForeignKey("dbo.FinalizeTemps", "TournamentID", "dbo.Tournaments", "Id", cascadeDelete: true);
            AddForeignKey("dbo.PlayerHistories", "GameID", "dbo.Games", "Id", cascadeDelete: true);
            AddForeignKey("dbo.PlayerHistories", "regionID", "dbo.NineTapRegions", "NineTapRegionID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PlayerHistories", "regionID", "dbo.NineTapRegions");
            DropForeignKey("dbo.PlayerHistories", "GameID", "dbo.Games");
            DropForeignKey("dbo.FinalizeTemps", "TournamentID", "dbo.Tournaments");
            DropForeignKey("dbo.Participants", "ParticipantRegionID", "dbo.NineTapRegions");
            DropForeignKey("dbo.Tournaments", "TourneyRegion", "dbo.NineTapRegions");
            DropForeignKey("dbo.FinalizeTemps", "FinalizeRegionID", "dbo.NineTapRegions");
            DropForeignKey("dbo.FinalizeTemps", "MemberId", "dbo.Members");
            DropForeignKey("dbo.FinalizeTemps", "GameId", "dbo.Games");
            DropForeignKey("dbo.Games", "gameRegionID", "dbo.NineTapRegions");
            DropIndex("dbo.PlayerHistories", new[] { "regionID" });
            DropIndex("dbo.PlayerHistories", new[] { "GameID" });
            DropIndex("dbo.Participants", new[] { "ParticipantRegionID" });
            DropIndex("dbo.Tournaments", new[] { "TourneyRegion" });
            DropIndex("dbo.Games", new[] { "gameRegionID" });
            DropIndex("dbo.FinalizeTemps", new[] { "FinalizeRegionID" });
            DropIndex("dbo.FinalizeTemps", new[] { "MemberId" });
            DropIndex("dbo.FinalizeTemps", new[] { "GameId" });
            DropIndex("dbo.FinalizeTemps", new[] { "TournamentID" });
        }
    }
}
