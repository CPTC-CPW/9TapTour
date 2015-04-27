namespace NineTapTour.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _201504272037146_InitialDatabase : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Members", "Notes", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Members", "Notes");
        }
    }
}
