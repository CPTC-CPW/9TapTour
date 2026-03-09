using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace NineTapTour.Models
{
    public class Participant : IComparable<Participant>, IEquatable<Participant>
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Column("SquadNumber")]
        public int Squad { get; set; }
        #region Foreign Keys
        [Required]
        public Member Member { get; set; }

        [Required]
        public Game Game { get; set; }

        [Required]
        public Tournament Tournament { get; set; }
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

        public bool Equals(Participant other)
        {
            if(Id == other.Id)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override string ToString()
        {
            return $"{Member.LastName}, {Member.FirstName}: Squad {Squad}";
        }
    }
}