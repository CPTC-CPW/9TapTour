using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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

        }
        private List<int> Years()
        {
            List<int> years = new List<int>();
            for (int i = 10; i > 0; i--)
            {

            }
            return years;
        } 
    }
}
