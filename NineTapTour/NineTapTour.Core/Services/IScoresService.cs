#nullable disable
using NineTapTour.Core.Entities;
using NineTapTour.Core.Models;
using System.Collections.Generic;
using static NineTapTour.Core.Calculations.ReportHelper;

namespace NineTapTour.Core.Services;

/// <summary>
/// Headless score-entry hub logic extracted from FrmMemberScores. The form reads its
/// controls into plain values, this service performs the lookups, calculations, and
/// persistence orchestration, and the form writes the results back to its controls.
/// </summary>
public interface IScoresService
{
    /// <summary>
    /// Builds the high-game and high-series leaderboards for a tournament, filtered
    /// by a single squad (1-8), by the multi-squad filter list, or unfiltered (0),
    /// and ordered according to the selected report type.
    /// </summary>
    LeaderboardResult GetTournamentLeaderboards(int tournamentId, bool isThreeOfFourTournament,
        ReportType reportType, int qualifyBySquadNumber, IReadOnlyList<int> filterSquads);

    /// <summary>
    /// Returns the high-game member scores for a tournament, sorted by score descending.
    /// </summary>
    List<MemberScores> GetGameScores(int tournamentId);

    /// <summary>
    /// Returns the series standings for a tournament: selects the correct standings
    /// query for scratch/handicap, 3-of-4, and squad filtering, combines doubles
    /// partners into team rows for doubles tournaments, and sorts by score descending.
    /// A squad list containing 0 means "all squads".
    /// </summary>
    List<MemberScores> GetSeriesStandings(int tournamentId, bool isThreeOfFourTournament,
        bool isDoubles, bool useHandicap, bool useScratch, List<int> squadList);

    /// <summary>
    /// Adds or updates a bowler's score entry in a tournament squad: resolves the
    /// existing game/participant ids, carries the handicap and bonus forward from the
    /// existing entry or the member's most recent average, saves the entry, and
    /// updates the member's last-bowled date.
    /// </summary>
    ScoreEntryResult SaveScoreEntry(ScoreEntryRequest request);
}
