using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NineTapTour.Models;
using static NineTapTour.Database.ReportHelper;
using Excel = Microsoft.Office.Interop.Excel;

namespace NineTapTour.Forms
{
    public partial class FrmMemberScoresReports : Form
    {
        // the members and their scores
        List<Models.MemberScores> temp;
        // used in the print class to print the date and location
        Tournament selectedTournament;
        
        ReportType reportTypeNum;
        int currentSquad;
        List<int> squadList;
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

        private void btnPrint_Click(object sender, EventArgs e)
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

                temp = Calculations.Calculations.MakeTopMembersByPlacementList(temp, numMembers);
                // print( go to print class )
                Database.Print.printMemberReport(temp, selectedTournament, reportTypeNum, currentSquad, squadList, printDues);

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

        private void btnSave_Click(object sender, EventArgs e)
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
                // code to save goes here --------------------------------------------------
                temp = Calculations.Calculations.MakeTopMembersByPlacementList(temp, numMembers);
                // loads the loading screen if takes long time
                bool wait = true;
                while (wait)
                {
                    frmPleaseWait please = new frmPleaseWait();
                    please.Show();
                    exportToExcel(); //temp, selectedTournament, reportTypeNum, currentSquad, squadList, printDues
                    wait = false;
                    please.Close();
                }

                this.Close();
            }
            // if user inputs a bigger number than the number of members
            else
            {
                MessageBox.Show("There are only " + temp.Count + " participants in the tournament selected.");
            }
        }

        private void exportToExcel()
        {
           
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
            string getFilePath = Path.GetFullPath("Resources/SeriesReportTemplate.xls");

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
            
            int tiePlace = 0;

            Microsoft.Office.Interop.Excel.Application xlApp; // used to open the excel application
            Microsoft.Office.Interop.Excel.Workbook xlWorkBook; // used to open the worksheet
            Microsoft.Office.Interop.Excel.Worksheet xlWorkSheet; // this is the sheet of the excel worksheet
            object misValue = System.Reflection.Missing.Value;

            xlApp = new Microsoft.Office.Interop.Excel.Application(); // open the excel application
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
                        if (i < 3)
                        {
                            //store the place of the next player for comparison of ties
                            tempData = dt.Rows[i + 1].ItemArray[j].ToString();

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
                                if (i == 0)
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

                            if (j == 6)
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
                                if ((i + 1) < dt.Rows.Count)
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
                                if (data == "1" || data == "2" || data == "3")
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

                // saves the excel file with the file name
                try
                {
                    if (fileName != "SeriesReportTemplate.xls" || !string.IsNullOrEmpty(fileName))
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

    }
}
