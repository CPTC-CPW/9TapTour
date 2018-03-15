namespace NineTapTour.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class Addedregiontoclasses : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Members", "IX_MemberNumber");
            CreateTable(
                "dbo.NineTapRegions",
                c => new
                {
                    NineTapRegionID = c.Int(nullable: false, identity: true),
                    NineTapRegionName = c.String(),
                })
                .PrimaryKey(t => t.NineTapRegionID);

            CreateTable(
                "dbo.PlayerHistories",
                c => new
                {
                    hisID = c.Int(nullable: false, identity: true),
                    MemberNumber = c.Int(nullable: false),
                    GamesPlayed = c.Int(nullable: false),
                    TournamentDate = c.DateTime(nullable: false),
                    GameID = c.Int(nullable: false),
                    Game1 = c.Int(nullable: false),
                    Game2 = c.Int(nullable: false),
                    Game3 = c.Int(nullable: false),
                    Game4 = c.Int(nullable: false),
                    TotalScore = c.Int(nullable: false),
                    HandiCap = c.Int(nullable: false),
                    Bonus = c.Int(nullable: false),
                    MoneyWon = c.Decimal(nullable: false, precision: 18, scale: 2),
                    Notes = c.String(),
                    AverageForGame = c.Double(nullable: false),
                    trueAVG = c.Double(nullable: false),
                    AVG = c.Int(nullable: false),
                    ProPot = c.String(),
                    PPHG = c.String(),
                    regionID = c.Int(nullable: false),
                })
                .PrimaryKey(t => t.hisID)
                .Index(t => t.MemberNumber);

            AddColumn("dbo.FinalizeTemps", "memberNumber", c => c.Int(nullable: false));
            AddColumn("dbo.FinalizeTemps", "LeagueAverage", c => c.Int(nullable: false));
            AddColumn("dbo.FinalizeTemps", "AdjustedAvg", c => c.Int(nullable: false));
            AddColumn("dbo.FinalizeTemps", "HandicapTotal", c => c.Int(nullable: false));
            AddColumn("dbo.FinalizeTemps", "FinalizeRegionID", c => c.Int(nullable: false));
            AddColumn("dbo.Games", "gameRegionID", c => c.Int(nullable: false));
            AddColumn("dbo.Members", "MoneyEarned", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Members", "NineTapRegionID", c => c.Int(nullable: false));
            AddColumn("dbo.Participants", "ParticipantRegionID", c => c.Int(nullable: false));
            AddColumn("dbo.Tournaments", "TourneyRegion", c => c.Int(nullable: false));
            AlterColumn("dbo.Games", "MoneyWon", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.Games", "PlaceStanding", c => c.Byte());
            AlterColumn("dbo.Members", "FirstName", c => c.String());
            AlterColumn("dbo.Members", "LastName", c => c.String());
            AlterColumn("dbo.Members", "Street", c => c.String());
            AlterColumn("dbo.Members", "City", c => c.String());
            AlterColumn("dbo.Members", "State", c => c.String());
            AlterColumn("dbo.Members", "PostalCode", c => c.String());
            AlterColumn("dbo.Members", "PrimaryPhone", c => c.String());
        }

        public override void Down()
        {
            DropIndex("dbo.PlayerHistories", new[] { "MemberNumber" });
            AlterColumn("dbo.Members", "PrimaryPhone", c => c.String(nullable: false));
            AlterColumn("dbo.Members", "PostalCode", c => c.String(nullable: false));
            AlterColumn("dbo.Members", "State", c => c.String(nullable: false));
            AlterColumn("dbo.Members", "City", c => c.String(nullable: false));
            AlterColumn("dbo.Members", "Street", c => c.String(nullable: false));
            AlterColumn("dbo.Members", "LastName", c => c.String(nullable: false));
            AlterColumn("dbo.Members", "FirstName", c => c.String(nullable: false));
            AlterColumn("dbo.Games", "PlaceStanding", c => c.Byte(nullable: false));
            AlterColumn("dbo.Games", "MoneyWon", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("dbo.Tournaments", "TourneyRegion");
            DropColumn("dbo.Participants", "ParticipantRegionID");
            DropColumn("dbo.Members", "NineTapRegionID");
            DropColumn("dbo.Members", "MoneyEarned");
            DropColumn("dbo.Games", "gameRegionID");
            DropColumn("dbo.FinalizeTemps", "FinalizeRegionID");
            DropColumn("dbo.FinalizeTemps", "HandicapTotal");
            DropColumn("dbo.FinalizeTemps", "AdjustedAvg");
            DropColumn("dbo.FinalizeTemps", "LeagueAverage");
            DropColumn("dbo.FinalizeTemps", "memberNumber");
            DropTable("dbo.PlayerHistories");
            DropTable("dbo.NineTapRegions");
            CreateIndex("dbo.Members", "Number", unique: true, name: "IX_MemberNumber");
        }
    }
}
