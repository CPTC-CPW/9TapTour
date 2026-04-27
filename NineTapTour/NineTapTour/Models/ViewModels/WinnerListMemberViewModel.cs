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

        /// <summary>Director-assigned adjusted average persisted from a prior session.</summary>
        public int AdjustedAvg { get; set; }

        /// <summary>
        /// Nullable so we can distinguish "explicitly set" from "never touched".
        /// When null, LoadTournamentGrid falls back to Game1.HasValue (first-open default).
        /// </summary>
        public bool? UseGame1 { get; set; }
        public bool? UseGame2 { get; set; }
        public bool? UseGame3 { get; set; }
        public bool? UseGame4 { get; set; }

        /// <summary>Maps to the Director Check checkbox — persisted as Game.KeepAdjustedAvg.</summary>
        public bool KeepAdjustedAvg { get; set; }

        public int Squad { get; set; }
    }
}
