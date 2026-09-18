using ClosedXML.Excel;
using NineTapTour.Core.Entities;
using NineTapTour.Core.Models;
using NineTapTour.Core.Printing;
using static NineTapTour.Core.Calculations.ReportHelper;

namespace NineTapTour.Core.Export;

public class StandingsReportExcelExporter : IStandingsReportExcelExporter
{
    /// <summary>Name of the template workbook shipped with both UIs.</summary>
    public const string TemplateFileName = "SeriesReportTemplate.xlsx";

    private const int FirstDataRow = 5;

    public string BuildFileName(StandingsReportExportRequest request)
    {
        string date = request.TournamentDate.ToString("MM-dd-yyyy");
        return $"{ReportTypeFileLabel(request.ReportType)}{request.TournamentLocation} {request.TournamentEvent} {date}.xlsx";
    }

    public void Export(string templatePath, string outputPath, StandingsReportExportRequest request)
    {
        using FileStream output = File.Create(outputPath);
        Export(templatePath, output, request);
    }

    public void Export(string templatePath, Stream destination, StandingsReportExportRequest request)
    {
        using XLWorkbook workbook = new(templatePath);
        IXLWorksheet ws = workbook.Worksheet(1);

        ws.Cell(3, 1).Value = request.TournamentLocation;
        ws.Cell(3, 4).Value = request.TournamentEvent;
        ws.Cell(3, 5).Value = request.TournamentDate;
        ws.Cell(4, 2).Value = ReportTypeSheetLabel(request.ReportType);

        if (request.PrintDues)
        {
            ws.Cell(4, 5).Value = "Membership Paid To";
        }

        for (int index = 0; index < request.Rows.Count; index++)
        {
            int row = FirstDataRow + index;
            MemberScores entry = request.Rows[index];

            ws.Cell(row, 1).Value = entry.placing;
            ws.Cell(row, 2).Value = entry.Score;
            if (entry is TeamMemberScores team)
            {
                ws.Cell(row, 3).Value = $"{team.Partner1MemberId} & {team.Partner2MemberId}";
                ws.Cell(row, 4).Value = $"{team.Partner1FirstName} {team.Partner1LastName} & {team.Partner2FirstName} {team.Partner2LastName}";
            }
            else
            {
                ws.Cell(row, 3).Value = entry.MemberId;
                ws.Cell(row, 4).Value = entry.LastName + ", " + entry.FirstName;
            }

            if (request.PrintDues)
            {
                ws.Cell(row, 5).Value = entry is TeamMemberScores teamEntry
                    ? $"{PrintContentBuilder.FormatDuesYear(teamEntry.LastPaymentYear)} & {PrintContentBuilder.FormatDuesYear(teamEntry.Partner2LastPaymentYear)}"
                    : PrintContentBuilder.FormatDuesYear(entry.LastPaymentYear);
            }
        }

        workbook.SaveAs(destination);
    }

    /// <summary>Prefix used in the suggested file name.</summary>
    public static string ReportTypeFileLabel(ReportType reportType)
    {
        return reportType switch
        {
            ReportType.HighSeriesScratch => "Series",
            ReportType.HighGameHandicapGameSenior => "Senior",
            _ => "FinalGame",
        };
    }

    /// <summary>Label written into the template's report-type cell.</summary>
    public static string ReportTypeSheetLabel(ReportType reportType)
    {
        return reportType == ReportType.HighSeriesScratch ? "Series" : "Game";
    }
}
