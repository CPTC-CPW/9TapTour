using NineTapTour.Models;

namespace NineTapTour.Abstractions
{
    /// <summary>
    /// Builds a tournament's winners list (per-bowler totals with prior-tournament handicaps, or the
    /// combined-team results for doubles). Extracted from FrmTournamentResults so the results
    /// computation is UI-free and testable.
    /// </summary>
    public interface ITournamentResultsService
    {
        WinnersListResult BuildWinnersList(int tournamentId, bool isDoubles, bool isThreeOfFour);
    }
}
