using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NineTapTour.Abstractions;
using NineTapTour.Calculations;
using NineTapTour.Database;   // NineTapDb lives here (in the Data assembly)
using NineTapTour.Models;

namespace NineTapTour.Data.Repositories
{
    public sealed class TournamentStatsRepository : ITournamentStatsRepository
    {
        private readonly IDbContextFactory<NineTapDb> _factory;
        public TournamentStatsRepository(IDbContextFactory<NineTapDb> factory) => _factory = factory;

        /// <summary>
        /// Grabs and returns a list of TournamentStatsList from the database
        /// </summary>
        /// <param name="selectedTournament">The Id of the tournament</param>
        public List<TournamentStatsList> GetTournamentStatsList(int selectedTournament)
        {
            using (NineTapDb db = _factory.CreateDbContext())
            {
                return [.. (from p in db.Participants
                    join m in db.Members on p.Member.Id equals m.Id
                    join g in db.Games on p.Game.Id equals g.Id
                    join t in db.Tournaments on p.Tournament.Id equals t.Id
                    where t.Id == selectedTournament
                    orderby (g.Game1 + g.Game2 + g.Game3 + g.Game4) descending
                    select new TournamentStatsList
                    {
                        Id = p.Member.Number,
                        FirstName = p.Member.FirstName,
                        LastName = p.Member.LastName,
                        Squad = p.Squad,
                        ScratchTotal = (
                            (g.Game1.HasValue ? g.Game1 : 0) +
                            (g.Game2.HasValue ? g.Game2 : 0) +
                            (g.Game3.HasValue ? g.Game3 : 0) +
                            (g.Game4.HasValue ? g.Game4 : 0)
                        ),
                        Top3Scores = (
                            // Scratch Total
                            (g.Game1.HasValue ? g.Game1 : 0) +
                            (g.Game2.HasValue ? g.Game2 : 0) +
                            (g.Game3.HasValue ? g.Game3 : 0) +
                            (g.Game4.HasValue ? g.Game4 : 0)) +
                            // Handicap
                            (p.Game.Handicap * 3) +
                            // Bonus
                            (p.Game.Bonus * 3),

                        /* It dosent even use game total, why does this exist
                        GameTotal = (
                            ((g.Game1.HasValue ? g.Game1 : 0) + (g.Handicap + g.Bonus)) +
                            ((g.Game2.HasValue ? g.Game2 : 0) + (g.Handicap + g.Bonus)) +
                            ((g.Game3.HasValue ? g.Game3 : 0) + (g.Handicap + g.Bonus)) +
                            ((g.Game4.HasValue ? g.Game4 : 0) + (g.Handicap + g.Bonus))
                        ),
                        */
                        Game1 = g.Game1,
                        Game2 = g.Game2,
                        Game3 = g.Game3,
                        Game4 = g.Game4,
                        Handicap = p.Game.Handicap,
                        Bonus = p.Game.Bonus,
                    }).AsNoTracking()];
            }
        }

        /// <summary>
        /// Grabs and returns a list of TournamentStatsList from the database,
        /// sets the Top3Scores to the top 3 out of 4
        /// </summary>
        /// <param name="selectedTournament">The Id of the tournament</param>
        public List<TournamentStatsList> Get3OutOf4TournamentStatsList(int selectedTournament)
        {
            // query database
            using (NineTapDb db = _factory.CreateDbContext())
            {
                return [.. (from p in db.Participants
                     join m in db.Members on p.Member.Id equals m.Id
                     join g in db.Games on p.Game.Id equals g.Id
                     join t in db.Tournaments on p.Tournament.Id equals t.Id
                     where t.Id == selectedTournament
                     orderby (g.Game1 + g.Game2 + g.Game3 + g.Game4) descending
                     select new TournamentStatsList
                     {
                         Id = p.Member.Number,
                         FirstName = p.Member.FirstName,
                         LastName = p.Member.LastName,
                         Squad = p.Squad,
                         ScratchTotal = (
                                 (g.Game1.HasValue ? g.Game1 : 0) +
                                 (g.Game2.HasValue ? g.Game2 : 0) +
                                 (g.Game3.HasValue ? g.Game3 : 0) +
                                 (g.Game4.HasValue ? g.Game4 : 0)
                             ),
                         Top3Scores = NineTapTour.Calculations.TournamentStatsCalculator.GetTop3OutOf4(g.Game1, g.Game2, g.Game3, g.Game4).Sum() +
                                 // Handicap
                                 (p.Game.Handicap * 3) +
                                 // Bonus
                                 (p.Game.Bonus * 3),
                         Game1 = g.Game1,
                         Game2 = g.Game2,
                         Game3 = g.Game3,
                         Game4 = g.Game4,
                         Handicap = p.Game.Handicap,
                         Bonus = p.Game.Bonus,
                     }).AsNoTracking()];
            }
        }
    }
}
