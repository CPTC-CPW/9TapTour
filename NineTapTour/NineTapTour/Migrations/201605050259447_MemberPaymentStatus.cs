namespace NineTapTour.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MemberPaymentStatus : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Members", "LastPayment", c => c.DateTime());
            AddColumn("dbo.Members", "IsLifetimeMember", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Members", "IsLifetimeMember");
            DropColumn("dbo.Members", "LastPayment");
        }
    }
}
