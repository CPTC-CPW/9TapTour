namespace NineTapTour.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangedDatetoNullableDateTime : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Members", "DateOfBirth", c => c.DateTime());
            AlterColumn("dbo.Members", "JoinDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Members", "JoinDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Members", "DateOfBirth", c => c.DateTime(nullable: false));
        }
    }
}
