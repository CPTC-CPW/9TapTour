namespace NineTapTour.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class middleinitialnullable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Members", "MiddleInitial", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Members", "MiddleInitial", c => c.String(nullable: false));
        }
    }
}
