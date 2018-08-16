using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace NineTapTour.Models
{
    public class Participant : IComparable<Participant>
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Column("SquadNumber")]
        public int Squad { get; set; }
        #region Foreign Keys
        [Required]
        public Member Member { get; set; }

        public Game Game { get; set; }
        public Tournament Tournament { get; set; }


        public int ParticipantRegionID { get; set; }
        #endregion


        public int CompareTo(Participant other)
        {
            if (this.Game.AllGameScores().Where(sc => sc.HasValue).Sum() > other.Game.AllGameScores().Where(osc => osc.HasValue).Sum())
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
    }
}