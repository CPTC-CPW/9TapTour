#nullable disable
using Microsoft.EntityFrameworkCore;
using NineTapTour.Core.Data;
using NineTapTour.Core.Entities;
using System.Collections.Generic;
using System.Linq;

namespace NineTapTour.Core.Repositories;

public class DoublesTeamRepository : IDoublesTeamRepository
{
    private readonly IDbContextFactory<NineTapDb> dbFactory;

    public DoublesTeamRepository(IDbContextFactory<NineTapDb> dbFactory)
    {
        this.dbFactory = dbFactory;
    }

    /// <summary>
    /// Returns all DoublesTeam records for the given tournament,
    /// with Member1 and Member2 navigation properties loaded.
    /// </summary>
    public List<DoublesTeam> GetTeamsByTournament(int tournamentId)
    {
        using var db = dbFactory.CreateDbContext();
        return [.. db.DoublesTeams
            .Include(dt => dt.Member1)
            .Include(dt => dt.Member2)
            .Include(dt => dt.Tournament)
            .Where(dt => dt.Tournament.Id == tournamentId)
            .OrderBy(dt => dt.Id)];
    }

    /// <summary>
    /// Returns true if a pairing between memberId1 and memberId2 already exists in
    /// the tournament for the given squad (order-independent: (A,B) == (B,A)).
    /// </summary>
    public bool TeamExists(int tournamentId, int memberId1, int memberId2, int squad)
    {
        using var db = dbFactory.CreateDbContext();
        return db.DoublesTeams.Any(dt =>
            dt.Tournament.Id == tournamentId &&
            dt.Squad == squad &&
            ((dt.Member1.Id == memberId1 && dt.Member2.Id == memberId2) ||
             (dt.Member1.Id == memberId2 && dt.Member2.Id == memberId1)));
    }

    /// <summary>
    /// Creates a new doubles pairing for the tournament and squad.
    /// Returns false (without saving) if the pairing already exists in that squad or if
    /// both member IDs are the same.
    /// </summary>
    public bool AddTeam(int tournamentId, int memberId1, int memberId2, int squad)
    {
        if (memberId1 == memberId2)
            return false;

        if (TeamExists(tournamentId, memberId1, memberId2, squad))
            return false;

        using var db = dbFactory.CreateDbContext();

        var tournament = db.Tournaments.Find(tournamentId);
        var member1 = db.Members.Find(memberId1);
        var member2 = db.Members.Find(memberId2);

        if (tournament == null || member1 == null || member2 == null)
            return false;

        var team = new DoublesTeam
        {
            Tournament = tournament,
            Member1 = member1,
            Member2 = member2,
            Squad = squad
        };

        db.DoublesTeams.Add(team);
        db.SaveChanges();
        return true;
    }

    /// <summary>
    /// Removes the DoublesTeam record with the given ID.
    /// </summary>
    public void RemoveTeam(int teamId)
    {
        using var db = dbFactory.CreateDbContext();
        var team = db.DoublesTeams.Find(teamId);
        if (team != null)
        {
            db.DoublesTeams.Remove(team);
            db.SaveChanges();
        }
    }
}
