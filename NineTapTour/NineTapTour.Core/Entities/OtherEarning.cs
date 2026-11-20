#nullable disable
using System;
using System.ComponentModel.DataAnnotations;

namespace NineTapTour.Core.Entities;

/// <summary>
/// Cash a member won that is not tied to a tour stop entry: career totals from
/// before the legacy books started, satellites, Yakima, 3RT, Hawaii and other
/// outside events. The legacy books count these in a bowler's lifetime cash, so
/// the earnings reports can add them in, but they never count as entries.
/// </summary>
public class OtherEarning
{
    public const int DescriptionMaxLength = 200;
    public const int SourceMaxLength = 300;

    [Key]
    public int Id { get; set; }

    [Required]
    public Member Member { get; set; }

    public decimal Amount { get; set; }

    /// <summary>
    /// What the money was for, e.g. "10th Yak" or "2003".
    /// </summary>
    [MaxLength(DescriptionMaxLength)]
    public string Description { get; set; }

    /// <summary>
    /// When the money was won, or null when the book does not say.
    /// </summary>
    public DateTime? Date { get; set; }

    /// <summary>
    /// True for rows created by the legacy history import. A re-run of the import
    /// replaces a book's imported rows; rows added by hand are never touched.
    /// </summary>
    public bool IsImported { get; set; }

    /// <summary>
    /// Where an imported row came from: book file, sheet and row.
    /// </summary>
    [MaxLength(SourceMaxLength)]
    public string Source { get; set; }
}
