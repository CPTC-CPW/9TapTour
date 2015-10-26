namespace NineTapTour.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Tournamentisnullable : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Participants", "Tournament_Id", "dbo.Tournaments");
            DropIndex("dbo.Participants", new[] { "Tournament_Id" });
            AlterColumn("dbo.Participants", "Tournament_Id", c => c.Int());
            CreateIndex("dbo.Participants", "Tournament_Id");
            AddForeignKey("dbo.Participants", "Tournament_Id", "dbo.Tournaments", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Participants", "Tournament_Id", "dbo.Tournaments");
            DropIndex("dbo.Participants", new[] { "Tournament_Id" });
            AlterColumn("dbo.Participants", "Tournament_Id", c => c.Int(nullable: false));
            CreateIndex("dbo.Participants", "Tournament_Id");
            AddForeignKey("dbo.Participants", "Tournament_Id", "dbo.Tournaments", "Id", cascadeDelete: true);
        }
    }
}
