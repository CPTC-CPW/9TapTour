using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NineTapTour.Forms
{
    public partial class UpdatefrmActiveMem : Form
    {
        public UpdatefrmActiveMem()
        {
            InitializeComponent();
            dateTimePicker1.Value = DateTime.Today.AddDays(-180);
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void btnUpdateActive_Click(object sender, EventArgs e)
        {

        }

        private void btnCheckInactive_Click(object sender, EventArgs e)
        {

        }
    }
}
