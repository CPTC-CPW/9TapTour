using NineTapTour.Database;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.ComponentModel.DataAnnotations;
using static NineTapTour._NineTapTour_NineTapDbDataSet;
using System.Data.SqlClient;
using System.Configuration;
using System.Data.Entity;
using NineTapTour.Models;

/// <summary>
/// Author Julie Edwards
/// </summary>
namespace NineTapTour.Forms
{
    public partial class FrmUpdateActiveMem : Form
    {
        int RegionID;
        DateTime targetDate;
        List<Member> InActiveList; 
        List<Member> AllMembers;

        #region FrmUpdateActiveMem
        public FrmUpdateActiveMem(int RID)
        {
            InitializeComponent();
            RegionID = RID;
            dateTimePicker1.Value = DateTime.Today.AddDays(-180);
            targetDate = dateTimePicker1.Value;
            InActiveList = MemberDB.GetMemberList(RegionID);
            AllMembers = MemberDB.GetMemberList(RegionID);
            PopulateInactiveList();
        }
        #endregion

        #region DateTime
        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            targetDate = dateTimePicker1.Value;
        }
        #endregion

        #region Button
        private void btnCheckInactive_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < InactiveListCheckBox.Items.Count; i++)
            {
                InactiveListCheckBox.SetItemChecked(i, true);
            }
        }

        private void btnUpdateActive_Click(object sender, EventArgs e)
        {
            if (InactiveListCheckBox.CheckedItems.Count == 0)
            {
                MessageBox.Show("No members checked.");
                return;
            }

            var db = new NineTapDb();
            if (MessageBox.Show("Update the selected Members to inactive?", "", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {

                foreach (Member mem in InactiveListCheckBox.CheckedItems)
                {
                    mem.IsActive = false;
                    db.Entry(mem).State = EntityState.Modified;

                }
                db.SaveChanges();
                InactiveListCheckBox.Items.Clear();
                PopulateInactiveList();
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Populates the InactiveListCheckBox with all Inactive Members
        /// </summary>
        private void PopulateInactiveList()
        {
            if (AllMembers == null)
            {
                MessageBox.Show("There are no members within this region in the database");
                return;
            }
            AllMembers.Sort(new MemberNumComparer());

            InactiveListCheckBox.Sorted = false;

            foreach (var mem in AllMembers)
            {
                // add members to the list
                if (mem.IsActive && (mem.LastBowled <= targetDate || mem.LastBowled.ToString() == ""))
                {
                    InactiveListCheckBox.Items.Add(mem);
                }
            }
        }
        #endregion
    }

    /* this class needs to be moved into its own file -TS */
    public class MemberNumComparer : IComparer<Member>
    {
        int IComparer<Member>.Compare(Member x, Member y)
        {
            int mem1 = x.Number;
            int mem2 = y.Number;
            return mem1.CompareTo(mem2);
        }
    }
}
