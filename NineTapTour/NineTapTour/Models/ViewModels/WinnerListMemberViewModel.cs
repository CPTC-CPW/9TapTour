using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineTapTour.Models.ViewModels
{
    /// <summary>
    /// Member, game, and participant data for a single bowler
    /// to build a Winners List for the Tournament
    /// </summary>
    public class WinnerListMemberViewModel
    {
        public int? PlaceStanding { get; set; }

        public int MemberNumber { get; set; }

        /// <summary>
        /// The first and last name of the bowler
        /// ex. John Doe
        /// </summary>
        public string BowlerName { get; set; }

        public int? Handicap { get; set; }

        public int? Bonus { get; set; }

        public decimal? MoneyWon { get; set; }

        public decimal? SidePot { get; set; }

        public int GameId { get; set; }

        public int? Game1 { get; set; }

        public int? Game2 { get; set; }

        public int? Game3 { get; set; }

        public int? Game4 { get; set; }

        public bool IsComp { get; set; }

        /// <summary>
        /// The bowler's 30-game league average at the time of the tournament entry.
        /// Used to detect sandbagging (score 40+ pins below average).
        /// </summary>
        public double LeagueAverage { get; set; }
    }
}
