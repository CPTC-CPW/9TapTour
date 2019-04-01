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
            string tournamentDate = tournyDate.Replace(tourneyDate, " ");

            // create the name of the file by adding together the location, the event, and the
            // date of the tournament
            string fileName = "Series" + selectedTournament.Location + " " + selectedTournament.Event + " " + tournamentDate + ".xls";

            // save the file in the documents folder
            string saveFile = @"\Documents\" + fileName;

            int i = 0; // used to determine which row to save data into
            int j = 0; // used to determine which column to save the data into
         
            Excel.Application xlApp; // used to open the excel application
            Excel.Workbook xlWorkBook; // used to open the worksheet
            Excel.Worksheet xlWorkSheet; // this is the sheet of the excel worksheet
            object misValue = System.Reflection.Missing.Value;

            xlApp = new Excel.Application(); // open the excel application
            xlWorkBook = xlApp.Workbooks.Add(misValue);
            int.TryParse(txtNumberOfMembers.Text, out int numMembers); // how many people to save in the report
            // get and open the excel file:
            try
            {
                // ********************************************************************************************* need to figure out why does not load
                // opens the file that will be written to
                //xlWorkBook = xlApp.Workbooks.Open(getFilePath, misValue, misValue, misValue, misValue, misValue,
                //                                   misValue, misValue, misValue, misValue, misValue, misValue,
                //                                   misValue, misValue, misValue);

                // gets the sheet on the excel file that will be written to
                xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

                // adds in the tourney location in the cell A3
                xlWorkSheet.Cells[3, 1] = selectedTournament.Location;
                xlWorkSheet.Cells[3, 4] = selectedTournament.Event;
                // adds in the date of the tourney in the cell 
                xlWorkSheet.Cells[3, 5] = selectedTournament.Date;

                //// use these for loops to populate data in each of
                //// the rows and cells that have data
                // ********************************************************************************************* need to finish populating the excel file here
                for (i = 5; i <= numMembers; i++)
                {
                    for (j = 1; j <= 5; j++)
                    {
                        // first insert a new line into the excel spreadsheet
                        if (i >= 5 && j == 1)
                        {
                            // Get the range on where to insert a new row into the spreadsheet
                            Excel.Range line = (Excel.Range)xlWorkSheet.Rows[i + 7];

                            // insert the new row
                            line.Insert();

                            //// get these cells
                            //Excel.Range r = xlWorkSheet.get_Range("B" + (i + 7), "E" + (i + 7));

                            //// merge the cells of the excel sheet
                            //r.MergeCells = true;

                            //// get these cells
                            //Excel.Range r2 = xlWorkSheet.get_Range("G" + (i + 7), "H" + (i + 7));

                            //// merge the cells
                            //r2.MergeCells = true;
                        }

                        //temp, selectedTournament, reportTypeNum, currentSquad, squadList, printDues
                        // Adds the finish place
                        if (j == 1) 
                        {
                            xlWorkSheet.Cells[i,j] = (i-4).ToString();
                        }

                        // Add the series or game depending what clicked
                        if (j == 2)
                        {
                            xlWorkSheet.Cells[i, j] = "Series"; 
                        }

                        // Adds the member number
                        if (j == 3)
                        {
                            xlWorkSheet.Cells[i, j] = "Mem#";  
                        }

                        // Adds the name
                        if (j == 4)
                        {
                            xlWorkSheet.Cells[i, j] = "Name"; 
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
