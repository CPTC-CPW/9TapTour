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
    public partial class FrmSearch : Form
    {
        /// <summary>
        /// Opens the "Search" form.
        /// </summary>
        public FrmSearch()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Closes the "Search" form without doing anything.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
