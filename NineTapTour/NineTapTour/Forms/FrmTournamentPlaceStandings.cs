using NineTapTour.Models;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace NineTapTour.Forms;

public partial class FrmTournamentPlaceStandings : Form
{
    public FrmTournamentPlaceStandings()
    {
        InitializeComponent();
    }

    private void TournamentPlaceStandings_Load(object sender, EventArgs e)
    {
        lblTournamentName.Text = FrmMemberScoresHelpers.selectedTournament.TourneyNameDate;
        if (FrmMemberScoresHelpers.selectedTournament.Doubles)
        {
            lblTournamentName.Text += " (DOUBLES TOURNAMENT)";                 
        }

        if (FrmMemberScoresHelpers.selectedTournament.ThreeOutOf4)
        {
            lblTournamentName.Text += " (3 OUT OF 4 TOURNAMENT)";
        }
    }

    private void BtnPrint_Click(object sender, EventArgs e)
    {
        printDialog1.Document = printDocument1;

        if (printDialog1.ShowDialog() == DialogResult.OK)
        {
            printDocument1.Print();
        }
    }      

    private void PrintDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
    {
        Bitmap bm = new(this.dgvTournamentStandings.Width, this.dgvTournamentStandings.Height);
        this.dgvTournamentStandings.DrawToBitmap(bm, new Rectangle(0, 0, this.dgvTournamentStandings.Width, this.dgvTournamentStandings.Height));
        e.Graphics.DrawImage(bm, 0, 0);
    }
}
