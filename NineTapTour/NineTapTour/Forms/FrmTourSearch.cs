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
using System.Linq.Dynamic;
using System.Text;

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
            listSearch.DataSource = null;

            List<Tournament> tourList = new List<Tournament>();
            StringBuilder whereClause = new StringBuilder();

            using (NineTapDb db = new NineTapDb())
            {
                var query = from t in db.Tournaments
                                //orderby t.Date descending
                            select t;

                // Location?
                if (!String.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    query = query.Where(t => t.Location == txtSearch.Text);
                }
                // Event?
                if (!String.IsNullOrWhiteSpace(txtEvent.Text))
                {
                    query = query.Where(t => t.Event == txtEvent.Text);
                }
                // Date?
                if (!String.IsNullOrWhiteSpace(txtDate.Text))
                {
                    try {
                        DateTime date = Convert.ToDateTime(txtDate.Text);
                        query = query.Where(t => t.Date == date);
                    } finally
                    {

                    }
                }
                query = query.OrderBy(t => t.Date);

                var results = query.Select(t => new
                {
                    Location = t.Location,
                    Event = t.Event,
                    Date = t.Date,
                    Id = t.Id,
                    Sponsors = t.Sponsors,
                    Participant = t.Participant,
                    Notes = t.Notes
                }).ToList();

                tourList = results.Select(x=> new Tournament
                {
                    Location = x.Location,
                    Event = x.Event,
                    Date = x.Date,
                    Id = x.Id,
                    Sponsors = x.Sponsors,
                    Participant = x.Participant,
                    Notes = x.Notes
                }).ToList();

                foreach (Tournament t in tourList)
                {
                    Console.WriteLine(t.Event);
                }
            }

            if (tourList.Count > 0)
            {
                listSearch.DataSource = tourList;
                listSearch.DisplayMember = "TourneyNameDate";
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
                listSearch.TabStop = true;
            } else
            {
                btnAccept.Enabled = false;
                listSearch.TabStop = false;
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            listSearch.DataSource = null;
            txtSearch.Text = null;
            txtEvent.Text = null;
            txtDate.Text = null;
            btnClear.Enabled = false;
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            decideCanClear();
        }

        private void decideCanClear()
        {
            if (String.IsNullOrWhiteSpace(txtSearch.Text.Trim()) && String.IsNullOrWhiteSpace(txtEvent.Text.Trim()) && String.IsNullOrWhiteSpace(txtDate.Text.Trim()))
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

        private void txtEvent_TextChanged(object sender, EventArgs e)
        {
            decideCanClear();
        }

        private void txtDate_TextChanged(object sender, EventArgs e)
        {
            decideCanClear();
        }
    }
}
