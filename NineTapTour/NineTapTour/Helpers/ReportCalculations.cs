using System;
using System.Collections.Generic;
using System.Linq;
using NineTapTour.Models;

namespace NineTapTour.Calculations;

/// <summary>
/// Pure calculation logic for the individual and tour-wide client reports.
/// All methods operate on flattened ReportGameEntry lists produced by ReportsDB
/// so they can be unit tested without a database.
/// </summary>
public static class ReportCalculations
{
    const int FIRST_PLACE = 1;
    const int SECOND_PLACE = 2;
    const int THIRD_PLACE = 3;
    const int TOP_5_CUTOFF = 5;
    const int TOP_10_CUTOFF = 10;

    /// <summary>
    /// Returns tournament entries ranked by scratch series, highest first.
    /// Entries with the same series share the same rank.
    /// </summary>
    /// <param name="topN">Maximum rows to return, or null for all</param>
    public static List<HighSeriesRow> GetHighSeries(List<ReportGameEntry> entries, int? topN = null)
    {
        var rows = entries
            .Where(e => e.GamesPlayed > 0)
            .OrderByDescending(e => e.ScratchSeries)
            .ThenByDescending(e => e.HandicapSeries)
            .Select(e => new HighSeriesRow
            {
                Member = e.MemberNumber,
                Name = e.FullName,
                Date = e.TournamentDate,
                Location = e.Location,
                Series = e.ScratchSeries,
                SeriesWithHdcp = e.HandicapSeries
            })
            .ToList();

        AssignRanks(rows, r => r.Series, (r, rank) => r.Rank = rank);
        return Limit(rows, topN);
    }

    /// <summary>
    /// Returns individual game scores ranked highest first. Every used game from
    /// every entry is a candidate. Games with the same score share the same rank.
    /// </summary>
    /// <param name="topN">Maximum rows to return, or null for all</param>
    public static List<HighGameRow> GetHighGames(List<ReportGameEntry> entries, int? topN = null)
    {
        var rows = entries
            .SelectMany(e => e.GameScores.Select(score => new HighGameRow
            {
                Member = e.MemberNumber,
                Name = e.FullName,
                Date = e.TournamentDate,
                Location = e.Location,
                Game = score
            }))
            .OrderByDescending(r => r.Game)
            .ToList();

        AssignRanks(rows, r => r.Game, (r, rank) => r.Rank = rank);
        return Limit(rows, topN);
    }

    /// <summary>
    /// Builds one aggregate summary per member from the given entries:
    /// entry/tournament counts, earnings, scratch average, high series/game,
    /// and placement counts (1st/2nd/3rd and Top 5/Top 10 finishes).
    /// The returned list is unranked and unsorted; see GetTourReport.
    /// </summary>
    /// <param name="includeSidePots">When true, side pot winnings are added to earnings</param>
    public static List<MemberReportSummary> BuildMemberSummaries(List<ReportGameEntry> entries, bool includeSidePots = false)
    {
        return entries
            .GroupBy(e => e.MemberNumber)
            .Select(group =>
            {
                int totalPins = group.Sum(e => e.ScratchSeries);
                int totalGames = group.Sum(e => e.GamesPlayed);

                return new MemberReportSummary
                {
                    Member = group.Key,
                    Name = group.First().FullName,
                    Entries = group.Count(),
                    Tournaments = group.Select(e => e.TournamentId).Distinct().Count(),
                    Earnings = group.Sum(e => e.MoneyWon + (includeSidePots ? e.SidePot : 0)),
                    Average = totalGames == 0 ? 0 : Math.Round((double)totalPins / totalGames, 2),
                    HighSeries = group.Max(e => e.ScratchSeries),
                    HighGame = group.Max(e => e.GameScores.Count == 0 ? 0 : e.GameScores.Max()),
                    FirstPlace = group.Count(e => e.PlaceStanding == FIRST_PLACE),
                    SecondPlace = group.Count(e => e.PlaceStanding == SECOND_PLACE),
                    ThirdPlace = group.Count(e => e.PlaceStanding == THIRD_PLACE),
                    Top5 = group.Count(e => e.PlaceStanding >= FIRST_PLACE && e.PlaceStanding <= TOP_5_CUTOFF),
                    Top10 = group.Count(e => e.PlaceStanding >= FIRST_PLACE && e.PlaceStanding <= TOP_10_CUTOFF)
                };
            })
            .ToList();
    }

    /// <summary>
    /// Builds the tour-wide report for the given category: member summaries
    /// sorted and ranked by the category's value.
    /// </summary>
    /// <param name="topN">Maximum rows to return, or null for all</param>
    /// <param name="includeSidePots">When true, side pot winnings are added to earnings</param>
    public static List<MemberReportSummary> GetTourReport(List<ReportGameEntry> entries, TourReportCategory category, int? topN = null, bool includeSidePots = false)
    {
        List<MemberReportSummary> summaries = BuildMemberSummaries(entries, includeSidePots);

        Func<MemberReportSummary, double> sortValue = category switch
        {
            TourReportCategory.HighAverages => s => s.Average,
            TourReportCategory.TotalEntries => s => s.Entries,
            TourReportCategory.Earnings => s => (double)s.Earnings,
            TourReportCategory.FirstPlaceFinishes => s => s.FirstPlace,
            TourReportCategory.Top5Finishes => s => s.Top5,
            TourReportCategory.Top10Finishes => s => s.Top10,
            _ => s => s.HighSeries
        };

        var rows = summaries
            .OrderByDescending(sortValue)
            .ThenBy(s => s.Name)
            .ToList();

        AssignRanks(rows, sortValue, (r, rank) => r.Rank = rank);
        return Limit(rows, topN);
    }

    /// <summary>
    /// Builds the name/value statistic rows displayed for an individual member's
    /// summary report. Returns an empty list when the member has no entries.
    /// </summary>
    /// <param name="includeSidePots">When true, side pot winnings are added to earnings</param>
    public static List<ReportStatistic> BuildIndividualSummary(List<ReportGameEntry> entries, bool includeSidePots = false)
    {
        if (entries.Count == 0)
        {
            return [];
        }

        MemberReportSummary summary = BuildMemberSummaries(entries, includeSidePots).First();

        ReportGameEntry bestSeries = entries
            .Where(e => e.GamesPlayed > 0)
            .OrderByDescending(e => e.ScratchSeries)
            .FirstOrDefault();

        List<HighGameRow> bestGames = GetHighGames(entries, 1);

        List<ReportStatistic> stats =
        [
            new() { Statistic = "Member", Value = $"{summary.Name} ({summary.Member})" },
            new() { Statistic = "Total Entries", Value = summary.Entries.ToString() },
            new() { Statistic = "Tournaments Bowled", Value = summary.Tournaments.ToString() },
            new() { Statistic = includeSidePots ? "Total Earnings (incl. Side Pots)" : "Total Earnings", Value = summary.Earnings.ToString("C2") },
            new() { Statistic = "Average (Scratch)", Value = summary.Average.ToString("N2") },
            new() { Statistic = "High Series (Scratch)", Value = FormatBestSeries(bestSeries) },
            new() { Statistic = "High Game (Scratch)", Value = FormatBestGame(bestGames) },
            new() { Statistic = "1st Place Finishes", Value = summary.FirstPlace.ToString() },
            new() { Statistic = "2nd Place Finishes", Value = summary.SecondPlace.ToString() },
            new() { Statistic = "3rd Place Finishes", Value = summary.ThirdPlace.ToString() },
            new() { Statistic = "Top 5 Finishes", Value = summary.Top5.ToString() },
            new() { Statistic = "Top 10 Finishes", Value = summary.Top10.ToString() },
        ];

        return stats;
    }

    static string FormatBestSeries(ReportGameEntry bestSeries)
    {
        if (bestSeries == null)
        {
            return "N/A";
        }
        return $"{bestSeries.ScratchSeries} — {bestSeries.Location} {bestSeries.TournamentDate.ToShortDateString()}";
    }

    static string FormatBestGame(List<HighGameRow> bestGames)
    {
        if (bestGames.Count == 0)
        {
            return "N/A";
        }
        HighGameRow best = bestGames[0];
        return $"{best.Game} — {best.Location} {best.Date.ToShortDateString()}";
    }

    /// <summary>
    /// Assigns standard competition ranks (1, 2, 2, 4) to a list already sorted
    /// by value descending. Rows with equal values share the same rank.
    /// </summary>
    static void AssignRanks<T>(List<T> sortedRows, Func<T, double> value, Action<T, int> setRank)
    {
        int currentRank = 1;

        for (int i = 0; i < sortedRows.Count; i++)
        {
            if (i == 0 || value(sortedRows[i]) != value(sortedRows[i - 1]))
            {
                currentRank = i + 1;
            }
            setRank(sortedRows[i], currentRank);
        }
    }

    static List<T> Limit<T>(List<T> rows, int? topN)
    {
        if (topN.HasValue && topN.Value > 0 && rows.Count > topN.Value)
        {
            return rows.Take(topN.Value).ToList();
        }
        return rows;
    }
}
