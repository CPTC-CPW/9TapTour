#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineTapTour.Core.ViewModels
{
    public class ParticipantsGameViewModel
    {
        public ParticipantsGameViewModel(int memberNo, string firstName, string lastName, int squad, int? highScore, int? handicap, int bonus)
        {
            this.MemberNo = memberNo;
            this.FirstName = firstName;
            this.LastName = lastName;
            this.Squad = squad;
            this.HighScore = highScore;
            this.Handicap = handicap;
            this.Bonus = bonus;
            SetScratchTotalToString();
            SetHandicapScoreToString();
        }

        public int MemberNo { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Squad { get; set; }
        public int? HighScore { get; set; }
        public int? Handicap { get; set; }
        public int Bonus { get; set; }
        public string ScratchScoreToString { get; set; }
        public string HandicapScoreToString { get; set; }

        public void SetScratchTotalToString()
        {
            ScratchScoreToString = $"{HighScore, -10} {$"[{MemberNo}]", -16}  {FirstName} {LastName}";
        }

        public void SetHandicapScoreToString()
        {
            int? handicapScore = HighScore + Handicap + Bonus;
            HandicapScoreToString = $"{handicapScore, -10} {$"[{MemberNo}]", -16} {FirstName} {LastName}";
        }
    }
}
