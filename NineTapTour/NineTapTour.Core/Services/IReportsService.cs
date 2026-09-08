using NineTapTour.Core.Models;

namespace NineTapTour.Core.Services;

/// <summary>
/// Runs the client reports (individual summary / high series / high games and
/// the tour-wide categories) over a career, single year, or year range.
/// Extracted from FrmReports so the desktop and web share the category list,
/// title composition, and column formatting.
/// </summary>
public interface IReportsService
{
    /// <summary>Category names offered for individual member reports.</summary>
    IReadOnlyList<string> IndividualCategories { get; }

    /// <summary>Category names offered for tour-wide reports.</summary>
    IReadOnlyList<string> TourCategories { get; }

    /// <summary>Years that have finalized tournament entries, for the period pickers.</summary>
    List<int> GetTournamentYears();

    /// <summary>
    /// Runs the report. Returns a table with no rows when no finalized entries match.
    /// </summary>
    /// <exception cref="ArgumentException">Unknown category, or an individual report without a valid member.</exception>
    ReportTable Run(ReportRequest request);
}
