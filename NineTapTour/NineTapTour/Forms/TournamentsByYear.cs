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
    public partial class TournamentsByYear : Form
    {
        public int RegionID;

        #region TorunamentByYear
        public TournamentsByYear(int RegionID)
        {
            InitializeComponent();
            this.RegionID = RegionID;
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
        #endregion

        #region Methods
        /// <summary>
        /// Gets the last 25 years from the current year down
        /// </summary>
        /// <returns>List of Years from current to 25 years ago</returns>
        /// added +1 to year so you could search any precreated tournaments that were created after the current year.
        private List<int> Years()
        {
            int currentYear = DateTime.Now.Year + 1;
            List<int> years = new List<int>();
            for (int i = 25; i > 0; i--)
            {
                years.Add(currentYear--);
            }
            return years;
        }
        #endregion

        #region CheckBoxs
        private void cbxYear_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnSearch.Enabled = true;
        }
        #endregion

        #region Buttons
        private void btnSearch_Click(object sender, EventArgs e)
        {
            dgvAllTournaments.DataSource = 
                TournamentDB.GetTournamentsByYear(Convert.ToInt32(cbxYear.Text), RegionID);
        }
        #endregion
    }
}
