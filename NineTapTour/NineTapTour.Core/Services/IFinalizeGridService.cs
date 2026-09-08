using NineTapTour.Core.Models;

namespace NineTapTour.Core.Services;

/// <summary>
/// The finalize tournament grid extracted from FrmFinalizeTournament. The grid
/// is always rebuilt from the database: the desktop persisted every edit as it
/// happened, so a stateless build-edit-rebuild cycle reproduces its behaviour.
/// 2-day New Bonus overrides are the one piece of state the caller owns; they
/// are passed in and applied on every build until finalization writes them.
/// </summary>
public interface IFinalizeGridService
{
    /// <summary>Builds the grid with all computed columns, previews, and validation.</summary>
    FinalizeGridModel BuildGrid(int tournamentId, IReadOnlyDictionary<int, int>? newBonusOverridesByMember = null);

    /// <summary>
    /// Persists an edited row (mirroring ADJ AVG, Bonus, and Director Check to the
    /// member's other entries, as the desktop did) and returns the rebuilt grid.
    /// Edits are ignored once the tournament is finalized.
    /// </summary>
    FinalizeGridModel ApplyRowEdit(int tournamentId, FinalizeRowEdit edit, IReadOnlyDictionary<int, int>? newBonusOverridesByMember = null);

    /// <summary>The member detail panel: this tournament's live entries followed by finalized history.</summary>
    FinalizeDetailModel BuildDetail(FinalizeGridModel grid, int memberNumber);

    /// <summary>Doubles only: one summary row per team from the consecutive row pairs.</summary>
    List<FinalizeTeamRow> BuildTeamView(FinalizeGridModel grid);

    /// <summary>
    /// Validates every row (Director Check set and ADJ AVG non-zero) and, when all
    /// pass, marks the games finalized, records league averages, updates each
    /// member's Average/Handicap/Bonus once, and flags the tournament finalized.
    /// </summary>
    FinalizeOutcome Finalize(int tournamentId, IReadOnlyDictionary<int, int>? newBonusOverridesByMember = null);
}
