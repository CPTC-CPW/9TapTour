namespace NineTapTour.Core.Models;

/// <summary>
/// One printed row of a member report: every value is the exact string the
/// printer draws, already formatted (team vs single member, dues text, etc.).
/// </summary>
public record ReportRowContent(string Placing, string Score, string MemberNumber, string Name, string DuesText);

/// <summary>
/// The rows for one printed page of a member report.
/// <see cref="CutoffAfterRowIndex"/> is the zero-based row index on this page
/// after which the red winners cutoff line is drawn, or null when no line
/// falls on this page.
/// </summary>
public record ReportPageContent(IReadOnlyList<ReportRowContent> Rows, int? CutoffAfterRowIndex);

/// <summary>
/// Everything a member report prints, computed up front: the header strings
/// (repeated on every page) and the rows chunked into pages.
/// <see cref="SeriesSubtitle"/> is only non-null for series reports.
/// </summary>
public record MemberReportContent(
    string TournamentLine,
    string? SeriesSubtitle,
    string Title,
    string ColumnHeaderLine,
    IReadOnlyList<ReportPageContent> Pages);

/// <summary>
/// The text drawn on a single member's recap card: average, handicap (drawn
/// once per game), the pre-multiplied total handicap, and identity lines.
/// </summary>
public record RecapCardContent(
    string AverageText,
    string HandicapText,
    string BonusText,
    string TotalHandicapText,
    string NameLine,
    string CityLine,
    string MemberNumberText);

/// <summary>
/// The three text lines printed on a single mailing label.
/// </summary>
public record LabelContent(string NameLine, string StreetLine, string CityStateZipLine);
