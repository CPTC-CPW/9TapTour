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
        int seriesCurrentSquad;

        public FrmMemberScoresReports(List<MemberScores> temp, Tournament selectedTournament, ReportType reportTypeNum, int currentSquad, int seriesCurrentSquad)
        {
            InitializeComponent();
            this.temp = temp;
            this.selectedTournament = selectedTournament;
            this.reportTypeNum = reportTypeNum;
            this.currentSquad = currentSquad;
            this.seriesCurrentSquad = seriesCurrentSquad;
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                // if user inputs 0
                if (Convert.ToInt32(txtNumberOfMembers.Text) == 0)
                {
                    MessageBox.Show("Please do not Input 0.");
                }
                // if good to go
                else if (Convert.ToInt32(txtNumberOfMembers.Text) <= temp.Count)
                {
                    // only take the inputted number of members
                    temp = temp.Take(Convert.ToInt32(txtNumberOfMembers.Text)).ToList();
                    Calculations.Calculations.CalculatePlaceStandings(temp);
                    // print( go to print class )
                    Database.Print.printMemberReport(temp, selectedTournament, reportTypeNum, currentSquad, seriesCurrentSquad);

                    this.Close();
                }
                // if user inputs a bigger number than the number of members
                else
                {
                    MessageBox.Show("There are only " + temp.Count + " participant/s in the tournament selected.");
                }
            }
            // if user did not input a number
            catch (FormatException)
            {
                MessageBox.Show("Please only input a number");
            }
        }

        private void FrmMemberScoresReports_Load(object sender, EventArgs e)
        {
            txtNumberOfMembers.Focus();
        }
    }
}
