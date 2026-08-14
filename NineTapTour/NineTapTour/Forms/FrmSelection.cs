using NineTapTour.Database;
using NineTapTour.Core.Entities;
using NineTapTour.Core.Models;
using NineTapTour.Core.Repositories;
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
        private readonly ITournamentRepository tournamentRepository;

        public FrmSelection(ITournamentRepository tournamentRepository)
        {
            this.tournamentRepository = tournamentRepository;

            InitializeComponent();
            PopulateTournamentCbo();
        }

        public void PopulateTournamentCbo()
        {
            List<Tournament> allTournaments = tournamentRepository.GetTournamentList();
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
