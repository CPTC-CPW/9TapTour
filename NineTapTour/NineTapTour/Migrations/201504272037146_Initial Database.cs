namespace NineTapTour.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialDatabase : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Members",
                c => new
                    {
                        MemberNumber = c.Int(nullable: false, identity: true),
                        FirstName = c.String(),
                        LastName = c.String(),
                        MiddleInitial = c.String(),
                        DateOfBirth = c.DateTime(nullable: false),
                        Gender = c.Int(nullable: false),
                        IsSenior = c.Boolean(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        JoinDate = c.DateTime(nullable: false),
                        Referals = c.Int(nullable: false),
                        StreetAddress = c.String(),
                        City = c.String(),
                        State = c.String(),
                        PostalCode = c.String(),
                        PrimaryPhone = c.String(),
                        SecondaryPhone = c.String(),
                    })
                .PrimaryKey(t => t.MemberNumber);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Members");
        }
    }
}
