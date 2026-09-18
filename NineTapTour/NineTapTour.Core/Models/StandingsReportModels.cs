using NineTapTour.Core.Entities;
using static NineTapTour.Core.Calculations.ReportHelper;

namespace NineTapTour.Core.Models;

/// <summary>
/// Everything needed to fill the standings (series / game / senior) report
/// workbook from the SeriesReportTemplate: tournament header values, which
/// report is being exported, whether to include the "Membership Paid To"
/// column, and the already-ranked rows (doubles rows are TeamMemberScores).
/// </summary>
public sealed record StandingsReportExportRequest(
    string TournamentLocation,
    string TournamentEvent,
    DateTime TournamentDate,
    ReportType ReportType,
    bool PrintDues,
    IReadOnlyList<MemberScores> Rows);
