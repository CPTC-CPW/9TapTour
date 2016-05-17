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

namespace NineTapTour
{
    public partial class frmListTournamentsByYear : Form
    {
        public static int selectedYear;
        public frmListTournamentsByYear()
        {
            InitializeComponent();
        }

        private void frmListTournamentsByYear_Load(object sender, EventArgs e)
        {
            btnSearch.Enabled = false;
            cbxYear.Items.Clear();
            foreach (var y in Years())
            {
                cbxYear.Items.Add(y);
            }
        }
        private List<int> Years()
        {
            int currentYear = DateTime.Now.Year;
            List<int> years = new List<int>();
            for (int i = 25; i > 0; i--)
            {
                years.Add(currentYear--);
            }
            return years;
        }

        private void cbxYear_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedYear = 0;
            btnSearch.Enabled = true;
            selectedYear = (int)cbxYear.SelectedValue;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            NineTapDb db = new NineTapDb();
            var tournaments = (from t in db.Tournaments
                               orderby t.Date descending
                               where t.Date.Year == selectedYear
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
        }
    }
}
