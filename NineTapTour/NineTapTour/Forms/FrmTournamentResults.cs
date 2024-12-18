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
using Excel = Microsoft.Office.Interop.Excel;
using System.Collections;
using System.Text.RegularExpressions;
using NineTapTour.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace NineTapTour.Forms
{
    public partial class FrmTournamentResults : Form
    {
        // Names of the Colums in the DataGridView
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

                g.PlaceStanding = Convert.ToByte(dgvTournamentResults[PLACE_STANDING_COLUMN_NAME, currentIndex].Value);
                g.MoneyWon = Convert.ToDecimal(dgvTournamentResults[EARNINGS_COLUMN_NAME, currentIndex].Value);

                // if user enters something other than a decimal number, set SidePot to 0.00 and enter the string into notes
                if (Decimal.TryParse(Convert.ToString(dgvTournamentResults[PROGRESSIVEPOT_COLUMN_NAME, currentIndex].Value), out decimal a))
                {
                    g.SidePot = Convert.ToDecimal(dgvTournamentResults[PROGRESSIVEPOT_COLUMN_NAME, currentIndex].Value);
                }
                else
                {
                    g.Notes = $"Progressive Pot was entered as: {Convert.ToString(dgvTournamentResults[PROGRESSIVEPOT_COLUMN_NAME, currentIndex].Value)}";
                }

                g.gameRegionID = tourny.TourneyRegion;
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

            // Sets winnersCount to clientRequested or 0, whichever is bigger
            int winnersCount = 0;
            if(clientRequested.Count() > 0)
            {
                winnersCount = clientRequested.Count();
            }
            
            double earnings = 0.00;

            int MonEarnCount = 0;
            if (TempVariablesForGlobalLevel.MoneyEarnings != null)
            {
                MonEarnCount = TempVariablesForGlobalLevel.MoneyEarnings.Count;
            }

            // Create rows and populate with each member's data for each row
            for (int wc = 0; wc < clientRequested.Count(); wc++)
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

            for (int tr = clientRequested.Count(); tr < clientInput ; tr++)
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

            totalTournamentEntries = bowlers.Count();

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
                    if (scores.Count() == 4)
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
                        + (scores.Count() * (m.Handicap + m.Bonus));
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
        
        private void ExportToExcel()
        {
            /// <summary>
            /// Saves participants' place standing and earnings won to the database
            /// </summary>
            for (int currentIndex = 0; currentIndex < dgvTournamentResults.RowCount; currentIndex++)
            {
                int gameId = Convert.ToInt32(dgvTournamentResults[GAME_ID_COLUMN_NAME, currentIndex].Value.ToString());
                Game g = GameDB.GetGame(gameId);

                g.PlaceStanding = Convert.ToByte(dgvTournamentResults[PLACE_STANDING_COLUMN_NAME, currentIndex].Value);
                g.MoneyWon = Convert.ToDecimal(dgvTournamentResults[EARNINGS_COLUMN_NAME, currentIndex].Value);
                g.SidePot = Convert.ToDecimal(dgvTournamentResults[PROGRESSIVEPOT_COLUMN_NAME, currentIndex].Value);
              
                g.gameRegionID = tourny.TourneyRegion;
                
                db.SaveChanges();
            }
            

            // have program open template file automatically and auto save
            // with a specific naming conventions such as "Pacific 3Of4 1-12-18" 
            // without using open/save file dialogues
            // get the full path to where the tournament results template is located
            string getFilePath = Path.GetFullPath("Resources/TournamentResultsTemplate.xls");

            // get the date of the tourney and convert it to a string
            string tourneyDate = tourny.Date.ToString("MM/dd/yyyy");

            // replace the forward slashes with a dash
            string tournyDate = tourneyDate.Replace("/", "-");

            // remove the time from the end of the date
            string tournamentDate = tournyDate.Replace(tourneyDate, "");

            // create the name of the file by adding together the location, the event, and the
            // date of the tournament
            string fileName = tourny.Location + " " + tourny.Event + " " + tournamentDate + ".xls";

            // save the file in the documents folder
            string saveFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "TournamentResultsCopy.xls");

            // Copy template file to modify
            File.Copy(getFilePath, saveFile, true);

            string data = null; // the data to be added to the excel spreadsheet cells
            string tempData = null;
            string tempData2 = null;
            string tempData3 = null;
            
            int i = 0; // used to determine which row to save data into
            int j = 0; // used to determine which column to save the data into
            bool FormatBool = false;
            int tiePlace = 0;

            Excel.Application xlApp; // used to open the excel application
            Excel.Workbook xlWorkBook; // used to open the worksheet
            Excel.Worksheet xlWorkSheet; // this is the sheet of the excel worksheet
            object misValue = System.Reflection.Missing.Value;

            xlApp = new Excel.Application(); // open the excel application
            xlWorkBook = xlApp.Workbooks.Add(misValue);

            // get and open the excel file:
            try
            {
                // opens the file that will be written to
                xlWorkBook = xlApp.Workbooks.Open(saveFile, misValue, misValue, misValue, misValue, misValue,
                                                   misValue, misValue, misValue, misValue, misValue, misValue,
                                                   misValue, misValue, misValue);

                // gets the sheet on the excel file that will be written to
                xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

                // adds in the tourney location in the cell A1
                xlWorkSheet.Cells[1, 1] = tourny.Location + tourny.Event;

                // adds in the date of the tourney in the cell A2
                xlWorkSheet.Cells[2, 1] = tourny.Date;

                // use these for loops to populate data in each of
                // the rows and cells that have data
                for (i = 0; i < dt.Rows.Count; i++)
                {
                    for (j = 0; j <= dt.Columns.Count - 1; j++)
                    {
                        data = dt.Rows[i].ItemArray[j].ToString();

                        // runs for the first 3 places in the data table
                        // due to their special formatting
                        if(i < 3)
                        {
                            //store the place of the next player for comparison of ties
                            tempData = dt.Rows[i + 1].ItemArray[j].ToString();

                            // add place standing into the first column of the current row
                            if (j == 0)
                            {
                                //check for first place
                                if (data == "1")
                                {
                                    // check for 1st place tie
                                    if (i > 0 || data == tempData)
                                    {
                                        // add place into 1st column with a "T" for tie
                                        xlWorkSheet.Cells[i + (i + 4), j + 1] = data + "stT";

                                        // add placement to 2nd column 1 row down
                                        xlWorkSheet.Cells[(i * 2) + 5, j + 2] = "1st Place";

                                        // add place without "st" into column 11
                                        xlWorkSheet.Cells[i + (i + 4), j + 11] = data;
                                    }
                                    
                                    else
                                    {   // no tie
                                        // add place into 1st column
                                        xlWorkSheet.Cells[i + (i + 4), j + 1] = data + "st";

                                        // add place without "st" into column 11
                                        xlWorkSheet.Cells[i + (i + 4), j + 11] = data;
                                    }
                                }
                                else if (data == "2")
                                {   //check for second place
                                    // check for second place tie
                                    if (i > 1 || data == tempData)
                                    {
                                        // add place into 1st column with a "T" for tie
                                        xlWorkSheet.Cells[i + (i + 4), j + 1] = data + "ndT";

                                        // add placement to 2nd column 1 row down
                                        xlWorkSheet.Cells[(i * 2) + 5, j + 2] = "2nd Place";

                                        // add place without "st" into column 11
                                        xlWorkSheet.Cells[i + (i + 4), j + 11] = data;
                                    }
                                    else
                                    {   //no tie
                                        // add place into 1st column
                                        xlWorkSheet.Cells[i + (i + 4), j + 1] = data + "nd";

                                        // add place without "st" into column 11
                                        xlWorkSheet.Cells[i + (i + 4), j + 11] = data;
                                    }
                                }
                                else
                                {   //else its third place
                                    //check for tie with player below
                                    if (data == tempData)
                                    {
                                        xlWorkSheet.Cells[i + (i + 4), j + 1] = data + "rdT";
                                        xlWorkSheet.Cells[9, j + 2] = "3rd Place";

                                        // add place without "st" into column 11
                                        xlWorkSheet.Cells[i + (i + 4), j + 11] = data;
                                    }
                                    else
                                    {
                                        xlWorkSheet.Cells[i + (i + 4), j + 1] = data + "rd";

                                        // add place without "st" into column 11
                                        xlWorkSheet.Cells[i + (i + 4), j + 11] = data;
                                    }
                                }
                            }

                            // Add the name into the 2nd column of the current row
                            if (j == 1)
                            {
                                xlWorkSheet.Cells[i + (4 + i), j + 1] = data;
                            }

                            // Add the handicap into the 6th column of the current
                            if (j == 2)
                            {
                                xlWorkSheet.Cells[i + (4 + i), j + 4] = data;
                            }

                            // Add the total score into the 7th column of the current row
                            if (j == 3)
                            {
                                xlWorkSheet.Cells[i + (4 + i), j + 4] = data;
                            }

                            // Add the money won into the 9th column of the current row
                            if (j == 4)
                            {
                                xlWorkSheet.Cells[i + (4 + i), j + 5] = data;

                                // put equation into column 15 that will display the total
                                // amount the player earned minus the yearly membership and
                                // any money adjustments
                               if(i == 0)
                                {
                                    xlWorkSheet.Cells[i + (4 + i), j + 11] = "=I" + (i + 4) + "+I" + (i + 5) + "-M" + (i + 4) + "-N" + (i + 4);
                                }
                                else if (i == 1)
                                {
                                    xlWorkSheet.Cells[i + (4 + i), j + 11] = "=I" + (i + 5) + "+I" + (i + 6) + "-M" + (i + 5) + "-N" + (i + 5);
                                }
                                else
                                {
                                    xlWorkSheet.Cells[i + (4 + i), j + 11] = "=I" + (i + 6) + "+I" + (i + 7) + "-M" + (i + 6) + "-N" + (i + 6);
                                }
                            }

                            // Add the member number into the 12th column of the 4th row
                            if (j == 5)
                            {
                                xlWorkSheet.Cells[i + (4 + i), j + 7] = data;
                            }

                            if(j == 6)
                            {
                                xlWorkSheet.Cells[i + (5 + i), 9] = dt.Rows[i].ItemArray[7].ToString();
                            }
                        }

                        // For rows 3 and higher in the data table
                        if (i >= 3)
                        {
                            // first insert a new line into the excel spreadsheet
                            if (i >= 27 && j == 0)
                            {
                                // Get the range on where to insert a new row into the spreadsheet
                                Excel.Range line = (Excel.Range)xlWorkSheet.Rows[i + 7];

                                // insert the new row
                                line.Insert();

                                // get these cells
                                Excel.Range r = xlWorkSheet.get_Range("B" + (i + 7), "E" + (i + 7));

                                // merge the cells of the excel sheet
                                r.MergeCells = true;

                                // get these cells
                                Excel.Range r2 = xlWorkSheet.get_Range("G" + (i + 7), "H" + (i + 7));

                                // merge the cells
                                r2.MergeCells = true;
                            }

                            // Add the place standing and 
                            // displays for example: 4th, 5th, 6th,
                            // 21st, 22nd, 23rd, etc.
                            if (j == 0)
                            {
                                //store the place value of the player before the current
                                tempData = dt.Rows[i - 1].ItemArray[j].ToString();

                                //if there is a next player, grab their place value aswell
                                if((i + 1) < dt.Rows.Count)
                                {
                                    tempData2 = dt.Rows[i + 1].ItemArray[j].ToString();
                                }
                                else
                                {
                                    tempData2 = tempData;
                                }

                                // check the place and then add "st", "nd", "rd" or "th"
                                string place = GetPlace(data);

                                //if the player's score is tied for one of the top 3 spots, format sheet accordingly
                                if(data == "1" || data == "2" || data == "3")
                                {
                                    //set the row with the place and name to bold, with a font size of 16, 
                                    //and set the row's height to 22
                                    xlWorkSheet.Cells[i + 7, 1].EntireRow.Font.Bold = true;
                                    xlWorkSheet.get_Range("B" + (i + 7), "B" + (i + 7)).Cells.Font.Size = 16;
                                    xlWorkSheet.Cells[i + 7, 1].EntireRow.RowHeight = 22;

                                    //set variables used to format the second line
                                    //added that shows placement and money earned i.e. the red text 
                                    tiePlace += 1;
                                    FormatBool = true;
                                    tempData3 = data;
                                    
                                    //Set the finishing place text with a T for tie
                                    xlWorkSheet.Cells[i + 7, j + 1] = data + place + "T";

                                    // add place without "st, nd, rd, or th" into column 11
                                    xlWorkSheet.Cells[i + 7, j + 11] = data;
                                }
                                else if (data == tempData || data == tempData2)
                                {   //check for a tie with either the player before or after the current player
                                    //set the finishing place text with a T for tie
                                    xlWorkSheet.Cells[i + 7, j + 1] = data + place + "T";
                                    // add place without "st, nd, rd, or th" into column 11
                                    xlWorkSheet.Cells[i + 7, j + 11] = data;
                                }
                                else
                                {
                                    //set the finishing place text
                                    xlWorkSheet.Cells[i + 7, j + 1] = data + place;
                                    // add place without "st, nd, rd, or th" into column 11
                                    xlWorkSheet.Cells[i + 7, j + 11] = data;
                                }
                            }

                            // Add the name into the 2nd column in all the rows 10 - 32 of the excel sheet
                            if (j == 1)
                            {
                                xlWorkSheet.Cells[i + 7, j + 1] = data;
                            }

                            // Add the handicap in the 6th column in all the rows 10 - 32 of the excel sheet
                            if (j == 2)
                            {
                                xlWorkSheet.Cells[i + 7, j + 4] = data;
                            }

                            // Add the total score in the 7th column in all the rows 10 - 32 of the excel sheet
                            if (j == 3)
                            {
                                xlWorkSheet.Cells[i + 7, j + 4] = data;
                            }

                            // Add the money won into the 9th column in all the rows 10 - 32 of the excel sheet
                            if (j == 4)
                            {
                                xlWorkSheet.Cells[i + 7, j + 5] = data;

                                // put equation into column 15 that will display the total
                                // amount the player earned minus the yearly membership and
                                // any money adjustments
                                xlWorkSheet.Cells[i + 7, j + 11] = "=I" + (i + 7) + "-M" + (i + 7) + "-N" + (i + 7);
                            }

                            // Add the member number into the 12th column in all the rows 10 - 32 of the excel sheet
                            if (j == 5)
                            {
                                xlWorkSheet.Cells[i + 7, j + 7] = data;
                            }
                        }

                        // if we are on last row and i is less than 28
                        if (i == dt.Rows.Count - 1 && i < 27 && j == 5)
                        {
                            // grab the extra rows in the excel spreadsheet
                            Excel.Range range = xlWorkSheet.get_Range("A" + (i + 8), "O" + 34);
                            // delete the extra rows
                            range.Delete();
                            // calculate the total amount of money that was paid out
                            //xlWorkSheet.Cells[2, 8] = "=SUM(I" + 4 + ":I" + (i + 7) + ")";
                        }
                    }
                }

                if (FormatBool)
                {
                    FormatBigTie(tempData3, tiePlace, xlWorkSheet);
                }

                //set the Total Payout to the correct number
                SetTotalPayout(xlWorkSheet);

                // saves the excel file with the file name
                try
                {
                    if (!(fileName == "TournamentResultsTemplate.xls" && string.IsNullOrEmpty(fileName)))
                    {
                        SaveFileDialog savefile = new()
                        {
                            Filter = FileHelper.GetExcelFilterStringForFileDialogs(),
                            FileName = fileName
                        };
                        DialogResult result = savefile.ShowDialog();

                        if(result == DialogResult.OK)
                        {
                            fileName = savefile.FileName;

                            xlWorkBook.SaveAs(fileName, Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
                            MessageBox.Show("Excel file created , you can find the file at: " + fileName);
                        }
                    }
                }
                catch
                {
                    MessageBox.Show("The file may already be created and was open when you tried to save over it.");
                }

                xlWorkBook.Close(true, misValue, misValue);
                xlApp.Quit();

                ReleaseObject(xlWorkSheet);
                ReleaseObject(xlWorkBook);
                ReleaseObject(xlApp);
            }
            catch
            {
            // if the workbook does not get opened, display an error message
            MessageBox.Show("Must choose a file to export to. \n" +
                            " *Must have at least 20 bowlers and 4 money winners* ");
            xlWorkBook.Close(true, misValue, misValue);
            xlApp.Quit();
            }
        }

        /// <summary>
        /// Gets and sets the total amount of payout for the 
        /// winners in the total payout box of the excel sheet
        /// </summary>
        private void SetTotalPayout(Excel.Worksheet xlWorkSheet)
        {
            double money = 0;

            for(int i = 0; i < dt.Rows.Count; i++)
            {
                money += Convert.ToDouble(dt.Rows[i].ItemArray[4]);
            }
            xlWorkSheet.Cells[2, 8] = money.ToString();
        }
        
        //format and populate 
        private void FormatBigTie(string tempData3, int tiePlace, Excel.Worksheet xlWorkSheet)
        {
            for (int i = 0; i < tiePlace; i++)
            {
                //get range on which to insert extra line
                Excel.Range line = (Excel.Range)xlWorkSheet.Rows[(i * 2) + 11];

                // insert the new row
                line.Insert();

                //copy the formatting from row 9, column B
                Excel.Range R5 = (Excel.Range)xlWorkSheet.Cells[9, 2];
                R5.Copy(Type.Missing);

                //apply format to the current row, column B
                Excel.Range R6 = (Excel.Range)xlWorkSheet.Cells[(i * 2) + 11, 2];
                R6.PasteSpecial(Excel.XlPasteType.xlPasteFormats,
                Excel.XlPasteSpecialOperation.xlPasteSpecialOperationNone, false, false);

                //copy the formatting from row 9, column F
                Excel.Range R1 = (Excel.Range)xlWorkSheet.Cells[9, 6];
                R1.Copy(Type.Missing);

                //apply format to the current row, column F
                Excel.Range R2 = (Excel.Range)xlWorkSheet.Cells[(i * 2) + 11, 6];
                R2.PasteSpecial(Excel.XlPasteType.xlPasteFormats,
                Excel.XlPasteSpecialOperation.xlPasteSpecialOperationNone, false, false);

                //copy the formatting from row 9, column I
                Excel.Range R3 = (Excel.Range)xlWorkSheet.Cells[9, 9];
                R3.Copy(Type.Missing);

                //apply format to the current row, column I
                Excel.Range R4 = (Excel.Range)xlWorkSheet.Cells[(i * 2) + 11, 9];
                R4.PasteSpecial(Excel.XlPasteType.xlPasteFormats,
                Excel.XlPasteSpecialOperation.xlPasteSpecialOperationNone, false, false);

                //copy the formatting from row 9, column C
                Excel.Range R7 = (Excel.Range)xlWorkSheet.Cells[9, 3];
                R7.Copy(Type.Missing);

                //apply format to the current row, column C
                Excel.Range R8 = (Excel.Range)xlWorkSheet.Cells[(i * 2) + 11, 3];
                R8.PasteSpecial(Excel.XlPasteType.xlPasteFormats,
                Excel.XlPasteSpecialOperation.xlPasteSpecialOperationNone, false, false);

                //get the cells from the current cell: current row ((i * 2) + 11), column C
                //to the cell: current row, column E
                Excel.Range r = xlWorkSheet.get_Range("C" + ((i * 2) + 11), "E" + ((i * 2) + 11));

                // merge the selected cells together
                r.MergeCells = true;

                // get these cells from the current cell: current row, column F
                //to the cell: current row, column H
                Excel.Range r2 = xlWorkSheet.get_Range("F" + ((i * 2) + 11), "H" + ((i * 2) + 11));

                // merge the selected cells together
                r2.MergeCells = true;

                //display the player's placement in red text under their name
                if (tempData3 == "1")
                {
                    xlWorkSheet.Cells[(i * 2) + 11, 2] = tempData3 + "st Place";
                }
                else if (tempData3 == "2")
                {
                    xlWorkSheet.Cells[(i * 2) + 11, 2] = tempData3 + "nd Place";
                }
                else if (tempData3 == "3")
                {
                    xlWorkSheet.Cells[(i * 2) + 11, 2] = tempData3 + "rd Place";
                }

                //set money earned in red text next to place
                xlWorkSheet.Cells[(i * 2) + 11, 3] = "=SUM(I" + ((i * 2) + 10) + ":I" + ((i * 2) + 11) + ")";

                //label the progressive prize pot
                xlWorkSheet.Cells[(i * 2) + 11, 6] = "$20 Progressive Pot";

                //Set the Progressive pot earnings
                xlWorkSheet.Cells[(i * 2) + 11, 9] = dt.Rows[i + 3].ItemArray[7].ToString();
            }
        }
        #endregion

        /// <summary>
        /// Checks to see what place the players have placed at 
        /// and then returns the appropriate ending for the place
        /// standing to be added onto the number they placed. It 
        /// will either be "st", "nd", "rd", or "th".
        /// </summary>
        private static string GetPlace(string data)
        {
            // Convert data to an int to easily find its suffix
            int dataNum = Int32.Parse(data);

            // 11, 12, and 13 are exceptions to the rules bellow, ending in "th"
            if(dataNum == 11 || dataNum == 12 || dataNum == 13) {
                return "th";
            }

            // if data ends in 1, return "st" 
            // { 1st, 21st, 41st, etc. }
            if (dataNum % 10 == 1)
            {
                return "st";
            }

            // if data ends in 2, return "nd"
            // { 2nd, 32nd, 52nd, etc. }
            else if (dataNum % 10 == 2)
            {
                return "nd";
            }

            // if data ends in 3, return "rd"
            // { 3rd, 43rd, 63rd, etc. }
            else if (dataNum % 10 == 3)
            {
                return "rd";
            }

            // if it is any other number than the ones above return th
            // { 5th, 26th, 48th, etc. }
            else
            {
                return "th";
            }
        }

        /// <summary>
        /// This method is used to clean up the references to the Excel Objects
        /// so that Excel does not remain running.
        /// </summary>
        /// <param name="obj"></param>
        private static void ReleaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception ex)
            {
                obj = null;
                MessageBox.Show("Exception Occured while releasing object " + ex.ToString());
            }
            finally
            {
                GC.Collect();
            }
        }

        /// <summary>
        /// This method was made by accident, if deleted will mess up tbClientInputCount
        /// </summary>
        private void TbClientInputCount_TextChanged(object sender, EventArgs e) { }

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
            int pasteCount = lines.Count();
            int paste = 0;
            if(pasteCount < pasteAble)
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
