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

namespace NineTapTour.Forms
{
    public partial class FrmMemberScoresReports : Form
    {
        // the members and their scores
        List<frmMemberScores.MemberScores> temp;
        // used in the print class to print the date and location
        Tournament selectedTournament;
        // to know the report type
        // 0 for High game handicap/senior, 1 for game/high game, 2 for series/high series
        int reportTypeNum;
        int currentSquad;

        public FrmMemberScoresReports(List<frmMemberScores.MemberScores> temp, Tournament selectedTournament, int reportTypeNum, int currentSquad)
        {
            InitializeComponent();
            this.temp = temp;
            this.selectedTournament = selectedTournament;
            this.reportTypeNum = reportTypeNum;
            this.currentSquad = currentSquad;
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
                    // print( go to print class )
                    Database.Print.printMemberReport(temp, selectedTournament, reportTypeNum,currentSquad);

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
    }
}
