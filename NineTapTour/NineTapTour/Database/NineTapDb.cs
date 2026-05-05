using Microsoft.EntityFrameworkCore;
using NineTapTour.Models;

namespace NineTapTour.Database;

public class NineTapDb : DbContext
{
    public NineTapDb() { }

    public NineTapDb(DbContextOptions<NineTapDb> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        // Configure one-to-one relationship between Participant and Game
        // Participant is the owner (has foreign key), Game is the dependent
        builder.Entity<Participant>()
            .HasOne(p => p.Game)
            .WithOne(g => g.Participant)
            .HasForeignKey<Participant>("GameId") // Shadow property for foreign key
            .IsRequired(false); // Game can exist without Participant during creation

        // DoublesTeam has two FK references to Members on the same table,
        // which would create multiple cascade paths in SQL Server.
        // Use ClientSetNull (no DB-level cascade) to avoid the error.
        builder.Entity<DoublesTeam>()
            .HasOne(dt => dt.Member1)
            .WithMany()
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<DoublesTeam>()
            .HasOne(dt => dt.Member2)
            .WithMany()
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<DoublesTeam>()
            .HasOne(dt => dt.Tournament)
            .WithMany()
            .OnDelete(DeleteBehavior.Cascade);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=NineTapDb2025;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False");
        }
    }

    // Add DbSets for each type to store in the database
    public virtual DbSet<Member> Members { get; set; }
    public virtual DbSet<Game> Games { get; set; }
    public virtual DbSet<Tournament> Tournaments { get; set; }
    public virtual DbSet<Participant> Participants { get; set; }
    public virtual DbSet<DoublesTeam> DoublesTeams { get; set; }
}