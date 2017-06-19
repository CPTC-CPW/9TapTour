namespace Member_Import_Test.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class allownullableforallmemberdatatypes : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Members",
                c => new
                {
                    MemberNumber = c.Int(nullable: false, identity: true),
                    FirstName = c.String(nullable: true),
                    LastName = c.String(nullable: true),
                    MiddleInitial = c.String(nullable: true),
                    DateOfBirth = c.DateTime(nullable: true),
                    Gender = c.Int(nullable: true),
                    IsSenior = c.Boolean(nullable: true),
                    IsActive = c.Boolean(nullable: true),
                    JoinDate = c.DateTime(nullable: true),
                    Referals = c.Int(nullable: true),
                    StreetAddress = c.String(nullable: true),
                    City = c.String(nullable: true),
                    State = c.String(nullable: true),
                    PostalCode = c.String(nullable: true),
                    PrimaryPhone = c.String(nullable: true),
                    SecondaryPhone = c.String(nullable: true),
                })
                .PrimaryKey(t => t.MemberNumber);

        }

        public override void Down()
        {
            DropTable("dbo.Members");
        }
    }
}
