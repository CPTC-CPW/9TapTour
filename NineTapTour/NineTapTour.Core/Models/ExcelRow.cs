namespace NineTapTour.Core.Models;

public class ExcelRow
{
    public string PlayerFirstName { get; set; }
    public string PlayerMiddleName { get; set; }
    public string PlayerLastName { get; set; }
    public int PlayerNumber { get; set; }
    public int PlayerOrginalAVG { get; set; }
    public int GameTotal { get; set; }
    public DateTime Date { get; set; }
    public int Game1 { get; set; }
    public int Game2 { get; set; }
    public int Game3 { get; set; }
    public int Game4 { get; set; }
    public int Total { get; set; }
    public double AverageOfRow { get; set; }
    public double TrueAverage { get; set; }
    public int AVG { get; set; }
    public int Bonus { get; set; }
    public int HandyCap { get; set; }
    public string PotPro { get; set; }
    public string FinPPHG { get; set; }
    public double Cash { get; set; }
    public string Notes { get; set; }
}
