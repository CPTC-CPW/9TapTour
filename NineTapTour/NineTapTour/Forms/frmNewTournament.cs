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
        Tournament tourToEdit;

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
            if (ckbx3outOf4.Checked)
            {
                NewTournament.ThreeOutOf4 = true;
            }
            //else
            //{
            //    NewTournament.Doubles = false;
            //}

            try
            {
                // If tourID isn't null it means they chose a tour to edit
                if (tourToEdit == null)
                {
                    TournamentDb.AddTournament(NewTournament);
                    MessageBox.Show(@"Tournament Created Successfully.");
                    ((FrmMain)MdiParent)._tournamentList = TournamentDb.GetTournamentList();
                }
                else
                {
                    DialogResult dr = MessageBox.Show("Are you sure you want to modify this tournament?", "Confirm Edit", MessageBoxButtons.YesNo);

                    if (dr == DialogResult.Yes)
                    {
                        NewTournament.Id = tourToEdit.Id;
                        if (TournamentDb.UpdateTournament(NewTournament))
                        {
                            MessageBox.Show(@"Tournament modified.");
                            ((FrmMain)MdiParent)._tournamentList = TournamentDb.GetTournamentList();
                        } else
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
                Tournament currTourney = NewTournament;
                clearTournamentForm();

                var newFrmMemberScores = Application.OpenForms["FrmMemberScores"] as frmMemberScores;
                ((FrmMain)MdiParent).OpenOrDisplayForm(ref newFrmMemberScores);

                //populates selected tournament with recently edited or created tournament back in MemberScores.
                newFrmMemberScores.populateSelectedTournament(currTourney);
            }

        }

        private void btnEditTour_Click(object sender, EventArgs e)
        {
            FrmTourSearch getEdit = new FrmTourSearch();
            getEdit.ShowDialog();
            tourToEdit = getEdit.getResult();

            if (tourToEdit != null)
            {
                dtpDate.Value = tourToEdit.Date;
                txtLocation.Text = tourToEdit.Location;
                txtEvent.Text = tourToEdit.Event;
                txtSponsors.Text = tourToEdit.Sponsors;
                ckbxDoubles.Checked = tourToEdit.Doubles ? true : false;
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
            ckbxDoubles.Checked = false;
            ckbx3outOf4.Checked = false;
            ckbxDoubles.Enabled = true;
            ckbx3outOf4.Enabled = true;
            rtxtNotes.Clear();
            tourToEdit = null;
            lblEdit.Text = "";
        }

        private void txtLocation_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtLocation.Text.Trim())) {
                btnSubmit.Enabled = true;
            } else
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
                ckbxDoubles.Checked ||
                ckbx3outOf4.Checked ||
                !string.IsNullOrWhiteSpace(rtxtNotes.Text.Trim()) ||
                tourToEdit != null
                )
            {
                btnClear.Enabled = true;
            } else
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

        private void ckbxDoubles_CheckedChanged(object sender, EventArgs e)
        {
            checkCleared();
            if (ckbx3outOf4.Enabled)
            {
                ckbx3outOf4.Enabled = false;
            }
            else
            {
                ckbx3outOf4.Enabled = true;
            }
        }

        private void rtxtNotes_TextChanged(object sender, EventArgs e)
        {
            checkCleared();
        }

        private void ckbx3outOf4_CheckedChanged(object sender, EventArgs e)
        {
            checkCleared();
            if (ckbxDoubles.Enabled)
            {
                ckbxDoubles.Enabled = false;
            }
            else
            {
                ckbxDoubles.Enabled = true;
            }
            
        }
    }
}
