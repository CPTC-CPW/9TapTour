#nullable disable
using NineTapTour.Core.Models;
using System.Collections.Generic;

namespace NineTapTour.Core.Repositories;

/// <summary>
/// Data access for tournament statistics. Instance replacement for the old static
/// TournamentStatsListDB; method names and behavior are unchanged.
/// </summary>
public interface ITournamentStatsRepository
{
    List<TournamentStatsList> GetTournamentStatsList(int selectedTournament);
    List<int> GetTop3OutOf4(int? game1, int? game2, int? game3, int? game4);
    List<TournamentStatsList> Get3OutOf4TournamentStatsList(int selectedTournament);
}
