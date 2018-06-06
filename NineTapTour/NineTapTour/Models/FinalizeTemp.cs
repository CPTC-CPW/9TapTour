using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace NineTapTour.Models
{
    public class FinalizeTemp
    {
        [Key]
        public int FinalizeID { get; set; }
        public int TournamentID { get; set; }
        public int GameId { get; set; }
        public int MemberId { get; set; }
        public int memberNumber { get; set; }
        public String FirstName { get; set; }
        public String LastName { get; set; }
        public int Squad { get; set; }
        public int Game1 { get; set; }
        public int Game2 { get; set; }
        public int Game3 { get; set; }
        public int Game4 { get; set; }
        [DefaultValue(true)]
        public bool UseGame1 { get; set; }
        [DefaultValue(true)]
        public bool UseGame2 { get; set; }
        [DefaultValue(true)]
        public bool UseGame3 { get; set; }
        [DefaultValue(true)]
        public bool UseGame4 { get; set; }
        public int LeagueAverage { get; set; }
        public int AdjustedAvg { get; set; }
        public string Notes { get; set; }
        public int ScratchTotal { get; set; }
        [DefaultValue(false)]
        public bool KeepAdjustedAvg { get; set; }
        public int GameAvg { get; set; }
        public int Handicap { get; set; }
        public int Bonus { get; set; }

        public int HandicapTotal { get; set; }

        public int FinalizeRegionID { get; set; }
    }
}
