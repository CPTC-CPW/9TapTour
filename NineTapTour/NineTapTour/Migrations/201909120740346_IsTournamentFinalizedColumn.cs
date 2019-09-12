namespace NineTapTour.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IsTournamentFinalizedColumn : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tournaments", "IsTournamentFinalized", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Tournaments", "IsTournamentFinalized");
        }
    }
}
