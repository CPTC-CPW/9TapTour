namespace NineTapTour.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class pending : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tournaments", "Sponsors", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Tournaments", "Sponsors");
        }
    }
}
