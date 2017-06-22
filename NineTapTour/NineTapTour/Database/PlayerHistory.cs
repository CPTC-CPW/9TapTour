using System;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace NineTapTour.Database
{
    public class PlayerHistory
    {
        [Key] public int hisID { get; set; }

        //required
        [Index("IX_MemberNumber", IsUnique = false)]
        public int MemberNumber { get; set; }
        public int GamesPlayed { get; set; }

        public DateTime TournamentDate { get; set; }

        public Game Game {get ; set; }

        public int ProPot { get; set; }

        public string PPHG { get; set; }


    }
}
