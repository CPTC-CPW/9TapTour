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

namespace NineTapTour.Forms
{
    public partial class FrmUpdateActiveMem : Form
    {
        Member currentMem;
        DateTime targetDate;
        List<Member> InActiveList = MemberDb.GetMemberList();
        List<Member> AllMembers = MemberDb.GetMemberList();
        public FrmUpdateActiveMem()
        {
            InitializeComponent();
            dateTimePicker1.Value = DateTime.Today.AddDays(-180);
            targetDate = dateTimePicker1.Value;
        }
        
        private void FrmUpdateActiveMem_Load(object sender, EventArgs e)
        {
     
            try
            {

               
                AllMembers.ForEach(delegate(Member mem
                    ) {
                        if (mem.IsActive && (mem.LastBowled >= targetDate || mem.LastBowled.ToString() == ""))
                        {
                            
                            string lastplayed = (mem.LastBowled).ToString();
                            if (lastplayed == "")
                            {
                                lastplayed = "never";
                            }
                            string InActive = String.Format("({2,-5}) {1,10}{3,0}{0,10}",
                             mem.LastName + ", " + mem.FirstName, lastplayed, mem.Number, "     ");
                            //TODO change from string to object
                            checkedListBox1.Items.Add(InActive, false);

                        }

                            
                    });
                currentMem = InActiveList.FirstOrDefault();
                
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
            if (MessageBox.Show("Update the selected Members to inactive?", "", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                try
                {
                    

                    foreach(var item in checkedListBox1.CheckedItems)
                    {
                        string value = item.ToString();
                        string output = value.Split('(', ')')[1];
                        MessageBox.Show(output);//for testing
                        int n = Int32.Parse(output);
                        InActiveList[n].IsActive = false;
                    }
                }
                catch
                {
                    MessageBox.Show("No Members Selected");
                }
              
            }
        }

        private void btnCheckInactive_Click(object sender, EventArgs e)
        {
            if (checkedListBox1.CheckedItems.Count < checkedListBox1.Items.Count)
            { 
                for (int i = 0; i < checkedListBox1.Items.Count; i++)
                {
                    checkedListBox1.SetItemChecked(i, true);
                }
            }
            else
            {
                for (int i = 0; i < checkedListBox1.Items.Count; i++)
                {
                    checkedListBox1.SetItemChecked(i, false);
                }
            }
        }
    }
}
