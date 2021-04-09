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

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#if DEBUG
                optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;database=NineTapTour.NineTapDb;trusted_connection=true;MultipleActiveResultSets=true");
#endif

#if !DEBUG
                optionsBuilder.UseSqlServer("Server=.\\SQLExpress;database=NineTapTour.NineTapDb;trusted_connection=true;MultipleActiveResultSets=true");
#endif
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