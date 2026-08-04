using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using ClosedXML.Excel;
using NineTapTour.Calculations;
using NineTapTour.Database;
using NineTapTour.Models;

namespace NineTapTour.Forms;

/// <summary>
/// Client reports form. Runs individual member reports (summary, high series,
/// high games) and tour-wide reports (high series/games/averages, total entries,
/// earnings, and placement finishes) over a career, single year, or year range.
/// </summary>
public partial class FrmReports : Form
{
    const string CATEGORY_SUMMARY = "Summary";
    const string CATEGORY_HIGH_SERIES = "High Series";
    const string CATEGORY_HIGH_GAMES = "High Games";

    static readonly Dictionary<string, TourReportCategory> TourCategories = new()
    {
        { CATEGORY_HIGH_SERIES, TourReportCategory.HighSeries },
        { CATEGORY_HIGH_GAMES, TourReportCategory.HighGames },
        { "High Averages", TourReportCategory.HighAverages },
        { "Total Entries", TourReportCategory.TotalEntries },
        { "Earnings", TourReportCategory.Earnings },
        { "1st Place Finishes", TourReportCategory.FirstPlaceFinishes },
        { "Top 5 Finishes", TourReportCategory.Top5Finishes },
        { "Top 10 Finishes", TourReportCategory.Top10Finishes },
    };

    static readonly string[] IndividualCategories =
    [
        CATEGORY_SUMMARY, CATEGORY_HIGH_SERIES, CATEGORY_HIGH_GAMES
    ];

    /// <summary>
    /// Friendly column header text for the auto-generated grid columns.
    /// </summary>
    static readonly Dictionary<string, string> ColumnHeaders = new()
    {
        { "Member", "Member #" },
        { "SeriesWithHdcp", "Series w/HDCP" },
        { "HighSeries", "High Series" },
        { "HighGame", "High Game" },
        { "FirstPlace", "1st Place" },
        { "SecondPlace", "2nd Place" },
        { "ThirdPlace", "3rd Place" },
        { "Top5", "Top 5" },
        { "Top10", "Top 10" },
    };

    /// <summary>
    /// Describes the report currently shown in the grid; used for the Excel export.
    /// </summary>
    string currentReportTitle = "";

    public FrmReports()
    {
        InitializeComponent();
    }

    private void FrmReports_Load(object sender, EventArgs e)
    {
        List<int> years = ReportsDB.GetTournamentYears();
        foreach (int year in years)
        {
            cmbYear.Items.Add(year);
            cmbYearFrom.Items.Add(year);
            cmbYearTo.Items.Add(year);
        }

        List<Member> members = MemberDB.GetMemberList()
            .OrderBy(m => m.LastName)
            .ThenBy(m => m.FirstName)
            .ToList();
        cmbMember.DataSource = members;

        PopulateCategories();
    }

    /// <summary>
    /// Fills the category dropdown based on the selected report scope,
    /// preserving the current selection when it exists in both scopes.
    /// </summary>
    private void PopulateCategories()
    {
        string previousSelection = cmbCategory.Text;

        cmbCategory.Items.Clear();
        string[] categories = rbIndividual.Checked
            ? IndividualCategories
            : TourCategories.Keys.ToArray();

        foreach (string category in categories)
        {
            cmbCategory.Items.Add(category);
        }

        int previousIndex = cmbCategory.Items.IndexOf(previousSelection);
        cmbCategory.SelectedIndex = previousIndex >= 0 ? previousIndex : 0;
    }

    private void RbScope_CheckedChanged(object sender, EventArgs e)
    {
        cmbMember.Enabled = rbIndividual.Checked;
        PopulateCategories();
    }

    private void RbPeriod_CheckedChanged(object sender, EventArgs e)
    {
        cmbYear.Enabled = rbYear.Checked;
        cmbYearFrom.Enabled = rbYearRange.Checked;
        cmbYearTo.Enabled = rbYearRange.Checked;
    }

    private void BtnRunReport_Click(object sender, EventArgs e)
    {
        if (!TryGetSelectedPeriod(out int? startYear, out int? endYear, out string periodLabel))
        {
            return;
        }

        if (!TryGetTopN(out int? topN))
        {
            return;
        }

        int? memberNumber = null;
        if (rbIndividual.Checked)
        {
            if (cmbMember.SelectedItem is not Member selectedMember)
            {
                MessageBox.Show("Please select a member for an individual report.");
                return;
            }
            memberNumber = selectedMember.Number;
        }

        List<ReportGameEntry> entries;
        try
        {
            entries = ReportsDB.GetReportEntries(startYear, endYear, memberNumber);
        }
        catch (Exception ex)
        {
            MessageBox.Show("An error occurred while loading report data:\n" + ex.Message,
                "Report Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        if (entries.Count == 0)
        {
            dgvReport.DataSource = null;
            MessageBox.Show("No finalized tournament entries were found for the selected criteria.");
            return;
        }

        bool includeSidePots = chkIncludeSidePots.Checked;
        string category = cmbCategory.Text;
        string scopeLabel = rbIndividual.Checked ? cmbMember.Text : "Tour-Wide";
        currentReportTitle = $"{scopeLabel} — {category} — {periodLabel}";
        if (includeSidePots)
        {
            currentReportTitle += " (incl. side pots)";
        }

        if (rbIndividual.Checked)
        {
            dgvReport.DataSource = category switch
            {
                CATEGORY_HIGH_SERIES => ReportCalculations.GetHighSeries(entries, topN),
                CATEGORY_HIGH_GAMES => ReportCalculations.GetHighGames(entries, topN),
                _ => (object)ReportCalculations.BuildIndividualSummary(entries, includeSidePots)
            };
        }
        else
        {
            dgvReport.DataSource = TourCategories[category] switch
            {
                TourReportCategory.HighSeries => ReportCalculations.GetHighSeries(entries, topN),
                TourReportCategory.HighGames => ReportCalculations.GetHighGames(entries, topN),
                var tourCategory => (object)ReportCalculations.GetTourReport(entries, tourCategory, topN, includeSidePots)
            };
        }

        FormatGrid();
    }

    /// <summary>
    /// Applies friendly headers and value formatting to the auto-generated columns.
    /// </summary>
    private void FormatGrid()
    {
        foreach (DataGridViewColumn column in dgvReport.Columns)
        {
            if (ColumnHeaders.TryGetValue(column.Name, out string header))
            {
                column.HeaderText = header;
            }

            if (column.ValueType == typeof(decimal))
            {
                column.DefaultCellStyle.Format = "C2";
            }
            else if (column.ValueType == typeof(DateTime))
            {
                column.DefaultCellStyle.Format = "M/d/yyyy";
            }
            else if (column.ValueType == typeof(double))
            {
                column.DefaultCellStyle.Format = "N2";
            }

            column.AutoSizeMode = column.Name is "Statistic" or "Value" or "Name" or "Location"
                ? DataGridViewAutoSizeColumnMode.AllCells
                : DataGridViewAutoSizeColumnMode.DisplayedCells;
        }
    }

    /// <summary>
    /// Reads the selected time period. Returns false and shows a message when the
    /// selection is incomplete. Career periods return null year bounds.
    /// </summary>
    private bool TryGetSelectedPeriod(out int? startYear, out int? endYear, out string periodLabel)
    {
        startYear = null;
        endYear = null;
        periodLabel = "Career";

        if (rbYear.Checked)
        {
            if (cmbYear.SelectedItem == null)
            {
                MessageBox.Show("Please select a year.");
                return false;
            }
            startYear = (int)cmbYear.SelectedItem;
            endYear = startYear;
            periodLabel = startYear.ToString();
        }
        else if (rbYearRange.Checked)
        {
            if (cmbYearFrom.SelectedItem == null || cmbYearTo.SelectedItem == null)
            {
                MessageBox.Show("Please select both a starting and ending year.");
                return false;
            }
            startYear = (int)cmbYearFrom.SelectedItem;
            endYear = (int)cmbYearTo.SelectedItem;

            if (startYear > endYear)
            {
                (startYear, endYear) = (endYear, startYear);
            }
            periodLabel = $"{startYear} to {endYear}";
        }

        return true;
    }

    /// <summary>
    /// Reads the "Show Top" limit. Blank means no limit. Returns false and shows
    /// a message for non-numeric or negative input.
    /// </summary>
    private bool TryGetTopN(out int? topN)
    {
        topN = null;

        if (string.IsNullOrWhiteSpace(txtTopN.Text))
        {
            return true;
        }

        if (!int.TryParse(txtTopN.Text, out int parsed) || parsed <= 0)
        {
            MessageBox.Show("Show Top must be a positive number, or blank to show all rows.");
            return false;
        }

        topN = parsed;
        return true;
    }

    /// <summary>
    /// Exports the report currently shown in the grid to a new Excel file.
    /// </summary>
    private void BtnExport_Click(object sender, EventArgs e)
    {
        if (dgvReport.Rows.Count == 0)
        {
            MessageBox.Show("Run a report before exporting.");
            return;
        }

        try
        {
            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Report");

            ws.Cell(1, 1).Value = "9 Tap Tour Report";
            ws.Cell(1, 1).Style.Font.SetBold();
            ws.Cell(2, 1).Value = currentReportTitle;

            const int HEADER_ROW = 4;
            for (int col = 0; col < dgvReport.Columns.Count; col++)
            {
                var headerCell = ws.Cell(HEADER_ROW, col + 1);
                headerCell.Value = dgvReport.Columns[col].HeaderText;
                headerCell.Style.Font.SetBold();
            }

            for (int row = 0; row < dgvReport.Rows.Count; row++)
            {
                for (int col = 0; col < dgvReport.Columns.Count; col++)
                {
                    object value = dgvReport.Rows[row].Cells[col].Value;
                    var cell = ws.Cell(HEADER_ROW + 1 + row, col + 1);
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

            string safeTitle = string.Join("_", currentReportTitle.Split(System.IO.Path.GetInvalidFileNameChars()));
            SaveFileDialog saveFile = new()
            {
                Filter = FileHelper.GetExcelFilterStringForFileDialogs(),
                FileName = safeTitle + ".xlsx"
            };

            if (saveFile.ShowDialog() == DialogResult.OK)
            {
                workbook.SaveAs(saveFile.FileName);
                MessageBox.Show("Excel file created, you can find the file at: " + saveFile.FileName);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("An error occurred during the export process:\n" + ex.Message,
                "Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
