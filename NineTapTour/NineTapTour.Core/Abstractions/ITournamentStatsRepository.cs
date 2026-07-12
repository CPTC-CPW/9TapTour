using System.Collections.Generic;
using NineTapTour.Models;

namespace NineTapTour.Abstractions
{
    public interface ITournamentStatsRepository
    {
        List<TournamentStatsList> GetTournamentStatsList(int selectedTournament);

        List<TournamentStatsList> Get3OutOf4TournamentStatsList(int selectedTournament);
    }
}
