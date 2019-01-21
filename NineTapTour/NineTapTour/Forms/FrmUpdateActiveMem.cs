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
        public FrmUpdateActiveMem(int RID)
        {
            InitializeComponent();
            RegionID = RID;
            dateTimePicker1.Value = DateTime.Today.AddDays(-180);
            targetDate = dateTimePicker1.Value;
            InActiveList = MemberDb.GetMemberList(RegionID);
            AllMembers = MemberDb.GetMemberList(RegionID);
            UpdateList();
        }

        private void UpdateList()
        {
            if (AllMembers == null)
            {
                MessageBox.Show("There are no members within this region in the database");
                return;
            }

            AllMembers.ForEach(delegate (Member mem)
            {
                if (mem.IsActive && (mem.LastBowled <= targetDate || mem.LastBowled.ToString() == ""))
                {
                    InactiveListCheckBox.Items.Add(mem);
                }
            });

            // add a sort feature here for inactive members in better visual order
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            targetDate = dateTimePicker1.Value;
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
                UpdateList();
              
            }
        }

        private void btnCheckInactive_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < InactiveListCheckBox.Items.Count; i++)
            {
                InactiveListCheckBox.SetItemChecked(i, true);
            }
        }
    }
}
