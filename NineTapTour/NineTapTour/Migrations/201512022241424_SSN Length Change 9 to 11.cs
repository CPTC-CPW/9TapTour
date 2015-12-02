namespace NineTapTour.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SSNLengthChange9to11 : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Members", "IX_MemberSSN");
            AlterColumn("dbo.Members", "SSN", c => c.String(nullable: false, maxLength: 11, fixedLength: true, unicode: false));
            CreateIndex("dbo.Members", "SSN", unique: true, name: "IX_MemberSSN");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Members", "IX_MemberSSN");
            AlterColumn("dbo.Members", "SSN", c => c.String(nullable: false, maxLength: 9, fixedLength: true, unicode: false));
            CreateIndex("dbo.Members", "SSN", unique: true, name: "IX_MemberSSN");
        }
    }
}
