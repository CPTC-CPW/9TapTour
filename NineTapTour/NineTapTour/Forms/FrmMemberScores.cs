using NineTapTour.Database;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NineTapTour.Forms
{
    public partial class pnlMemberStatus : Form
    {
        //IOrderedEnumerable<Member> _membersList;
        Member currentMem;
        TextBox[] scratchArray = new TextBox[4];
        TextBox[] handicappArray = new TextBox[4];

        public pnlMemberStatus()
        {
            InitializeComponent();
        }

        private void FrmMemberScores_Load(object sender, EventArgs e)
        {
            //_membersList = ((FrmMain)MdiParent)._membersList;
            scratchArray = new TextBox[4] { txtScratchScore1, txtScratchScore2, txtScratchScore3, txtScratchScore4 };
            handicappArray = new TextBox[4]{txtHandicapScore1, txtHandicapScore2, txtHandicapScore3, txtHandicapScore4};
            
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
                    MemberStatus("", Color.Black, SystemColors.Control, true);
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
            MemberStatus("", Color.Black, SystemColors.Control, true);
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
                    if (currentMem.IsActive)
                    {
                        MemberStatus("Active", Color.Green, Color.Lime, false);
                    }
                    else
                    {
                        //lblMemberStatus.Text = "Inactive";
                        //lblMemberStatus.ForeColor = System.Drawing.Color.Red;
                        //pnlMemStat.BackColor = System.Drawing.Color.Pink;
                        ////Will change later, just for presentation
                        //txtScratchScore1.ReadOnly = true;
                        //txtScratchScore2.ReadOnly = true;
                        //txtScratchScore3.ReadOnly = true;
                        //txtScratchScore4.ReadOnly = true;
                        MemberStatus("Inactive", Color.Red, Color.Pink, true);
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
        public void MemberStatus(string text, Color forColor, Color backColor, bool active)
        {
            lblMemberStatus.Text = text;
            lblMemberStatus.ForeColor = forColor;
            pnlMemStat.BackColor = backColor;
            foreach(TextBox scratch in scratchArray)
            {
                scratch.Clear();
                scratch.ReadOnly = active;
            }
        }

        public void scratchTotal(object sender, EventArgs e)
        {
            int scratchTotal = 0;
            int cScore = 0;
            string id;
            foreach (TextBox score in scratchArray)
            {
                id = Regex.Match(score.Name, @"\d+").Value;
                if (int.TryParse(score.Text, out cScore))
                {
                    if (cScore >= 0 && cScore <= 300)
                    {
                        scratchTotal += cScore;
                        handicapTotal(id, cScore);
                    }
                    else
                    {
                        MessageBox.Show("Score out of range.", "Error");
                        score.Clear();
                    }
                }
                else
                {
                    score.Clear();
                    handicapTotal(id, cScore);
                }
                txtScratchTotal.Text = scratchTotal.ToString();
            }
        }

        private void handicapTotal(string id, int score)
        {
            int totalScore = 0;
            foreach(TextBox hScore in handicappArray)
            {
                if(hScore.Name.Contains(id))
                {
                    if(score!= 0 && txtHandicap.Text !="" && txtBonusPins.Text !="")
                    {
                        hScore.Text = Convert.ToString(score + Convert.ToInt32(txtHandicap.Text) + Convert.ToInt32(txtBonusPins.Text));
                    }
                    else
                    {
                        hScore.Clear();
                    }
                }
                if(hScore.Text!="")
                {
                    totalScore += Convert.ToInt32(hScore.Text);
                }
            }
            txtHandicapTotal.Text = Convert.ToString(totalScore);
        }
    }
}
