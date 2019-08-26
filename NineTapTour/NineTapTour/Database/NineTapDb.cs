using System.Data.Entity;
using NineTapTour.Models;

namespace NineTapTour.Database
{
    public class NineTapDb : DbContext
    {
        // Your context has been configured to use a 'NineTapDb' connection string from your application's 
        // configuration file (App.config or Web.config). By default, this connection string targets the 
        // 'NineTapTour.NineTapDb' database on your LocalDb instance. 
        // 
        // If you wish to target a different database and/or database provider, modify the 'NineTapDb' 
        // connection string in the application configuration file.
        public NineTapDb()
            : base("name=NineTapDbConnection")
        {
        }

        // Add a DbSet for each entity type that you want to include in your model. For more information 
        // on configuring and using a Code First model, see http://go.microsoft.com/fwlink/?LinkId=390109.

        // public virtual DbSet<MyEntity> MyEntities { get; set; }

        public virtual DbSet<Member> Members { get; set; }
        public virtual DbSet<Game> Games { get; set; }
        public virtual DbSet<Tournament> Tournaments { get; set; }
        public virtual DbSet<Participant> Participants { get; set; }
        //public virtual DbSet<Squad> Squads { get; set; }
        public virtual DbSet<FinalizeTemp> FinalizeTemp { get; set; }
        public virtual DbSet<PlayerHistory> PlayerHistory { get; set; }
        public virtual DbSet<NineTapRegion> NineTapRegion { get; set; }

    }

    //public class MyEntity
    //{
    //    public int Id { get; set; }
    //    public string Name { get; set; }
    //}
}