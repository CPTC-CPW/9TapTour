#nullable disable
using NineTapTour.Core.Entities;
using System.Collections.Generic;

namespace NineTapTour.Core.Repositories;

/// <summary>
/// Data access for doubles teams. Instance replacement for the old static DoublesTeamDB;
/// method names and behavior are unchanged.
/// </summary>
public interface IDoublesTeamRepository
{
    List<DoublesTeam> GetTeamsByTournament(int tournamentId);
    bool TeamExists(int tournamentId, int memberId1, int memberId2, int squad);
    bool AddTeam(int tournamentId, int memberId1, int memberId2, int squad);
    void RemoveTeam(int teamId);
}
