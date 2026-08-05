#nullable disable
using System;
using System.Collections.Generic;

namespace NineTapTour.Core.Models;

/// <summary>
/// One game entry fed into the all-time member stat averages computation
/// (moved from FrmStats.populateStats, M7.5).
/// </summary>
public record StatGameEntry(int? Game1, int? Game2, int? Game3, int? Game4, int? Handicap, int? Bonus);

/// <summary>
/// All-time stat averages for one member, one value per summary text box on FrmStats.
/// Values are NaN when the member has no entries, matching the original form math.
/// </summary>
public record MemberStatAverages(
    double Game1Average, double Game2Average, double Game3Average, double Game4Average,
    double ScratchTotalAverage, double GameTotalAverage, double AveragePerGame,
    double HandicapAverage, double BonusAverage);

/// <summary>
/// Raw finalized game entry queried for the member stats grid, before display shaping.
/// Null game scores are coalesced to 0 by the query, exactly like the original form query.
/// </summary>
public record FinalizedGameEntry(
    int GameId, int GamesPlayed, DateTime TournamentDate,
    int Game1, int Game2, int Game3, int Game4,
    int ScratchTotal, int HandicapTotal, double LeagueAverage, int AdjustedAvg,
    int Handicap, int Bonus, decimal MoneyWon, int? PlaceStanding, string Notes);

/// <summary>
/// One display-ready row of the member stats grid. Zero game scores and a zero
/// adjusted average are shaped to null so the grid shows empty cells.
/// </summary>
public record MemberStatsRow(
    int GamesPlayed, DateTime TournamentDate,
    int? Game1, int? Game2, int? Game3, int? Game4,
    int ScratchTotal, int HandicapTotal, double LeagueAverage, int? AdjustedAvg,
    int Handicap, int Bonus, decimal MoneyWon, string Place, string Notes, int GameId);

/// <summary>
/// Everything the member stats grid binds: the shaped rows plus the member's total
/// money won (displayed in the Money Won column header).
/// </summary>
public record MemberStatsResult(List<MemberStatsRow> Rows, decimal TotalMoneyWon);

/// <summary>
/// Summary values over a member's most recent (up to 30) finalized entries, shown in
/// the FrmStats summary text boxes. When <see cref="EntryCount"/> is 0 the form leaves
/// the boxes untouched, matching the original load logic.
/// </summary>
public record Last30Averages(
    int Game1Average, int Game2Average, int Game3Average, int Game4Average,
    int ScratchTotal, int GameTotal, int EntryCount);
