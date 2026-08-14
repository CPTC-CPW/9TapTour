#nullable disable
using System.Collections.Generic;

namespace NineTapTour.Core.Models;

/// <summary>
/// Identifies the tournament whose winners list should be built, plus the
/// tournament-type flags that change how total scores are computed.
/// </summary>
public record WinnersListRequest(int TournamentId, bool Doubles, bool ThreeOutOf4);

/// <summary>
/// The built winners list plus the entry counts the form tracks:
/// <see cref="TotalEntries"/> is the number of entries across all squads and
/// <see cref="CompEntries"/> is the number of comped (free) entries.
/// </summary>
public record WinnersListResult(List<ExcelMember> Winners, int TotalEntries, int CompEntries);

/// <summary>
/// Outcome of a 2-day member auto-fill lookup. The form shows the matching
/// message for the non-success statuses.
/// </summary>
public enum TwoDayAutoFillStatus
{
    Success,
    MemberNotFound,
    GameNotFound
}

/// <summary>
/// Values to write into a 2-day grid row after looking up a member number:
/// the member's name, "hdcp + bonus" display, handicap total score, and the
/// hidden member/game keys.
/// </summary>
public record TwoDayAutoFillResult(
    TwoDayAutoFillStatus Status,
    string FullName,
    string HandicapDisplay,
    int TotalScore,
    int MemberNumber,
    int GameId);

/// <summary>
/// One doubles team reconstructed from the consecutive-pair winners list,
/// with its shared place and whether that place is tied with another team.
/// </summary>
public record DoublesTeamPairing(ExcelMember Member1, ExcelMember Member2, int Place, bool IsTie);
