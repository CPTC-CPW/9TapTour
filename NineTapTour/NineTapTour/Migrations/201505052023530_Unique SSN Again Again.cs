namespace NineTapTour.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UniqueSSNAgainAgain : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Members", "SSN", c => c.String(maxLength: 9, fixedLength: true, unicode: false));
            CreateIndex("dbo.Members", "SSN", unique: true, name: "IX_MemberSSN");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Members", "IX_MemberSSN");
            AlterColumn("dbo.Members", "SSN", c => c.String());
        }
    }
}
