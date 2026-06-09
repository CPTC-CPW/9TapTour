namespace NineTapTour.Core.Models;

/// <summary>
/// This represents one entry for a bowler in the weekly books for the workbook in Excel
/// </summary>
public class ExcelRow
{
    /// <summary>
    /// Member bowler first name
    /// </summary>
    public string PlayerFirstName { get; set; }

    /// <summary>
    /// Member bowler middle name
    /// </summary>
    public string PlayerMiddleName { get; set; }

    /// <summary>
    /// Member bowler last name
    /// </summary>
    public string PlayerLastName { get; set; }

    /// <summary>
    /// Member allocated number when signed in as a new member
    /// </summary>
    public int PlayerNumber { get; set; }

    /// <summary>
    /// Member original average score from signing as a new member
    /// </summary>
    public int PlayerOrginalAVG { get; set; }

    /// <summary>
    /// Member total games played
    /// </summary>
    public int GameTotal { get; set; }

    /// <summary>
    /// The current date and time
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// Game 1 final score
    /// </summary>
    public int Game1 { get; set; }

    /// <summary>
    /// Game 2 final score
    /// </summary>
    public int Game2 { get; set; }

    /// <summary>
    /// Game 3 final score
    /// </summary>
    public int Game3 { get; set; }

    /// <summary>
    /// Game 4 final score
    /// </summary>
    public int Game4 { get; set; }

    /// <summary>
    /// Scratch total is the sum for all games bowled before bonus pin(s) and handicap
    /// </summary>
    public int Total { get; set; }

    /// <summary>
    /// The average of all games bowled
    /// </summary>
    public double AverageOfRow { get; set; }

    /// <summary>
    /// The true average is for as many entries up to 43 in the weekly books per page
    /// this application cuts off at 30 entries and is tracked as 30 game average
    /// in the finalize form it is under 30 Entry AVG
    /// </summary>
    public double TrueAverage { get; set; }

    /// <summary>
    /// The director adjusted average,*set to -1 if no value is present
    /// </summary>
    public int AVG { get; set; }

    /// <summary>
    /// The number of bonus pin(s)
    /// </summary>
    public int Bonus { get; set; }

    /// <summary>
    /// The handicap value
    /// </summary>
    public int HandyCap { get; set; }

    /// <summary>
    /// Finalized progressive pot, used in import to see if a member placed
    /// </summary>
    public string FinPPHG { get; set; }

    /// <summary>
    /// The earnings column
    /// </summary>
    public double Cash { get; set; }

    /// <summary>
    /// Extra notes for that particular bowler
    /// </summary>
    public string Notes { get; set; }
}