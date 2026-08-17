using NineTapTour.Core.Calculations;
using NineTapTour.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NineTapTour.Core.Services;

/// <summary>
/// Headless implementation of the finalize-tournament calculation and decision logic.
/// All methods are pure; logic was moved verbatim from FrmFinalizeTournament (M7.1).
/// </summary>
public class FinalizeCalculationService : IFinalizeCalculationService
{
    // Scores this many pins (or more) below the league average flag potential sandbagging
    private const int SandbagThreshold = 40;

    // Size of the rolling entry window used for the 30-entry average
    private const int ThirtyEntryWindow = 30;

    public FinalizeRowResult RecalculateRow(FinalizeRowInput input)
    {
        int hdcp = input.Handicap;
        bool handicapWasDerived = false;

        // When no stored handicap is available, derive it from the ADJ AVG entered by the
        // director. This only fills in a missing value — it never overwrites a valid stored handicap.
        if (hdcp == 0 && input.AdjustedAvg > 0)
        {
            hdcp = TournamentCalculations.CalculateHandicapPins(input.AdjustedAvg);
            handicapWasDerived = true;
        }

        int g1 = input.Game1Checked ? input.Game1 ?? 0 : 0;
        int g2 = input.Game2Checked ? input.Game2 ?? 0 : 0;
        int g3 = input.Game3Checked ? input.Game3 ?? 0 : 0;
        int g4 = input.Game4Checked ? input.Game4 ?? 0 : 0;

        int scratch      = g1 + g2 + g3 + g4;
        int checkedGames = (input.Game1Checked ? 1 : 0) + (input.Game2Checked ? 1 : 0)
                         + (input.Game3Checked ? 1 : 0) + (input.Game4Checked ? 1 : 0);

        int hdcpTotal = scratch + (checkedGames * (hdcp + input.BaseBonus));
        int entryAvg  = checkedGames > 0 ? scratch / checkedGames : 0;

        return new FinalizeRowResult(hdcp, handicapWasDerived, scratch, checkedGames, entryAvg, hdcpTotal);
    }

    public int ComputeCombinedHdcpTotal(int myScratch, int myCheckedGames, int myHandicap, int myBaseBonus,
        int partnerScratch, int partnerCheckedGames, int partnerHandicap, int partnerBaseBonus)
    {
        return myScratch + partnerScratch
            + myCheckedGames * (myHandicap + myBaseBonus)
            + partnerCheckedGames * (partnerHandicap + partnerBaseBonus);
    }

    public int? ComputeNewHdcpPreview(int adjustedAvg)
    {
        return adjustedAvg > 0 ? TournamentCalculations.CalculateHandicapPins(adjustedAvg) : null;
    }

    public double Compute30EntryAverage(int historyScratch, int historyGames, int currentScratch, int currentGames)
    {
        int totalGames   = historyGames + currentGames;
        int totalScratch = historyScratch + currentScratch;
        return totalGames > 0 ? Math.Round((double)totalScratch / totalGames, 1) : 0;
    }

    public (int Scratch, int Games) Compute30EntryHistory(IEnumerable<HistoryGameEntry> entriesNewestFirst, int currentEntryCount)
    {
        int limit = Math.Max(ThirtyEntryWindow - currentEntryCount, 0);
        int totalScratch = 0, totalGames = 0, taken = 0;

        foreach (HistoryGameEntry g in entriesNewestFirst)
        {
            if (taken >= limit) break;
            int gCount = (g.UseGame1 != false && g.Game1.HasValue ? 1 : 0)
                       + (g.UseGame2 != false && g.Game2.HasValue ? 1 : 0)
                       + (g.UseGame3 != false && g.Game3.HasValue ? 1 : 0)
                       + (g.UseGame4 != false && g.Game4.HasValue ? 1 : 0);
            if (gCount == 0) continue;
            totalScratch += (g.UseGame1 != false ? (g.Game1 ?? 0) : 0)
                          + (g.UseGame2 != false ? (g.Game2 ?? 0) : 0)
                          + (g.UseGame3 != false ? (g.Game3 ?? 0) : 0)
                          + (g.UseGame4 != false ? (g.Game4 ?? 0) : 0);
            totalGames   += gCount;
            taken++;
        }

        return (totalScratch, totalGames);
    }

    public bool IsSandbaggingScore(double leagueAverage, int score)
    {
        return leagueAverage > 0 && score > 0 && (leagueAverage - score) >= SandbagThreshold;
    }

    public bool IsRowValid(bool directorChecked, int adjustedAvg)
    {
        return directorChecked && adjustedAvg != 0;
    }

    public UseGameFlags DetermineUseGameDefaults(int? game1, int? game2, int? game3, int? game4,
        bool? useGame1, bool? useGame2, bool? useGame3, bool? useGame4, bool threeOutOf4)
    {
        // A game is checked when it has a recorded score (non-null) — used as default
        // when UseGame flags have never been explicitly saved.
        bool g1Checked = game1.HasValue;
        bool g2Checked = game2.HasValue;
        bool g3Checked = game3.HasValue;
        bool g4Checked = game4.HasValue;

        // 3-of-4: uncheck the lowest-scoring game when all 4 are present
        // Only auto-apply when the flags have never been explicitly saved
        bool useGameNeverSaved = useGame1 == null && useGame2 == null
                              && useGame3 == null && useGame4 == null;
        if (threeOutOf4 && useGameNeverSaved)
        {
            var validScores = new[]
            {
                (Score: game1, Game: 1),
                (Score: game2, Game: 2),
                (Score: game3, Game: 3),
                (Score: game4, Game: 4)
            }.Where(x => x.Score.HasValue).ToList();

            if (validScores.Count == 4)
            {
                int lowestGame = validScores.MinBy(x => x.Score!.Value).Game;
                if (lowestGame == 1) g1Checked = false;
                else if (lowestGame == 2) g2Checked = false;
                else if (lowestGame == 3) g3Checked = false;
                else if (lowestGame == 4) g4Checked = false;
            }
        }

        // Restore explicitly saved use-game flags (overrides defaults and 3-of-4 auto logic)
        if (useGame1.HasValue) g1Checked = useGame1.Value;
        if (useGame2.HasValue) g2Checked = useGame2.Value;
        if (useGame3.HasValue) g3Checked = useGame3.Value;
        if (useGame4.HasValue) g4Checked = useGame4.Value;

        return new UseGameFlags(g1Checked, g2Checked, g3Checked, g4Checked);
    }

    public (int Hdcp, int Bonus) ComputePreviousHandicapAndBonus(IReadOnlyList<PreviousEntrySnapshot> previousEntries)
    {
        PreviousEntrySnapshot withAvg = previousEntries.FirstOrDefault(e => e.AdjustedAvg > 0);
        int prevHdcp  = withAvg != null ? TournamentCalculations.CalculateHandicapPins(withAvg.AdjustedAvg) : 0;
        int prevBonus = previousEntries.Any(e => e.MoneyWon > 0)
            ? previousEntries.Min(e => e.Bonus)
            : previousEntries.Max(e => e.Bonus);
        return (prevHdcp, prevBonus);
    }

    public BonusPreviewResult ComputeBonusPreview(int baseBonus, int memberPlacing, int cashLine, bool isTwoDay,
        int historicalEntryCount, int currentEntryCount, decimal memberMoneyWon)
    {
        // A bowler cashes when the director awarded them place money, or when they finished
        // within the cash line. The money check matters because the cash line
        // ((entries - comps) / 5) rounds down to 0 in tournaments with fewer than five
        // entries, which would otherwise let the winner keep their bonus pins.
        // 2-day championships store a round group in PlaceStanding rather than a placement,
        // so the place-based deduction never applies to them.
        bool isCashing = !isTwoDay && memberPlacing > 0
                      && (memberMoneyWon > 0 || memberPlacing <= cashLine);
        int displayBonus = isCashing
            ? TournamentCalculations.DeductFromBonusPins(memberPlacing, baseBonus)
            : baseBonus;

        // Award +1 bonus pin to new bowlers reaching their 3rd total entry
        // (history + current tournament), but only when they are not cashing.
        // Not applicable to 2-day championships (earnings are set manually).
        bool awardedThirdEntryBonus = false;
        if (!isCashing && !isTwoDay && historicalEntryCount + currentEntryCount == 3)
        {
            displayBonus = TournamentCalculations.ValidateBonusPins(displayBonus + 1);
            awardedThirdEntryBonus = true;
        }

        return new BonusPreviewResult(displayBonus, isCashing, awardedThirdEntryBonus);
    }

    public int ResolveDisplayHandicap(int? previousHandicap, int storedHandicap, int adjustedAvg)
    {
        if (previousHandicap is > 0) return previousHandicap.Value;
        if (storedHandicap > 0) return storedHandicap;
        return adjustedAvg > 0 ? TournamentCalculations.CalculateHandicapPins(adjustedAvg) : 0;
    }

    public int ComputeEntryTotalScore(int? game1, int? game2, int? game3, int? game4, int handicap, int bonus, bool threeOutOf4)
    {
        if (threeOutOf4)
        {
            List<int> scores = new[] { game1, game2, game3, game4 }
                .Where(g => g.HasValue).Select(g => g.Value).ToList();
            if (scores.Count == 4)
                scores.Remove(scores.Min());
            return scores.Sum() + (scores.Count * (handicap + bonus));
        }

        int validGames = (game1.HasValue ? 1 : 0) + (game2.HasValue ? 1 : 0)
                       + (game3.HasValue ? 1 : 0) + (game4.HasValue ? 1 : 0);
        int scratch    = (game1 ?? 0) + (game2 ?? 0) + (game3 ?? 0) + (game4 ?? 0);
        return scratch + (validGames * (handicap + bonus));
    }

    public int[] AssignTeamPlaces(IReadOnlyList<int> combinedTotalsDescending)
    {
        int[] places = new int[combinedTotalsDescending.Count];
        if (places.Length == 0) return places;

        places[0] = 1;
        for (int i = 1; i < places.Length; i++)
            places[i] = combinedTotalsDescending[i] == combinedTotalsDescending[i - 1]
                ? places[i - 1]
                : i + 1;
        return places;
    }

    /// <summary>
    /// Applies the 9-Tap half-rate bonus rule for doubles tournaments.
    /// Cashers lose half as many bonus pins (magnitude rounded up so the bowler
    /// retains more pins). Non-cashers keep their base bonus unchanged.
    /// </summary>
    public static int ComputeHalfRateBonus(int baseBonus, int place, bool isCashing)
    {
        if (!isCashing) return baseBonus;
        int normalResult = TournamentCalculations.DeductFromBonusPins(place, baseBonus);
        int delta        = normalResult - baseBonus; // negative for cashers
        // Round the magnitude up so the deduction is slightly larger (bowler loses
        // slightly more than a pure half, which is the standard rounding convention).
        int halfDelta = delta >= 0
            ? (int)Math.Ceiling(delta / 2.0)
            : -(int)Math.Ceiling(-delta / 2.0);
        return baseBonus + halfDelta;
    }
}
