namespace NineTapTour.Core.Models;

/// <summary>
/// Input values for recalculating a single tournament grid row.
/// The form gathers these from grid cells; the service computes the results.
/// </summary>
public record FinalizeRowInput(
    int? Game1, int? Game2, int? Game3, int? Game4,
    bool Game1Checked, bool Game2Checked, bool Game3Checked, bool Game4Checked,
    int Handicap, int AdjustedAvg, int BaseBonus);

/// <summary>
/// Computed values for a single tournament grid row.
/// <see cref="HandicapWasDerived"/> is true when the handicap was missing and
/// was derived from the adjusted average, so the form can write it back to the grid.
/// </summary>
public record FinalizeRowResult(
    int ResolvedHandicap, bool HandicapWasDerived,
    int ScratchTotal, int CheckedGames, int EntryAvg, int HdcpTotal);

/// <summary>
/// Which game checkboxes should be checked when a row is first loaded.
/// </summary>
public record UseGameFlags(bool Game1, bool Game2, bool Game3, bool Game4);

/// <summary>
/// Result of the bonus preview computation for a loaded row: the value shown in the
/// Bonus column plus flags telling the form which tracking sets to update.
/// </summary>
public record BonusPreviewResult(int DisplayBonus, bool IsCashing, bool AwardedThirdEntryBonus);

/// <summary>
/// One finalized entry from a member's most recent prior tournament, used to derive
/// the carry-forward handicap and bonus.
/// </summary>
public record PreviousEntrySnapshot(int AdjustedAvg, int Bonus, decimal MoneyWon);

/// <summary>
/// One historical finalized game record used when accumulating the 30-entry average window.
/// </summary>
public record HistoryGameEntry(
    int? Game1, int? Game2, int? Game3, int? Game4,
    bool? UseGame1, bool? UseGame2, bool? UseGame3, bool? UseGame4);
