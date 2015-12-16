namespace NineTapTour.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class nullablehandicapandbonus : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Games", "Handicap", c => c.Int());
            AlterColumn("dbo.Games", "Bonus", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Games", "Bonus", c => c.Int(nullable: false));
            AlterColumn("dbo.Games", "Handicap", c => c.Int(nullable: false));
        }
    }
}
