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
    public partial class FrmTournamentsByYear : Form
    {
        public int RID;

        public FrmTournamentsByYear(int RegionID)
        {
            InitializeComponent();
            this.RID = RegionID;
        }

        private void TournamentsByYear_Load(object sender, EventArgs e)
        {
            btnSearch.Enabled = false;
            cbxYear.Items.Clear();
            foreach (var y in Years())
            {
                cbxYear.Items.Add(y);
            }
        }

        /// <summary>
        /// Gets the last 25 years from the current year down
        /// </summary>
        /// <returns>List of Years from current to 25 years ago</returns>
        /// added +1 to year so you could search any precreated tournaments that were created after the current year.
        private List<int> Years()
        {
            int currentYear = DateTime.Now.Year + 1;
            List<int> years = [];
            for (int i = 25; i > 0; i--)
            {
                years.Add(currentYear--);
            }
            return years;
        }

        private void cbxYear_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnSearch.Enabled = true;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            PopulateTournamentsByYear(Convert.ToInt32(cbxYear.Text), RID);
        }

        /// <summary>
        /// Gets all the tournaments from a specific year (Phase 6: uses TourneyRegion FK)
        /// </summary>
        /// <param name="selectedYear">Year selected</param>
        public void PopulateTournamentsByYear(int selectedYear, int regionID)
        {
            NineTapDb db = new();
            // Phase 6: Use Tournament.TourneyRegion.NineTapRegionID for proper FK relationship
            var tournaments = (from t in db.Tournaments
                               orderby t.Date descending
                               where t.Date.Year == selectedYear && t.TourneyRegion.NineTapRegionID == regionID
                               select new
                               {
                                   t.Id,
                                   t.Date,
                                   t.Location,
                                   t.Event,
                                   t.Doubles,
                                   t.ThreeOutOf4,
                                   t.Notes,
                                   t.Sponsors
                               }).ToList();
            dgvAllTournaments.DataSource = tournaments;
        }
    }
}
