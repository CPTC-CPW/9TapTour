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
            
            
            builder.Entity<NineTapRegion>().ToTable("NineTapRegions");
            
            
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
                optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=NineTapDb2025;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False");

//#if !DEBUG
//                optionsBuilder.UseSqlServer("Server=.\\SQLExpress;database=NineTapTour.NineTapDb2025;trusted_connection=true;MultipleActiveResultSets=true");
//#endif
            }
        }

        // Add DbSets for each type to store in the database
        public virtual DbSet<Member> Members { get; set; }
        public virtual DbSet<Game> Games { get; set; }
        public virtual DbSet<Tournament> Tournaments { get; set; }
        public virtual DbSet<Participant> Participants { get; set; }
        
        public virtual DbSet<NineTapRegion> NineTapRegion { get; set; }

    }
}