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
    public partial class pnlMemberStatus : Form
    {
        //IOrderedEnumerable<Member> _membersList;
        Member currentMem;

        public pnlMemberStatus()
        {
            InitializeComponent();
        }

        private void FrmMemberScores_Load(object sender, EventArgs e)
        {
            //_membersList = ((FrmMain)MdiParent)._membersList;
        }

        private void txtMemberNum_TextChanged(object sender, EventArgs e)
        {
            
                if(currentMem == null || ((TextBox)sender).Text=="")
                {
                    txtLastName.Clear();
                    txtFirstName.Clear();
                    txtMiddleInitial.Clear();
                    txtHandicap.Clear();
                    txtBonusPins.Clear();
                    lblMemberStatus.Text = "";
                    lblMemberStatus.ForeColor = System.Drawing.Color.Black;
                    pnlMemStat.BackColor = System.Drawing.SystemColors.Control;
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
            lblMemberStatus.Text = "";
            lblMemberStatus.ForeColor = System.Drawing.Color.Black;
            pnlMemStat.BackColor = System.Drawing.SystemColors.Control;
            //Will change later, only for presentation
            txtScratchScore1.ReadOnly = false;
            txtScratchScore2.ReadOnly = false;
            txtScratchScore3.ReadOnly = false;
            txtScratchScore4.ReadOnly = false;
        }

        private void GetMember(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;

            string searchNumber = txtMemberNum.Text;
            //if(searchNumber.Trim()=="") return;
            for (int i = 0; i < searchNumber.Length; i++)
            {
                if (!char.IsNumber(searchNumber[i]))
                {
                    MessageBox.Show("Please input numbers only.", "Your Attention Please.");
                    txtMemberNum.Clear();
                    return;
                }
            }
            if (searchNumber.Trim() != "")
            {
                int memberNumber = Convert.ToInt16(txtMemberNum.Text);
                currentMem = ((FrmMain)MdiParent)._membersList.FirstOrDefault(m => m.Number == memberNumber);
                if (currentMem != null)
                {
                    if(currentMem.IsActive)
                    {
                        lblMemberStatus.Text = "Active";
                        lblMemberStatus.ForeColor = System.Drawing.Color.Green;
                        pnlMemStat.BackColor = System.Drawing.Color.Lime;
                        //Will change later, just for presentation
                        txtScratchScore1.ReadOnly = false;
                        txtScratchScore2.ReadOnly = false;
                        txtScratchScore3.ReadOnly = false;
                        txtScratchScore4.ReadOnly = false;
                    }
                    else
                    {
                        lblMemberStatus.Text = "Inactive";
                        lblMemberStatus.ForeColor = System.Drawing.Color.Red;
                        pnlMemStat.BackColor = System.Drawing.Color.Pink;
                        //Will change later, just for presentation
                        txtScratchScore1.ReadOnly = true;
                        txtScratchScore2.ReadOnly = true;
                        txtScratchScore3.ReadOnly = true;
                        txtScratchScore4.ReadOnly = true;
                    }
                    txtLastName.Text = currentMem.LastName;
                    txtFirstName.Text = currentMem.FirstName;
                    txtMiddleInitial.Text = currentMem.MiddleInitial;
                    txtHandicap.Text = currentMem.Handicap.ToString();
                    txtBonusPins.Text = currentMem.Bonus.ToString();
                }
                else
                {
                    MessageBox.Show(string.Format("A member with the number {0} does not exist", txtMemberNum.Text), "Your Attention Please.");
                    txtMemberNum.Clear();
                }

            }
        } 
    }
}
