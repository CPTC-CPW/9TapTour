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
        }
    }
}
