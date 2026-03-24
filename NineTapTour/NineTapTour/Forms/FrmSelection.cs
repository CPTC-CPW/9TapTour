using NineTapTour.Database;
using NineTapTour.Models;
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
    public partial class FrmSelection : Form
    {
        public Tournament selectedTournament;
        public FrmSelection()
        {
            InitializeComponent();
            PopulateTournamentCbo();
        }

        public void PopulateTournamentCbo()
        {
            List<Tournament> allTournaments = TournamentDB.GetTournamentList();
            cbxTournaments.DataSource = allTournaments;
            cbxTournaments.DisplayMember = nameof(Tournament.TourneyNameDate);
        }

        private void btnSelectTournament_Click(object sender, EventArgs e)
        {
            selectedTournament = (Tournament)cbxTournaments.SelectedItem;
            DialogResult = DialogResult.OK;
        }
    }
}
