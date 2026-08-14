#nullable disable
namespace NineTapTour.Core.Models
{
    /// <summary>
    /// Object used to fill DataTable for Tournament Stats Form
    /// </summary>
    public class TournamentStatsList
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Squad { get; set; }
        public int? ScratchTotal { get; set; }
        public int? Top3Scores { get; set; }
        public int? Game1 { get; set; }
        public int? Game2 { get; set; }
        public int? Game3 { get; set; }
        public int? Game4 { get; set; }
        public int? Handicap { get; set; }
        public int? Bonus { get; set; }
    }
}
