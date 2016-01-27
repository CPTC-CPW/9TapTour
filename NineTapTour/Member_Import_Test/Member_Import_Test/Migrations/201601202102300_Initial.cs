namespace Member_Import_Test.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Members",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Number = c.Int(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        FirstName = c.String(nullable: false),
                        LastName = c.String(nullable: false),
                        MiddleInitial = c.String(),
                        DateOfBirth = c.DateTime(nullable: false),
                        SSN = c.String(nullable: false, maxLength: 11, fixedLength: true, unicode: false),
                        Gender = c.Int(nullable: false),
                        Street = c.String(nullable: false),
                        City = c.String(nullable: false),
                        State = c.String(nullable: false),
                        PostalCode = c.String(nullable: false),
                        Email = c.String(nullable: false),
                        PrimaryPhone = c.String(nullable: false),
                        SecondaryPhone = c.String(),
                        Average = c.Int(),
                        Handicap = c.Int(),
                        Bonus = c.Int(),
                        JoinDate = c.DateTime(nullable: false),
                        RejoinDate = c.DateTime(),
                        LastBowled = c.DateTime(),
                        MoneyEarned = c.Decimal(nullable: false, storeType: "money"),
                        Notes = c.String(),
                        Referrals = c.Int(),
                        IsSenior = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Number, unique: true, name: "IX_MemberNumber")
                .Index(t => t.SSN, unique: true, name: "IX_MemberSSN");
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.Members", "IX_MemberSSN");
            DropIndex("dbo.Members", "IX_MemberNumber");
            DropTable("dbo.Members");
        }
    }
}
