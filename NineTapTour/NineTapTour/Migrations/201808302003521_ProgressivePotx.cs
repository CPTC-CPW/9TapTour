namespace NineTapTour.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProgressivePotx : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Games", "SidePot", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Games", "SidePot");
        }
    }
}
