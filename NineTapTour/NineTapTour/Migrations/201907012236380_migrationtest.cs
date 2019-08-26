namespace NineTapTour.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class migrationtest : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.Members", "NineTapRegionID");
            AddForeignKey("dbo.Members", "NineTapRegionID", "dbo.NineTapRegions", "NineTapRegionID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Members", "NineTapRegionID", "dbo.NineTapRegions");
            DropIndex("dbo.Members", new[] { "NineTapRegionID" });
        }
    }
}
