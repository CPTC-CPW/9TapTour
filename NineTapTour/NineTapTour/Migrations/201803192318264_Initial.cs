namespace NineTapTour.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.FinalizeTemps",
                c => new
                    {
                        FinalizeID = c.Int(nullable: false, identity: true),
                        TournamentID = c.Int(nullable: false),
                        GameId = c.Int(nullable: false),
                        MemberId = c.Int(nullable: false),
                        memberNumber = c.Int(nullable: false),
                        FirstName = c.String(),
                        LastName = c.String(),
                        Squad = c.Int(nullable: false),
                        Game1 = c.Int(nullable: false),
                        Game2 = c.Int(nullable: false),
                        Game3 = c.Int(nullable: false),
                        Game4 = c.Int(nullable: false),
                        UseGame1 = c.Boolean(nullable: false),
                        UseGame2 = c.Boolean(nullable: false),
                        UseGame3 = c.Boolean(nullable: false),
                        UseGame4 = c.Boolean(nullable: false),
                        LeagueAverage = c.Int(nullable: false),
                        AdjustedAvg = c.Int(nullable: false),
                        Notes = c.String(),
                        ScratchTotal = c.Int(nullable: false),
                        KeepAdjustedAvg = c.Boolean(nullable: false),
                        GameAvg = c.Int(nullable: false),
                        Handicap = c.Int(nullable: false),
                        Bonus = c.Int(nullable: false),
                        HandicapTotal = c.Int(nullable: false),
                        FinalizeRegionID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.FinalizeID);
            
            CreateTable(
                "dbo.Games",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        InputtedAvg = c.Int(),
                        Game1 = c.Int(),
                        Game2 = c.Int(),
                        Game3 = c.Int(),
                        Game4 = c.Int(),
                        UseGame1 = c.Boolean(),
                        UseGame2 = c.Boolean(),
                        UseGame3 = c.Boolean(),
                        UseGame4 = c.Boolean(),
                        Notes = c.String(),
                        Handicap = c.Int(),
                        Bonus = c.Int(),
                        MoneyWon = c.Decimal(precision: 18, scale: 2),
                        PlaceStanding = c.Byte(),
                        gameRegionID = c.Int(nullable: false),
                        IsComp = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Members",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Number = c.Int(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        FirstName = c.String(),
                        LastName = c.String(),
                        MiddleInitial = c.String(),
                        DateOfBirth = c.DateTime(),
                        SSN = c.String(maxLength: 11, fixedLength: true, unicode: false),
                        Gender = c.Int(nullable: false),
                        Street = c.String(),
                        City = c.String(),
                        State = c.String(),
                        PostalCode = c.String(),
                        Email = c.String(),
                        PrimaryPhone = c.String(),
                        SecondaryPhone = c.String(),
                        Average = c.Int(),
                        StartAvg = c.Int(),
                        Handicap = c.Int(),
                        Bonus = c.Int(nullable: false),
                        JoinDate = c.DateTime(),
                        RejoinDate = c.DateTime(),
                        LastBowled = c.DateTime(),
                        LastPayment = c.DateTime(),
                        IsLifetimeMember = c.Boolean(nullable: false),
                        Notes = c.String(),
                        Referrals = c.Int(),
                        IsSenior = c.Boolean(nullable: false),
                        MoneyEarned = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NineTapRegionID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.SSN, name: "IX_MemberSSN");
            
            CreateTable(
                "dbo.NineTapRegions",
                c => new
                    {
                        NineTapRegionID = c.Int(nullable: false, identity: true),
                        NineTapRegionName = c.String(),
                    })
                .PrimaryKey(t => t.NineTapRegionID);
            
            CreateTable(
                "dbo.Participants",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SquadNumber = c.Int(nullable: false),
                        ParticipantRegionID = c.Int(nullable: false),
                        Game_Id = c.Int(),
                        Member_Id = c.Int(nullable: false),
                        Tournament_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Games", t => t.Game_Id)
                .ForeignKey("dbo.Members", t => t.Member_Id, cascadeDelete: true)
                .ForeignKey("dbo.Tournaments", t => t.Tournament_Id)
                .Index(t => t.Game_Id)
                .Index(t => t.Member_Id)
                .Index(t => t.Tournament_Id);
            
            CreateTable(
                "dbo.Tournaments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false),
                        Location = c.String(nullable: false),
                        Event = c.String(),
                        Notes = c.String(),
                        Sponsors = c.String(),
                        Squads = c.Int(nullable: false),
                        Doubles = c.Boolean(nullable: false),
                        ThreeOutOf4 = c.Boolean(nullable: false),
                        TourneyRegion = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
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
            
            CreateTable(
                "dbo.Squads",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Number = c.Int(nullable: false),
                        Game_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Games", t => t.Game_Id)
                .Index(t => t.Game_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Squads", "Game_Id", "dbo.Games");
            DropForeignKey("dbo.Participants", "Tournament_Id", "dbo.Tournaments");
            DropForeignKey("dbo.Participants", "Member_Id", "dbo.Members");
            DropForeignKey("dbo.Participants", "Game_Id", "dbo.Games");
            DropIndex("dbo.Squads", new[] { "Game_Id" });
            DropIndex("dbo.PlayerHistories", new[] { "MemberNumber" });
            DropIndex("dbo.Participants", new[] { "Tournament_Id" });
            DropIndex("dbo.Participants", new[] { "Member_Id" });
            DropIndex("dbo.Participants", new[] { "Game_Id" });
            DropIndex("dbo.Members", "IX_MemberSSN");
            DropTable("dbo.Squads");
            DropTable("dbo.PlayerHistories");
            DropTable("dbo.Tournaments");
            DropTable("dbo.Participants");
            DropTable("dbo.NineTapRegions");
            DropTable("dbo.Members");
            DropTable("dbo.Games");
            DropTable("dbo.FinalizeTemps");
        }
    }
}
