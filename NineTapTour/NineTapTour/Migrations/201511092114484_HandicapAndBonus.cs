namespace NineTapTour.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class HandicapAndBonus : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Games", "Handicap", c => c.Int(nullable: false));
            AddColumn("dbo.Games", "Bonus", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Games", "Bonus");
            DropColumn("dbo.Games", "Handicap");
        }
    }
}
