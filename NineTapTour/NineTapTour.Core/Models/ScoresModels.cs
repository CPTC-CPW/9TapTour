#nullable disable
using NineTapTour.Core.Entities;
using NineTapTour.Core.ViewModels;
using System.Collections.Generic;

namespace NineTapTour.Core.Models;

/// <summary>
/// The two leaderboard lists shown in the FrmMemberScores high-score listbox:
/// one top game per participant per squad, and the series (all games) totals.
/// </summary>
public record LeaderboardResult(
    List<ParticipantsGameViewModel> ParticipantsGameScores,
    List<TopParticipantGameViewModel> Top3Scores);

/// <summary>
/// Captured control input for adding or updating a bowler's score entry in a
/// tournament squad. Built by the form from its textboxes; contains no UI types.
/// </summary>
public record ScoreEntryRequest(
    int TournamentId,
    int MemberNumber,
    int Squad,
    int? Game1,
    int? Game2,
    int? Game3,
    int? Game4,
    decimal MoneyWon,
    bool IsComp);

/// <summary>
/// Result of persisting a score entry. Member and Tournament are the entities the
/// service loaded so the form can reuse them without re-querying.
/// </summary>
public record ScoreEntryResult(bool Success, string ErrorMessage, Member Member, Tournament Tournament);

/// <summary>
/// Scratch and handicap series totals for a single bowler's entry.
/// </summary>
public record ScoreTotals(int ScratchTotal, int HandicapTotal);
