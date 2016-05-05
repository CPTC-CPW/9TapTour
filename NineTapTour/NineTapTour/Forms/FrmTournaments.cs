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
    public partial class FrmTournaments : Form
    {
        public FrmTournaments()
        {
            InitializeComponent();
        }

        private void FrmTournamentStats_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the '_NineTapTour_NineTapDbDataSet.Tournaments' table. You can move, or remove it, as needed.
            this.tournamentsTableAdapter.Fill(this._NineTapTour_NineTapDbDataSet.Tournaments);
            PopulateTournaments();

        }

        public void PopulateTournaments()
        {
            NineTapDb db = new NineTapDb();
            var tournaments = (from t in db.Tournaments
                               orderby t.Date descending
                               select new
                               {
                                   t.Id,
                                   t.Date,
                                   t.Location,
                                   t.Event,
                                   t.Notes,
                                   t.Sponsors
                           }).ToList();
            dgvAllTournaments.DataSource = tournaments;
        }

        private void dgvAllTournaments_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            btnTournamentInfo.Enabled = true;
        }
    }
}
