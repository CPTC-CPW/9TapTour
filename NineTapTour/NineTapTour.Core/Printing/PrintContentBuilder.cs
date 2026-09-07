using NineTapTour.Core.Entities;
using NineTapTour.Core.Models;
using static NineTapTour.Core.Calculations.ReportHelper;

namespace NineTapTour.Core.Printing;

/// <summary>
/// Computes WHAT gets printed - ordered rows, page chunking, the winners
/// cutoff-line position, and formatted strings - so the WinForms Print class
/// only handles HOW it is drawn (fonts, Graphics, coordinates). Logic was
/// moved verbatim from NineTapTour.Database.Print; expectations must not change.
/// </summary>
public static class PrintContentBuilder
{
    public const int BowlersPerPage = 40;

    private const int ThreeGameCount = 3;
    private const int FourGameCount = 4;

    /// <summary>
    /// Builds the full content of a member report: header strings plus the
    /// rows chunked into pages of <see cref="BowlersPerPage"/>, with the
    /// winners cutoff line placed after 20 percent of the members (minimum
    /// list handling below) unless <paramref name="manualCutoff"/> overrides it.
    /// </summary>
    public static MemberReportContent BuildMemberReport(List<MemberScores> tempMemberList, Tournament selectedTournament, ReportType reportTypeNum, int currentSquad, List<int> squadList, bool printDues, int? manualCutoff = null)
    {
        // This var is used to draw a line after the rows of money-winning members are printed
        int winningPlaces;
        if (tempMemberList.Count < 5)
        {
            winningPlaces = 5;
        }
        else
        {
            winningPlaces = tempMemberList.Count / 5;
        }

        string tournamentType = "";

        if (selectedTournament.ThreeOutOf4)
        {
            tournamentType = "3of4 ";
        }

        // The location and date (Month, Day, Year, e.g. May 13th 2019 = 5-13-2019)
        string tournamentLine = selectedTournament.Location + " " + tournamentType + string.Format("{0:M-d-yyyy}", selectedTournament.Date);

        string header = "9 Tap Tour High - ";

        string reportType = "";
        string? seriesSubtitle = null;

        // For building the report type using the reportTypeNum
        if (reportTypeNum == ReportType.HighGameHandicapGameSenior)
        {
            reportType = "Game Senior";
        }
        else if (reportTypeNum == ReportType.HighGame)
        {
            reportType = "Game";
        }
        else if (reportTypeNum == ReportType.HighSeriesScratch)
        {
            // The 'Through squad x' header is only drawn for Series Reports
            seriesSubtitle = BuildSeriesSubtitle(squadList);
            reportType = "Series";
        }

        string title;

        // If Series button was clicked, should not say final based on qual by squad, rather by Filter Series by Squad. Still shows qual by squad filters on the listed players.
        if (currentSquad == 0 && string.Equals(reportType, "Series"))
        {
            title = header + reportType + " Standings";
        }
        // The report title
        else if (currentSquad == 0)
        {
            title = header + reportType + " Final Standings";
        }
        else
        {
            title = header + reportType + "     Squad " + currentSquad + " Standings ";
        }

        if (reportTypeNum == 0)
        {
            reportType = "Game";
        }

        // The header of the data
        string columnHeaderLine;
        if (printDues)
        {
            columnHeaderLine = "       " + reportType + "     Mem No            Name                                           Membership Paid To";
        }
        else
        {
            columnHeaderLine = "       " + reportType + "     Mem No            Name";
        }

        int cutoffPlace = manualCutoff ?? winningPlaces;

        List<ReportPageContent> pages = [];
        for (int pageStart = 0; pageStart < tempMemberList.Count; pageStart += BowlersPerPage)
        {
            List<ReportRowContent> rows = [];
            for (int i = pageStart; i < tempMemberList.Count && i < pageStart + BowlersPerPage; i++)
            {
                rows.Add(BuildReportRow(tempMemberList[i], printDues));
            }

            // The cutoff line is drawn after the row of the last money-winning
            // member when that row falls on this page
            int? cutoffAfterRowIndex = null;
            int cutoffRow = cutoffPlace - 1 - pageStart;
            if (cutoffRow >= 0 && cutoffRow < rows.Count)
            {
                cutoffAfterRowIndex = cutoffRow;
            }

            pages.Add(new ReportPageContent(rows, cutoffAfterRowIndex));
        }

        return new MemberReportContent(tournamentLine, seriesSubtitle, title, columnHeaderLine, pages);
    }

    /// <summary>
    /// Builds the 'Final' / 'Through Squad x' subtitle drawn on series reports.
    /// </summary>
    private static string BuildSeriesSubtitle(List<int> squadList)
    {
        if (squadList[0] == 0) // 'All Squads' is checked
        {
            return "Final";
        }

        // Create helper ints and bool
        int min = squadList[0];
        int max = squadList[squadList.Count - 1];
        string list = string.Join(",", squadList.ToArray());
        bool consective = true;

        if (squadList.Count == 1) // If one squad
        {
            if (min == 1) // Checks for squad 1 is test for progression based filter
            {
                return "Through Squad " + min;
            }
            return "Squad " + min;
        }

        // If more than one squad
        // Test to see if squads given are consecutive
        for (int i = 1; i < squadList.Count; i++)
        {
            if (squadList[i] - squadList[i - 1] != 1)
            {
                consective = false;
            }
        }

        if (squadList.Count == 2) // If filtering two squads
        {
            if (consective) // Calls if bool consecutive is true
            {
                if (min == 1)
                {
                    return "Through Squad " + max;
                }
                return "Squads " + min + " Through " + max;
            }
            // If bool not true
            return "Squad " + min + " and " + max;
        }

        // If three or more squads being filtered
        if (consective)
        {
            if (min == 1)
            {
                return "Through squad" + max;
            }
            return "Squads " + min + " Through " + max;
        }
        return "Squads " + list;
    }

    /// <summary>
    /// Builds the formatted strings for one report row, handling doubles
    /// (team) entries and the optional membership dues column.
    /// </summary>
    private static ReportRowContent BuildReportRow(MemberScores entry, bool printDues)
    {
        string memberNumString = (entry is TeamMemberScores tms)
            ? $"{tms.Partner1MemberId} & {tms.Partner2MemberId}"
            : entry.MemberId.ToString();

        // Decides if the last date the member paid their dues prints on the page
        string duesText = string.Empty;
        if (printDues)
        {
            duesText = (entry is TeamMemberScores tmsEntry)
                ? $"{FormatDuesYear(tmsEntry.LastPaymentYear)} & {FormatDuesYear(tmsEntry.Partner2LastPaymentYear)}"
                : FormatDuesYear(entry.LastPaymentYear);
        }

        // Create name string containing lastname, firstname
        string nameString = (entry is TeamMemberScores teamEntry)
            ? $"{teamEntry.Partner1FirstName} {teamEntry.Partner1LastName} & {teamEntry.Partner2FirstName} {teamEntry.Partner2LastName}"
            : entry.LastName + ", " + entry.FirstName;

        return new ReportRowContent(entry.placing.ToString(), entry.Score.ToString(), memberNumString, nameString, duesText);
    }

    /// <summary>
    /// Formats the year a member's dues are paid through from their last
    /// payment year: one year after a numeric payment year, 'life ' passes
    /// through unchanged, and missing values become 'N/A'.
    /// </summary>
    public static string FormatDuesYear(string lastPaymentYear)
    {
        if (string.IsNullOrWhiteSpace(lastPaymentYear))
            return "N/A";
        if (lastPaymentYear.Equals("life "))
            return lastPaymentYear;
        if (int.TryParse(lastPaymentYear, out int year))
            return (year + 1).ToString();
        return lastPaymentYear;
    }

    /// <summary>
    /// Builds the recap card text for a member record.
    /// </summary>
    public static RecapCardContent BuildRecapCard(Member mem)
    {
        int handicap = (mem.Handicap != null) ? (int)mem.Handicap : 0;
        string average = (mem.Average != null) ? mem.Average.ToString() : "";
        return BuildRecapCard(handicap, mem.Number, mem.City, mem.FirstName, mem.LastName, average, mem.Bonus);
    }

    /// <summary>
    /// Builds the recap card text from individual member values. The total
    /// handicap shown on the card is the per-game handicap applied over three
    /// games and over four games, displayed as "3-game total / 4-game total"
    /// (e.g. a handicap of 25 prints as "75 / 100").
    /// </summary>
    public static RecapCardContent BuildRecapCard(int handicap, int memberNumber, string city, string firstName, string lastName, string average, int bonus)
    {
        return new RecapCardContent(
            average,
            handicap.ToString(),
            bonus.ToString(),
            FormatTotalHandicap(handicap),
            lastName + ", " + firstName,
            city,
            memberNumber.ToString());
    }

    /// <summary>
    /// Formats the recap card total handicap as the 3-game total followed by
    /// the 4-game total, e.g. a per-game handicap of 25 becomes "75 / 100".
    /// </summary>
    public static string FormatTotalHandicap(int handicap)
    {
        return $"{handicap * ThreeGameCount} / {handicap * FourGameCount}";
    }

    /// <summary>
    /// Orders members for recap printing. Client wants the recaps ordered by
    /// last name first.
    /// </summary>
    public static List<Member> OrderMembersForRecaps(List<Member> members)
    {
        return [.. members.OrderBy(member => member.LastName).ThenBy(member => member.FirstName)];
    }

    /// <summary>
    /// Builds the three text lines printed on a member's mailing label.
    /// </summary>
    public static LabelContent BuildLabelLines(Member mem)
    {
        return new LabelContent(
            mem.FirstName + " " + mem.LastName,
            mem.Street,
            mem.City + ", " + mem.State + " " + mem.PostalCode);
    }
}
