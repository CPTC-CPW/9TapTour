#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineTapTour.Core.Models
{
    public class CurrentHistory
    {
        public int ScratchTotal { get; set; }

        public bool UseGame1 { get; set; }

        public bool UseGame2 { get; set; }

        public bool UseGame3 { get; set; }

        public bool UseGame4 { get; set; }

    }

    public class PreviousHistory
    {
        public DateTime TournamentDate { get; set; }

        public int GamesPlayed { get; set; }

        public int TotalScore { get; set; }
    }
}
