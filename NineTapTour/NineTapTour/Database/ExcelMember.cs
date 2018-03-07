

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineTapTour.Database
{
    class ExcelMember
    {
        public int PlaceStanding { get; set; }
        public int MemberId { get; set; }
        public string Name { get; set; }
        public int Handicap { get; set; }
        public int Bonus { get; set; }
        public decimal? MoneyWon { get; set; }
        public int GameId { get; set; }
        public int Game1Score { get; set; }
        public int Game2Score { get; set; }
        public int Game3Score { get; set; }
        public int Game4Score { get; set; }
        public int TotalScore { get; set; }
    }
}
