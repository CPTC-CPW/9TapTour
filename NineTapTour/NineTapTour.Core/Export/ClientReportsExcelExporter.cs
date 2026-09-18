using ClosedXML.Excel;
using NineTapTour.Core.Models;

namespace NineTapTour.Core.Export;

public class ClientReportsExcelExporter : IClientReportsExcelExporter
{
    private const int HeaderRow = 4;

    public string BuildFileName(ReportTable table)
    {
        string safeTitle = string.Join("_", table.Title.Split(Path.GetInvalidFileNameChars()));
        return safeTitle + ".xlsx";
    }

    public void Export(ReportTable table, Stream destination)
    {
        using XLWorkbook workbook = new();
        IXLWorksheet ws = workbook.Worksheets.Add("Report");

        ws.Cell(1, 1).Value = "9 Tap Tour Report";
        ws.Cell(1, 1).Style.Font.SetBold();
        ws.Cell(2, 1).Value = table.Title;

        for (int col = 0; col < table.Columns.Count; col++)
        {
            IXLCell headerCell = ws.Cell(HeaderRow, col + 1);
            headerCell.Value = table.Columns[col].Header;
            headerCell.Style.Font.SetBold();
        }

        for (int row = 0; row < table.Rows.Count; row++)
        {
            object?[] values = table.Rows[row];
            for (int col = 0; col < table.Columns.Count; col++)
            {
                object? value = col < values.Length ? values[col] : null;
                IXLCell cell = ws.Cell(HeaderRow + 1 + row, col + 1);
                cell.Value = XLCellValue.FromObject(value);

                if (value is decimal)
                {
                    cell.Style.NumberFormat.Format = "$#,##0.00";
                }
                else if (value is DateTime)
                {
                    cell.Style.DateFormat.Format = "m/d/yyyy";
                }
            }
        }

        ws.Columns().AdjustToContents();
        workbook.SaveAs(destination);
    }
}
