#nullable disable
using NineTapTour.Core.Entities;
using NineTapTour.Core.Models;
using System.Collections.Generic;

namespace NineTapTour.Core.Repositories;

/// <summary>
/// Data access for tournament finalization. Instance replacement for the old static FinalizeTempDB;
/// method names and behavior are unchanged.
/// </summary>
public interface IFinalizeTempRepository
{
    double Get30GameAverage(Member mem);
    int LeagueAverageHelper(bool? g1, bool? g2, bool? g3, bool? g4);
    double Get30GameAverage(int memberNumber, int tournamentId);
    List<CurrentHistory> GetCurrentHistory(int memberNumber, int tournamentId);
    List<PreviousHistory> GetPreviousHistory(int memberNumber, List<CurrentHistory> curHistory);
    void AddFinalizeTemp(GameViewModel temp);
    Participant GetParticipantByGameId(int gameID);
    void DeleteParticipant(Participant p);
    int GetMembersGameEntryCount(int tourneyId, int memberNum);
}
