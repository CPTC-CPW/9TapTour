using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using NineTapTour.Database;
using NineTapTour.Models;
using ClosedXML.Excel;

namespace NineTapTour.Forms;

public partial class FrmMemberScoresReports : Form
{
    // the members and their scores
    List<Models.MemberScores> temp;
    // used in the print class to print the date and location
    readonly Tournament selectedTournament;

    readonly ReportType reportTypeNum;
    readonly int currentSquad;
    readonly List<int> squadList;
    bool printDues = false;

    public FrmMemberScoresReports(List<MemberScores> temp, Tournament selectedTournament, ReportType reportTypeNum, int currentSquad, List<int> squadList)
    {
        InitializeComponent();
        this.temp = temp;
        this.selectedTournament = selectedTournament;
        this.reportTypeNum = reportTypeNum;
        this.currentSquad = currentSquad;
        this.squadList = squadList;
    }

    private void BtnPrint_Click(object sender, EventArgs e)
    {
        if (!int.TryParse(txtNumberOfMembers.Text, out int numMembers))
        {
            MessageBox.Show("Please only input a number");
        }
        // if user inputs 0
        else if (numMembers == 0)
        {
            MessageBox.Show("Please do not Input 0.");
        }
        // if good to go
        else if (numMembers <= temp.Count)
        {
            //See if they want the date for membership dues to be printed.
            if (cbPrintDues.Checked) {
                printDues = true;
            }

            temp = Calculations.TournamentCalculations.MakeTopMembersByPlacementList(temp, numMembers, selectedTournament.Doubles);
            // print( go to print class )
            int? manualCutoffLine = null;
            if(int.TryParse(txtCutoffLine.Text, out int result))
            {
                manualCutoffLine = result;
            }

            Database.Print.PrintMemberReport(temp, selectedTournament, reportTypeNum, currentSquad, squadList, printDues, manualCutoffLine);

            this.Close();
        }
        // if user inputs a bigger number than the number of members
        else
        {
            MessageBox.Show("There are only " + temp.Count + " participants in the tournament selected.");
        }
    }

    private void FrmMemberScoresReports_Load(object sender, EventArgs e)
    {
        txtNumberOfMembers.Focus();
    }

    private void BtnSave_Click(object sender, EventArgs e)
    {
        if (!int.TryParse(txtNumberOfMembers.Text, out int numMembers))
        {
            MessageBox.Show("Please only input a number");
        }
        // if user inputs 0
        else if (numMembers == 0)
        {
            MessageBox.Show("Please do not Input 0.");
        }
        // if good to go
        else if (numMembers <= temp.Count)
        {
            //See if they want the date for membership dues to be printed.
            if (cbPrintDues.Checked)
            {
                printDues = true;
            }
            temp = Calculations.TournamentCalculations.MakeTopMembersByPlacementList(temp, numMembers, selectedTournament.Doubles); // results of inquiry
            ExportToExcel(); // Exports to excel file

            this.Close();
        }
        // if user inputs a bigger number than the number of members
        else
        {
            MessageBox.Show("There are only " + temp.Count + " participants in the tournament selected.");
        }
    }

    private void ExportToExcel()
    {
        string reportTypeToSave;
        string reportLabelToSave = "Game";
        if (reportTypeNum == ReportType.HighSeriesScratch)
        {
            reportLabelToSave = "Series";
            reportTypeToSave = "Series";
        }
        else if (reportTypeNum == ReportType.HighGameHandicapGameSenior)
        {
            reportTypeToSave = "Senior";
        }
        else
        {
            reportTypeToSave = "FinalGame";
        }

        string getFilePath = Path.GetFullPath("Resources/SeriesReportTemplate.xlsx");
        string tourneyDate = selectedTournament.Date.ToString("MM/dd/yyyy");
        string tournyDate = tourneyDate.Replace("/", "-");
        string tournamentDate = tournyDate;
        string fileName = reportTypeToSave + selectedTournament.Location + " " + selectedTournament.Event + " " + tournamentDate + ".xlsx";
        string saveFile = fileName;

        try
        {
            File.Copy(getFilePath, saveFile, true);
            using var workbook = new XLWorkbook(saveFile);
            var ws = workbook.Worksheet(1);
            ws.Cell(3, 1).Value = selectedTournament.Location;
            ws.Cell(3, 4).Value = selectedTournament.Event;
            ws.Cell(3, 5).Value = selectedTournament.Date;
            ws.Cell(4, 2).Value = reportLabelToSave;

            int printDuesOffset = 0;
            if (printDues)
            {
                ws.Cell(4, 5).Value = "Membership Paid To";
                printDuesOffset = 1;
            }

            const int headerRowsOffset = 4;
            int numMembers = temp.Count;
            for (int row = 5; row <= numMembers + headerRowsOffset; row++)
            {
                int idx = row - 5;
                ws.Cell(row, 1).Value = temp[idx].placing;
                ws.Cell(row, 2).Value = temp[idx].Score;
                if (temp[idx] is TeamMemberScores t)
                {
                    ws.Cell(row, 3).Value = $"{t.Partner1MemberId} & {t.Partner2MemberId}";
                    ws.Cell(row, 4).Value = $"{t.Partner1FirstName} {t.Partner1LastName} & {t.Partner2FirstName} {t.Partner2LastName}";
                }
                else
                {
                    ws.Cell(row, 3).Value = temp[idx].MemberId;
                    ws.Cell(row, 4).Value = temp[idx].LastName + ", " + temp[idx].FirstName;
                }
                if (printDuesOffset == 1)
                {
                    string cellValue = (temp[idx] is TeamMemberScores teamEntry)
                        ? $"{FormatDuesYear(teamEntry.LastPaymentYear)} & {FormatDuesYear(teamEntry.Partner2LastPaymentYear)}"
                        : FormatDuesYear(temp[idx].LastPaymentYear);
                    ws.Cell(row, 5).Value = cellValue;

                    static string FormatDuesYear(string lastPaymentYear)
                    {
                        if (string.IsNullOrWhiteSpace(lastPaymentYear))
                            return "N/A";
                        if (lastPaymentYear.Equals("life "))
                            return lastPaymentYear;
                        if (int.TryParse(lastPaymentYear, out int year))
                            return (year + 1).ToString();
                        return lastPaymentYear;
                    }
                }
            }

            SaveFileDialog savefile = new()
            {
                Filter = FileHelper.GetExcelFilterStringForFileDialogs(),
                FileName = fileName
            };
            DialogResult result = savefile.ShowDialog();
            if (result == DialogResult.OK)
            {
                workbook.SaveAs(savefile.FileName);
                MessageBox.Show("Excel file created , you can find the file at: " + savefile.FileName);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("An error occurred during the export process:\n" + ex.Message, "Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void TxtCutoffLine_Click(object sender, EventArgs e)
    {
        FormHelper.GoToFirstIndexInTextboxIfEmpty(txtCutoffLine);
    }
}
