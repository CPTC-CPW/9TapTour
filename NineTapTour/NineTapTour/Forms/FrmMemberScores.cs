using NineTapTour.Database;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NineTapTour.Forms
{
    public partial class FrmMemberScores : Form
    {
        //IOrderedEnumerable<Member> _membersList;
        Member currentMem;

        public FrmMemberScores()
        {
            InitializeComponent();
        }

        private void FrmMemberScores_Load(object sender, EventArgs e)
        {
            //_membersList = ((FrmMain)MdiParent)._membersList;
        }

        private void txtMemberNum_TextChanged(object sender, EventArgs e)
        {
            string searchNumber = txtMemberNum.Text;
            if(searchNumber.Trim()=="") return;
                for(int i = 0; i < searchNumber.Length; i++)
                {
                   if(!char.IsNumber(searchNumber[i]))
                   {
                       MessageBox.Show("Please input numbers only.","Your Attention Please.");
                       txtMemberNum.Clear();
                       txtLastName.Clear();
                       txtFirstName.Clear();
                       txtMiddleInitial.Clear();
                       txtHandicap.Clear();
                       txtBonusPins.Clear();
                       return;
                   }
                }
                int memberNumber = Convert.ToInt16(txtMemberNum.Text);
                currentMem = ((FrmMain)MdiParent)._membersList.FirstOrDefault(m => m.Number == memberNumber);
                if(currentMem != null)
                {
                    txtLastName.Text = currentMem.LastName;
                    txtFirstName.Text = currentMem.FirstName;
                    txtMiddleInitial.Text = currentMem.MiddleInitial;
                    txtHandicap.Text = currentMem.Handicap.ToString();
                    txtBonusPins.Text = currentMem.Bonus.ToString();
                }
                else
                {
                    txtLastName.Clear();
                    txtFirstName.Clear();
                    txtMiddleInitial.Clear();
                    txtHandicap.Clear();
                    txtBonusPins.Clear();
                }
                    
        }

        private void FrmMemberScores_Activated(object sender, EventArgs e)
        {
            txtMemberNum.Clear();
            txtLastName.Clear();
            txtFirstName.Clear();
            txtMiddleInitial.Clear();
            txtHandicap.Clear();
            txtBonusPins.Clear();
        } 
    }
}
