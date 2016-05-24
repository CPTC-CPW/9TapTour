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

        #region Fields
        /// <summary>
        /// fields for the FrmTournament class
        /// </summary>
        public int tournamentID;
        public List<Tournament> listTournaments;
        public static Tournament selectedTournament;
        #endregion

        public FrmTournaments()
        {
            InitializeComponent();
        }

        private void FrmTournamentStats_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the '_NineTapTour_NineTapDbDataSet.Tournaments' table. You can move, or remove it, as needed.
            //this.tournamentsTableAdapter.Fill(this._NineTapTour_NineTapDbDataSet.Tournaments);
            foreach (var d in Years())
            {
                cbxYearsForTournamentSearch.Items.Add(d);
            }
        }
        private List<int> Years()
        {
            int currentYear = DateTime.Now.Year;
            List<int> years = new List<int>();
            for (int i = 10; i > 0; i--)
            {
                years.Add(currentYear--);                
            }
            return years;
        }

        /// <summary>
        /// Populates the Tournaments in the Tournaments form
        /// </summary>
        public void PopulateTournaments(int yearSelected)
        {
            NineTapDb db = new NineTapDb();            
            
            var tournaments = (from t in db.Tournaments
                               orderby t.Date descending
                               where t.Date.Year == yearSelected 
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

            listTournaments = TournamentDb.GetTournamentList();                        
        }

        private void dgvAllTournaments_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                btnTournamentInfo.Enabled = true;
                // checks to see if any of the tournamnets were selected, and assigns the field 'tournamentID' with that value.
                if (dgvAllTournaments.SelectedCells.Count > 0)
                {
                    tournamentID = Convert.ToInt32(dgvAllTournaments.SelectedRows[0].Index);
                }
            }
            catch (ArgumentOutOfRangeException)
            {

            }                        
        }

        /// <summary>
        /// Gets the specific Tournament that is selected on the tournaments form
        /// </summary>
        /// <param name="id">tournament id for the row selected</param>
        /// <returns>Tournament Object that was selected</returns>
        public Tournament GetSpecificTournament(int id)
        {
            return listTournaments[id];
        }

        private void btnTournamentInfo_Click(object sender, EventArgs e)
        {
            // gets the specific Tournament object for the tournament that was selected using the GetSpecificTournament method
            selectedTournament = GetSpecificTournament(tournamentID);  
            // Instantiates a new Tournament Stats form          
            FrmTournamentStats tourneyStats = new FrmTournamentStats();

            tourneyStats.ShowDialog();

        }

        private void btnYearSelectedForTourneys_Click(object sender, EventArgs e)
        {
            PopulateTournaments(Convert.ToInt32(cbxYearsForTournamentSearch.Text));
        }
    }
}
