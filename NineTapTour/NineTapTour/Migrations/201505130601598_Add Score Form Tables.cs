namespace NineTapTour.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddScoreFormTables : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Members", "IX_MemberSSN");
            CreateTable(
                "dbo.Games",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Number = c.Int(nullable: false),
                        Score = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Participants",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SquadNumber = c.Int(nullable: false),
                        Game_Id = c.Int(nullable: false),
                        Member_Id = c.Int(nullable: false),
                        Tournament_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Games", t => t.Game_Id, cascadeDelete: true)
                .ForeignKey("dbo.Members", t => t.Member_Id, cascadeDelete: true)
                .ForeignKey("dbo.Tournaments", t => t.Tournament_Id, cascadeDelete: true)
                .Index(t => t.Game_Id)
                .Index(t => t.Member_Id)
                .Index(t => t.Tournament_Id);
            
            CreateTable(
                "dbo.Tournaments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false),
                        Location = c.String(nullable: false),
                        Event = c.String(),
                        Notes = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AlterColumn("dbo.Members", "FirstName", c => c.String(nullable: false));
            AlterColumn("dbo.Members", "LastName", c => c.String(nullable: false));
            AlterColumn("dbo.Members", "MiddleInitial", c => c.String(nullable: false));
            AlterColumn("dbo.Members", "SSN", c => c.String(nullable: false, maxLength: 9, fixedLength: true, unicode: false));
            AlterColumn("dbo.Members", "Street", c => c.String(nullable: false));
            AlterColumn("dbo.Members", "City", c => c.String(nullable: false));
            AlterColumn("dbo.Members", "State", c => c.String(nullable: false));
            AlterColumn("dbo.Members", "PostalCode", c => c.String(nullable: false));
            AlterColumn("dbo.Members", "Email", c => c.String(nullable: false));
            AlterColumn("dbo.Members", "PrimaryPhone", c => c.String(nullable: false));
            CreateIndex("dbo.Members", "SSN", unique: true, name: "IX_MemberSSN");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Participants", "Tournament_Id", "dbo.Tournaments");
            DropForeignKey("dbo.Participants", "Member_Id", "dbo.Members");
            DropForeignKey("dbo.Participants", "Game_Id", "dbo.Games");
            DropIndex("dbo.Participants", new[] { "Tournament_Id" });
            DropIndex("dbo.Participants", new[] { "Member_Id" });
            DropIndex("dbo.Participants", new[] { "Game_Id" });
            DropIndex("dbo.Members", "IX_MemberSSN");
            AlterColumn("dbo.Members", "PrimaryPhone", c => c.String());
            AlterColumn("dbo.Members", "Email", c => c.String());
            AlterColumn("dbo.Members", "PostalCode", c => c.String());
            AlterColumn("dbo.Members", "State", c => c.String());
            AlterColumn("dbo.Members", "City", c => c.String());
            AlterColumn("dbo.Members", "Street", c => c.String());
            AlterColumn("dbo.Members", "SSN", c => c.String(maxLength: 9, fixedLength: true, unicode: false));
            AlterColumn("dbo.Members", "MiddleInitial", c => c.String());
            AlterColumn("dbo.Members", "LastName", c => c.String());
            AlterColumn("dbo.Members", "FirstName", c => c.String());
            DropTable("dbo.Tournaments");
            DropTable("dbo.Participants");
            DropTable("dbo.Games");
            CreateIndex("dbo.Members", "SSN", unique: true, name: "IX_MemberSSN");
        }
    }
}
