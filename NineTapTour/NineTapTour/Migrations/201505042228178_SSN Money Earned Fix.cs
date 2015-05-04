namespace NineTapTour.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SSNMoneyEarnedFix : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Members", "IX_MemberNumber");
            AlterColumn("dbo.Members", "Notes", c => c.String());
            CreateIndex("dbo.Members", "Number", unique: true, name: "IX_MemberNumber");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Members", "IX_MemberNumber");
            AlterColumn("dbo.Members", "Notes", c => c.String(storeType: "ntext"));
            CreateIndex("dbo.Members", "Number", unique: true, name: "IX_MemberNumber");
        }
    }
}
