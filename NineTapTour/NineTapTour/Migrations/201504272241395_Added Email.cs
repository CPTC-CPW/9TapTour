namespace NineTapTour.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedEmail : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Members", "Email", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Members", "Email");
        }
    }
}
