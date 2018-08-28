using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineTapTour.Models.ViewModels
{
    class TopParticipantGameViewModel
    {
        public TopParticipantGameViewModel(int memberID, string firstName, string lastName, int placing, int scratchTotal,

            int? top3ScratchScore, int? top3HandiScores, int? game1, int? game2, int? game3, int? game4, int? handicap, int bonus, int gameID, int squad)//todo: change static numbers
        {
            this.memberID = memberID;
            FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
            LastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
            Placing = placing;
            ScratchTotal = scratchTotal;

            Top3ScratchScore = top3ScratchScore ?? throw new ArgumentNullException(nameof(top3ScratchScore));
            Top3HandiScores = top3HandiScores ?? throw new ArgumentNullException(nameof(top3HandiScores));
            Game1 = game1;
            Game2 = game2;
            Game3 = game3;
            Game4 = game4;
            Handicap = handicap ?? throw new ArgumentNullException(nameof(handicap));
            Bonus = bonus;
            GameID = gameID;
            Squad = squad;
            HandicapScore = this.ScratchTotal + (this.Handicap * 4) + (this.Bonus * 4); //TODO: implement flexibile num on ( 4 )
            SetHandicapTotalToString();
            SetScratchTotalToString();
        }

        #region Properties

        public int memberID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Placing { get; set; }
        public int ScratchTotal { get; set; }
        public int HandicapScore { get; set; }
        public int? Top3ScratchScore { get; set; }
        public int? Top3HandiScores { get; set; }
        public int? Game1 { get; set; }
        public int? Game2 { get; set; }
        public int? Game3 { get; set; }
        public int? Game4 { get; set; }
        public int Handicap { get; set; }
        public int Bonus { get; set; }
        public int GameID { get; set; }
        public int Squad { get; set; }
        public string ScratchTotalToString { get; set; }
        public string HandicapTotalToString { get; set; }      
        #endregion

        public void SetScratchTotalToString()
        {
            this.ScratchTotalToString = $"{this.ScratchTotal,-10} {$"[{this.memberID}]",-16} {this.FirstName} {this.LastName}";
        }

        public void SetHandicapTotalToString()
        {
            this.HandicapTotalToString = $"{this.HandicapScore, -10} {$"[{this.memberID}]",-16} {this.FirstName} {this.LastName}";
        }
    }
}
