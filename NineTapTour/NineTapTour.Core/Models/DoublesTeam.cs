using System.ComponentModel.DataAnnotations;

namespace NineTapTour.Models;

/// <summary>
/// Represents a doubles pairing within a specific tournament squad.
/// Each record links two Members as a team for a given squad.
/// The same two members can form separate teams in different squads or tournaments,
/// but cannot be paired more than once in the same tournament + squad
/// (the uniqueness check is order-independent: (A,B) == (B,A)).
/// </summary>
public class DoublesTeam
{
    [Key]
    public int Id { get; set; }

    [Required]
    public Tournament Tournament { get; set; }

    [Required]
    public Member Member1 { get; set; }

    [Required]
    public Member Member2 { get; set; }

    /// <summary>The squad number this pairing is entered in.</summary>
    public int Squad { get; set; }
}
