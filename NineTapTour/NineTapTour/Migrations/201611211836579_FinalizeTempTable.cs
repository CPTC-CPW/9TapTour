namespace NineTapTour.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FinalizeTempTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.FinalizeTemps",
                c => new
                    {
                        GameId = c.Int(nullable: false, identity: true),
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
                .PrimaryKey(t => t.GameId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.FinalizeTemps");
        }
    }
}
