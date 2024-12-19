using Microsoft.EntityFrameworkCore;
using NineTapTour.Models;

namespace NineTapTour.Database
{
    public class NineTapDb : DbContext
    {
        public NineTapDb() { }

        public NineTapDb(DbContextOptions<NineTapDb> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Modify generated table names to match old EF6 database for script compatibility
            builder.Entity<FinalizeTemp>().ToTable("FinalizeTemps");
            builder.Entity<NineTapRegion>().ToTable("NineTapRegions");
            builder.Entity<PlayerHistory>().ToTable("PlayerHistories");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=NineTapDb2021;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False");

//#if !DEBUG
//                optionsBuilder.UseSqlServer("Server=.\\SQLExpress;database=NineTapTour.NineTapDb;trusted_connection=true;MultipleActiveResultSets=true");
//#endif
            }
        }

        // Add DbSets for each type to store in the database
        public virtual DbSet<Member> Members { get; set; }
        public virtual DbSet<Game> Games { get; set; }
        public virtual DbSet<Tournament> Tournaments { get; set; }
        public virtual DbSet<Participant> Participants { get; set; }
        public virtual DbSet<Squad> Squads { get; set; }
        public virtual DbSet<FinalizeTemp> FinalizeTemp { get; set; }
        public virtual DbSet<PlayerHistory> PlayerHistory { get; set; }
        public virtual DbSet<NineTapRegion> NineTapRegion { get; set; }

    }
}