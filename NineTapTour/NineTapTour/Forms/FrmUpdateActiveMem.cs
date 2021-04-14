using NineTapTour.Database;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
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
            InActiveList = MemberDB.GetMemberList(RegionID);
            AllMembers = MemberDB.GetMemberList(RegionID);
            UpdateList();
        }

        private void UpdateList()
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
