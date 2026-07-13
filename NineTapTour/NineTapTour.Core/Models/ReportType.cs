namespace NineTapTour.Models
{
    /// <summary>The kind of member-scores report being displayed/printed.</summary>
    public enum ReportType
    {
        HighGameHandicapGameSenior,
        HighGame,
        HighSeriesScratch,
        HighSeriesHandicap
    }

    /// <summary>
    /// Holds data for a single bowler's entry in a report.
    /// </summary>
    /// <param name="Placing">Place the bowler placed in a given tournament.</param>
    /// <param name="Score">Score to display for the report.</param>
    /// <param name="MemberNumber">The bowler's member number.</param>
    /// <param name="FullName">The bowler's last name followed by first name, e.g. "Smith, Jane".</param>
    public record ReportEntry(int Placing, int Score, int MemberNumber, string FullName);
}
