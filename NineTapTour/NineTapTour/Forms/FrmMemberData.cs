using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using NineTapTour.Database;
using System.Globalization;

namespace NineTapTour.Forms
{
    public partial class FrmMemberData : Form
    {
        List<Member> _membersList = MemberDb.GetMemberList();
        int memberID;

        public FrmMemberData()
        {
            InitializeComponent();
        }

        public void UpdateMemberInfo()
        {
            var currentMem = _membersList.FirstOrDefault(m => m.Number == Convert.ToInt32(txtMemberNumber.Text));

            if (currentMem != null)
            {
                #region Personal Info
                memberID = currentMem.Id;
                txtMemberNumber.Text = currentMem.Number.ToString();
                txtLastName.Text = currentMem.LastName;
                txtFirstName.Text = currentMem.FirstName;
                txtMiddleInitial.Text = currentMem.MiddleInitial;
                txtDOB.Text = currentMem.DateOfBirth.ToShortDateString();
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
                txtDateJoined.Text = currentMem.JoinDate.ToShortDateString();
                if (currentMem.RejoinDate.HasValue)
                {
                    txtRejoinDate.Text = currentMem.RejoinDate.GetValueOrDefault().ToShortDateString();
                }
                if (currentMem.LastBowled.HasValue)
                {
                    txtLastBowled.Text = currentMem.LastBowled.GetValueOrDefault().ToShortDateString();
                }
                txtMoneyEarned.Text = currentMem.MoneyEarned.ToString("C");
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
            else if (_membersList.Count != 0)
            {
                Controls.Clear();
                InitializeComponent();
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
            var confirm = MessageBox.Show(@"Are You Sure?", @"Confirm Save", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirm == DialogResult.No) return;
            var temp = new Member
            {
                Id = memberID,
                Number = Convert.ToInt32(txtMemberNumber.Text),
                IsActive = rdoActive.Checked,
                JoinDate = DateTime.Now,

                #region Personal Info
                LastName = txtLastName.Text,
                FirstName = txtFirstName.Text,
                MiddleInitial = txtMiddleInitial.Text,
                DateOfBirth = Convert.ToDateTime(txtDOB.Text),
                SSN = txtSSN.Text,
                IsSenior = chbSenior.Checked,
                Gender = (rdoFemale.Checked) ? MemberGenders.Female : MemberGenders.Male,
                #endregion

                #region Postal Address
                Street = txtAddress.Text,
                City = txtCity.Text,
                State = txtState.Text,
                PostalCode = txtZip.Text,
                #endregion

                #region Contact Info
                Email = txtEmail.Text,
                PrimaryPhone = txtPhoneNumber.Text,
                SecondaryPhone = txtPhoneNumber2.Text,
                #endregion

                #region Score Info
                Average = (txtAverage.Text == string.Empty) ? 0 : Convert.ToInt16(txtAverage.Text),
                Handicap = (txtHandicap.Text == string.Empty) ? 0 : Convert.ToInt16(txtHandicap.Text),
                Bonus = (txtBonus.Text == string.Empty) ? 0 : Convert.ToInt16(txtBonus.Text),
                #endregion

                #region Misc. Info
                RejoinDate = (txtRejoinDate.Text == string.Empty) ? (DateTime?)null : Convert.ToDateTime(txtRejoinDate.Text),
                LastBowled = (txtLastBowled.Text == string.Empty) ? (DateTime?)null : Convert.ToDateTime(txtLastBowled.Text),
                MoneyEarned = (txtMoneyEarned.Text == string.Empty) ? 0 : decimal.Parse(txtMoneyEarned.Text, NumberStyles.Currency),
                //MoneyEarned = (txtMoneyEarned.Text == string.Empty) ? 0 : Convert.ToDecimal(txtMoneyEarned.Text),
                Notes = txtNotes.Text,
                Referrals = txtReferrals.Text == string.Empty ? 0 : Convert.ToInt16(txtReferrals.Text)
                #endregion
            };

            if (!MemberDb.AddMember(temp)) return;
            MessageBox.Show(@"Bowler Added Successfully.");
            _membersList = MemberDb.GetMemberList();
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
                MessageBox.Show(@"Beginning of file.", @"Notice");
            }

        }

        private void btnRightArrow_Click(object sender, EventArgs e)
        {
            if (Convert.ToInt32(txtMemberNumber.Text) < _membersList.Count())
            {
                txtMemberNumber.Text = (Convert.ToInt32(txtMemberNumber.Text) + 1).ToString();
                UpdateMemberInfo();
            }
            else
            {
                MessageBox.Show(@"End of file.", @"Notice");
            }

        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            Controls.Clear();
            InitializeComponent();
            txtMemberNumber.Text = (_membersList.Count + 1).ToString();
        }

        private void btnFirstRecord_Click(object sender, EventArgs e)
        {
            txtMemberNumber.Text = @"1";
            UpdateMemberInfo();
        }

        private void btnLastRecord_Click(object sender, EventArgs e)
        {
            txtMemberNumber.Text = _membersList.Count().ToString();
            UpdateMemberInfo();
        }

        private void btnMemberNumber_Click(object sender, EventArgs e)
        {
            //string schNumber = ShowDialog("Seach By Number", "Member Number To Search:");
        }


        private void InputRequired (object sender, EventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null)
            {
                textBox.BackColor = textBox.Text == string.Empty ? Color.LightPink : Color.White;
            }
        }
    }
}
