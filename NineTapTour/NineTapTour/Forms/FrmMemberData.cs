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
        List<Member> _membersList;
        int _memberId;

        public FrmMemberData()
        {
            InitializeComponent();
        }

        private void MemberDataForm_Load(object sender, EventArgs e)
        {
            _membersList = ((FrmMain)MdiParent)._membersList;

            dateRejoin.Format = DateTimePickerFormat.Custom;
            dateRejoin.CustomFormat = @" ";

            dateLastBowled.Format = DateTimePickerFormat.Custom;
            dateLastBowled.CustomFormat = @" ";

            UpdateMemberInfo();
        }

        public void UpdateMemberInfo()
        {
            var currentMem = _membersList.FirstOrDefault(m => m.Number == Convert.ToInt32(txtMemberNumber.Text));

            if (currentMem != null)
            {
                #region Personal Info
                _memberId = currentMem.Id;
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
                //TODO: Pull datetime from database correctly
                dateJoined.Value = currentMem.JoinDate;
                if (currentMem.RejoinDate.HasValue)
                {
                    dateRejoin.Value = (DateTime) currentMem.RejoinDate;
                }
                else
                {
                    dateRejoin.Format = DateTimePickerFormat.Custom;
                    dateRejoin.CustomFormat = @" ";
                }
                if (currentMem.LastBowled.HasValue)
                {
                    dateLastBowled.Value = (DateTime) currentMem.LastBowled;
                }
                else
                {
                    dateLastBowled.Format = DateTimePickerFormat.Custom;
                    dateLastBowled.CustomFormat = @" ";
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
            else if (_membersList.Count == 0)
            {
                Controls.Clear();
                InitializeComponent();
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            var confirm = MessageBox.Show(@"Are You Sure?", @"Confirm Save", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirm == DialogResult.No) return;

            var temp = new Member
            {
                Id = _memberId,
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
                RejoinDate = (dateRejoin.CustomFormat ==  @" ") ? (DateTime?) null : dateRejoin.Value,
                LastBowled = (dateLastBowled.CustomFormat == @" ") ? (DateTime?) null : dateLastBowled.Value,
                MoneyEarned = (txtMoneyEarned.Text == string.Empty) ? 0 : decimal.Parse(txtMoneyEarned.Text, NumberStyles.Currency),
                //MoneyEarned = (txtMoneyEarned.Text == string.Empty) ? 0 : Convert.ToDecimal(txtMoneyEarned.Text),
                Notes = txtNotes.Text,
                Referrals = txtReferrals.Text == string.Empty ? 0 : Convert.ToInt16(txtReferrals.Text)
                #endregion
            };

            // Adds Member to Database
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

        private void InputRequired(object sender, EventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null)
            {
                textBox.BackColor = textBox.Text == string.Empty ? Color.LightPink : Color.White;
            }
        }

        private void ApplyCalendarForm(object sender, EventArgs e)
        {
            var datePicker = sender as DateTimePicker;

            if (datePicker != null)
            {
                datePicker.Format = DateTimePickerFormat.Short;
            }
        }

        private void ClearCalendar(object sender, KeyEventArgs e)
        {
            var datePicker = sender as DateTimePicker;

            if (datePicker == null || (e.KeyCode != Keys.Delete && e.KeyCode != Keys.Back)) return;

            datePicker.Format = DateTimePickerFormat.Custom;
            datePicker.CustomFormat = @" ";
        }
    }
}
