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
    public partial class FrmTourSearch : Form
    {
        List<Tournament> tours;
        /// <summary>
        /// This takes a tour list and modifies it. The tour you pass in will
        /// be different when this form has finished running.
        /// However it will not be modified before the accept button is clicked,
        /// so the X can be pressed safely at any time.
        /// </summary>
        /// <param name="tours"></param>
        public FrmTourSearch(List<Tournament> tours)
        {
            InitializeComponent();
            this.tours = tours;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(txtSearch.Text.Trim()))
            {
                List<Tournament> tourList = TournamentDb.GetTournamentList(txtSearch.Text.Trim());
                listSearch.DataSource = tourList;
                listSearch.DisplayMember = "TourneyNameDate";

            } else
            {
                listSearch.DataSource = null;
                decideCanClear();
            }
        }

        private void btnAccept_Click(object sender, EventArgs e)
        {
            foreach (Tournament tour in listSearch.SelectedItems)
            {
                tours.Add(tour);
                this.Close();
            }
        }

        private void listSearch_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listSearch.SelectedIndex != -1)
            {
                btnAccept.Enabled = true;
            } else
            {
                btnAccept.Enabled = false;
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            listSearch.DataSource = null;
            txtSearch.Text = null;
            btnClear.Enabled = false;
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            decideCanClear();
        }

        private void decideCanClear()
        {
            if (txtSearch.Text.Trim() == "")
            {
                btnSearch.Enabled = false;
                if (listSearch.SelectedIndex == -1)
                {
                    btnClear.Enabled = false;
                }
            }
            else
            {
                btnSearch.Enabled = true;
                btnClear.Enabled = true;
            }
        }
    }
}
