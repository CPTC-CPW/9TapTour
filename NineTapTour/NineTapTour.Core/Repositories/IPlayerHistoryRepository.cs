#nullable disable
using NineTapTour.Core.Entities;
using NineTapTour.Core.Models;
using System.Collections.Generic;

namespace NineTapTour.Core.Repositories;

/// <summary>
/// Data access for player history. Instance replacement for the old static PlayerHistoryDB;
/// method names and behavior are unchanged.
/// </summary>
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
