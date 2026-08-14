#nullable disable
using System;
using System.Collections.Generic;

namespace NineTapTour.Core.Models;

/// <summary>
/// One results row to write into the series report workbook. Values are the raw
/// display strings read from the results table; <see cref="PlaceGroupLabel"/> is
/// only populated for 2-day tournaments.
/// </summary>
public record SeriesReportRow(
    string PlaceDisplay,
    string PlaceGroupLabel,
    string FullName,
    string HandicapDisplay,
    string TotalScoreDisplay,
    string MemberNumberText,
    string EarningsText);

/// <summary>
/// Everything the exporter needs to fill a series report template:
/// tournament header info, the results rows, membership currency per member, and
/// whether doubles check-sheet consolidation should run.
/// </summary>
public record SeriesReportExportRequest(
    string TournamentLocation,
    string TournamentEvent,
    DateTime TournamentDate,
    bool IsTwoDay,
    bool ApplyDoublesCheckConsolidation,
    IReadOnlyList<SeriesReportRow> Rows,
    IReadOnlyDictionary<int, bool> IsMembershipCurrentByMemberNumber);

/// <summary>
/// Earnings and progressive pot read back from one bowler row of a pre-filled
/// results template.
/// </summary>
public record TemplateEarningsRow(decimal Earnings, decimal ProgressivePot);
