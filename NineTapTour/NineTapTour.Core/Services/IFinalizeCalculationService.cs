using NineTapTour.Core.Models;
using System.Collections.Generic;

namespace NineTapTour.Core.Services;

/// <summary>
/// Headless calculation and decision logic for finalizing a tournament, extracted from
/// FrmFinalizeTournament. The form gathers grid cell values into POCOs, this service
/// computes results, and the form writes them back to the grid.
/// </summary>
public interface IFinalizeCalculationService
{
    /// <summary>
    /// Recomputes Scratch Total, HDCP Total, and Entry AVG for a row based on which
    /// game checkboxes are checked. When no stored handicap is available it is derived
    /// from the adjusted average (a missing value is filled in — a valid stored
    /// handicap is never overwritten).
    /// </summary>
    FinalizeRowResult RecalculateRow(FinalizeRowInput input);

    /// <summary>
    /// Computes the combined HDCP total shared by both individual rows of a doubles team.
    /// </summary>
    int ComputeCombinedHdcpTotal(int myScratch, int myCheckedGames, int myHandicap, int myBaseBonus,
        int partnerScratch, int partnerCheckedGames, int partnerHandicap, int partnerBaseBonus);

    /// <summary>
    /// Returns the New HDCP preview for the given adjusted average — the handicap the
    /// Member record will receive when the tournament is finalized — or null when the
    /// adjusted average is not set.
    /// </summary>
    int? ComputeNewHdcpPreview(int adjustedAvg);

    /// <summary>
    /// Combines historical and live current-tournament scratch/game totals into the
    /// rolling 30-entry average, rounded to one decimal. Returns 0 when there are no games.
    /// </summary>
    double Compute30EntryAverage(int historyScratch, int historyGames, int currentScratch, int currentGames);

    /// <summary>
    /// Accumulates the historical portion of the 30-entry average window from finalized
    /// entries (newest first), taking at most (30 − currentEntryCount) entries.
    /// Entries with no counted games are skipped and do not consume the window.
    /// </summary>
    (int Scratch, int Games) Compute30EntryHistory(IEnumerable<HistoryGameEntry> entriesNewestFirst, int currentEntryCount);

    /// <summary>
    /// Returns true when a game score is 40 or more pins below the bowler's league
    /// average, indicating potential sandbagging. Always false when the league average
    /// or the score is not set.
    /// </summary>
    bool IsSandbaggingScore(double leagueAverage, int score);

    /// <summary>
    /// Returns true when the row passes finalization validation: Director Check is
    /// checked and the adjusted average is non-zero.
    /// </summary>
    bool IsRowValid(bool directorChecked, int adjustedAvg);

    /// <summary>
    /// Determines the default checked state for each game checkbox: a game is checked
    /// when it has a recorded score; for 3-of-4 tournaments the lowest of four games is
    /// unchecked; explicitly saved use-game flags override both defaults.
    /// </summary>
    UseGameFlags DetermineUseGameDefaults(int? game1, int? game2, int? game3, int? game4,
        bool? useGame1, bool? useGame2, bool? useGame3, bool? useGame4, bool threeOutOf4);

    /// <summary>
    /// Derives the carry-forward handicap and bonus from a member's entries in their most
    /// recent finalized tournament. The handicap comes from the first entry with a positive
    /// adjusted average; the bonus is the minimum when the member cashed, otherwise the maximum.
    /// The list must be non-empty.
    /// </summary>
    (int Hdcp, int Bonus) ComputePreviousHandicapAndBonus(IReadOnlyList<PreviousEntrySnapshot> previousEntries);

    /// <summary>
    /// Computes the New Bonus column for a row: the bonus pins the member carries out of
    /// this tournament. Pins are deducted from members who cashed — those who won place
    /// money or finished within the cash line — and +1 pin is awarded to members reaching
    /// their 3rd total entry (not cashing, not a 2-day championship).
    /// </summary>
    /// <param name="memberMoneyWon">
    /// Place money the member won across all of their entries, excluding side pots.
    /// Any amount above zero makes the member a casher regardless of the cash line.
    /// </param>
    BonusPreviewResult ComputeBonusPreview(int baseBonus, int memberPlacing, int cashLine, bool isTwoDay,
        int historicalEntryCount, int currentEntryCount, decimal memberMoneyWon);

    /// <summary>
    /// Resolves the handicap shown in the grid: the previous tournament's handicap when
    /// positive, else the stored game handicap, else a value derived from the adjusted
    /// average, else 0.
    /// </summary>
    int ResolveDisplayHandicap(int? previousHandicap, int storedHandicap, int adjustedAvg);

    /// <summary>
    /// Computes an entry's total score (with handicap and bonus) for place standing
    /// calculation. For 3-of-4 tournaments the lowest of four games is dropped.
    /// </summary>
    int ComputeEntryTotalScore(int? game1, int? game2, int? game3, int? game4, int handicap, int bonus, bool threeOutOf4);

    /// <summary>
    /// Assigns place standings to doubles teams already sorted by combined total
    /// descending. Tied totals share a place; the next distinct total gets its
    /// positional place (1-based index + 1).
    /// </summary>
    int[] AssignTeamPlaces(IReadOnlyList<int> combinedTotalsDescending);
}
