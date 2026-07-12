namespace NineTapTour.Models
{
    public class ExcelMember
    {
        public int PlaceStanding { get; set; }
        public int MemberNumber { get; set; }
        public string Name { get; set; }
        public int Handicap { get; set; }
        public int Bonus { get; set; }
        public decimal? MoneyWon { get; set; }
        public int GameId { get; set; }
        public int Game1Score { get; set; }
        public int Game2Score { get; set; }
        public int Game3Score { get; set; }
        public int Game4Score { get; set; }
        public int TotalScore { get; set; }
        public decimal? SidePot { get; set; }
    }
}
