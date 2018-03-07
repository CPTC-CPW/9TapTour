using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Globalization;
using System.Text.RegularExpressions;
using Member_Import_Test.Classes;
using NineTapTour.Database;

namespace Member_Import_Test
{
    public partial class FrmMemberData : Form
    {
        int _memberId;
        Member currentMem;
        private int _memberNum;
        public int MemberNum
        {
            set { _memberNum = value; }
        }
        /// <summary>
        /// Opens the "Member Data" Form.
        /// </summary>
        List<Member> invalidMembers;
        int listPosition = 0;
        Form home;
        public FrmMemberData(List<Member> Invalid, frmMain main)
        {
            InitializeComponent();
            invalidMembers = Invalid;
            home = main;
        }
        /// <summary>
        /// Updates information in the "Member Data" form.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MemberDataForm_Load(object sender, EventArgs e)
        {
            UpdateMemberInfo();
        }
        /// <summary>
        /// Finds "Member Number" in the database and populates the "Member Data" form.
        /// If that "Member Number" is not assigned then display error box.
        /// </summary>
        /// <param name="searchMem"></param>
        public void UpdateMemberInfo(Member searchMem = null)
        {
            
            
            currentMem = invalidMembers[listPosition];

            #region Personal Info
            _memberId = currentMem.Id;
            txtMemberNumber.Text = currentMem.Number.ToString();
            txtLastName.Text = currentMem.LastName;
            txtFirstName.Text = currentMem.FirstName;
            txtMiddleInitial.Text = currentMem.MiddleInitial;
            mtxtBoxDOB.Text = currentMem.DateOfBirth.ToString();
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

            //need to set up some sort of check if they don't have a join
            Console.WriteLine(currentMem.JoinDate);
            DateTime nullDate = new DateTime(1/1/0001);
            txtdateJoined.Text = currentMem.JoinDate.ToString();
            txtrejoinDate.Text = currentMem.RejoinDate.ToString();
            txtlastBowled.Text = currentMem.LastBowled.ToString();
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

        //public Member searchList(int memberNumber)
        //{
        //    currentMem = _membersList.FirstOrDefault(m => m.Number == memberNumber);
        //    return currentMem;
        //}

        // method checks for valid characters. 
        // TODO: add more textfields to validate for the whole form to submit
        public bool isValid()
        {
            DateTime outPut;
            // check if Active radio button is checked
            if (!rdoActive.Checked && !rdoInActive.Checked)
            {
                MessageBox.Show("Member must be checked active or inactive");
                return false;
            }
            // check if gender radio button is checked
            if (!rdoMale.Checked && !rdoFemale.Checked)
            {
                MessageBox.Show("A gender must be chosen");
                return false;
            }
            if (!Regex.IsMatch(txtFirstName.Text, "^[a-zA-Z0-9_]+( +[a-zA-Z0-9_]+)*$"))
            {// old regex : "^[a-zA-Z]+$"
                MessageBox.Show("First Name field is invalid");
                return false;
            }
            //use better regex expression that includes spaces and hyphens
            if (!Regex.IsMatch(txtLastName.Text, "^[a-zA-Z0-9_]+( +[a-zA-Z0-9_]+)*$"))
            {
                MessageBox.Show("Last Name field is invalid");
                return false;
            }
            if (!Regex.IsMatch(mtxtBoxZip.Text, "^\\d{5}(?:[-\\s]\\d{4})?$"))
            {
                MessageBox.Show("Invalid zip code field");
                return false;
            }
            if (!Regex.IsMatch(mtxtBoxPhone.Text, "^(\\+\\d{1,2}\\s)?\\(?\\d{3}\\)?[\\s.-]\\d{3}[\\s.-]\\d{4}$"))
            {
                MessageBox.Show("Invalid Primary Phone field");
                return false;
            }
            if (txtAddress.Text == "")
            {
                MessageBox.Show("Address field cannot be null");
                return false;
            }
            if (txtCity.Text == "")
            {
                MessageBox.Show(" City field cannot be null");
                return false;
            }
            if (txtEmail.Text == "")
            {
                MessageBox.Show("email field cannot be null");
                return false;
            }
            if (txtState.Text == "")
            {
                MessageBox.Show("state field cannot be null");
                return false;
            }
            if (mtxtBoxDOB.Text == "01/01/0001" || !DateTime.TryParse(mtxtBoxDOB.Text, out outPut))
            {
                MessageBox.Show("Date Of Birth Must be a valid date.");
                return false;
            }
            if(txtdateJoined.Text== "1/1/0001 12:00:00 AM" || !DateTime.TryParse(txtdateJoined.Text, out outPut))
            {
                MessageBox.Show("Join Date must be a valid Date.");
                return false;
            }
            if(txtrejoinDate.Text != "")
            {
                if (!DateTime.TryParse(txtrejoinDate.Text, out outPut))
                {
                    MessageBox.Show("Rejoin Date must be a valid Date.");
                    return false;
                }
            }
            if (txtlastBowled.Text != "")
            {
                if (!DateTime.TryParse(txtlastBowled.Text, out outPut))
                {
                    MessageBox.Show("Last Bowled Date must be a valid Date.");
                    return false;
                }
            }
            int num;
            bool isNum = int.TryParse(txtReferrals.Text, out num);
            if (!isNum && txtReferrals.Text != "")
            {
                MessageBox.Show("Referals Must be a valid number");
                return false;
            }
            return true;
        }
        /// <summary>
        /// Saves the information entered in the "Member Data" form.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, EventArgs e)
        {
            //checks to see if firstname,lastname, and zip is valid.
            //Then runs the rest of the btnSave_Click and adds a member into the database.
      
            if (isValid())
            {
                var confirm = MessageBox.Show(@"Are You Sure?", @"Confirm Save", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (confirm == DialogResult.No)
                    return;
                Member temp;

                //if (_memberId != -1)
                //{
                //    temp = new Member
                //    {
                //        Id = _memberId,
                //        Number = Convert.ToInt32(txtMemberNumber.Text),
                //        IsActive = rdoActive.Checked,
                //        JoinDate = DateTime.Now,

                //        #region Personal Info
                //        LastName = txtLastName.Text,
                //        FirstName = txtFirstName.Text,
                //        MiddleInitial = txtMiddleInitial.Text,
                //        DateOfBirth = Convert.ToDateTime(mtxtBoxDOB.Text),
                //        SSN = mtxtBoxSSN.Text,
                //        IsSenior = chbSenior.Checked,
                //        Gender = (rdoFemale.Checked) ? MemberGenders.Female : MemberGenders.Male,
                //        #endregion

                //        #region Postal Address
                //        Street = txtAddress.Text,
                //        City = txtCity.Text,
                //        State = txtState.Text,
                //        PostalCode = mtxtBoxZip.Text,
                //        #endregion

                //        #region Contact Info
                //        Email = txtEmail.Text,
                //        PrimaryPhone = mtxtBoxPhone.Text,
                //        SecondaryPhone = mtxtBoxPhone2.Text,
                //        #endregion

                //        #region Score Info
                //        Average = (txtAverage.Text == string.Empty) ? 0 : Convert.ToInt16(txtAverage.Text),
                //        Handicap = (txtHandicap.Text == string.Empty) ? 0 : Convert.ToInt16(txtHandicap.Text),
                //        Bonus = (txtBonus.Text == string.Empty) ? 0 : Convert.ToInt16(txtBonus.Text),
                //        #endregion

                //        #region Misc. Info
                //        RejoinDate = Convert.ToDateTime(txtdateJoined.Text),
                //        LastBowled = Convert.ToDateTime(txtlastBowled.Text),
                //        MoneyEarned = (txtMoneyEarned.Text == string.Empty) ? 0 : decimal.Parse(txtMoneyEarned.Text, NumberStyles.Currency),
                //        //MoneyEarned = (txtMoneyEarned.Text == string.Empty) ? 0 : Convert.ToDecimal(txtMoneyEarned.Text),
                //        Notes = txtNotes.Text,
                //        Referrals = txtReferrals.Text == string.Empty ? 0 : Convert.ToInt16(txtReferrals.Text)
                //        #endregion
                //    };

                //}
                //else
                //{
                if (txtReferrals.Text == "")
                {
                    txtReferrals.Text = "0";
                }
                temp = new Member()
                {

                    Id = Convert.ToInt32(txtMemberNumber.Text),
                    Number = Convert.ToInt32(txtMemberNumber.Text),
                    IsActive = rdoActive.Checked,
                    JoinDate = DateTime.Now,

                    #region Personal Info
                    LastName = txtLastName.Text,
                    FirstName = txtFirstName.Text,
                    MiddleInitial = txtMiddleInitial.Text,
                    DateOfBirth = Convert.ToDateTime(mtxtBoxDOB.Text),
                    // SSN = mtxtBoxSSN.Text,
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
                    RejoinDate = Convert.ToDateTime(txtdateJoined.Text),
                    LastBowled = Convert.ToDateTime(txtlastBowled.Text),
                    // MoneyEarned = (txtMoneyEarned.Text == string.Empty) ? 0 : decimal.Parse(txtMoneyEarned.Text, NumberStyles.Currency),
                    //MoneyEarned = (txtMoneyEarned.Text == string.Empty) ? 0 : Convert.ToDecimal(txtMoneyEarned.Text),
                    Notes = txtNotes.Text,
                    Referrals = Convert.ToInt16(txtReferrals.Text)
                    #endregion
                };
                //}

                // Adds Member to Database
                try
                {

                    NineTapTour.Database.MemberDb.AddMember(temp);
                    MessageBox.Show(@"Bowler Added Successfully.");
                    invalidMembers.RemoveAt(listPosition);
                    if(invalidMembers.Count() != 0)
                    {
                        UpdateMemberInfo();
                    }
                    else
                    {
                        MessageBox.Show("All invalid members processed, returning to main menu.");
                        this.Close();
                        home.Show();
                    }
                    
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        /// <summary>
        /// Displays the previous "Member Number"'s information when the left arrow button is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnArrowLeft_Click(object sender, EventArgs e)
        {
            if (invalidMembers.Count() == 0 || currentMem.Number <= invalidMembers.First().Number)
            {
                MessageBox.Show(@"Beginning of file.", @"Notice");
            }
            else
            {
                listPosition--;
                UpdateMemberInfo();
            }

        }

        /// <summary>
        /// Displays the next "Member Number"'s information when the right arrow button is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRightArrow_Click(object sender, EventArgs e)
        {
            if (invalidMembers.Count() == 0 || currentMem.Number >= invalidMembers.Last().Number)
            {
                MessageBox.Show(@"End of file.", @"Notice");
            }
            else
            {
                listPosition++;
                UpdateMemberInfo();
            }

        }

        //After the InitializeComponent(); call, the dateRejoin Format & dateLastBowled are reused.
        /// <summary>
        /// Brings up the first "Member Number".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnFirstRecord_Click(object sender, EventArgs e)
        {
            listPosition = 0;
            UpdateMemberInfo();
        }

        /// <summary>
        /// Brings up the last "Member Number".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLastRecord_Click(object sender, EventArgs e)
        {
            listPosition = invalidMembers.Count - 1;
            UpdateMemberInfo();
        }

        private void InputRequired(object sender, EventArgs e)
        {
            if(sender is TextBox)
            {
                var textBox = sender as TextBox;
                if(textBox.Name == "txtdateJoined")
                {
                    textBox.BackColor = textBox.Text == "1/1/0001 12:00:00 AM" ? Color.LightPink : Color.White;
                }
                else if (textBox.Name == "txtReferrals")
                {
                    int num;
                    bool isNum = int.TryParse(textBox.Text, out num);
                    textBox.BackColor = isNum || textBox.Text.Trim() == "" ? Color.White : Color.LightPink;
                }
                else if(textBox != null && textBox.Name != "txtReferrals")
                {
                    textBox.BackColor = textBox.Text == string.Empty ? Color.LightPink : Color.White;
                }

                if (textBox.Name == "txtlastBowled")
                {
                    DateTime date;
                    bool isDate = DateTime.TryParse(textBox.Text, out date);
                    textBox.BackColor = !isDate || textBox.Text.Trim() == "" ? Color.LightPink : Color.White;
                }
            }
            else if(sender is MaskedTextBox)
            {
                var maskedtextBox = sender as MaskedTextBox;
                if(maskedtextBox.Name=="mtxtBoxPhone")
                {
                    maskedtextBox.BackColor = maskedtextBox.Text == "(   )    -" ? Color.LightPink : Color.White;
                }
                else if (maskedtextBox.Name == "mtxtBoxDOB")
                {
                    maskedtextBox.BackColor = maskedtextBox.Text == "01/01/0001" ? Color.LightPink : Color.White;
                }
                else if (maskedtextBox != null)
                {
                    maskedtextBox.BackColor = maskedtextBox.Text == string.Empty ? Color.LightPink : Color.White;
                }
            }

            
        }

        private void FrmMemberData_FormClosed(object sender, FormClosedEventArgs e)
        {
            home.Show();
        }

        private void txtNotes_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
