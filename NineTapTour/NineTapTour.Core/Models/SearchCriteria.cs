namespace NineTapTour.Core.Models;

/// <summary>
/// Member search filters from FrmSearch. Every filter is optional; text
/// filters are case-insensitive "contains" matches.
/// </summary>
public sealed record MemberSearchCriteria(
    int? Number = null,
    string? FirstName = null,
    string? LastName = null,
    bool? IsActive = null,
    int? Average = null,
    int? Handicap = null,
    int? Bonus = null);

/// <summary>
/// Tournament search filters from FrmTourSearch and FrmTournamentsByYear.
/// Dates are inclusive and compared by calendar day.
/// </summary>
public sealed record TournamentSearchCriteria(
    string? Location = null,
    string? Event = null,
    DateTime? From = null,
    DateTime? To = null,
    int? Year = null);
