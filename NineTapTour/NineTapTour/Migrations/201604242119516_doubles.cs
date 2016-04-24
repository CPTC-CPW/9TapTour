namespace NineTapTour.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class doubles : DbMigration
    {
        public override void Up()
        {
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
            DropIndex("dbo.Squads", new[] { "Game_Id" });
            DropTable("dbo.Squads");
        }
    }
}
