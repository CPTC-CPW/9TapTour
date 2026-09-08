namespace NineTapTour.Core.Models;

/// <summary>Whether a client report covers one member or the whole tour.</summary>
public enum ReportScope
{
    Individual,
    TourWide,
}

/// <summary>How a report column's values should be formatted.</summary>
public enum ReportColumnKind
{
    Text,
    Integer,
    Number,
    Currency,
    Date,
}

/// <summary>One column of a client report: the property name, its friendly header, and its value kind.</summary>
public sealed record ReportColumn(string Name, string Header, ReportColumnKind Kind);

/// <summary>
/// A client report ready for display or export: title, typed columns, and rows
/// of raw values in column order. Replaces the DataGridView's auto-generated
/// columns so the desktop grid, the web table, and the Excel export all agree.
/// </summary>
public sealed class ReportTable
{
    public string Title { get; init; } = string.Empty;

    public IReadOnlyList<ReportColumn> Columns { get; init; } = [];

    public IReadOnlyList<object?[]> Rows { get; init; } = [];

    public bool IsEmpty => Rows.Count == 0;
}

/// <summary>
/// The choices made on the reports screen. Null years mean "career"; a single
/// year has StartYear == EndYear; TopN null means show every row.
/// </summary>
public sealed record ReportRequest(
    ReportScope Scope,
    string Category,
    int? MemberNumber,
    int? StartYear,
    int? EndYear,
    int? TopN,
    bool IncludeSidePots,
    bool IncludeImported);
