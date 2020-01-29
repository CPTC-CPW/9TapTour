using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NineTapTour.Models;

namespace NineTapTour.Database
{
    class TournamentStatsListDB
    {
        public static List<TournamentStatsList> GetTournamentStatsList(int selectedTournament)
        {
            using (NineTapDb db = new NineTapDb())
            {
                return (from p in db.Participants
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
                    }).ToList();
            }
        }
    }
}
