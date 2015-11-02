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

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            Tournament New = new Tournament();
            New.Date = dtpDate.Value.Date;
            New.Location = txtLocation.Text;
            New.Event = txtEvent.Text;
            New.Sponsors = txtSponsors.Text;
            New.Notes = rtxtNotes.Text;
            try
            {
                TournamentDb.AddTournament(New);
                MessageBox.Show(@"Tournament Created Successfully.");
                ((FrmMain)MdiParent)._tournamentList = TournamentDb.GetTournamentList();
                 
            }
            catch (TournamentTableException ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        
        private void frmNewTournament_Load(object sender, EventArgs e)
        {

        }
    }
}
