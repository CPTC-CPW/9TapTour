namespace NineTapTour.Core.Import;

/// <summary>
/// Player identity parsed from the header row (row 1) of a legacy
/// weekly-book worksheet.
/// </summary>
public class ExcelPlayerHeader
{
    public string FirstName { get; set; } = "";

    public string MiddleName { get; set; } = "";

    public string LastName { get; set; } = "";

    public int OriginalAverage { get; set; } = -1;

    public int PlayerNumber { get; set; }
}
