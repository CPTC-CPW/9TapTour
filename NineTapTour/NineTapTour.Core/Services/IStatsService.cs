#nullable disable
using NineTapTour.Core.Models;
using System.Collections.Generic;

namespace NineTapTour.Core.Services;

/// <summary>
/// Headless per-member and per-tournament statistics logic extracted from FrmStats
/// and FrmTournamentStats (M7.5). The forms bind the results to their grids and
/// text boxes; this service performs the queries and the math.
/// </summary>
public interface IStatsService
{
    /// <summary>
    /// Returns the display-ready rows for the member stats grid (finalized games,
    /// newest tournament first) along with the member's total money won.
    /// </summary>
    MemberStatsResult GetMemberStats(int memberNum);

    /// <summary>
    /// Returns the all-time stat averages for a member across every tournament entry
    /// (finalized or not), one value per FrmStats summary text box.
    /// </summary>
    MemberStatAverages GetMemberStatAverages(int memberNum);

    /// <summary>
    /// Returns the summary values over the member's most recent (up to 30)
    /// finalized entries for the FrmStats summary text boxes.
    /// </summary>
    Last30Averages GetLast30Averages(int memberNum);

    /// <summary>
    /// Returns the stats list for a tournament, using the 3-out-of-4 variant of the
    /// query when the tournament is a 3-out-of-4 tournament.
    /// </summary>
    List<TournamentStatsList> GetTournamentStats(int tournamentId, bool threeOutOf4);
}
