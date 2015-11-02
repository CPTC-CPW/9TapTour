namespace NineTapTour.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class gametable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Games", "Game1", c => c.Int(nullable: false));
            AddColumn("dbo.Games", "Game2", c => c.Int(nullable: false));
            AddColumn("dbo.Games", "Game3", c => c.Int(nullable: false));
            AddColumn("dbo.Games", "Game4", c => c.Int(nullable: false));
            AddColumn("dbo.Games", "Member_Id", c => c.Int(nullable: false));
            CreateIndex("dbo.Games", "Member_Id");
            AddForeignKey("dbo.Games", "Member_Id", "dbo.Members", "Id", cascadeDelete: false);
            DropColumn("dbo.Games", "Number");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Games", "Number", c => c.Int(nullable: false));
            DropForeignKey("dbo.Games", "Member_Id", "dbo.Members");
            DropIndex("dbo.Games", new[] { "Member_Id" });
            DropColumn("dbo.Games", "Member_Id");
            DropColumn("dbo.Games", "Game4");
            DropColumn("dbo.Games", "Game3");
            DropColumn("dbo.Games", "Game2");
            DropColumn("dbo.Games", "Game1");
        }
    }
}
