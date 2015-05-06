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

namespace NineTapTour
{
    public partial class FrmMemberData : Form
    {
        List<Member> membersList = MemberDB.getMember();

        public FrmMemberData()
        {
            InitializeComponent();
        }

        public void UpdateMemberInfo()
        {

            Member currentMem = new Member();

            foreach (Member m in membersList)
            {
                if (m.Number == Convert.ToInt32(txtMemberNumber.Text))
                {
                    currentMem = m;
                }
            }

            if (currentMem.Number != 0)
            {
                #region Personal Info
                txtMemberNumber.Text = currentMem.Number.ToString();
                txtLastName.Text = currentMem.LastName;
                txtFirstName.Text = currentMem.FirstName;
                txtMiddleInitial.Text = currentMem.MiddleInitial;
                txtDOB.Text = currentMem.DateOfBirth.ToString();
                txtSSN.Text = currentMem.SSN;
                #endregion

                #region Postal Address
                txtAddress.Text = currentMem.Street;
                txtCity.Text = currentMem.City;
                txtState.Text = currentMem.State;
                txtZip.Text = currentMem.PostalCode;
                #endregion

                #region Contact Info
                txtEmail.Text = currentMem.Email;
                txtPhoneNumber.Text = currentMem.PrimaryPhone;
                txtPhoneNumber2.Text = currentMem.SecondaryPhone;
                #endregion

                #region Score Info
                txtAverage.Text = currentMem.Average.ToString();
                txtHandicap.Text = currentMem.Handicap.ToString();
                txtBonus.Text = currentMem.Bonus.ToString();
                #endregion

                #region Misc. Info
                txtDateJoined.Text = currentMem.JoinDate.ToString();
                txtreJoinDate.Text = currentMem.RejoinDate.ToString();
                txtLastBowled.Text = currentMem.LastBowled.ToString();
                txtMoneyEarned.Text = currentMem.MoneyEarned.ToString();
                txtNotes.Text = currentMem.Notes;
                txtReferrals.Text = currentMem.Referrals.ToString();
                chbSenior.Checked = currentMem.IsSenior;
                
                if (currentMem.IsActive)
                {
                    rdoActive.Checked = true;
                }
                else
                {
                    rdoInActive.Checked = true;
                }
                if (currentMem.Gender.ToString() == MemberGenders.Female.ToString())
                {
                    rdoFemale.Checked = true;
                }
                else
                {
                    rdoMale.Checked = true;
                }
                #endregion

            }
            else if (membersList.Count != 0)
            {
                this.Controls.Clear();
                this.InitializeComponent();
                //UpdateMemberInfo();
            }
        }

        //public static string ShowDialog(string text, string caption)
        //{
        //    Form prompt = new Form();
        //    prompt.Width = 500;
        //    prompt.Height = 150;
        //    prompt.FormBorderStyle = FormBorderStyle.FixedDialog;
        //    prompt.Text = caption;
        //    prompt.StartPosition = FormStartPosition.CenterScreen;
        //    Label lblInfo = new Label() { Left = 50, Top = 20, Text = text };
        //    TextBox txtSearch = new TextBox() { Left = 50, Top = 50, Width = 400 };
        //    Button confirmation = new Button() { Text = "Ok", Left = 350, Width = 100, Top = 70 };
        //    confirmation.Click += (sender, e) => { prompt.Close(); };
        //    prompt.Controls.Add(txtSearch);
        //    prompt.Controls.Add(confirmation);
        //    prompt.Controls.Add(lblInfo);
        //    prompt.AcceptButton = confirmation;
        //    prompt.ShowDialog();
        //    return txtSearch.Text;
        //}

        private void MemberDataForm_Load(object sender, EventArgs e)
        {
            UpdateMemberInfo();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            DialogResult confirm = MessageBox.Show("Are You Sure?", "Confirm Save", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirm == DialogResult.Yes)
            {
                Member temp = new Member();
                #region Personal Info
                temp.Number = Convert.ToInt32(txtMemberNumber.Text);
                temp.LastName = txtLastName.Text;
                temp.FirstName = txtFirstName.Text;
                temp.MiddleInitial = txtMiddleInitial.Text;
                temp.DateOfBirth = Convert.ToDateTime(txtDOB.Text);
                temp.SSN = txtSSN.Text;
                #endregion

                #region Postal Address
                temp.Street = txtAddress.Text;
                temp.City = txtCity.Text;
                temp.State = txtState.Text;
                temp.PostalCode = txtZip.Text;
                #endregion

                #region Contact Info
                temp.Email = txtEmail.Text;
                temp.PrimaryPhone = txtPhoneNumber.Text;
                temp.SecondaryPhone = txtPhoneNumber2.Text;
                #endregion

                #region Score Info
                if (txtAverage.Text == "")
                {
                    temp.Average = 0;
                }
                else
                {
                    temp.Average = Convert.ToInt16(txtAverage.Text);
                }
                if (txtHandicap.Text == "")
                {
                    temp.Handicap = 0;
                }
                else
                {
                    temp.Handicap = Convert.ToInt16(txtHandicap.Text);
                }
                if (txtBonus.Text == "")
                {
                    temp.Bonus = 0;
                }
                else
                {
                    temp.Bonus = Convert.ToInt16(txtBonus.Text);
                }
                #endregion

                #region Misc. Info
                temp.JoinDate = DateTime.Now;
                if(txtreJoinDate.Text != "")
                {
                    temp.RejoinDate = Convert.ToDateTime(txtreJoinDate.Text);
                }
                if(txtLastBowled.Text != "")
                {
                    temp.LastBowled = Convert.ToDateTime(txtLastBowled.Text);
                }
                if(txtMoneyEarned.Text !="")
                {
                    temp.MoneyEarned = Convert.ToDecimal(txtMoneyEarned.Text);
                }
                temp.Notes = txtNotes.Text;
                if (txtReferrals.Text == "")
                {
                    temp.Referrals = 0;
                }
                else
                {
                    temp.Referrals = Convert.ToInt16(txtReferrals.Text);
                }

                if (chbSenior.Checked)
                {
                    temp.IsSenior = true;
                }
                else
                {
                    temp.IsSenior = false;
                }
                if (rdoActive.Checked)
                {
                    temp.IsActive = true;
                }
                else if(rdoInActive.Checked)
                {
                    temp.IsActive = false;
                }

                if (rdoFemale.Checked)
                {
                    temp.Gender = MemberGenders.Female;
                }
                else if(rdoMale.Checked)
                {
                    temp.Gender = MemberGenders.Male;
                }
                #endregion

                if (MemberDB.addMember(temp))
                {
                    MessageBox.Show("Bowler Added Successfully.");
                    membersList = MemberDB.getMember();
                }


            }
        }

        private void btnArrowLeft_Click(object sender, EventArgs e)
        {
            if (Convert.ToInt32(txtMemberNumber.Text) != 1)
            {
                txtMemberNumber.Text = (Convert.ToInt32(txtMemberNumber.Text) - 1).ToString();
                UpdateMemberInfo();
            }
            else
            {
                MessageBox.Show("Beginning of file.", "Notice");
            }

        }

        private void btnRightArrow_Click(object sender, EventArgs e)
        {
            if (Convert.ToInt32(txtMemberNumber.Text) < membersList.Count())
            {
                txtMemberNumber.Text = (Convert.ToInt32(txtMemberNumber.Text) + 1).ToString();
                UpdateMemberInfo();
            }
            else
            {
                MessageBox.Show("End of file.", "Notice");
            }

        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            this.Controls.Clear();
            this.InitializeComponent();
            txtMemberNumber.Text = (membersList.Count + 1).ToString();
        }

        private void btnFirstRecord_Click(object sender, EventArgs e)
        {
            txtMemberNumber.Text = "1";
            UpdateMemberInfo();
        }

        private void btnLastRecord_Click(object sender, EventArgs e)
        {
            txtMemberNumber.Text = membersList.Count().ToString();
            UpdateMemberInfo();
        }

        private void btnMemberNumber_Click(object sender, EventArgs e)
        {
            //string schNumber = ShowDialog("Seach By Number", "Member Number To Search:");
        }


        private void inputRequired (object sender, EventArgs e)
        {
            if (sender is TextBox)
            {
                TextBox tb = (TextBox)sender;
                if (tb.Text == "")
                {
                    tb.BackColor = System.Drawing.Color.IndianRed;
                }
                else
                    tb.BackColor = System.Drawing.Color.White;
            }
        }
    }
}
