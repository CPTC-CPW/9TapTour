using System;
using System.ComponentModel.DataAnnotations;

namespace NineTapTour.Models;

/// <summary>
/// Stores the planned number of partners for a bowler in a doubles tournament squad.
/// </summary>
public class DoublesPartnerPlan
{
    [Key]
    public int Id { get; set; }

    [Required]
    public Tournament Tournament { get; set; }

    [Required]
    public Member Member { get; set; }

    [Required]
    public int Squad { get; set; }

    [Required]
    public int ExpectedPartnerCount { get; set; }

    [Required]
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
}
