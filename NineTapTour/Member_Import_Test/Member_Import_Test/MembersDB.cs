namespace Member_Import_Test
{
    using Classes;
    using System;
    using System.Data.Entity;
    using System.Linq;

    public class MembersDB : DbContext
    {
        // Your context has been configured to use a 'Members' connection string from your application's 
        // configuration file (App.config or Web.config). By default, this connection string targets the 
        // 'Member_Import_Test.Members' database on your LocalDb instance. 
        // 
        // If you wish to target a different database and/or database provider, modify the 'Members' 
        // connection string in the application configuration file.
        public MembersDB()
            : base("name=MembersDB")
        {
        }

        // Add a DbSet for each entity type that you want to include in your model. For more information 
        // on configuring and using a Code First model, see http://go.microsoft.com/fwlink/?LinkId=390109.

        // public virtual DbSet<MyEntity> MyEntities { get; set; }
        public DbSet<Member> Members { get; set; }
    }

    //public class MyEntity
    //{
    //    public int Id { get; set; }
    //    public string Name { get; set; }
    //}
}