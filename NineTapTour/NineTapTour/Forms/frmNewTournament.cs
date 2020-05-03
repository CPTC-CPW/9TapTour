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
            Tournament NewTournament = new Tournament();
            NewTournament.Date = dtpDate.Value.Date;
            NewTournament.Location = txtLocation.Text;
            NewTournament.Event = txtEvent.Text;
            NewTournament.Sponsors = txtSponsors.Text;
            NewTournament.Notes = rtxtNotes.Text;
            bool errors = false;
            NewTournament.TourneyRegion = ((FrmMain)MdiParent).RegionID;
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
                    NewTournament.Squads = Convert.ToInt32(txtSquads.Text);
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
                NewTournament.Doubles = true;
            }

            if (rdo3OutOf4.Checked)
            {
                NewTournament.ThreeOutOf4 = true;
            }

            try
            {
                // If tourID isn't null it means they chose a tour to edit
                if (tourToEdit == null)
                {
                    if (!errors)
                    {
                        TournamentDB.AddTournament(NewTournament);
                        MessageBox.Show(@"Tournament Created Successfully.");
                        ((FrmMain)MdiParent)._tournamentList = TournamentDB.GetTournamentList(NewTournament.TourneyRegion);
                    }
                }
                else
                {
                    DialogResult dr = MessageBox.Show("Are you sure you want to modify this tournament?", "Confirm Edit", MessageBoxButtons.YesNo);

                    if (dr == DialogResult.Yes)
                    {
                        NewTournament.Id = tourToEdit.Id;
                        NewTournament = TournamentDB.GetTourneyByID(tourToEdit.Id);
                        // Editing Tournament with form data
                        NewTournament.Date = dtpDate.Value.Date;
                        NewTournament.Location = txtLocation.Text;
                        NewTournament.Event = txtEvent.Text;
                        NewTournament.Sponsors = txtSponsors.Text;
                        NewTournament.Notes = rtxtNotes.Text;

                        if (TournamentDB.UpdateTournament(NewTournament))
                        {
                            MessageBox.Show(@"Tournament modified.");
                            ((FrmMain)MdiParent)._tournamentList = TournamentDB.GetTournamentList(NewTournament.TourneyRegion);
                        }
                        else
                        {
                            MessageBox.Show("The database failed to update.");
                        }
                    }
                }
            }
            catch (TournamentTableException ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (!errors)
                {
                    Tournament currTourney = NewTournament;
                    clearTournamentForm();

                    var newFrmMemberScores = Application.OpenForms["FrmMemberScores"] as frmMemberScores;
                    ((FrmMain)MdiParent).OpenOrDisplayForm(ref newFrmMemberScores);

                    //populates selected tournament with recently edited or created tournament back in MemberScores.
                    newFrmMemberScores.populateSelectedTournament(currTourney);
                }
            }
        }

        private void btnEditTour_Click(object sender, EventArgs e)
        {
            FrmTourSearch getEdit = new FrmTourSearch(((FrmMain)MdiParent).RegionID);
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
#if DEBUG
            else
            {
                Console.WriteLine("Search returned a null. probably was closed.");
            }
#endif
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

            // False for now while feature is being implemented.
            rdoDoubles.Enabled = false;
        }
    }
}
