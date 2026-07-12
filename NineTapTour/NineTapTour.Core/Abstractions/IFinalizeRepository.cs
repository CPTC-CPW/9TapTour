using System.Collections.Generic;
using NineTapTour.Models;

namespace NineTapTour.Abstractions
{
    /// <summary>Data access for tournament finalization. Replaces the static <c>FinalizeTempDB</c>.</summary>
    public interface IFinalizeRepository
    {
        /// <summary>
        /// Calculates league average for given member based off last 30 games
        /// or total games played if less than 30.
        /// </summary>
        double Get30GameAverage(Member mem);

        /// <summary>
        /// Get's the leauge average based off the formula
        /// (Total of Scratch Score / Total of games played)
        /// from the last 30 entries
        /// </summary>
        double Get30GameAverage(int memberNumber, int tournamentId);

        List<CurrentHistory> GetCurrentHistory(int memberNumber, int tournamentId);

        List<PreviousHistory> GetPreviousHistory(int memberNumber, List<CurrentHistory> curHistory);

        /// <summary>
        /// [Phase 4 REFACTORED] Updates Game entity with finalization properties.
        /// FinalizeTemp writes are deprecated - only Games table is updated.
        /// </summary>
        void AddFinalizeTemp(GameViewModel temp);

        /// <summary>
        /// Retrieves a single participant from a tournament based on given gameID.
        /// Return null if no participant is found
        /// </summary>
        Participant GetParticipantByGameId(int gameID);

        /// <summary>
        /// Deletes the Participant given from the database
        /// </summary>
        void DeleteParticipant(Participant p);

        /// <summary>
        /// Gets the entry quantity for a member in a tournament (Phase 3: reads from Games only).
        /// </summary>
        int GetMembersGameEntryCount(int tourneyId, int memberNum);

        /// <summary>
        /// Helper that takes the useGames from the database to see if they are true or null.
        /// We are saying that null is true, because they are optional booleans in the database.
        /// </summary>
        int LeagueAverageHelper(bool? g1, bool? g2, bool? g3, bool? g4);
    }
}
