using System.ComponentModel.DataAnnotations;

namespace NineTapTour.Models;

/// <summary>
/// Represents a doubles pairing within a specific tournament.
/// Each record links two Members as a team.
/// The same two members can form separate teams in different tournaments,
/// but cannot be paired more than once in the same tournament
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
}
