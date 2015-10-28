namespace NineTapTour.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Addedlistofscores : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Games", "Score");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Games", "Score", c => c.Int(nullable: false));
        }
    }
}
