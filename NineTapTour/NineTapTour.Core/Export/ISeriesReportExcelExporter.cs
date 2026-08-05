#nullable disable
using NineTapTour.Core.Models;
using System.Collections.Generic;

namespace NineTapTour.Core.Export;

/// <summary>
/// Writes tournament results into a series report Excel template (workbook writing,
/// check-sheet formula remapping for doubles, and drawing restoration), extracted
/// from FrmTournamentResults. The form owns all dialogs and passes the chosen
/// template and destination paths in.
/// </summary>
public interface ISeriesReportExcelExporter
{
    /// <summary>
    /// Reads earnings (column I) and any progressive pot row that follows each bowler
    /// row from a pre-filled results template, starting at row 4. Returns one entry
    /// per bowler row, in order.
    /// </summary>
    List<TemplateEarningsRow> ReadEarningsAndPots(string templatePath, int rowCount);

    /// <summary>
    /// Opens the template at <paramref name="templatePath"/>, writes the tournament
    /// results into it, and saves the result to <paramref name="destinationPath"/>,
    /// restoring the template's drawing parts afterwards.
    /// </summary>
    void Export(string templatePath, string destinationPath, SeriesReportExportRequest request);
}
