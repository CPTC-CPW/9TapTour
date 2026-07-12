using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NineTapTour.Models;

namespace NineTapTour.Models.ViewModels
{
    public class TopParticipantGameViewModel
    {
        public TopParticipantGameViewModel(int memberNo, string firstName, string lastName, int placing, int scratchTotal,

            int? top3ScratchScore, int? top3HandiScores, int? game1, int? game2, int? game3, int? game4, int? handicap, int bonus, int gameID, int squad)//todo: change static numbers
        {
            this.MemberNo = memberNo;
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
            int numGamesTotal = 0;
            if (Game1.HasValue) numGamesTotal++;
            if (Game2.HasValue) numGamesTotal++;
            if (Game3.HasValue) numGamesTotal++;
            if (Game4.HasValue) numGamesTotal++;

            HandicapScore = this.ScratchTotal + (this.Handicap * numGamesTotal) + (this.Bonus * numGamesTotal);
        }

        #region Properties

        public int MemberNo { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Placing { get; set; }
        public int ScratchTotal { get; private set; }
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

        /// <summary>
        /// When true, display uses the best-3-of-4 totals. Set by the caller from the tournament
        /// so this view model does not depend on global UI state.
        /// </summary>
        public bool IsThreeOutOf4 { get; set; }

        public string ScratchTotalToString
        {
            get
            {
                if (this.IsThreeOutOf4)
                {
                    return $"{this.Top3ScratchScore,-10} {$"[{this.MemberNo}]",-16} {this.FirstName} {this.LastName}";
                }
                else
                {
                    return $"{this.ScratchTotal,-10} {$"[{this.MemberNo}]",-16} {this.FirstName} {this.LastName}";
                }
            }
        }

        public string HandicapTotalToString
        {
            get
            {
                if (this.IsThreeOutOf4)
                {
                    return $"{this.Top3HandiScores,-10} {$"[{this.MemberNo}]",-16} {this.FirstName} {this.LastName}";
                }
                else
                {
                    return $"{this.HandicapScore,-10} {$"[{this.MemberNo}]",-16} {this.FirstName} {this.LastName}";
                }
            }
        }      
        #endregion
    }
}
