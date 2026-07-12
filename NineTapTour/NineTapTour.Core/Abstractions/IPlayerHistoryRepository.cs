using System.Collections.Generic;
using NineTapTour.Models;

namespace NineTapTour.Abstractions
{
    /// <summary>Data access for player history records. Replaces the static <c>PlayerHistoryDB</c>.</summary>
    public interface IPlayerHistoryRepository
    {
        List<PlayerHistoryViewModel> GetMemberPlayerHistory(int memberNum);
        List<PlayerHistoryViewModel> GetLastQtyGamesMoneyWon(int memberNum, int howmany);
        List<PlayerHistoryViewModel> GetLastFiveTournaments(int memberNum);
        PlayerHistoryViewModel GetMostRecentTournament(int memberNum);
        void DeleteGame(Game game);
        decimal GetTotalMoneyWon(int memberNum);
        int? GetMostRecentAverage(int memberNum);
    }
}
