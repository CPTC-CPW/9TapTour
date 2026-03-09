using NineTapTour.Database;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NineTapTour.Models;
using ClosedXML.Excel;
using System.Collections;
using System.Text.RegularExpressions;
using NineTapTour.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace NineTapTour.Forms
{
    public partial class FrmTournamentResults : Form
    {
        // Names of the Columns in the DataGridView
        const string PLACE_STANDING_COLUMN_NAME = "Place";
        const string FULLNAME_COLUMN_NAME = "Full Name";
        const string HANDICAP_COLUMN_NAME = "H/B*";
        const string TOTAL_SCORE_COLUMN_NAME = "Total Score";
        const string EARNINGS_COLUMN_NAME = "Earnings";
        const string MEMBER_ID_COLUMN_NAME = "Member ID";
        const string GAME_ID_COLUMN_NAME = "Game ID";
        const string PROGRESSIVEPOT_COLUMN_NAME = "Progressive Pot";

        readonly DataTable dt = new(); // Instantiate Data Table
        readonly NineTapDb db = new(); // Get access to database
        readonly Tournament tourny = FrmMemberScoresHelpers.selectedTournament; // Get Tournament
        static int totalTournamentEntries;  // Total number of entries for all squads in tournament
        static int clientInput; // how many winners the client wants to see
        List<ExcelMember> clientRequested = [];
        List<ExcelMember> winners = [];

        /* Floor directors get a comp entry into tournament when they help with tournament. 
         * They don't pay the entry fee, but do qualify to cash.
         */
        static int compEntries;

        #region Form Initilizers and Closers
        public FrmTournamentResults()
        {
            InitializeComponent();
        }
        private void FrmTournamentResults_Load(object sender, EventArgs e)
        {
            // Set compEntries to 0; increment when building winners list
            compEntries = 0;

            // Display tournament name, date and type (if applicable) on form
            lblTournamentName.Text = tourny.TourneyNameDate;
            if (tourny.Doubles)
            {
                lblTournamentName.Text += " (DOUBLES TOURNAMENT)";
            }
            if (tourny.ThreeOutOf4)
            {
                lblTournamentName.Text += " (3 OUT OF 4 TOURNAMENT)";
            }

            // Create a List<ExcelMember> and populate it with this tournament's participants
            winners = BuildWinnersList();
            
            ActiveControl = tbClientInputCount;
        }
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            if (e.CloseReason == CloseReason.WindowsShutDown) return;
            List<double> Winnings = [];
            for (int winningList = 0; winningList < dgvTournamentResults.RowCount; winningList++)
            {
                Winnings.Add(Convert.ToDouble(dgvTournamentResults[EARNINGS_COLUMN_NAME, winningList].Value));
            }
            TempVariablesForGlobalLevel.MoneyEarnings = Winnings;

            // Save all changes made to the dataGridView
            for (int currentIndex = 0; currentIndex < clientRequested.Count; currentIndex++)
            {
                int gameId = Convert.ToInt32(dgvTournamentResults[GAME_ID_COLUMN_NAME, currentIndex].Value.ToString());
                Game g = GameDB.GetGame(gameId);

                g.PlaceStanding = ParsePlaceStanding(dgvTournamentResults[PLACE_STANDING_COLUMN_NAME, currentIndex].Value);
                g.MoneyWon = Convert.ToDecimal(dgvTournamentResults[EARNINGS_COLUMN_NAME, currentIndex].Value);

                // if user enters something other than a decimal number
                if (Decimal.TryParse(Convert.ToString(dgvTournamentResults[PROGRESSIVEPOT_COLUMN_NAME, currentIndex].Value), out decimal _))
                {
                    g.SidePot = Convert.ToDecimal(dgvTournamentResults[PROGRESSIVEPOT_COLUMN_NAME, currentIndex].Value);
                }
                else
                {
                    g.Notes = $"Progressive Pot was entered as: {Convert.ToString(dgvTournamentResults[PROGRESSIVEPOT_COLUMN_NAME, currentIndex].Value)}";
                }

                // Phase 4: Removed g.gameRegionID assignment - stored in Participant entity
                db.Entry(g).State = EntityState.Modified;
                db.SaveChanges();
            }
        }
        #endregion

        /// <summary>
        /// Creates the DataGridView table and populates it with the list of cashed winners
        /// </summary>
        private void CreateDataGridView(List<ExcelMember> clientRequested, int clientInput)
        {
            // Create data table and add columns, columns with ReadOnly set to False are editable      
            dt.Columns.Add(PLACE_STANDING_COLUMN_NAME).ReadOnly = true;
            dt.Columns.Add(FULLNAME_COLUMN_NAME).ReadOnly = true;
            dt.Columns.Add(HANDICAP_COLUMN_NAME).ReadOnly = true;
            dt.Columns.Add(TOTAL_SCORE_COLUMN_NAME).ReadOnly = true;
            dt.Columns.Add(EARNINGS_COLUMN_NAME).ReadOnly = false;
            dt.Columns.Add(MEMBER_ID_COLUMN_NAME).ReadOnly = true;
            dt.Columns.Add(GAME_ID_COLUMN_NAME).ReadOnly = true;
            dt.Columns.Add(PROGRESSIVEPOT_COLUMN_NAME).ReadOnly = false;
          
            double earnings = 0.00;

            int MonEarnCount = 0;
            if (TempVariablesForGlobalLevel.MoneyEarnings != null)
            {
                MonEarnCount = TempVariablesForGlobalLevel.MoneyEarnings.Count;
            }

            // Create rows and populate with each member's data for each row
            for (int wc = 0; wc < clientRequested.Count; wc++)
            {
                DataRow newRow = dt.NewRow();
                if (MonEarnCount > 0)
                {
                    if (wc < MonEarnCount)
                    {
                        newRow[EARNINGS_COLUMN_NAME] = TempVariablesForGlobalLevel.MoneyEarnings[wc];
                    }
                    else
                    {
                        newRow[EARNINGS_COLUMN_NAME] = earnings;
                    }
                }
                else
                {
                    newRow[EARNINGS_COLUMN_NAME] = Convert.ToInt32(clientRequested[wc].MoneyWon);
                }
                
                newRow[PLACE_STANDING_COLUMN_NAME] = clientRequested[wc].PlaceStanding;
                newRow[FULLNAME_COLUMN_NAME] = clientRequested[wc].Name;
                newRow[HANDICAP_COLUMN_NAME] = (clientRequested[wc].Handicap) + " + " + clientRequested[wc].Bonus;
                newRow[TOTAL_SCORE_COLUMN_NAME] = clientRequested[wc].TotalScore;
                newRow[MEMBER_ID_COLUMN_NAME] = clientRequested[wc].MemberNumber;
                newRow[GAME_ID_COLUMN_NAME] = clientRequested[wc].GameId;

                if (clientRequested[wc].SidePot == null)
                {
                    newRow[PROGRESSIVEPOT_COLUMN_NAME] = "0.00";
                }
                else
                {
                    newRow[PROGRESSIVEPOT_COLUMN_NAME] = clientRequested[wc].SidePot;
                }
                dt.Rows.Add(newRow);
            }

            for (int tr = clientRequested.Count; tr < clientInput ; tr++)
            {
                DataRow newRow = dt.NewRow();
                if (MonEarnCount > 0 && tr < MonEarnCount)
                {
                    newRow[EARNINGS_COLUMN_NAME] = TempVariablesForGlobalLevel.MoneyEarnings[tr];
                }
                else
                {
                    newRow[EARNINGS_COLUMN_NAME] = earnings;
                }
                newRow[PLACE_STANDING_COLUMN_NAME] = tr + 1;
                newRow[FULLNAME_COLUMN_NAME] = "";
                newRow[HANDICAP_COLUMN_NAME] = "";
                newRow[TOTAL_SCORE_COLUMN_NAME] = "";
                newRow[MEMBER_ID_COLUMN_NAME] = "";
                newRow[GAME_ID_COLUMN_NAME] = tr;
                newRow[PROGRESSIVEPOT_COLUMN_NAME] = "0.00";
                dt.Rows.Add(newRow);
            }

            // Append "T" to the place standing of any tied rows (same numeric place as a neighbour)
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (!int.TryParse(dt.Rows[i][PLACE_STANDING_COLUMN_NAME]?.ToString(), out int place) || place == 0)
                    continue;
                bool isTie = (i > 0
                                && int.TryParse(dt.Rows[i - 1][PLACE_STANDING_COLUMN_NAME]?.ToString(), out int prev)
                                && prev == place)
                           || (i < dt.Rows.Count - 1
                                && int.TryParse(dt.Rows[i + 1][PLACE_STANDING_COLUMN_NAME]?.ToString(), out int next)
                                && next == place);
                if (isTie)
                    dt.Rows[i][PLACE_STANDING_COLUMN_NAME] = $"{place}T";
            }

            // If there is data in datatable rows, then set datatable as source for datagridview
            // Hide the GameId and MemberId columns and don't allow user to add rows
            // Size datagridview columns to fit contents with name column filling rest of datagridview
            // Set the current cell to the cell in column 4, row 0 (MoneyWon column)
            if (dt.Rows.Count > 0)
            {
                dgvTournamentResults.DataSource = dt;

                dgvTournamentResults.Columns[MEMBER_ID_COLUMN_NAME].Visible = false;
                dgvTournamentResults.Columns[GAME_ID_COLUMN_NAME].Visible = false;
                dgvTournamentResults.AllowUserToAddRows = false;

                dgvTournamentResults.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                dgvTournamentResults.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                dgvTournamentResults.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                dgvTournamentResults.Columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                dgvTournamentResults.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dgvTournamentResults.Columns[5].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                
                dgvTournamentResults.CurrentCell = dgvTournamentResults[4, 0];
            }
        }

        /// <summary>
        /// Tabs the user though the cells of dgvTournamentResults
        /// </summary>
        private void DgvTournamentResults_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvTournamentResults.CurrentRow.Cells[e.ColumnIndex].ReadOnly)
            {
                SendKeys.Send("{tab}");
            }
        }

        /// <summary>
        /// Returns a list of tourament winners
        /// </summary>
        /// <returns> List<ExcelMember> </returns>
        private List<ExcelMember> BuildWinnersList()
        {
            List<ExcelMember> tournyBowlers = [];
            List<WinnerListMemberViewModel> bowlers = TournamentDB.GetWinnerListMemberData(tourny.Id);

            totalTournamentEntries = bowlers.Count;

            foreach (var b in bowlers)
            {
                if (b.IsComp)
                {
                    compEntries++;
                }

                ExcelMember m = new()
                {
                    MemberNumber = b.MemberNumber,
                    Name = b.BowlerName,
                    Handicap = Convert.ToInt32(b.Handicap),
                    Bonus = Convert.ToInt32(b.Bonus),
                    MoneyWon = b.MoneyWon,
                    SidePot = b.SidePot,
                    GameId = b.GameId,
                    // If the game scores are null then a 0 will be placed in the the game
                    Game1Score = Convert.ToInt32(b.Game1),
                    Game2Score = Convert.ToInt32(b.Game2),
                    Game3Score = Convert.ToInt32(b.Game3),
                    Game4Score = Convert.ToInt32(b.Game4)
                };

                if (tourny.ThreeOutOf4)
                {
                    List<int> scores = [m.Game1Score, m.Game2Score, m.Game3Score, m.Game4Score];

                    // Remove the 0s from the scores list
                    scores.RemoveAll(x => x == 0);

                    // remove lowest score if there are 4 games
                    if (scores.Count == 4)
                    {
                        int minScore = scores.Min();
                        scores.Remove(minScore);
                        if (m.Game1Score == minScore)
                            m.Game1Score = 0;
                        else if (m.Game2Score == minScore)
                            m.Game2Score = 0;
                        else if (m.Game3Score == minScore)
                            m.Game3Score = 0;
                        else if (m.Game4Score == minScore)
                            m.Game4Score = 0;
                    }

                    m.TotalScore = scores.Sum()
                        + (scores.Count * (m.Handicap + m.Bonus));
                }
                else
                {
                    int totalValidGames = 0;
                    if (m.Game1Score > 0)
                        totalValidGames++;
                    if (m.Game2Score > 0)
                        totalValidGames++;
                    if (m.Game3Score > 0)
                        totalValidGames++;
                    if (m.Game4Score > 0)
                        totalValidGames++;
                    
                    m.TotalScore = m.Game1Score + m.Game2Score + m.Game3Score
                        + m.Game4Score + (totalValidGames * (m.Handicap + m.Bonus));
                }
                tournyBowlers.Add(m);
            }
            return tournyBowlers;
        }

        #region Export to Excel (code is fragile)
        private void BtnExportToExcel_Click(object sender, EventArgs e)
        {
            bool wait = true;
            while (wait)
            {
                frmPleaseWait please = new();
                please.Show();
                ExportToExcel();
                wait = false;
                please.Close();
            }
        }
        
        /// <summary>
        /// Returns the ordinal representation of a given place, optionally appending a tie indicator.
        /// </summary>
        /// <param name="place">The numeric position to convert to an ordinal string. Must be a positive integer.</param>
        /// <param name="isTie">A boolean indicating whether to append a tie indicator. If <see langword="true"/>, "T" is appended to the
        /// result.</param>
        /// <returns>A string representing the ordinal form of the specified place, with an optional tie indicator.</returns>
        private static string GetOrdinalWithTie(int place, bool isTie)
        {
            string suffix;
            int ones = place % 10;
            int tens = (place % 100) / 10;
            if (tens == 1)
                suffix = "th";
            else if (ones == 1)
                suffix = "st";
            else if (ones == 2)
                suffix = "nd";
            else if (ones == 3)
                suffix = "rd";
            else
                suffix = "th";
            return $"{place}{suffix}{(isTie ? "T" : "" )}";
        }

        /// <summary>
        /// Parses a place-standing cell value that may have a trailing "T" tie indicator
        /// and returns the numeric portion as a <see cref="byte"/>.
        /// </summary>
        private static byte ParsePlaceStanding(object value)
        {
            string s = value?.ToString()?.TrimEnd('T') ?? "0";
            return byte.TryParse(s, out byte result) ? result : (byte)0;
        }

        private void ExportToExcel()
        {
            // Saves participants' place standing and earnings won to the database
            for (int currentIndex = 0; currentIndex < dgvTournamentResults.RowCount; currentIndex++)
            {
                int gameId = Convert.ToInt32(dgvTournamentResults[GAME_ID_COLUMN_NAME, currentIndex].Value.ToString());
                Game g = GameDB.GetGame(gameId);

                g.PlaceStanding = ParsePlaceStanding(dgvTournamentResults[PLACE_STANDING_COLUMN_NAME, currentIndex].Value);
                g.MoneyWon = Convert.ToDecimal(dgvTournamentResults[EARNINGS_COLUMN_NAME, currentIndex].Value);
                g.SidePot = Convert.ToDecimal(dgvTournamentResults[PROGRESSIVEPOT_COLUMN_NAME, currentIndex].Value);
                
                // Phase 4: Removed g.gameRegionID assignment - stored in Participant entity
                db.SaveChanges();
            }

            string getFilePath = Path.GetFullPath("Resources/TournamentResultsTemplate.xlsx");
            string tourneyDate = tourny.Date.ToString("MM/dd/yyyy");
            string tournyDateDash = tourneyDate.Replace("/", "-");
            string tournamentDate = tournyDateDash; // Already formatted
            string fileName = tourny.Location + " " + tourny.Event + " " + tournamentDate + ".xlsx";
            string saveFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "TournamentResultsCopy.xlsx");
            File.Copy(getFilePath, saveFile, true);

            try
            {
                using (var workbook = new XLWorkbook(saveFile))
                {
                    var ws = workbook.Worksheet(1);
                    ws.Cell(1, 1).Value = tourny.Location + tourny.Event;
                    ws.Cell(2, 1).Value = tourny.Date;

                    int excelRow = 4;
                    int i = 0;
                    while (i < dt.Rows.Count)
                    {
                        if (excelRow >= 35)
                        {
                            ws.Row(excelRow).InsertRowsAbove(1);
                            ws.Range($"G{excelRow}:H{excelRow}").Merge();
                        }

                        var row = dt.Rows[i];
                        int currentPlace = 0;
                        int.TryParse(row[PLACE_STANDING_COLUMN_NAME]?.ToString()?.TrimEnd('T'), out currentPlace);
                        // Check for tie: if previous or next row has the same place
                        bool isTie = false;
                        if (i > 0)
                        {
                            int prevPlace = 0;
                            int.TryParse(dt.Rows[i - 1][PLACE_STANDING_COLUMN_NAME]?.ToString()?.TrimEnd('T'), out prevPlace);
                            if (prevPlace == currentPlace) isTie = true;
                        }
                        if (i < dt.Rows.Count - 1)
                        {
                            int nextPlace = 0;
                            int.TryParse(dt.Rows[i + 1][PLACE_STANDING_COLUMN_NAME]?.ToString()?.TrimEnd('T'), out nextPlace);
                            if (nextPlace == currentPlace) isTie = true;
                        }
                        // Parse the progressive pot once; spVal = 0 if the cell is empty or non-numeric.
                        decimal.TryParse(row[PROGRESSIVEPOT_COLUMN_NAME]?.ToString(), out decimal spVal);
                        // Use InvariantCulture so the decimal separator in the formula literal is always "."
                        // regardless of the system locale (e.g. prevents "20,5" in German locales).
                        string sidePotValue = spVal.ToString(System.Globalization.CultureInfo.InvariantCulture);

                        // Use ordinal with tie for place standing
                        ws.Cell(excelRow, 1).Value = GetOrdinalWithTie(currentPlace, isTie);
                        ws.Cell(excelRow, 2).Value = row[FULLNAME_COLUMN_NAME]?.ToString();
                        ws.Cell(excelRow, 6).Value = row[HANDICAP_COLUMN_NAME]?.ToString();
                        ws.Cell(excelRow, 7).Value = row[TOTAL_SCORE_COLUMN_NAME]?.ToString();
                        ws.Cell(excelRow, 9).Value = row[EARNINGS_COLUMN_NAME] != null
                            ? double.TryParse(row[EARNINGS_COLUMN_NAME].ToString(), out var val)
                                ? val.ToString("C0")
                                : row[EARNINGS_COLUMN_NAME]?.ToString()
                            : "$0";
                        ws.Cell(excelRow, 11).Value = row[PLACE_STANDING_COLUMN_NAME]?.ToString()?.TrimEnd('T');
                        ws.Cell(excelRow, 12).Value = row[MEMBER_ID_COLUMN_NAME]?.ToString();
                        ws.Cell(excelRow, 15).FormulaA1 = $"=I{excelRow}-M{excelRow}-N{excelRow}+{sidePotValue}";
                        // Write as a numeric value so Excel treats it as a number, not text.
                        ws.Cell(excelRow, 8).Value = spVal;

                        // Any entry that placed 1st–3rd gets a progressive pot row directly below it.
                        // The template pre-formats those rows at positions 5, 7, and 9 (covering excelRows 4, 6, 8).
                        // When extra ties push a 4th (or more) top-3 entry past row 8, insert a new row so the
                        // progressive pot slot is always present regardless of how many bowlers tied into places 1–3.
                        if (currentPlace >= 1 && currentPlace <= 3)
                        {
                            if (excelRow > 8)
                            {
                                ws.Row(excelRow + 1).InsertRowsAbove(1);
                                ws.Range($"G{excelRow + 1}:H{excelRow + 1}").Merge();
                            }
                            // Reuse the already-parsed value — no second TryParse needed.
                            ws.Cell(excelRow + 1, 9).Value = spVal;
                            excelRow++;
                        }

                        i++;
                        excelRow++;
                    }

                    // Set total payout
                    double money = 0;
                    for (int k = 0; k < dt.Rows.Count; k++)
                        money += Convert.ToDouble(dt.Rows[k][EARNINGS_COLUMN_NAME]);
                    ws.Cell(2, 8).Value = money;

                    // Save dialog
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
            }
            catch (Exception ex)
            {
                MessageBox.Show("Must choose a file to export to.\n *Must have at least 20 bowlers and 4 money winners*\n" + ex.Message);
            }
        }
        #endregion

        /// <summary>
        /// Runs AcceptClientInputForResults if the user presses the "Enter" key
        /// </summary>
        private void TbClientInputCount_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                AcceptClientInputForResults();
            }
        }

        /// <summary>
        /// Clears dgvTournamentResults and repopulates with the winners
        /// </summary>
        private void AcceptClientInputForResults()
        {
            this.dgvTournamentResults.DataSource = null;
            this.dgvTournamentResults.Rows.Clear();
            this.dgvTournamentResults.Columns.Clear();

            if (tbClientInputCount.Text == null || tbClientInputCount.Text == "")
            {
                MessageBox.Show("Please Enter Number Of Winners");
            }
            else
            {
                try
                {
                    clientInput = Convert.ToInt32(tbClientInputCount.Text);
                    tbClientInputCount.Enabled = false;

                    // Create list of participants list for client request of how many show up in tournament results
                    clientRequested = Calculations.Calculations.MakeTopMembersByPlacementList(winners, clientInput);

                    // Create datagridview and populate with cashedWinners list
                    CreateDataGridView(clientRequested, clientInput);
                }
                catch (FormatException)
                {
                    MessageBox.Show("Please enter a nunmber");
                }
            }
        }

        /// <summary>
        /// Populates dgvTournamentResults when clicked on
        /// </summary>
        private void BtnPaste_Click(object sender, EventArgs e)
        {
            // Stops this method from working if user didnt enter the number of winners
            if (string.IsNullOrWhiteSpace(tbClientInputCount.Text))
            {
                MessageBox.Show("Please enter the number of winners first");
                return;
            }

            // Stops this method from working if the user did not copy from Excel first
            string clipboard = Clipboard.GetText();
            if (string.IsNullOrWhiteSpace(clipboard))
            {
                MessageBox.Show("Please copy the earnings from Excel first");
                return;
            }

            // Removes all $ symboles
            clipboard = clipboard.Replace("$", "");

            // Lines becomes clipboard as an array
            string[] lines = clipboard.Replace("\n", "").Split('\r');
            // Lines2 becomes an empty version of lines
            string[] lines2 = new string[lines.Length];

            // Populates lines2 with all values in lines
            for(int t = 0; t < lines.Length; t++)
            {
               lines2[t] = lines[t];
            }
            int row = 0;
            int col = 4;

            int pasteAble = Convert.ToInt32(tbClientInputCount.Text) + 3; // +3 for the pro pot entries
            int pasteCount = lines.Length;
            int paste;
            if (pasteCount < pasteAble)
            {
                paste = pasteCount - 1;
            }
            else
            {
                paste = pasteAble; 
            }

            // Populates dgvTournamentResults
            for (int i = 0; i < paste; i++)
            {
                string check = lines2[i];
                if (check != "")
                {
                    if (i == 1 || i == 3 || i == 5)
                    {
                        dgvTournamentResults[col + 3, row].Value = lines2[i];
                        row++;
                    }
                    else
                    {
                        dgvTournamentResults[col, row].Value = lines2[i];
                        if (i > 5) {
                            row++;
                        }
                    }
                }
            }
        }
    }
}
