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
using NineTapTour.Exceptions;

namespace NineTapTour.Forms
{
    public partial class frmNewTournament : Form
    {
        public frmNewTournament()
        {
            InitializeComponent();
        }

        /// <summary>
        /// close the tournament form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// create a new tournament
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSubmit_Click(object sender, EventArgs e)
        {
            Tournament NewTournament = new Tournament();
            NewTournament.Date = dtpDate.Value.Date;
            NewTournament.Location = txtLocation.Text;
            NewTournament.Event = txtEvent.Text;
            NewTournament.Sponsors = txtSponsors.Text;
            NewTournament.Notes = rtxtNotes.Text;

            try
            {
                TournamentDb.AddTournament(NewTournament);
                MessageBox.Show(@"Tournament Created Successfully.");
                ((FrmMain)MdiParent)._tournamentList = TournamentDb.GetTournamentList();
                this.Close();

            }
            catch (TournamentTableException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
