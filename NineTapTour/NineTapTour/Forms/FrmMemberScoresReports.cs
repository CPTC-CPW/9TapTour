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
            //temp, selectedTournament, reportTypeNum, currentSquad, squadList, printDues
            
            // have program open template file automatically and auto save
            // with a specific naming conventions such as "Series Pacific 3Of4 1-12-18" 
            // without using open/save file dialogues
            // get the full path to where the tournament results template is located
            string getFilePath = Path.GetFullPath("Resources/SeriesReportTemplate.xls");

            // get the date of the tourney and convert it to a string
            string tourneyDate = selectedTournament.Date.ToString("MM/dd/yyyy");

            // replace the forward slashes with a dash
            string tournyDate = tourneyDate.Replace("/", "-");

            // remove the time from the end of the date
            string tournamentDate = tournyDate.Replace(tourneyDate, "");

            // create the name of the file by adding together the location, the event, and the
            // date of the tournament
            string fileName = "Series" + selectedTournament.Location + " " + selectedTournament.Event + " " + tournamentDate + ".xls";

            // save the file in the documents folder
            string saveFile = @"\Documents\" + fileName;

            string data = null; // the data to be added to the excel spreadsheet cells
            string tempData = null;
            string tempData2 = null;
            string tempData3 = null;

            int i = 0; // used to determine which row to save data into
            int j = 0; // used to determine which column to save the data into
            
    

            Excel.Application xlApp; // used to open the excel application
            Excel.Workbook xlWorkBook; // used to open the worksheet
            Excel.Worksheet xlWorkSheet; // this is the sheet of the excel worksheet
            object misValue = System.Reflection.Missing.Value;

            xlApp = new Microsoft.Office.Interop.Excel.Application(); // open the excel application
            xlWorkBook = xlApp.Workbooks.Add(misValue);

            // get and open the excel file:
            try
            {
                // ********************************************************************************************* need to figure out why does not load
                // opens the file that will be written to
                xlWorkBook = xlApp.Workbooks.Open(getFilePath, misValue, misValue, misValue, misValue, misValue,
                                                   misValue, misValue, misValue, misValue, misValue, misValue,
                                                   misValue, misValue, misValue);

                // gets the sheet on the excel file that will be written to
                xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

                // adds in the tourney location in the cell A3
                xlWorkSheet.Cells[3, 1] = selectedTournament.Location + selectedTournament.Event;

                // adds in the date of the tourney in the cell D3
                xlWorkSheet.Cells[3, 4] = selectedTournament.Date;

                //// use these for loops to populate data in each of
                //// the rows and cells that have data
                // ********************************************************************************************* need to finish populating the excel file here
                for (i = 4; i < 5; i++)
                {
                    for (j = 0; j <= 5 - 1; j++)
                    {
                        data = dt.Rows[i].ItemArray[j].ToString();

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

                            // Add the place standing and displays for example: 4th, 5th, 6th, 21st, 22nd, 23rd, etc.
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

                                if (data == tempData || data == tempData2)
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

                            // Add the 
                            if (j == 1)
                            {
                                xlWorkSheet.Cells[i + 7, j + 1] = data;
                            }

                            // Add the
                            if (j == 2)
                            {
                                xlWorkSheet.Cells[i + 7, j + 4] = data;
                            }

                            // Add the 
                            if (j == 3)
                            {
                                xlWorkSheet.Cells[i + 7, j + 4] = data;
                            }

                            // Add the 
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
                    }
                }
                // ********************************************************************************************* need to finish populating the excel above file here

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
