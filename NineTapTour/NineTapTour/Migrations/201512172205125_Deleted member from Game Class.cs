namespace NineTapTour.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DeletedmemberfromGameClass : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Games", "Member_Id", "dbo.Members");
            DropIndex("dbo.Games", new[] { "Member_Id" });
            DropColumn("dbo.Games", "Member_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Games", "Member_Id", c => c.Int(nullable: false));
            CreateIndex("dbo.Games", "Member_Id");
            AddForeignKey("dbo.Games", "Member_Id", "dbo.Members", "Id", cascadeDelete: true);
        }
    }
}
