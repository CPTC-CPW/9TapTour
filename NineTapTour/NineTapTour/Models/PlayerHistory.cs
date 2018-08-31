using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NineTapTour.Models
{
    public class PlayerHistory
    {
        [Key] public int hisID { get; set; }

        //required
        [Index("IX_MemberNumber", IsUnique = false)]
        public int MemberNumber { get; set; }
        public int GamesPlayed { get; set; }

        public DateTime TournamentDate { get; set; }

        public int GameID {get ; set; }


        public int? Game1 { get; set; }
        public int? Game2 { get; set; }
        public int? Game3 { get; set; }
        public int? Game4 { get; set; }


        public int TotalScore { get; set; }
        public int HandiCap { get; set; }

        public int Bonus { get; set; }

        public decimal MoneyWon { get; set; }

        public string Notes { get; set; }



        public double AverageForGame { get; set;}

        public double trueAVG { get; set; }

        public int AVG { get; set; }

        public string ProPot { get; set; }

        public string PPHG { get; set; }

        public int regionID { get; set; }


    }
}
