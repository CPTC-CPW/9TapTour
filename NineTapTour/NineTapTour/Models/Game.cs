using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Linq.Mapping;

namespace NineTapTour.Models
{
    public class Game
    {
        [Key]
        public int Id { get; set; }
        public int? InputtedAvg { get; set; }
        public int? Game1 { get; set; }
        public int? Game2 { get; set; }
        public int? Game3 { get; set; }
        public int? Game4 { get; set; }

        //Default as true, This would be used to calculate bonus pins, score, 
        //placestandings and especially repopulate Finalize tournament form.
        // do we use bit or bool?
        [DefaultValue(true)]
        public bool? UseGame1 { get; set; }
        [DefaultValue(true)]
        public bool? UseGame2 { get; set; }
        [DefaultValue(true)]
        public bool? UseGame3 { get; set; }
        [DefaultValue(true)]
        public bool? UseGame4 { get; set; }
       // We currently dont have a notes field, do we want one here?
        public string Notes { get; set; } 

        public int? Handicap { get; set; }
        public int? Bonus { get; set; }

        [Column(Expression = "Game1 + Game2 + Game3 + Game4")]
        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public int? TotalScore { get; set; }
        public decimal? MoneyWon { get; set; }
        public decimal? SidePot { get; set; }
        public byte? PlaceStanding { get; set;}

        public int GameRegionID { get; set; }

        /// <summary>
        /// If true, the player bowls for free. Comp players 
        /// bowl free of charge for helping with the tournament.
        /// </summary>
        [DefaultValue(false)]
        public bool IsComp { get; set; } 

        /// <summary>
        /// Returns a list of all games the bowler has played in the tournament entry.
        /// </summary>
        /// <returns></returns>
        public List<int?> AllGameScores()
        {
            var newList = new List<int?> {Game1, Game2, Game3, Game4};
            return newList;
        }
    }
}