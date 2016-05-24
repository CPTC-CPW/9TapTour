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
        // If a new tournament was selected to edit, this will be set to something other than null.
        Tournament tourToEdit = null;

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
            if (ckbxDoubles.Checked)
            {
                NewTournament.Doubles = true;
            }
            else
            {
                NewTournament.Doubles = false;
            }

            //// validation prototype of the only non-nullable text box on the form
            if (String.IsNullOrEmpty(txtEvent.Text.Trim()) == true)
                MessageBox.Show("Event cannot be blank");
            else {
                try
                {
                    // If tourID isn't null it means they chose a tour to edit
                    if (tourToEdit == null)
                    {
                        TournamentDb.AddTournament(NewTournament);
                        MessageBox.Show(@"Tournament Created Successfully.");
                        ((FrmMain)MdiParent)._tournamentList = TournamentDb.GetTournamentList();
                    } else
                    {
                        DialogResult dr = MessageBox.Show("Confirm Edit", "Are you sure you want to modify this tournament?", MessageBoxButtons.YesNo);

                        if (dr == DialogResult.Yes)
                        {
                            TournamentDb.UpdateTournament(NewTournament, tourID);
                        }
                    }
                    this.Close();
                }
                catch (TournamentTableException ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void btnEditTour_Click(object sender, EventArgs e)
        {
            new FrmTourSearch(tourToEdit).ShowDialog();
            if (tourToEdit != null)
            {
                dtpDate.Value = tourToEdit.Date;
                txtLocation.Text = tourToEdit.Location;
                txtEvent.Text = tourToEdit.Event;
                txtSponsors.Text = tourToEdit.Sponsors;
                ckbxDoubles.Checked = tourToEdit.Doubles ? true : false;
                rtxtNotes.Text = tourToEdit.Notes;
                btnSubmit.Text = "Update Tournament";
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            btnSubmit.Text = "Create Tournament";
            btnSubmit.Enabled = false;
            dtpDate.Value = DateTime.Now;
            txtLocation.Clear();
            txtEvent.Clear();
            txtSponsors.Clear();
            ckbxDoubles.Checked = false;
            rtxtNotes.Clear();
        }
    }
}
