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
                List<Member> AllMembers = MemberDb.GetMemberList();
              
                AllMembers.ForEach(delegate(Member mem
                    ) {
                        if (mem.IsActive) {
                            InActiveList.Add(mem);
                            string lastplayed = (mem.LastBowled).ToString();
                            if (lastplayed == "") {
                                lastplayed = "never";
                            }
                           string InActive = mem.LastName + ", " + mem.FirstName + " Last game: " + lastplayed + " ID: "+ mem.Number;
                           this.checkedListBox1.Items.Add(InActive, false);
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

        }

        private void btnUpdateActive_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Update the selected Members to inactive?", "", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                try
                {
                    MessageBox.Show(currentMem.FirstName + " This One");
                }
                catch
                {
                    MessageBox.Show("No Members Selected");
                }
              
            }
        }

        private void btnCheckInactive_Click(object sender, EventArgs e)
        {

        }
    }
}
