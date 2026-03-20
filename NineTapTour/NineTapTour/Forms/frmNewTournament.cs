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
using NineTapTour.Models;

namespace NineTapTour.Forms
{
    public partial class FrmNewTournament : Form
    {
        // If a new tournament was selected to edit, this will be set to something other than null.
        Tournament tourToEdit;

        public FrmNewTournament()
        {
            InitializeComponent();
            txtSquads.Text = 4.ToString();
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
            Tournament newTournament = new();
            newTournament.Date = dtpDate.Value.Date;
            newTournament.Location = txtLocation.Text;
            newTournament.Event = txtEvent.Text;
            newTournament.Sponsors = txtSponsors.Text;
            newTournament.Notes = rtxtNotes.Text;
            bool errors = false;
            
            int numSquads;
            bool validateSquads = int.TryParse(txtSquads.Text, out numSquads);

            if (validateSquads)
            {
                if (numSquads < 1 || numSquads > 9)
                {
                    MessageBox.Show("Squads must be between 1 - 8");
                    errors = true;
                }
                else
                {
                    newTournament.Squads = Convert.ToInt32(txtSquads.Text);
                    errors = false;
                }
            }
            else
            {
                MessageBox.Show("Squads must be a number between 1 - 8");
                errors = true;
            }

            if (rdoDoubles.Checked)
            {
                newTournament.Doubles = true;
            }
            else if (rdo3OutOf4.Checked)
            {
                newTournament.ThreeOutOf4 = true;
            }
            else if (rdoThreeGame.Checked)
            {
                newTournament.ThreeOutOf4 = true;
                newTournament.IsOnlyThreeGames = true;
            }


            try
            {
                // If tourID isn't null it means they chose a tour to edit
                if (tourToEdit == null)
                {
                    if (!errors)
                    {
                        TournamentDB.AddTournament(newTournament);
                        MessageBox.Show(@"Tournament Created Successfully.");
                        ((FrmMain)MdiParent).TournamentList = TournamentDB.GetTournamentList();
                    }
                }
                else
                {
                    DialogResult dr = MessageBox.Show("Are you sure you want to modify this tournament?", "Confirm Edit", MessageBoxButtons.YesNo);

                    if (dr == DialogResult.Yes)
                    {
                        newTournament.Id = tourToEdit.Id;
                        newTournament = TournamentDB.GetTourneyByID(tourToEdit.Id);
                        // Editing Tournament with form data
                        newTournament.Date = dtpDate.Value.Date;
                        newTournament.Location = txtLocation.Text;
                        newTournament.Event = txtEvent.Text;
                        newTournament.Sponsors = txtSponsors.Text;
                        newTournament.Notes = rtxtNotes.Text;

                        if (TournamentDB.UpdateTournament(newTournament))
                        {
                            MessageBox.Show(@"Tournament modified.");
                            ((FrmMain)MdiParent).TournamentList = TournamentDB.GetTournamentList();
                        }
                        else
                        {
                            MessageBox.Show("The database failed to update.");
                        }
                    }
                }
            }
            finally
            {
                if (!errors)
                {
                    Tournament currTourney = newTournament;
                    clearTournamentForm();

                    var newFrmMemberScores = Application.OpenForms["FrmMemberScores"] as FrmMemberScores;
                    ((FrmMain)MdiParent).OpenOrDisplayForm(ref newFrmMemberScores);

                    //populates selected tournament with recently edited or created tournament back in MemberScores.
                    newFrmMemberScores.PopulateSelectedTournament(currTourney);
                }
            }
        }

        private void btnEditTour_Click(object sender, EventArgs e)
        {
            FrmTourSearch getEdit = new(((FrmMain)MdiParent).RegionID);
            getEdit.ShowDialog();
            tourToEdit = getEdit.getResult();

            if (tourToEdit != null)
            {
                dtpDate.Value = tourToEdit.Date;
                txtLocation.Text = tourToEdit.Location;
                txtEvent.Text = tourToEdit.Event;
                txtSponsors.Text = tourToEdit.Sponsors;
                rdoDoubles.Checked = tourToEdit.Doubles ? true : false;
                rdo3OutOf4.Checked = tourToEdit.ThreeOutOf4 ? true : false;
                rtxtNotes.Text = tourToEdit.Notes;
                btnSubmit.Text = "Update Tournament";
                lblEdit.Text = "Currently Editing " + tourToEdit.TourneyNameDate;
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            clearTournamentForm();
        }

        /// <summary>
        /// Clears the NewTournament Form
        /// </summary>
        private void clearTournamentForm()
        {
            btnSubmit.Text = "Create Tournament";
            btnSubmit.Enabled = false;
            dtpDate.Value = DateTime.Now;
            txtLocation.Clear();
            txtEvent.Clear();
            txtSponsors.Clear();
            rdoDoubles.Checked = false;
            rdo3OutOf4.Checked = false;
            //Disables the double tournament Radio Button
            //To enable Double tournament set rdoDoubles.Enabled to true
            rdoDoubles.Enabled = false;
            rdo3OutOf4.Enabled = true;
            rtxtNotes.Clear();
            tourToEdit = null;
            lblEdit.Text = "";
        }

        private void txtLocation_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtLocation.Text.Trim()))
            {
                btnSubmit.Enabled = true;
            }
            else
            {
                btnSubmit.Enabled = false;
            }
            checkCleared();
        }

        private void checkCleared()
        {
            if (
                !string.IsNullOrWhiteSpace(txtLocation.Text.Trim()) ||
                !string.IsNullOrWhiteSpace(txtEvent.Text.Trim()) ||
                !string.IsNullOrWhiteSpace(txtSponsors.Text.Trim()) ||
                rdoDoubles.Checked ||
                rdo3OutOf4.Checked ||
                !string.IsNullOrWhiteSpace(rtxtNotes.Text.Trim()) ||
                tourToEdit != null
                )
            {
                btnClear.Enabled = true;
            }
            else
            {
                btnClear.Enabled = false;
            }
        }

        private void txtEvent_TextChanged(object sender, EventArgs e)
        {
            checkCleared();
        }

        private void txtSponsors_TextChanged(object sender, EventArgs e)
        {
            checkCleared();
        }

        private void rdoDoubles_CheckedChanged(object sender, EventArgs e)
        {
            checkCleared();
            if (rdo3OutOf4.Enabled)
            {
                rdo3OutOf4.Enabled = false;
            }
            else
            {
                rdo3OutOf4.Enabled = true;
            }
        }

        private void rtxtNotes_TextChanged(object sender, EventArgs e)
        {
            checkCleared();
        }

        private void rdo3OutOf4_CheckedChanged(object sender, EventArgs e)
        {
            checkCleared();
            if (rdoDoubles.Enabled)
            {
                rdoDoubles.Enabled = false;
            }
            else
            {
                rdoDoubles.Enabled = true;
            }
        }

        private void FrmNewTournament_Load(object sender, EventArgs e)
        {
            rdoSingles.Checked = true;
        }
    }
}
