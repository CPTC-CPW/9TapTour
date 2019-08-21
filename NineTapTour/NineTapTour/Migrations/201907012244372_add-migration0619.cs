namespace NineTapTour.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addmigration0619 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Members", "NineTapRegionID", "dbo.NineTapRegions");
            DropIndex("dbo.Members", new[] { "NineTapRegionID" });
        }
        
        public override void Down()
        {
            CreateIndex("dbo.Members", "NineTapRegionID");
            AddForeignKey("dbo.Members", "NineTapRegionID", "dbo.NineTapRegions", "NineTapRegionID", cascadeDelete: true);
        }
    }
}
