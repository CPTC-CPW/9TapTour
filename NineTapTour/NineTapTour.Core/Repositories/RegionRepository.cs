using Microsoft.EntityFrameworkCore;
using NineTapTour.Core.Data;
using NineTapTour.Core.Entities;

namespace NineTapTour.Core.Repositories;

public class RegionRepository : IRegionRepository
{
    private readonly IDbContextFactory<NineTapDb> dbFactory;

    public RegionRepository(IDbContextFactory<NineTapDb> dbFactory)
    {
        this.dbFactory = dbFactory;
    }

    public List<Region> GetAll()
    {
        using NineTapDb db = dbFactory.CreateDbContext();
        return [.. db.Regions.OrderBy(r => r.Name)];
    }

    public Region? GetById(int id)
    {
        using NineTapDb db = dbFactory.CreateDbContext();
        return db.Regions.SingleOrDefault(r => r.Id == id);
    }

    public Region GetDefault()
    {
        using NineTapDb db = dbFactory.CreateDbContext();
        int defaultId = DefaultRegionResolver.ResolveDefaultRegionId(db);
        return db.Regions.Single(r => r.Id == defaultId);
    }

    public bool NameExists(string name, int? excludeId = null)
    {
        using NineTapDb db = dbFactory.CreateDbContext();
        string trimmed = name.Trim();
        return db.Regions.Any(r => r.Name == trimmed && (excludeId == null || r.Id != excludeId));
    }

    public void Add(Region region)
    {
        using NineTapDb db = dbFactory.CreateDbContext();
        region.Name = region.Name.Trim();
        db.Regions.Add(region);
        db.SaveChanges();
    }

    public void Update(Region region)
    {
        using NineTapDb db = dbFactory.CreateDbContext();
        Region original = db.Regions.Find(region.Id)
            ?? throw new ArgumentException($"Region {region.Id} was not found.", nameof(region));
        original.Name = region.Name.Trim();
        db.SaveChanges();
    }

    public RegionDeleteResult Delete(int id)
    {
        using NineTapDb db = dbFactory.CreateDbContext();
        Region? region = db.Regions.Find(id);
        if (region == null)
        {
            return new RegionDeleteResult(false, "The region was not found.", 0, 0);
        }

        int memberCount = db.Members.Count(m => m.RegionId == id);
        int tournamentCount = db.Tournaments.Count(t => t.RegionId == id);
        if (memberCount > 0 || tournamentCount > 0)
        {
            string reason = $"The region is used by {memberCount} member(s) and {tournamentCount} tournament(s). " +
                "Reassign them before deleting it.";
            return new RegionDeleteResult(false, reason, memberCount, tournamentCount);
        }

        if (db.Regions.Count() == 1)
        {
            return new RegionDeleteResult(false, "The last region cannot be deleted.", 0, 0);
        }

        db.Regions.Remove(region);
        db.SaveChanges();
        return new RegionDeleteResult(true, null, 0, 0);
    }
}
