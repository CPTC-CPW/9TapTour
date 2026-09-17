#nullable disable
using NineTapTour.Core.Models;
using System.Collections.Generic;

namespace NineTapTour.Core.Services;

/// <summary>
/// Headless winners-list and payout building logic extracted from FrmTournamentResults.
/// The form gathers grid values, this service performs the lookups and calculations,
/// and the form writes the results back to the grid.
/// </summary>
public interface IWinnersService
{
    /// <summary>
    /// Builds the full winners list for a tournament (singles or doubles), scoring each
    /// entry with the Member record's current handicap while the tournament is open
    /// (the game's own snapshot once finalized) and computing handicap total scores.
    /// </summary>
    WinnersListResult BuildWinnersList(WinnersListRequest request);

    /// <summary>
    /// Looks up a member and their best game entry in the given tournament across all
    /// squads for 2-day grid auto-fill. Handicap is the Member record's current handicap
    /// while the tournament is open (the game's own snapshot once finalized); bonus
    /// always comes from the Member record.
    /// </summary>
    TwoDayAutoFillResult AutoFillTwoDayMember(int memberNumber, int tournamentId);

    /// <summary>
    /// Returns, for each member number, whether the membership is current: lifetime
    /// members are always current, otherwise the last payment year + 1 must be at
    /// least the current year.
    /// </summary>
    Dictionary<int, bool> GetMembershipCurrentByMemberNumber(IReadOnlyCollection<int> memberNumbers);
}
