namespace Member_Import_Test.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ReferralsChange : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Members", "Referrals", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Members", "Referrals", c => c.Int());
        }
    }
}
