using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Linq.Mapping;
using System.Data.Entity;


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
        public int? Handicap { get; set; }
        public int? Bonus { get; set; }
        [Column(Expression = "Game1 + Game2 + Game3 + Game4")]
        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public int? TotalScore { get; set; }
        public decimal MoneyWon { get; set; }
        public byte PlaceStanding { get; set;}

    }
}