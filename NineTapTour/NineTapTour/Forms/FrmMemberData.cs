using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using NineTapTour.Database;
using System.Globalization;
using NineTapTour.Exceptions;
using System.Text.RegularExpressions;

namespace NineTapTour.Forms
{
    public partial class FrmMemberData : Form
    {
        //IOrderedEnumerable<Member> _membersList;
        int _memberId;
        Member currentMem;
        private int _memberNum;
        public int MemberNum
        {
            set { _memberNum = value; }
        }
        

        public FrmMemberData()
        {
            InitializeComponent();
        }

        private void MemberDataForm_Load(object sender, EventArgs e)
        {
            //_membersList = ((FrmMain)MdiParent)._membersList;
            dateRejoin.Format = DateTimePickerFormat.Custom;
            dateRejoin.CustomFormat = @" ";

            dateLastBowled.Format = DateTimePickerFormat.Custom;
            dateLastBowled.CustomFormat = @" ";

            UpdateMemberInfo();
        }

        public void UpdateMemberInfo(Member searchMem = null)
        {
            _memberNum = Convert.ToInt32(txtMemberNumber.Text);
            if(searchMem == null)
            {
                currentMem = ((FrmMain)MdiParent)._membersList.FirstOrDefault(m => m.Number == _memberNum);
            }
            else
            {
                currentMem = searchMem;
                _memberNum = currentMem.Number;
            }

            if (currentMem == null)
            {
                currentMem = new Member
                {
                    Number = _memberNum
                };
                //Controls.Clear();
                //InitializeComponent();
                txtMemberNumber.Text = _memberNum.ToString();
                _memberId = -1;

                #region Personal Info
                txtMemberNumber.Text = currentMem.Number.ToString();
                txtLastName.Text = "";
                txtFirstName.Text = "";
                txtMiddleInitial.Text = "";
                mtxtBoxDOB.Text = "";
                mtxtBoxSSN.Text = "";
                #endregion

                #region Postal Address
                txtAddress.Text = "";
                txtCity.Text = "";
                txtState.Text = "";
                mtxtBoxZip.Text = "";
                #endregion

                #region Contact Info
                txtEmail.Text = "";
                mtxtBoxPhone.Text = "";
                mtxtBoxPhone2.Text = "";
                #endregion

                #region Score Info
                txtAverage.Text = "";
                txtHandicap.Text = "";
                txtBonus.Text = "";
                #endregion

                #region Misc. Info

                dateJoined.Format = DateTimePickerFormat.Custom;
                dateJoined.CustomFormat = @" ";

                //dateJoined.Value = currentMem.JoinDate;
                //if (currentMem.RejoinDate.HasValue)
                //{
                //    dateRejoin.Value = (DateTime)currentMem.RejoinDate;
                //}
                //else
                //{
                //    dateRejoin.Format = DateTimePickerFormat.Custom;
                //    dateRejoin.CustomFormat = @" ";
                //}
                //if (currentMem.LastBowled.HasValue)
                //{
                //    dateLastBowled.Value = (DateTime)currentMem.LastBowled;
                //}
                //else
                //{
                //    dateLastBowled.Format = DateTimePickerFormat.Custom;
                //    dateLastBowled.CustomFormat = @" ";
                //}
                txtMoneyEarned.Text = "";
                txtNotes.Text = "";
                txtReferrals.Text = "";
                chbSenior.Checked = false;

                foreach(var check in grpStatus.Controls.OfType<RadioButton>())
                {
                    check.Checked = false;
                }

                foreach (var check in grpGender.Controls.OfType<RadioButton>())
                {
                    check.Checked = false;
                }
                #endregion
            }
            else
            {
                #region Personal Info
                _memberId = currentMem.Id;
                txtMemberNumber.Text = currentMem.Number.ToString();
                txtLastName.Text = currentMem.LastName;
                txtFirstName.Text = currentMem.FirstName;
                txtMiddleInitial.Text = currentMem.MiddleInitial;
                mtxtBoxDOB.Text = currentMem.DateOfBirth.ToShortDateString();
                mtxtBoxSSN.Text = currentMem.SSN;
               // txtSSN.PasswordChar = '*'; //This hides the SSN within the form of '*'.
                #endregion

                #region Postal Address
                txtAddress.Text = currentMem.Street;
                txtCity.Text = currentMem.City;
                txtState.Text = currentMem.State;
                mtxtBoxZip.Text = currentMem.PostalCode;
                #endregion

                #region Contact Info
                txtEmail.Text = currentMem.Email;
                mtxtBoxPhone.Text = currentMem.PrimaryPhone;
                mtxtBoxPhone2.Text = currentMem.SecondaryPhone;
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
        }

        //public Member searchList(int memberNumber)
        //{
        //    currentMem = _membersList.FirstOrDefault(m => m.Number == memberNumber);
        //    return currentMem;
        //}

        // method checks for valid characters. 
        // UPDATE THIS METHOD: add more textfields to validate for the whole form to submit
        public bool isValid( String firstName, String lastName, String zip)
        {
            firstName = txtFirstName.Text;
            lastName = txtLastName.Text;
            zip = mtxtBoxZip.Text;
           
            // check if Active radio button is checked
            if (!rdoActive.Checked && !rdoInActive.Checked)
            {
                MessageBox.Show("member must be checked active or inactive");
                return false;
            }
            // check if gender radio button is checked
            if (!rdoMale.Checked && !rdoFemale.Checked)
            {
                MessageBox.Show("a gender must be chosen");
                return false;
            }
            if(!Regex.IsMatch(firstName, "^[a-zA-Z]+$"))
            {
                 MessageBox.Show("field cannot be blank"); 
                 txtFirstName.Clear();
                 return false;
                
            }
            
           if(!Regex.IsMatch(lastName, "^[a-zA-Z]+$"))
             {
                 MessageBox.Show("field cannot be blank");
                 txtLastName.Clear();
                 return false;
                 
             } 
            if(!Regex.IsMatch(zip, "^\\d{5}(?:[-\\s]\\d{4})?$"))
            {
                MessageBox.Show("Invalid zip code field");
                 mtxtBoxZip.Clear();
                 return false;
                
            }
            return true;
             //return Regex.IsMatch(firstName, "^[a-zA-Z]+$");
        
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            isValid(txtFirstName.Text, txtLastName.Text, mtxtBoxZip.Text);

            //checks to see if firstname,lastname, and zip is valid.
            //Then runs the rest of the btnSave_Click and adds a member into the database.
            //TODO: needs more fields to validate inorder for the user to save a new member. (SSN, Address, Phone Number, etc...)
            if (isValid(txtFirstName.Text, txtLastName.Text, mtxtBoxZip.Text))
            {
            var confirm = MessageBox.Show(@"Are You Sure?", @"Confirm Save", MessageBoxButtons.YesNo, MessageBoxIcon.Question);


            if (confirm == DialogResult.No) 
                return;
            Member temp;

            if(_memberId != -1)
            {
                temp = new Member
                {
                    Id = _memberId,
                    Number = Convert.ToInt32(txtMemberNumber.Text),
                    IsActive = rdoActive.Checked,
                    JoinDate = DateTime.Now,

                    #region Personal Info
                    LastName = txtLastName.Text,
                    FirstName = txtFirstName.Text,
                    MiddleInitial = txtMiddleInitial.Text,
                    DateOfBirth = Convert.ToDateTime(mtxtBoxDOB),
                    SSN = mtxtBoxSSN.Text,
                    IsSenior = chbSenior.Checked,
                    Gender = (rdoFemale.Checked) ? MemberGenders.Female : MemberGenders.Male,
                    #endregion

                    #region Postal Address
                    Street = txtAddress.Text,
                    City = txtCity.Text,
                    State = txtState.Text,
                    PostalCode = mtxtBoxZip.Text,
                    #endregion

                    #region Contact Info
                    Email = txtEmail.Text,
                    PrimaryPhone =  mtxtBoxPhone.Text,
                    SecondaryPhone = mtxtBoxPhone2.Text,
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

            }
            else
            {
                temp = new Member()
                {
                    Number = Convert.ToInt32(txtMemberNumber.Text),
                    IsActive = rdoActive.Checked,
                    JoinDate = DateTime.Now,

                    #region Personal Info
                    LastName = txtLastName.Text,
                    FirstName = txtFirstName.Text,
                    MiddleInitial = txtMiddleInitial.Text,
                    DateOfBirth = Convert.ToDateTime(mtxtBoxDOB),
                    SSN = mtxtBoxSSN.Text,
                    IsSenior = chbSenior.Checked,
                    Gender = (rdoFemale.Checked) ? MemberGenders.Female : MemberGenders.Male,
                    #endregion

                    #region Postal Address
                    Street = txtAddress.Text,
                    City = txtCity.Text,
                    State = txtState.Text,
                    PostalCode = mtxtBoxZip.Text,
                    #endregion

                    #region Contact Info
                    Email = txtEmail.Text,
                    PrimaryPhone = mtxtBoxPhone.Text,
                    SecondaryPhone = mtxtBoxPhone2.Text,
                    #endregion

                    #region Score Info
                    Average = (txtAverage.Text == string.Empty) ? 0 : Convert.ToInt16(txtAverage.Text),
                    Handicap = (txtHandicap.Text == string.Empty) ? 0 : Convert.ToInt16(txtHandicap.Text),
                    Bonus = (txtBonus.Text == string.Empty) ? 0 : Convert.ToInt16(txtBonus.Text),
                    #endregion

                    #region Misc. Info
                    RejoinDate = (dateRejoin.CustomFormat == @" ") ? (DateTime?)null : dateRejoin.Value,
                    LastBowled = (dateLastBowled.CustomFormat == @" ") ? (DateTime?)null : dateLastBowled.Value,
                    MoneyEarned = (txtMoneyEarned.Text == string.Empty) ? 0 : decimal.Parse(txtMoneyEarned.Text, NumberStyles.Currency),
                    //MoneyEarned = (txtMoneyEarned.Text == string.Empty) ? 0 : Convert.ToDecimal(txtMoneyEarned.Text),
                    Notes = txtNotes.Text,
                    Referrals = txtReferrals.Text == string.Empty ? 0 : Convert.ToInt16(txtReferrals.Text)
                    #endregion
                };
            }

            // Adds Member to Database

            try
            {
                MemberDb.AddMember(temp);

                MessageBox.Show(@"Bowler Added Successfully.");
                //_membersList = MemberDb.GetMemberList().OrderBy(m => m.Number);
                ((FrmMain)MdiParent)._membersList = MemberDb.GetMemberList().OrderBy(m => m.Number);
                //_membersList = ((FrmMain)MdiParent)._membersList;
            }
            catch (MemberTableException ex)
            {
                MessageBox.Show(ex.Message);
            }

            }
            //else, there must be a validation error. Either something is null or the format of how the user entered was incorrect.
            else
            {
                MessageBox.Show("You have validation problems");
            }

        }

        private void btnArrowLeft_Click(object sender, EventArgs e)
        {
            if (currentMem.Number <= ((FrmMain)MdiParent)._membersList.First().Number)
            {
                MessageBox.Show(@"Beginning of file.", @"Notice");
            }
            else
            {
                txtMemberNumber.Text = (currentMem.Number - 1).ToString();
                UpdateMemberInfo();
            }

        }

        private void btnRightArrow_Click(object sender, EventArgs e)
        {
            if (currentMem.Number >= ((FrmMain)MdiParent)._membersList.Last().Number)
            {
                MessageBox.Show(@"End of file.", @"Notice");
            }
            else
            {
                txtMemberNumber.Text = (currentMem.Number + 1).ToString();
                UpdateMemberInfo();
            }

        }

        //After the InitializeComponent(); call, the dateRejoin Format & dateLastBowled are reused.
        private void btnNew_Click(object sender, EventArgs e)
        {
            Controls.Clear();
            InitializeComponent();
            dateRejoin.Format = DateTimePickerFormat.Custom;
            dateRejoin.CustomFormat = @" ";

            dateLastBowled.Format = DateTimePickerFormat.Custom;
            dateLastBowled.CustomFormat = @" ";
            _memberId = -1;
            txtMemberNumber.Text = (((FrmMain)MdiParent)._membersList.Last().Number + 1).ToString(); 
            currentMem = new Member
            {
                Number = (((FrmMain)MdiParent)._membersList.Last().Number + 1)
            };
        }

        private void btnFirstRecord_Click(object sender, EventArgs e)
        {
            txtMemberNumber.Text = ((FrmMain)MdiParent)._membersList.First().Number.ToString();
            UpdateMemberInfo();
        }

        private void btnLastRecord_Click(object sender, EventArgs e)
        {
            txtMemberNumber.Text = ((FrmMain)MdiParent)._membersList.Last().Number.ToString();
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

        private void btnDelete_Click(object sender, EventArgs e)
        {
            var confirm = MessageBox.Show(@"Are You Sure?", @"Confirm Save", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirm == DialogResult.No) return;
            try
            {
                MemberDb.DeleteMember(currentMem);

                MessageBox.Show(@"Bowler Removed Successfully.");
                //_membersList = MemberDb.GetMemberList().OrderBy(m => m.Number);
                ((FrmMain)MdiParent)._membersList = MemberDb.GetMemberList().OrderBy(m => m.Number);
                //_membersList = ((FrmMain)MdiParent)._membersList;
                UpdateMemberInfo();
            }
            catch (MemberTableException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnMemberNumber_Click(object sender, EventArgs e)
        {
            var newfrmStart = new FrmSearch();
            Width = newfrmStart.Width;
            Height = newfrmStart.Height + 20;
            newfrmStart.Show();
            //newfrmStart.WindowState = FormWindowState.Maximized; for maximizing the window.
        }
    }
}
