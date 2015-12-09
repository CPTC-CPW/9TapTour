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
        /// Closes the tournament form.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
   
        /// <summary>
        /// Creates a new tournament.
        /// Saves the date, location, event, sponsors, and extra notes.
        /// If all the information fits the criteria then the tournament is saved.
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

            //// validation prototype of the only non-nullable text box on the form
            if (String.IsNullOrEmpty(txtEvent.Text) == true)
                MessageBox.Show("Event cannot be blank");

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
