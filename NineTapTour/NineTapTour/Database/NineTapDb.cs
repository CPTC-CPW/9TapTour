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
            
            // Phase 5: FinalizeTemps table removed - no longer needed
            // builder.Entity<FinalizeTemp>().ToTable("FinalizeTemps");
            
            builder.Entity<NineTapRegion>().ToTable("NineTapRegions");
            
            // Phase 3: PlayerHistories table removed - all data now in Games table
            // builder.Entity<PlayerHistory>().ToTable("PlayerHistories");
            
            // Configure one-to-one relationship between Participant and Game
            // Participant is the owner (has foreign key), Game is the dependent
            builder.Entity<Participant>()
                .HasOne(p => p.Game)
                .WithOne(g => g.Participant)
                .HasForeignKey<Participant>("GameId") // Shadow property for foreign key
                .IsRequired(false); // Game can exist without Participant during creation
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
        
        // Phase 5: FinalizeTemp DbSet removed - data now in Games table
        // public virtual DbSet<FinalizeTemp> FinalizeTemp { get; set; }
        
        // Phase 3: PlayerHistory DbSet removed - all data now in Games table
        // All queries now use Games table as single source of truth
        // PlayerHistory class remains as ViewModel for backward compatibility
        // public virtual DbSet<PlayerHistory> PlayerHistory { get; set; }
        
        public virtual DbSet<NineTapRegion> NineTapRegion { get; set; }

    }
}