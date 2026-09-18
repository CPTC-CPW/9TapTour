#nullable disable
using Microsoft.EntityFrameworkCore;
using NineTapTour.Core.Entities;

namespace NineTapTour.Core.Data;

public class NineTapDb : DbContext
{
    public NineTapDb(DbContextOptions<NineTapDb> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        // Regions: every member has a home region and every tournament belongs
        // to a region. Restrict deletes so a region in use cannot be removed.
        builder.Entity<Region>()
            .Property(r => r.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Entity<Region>()
            .HasIndex(r => r.Name)
            .IsUnique();

        builder.Entity<Member>()
            .HasOne(m => m.Region)
            .WithMany()
            .HasForeignKey(m => m.RegionId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Tournament>()
            .HasOne(t => t.Region)
            .WithMany()
            .HasForeignKey(t => t.RegionId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

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

        builder.Entity<DoublesPartnerPlan>()
            .HasOne(p => p.Member)
            .WithMany()
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<DoublesPartnerPlan>()
            .HasOne(p => p.Tournament)
            .WithMany()
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<DoublesPartnerPlan>()
            .HasIndex("TournamentId", "MemberId", "Squad")
            .IsUnique();

        builder.Entity<DoublesPartnerClaim>()
            .HasOne(c => c.SourceMember)
            .WithMany()
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<DoublesPartnerClaim>()
            .HasOne(c => c.PartnerMember)
            .WithMany()
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<DoublesPartnerClaim>()
            .HasOne(c => c.Tournament)
            .WithMany()
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<DoublesPartnerClaim>()
            .HasIndex("TournamentId", "SourceMemberId", "PartnerMemberId", "Squad")
            .IsUnique();
    }

    // Add DbSets for each type to store in the database
    public virtual DbSet<Region> Regions { get; set; }
    public virtual DbSet<Member> Members { get; set; }
    public virtual DbSet<Game> Games { get; set; }
    public virtual DbSet<Tournament> Tournaments { get; set; }
    public virtual DbSet<Participant> Participants { get; set; }
    public virtual DbSet<DoublesTeam> DoublesTeams { get; set; }
    public virtual DbSet<DoublesPartnerPlan> DoublesPartnerPlans { get; set; }
    public virtual DbSet<DoublesPartnerClaim> DoublesPartnerClaims { get; set; }
}