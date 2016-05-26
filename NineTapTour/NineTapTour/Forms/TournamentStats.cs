using NineTapTour.Database;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NineTapTour.Database;

namespace NineTapTour.Forms
{
    public partial class TournamentStats : Form
    {
        public TournamentStats()
        {
            InitializeComponent();
        }

        private void TournamentStats_Load(object sender, EventArgs e)
        {
            Tournament selectedTournament = new Tournament();
            selectedTournament = frmMemberScores.selectedTournament;
            lblTournamentName.Text = "Tournament ID: (" + selectedTournament.Id + ")\nTournament Location: " + selectedTournament.Location + "\nDate: " + selectedTournament.Date;

            NineTapDb db = new NineTapDb();
            var tournamentStatsList = (from p in db.Participants
                                       join g in db.Games on p.Game.Id equals g.Id
                                       join t in db.Tournaments on p.Tournament.Id equals t.Id
                                       where t.Id == selectedTournament.Id
                                       orderby (g.Game1 + g.Game2 + g.Game3 + g.Game4) descending
                                       select new
                                       {
                                           p.Member.Id,
                                           p.Member.FirstName,
                                           p.Member.LastName,
                                           Gametotal = ((g.Game1.HasValue ? g.Game1 : 0) + (g.Game2.HasValue ? g.Game2 : 0) + (g.Game3.HasValue ? g.Game3 : 0) + (g.Game4.HasValue ? g.Game4 : 0)),
                                           g.Game1,
                                           g.Game2,
                                           g.Game3,
                                           g.Game4,
                                           p.Member.Handicap,
                                           p.Member.Bonus
                                       }).ToList();

            dgvTournamentStats.DataSource = tournamentStatsList;
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
            Bitmap bm = new Bitmap(this.dgvTournamentStats.Width, this.dgvTournamentStats.Height);
            this.dgvTournamentStats.DrawToBitmap(bm, new Rectangle(0, 0, this.dgvTournamentStats.Width, this.dgvTournamentStats.Height));
            e.Graphics.DrawImage(bm, 0, 0);
        }
    }
}
