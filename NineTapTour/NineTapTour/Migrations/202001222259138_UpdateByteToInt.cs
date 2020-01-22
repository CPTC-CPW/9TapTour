namespace NineTapTour.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateByteToInt : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Games", "PlaceStanding", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Games", "PlaceStanding", c => c.Byte());
        }
    }
}
