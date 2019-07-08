using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NineTapTour.Forms
{
    public partial class TournamentPlaceStandings : Form
    {
        public TournamentPlaceStandings()
        {
            InitializeComponent();
        }

        private void TournamentPlaceStandings_Load(object sender, EventArgs e)
        {
            lblTournamentName.Text = frmMemberScores.selectedTournament.TourneyNameDate;
            if (frmMemberScores.selectedTournament.Doubles)
            {
                lblTournamentName.Text += " (DOUBLES TOURNAMENT)";                 
            }

            if (frmMemberScores.selectedTournament.ThreeOutOf4)
            {
                lblTournamentName.Text += " (3 OUT OF 4 TOURNAMENT)";
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            printDialog1.Document = printDocument1;

            if (printDialog1.ShowDialog() == DialogResult.OK)
            {
                printDocument1.Print();
            }
        }      

        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            Bitmap bm = new Bitmap(this.dgvTournamentStandings.Width, this.dgvTournamentStandings.Height);
            this.dgvTournamentStandings.DrawToBitmap(bm, new Rectangle(0, 0, this.dgvTournamentStandings.Width, this.dgvTournamentStandings.Height));
            e.Graphics.DrawImage(bm, 0, 0);
        }
    }
}
