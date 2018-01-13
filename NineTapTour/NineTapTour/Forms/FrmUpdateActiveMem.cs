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

/// <summary>
/// Author Julie Edwards
/// </summary>



namespace NineTapTour.Forms
{
    public partial class FrmUpdateActiveMem : Form
    {
        int RegionID;
        //Member currentMem;
        DateTime targetDate;
        List<Member> InActiveList; //= MemberDb.GetMemberList();
        List<Member> AllMembers;// = MemberDb.GetMemberList();
        public FrmUpdateActiveMem()
        {
            InitializeComponent();
            dateTimePicker1.Value = DateTime.Today.AddDays(-180);
            targetDate = dateTimePicker1.Value;
        }
        
        private void FrmUpdateActiveMem_Load(object sender, EventArgs e)
        {
            RegionID = ((FrmMain)MdiParent).RegionID;
            InActiveList = MemberDb.GetMemberList(RegionID);
            AllMembers = MemberDb.GetMemberList(RegionID);
            UpdateList();
        }

        private void UpdateList()
        {
            try
            {


                AllMembers.ForEach(delegate (Member mem)
                {
                    if (mem.IsActive && (mem.LastBowled <= targetDate || mem.LastBowled.ToString() == ""))
                    {
                        checkedListBox1.Items.Add(mem);
                    }
                });

            }
            catch
            {
                MessageBox.Show("null list");
            }
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            targetDate = dateTimePicker1.Value;
        }

        private void btnUpdateActive_Click(object sender, EventArgs e)
        {
            var db = new NineTapDb();
            if (MessageBox.Show("Update the selected Members to inactive?", "", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                try
                {

                    //PrintToWord.CreateWordDoc("InActive.txt");//copy to word
                    foreach (Member mem in checkedListBox1.CheckedItems)
                    {

                        mem.IsActive = false;
                        //PrintToWord.WriteWordDoc("Name" + mem.FirstName + " " + mem.LastName);
                        db.Entry(mem).State = EntityState.Modified;
                        db.SaveChanges();
                        checkedListBox1.Items.Clear();
                        UpdateList();
                        //PrintToWord.OpenWordDoc();//Open word doc
                    }
                }
                catch
                {
                    //MessageBox.Show("No Members Selected");
                }
              
            }
        }

    }
}
