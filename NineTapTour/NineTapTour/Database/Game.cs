using System.ComponentModel.DataAnnotations;

namespace NineTapTour.Database
{
    public class Game
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int Number { get; set; }
        [Required]
        public int Score { get; set; }
    }
}