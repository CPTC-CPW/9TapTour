using NineTapTour.Core.Models;

namespace NineTapTour.Core.Export;

/// <summary>
/// Writes the standings report (top N bowlers by series, game, or senior game)
/// into a copy of the SeriesReportTemplate workbook. Extracted from
/// FrmMemberScoresReports.ExportToExcel so the desktop and web share it.
/// </summary>
public interface IStandingsReportExcelExporter
{
    /// <summary>File name the desktop app has always suggested: "{Series|Senior|FinalGame}{Location} {Event} {MM-dd-yyyy}.xlsx".</summary>
    string BuildFileName(StandingsReportExportRequest request);

    /// <summary>Fills the template at <paramref name="templatePath"/> and writes the workbook to <paramref name="destination"/>.</summary>
    void Export(string templatePath, Stream destination, StandingsReportExportRequest request);

    /// <summary>Fills the template and saves the workbook to <paramref name="outputPath"/>.</summary>
    void Export(string templatePath, string outputPath, StandingsReportExportRequest request);
}
