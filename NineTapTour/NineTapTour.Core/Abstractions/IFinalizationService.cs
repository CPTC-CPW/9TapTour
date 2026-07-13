using System.Collections.Generic;
using NineTapTour.Models;

namespace NineTapTour.Abstractions
{
    /// <summary>
    /// Applies the finalized bowler rows to their games/members and marks the tournament finalized,
    /// all in a single transaction. Encapsulates the finalize sequencing that used to live in
    /// FrmFinalizeTournament so it can be unit/integration tested without the UI.
    /// </summary>
    public interface IFinalizationService
    {
        void FinalizeTournament(int tournamentId, IReadOnlyList<FinalizeGameInput> rows);
    }
}
