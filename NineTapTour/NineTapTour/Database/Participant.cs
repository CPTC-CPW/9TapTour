using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NineTapTour.Database
{
    public class Participant
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Column("SquadNumber")]
        public int Squad { get; set; }
        #region Foreign Keys
        [Required]
        public Member Member { get; set; }

        public Tournament Tournament { get; set; }
        [Required]
        public Game Game { get; set; }
        #endregion
    }
}