using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NineTapTour.Database
{
    public class Game
    {
        [Key]
        public int Id { get; set; }
        
        public int? Game1 { get; set; }
        public int? Game2 { get; set; }
        public int? Game3 { get; set; }
        public int? Game4 { get; set; }
        [Required]
        public List<int> Score { get; set; }
        [Required]
        public Member Member { get; set; }
        public int? Handicap { get; set; }
        public int? Bonus { get; set; }

    }
}