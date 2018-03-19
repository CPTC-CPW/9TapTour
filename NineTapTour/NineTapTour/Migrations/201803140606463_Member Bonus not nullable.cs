namespace NineTapTour.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MemberBonusnotnullable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Members", "Bonus", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Members", "Bonus", c => c.Int());
        }
    }
}
