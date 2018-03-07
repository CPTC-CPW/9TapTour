namespace NineTapTour.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedIsComptoGametable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Games", "IsComp", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Games", "IsComp");
        }
    }
}
