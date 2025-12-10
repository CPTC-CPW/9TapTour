using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NineTapTour.Models
{
    public class Game
    {
        [Key]
        public int Id { get; set; }
        
        public int? Game1 { get; set; }
        public int? Game2 { get; set; }
        public int? Game3 { get; set; }
        public int? Game4 { get; set; }

        //Default as true, This would be used to calculate bonus pins, score, 
        //placestandings and especially repopulate Finalize tournament form.
        [DefaultValue(true)]
        public bool? UseGame1 { get; set; }
        [DefaultValue(true)]
        public bool? UseGame2 { get; set; }
        [DefaultValue(true)]
        public bool? UseGame3 { get; set; }
        [DefaultValue(true)]
        public bool? UseGame4 { get; set; }

        public string Notes { get; set; }

        /// <summary>
        /// The average for the 4 games entered for this participant in this tournament. Games are only
        /// counted if they are marked as "used" (UseGameX = true).
        /// </summary>
        [NotMapped]
        public double AverageForEntry
        {
            get
            {
                int gamesPlayed = GamesPlayed;
                if (gamesPlayed == 0)
                    return 0;
                return (double)ScratchTotal / gamesPlayed;
            }
        }

        public int? Handicap { get; set; }
        public int? Bonus { get; set; }

        [NotMapped]
        public int? TotalScore { get; set; }
        
        /// <summary>
        /// Calculated property: Sum of all games that are marked as "used".
        /// If UseGame flags are null, defaults to true (includes the game).
        /// </summary>
        [NotMapped]
        public int ScratchTotal
        {
            get
            {
                int total = 0;
                
                if ((UseGame1 ?? true) && Game1.HasValue)
                    total += Game1.Value;
                    
                if ((UseGame2 ?? true) && Game2.HasValue)
                    total += Game2.Value;
                    
                if ((UseGame3 ?? true) && Game3.HasValue)
                    total += Game3.Value;
                    
                if ((UseGame4 ?? true) && Game4.HasValue)
                    total += Game4.Value;
                
                return total;
            }
        }
        
        /// <summary>
        /// Calculated property: Count of games that are marked as "used" and have values.
        /// If UseGame flags are null, defaults to true (includes the game).
        /// </summary>
        [NotMapped]
        public int GamesPlayed
        {
            get
            {
                int count = 0;
                
                if ((UseGame1 ?? true) && Game1.HasValue)
                    count++;
                    
                if ((UseGame2 ?? true) && Game2.HasValue)
                    count++;
                    
                if ((UseGame3 ?? true) && Game3.HasValue)
                    count++;
                    
                if ((UseGame4 ?? true) && Game4.HasValue)
                    count++;
                
                return count;
            }
        }
        
        public decimal? MoneyWon { get; set; }
        public decimal? SidePot { get; set; }
        public int? PlaceStanding { get; set;}

        [DefaultValue(false)]
        public bool IsComp { get; set; } // comp is someone who bowls for free because they are helping with tournament 

        // Properties migrated from FinalizeTemp
        [DefaultValue(false)]
        public bool IsFinalized { get; set; }

        #region Averages

        /// <summary>
        /// The League Average/True Average is the bowler's average for the last 30 games at this point in time.
        /// </summary>
        public double LeagueAverage { get; set; }

        /// <summary>
        /// The average adjusted for tournament conditions. The director assigns this average
        /// (Described as AVG in old tournament import)
        /// </summary>
        public int AdjustedAvg { get; set; }

        /// <summary>
        /// Used during tournament finalization to indicate whether to keep the adjusted average
        /// </summary>
        [DefaultValue(false)]
        public bool KeepAdjustedAvg { get; set; }
        #endregion

        /// <summary>
        /// Total for all game scores + handicap per game + bonus per game
        /// </summary>
        public int HandicapTotal { get; set; }

        public virtual Participant Participant { get; set; }

        public List<int?> AllGameScores()
        {
            var newList = new List<int?> {Game1, Game2, Game3, Game4};
            return newList;
        }
    }
}