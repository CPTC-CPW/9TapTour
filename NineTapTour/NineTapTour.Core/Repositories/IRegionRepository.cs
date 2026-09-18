using NineTapTour.Core.Entities;

namespace NineTapTour.Core.Repositories;

/// <summary>
/// Outcome of a region delete attempt. Regions referenced by members or
/// tournaments, and the last remaining region, cannot be deleted.
/// </summary>
public sealed record RegionDeleteResult(bool Deleted, string? Reason, int MemberCount, int TournamentCount);

/// <summary>
/// Data access for regions.
/// </summary>
public interface IRegionRepository
{
    /// <summary>All regions ordered by name.</summary>
    List<Region> GetAll();

    Region? GetById(int id);

    /// <summary>The region applied when none is chosen (lowest Id).</summary>
    Region GetDefault();

    bool NameExists(string name, int? excludeId = null);

    void Add(Region region);

    void Update(Region region);

    RegionDeleteResult Delete(int id);
}
