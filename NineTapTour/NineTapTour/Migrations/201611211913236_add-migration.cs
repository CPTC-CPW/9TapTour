namespace NineTapTour.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addmigration : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Members", "IX_MemberSSN");
            CreateTable(
                "dbo.FinalizeTemps",
                c => new
                    {
                        FinalizeID = c.Int(nullable: false, identity: true),
                        TournamentID = c.Int(nullable: false),
                        GameId = c.Int(nullable: false),
                        MemberId = c.Int(nullable: false),
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
                        Notes = c.String(),
                        ScratchTotal = c.Int(nullable: false),
                        KeepAdjustedAvg = c.Boolean(nullable: false),
                        GameAvg = c.Int(nullable: false),
                        Handicap = c.Int(nullable: false),
                        Bonus = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.FinalizeID);
            
            AddColumn("dbo.Games", "InputtedAvg", c => c.Int());
            AddColumn("dbo.Games", "UseGame1", c => c.Boolean());
            AddColumn("dbo.Games", "UseGame2", c => c.Boolean());
            AddColumn("dbo.Games", "UseGame3", c => c.Boolean());
            AddColumn("dbo.Games", "UseGame4", c => c.Boolean());
            AddColumn("dbo.Games", "Notes", c => c.String());
            AddColumn("dbo.Games", "MoneyWon", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Games", "PlaceStanding", c => c.Byte(nullable: false));
            AddColumn("dbo.Members", "StartAvg", c => c.Int());
            AddColumn("dbo.Tournaments", "Squads", c => c.Int(nullable: false));
            AlterColumn("dbo.Members", "Email", c => c.String());
            CreateIndex("dbo.Members", "SSN", name: "IX_MemberSSN");
            DropColumn("dbo.Members", "MoneyEarned");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Members", "MoneyEarned", c => c.Decimal(nullable: false, storeType: "money"));
            DropIndex("dbo.Members", "IX_MemberSSN");
            AlterColumn("dbo.Members", "Email", c => c.String(nullable: false));
            DropColumn("dbo.Tournaments", "Squads");
            DropColumn("dbo.Members", "StartAvg");
            DropColumn("dbo.Games", "PlaceStanding");
            DropColumn("dbo.Games", "MoneyWon");
            DropColumn("dbo.Games", "Notes");
            DropColumn("dbo.Games", "UseGame4");
            DropColumn("dbo.Games", "UseGame3");
            DropColumn("dbo.Games", "UseGame2");
            DropColumn("dbo.Games", "UseGame1");
            DropColumn("dbo.Games", "InputtedAvg");
            DropTable("dbo.FinalizeTemps");
            CreateIndex("dbo.Members", "SSN", unique: true, name: "IX_MemberSSN");
        }
    }
}
