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
            btnSearch.Enabled = true;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            NineTapDb db = new NineTapDb();
        }
    }
}
