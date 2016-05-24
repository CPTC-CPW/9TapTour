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

namespace NineTapTour.Forms
{
    public partial class FrmTourSearch : Form
    {
        List<Tournament> tours;
        Tournament singleTour;
        bool single;
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
            single = false;
        }

        public FrmTourSearch(Tournament tour)
        {
            InitializeComponent();
            singleTour = tour;
            single = true;
            listSearch.SelectionMode = SelectionMode.One;
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
                    string temp = txtSearch.Text.ToLower().Trim();
                    query = query.Where(t => t.Location.ToLower().Contains(temp));
                }
                // Event?
                if (!String.IsNullOrWhiteSpace(txtEvent.Text))
                {
                    string temp = txtEvent.Text.ToLower().Trim();
                    query = query.Where(t => t.Event.ToLower().Contains(temp));
                }
                // Date?
                if (chkDate.Checked)
                {
                    if (dtpFrom.Value == dtpTo.Value)
                    {
                        query = query.Where(t => t.Date == dtpFrom.Value);
                    } else
                    {
                        query = query.Where(t => t.Date >= dtpFrom.Value && t.Date <= dtpTo.Value);
                    }
                }
                query = query.OrderBy(t => t.Date);

                Console.WriteLine(query.ToString());

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
            if (!single)
            {
                foreach (Tournament tour in listSearch.SelectedItems)
                {
                    tours.Add(tour);
                    this.Close();
                }
            } else
            {
                Console.WriteLine(listSearch.SelectedItem);
                //singleTour = (Tournament)listSearch.SelectedItem;
                //this.Close();
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
            chkDate.Checked = false;
            dtpTo.Value = DateTime.Now;
            dtpFrom.Value = dtpTo.Value;
            btnClear.Enabled = false;
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            decideCanClear();
        }

        private void decideCanClear()
        {
            if (String.IsNullOrWhiteSpace(txtSearch.Text.Trim()) && String.IsNullOrWhiteSpace(txtEvent.Text.Trim()) && !chkDate.Checked)
            {
                btnSearch.Enabled = false;
                if (listSearch.SelectedIndex == -1 && dtpTo.Value == dtpFrom.Value)
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

        private void chkDate_CheckedChanged(object sender, EventArgs e)
        {
            if (chkDate.Checked)
            {
                dtpFrom.Enabled = true;
                dtpTo.Enabled = true;
            } else
            {
                dtpFrom.Enabled = false;
                dtpTo.Enabled = false;
            }
            decideCanClear();
        }

        private void dtpTo_ValueChanged(object sender, EventArgs e)
        {
            if (dtpTo.Value < dtpFrom.Value)
            {
                dtpTo.Value = dtpFrom.Value;
            }
        }

        private void dtpFrom_ValueChanged(object sender, EventArgs e)
        {
            if (dtpFrom.Value > dtpTo.Value)
            {
                dtpFrom.Value = dtpTo.Value;
            }
        }
    }
}
