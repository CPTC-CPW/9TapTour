using System;
using System.ComponentModel.DataAnnotations;

namespace NineTapTour.Models;

/// <summary>
/// Stores a directional partner claim for a doubles tournament squad.
/// Example: SourceMember=A, PartnerMember=B means A listed B as a partner.
/// </summary>
public class DoublesPartnerClaim
{
    [Key]
    public int Id { get; set; }

    [Required]
    public Tournament Tournament { get; set; }

    [Required]
    public Member SourceMember { get; set; }

    [Required]
    public Member PartnerMember { get; set; }

    [Required]
    public int Squad { get; set; }

    [Required]
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
