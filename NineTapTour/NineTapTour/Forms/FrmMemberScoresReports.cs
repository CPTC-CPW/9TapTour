using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NineTapTour.Models;
using static NineTapTour.Database.ReportHelper;

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
                MessageBox.Show("There are only " + temp.Count + " participant/s in the tournament selected.");
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
                // code to save goes here
               

                this.Close();
            }
            // if user inputs a bigger number than the number of members
            else
            {
                MessageBox.Show("There are only " + temp.Count + " participant/s in the tournament selected.");
            }
        }
    }
}
