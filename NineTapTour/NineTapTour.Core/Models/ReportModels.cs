#nullable disable
using System;
using System.Collections.Generic;

namespace NineTapTour.Core.Models
{
    /// <summary>
    /// One tournament entry (one finalized Game row) flattened for report calculations.
    /// GameScores only contains games that are marked as "used" (UseGameX = true).
    /// </summary>
    public class ReportGameEntry
    {
        public int MemberNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int TournamentId { get; set; }
        public DateTime TournamentDate { get; set; }
        public string Location { get; set; }
        public string Event { get; set; }
        public List<int> GameScores { get; set; } = [];
        public int ScratchSeries { get; set; }
        public int HandicapSeries { get; set; }
        public int GamesPlayed { get; set; }
        public decimal MoneyWon { get; set; }
        public decimal SidePot { get; set; }
        public int? PlaceStanding { get; set; }

        public string FullName => LastName + ", " + FirstName;
    }

    /// <summary>
    /// A single tournament entry ranked by series score, for High Series reports.
    /// </summary>
    public class HighSeriesRow
    {
        public int Rank { get; set; }
        public int Member { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public string Location { get; set; }
        public int Series { get; set; }
        public int SeriesWithHdcp { get; set; }
    }

    /// <summary>
    /// A single game score ranked for High Games reports.
    /// </summary>
    public class HighGameRow
    {
        public int Rank { get; set; }
        public int Member { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public string Location { get; set; }
        public int Game { get; set; }
    }

    /// <summary>
    /// Per-member aggregate stats over a time period. Used for the tour-wide
    /// reports (sorted by different columns per category) and the individual summary.
    /// </summary>
    public class MemberReportSummary
    {
        public int Rank { get; set; }
        public int Member { get; set; }
        public string Name { get; set; }
        public int Entries { get; set; }
        public int Tournaments { get; set; }
        public decimal Earnings { get; set; }
        public double Average { get; set; }
        public int HighSeries { get; set; }
        public int HighGame { get; set; }
        public int FirstPlace { get; set; }
        public int SecondPlace { get; set; }
        public int ThirdPlace { get; set; }
        public int Top5 { get; set; }
        public int Top10 { get; set; }
    }

    /// <summary>
    /// A name/value pair used to display the individual member report summary.
    /// </summary>
    public class ReportStatistic
    {
        public string Statistic { get; set; }
        public string Value { get; set; }
    }

    /// <summary>
    /// The categories available for tour-wide reports.
    /// </summary>
    public enum TourReportCategory
    {
        HighSeries,
        HighGames,
        HighAverages,
        TotalEntries,
        Earnings,
        FirstPlaceFinishes,
        Top5Finishes,
        Top10Finishes
    }
}
