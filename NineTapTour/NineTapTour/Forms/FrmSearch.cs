using NineTapTour.Database;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
/*
    Author: Toby Fortuner
    Jun 5, 2016
    */
namespace NineTapTour.Forms
{
    public partial class FrmSearch : Form
    {
        int RegionID;
        public int searchResult { get; set; }
        /// <summary>
        /// Opens the "Search" form.
        /// </summary>
        public FrmSearch(int RegionID)
        {
            InitializeComponent();
            this.RegionID = RegionID;
        }

        private void FrmSearch_Load(object sender, EventArgs e)
        {
            
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            dtagrdResults.DataSource = null;

            List<Member> memList = new List<Member>();

            using (NineTapDb db = new NineTapDb())
            {
                var query = from m in db.Members
                                //orderby m.Id descending
                            where m.NineTapRegionID == RegionID

                            select m;

                // Member Number?
                if (!String.IsNullOrWhiteSpace(txtMemNumber.Text))
                {
                    int temp = 0;
                    Int32.TryParse(txtMemNumber.Text, out temp);
                    query = query.Where(m => m.Number.Equals(temp));
                }
                // First Name?
                if (!String.IsNullOrWhiteSpace(txtFirstName.Text))
                {
                    string temp = txtFirstName.Text.ToLower().Trim();
                    query = query.Where(m => m.FirstName.ToLower().Contains(temp));
                }
                // Last Name?
                if (!String.IsNullOrWhiteSpace(txtLastName.Text))
                {
                    string temp = txtLastName.Text.ToLower().Trim();
                    query = query.Where(m => m.LastName.ToLower().Contains(temp));
                }
                // Is Active?
                if (rdoActiveYes.Checked)
                {
                    query = query.Where(m => m.IsActive.Equals(true));
                }
                else if (rdoActiveNo.Checked)
                {
                    query = query.Where(m => m.IsActive.Equals(false));
                }
                // Average?
                if (!String.IsNullOrWhiteSpace(txtAverage.Text))
                {
                    int temp = 0;
                    Int32.TryParse(txtAverage.Text, out temp);
                    query = query.Where(m => m.Average == temp);
                }
                // Handicap?
                if (!String.IsNullOrWhiteSpace(txtHandicap.Text))
                {
                    int temp = 0;
                    Int32.TryParse(txtHandicap.Text, out temp);
                    query = query.Where(m => m.Handicap == temp);
                }
                // Bonus?
                if (!String.IsNullOrWhiteSpace(txtBonus.Text))
                {
                    int temp = 0;
                    Int32.TryParse(txtBonus.Text, out temp);
                    query = query.Where(m => m.Bonus == temp);
                }
                query = query.OrderBy(m => m.Number);

                Console.WriteLine(query.ToString());

                var results = query.Select(m => new
                {
                    Number = m.Number,
                    FirstName = m.FirstName,
                    LastName = m.LastName,
                    IsActive = m.IsActive,
                    MiddleInitial = m.MiddleInitial,
                    DateOfBirth = m.DateOfBirth,
                    Gender = m.Gender,
                    Street = m.Street,
                    City = m.City,
                    State = m.State,
                    PostalCode = m.PostalCode,
                    Email = m.Email,
                    PrimaryPhone = m.PrimaryPhone,
                    SecondaryPhone = m.SecondaryPhone,
                    StartAVG = m.StartAvg,       
                    Average = m.Average,
                    Handicap = m.Handicap,
                    Bonus = m.Bonus,
                    JoinDate = m.JoinDate,
                    RejoinDate = m.RejoinDate,
                    LastBowled = m.LastBowled,
                    LastPayment = m.LastPayment,
                    IsLifetimeMember = m.IsLifetimeMember,
                    MoneyEarned = m.MoneyEarned,
                    Notes = m.Notes,
                    Referrals = m.Referrals,
                    IsSenior = m.IsSenior,
                    
                    
                }).ToList();

                memList = results.Select(m => new Member
                {
                    Number = m.Number,
                    FirstName = m.FirstName,
                    LastName = m.LastName,
                    IsActive = m.IsActive,
                    MiddleInitial = m.MiddleInitial,
                    DateOfBirth = m.DateOfBirth,
                    Gender = m.Gender,
                    Street = m.Street,
                    City = m.City,
                    State = m.State,
                    PostalCode = m.PostalCode,
                    Email = m.Email,
                    PrimaryPhone = m.PrimaryPhone,
                    SecondaryPhone = m.SecondaryPhone,
                    Average = m.Average,
                    StartAvg = m.StartAVG,
                    Handicap = m.Handicap,
                    Bonus = m.Bonus,
                    JoinDate = m.JoinDate,
                    RejoinDate = m.RejoinDate,
                    LastBowled = m.LastBowled,
                    LastPayment = m.LastPayment,
                    IsLifetimeMember = m.IsLifetimeMember,
                    MoneyEarned = m.MoneyEarned,
                    Notes = m.Notes,
                    Referrals = m.Referrals,
                    IsSenior = m.IsSenior
                }).ToList();

                foreach (Member m in memList)
                {
                    Console.WriteLine(m.Id);
                }
            }

            if (memList.Count > 0)
            {
                dtagrdResults.DataSource = memList;

                AdvancedViewCheck();
            }
            else
            {
                EmptyGrid();
            }
            btnSelect.Focus();
        }

        private void chkAdvancedView_CheckStateChanged(object sender, EventArgs e)
        {
            //dtagrdResults.Columns["IsActive"].Visible = !dtagrdResults.Columns["IsActive"].Visible;
            dtagrdResults.Columns["MiddleInitial"].Visible = !dtagrdResults.Columns["MiddleInitial"].Visible;
            dtagrdResults.Columns["DateOfBirth"].Visible = !dtagrdResults.Columns["DateOfBirth"].Visible;
            dtagrdResults.Columns["Gender"].Visible = !dtagrdResults.Columns["Gender"].Visible;
            dtagrdResults.Columns["Street"].Visible = !dtagrdResults.Columns["Street"].Visible;
            dtagrdResults.Columns["City"].Visible = !dtagrdResults.Columns["City"].Visible;
            dtagrdResults.Columns["State"].Visible = !dtagrdResults.Columns["State"].Visible;
            dtagrdResults.Columns["PostalCode"].Visible = !dtagrdResults.Columns["PostalCode"].Visible;
            dtagrdResults.Columns["PrimaryPhone"].Visible = !dtagrdResults.Columns["PrimaryPhone"].Visible;
            dtagrdResults.Columns["SecondaryPhone"].Visible = !dtagrdResults.Columns["SecondaryPhone"].Visible;
            //dtagrdResults.Columns["Average"].Visible = !dtagrdResults.Columns["Average"].Visible;
            //dtagrdResults.Columns["Handicap"].Visible = !dtagrdResults.Columns["Handicap"].Visible;
            //dtagrdResults.Columns["Bonus"].Visible = !dtagrdResults.Columns["Bonus"].Visible;
            dtagrdResults.Columns["JoinDate"].Visible = !dtagrdResults.Columns["JoinDate"].Visible;
            dtagrdResults.Columns["RejoinDate"].Visible = !dtagrdResults.Columns["RejoinDate"].Visible;
            dtagrdResults.Columns["LastBowled"].Visible = !dtagrdResults.Columns["LastBowled"].Visible;
            dtagrdResults.Columns["LastPayment"].Visible = !dtagrdResults.Columns["LastPayment"].Visible;
            dtagrdResults.Columns["IsLifetimeMember"].Visible = !dtagrdResults.Columns["IsLifetimeMember"].Visible;
            //dtagrdResults.Columns["MoneyEarned"].Visible = !dtagrdResults.Columns["MoneyEarned"].Visible;
            dtagrdResults.Columns["Notes"].Visible = !dtagrdResults.Columns["Notes"].Visible;
            dtagrdResults.Columns["Referrals"].Visible = !dtagrdResults.Columns["Referrals"].Visible;
            dtagrdResults.Columns["IsSenior"].Visible = !dtagrdResults.Columns["IsSenior"].Visible;
        }

        private void FillGrid()
        {
            dtagrdResults.DataSource = null;
            List<Member> memList = MemberDb.GetMemberList(RegionID);
            dtagrdResults.DataSource = memList;
            AdvancedViewCheck();
        }

        private void EmptyGrid()
        {
            dtagrdResults.DataSource = null;

            //data bind an empty member list so the columns show up
            List<Member> memList = new List<Member>();
            dtagrdResults.DataSource = memList;
            AdvancedViewCheck();
        }

        private void AdvancedViewCheck()
        {
            //probably don't want SSN or ID to show up at all.
            dtagrdResults.Columns["Id"].Visible = false;
            dtagrdResults.Columns["SSN"].Visible = false;
            //checks if Advanced View is not checked
            if (!chkAdvancedView.Checked)
            {
                //dtagrdResults.Columns["IsActive"].Visible = false;
                dtagrdResults.Columns["MiddleInitial"].Visible = false;
                dtagrdResults.Columns["DateOfBirth"].Visible = false;
                dtagrdResults.Columns["Gender"].Visible = false;
                dtagrdResults.Columns["Street"].Visible = false;
                dtagrdResults.Columns["City"].Visible = false;
                dtagrdResults.Columns["State"].Visible = false;
                dtagrdResults.Columns["PostalCode"].Visible = false;
                dtagrdResults.Columns["PrimaryPhone"].Visible = false;
                dtagrdResults.Columns["SecondaryPhone"].Visible = false;
                //dtagrdResults.Columns["Average"].Visible = false;
                //dtagrdResults.Columns["Handicap"].Visible = false;
                //dtagrdResults.Columns["Bonus"].Visible = false;
                dtagrdResults.Columns["JoinDate"].Visible = false;
                dtagrdResults.Columns["RejoinDate"].Visible = false;
                dtagrdResults.Columns["LastBowled"].Visible = false;
                dtagrdResults.Columns["LastPayment"].Visible = false;
                dtagrdResults.Columns["IsLifetimeMember"].Visible = false;
                //dtagrdResults.Columns["MoneyEarned"].Visible = false;
                dtagrdResults.Columns["MoneyEarned"].ValueType = typeof(decimal);
                dtagrdResults.Columns["Notes"].Visible = false;
                dtagrdResults.Columns["Referrals"].Visible = false;
                dtagrdResults.Columns["IsSenior"].Visible = false;
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtMemNumber.Clear();
            txtFirstName.Clear();
            txtLastName.Clear();
            rdoActiveEither.Checked = true;
            txtAverage.Clear();
            txtHandicap.Clear();
            txtBonus.Clear();
            FillGrid();
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            if (dtagrdResults.Rows.Count > 0)
            {
                searchResult = (int)dtagrdResults.SelectedRows[0].Cells[1].Value;
                // Member Number
                //Console.WriteLine(dtagrdResults.SelectedRows[0].Cells[1].Value.ToString());
                // Member Name
                //Console.WriteLine(dtagrdResults.SelectedRows[0].Cells[3].Value.ToString());
                this.Close();
            }
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
