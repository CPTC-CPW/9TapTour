namespace NineTapTour.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddMissingColumns : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.Members");
            DropColumn("dbo.Members", "MemberNumber");
            DropColumn("dbo.Members", "Referals");
            DropColumn("dbo.Members", "StreetAddress");
            AddColumn("dbo.Members", "Id", c => c.Int(nullable: false, identity: true));
            AddColumn("dbo.Members", "Number", c => c.Int(nullable: false));
            AddColumn("dbo.Members", "SSN", c => c.String());
            AddColumn("dbo.Members", "Street", c => c.String());
            AddColumn("dbo.Members", "Average", c => c.Int());
            AddColumn("dbo.Members", "Handicap", c => c.Int());
            AddColumn("dbo.Members", "Bonus", c => c.Int());
            AddColumn("dbo.Members", "RejoinDate", c => c.DateTime());
            AddColumn("dbo.Members", "LastBowled", c => c.DateTime());
            AddColumn("dbo.Members", "MoneyEarned", c => c.Decimal(nullable: false, storeType: "money"));
            AddColumn("dbo.Members", "Referrals", c => c.Int());
            AlterColumn("dbo.Members", "DateOfBirth", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Members", "Notes", c => c.String(storeType: "ntext"));
            AddPrimaryKey("dbo.Members", "Id");
            CreateIndex("dbo.Members", "Number", unique: true, name: "IX_MemberNumber");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Members", "IX_MemberNumber");
            DropPrimaryKey("dbo.Members");
            AlterColumn("dbo.Members", "Notes", c => c.String());
            AlterColumn("dbo.Members", "DateOfBirth", c => c.DateTime());
            DropColumn("dbo.Members", "Referrals");
            DropColumn("dbo.Members", "MoneyEarned");
            DropColumn("dbo.Members", "LastBowled");
            DropColumn("dbo.Members", "RejoinDate");
            DropColumn("dbo.Members", "Bonus");
            DropColumn("dbo.Members", "Handicap");
            DropColumn("dbo.Members", "Average");
            DropColumn("dbo.Members", "Street");
            DropColumn("dbo.Members", "SSN");
            DropColumn("dbo.Members", "Number");
            DropColumn("dbo.Members", "Id");
            AddColumn("dbo.Members", "StreetAddress", c => c.String());
            AddColumn("dbo.Members", "Referals", c => c.Int(nullable: false));
            AddColumn("dbo.Members", "MemberNumber", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.Members", "MemberNumber");
        }
    }
}
