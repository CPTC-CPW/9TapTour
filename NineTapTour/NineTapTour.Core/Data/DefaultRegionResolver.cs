namespace NineTapTour.Core.Data;

/// <summary>
/// Resolves the region applied when a caller does not choose one. The desktop
/// app and the legacy importer have no region UI, so repositories fall back to
/// the first region (the "Default" row seeded by the AddRegions migration).
/// </summary>
internal static class DefaultRegionResolver
{
    public static int ResolveDefaultRegionId(NineTapDb db)
    {
        int? regionId = db.Regions
            .OrderBy(r => r.Id)
            .Select(r => (int?)r.Id)
            .FirstOrDefault();

        return regionId ?? throw new InvalidOperationException(
            "No regions exist. The AddRegions migration seeds a default region; run the database migrations.");
    }
}
