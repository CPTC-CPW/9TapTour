using NineTapTour.Core.Models;

namespace NineTapTour.Core.Export;

/// <summary>
/// Writes a client report table to a new Excel workbook. Extracted from
/// FrmReports.BtnExport_Click so the desktop and web produce identical files.
/// </summary>
public interface IClientReportsExcelExporter
{
    /// <summary>Suggested file name: the report title with invalid characters replaced, plus ".xlsx".</summary>
    string BuildFileName(ReportTable table);

    void Export(ReportTable table, Stream destination);
}
