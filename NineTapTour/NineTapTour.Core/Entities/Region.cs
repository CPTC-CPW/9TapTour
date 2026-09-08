using System.ComponentModel.DataAnnotations;

namespace NineTapTour.Core.Entities;

/// <summary>
/// A geographic grouping for members (home region) and tournaments. Regions
/// are descriptive for now; every member and tournament belongs to exactly one.
/// </summary>
public class Region
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    public override string ToString()
    {
        return Name;
    }
}
