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
    public partial class FrmTournamentStats : Form
    {
        public FrmTournamentStats()
        {
            InitializeComponent();
        }

        private void FrmTournamentStats_Load(object sender, EventArgs e)
        {
            lblTournamentName.Text = FrmTournaments.selectedTournament.Event;
            lblTournamentLocation.Text = FrmTournaments.selectedTournament.Location;
            lblTournamentDate.Text = FrmTournaments.selectedTournament.Date.ToString("MMMM dd, yyyy");
        }
    }
}
