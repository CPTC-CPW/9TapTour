namespace NineTapTour.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class finTempProPotValue : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FinalizeTemps", "ProPotEarnings", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.FinalizeTemps", "ProPotEarnings");
        }
    }
}
