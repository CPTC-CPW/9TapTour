using NineTapTour.Database;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NineTapTour.Models;
using Excel = Microsoft.Office.Interop.Excel;
using System.Collections;

namespace NineTapTour.Forms
{
    public partial class FrmTournamentResults : Form
    {
        // Set column names for datagridview
        static string PLACE_STANDING_COLUMN_NAME = "Place";
        static string FULLNAME_COLUMN_NAME = "Full Name";
        static string HANDICAP_COLUMN_NAME = "Handicap";
        static string TOTAL_SCORE_COLUMN_NAME = "Total Score";
        static string EARNINGS_COLUMN_NAME = "Earnings";
        static string MEMBER_ID_COLUMN_NAME = "Member ID";
        static string GAME_ID_COLUMN_NAME = "Game ID";
        static string PROGRESSIVEPOT_COLUMN_NAME = "Progressive Pot";

        DataTable dt = new DataTable(); // Instantiate Data Table
        NineTapDb db = new NineTapDb(); // Get access to database
        Tournament tourny = frmMemberScores.selectedTournament; // Get Tournament
        static int totalTournamentEntries;  // Total number of entries for all squads in tournament
        static int clientInput; // how many winners the client wants to see
        List<ExcelMember> cashedWinners = new List<ExcelMember>();

        // Floor directors get a comp entry into tournament when they help with tournament. 
        // They don't pay the entry fee, but do qualify to cash.
        static int compEntries; 
                                
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
            List<ExcelMember> winners = BuildWinnersList();

            // Create list of participants who cash
             cashedWinners = Calculations.Calculations.
                    MakeTopMembersByPlacementList(winners, totalTournamentEntries, compEntries);


            ActiveControl = tbClientInputCount;
        }

        /// <summary>
        /// Creates the DataGridView table and populates it with the list of cashed winners
        /// </summary>
        /// <param name="cashedWinners"></param>
        private void CreateDataGridView(List<ExcelMember> cashedWinners, int clientInput)
        {
            // Create data table and add columns 
            // Columns with ReadOnly set to False are editable        
            dt.Columns.Add(PLACE_STANDING_COLUMN_NAME).ReadOnly = true;
            dt.Columns.Add(FULLNAME_COLUMN_NAME).ReadOnly = true;
            dt.Columns.Add(HANDICAP_COLUMN_NAME).ReadOnly = true;
            dt.Columns.Add(TOTAL_SCORE_COLUMN_NAME).ReadOnly = true;
            dt.Columns.Add(EARNINGS_COLUMN_NAME).ReadOnly = false;
            dt.Columns.Add(MEMBER_ID_COLUMN_NAME).ReadOnly = true;
            dt.Columns.Add(GAME_ID_COLUMN_NAME).ReadOnly = true;
            dt.Columns.Add(PROGRESSIVEPOT_COLUMN_NAME).ReadOnly = false;


            // testing code for client input
            int winnersCount = 0;
            if(cashedWinners.Count() > 0)
            {
                winnersCount = cashedWinners.Count();
            }

            
            if (clientInput > cashedWinners.Count)
            {
                for (int z = cashedWinners.Count; z < clientInput; z++)
                {
                    // insert the cash data
                }
            }
            double earnings = 0.00;

            int itemCount = 0;
            int MonEarnCount = 0;
            if (TempVariablesForGlobalLevel.MoneyEarnings != null)
            {
                MonEarnCount = TempVariablesForGlobalLevel.MoneyEarnings.Count();
            }
            
            
            // Create rows and populate with each member's data for each row
            foreach (var item in cashedWinners)
            {

                DataRow newRow = dt.NewRow();
                if (MonEarnCount > 0)
                {
                    newRow[EARNINGS_COLUMN_NAME] =  TempVariablesForGlobalLevel.MoneyEarnings[itemCount]; 
                }
                else
                {
                    newRow[EARNINGS_COLUMN_NAME] = Convert.ToInt32(item.MoneyWon);
                }
                
                newRow[PLACE_STANDING_COLUMN_NAME] = item.PlaceStanding;
                newRow[FULLNAME_COLUMN_NAME] = item.Name;
                newRow[HANDICAP_COLUMN_NAME] = item.Handicap;
                newRow[TOTAL_SCORE_COLUMN_NAME] = item.TotalScore;
                newRow[MEMBER_ID_COLUMN_NAME] = item.MemberNumber;
                newRow[GAME_ID_COLUMN_NAME] = item.GameId;

                if (item.SidePot == null)
                {
                    newRow[PROGRESSIVEPOT_COLUMN_NAME] = "0.00";
                }
                else
                {
                    newRow[PROGRESSIVEPOT_COLUMN_NAME] = item.SidePot;
                }
                dt.Rows.Add(newRow);
                itemCount++;
            }

            int countTempMoney = 0;
                        
            for (int tr = winnersCount; tr < clientInput; tr++)
            {
                DataRow newRow = dt.NewRow();
                if (MonEarnCount > 0)
                {
                    if(tr < MonEarnCount)
                    {
                        newRow[EARNINGS_COLUMN_NAME] = TempVariablesForGlobalLevel.MoneyEarnings[tr];
                    }
                    if(tr >= MonEarnCount)
                    {
                        newRow[EARNINGS_COLUMN_NAME] = earnings;
                    }
                }
                else
                {
                    newRow[EARNINGS_COLUMN_NAME] = earnings;
                }
                newRow[PLACE_STANDING_COLUMN_NAME] = tr + 1;
                newRow[FULLNAME_COLUMN_NAME] = "";
                newRow[HANDICAP_COLUMN_NAME] = "";
                newRow[TOTAL_SCORE_COLUMN_NAME] = "";
                //newRow[EARNINGS_COLUMN_NAME] = earnings; // earnings;
                newRow[MEMBER_ID_COLUMN_NAME] = "";
                newRow[GAME_ID_COLUMN_NAME] = tr;
                newRow[PROGRESSIVEPOT_COLUMN_NAME] = "0.00";
                dt.Rows.Add(newRow);
                countTempMoney++;
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

        private void dgvTournamentResults_CellEnter(object sender, DataGridViewCellEventArgs e)
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
            List<ExcelMember> tournyBowlers = new List<ExcelMember>();

            // Get participant/member/game info to populate DataTable
            var bowlers = (from p in db.Participants
                           join m in db.Members on p.Member.Id equals m.Id
                           join g in db.Games on p.Game.Id equals g.Id
                           join t in db.Tournaments on p.Tournament.Id equals t.Id
                           let memberNumber = m.Number
                           let name = m.FirstName + " " + m.LastName
                           where tourny.Id == p.Tournament.Id
                           select new
                           {
                               g.PlaceStanding,
                               memberNumber,
                               name,
                               g.Handicap,
                               g.Bonus,
                               g.MoneyWon,
                               g.SidePot,
                               g.Id,
                               g.Game1,
                               g.Game2,
                               g.Game3,
                               g.Game4,
                               g.IsComp
                           }).ToList();

            totalTournamentEntries = bowlers.Count();

            // Use each anonymous type (bowler) to create a new ExcelMember object
            // and add them to the winners list
            foreach (var b in bowlers)
            {
                if (b.IsComp)
                {
                    compEntries++;
                }

                ExcelMember m = new ExcelMember();
                m.MemberNumber = b.memberNumber;
                m.Name = b.name;
                m.Handicap = Convert.ToInt32(b.Handicap);
                m.Bonus = Convert.ToInt32(b.Bonus);
                m.MoneyWon = b.MoneyWon;
                m.SidePot = b.SidePot;
                m.GameId = b.Id;
                m.Game1Score = Convert.ToInt32(b.Game1);
                m.Game2Score = Convert.ToInt32(b.Game2);
                m.Game3Score = Convert.ToInt32(b.Game3);
                m.Game4Score = Convert.ToInt32(b.Game4);
                m.TotalScore = m.Game1Score + m.Game2Score + m.Game3Score
                        + m.Game4Score + (4 * (m.Handicap + m.Bonus));

                // If tournament is 3 out of 4, then drop lowest score
                if (tourny.ThreeOutOf4)
                {
                    List<int> scores = new List<int>();
                    scores.Add(m.Game1Score);
                    scores.Add(m.Game2Score);
                    scores.Add(m.Game3Score);
                    scores.Add(m.Game4Score);
                    int lowScore = scores.Min();
                    m.TotalScore -= lowScore + m.Handicap + m.Bonus;
                }
                tournyBowlers.Add(m);
            }
            return tournyBowlers;
        }

        /***************************
              EXPORT TO EXCEL
         **************************/
        private void btnExportToExcel_Click(object sender, EventArgs e)
        {
            bool wait = true;
            while (wait)
            {
                frmPleaseWait please = new frmPleaseWait();
                please.Show();
                exportToExcel();
                wait = false;
                please.Close();
            }
        }
        
        private void exportToExcel()
        {
            /// <summary>
            /// Saves participants' place standing and earnings won to the database
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
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
            string saveFile = @"\Documents\" + fileName;

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
                xlWorkBook = xlApp.Workbooks.Open(getFilePath, misValue, misValue, misValue, misValue, misValue,
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
                                string place = getPlace(data);

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
                    formatBigTie(tempData3, tiePlace, xlWorkSheet, i);
                }

                //set the Total Payout to the correct number
                setTotalPayout(xlWorkSheet);

                // saves the excel file with the file name
                try
                {
                    if (fileName != "TournamentResultsTemplate.xls" || !string.IsNullOrEmpty(fileName))
                    {
                        SaveFileDialog savefile = new SaveFileDialog();
                        savefile.Filter = "Excel Files (*.xls)|*.xls";
                        savefile.FileName = fileName;
                        savefile.ShowDialog();

                        fileName = savefile.FileName;

                        xlWorkBook.SaveAs(fileName, Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
                        MessageBox.Show("Excel file created , you can find the file at: " + fileName);
                    }
                }
                catch
                {
                    MessageBox.Show("Either you cancelled the file save, or the file is already created and was open when you tried to save over it.");
                }

                xlWorkBook.Close(true, misValue, misValue);
                xlApp.Quit();

                releaseObject(xlWorkSheet);
                releaseObject(xlWorkBook);
                releaseObject(xlApp);
            }
            catch
            {
            // if the workbook does not get opened, display an error message
            MessageBox.Show("Must choose a file to export to.");
            xlWorkBook.Close(true, misValue, misValue);
            xlApp.Quit();
            }
        }

        //gets and sets the total amount of payout for the 
        //winners in the total payout box of the excel sheet
        private void setTotalPayout(Excel.Worksheet xlWorkSheet)
        {
            double money = 0;

            for(int i = 0; i < dt.Rows.Count; i++)
            {
                money += Convert.ToDouble(dt.Rows[i].ItemArray[4]);
            }
            xlWorkSheet.Cells[2, 8] = money.ToString();
        }

        //format and populate 
        private void formatBigTie(string tempData3, int tiePlace, Excel.Worksheet xlWorkSheet, int i)
        {
            for (i = 0; i < tiePlace; i++)
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

        /// <summary>
        /// Checks to see what place the players have placed at 
        /// and then returns the appropriate ending for the place
        /// standing to be added onto the number they placed. It 
        /// will either be "st", "nd", "rd", or "th".
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private string getPlace(string data)
        {
            if (data == "1" || data == "21" || data == "31" || data == "41" || data == "51" || data == "61" || data == "71" || data == "81" || data == "91")
            {   // if it is the 1st, 21st, 31st, 41st, 51st, 61st, 71st, 81st, or 91st return st
                return "st";
            }
            else if (data == "2" || data == "22" || data == "32" || data == "42" || data == "52" || data == "62" || data == "72" || data == "82" || data == "92")
            {   // if it is the 2nd, 22nd, 32nd, 42nd, 52nd, 62nd, 72nd, 82nd, or 92nd return nd
                return "nd";
            }
            else if (data == "3" || data == "23" || data == "33" || data == "43" || data == "53" || data == "63" || data == "73" || data == "83" || data == "93")
            {   // if it is the 3rd, 23rd, 33rd, 43rd, 53rd, 63rd, 73rd, 83rd, or 93rd return rd
                return "rd";
            }
            else
            {   // if it is any other number than the ones above return th
                return "th";
            }
        }

        /// <summary>
        /// This method is used to clean up the references to the Excel Objects
        /// so that Excel does not remain running.
        /// </summary>
        /// <param name="obj"></param>
        private void releaseObject(object obj)
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

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            if (e.CloseReason == CloseReason.WindowsShutDown) return;
            List<double> Winnings = new List<double>();
            for(int winningList = 0; winningList < dgvTournamentResults.RowCount; winningList++)
            {
                Winnings.Add( Convert.ToDouble(dgvTournamentResults[EARNINGS_COLUMN_NAME, winningList].Value));
            }
            TempVariablesForGlobalLevel.MoneyEarnings = Winnings;
            
            // Save all changes made to the dataGridView
            if (cashedWinners.Count() > 0)
            {
                for (int currentIndex = 0; currentIndex < dgvTournamentResults.RowCount - cashedWinners.Count(); currentIndex++)
                {
                    int gameId = Convert.ToInt32(dgvTournamentResults[GAME_ID_COLUMN_NAME, currentIndex].Value.ToString());
                    Game g = GameDB.GetGame(gameId);

                    g.PlaceStanding = Convert.ToByte(dgvTournamentResults[PLACE_STANDING_COLUMN_NAME, currentIndex].Value);
                    g.MoneyWon = Convert.ToDecimal(dgvTournamentResults[EARNINGS_COLUMN_NAME, currentIndex].Value);
                    g.SidePot = Convert.ToDecimal(dgvTournamentResults[PROGRESSIVEPOT_COLUMN_NAME, currentIndex].Value);
                    g.gameRegionID = tourny.TourneyRegion;

                    db.Entry(g).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
            }
            
        }

        private void tbClientInputCount_TextChanged(object sender, EventArgs e)
        {
           // dgvTournamentResults.Rows.Clear();
           // dgvTournamentResults.Refresh();
        }

        private void tbClientInputCount_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
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
                        // Create datagridview and populate with cashedWinners list
                        CreateDataGridView(cashedWinners, clientInput);
                    }
                    catch(FormatException)
                    {
                        MessageBox.Show("Please enter a nunmber");
                    }

                }
            }
        }
    }
}
