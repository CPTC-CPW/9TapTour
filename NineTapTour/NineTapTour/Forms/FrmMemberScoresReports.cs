using System;
using System.Collections.Generic;
using System.IO;
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
                //See if they want the date for membership dues to be printed.
                if (cbPrintDues.Checked)
                {
                    printDues = true;
                }
                temp = Calculations.Calculations.MakeTopMembersByPlacementList(temp, numMembers); // results of inquiry
                // loads the loading screen if takes long time
                bool wait = true;
                while (wait)
                {
                    frmPleaseWait please = new frmPleaseWait();
                    please.Show();
                    exportToExcel(); // Exports to excel file
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
            // this is used in a few places for labeling file name and displayed on the excel sheet
            string reportTypeToSave = "";
            string reportLabelToSave = "Game";
            // if its highseries display series  otherwise will display game
            // but will chage the save name
            if (reportTypeNum.ToString() == "HighSeries")
            {
                reportLabelToSave = "Series";
                reportTypeToSave = "Series";
            }
            else if (reportTypeNum.ToString() == "HighGameSenior")
            {
                reportTypeToSave ="Senior"; 
            }
            else
            {
                reportTypeToSave = "FinalGame";
            }
            // Has thr program open template file automatically and auto save
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

            // create the name of the file by adding together the type of report, location, the event, and the date of tourn.
            string fileName = reportTypeToSave + selectedTournament.Location + " " + selectedTournament.Event + " " + tournamentDate + ".xls";

            // save the file with this name
            string saveFile =  fileName;

            int i = 0; // which row to save data into
            int j = 0; // which column to save the data into
         
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
                // opens the file that will be written to
                xlWorkBook = xlApp.Workbooks.Open(getFilePath, misValue, misValue, misValue, misValue, misValue,
                                                   misValue, misValue, misValue, misValue, misValue, misValue,
                                                   misValue, misValue, misValue);

                // gets the sheet on the excel file that will be written to
                xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

                // adds in the tourney location 
                xlWorkSheet.Cells[3, 1] = selectedTournament.Location;
                // adds in the event name 
                xlWorkSheet.Cells[3, 4] = selectedTournament.Event;
                // adds in the date of the tourney
                xlWorkSheet.Cells[3, 5] = selectedTournament.Date;
                // adds changes game or series as needed
                xlWorkSheet.Cells[4, 2] = reportLabelToSave;
                


                int printDuesOffset = 0;
                if ( printDues )
                {
                    xlWorkSheet.Cells[4, 5] = "Membership Paid To";
                    printDuesOffset = 1;
                }

                //// use these loops to populate data to be displayed
                for (i = 5; i <= numMembers + 4; i++)
                {
                    for (j = 1; j <= (4+printDuesOffset); j++) // five columns wide 
                    {
                        // first insert a new line into the excel spreadsheet
                        if (i >= 30 && j == 1)
                        {
                            // Get the range on where to insert a new row into the spreadsheet
                            Excel.Range line = (Excel.Range)xlWorkSheet.Rows[i];

                            //insert the new row
                            line.Insert();
                        }
                        
                        // Adds the finish place
                        if (j == 1) 
                        {
                            xlWorkSheet.Cells[i,j] = temp[i-5].placing;
                        }

                        // Add the series or game depending what clicked
                        if (j == 2)
                        {
                            xlWorkSheet.Cells[i, j] = temp[i-5].Score; // "Series"; 
                        }

                        // Adds the member number
                        if (j == 3)
                        {
                            xlWorkSheet.Cells[i, j] = temp[i-5].MemberId; 
                        }

                        // Adds the name
                        if (j == 4)
                        {
                            xlWorkSheet.Cells[i, j] = temp[i-5].LastName + ", " + temp[i - 5].FirstName; 
                        }

                        //Add Membership Paid To
                        if (j == 5)
                        {
                            String paymentYear = temp[i - 5].LastPaymentYear;
                            if (paymentYear != "") {
                                if (paymentYear != "life ")
                                {
                                    int year = 0;
                                    int.TryParse(paymentYear, out year);
                                    year += 1;
                                    paymentYear = Convert.ToString(year);
                                    xlWorkSheet.Cells[i, j] = paymentYear;
                                } else
                                {
                                    
                                    xlWorkSheet.Cells[i, j] = temp[i - 5].LastPaymentYear;
                                }
                            }
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
            catch (Exception e)
            {
                // if the workbook does not get opened, display an error message
                MessageBox.Show("Must choose a file to export to.");
                MessageBox.Show(e.StackTrace);
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
                MessageBox.Show("Exception Occurred while releasing object " + ex.ToString());
            }
            finally
            {
                GC.Collect();
            }
        }

    }
}
