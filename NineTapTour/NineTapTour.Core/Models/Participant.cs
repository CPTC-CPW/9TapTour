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
            if (other is null)
            {
                return 1;
            }

            int thisSum = this.Game.AllGameScores().Where(sc => sc.HasValue).Sum() ?? 0;
            int otherSum = other.Game.AllGameScores().Where(osc => osc.HasValue).Sum() ?? 0;
            return thisSum.CompareTo(otherSum);
        }

        public bool Equals(Participant other)
        {
            return other is not null && Id == other.Id;
        }

        public override bool Equals(object obj) => Equals(obj as Participant);

        public override int GetHashCode() => Id.GetHashCode();

        public override string ToString()
        {
            return $"{Member.LastName}, {Member.FirstName}: Squad {Squad}";
        }
    }
}