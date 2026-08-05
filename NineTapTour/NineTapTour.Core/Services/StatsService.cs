#nullable disable
using Microsoft.EntityFrameworkCore;
using NineTapTour.Core.Data;
using NineTapTour.Core.Models;
using NineTapTour.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NineTapTour.Core.Services;

/// <summary>
/// Headless per-member and per-tournament statistics logic. The queries and math were
/// moved verbatim from FrmStats (tableview, populateStats, FrmStats_Load) and
/// FrmTournamentStats (M7.5); the pure computations are public statics so they can be
/// characterization-tested without a database.
/// </summary>
public class StatsService : IStatsService
{
    // Size of the recent-entries window shown in the FrmStats summary boxes
    private const int Last30Window = 30;

    private readonly IPlayerHistoryRepository playerHistoryRepository;
    private readonly ITournamentStatsRepository tournamentStatsRepository;
    private readonly IDbContextFactory<NineTapDb> dbFactory;

    public StatsService(
        IPlayerHistoryRepository playerHistoryRepository,
        ITournamentStatsRepository tournamentStatsRepository,
        IDbContextFactory<NineTapDb> dbFactory)
    {
        this.playerHistoryRepository = playerHistoryRepository;
        this.tournamentStatsRepository = tournamentStatsRepository;
        this.dbFactory = dbFactory;
    }

    /// <summary>
    /// Per-entry computed values for the all-time stat averages. Moved verbatim from
    /// the statHolder struct in FrmStats, keeping only the fields the averages use.
    /// </summary>
    private struct StatHolder
    {
        public StatHolder(int? Game1, int? Game2, int? Game3, int? Game4, int? Handicap, int? Bonus)
        {
            this.Game1 = Game1;
            this.Game2 = Game2;
            this.Game3 = Game3;
            this.Game4 = Game4;

            ScratchTotal = ((Game1.HasValue ? Game1 : 0) + (Game2.HasValue ? Game2 : 0) + (Game3.HasValue ? Game3 : 0) + (Game4.HasValue ? Game4 : 0));

            GameTotal = (((Game1.HasValue ? Game1 : 0) + (Handicap + Bonus)) + ((Game2.HasValue ? Game2 : 0) + (Handicap + Bonus)) + ((Game3.HasValue ? Game3 : 0) + (Handicap + Bonus)) + ((Game4.HasValue ? Game4 : 0) + (Handicap + Bonus)));

            AvgPerGame = ((Game1.HasValue ? Game1 : 0) + (Game2.HasValue ? Game2 : 0) + (Game3.HasValue ? Game3 : 0) + (Game4.HasValue ? Game4 : 0));

            int div = ((Game1.HasValue ? 1 : 0) + (Game2.HasValue ? 1 : 0) + (Game3.HasValue ? 1 : 0) + (Game4.HasValue ? 1 : 0));

            if (div != 0)
            {
                AvgPerGame /= div;
            }

            this.Handicap = Handicap;
            this.Bonus = Bonus;
        }

        public int? Game1;
        public int? Game2;
        public int? Game3;
        public int? Game4;
        public int? ScratchTotal;
        public int? GameTotal;
        public int? AvgPerGame;
        public int? Handicap;
        public int? Bonus;
    }

    public MemberStatsResult GetMemberStats(int memberNum)
    {
        using var db = dbFactory.CreateDbContext();

        var temp = (from p in db.Participants
                    join m in db.Members on p.Member.Id equals m.Id
                    join g in db.Games on p.Game.Id equals g.Id
                    join t in db.Tournaments on p.Tournament.Id equals t.Id
                    where m.Number == memberNum
                       && g.IsFinalized
                    orderby t.Date descending, g.Id descending
                    select new
                    {
                        GameID = g.Id,
                        GamesPlayed = g.GamesPlayed,
                        TournamentDate = t.Date,
                        Game1 = g.Game1 ?? 0,
                        Game2 = g.Game2 ?? 0,
                        Game3 = g.Game3 ?? 0,
                        Game4 = g.Game4 ?? 0,
                        ScratchTotal = g.ScratchTotal,
                        TotalScore = g.HandicapTotal,
                        trueAVG = g.LeagueAverage,
                        AVG = g.AdjustedAvg,
                        HandiCap = g.Handicap ?? 0,
                        Bonus = g.Bonus ?? 0,
                        MoneyWon = g.MoneyWon ?? 0,
                        PPHG = g.PlaceStanding, // Don't ToString() in LINQ expression
                        Notes = g.Notes
                    }).ToList(); // Materialize first, then transform

        List<FinalizedGameEntry> games = temp.Select(x => new FinalizedGameEntry(
            x.GameID, x.GamesPlayed, x.TournamentDate,
            x.Game1, x.Game2, x.Game3, x.Game4,
            x.ScratchTotal, x.TotalScore, x.trueAVG, x.AVG,
            x.HandiCap, x.Bonus, x.MoneyWon, x.PPHG, x.Notes)).ToList();

        return new MemberStatsResult(ShapeMemberStatsRows(games), playerHistoryRepository.GetTotalMoneyWon(memberNum));
    }

    /// <summary>
    /// Shapes raw finalized game entries into display-ready grid rows: entries with no
    /// games played are dropped, zero game scores and a zero adjusted average become
    /// null (empty cells), and the place standing becomes a string.
    /// </summary>
    public static List<MemberStatsRow> ShapeMemberStatsRows(IEnumerable<FinalizedGameEntry> games)
    {
        List<MemberStatsRow> rows = [];

        foreach (FinalizedGameEntry item in games)
        {
            #region display_fix_till_more_perm_fix_in_importation
            if (item.GamesPlayed == 0)
            {
                continue;
            }
            // some entries in the imported excel files have 0 gamesTotal and no relevant data to be
            // imported which would cause crash when displaying
            #endregion

            rows.Add(new MemberStatsRow(
                item.GamesPlayed,
                item.TournamentDate,
                item.Game1 == 0 ? null : item.Game1,
                item.Game2 == 0 ? null : item.Game2,
                item.Game3 == 0 ? null : item.Game3,
                item.Game4 == 0 ? null : item.Game4,
                item.ScratchTotal,
                item.HandicapTotal,
                item.LeagueAverage,
                item.AdjustedAvg == 0 ? null : item.AdjustedAvg,
                item.Handicap,
                item.Bonus,
                item.MoneyWon,
                item.PlaceStanding?.ToString() ?? "", // Convert after materializing
                item.Notes,
                item.GameId));
        }

        return rows;
    }

    public MemberStatAverages GetMemberStatAverages(int memberNum)
    {
        using var db = dbFactory.CreateDbContext();

        var temp = (from p in db.Participants
                    join m in db.Members on p.Member.Id equals m.Id
                    join g in db.Games on p.Game.Id equals g.Id
                    join t in db.Tournaments on p.Tournament.Id equals t.Id
                    where memberNum == p.Member.Number
                    orderby t.Date descending
                    select new
                    {
                        g.Game1,
                        g.Game2,
                        g.Game3,
                        g.Game4,
                        g.Handicap,
                        g.Bonus
                    }).ToList();

        return ComputeMemberStatAverages(temp.Select(x =>
            new StatGameEntry(x.Game1, x.Game2, x.Game3, x.Game4, x.Handicap, x.Bonus)).ToList());
    }

    /// <summary>
    /// Computes the all-time stat averages exactly as FrmStats.populateStats did:
    /// every entry counts toward every average (null values count as 0), and the
    /// results are NaN when there are no entries.
    /// </summary>
    public static MemberStatAverages ComputeMemberStatAverages(IReadOnlyList<StatGameEntry> entries)
    {
        List<StatHolder> stats = [];

        for (int i = 0; i < entries.Count; i++)
        {
            stats.Add(new StatHolder(
                entries[i].Game1,
                entries[i].Game2,
                entries[i].Game3,
                entries[i].Game4,
                entries[i].Handicap,
                entries[i].Bonus));
        }

        double sum = 0;
        double count = 0;
        #region Game 1 Average

        for (int i = 0; i < stats.Count; i++)
        {
            count++;
            sum += Convert.ToInt32(stats[i].Game1);
        }

        double game1Average = sum / count;
        #endregion
        #region Game 2 Average
        sum = 0;
        count = 0;

        for (int i = 0; i < stats.Count; i++)
        {
            count++;
            sum += Convert.ToInt32(stats[i].Game2);
        }

        double game2Average = sum / count;
        #endregion
        #region Game 3 Average
        sum = 0;
        count = 0;

        for (int i = 0; i < stats.Count; i++)
        {
            count++;
            sum += Convert.ToInt32(stats[i].Game3);
        }

        double game3Average = sum / count;
        #endregion
        #region Game 4 Average
        sum = 0;
        count = 0;

        for (int i = 0; i < stats.Count; i++)
        {
            count++;
            sum += Convert.ToInt32(stats[i].Game4);
        }

        double game4Average = sum / count;
        #endregion
        #region Scratch Total Average
        sum = 0;
        count = 0;

        for (int i = 0; i < stats.Count; i++)
        {
            count++;
            sum += Convert.ToInt32(stats[i].ScratchTotal);
        }

        double scratchTotalAverage = sum / count;
        #endregion
        #region Game Total Average
        sum = 0;
        count = 0;

        for (int i = 0; i < stats.Count; i++)
        {
            count++;
            sum += Convert.ToInt32(stats[i].GameTotal);
        }

        double gameTotalAverage = sum / count;
        #endregion
        #region Average Game Score
        sum = 0;

        foreach (StatHolder item in stats)
        {
            sum += Convert.ToDouble(item.AvgPerGame);
        }

        double averagePerGame = sum / stats.Count;
        #endregion
        #region Handicap Average
        sum = 0;
        count = 0;

        for (int i = 0; i < stats.Count; i++)
        {
            count++;
            sum += Convert.ToInt32(stats[i].Handicap);
        }

        double handicapAverage = sum / count;
        #endregion
        #region Bonus Pins Average
        sum = 0;
        count = 0;

        for (int i = 0; i < stats.Count; i++)
        {
            count++;
            sum += Convert.ToInt32(stats[i].Bonus);
        }

        double bonusAverage = sum / count;
        #endregion

        return new MemberStatAverages(
            game1Average, game2Average, game3Average, game4Average,
            scratchTotalAverage, gameTotalAverage, averagePerGame,
            handicapAverage, bonusAverage);
    }

    public Last30Averages GetLast30Averages(int memberNum)
    {
        List<PlayerHistoryViewModel> last30 = playerHistoryRepository.GetMemberPlayerHistory(memberNum).Take(Last30Window).ToList();
        return ComputeLast30Averages(last30);
    }

    /// <summary>
    /// Computes the recent-entries summary exactly as FrmStats_Load did, including its
    /// quirks: GameTotal keeps only the value from the last (oldest) entry, and the
    /// null-coalescing precedence means handicap and bonus never actually contribute
    /// to it for played games. Preserved verbatim for identical behavior.
    /// </summary>
    public static Last30Averages ComputeLast30Averages(IReadOnlyList<PlayerHistoryViewModel> last30)
    {
        int game1AVG = 0;
        int game2AVG = 0;
        int game3AVG = 0;
        int game4AVG = 0;
        int scratchTotal = 0;
        int gameTotal = 0;

        if (last30.Count > 0)
        {
            for (int i = 0; i < last30.Count; i++)
            {
                game1AVG += last30[i].Game1 ?? 0;
                game2AVG += last30[i].Game2 ?? 0;
                game3AVG += last30[i].Game3 ?? 0;
                game4AVG += last30[i].Game4 ?? 0;
                scratchTotal += (last30[i].Game1 ?? 0) + (last30[i].Game2 ?? 0) + (last30[i].Game3 ?? 0) + (last30[i].Game4 ?? 0);
                int total = (last30[i].Game1 != null) ? (last30[i].Game1 ?? 0 + last30[i].HandiCap + last30[i].Bonus) : 0;
                total += (last30[i].Game2 != null) ? (last30[i].Game2 ?? 0 + last30[i].HandiCap + last30[i].Bonus) : 0;
                total += (last30[i].Game3 != null) ? (last30[i].Game3 ?? 0 + last30[i].HandiCap + last30[i].Bonus) : 0;
                total += (last30[i].Game4 != null) ? (last30[i].Game4 ?? 0 + last30[i].HandiCap + last30[i].Bonus) : 0;
                gameTotal = total;
            }
            game1AVG /= last30.Count;
            game2AVG /= last30.Count;
            game3AVG /= last30.Count;
            game4AVG /= last30.Count;
        }

        return new Last30Averages(game1AVG, game2AVG, game3AVG, game4AVG, scratchTotal, gameTotal, last30.Count);
    }

    public List<TournamentStatsList> GetTournamentStats(int tournamentId, bool threeOutOf4)
    {
        return threeOutOf4
            ? tournamentStatsRepository.Get3OutOf4TournamentStatsList(tournamentId)
            : tournamentStatsRepository.GetTournamentStatsList(tournamentId);
    }
}
