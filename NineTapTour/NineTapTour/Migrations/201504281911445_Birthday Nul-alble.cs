namespace NineTapTour.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BirthdayNulalble : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Members", "DateOfBirth", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Members", "DateOfBirth", c => c.DateTime(nullable: false));
        }
    }
}
