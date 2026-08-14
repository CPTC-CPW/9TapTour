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
    /// Builds the full winners list for a tournament (singles or doubles), applying
    /// each member's carry-forward handicap from their most recent finalized prior
    /// tournament and computing handicap total scores.
    /// </summary>
    WinnersListResult BuildWinnersList(WinnersListRequest request);

    /// <summary>
    /// Batch-queries the most recent finalized tournament prior to
    /// <paramref name="excludeTournamentId"/> for each member and returns the handicap
    /// computed from that entry's AdjustedAvg. Members with no qualifying prior entry
    /// are absent from the result.
    /// </summary>
    Dictionary<int, int> BuildPrevHdcpByMember(HashSet<int> memberNumbers, int excludeTournamentId);

    /// <summary>
    /// Looks up a member and their best game entry in the given tournament across all
    /// squads for 2-day grid auto-fill. Handicap is derived from the member's most
    /// recent finalized previous tournament (falling back to the Member record);
    /// bonus always comes from the Member record.
    /// </summary>
    TwoDayAutoFillResult AutoFillTwoDayMember(int memberNumber, int tournamentId);

    /// <summary>
    /// Returns, for each member number, whether the membership is current: lifetime
    /// members are always current, otherwise the last payment year + 1 must be at
    /// least the current year.
    /// </summary>
    Dictionary<int, bool> GetMembershipCurrentByMemberNumber(IReadOnlyCollection<int> memberNumbers);
}
