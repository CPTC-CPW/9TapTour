using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using NineTapTour.Database;
using System.Globalization;
using NineTapTour.Exceptions;
using System.Text.RegularExpressions;
using System.Drawing.Printing;
using System.ComponentModel.DataAnnotations;

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
        
        /// <summary>
        /// Opens the "Member Data" Form.
        /// </summary>
        public FrmMemberData()
        {
            InitializeComponent();
            txtMiddleInitial.MaxLength = 1;
        }

  
        /// <summary>
        /// Updates information in the "Member Data" form.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MemberDataForm_Load(object sender, EventArgs e)
        {
            
            dateJoined.Format = DateTimePickerFormat.Custom;
            dateJoined.CustomFormat = @" ";

            //_membersList = ((FrmMain)MdiParent)._membersList;
            dateRejoin.Format = DateTimePickerFormat.Custom;
            dateRejoin.CustomFormat = @" ";

            dateLastBowled.Format = DateTimePickerFormat.Custom;
            dateLastBowled.CustomFormat = @" ";

            datePaid.Format = DateTimePickerFormat.Custom;
            datePaid.CustomFormat = @" ";

            UpdateMemberInfo();
        }
        /// <summary>
        /// Finds "Member Number" in the database and populates the "Member Data" form.
        /// If that "Member Number" is not assigned then display error box.
        /// </summary>
        /// <param name="searchMem"></param>
        public void UpdateMemberInfo(Member searchMem = null)
        {
            _memberNum = Convert.ToInt32(txtMemberNumber.Text);
            if (searchMem == null)
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

                dateDOB.Format = DateTimePickerFormat.Custom;
                dateDOB.CustomFormat = @" ";

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

                foreach (var check in grpStatus.Controls.OfType<RadioButton>())
                {
                    check.Checked = false;
                }

                foreach (var check in grpGender.Controls.OfType<RadioButton>())
                {
                    check.Checked = false;
                }
                #endregion

                chbLifetime.Checked = false;
                datePaid.Format = DateTimePickerFormat.Custom;
                datePaid.CustomFormat = @" ";
            }
            else
            {
                var db = new NineTapDb();
                #region Personal Info
                _memberId = currentMem.Id;
                txtMemberNumber.Text = currentMem.Number.ToString();
                txtLastName.Text = currentMem.LastName;
                txtFirstName.Text = currentMem.FirstName;
                txtMiddleInitial.Text = currentMem.MiddleInitial;
                dateDOB.Value = currentMem.DateOfBirth;
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

                txtAverage.Text = currentMem.StartAvg.ToString();
                txtTournAvg.Text = LeagueAverage(currentMem).ToString();
                /********************************************************************************
                updates the form's handicap even when the finalize tournament button is clicked
                -also updates the currentMem's handicap, so when the tournamnent gets it, it is the right handicap
                *********************************************************************************/
                currentMem.Handicap = db.Members.First(x => x.Id == currentMem.Id).Handicap;
                txtHandicap.Text = currentMem.Handicap.ToString(); 
                /********************************************************************************/
                txtBonus.Text = currentMem.Bonus.ToString();
                
                #endregion

                #region Misc. Info
                //TODO: Pull datetime from database correctly

                dateJoined.Value = currentMem.JoinDate;
                if (currentMem.RejoinDate.HasValue)
                {
                    dateRejoin.Value = (DateTime)currentMem.RejoinDate;
                }
                else
                {
                    dateRejoin.Format = DateTimePickerFormat.Custom;
                    dateRejoin.CustomFormat = @" ";
                }
                if (currentMem.LastBowled.HasValue)
                {
                    dateLastBowled.Value = (DateTime)currentMem.LastBowled;
                }
                else
                {
                    dateLastBowled.Format = DateTimePickerFormat.Custom;
                    dateLastBowled.CustomFormat = @" ";
                }
                //txtMoneyEarned.Text = currentMem.MoneyEarned.ToString("C");
                
                var result = (from p in db.Participants
                              join g in db.Games on p.Game.Id equals g.Id
                              where p.Member.Id == currentMem.Id
                              select g.MoneyWon).ToArray();
                txtMoneyEarned.Text = String.Format("{0:C}", result.Sum());
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

                chbLifetime.Checked = currentMem.IsLifetimeMember;
                
                if (currentMem.LastPayment.HasValue)
                {
                    datePaid.Format = DateTimePickerFormat.Short;
                    datePaid.Value = (DateTime)currentMem.LastPayment;
                    checkPayment();
                }
                else
                {
                    datePaid.Format = DateTimePickerFormat.Custom;
                    datePaid.CustomFormat = @" ";
                    lblPaymentInfo.Visible = false;
                }
                
            }
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
            // check if Active radio button is checked
            if (!rdoActive.Checked && !rdoInActive.Checked)
            {
                MessageBox.Show("Member must be checked active or inactive.");
                return false;
            }
            // check if gender radio button is checked
            if (!rdoMale.Checked && !rdoFemale.Checked)
            {
                MessageBox.Show("A gender must be chosen.");
                return false;
            }
            //use better regex expression that includes spaces and hyphens
            if (!Regex.IsMatch(txtLastName.Text, "^[-a-zA-Z]+$"))
            {
                MessageBox.Show("Last Name is required.");
                txtLastName.Clear();
                return false;
            }

            if (!Regex.IsMatch(txtFirstName.Text, "^[a-zA-Z]+$"))
            {
                MessageBox.Show("First Name is required.");
                txtFirstName.Clear();
                return false;
            }

            if (dateDOB.Format == DateTimePickerFormat.Custom)
            {
                MessageBox.Show("DOB field cannot be blank.");
                return false;
            }
            if (!Regex.IsMatch(mtxtBoxSSN.Text, "   -  -"))
            {
                if (!Regex.IsMatch(mtxtBoxSSN.Text, "^\\d{3}-?\\d{2}-?\\d{4}$"))
                {
                    MessageBox.Show("Invalid Social Security field.");
                    mtxtBoxSSN.Clear();
                    return false;
                }
            }
            var db = new NineTapDb();
            var id = Convert.ToInt32(txtMemberNumber.Text);
            var ssnList = (from p in db.Members
                           where p.Number != id
                           select p.SSN                        
                           ).ToList();
            if (ssnList.Contains(mtxtBoxSSN.Text) && mtxtBoxSSN.Text != null)
            {
                MessageBox.Show("Member with same SSN already exists");
                mtxtBoxSSN.Clear();
                mtxtBoxSSN.Focus();
                return false;
                
            }
            if (string.IsNullOrWhiteSpace(txtAddress.Text.Trim()))
            {
                MessageBox.Show("Address field cannot be null.");
                txtAddress.Clear();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtCity.Text.Trim()))
            {
                MessageBox.Show("City field cannot be null");
                txtCity.Clear();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtState.Text.Trim()))
            {
                MessageBox.Show("State field cannot be blank.");
                txtState.Clear();
                return false;
            }

            if (!Regex.IsMatch(mtxtBoxZip.Text, "^\\d{5}(?:[-\\s]\\d{4})?$"))
            {
                MessageBox.Show("Invalid zip code field.");
                mtxtBoxZip.Clear();
                return false;
            }
            if (!Regex.IsMatch(mtxtBoxPhone.Text, "^(\\+\\d{1,2}\\s)?\\(?\\d{3}\\)?[\\s.-]\\d{3}[\\s.-]\\d{4}$"))
            {
                MessageBox.Show("Invalid Primary Phone field.");
                mtxtBoxPhone.Clear();
                return false;
            }

            // I think the block following this might make this redundant so I'm commenting it out to see what happens -- Cody
            /*
            if (string.IsNullOrWhiteSpace(txtEmail.Text.Trim()))
            {
                MessageBox.Show("Email field cannot be blank.");
                txtEmail.Clear();
                return false;
            }
            */
            // email validation
            // Author: Toby Fortuner
            if (!(new EmailAddressAttribute().IsValid(txtEmail.Text) || string.IsNullOrWhiteSpace(txtEmail.Text)))
            {
                MessageBox.Show("Email field must be a valid email address.");
                txtEmail.Clear();
                return false;
            }
            if(dateJoined.Value != null)
            {
                if(dateRejoin.Value != null && dateRejoin.Value < dateJoined.Value)
                {
                    MessageBox.Show("Rejoin Date before Join Date");
                    dateRejoin.Focus();
                    return false;
                }
                else if (dateRejoin.Value != null && dateRejoin.Value == dateJoined.Value)
                {
                    MessageBox.Show("Rejoin Date same as Join Date");
                    dateRejoin.Focus();
                    return false;
                }
            }
            /********************************************************************************************************
            League average should only be between 125 - 210
            *********************************************************************************************************/
            if (txtAverage.Text == "" || Convert.ToInt32(txtAverage.Text) < 125 || Convert.ToInt32(txtAverage.Text) > 210)
            {
                MessageBox.Show("For your League Average, you should only input between 125 to 210.");
                txtAverage.Focus();
                return false;
            }
            /*******************************************************************************************************/

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

            //if (isValid())
            //{
                var confirm = MessageBox.Show(@"Are You Sure?", @"Confirm Save", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (confirm == DialogResult.No)
                    return;

                //use existing memberId if present or select the member id from the form
                int memId = (_memberId != -1) ? _memberId : Convert.ToInt32(txtMemberNumber.Text);

                try
                {
                    Member temp = new Member
                    {
                        Id = memId,
                        Number = Convert.ToInt32(txtMemberNumber.Text),
                        IsActive = rdoActive.Checked,
                        JoinDate = dateJoined.Value,

                        #region Personal Info
                        LastName = txtLastName.Text,
                        FirstName = txtFirstName.Text,
                        MiddleInitial = txtMiddleInitial.Text,
                        DateOfBirth = dateDOB.Value,
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
                        /*************************************************************************************
                        used to say Average = 0; which is always making the average in the database 0
                        **************************************************************************************/
                        Average = (txtTournAvg.Text == string.Empty) ? 0 : Convert.ToInt16(txtTournAvg.Text),
                        /*************************************************************************************/
                        StartAvg = (txtAverage.Text == string.Empty) ? 0 : Convert.ToInt16(txtAverage.Text),
                        Handicap = (txtHandicap.Text == string.Empty) ? 0 : Convert.ToInt16(txtHandicap.Text),
                        Bonus = (txtBonus.Text == string.Empty) ? 0 : Convert.ToInt16(txtBonus.Text),
                        #endregion

                        #region Misc. Info
                        RejoinDate = (dateRejoin.Format == DateTimePickerFormat.Custom) ? (DateTime?)null : dateRejoin.Value,
                        LastBowled = (dateLastBowled.Format == DateTimePickerFormat.Custom) ? (DateTime?)null : dateLastBowled.Value,
                        ///*MoneyEarned = (txtMoneyEarned.Text == string.Empty) ? 0 : decimal.Parse(txtMoneyEarned.Text, NumberStyles.Currency)*/
                        //MoneyEarned = (txtMoneyEarned.Text == string.Empty) ? 0 : Convert.ToDecimal(txtMoneyEarned.Text),
                        
                        Notes = txtNotes.Text,
                        Referrals = (txtReferrals.Text) == string.Empty ? 0 : Convert.ToInt16(txtReferrals.Text),
                        #endregion
                        LastPayment = (datePaid.Format == DateTimePickerFormat.Custom) ? (DateTime?)null : datePaid.Value,
                        IsLifetimeMember = chbLifetime.Checked
                    };

                    // Adds Member to Database

                    try
                    {
                        MemberDb.AddMember(temp);


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
                catch (FormatException fe)
                {
                    Console.WriteLine("Error Number : " + fe.Message);
                    //TODO - this field is a catch all for errors in fields that require numbers 
                    //League Score, Handicap, and referrals
                    MessageBox.Show("Referrals must be an integer number value.");
                }
            //}
        }

        /// <summary>
        /// Displays the previous "Member Number"'s information when the left arrow button is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnArrowLeft_Click(object sender, EventArgs e)
        {
            
            if (((FrmMain)MdiParent)._membersList.Count() == 0 || currentMem.Number <= ((FrmMain)MdiParent)._membersList.First().Number)
            {
                MessageBox.Show(@"Beginning of file.", @"Notice");
            }
            else
            {
                txtMemberNumber.Text = (currentMem.Number - 1).ToString();
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
            if (((FrmMain)MdiParent)._membersList.Count() == 0 || currentMem.Number >= ((FrmMain)MdiParent)._membersList.Last().Number)
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
        /// <summary>
        /// Adds a new "Member Number".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnNew_Click(object sender, EventArgs e)
        {
            if (isValid())
            {
                Controls.Clear();
                InitializeComponent();
                dateRejoin.Format = DateTimePickerFormat.Custom;
                dateRejoin.CustomFormat = @" ";

                dateJoined.Format = DateTimePickerFormat.Custom;//new
                dateJoined.CustomFormat = @" ";//new

                dateLastBowled.Format = DateTimePickerFormat.Custom;
                dateLastBowled.CustomFormat = @" ";

                datePaid.Format = DateTimePickerFormat.Custom;
                datePaid.CustomFormat = @" ";

                dateDOB.Format = DateTimePickerFormat.Custom;
                dateDOB.CustomFormat = @" ";
                _memberId = -1;

                //get latest member number, or set to 1 if no members in database
                int nextMemberNumber = ((FrmMain)MdiParent)._membersList.Any() ? (((FrmMain)MdiParent)._membersList.Last().Number + 1) : 1;
                txtMemberNumber.Text = nextMemberNumber.ToString();
                currentMem = new Member
                {
                    Number = nextMemberNumber
                };
            }
            //on new player button select this focuses on the last name texbox that way user does not have
            //to use the mouse to reclick when adding a new player
            txtLastName.Focus();
        }

        /// <summary>
        /// Brings up the first "Member Number".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnFirstRecord_Click(object sender, EventArgs e)
        {
            txtMemberNumber.Text = ((FrmMain)MdiParent)._membersList.First().Number.ToString();
            UpdateMemberInfo();
        }

        /// <summary>
        /// Brings up the last "Member Number".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
        /// <summary>
        /// Brings up the datePicker.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ApplyCalendarForm(object sender, EventArgs e)
        {
            var datePicker = sender as DateTimePicker;

            if (datePicker != null)
            {
                datePicker.Format = DateTimePickerFormat.Short;
            }
        }
        /// <summary>
        /// Puts the calendar back to the default selection.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClearCalendar(object sender, KeyEventArgs e)
        {
            var datePicker = sender as DateTimePicker;

            if (datePicker == null || (e.KeyCode != Keys.Delete && e.KeyCode != Keys.Back)) return;

            datePicker.Format = DateTimePickerFormat.Custom;
            datePicker.CustomFormat = @" ";
        }
        /// <summary>
        /// clears all elements on member data form.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClear_Click(object sender, EventArgs e)
        {
            //removed code for a delete function it is in the region below
            #region
            //if (isValid())
            //{
            //    var confirm = MessageBox.Show(@"Are You Sure?", @"Confirm Save", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            //    if (confirm == DialogResult.No) return;
            //    try
            //    {
            //        MemberDb.DeleteMember(currentMem);

            //        MessageBox.Show(@"Bowler Removed Successfully.");
            //        ((FrmMain)MdiParent)._membersList = MemberDb.GetMemberList().OrderBy(m => m.Number);
            //        if (((FrmMain)MdiParent)._membersList.Count() > 0)
            //        {
            //            UpdateMemberInfo();
            //        }
            //    }
            //    catch (MemberTableException ex)
            //    {
            //        MessageBox.Show(ex.Message);
            //    }
            //}
            #endregion\
            //clears all elements on member data form
            var confirm = MessageBox.Show(@"Are You Sure?", @"Confirm Clear", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            
             if (confirm == DialogResult.No) return;
             /// stores member number to be restored later
            string tempMemNum = txtMemberNumber.Text;
            while (Controls.Count > 0)
            {
                Controls[0].Dispose();
            }
            InitializeComponent();
            //restores member number
            txtMemberNumber.Text = tempMemNum;
        }

        private void btnMemberSearch_Click(object sender, EventArgs e)
        {
            FrmSearch SearchForm = new FrmSearch();
            SearchForm.ShowDialog();

            if (SearchForm.searchResult > 0)
            {
                txtMemberNumber.Text = SearchForm.searchResult.ToString();
                UpdateMemberInfo();
            }
        }

        private void btnStats_Click(object sender, EventArgs e)
        {
            var newfrmStart = new FrmStats(Convert.ToInt32(txtMemberNumber.Text), (txtFirstName.Text + " " + txtLastName.Text), currentMem);
            newfrmStart.populateStats();
            newfrmStart.Show();
        }

        private void btnThisRecap_Click(object sender, EventArgs e)
        {
            if (isValid())
            {
                //Set up compenents for printing
                PrintDialog printDialog = new PrintDialog();
                PrintDocument printDocument = new PrintDocument();
                //add the document to the dialog box
                printDialog.Document = printDocument;
                //add the event handler that will do the printing
                printDocument.PrintPage += new PrintPageEventHandler(singlePrint);

                DialogResult result = printDialog.ShowDialog();

                if (result == DialogResult.OK)
                {
                    printDocument.Print();
                }
            }
        }

        public void singlePrint(object sender, PrintPageEventArgs e)
        {
            NineTapTour.Database.Print.SinglePrint(
                new MemberPrintObj(Convert.ToInt32(txtHandicap.Text), 
                    Convert.ToInt32(txtMemberNumber.Text), 
                    txtCity.Text,
                    txtFirstName.Text, 
                    txtLastName.Text
                    , txtAverage.Text
                    , Convert.ToInt32(txtBonus.Text)), 
                e);
        }

        private void chbLifetime_CheckedChanged(object sender, EventArgs e)
        {
            if (chbLifetime.Checked)
            {
                lblPaymentInfo.Visible = false;
                datePaid.Enabled = false;
            }
            else
            {
                datePaid.Enabled = true;
                checkPayment();
            }
        }

        private void datePaid_ValueChanged(object sender, EventArgs e)
        {
            datePaid.Format = DateTimePickerFormat.Short;
            checkPayment();
        }

        private void checkPayment()
        {
            /*******************************************************************************************************
            added '&& chbLifetime.Checked == false' so when the member is a lifetime member, the lblPaymentInfo will 
            not be visible even if their last payment was due before
            ********************************************************************************************************/
            if (datePaid.Value != null && datePaid.Value <= DateTime.Now.AddYears(-1) && chbLifetime.Checked == false)
            /*******************************************************************************************************/
            {
                lblPaymentInfo.Visible = true;
            }
            else
            {
                lblPaymentInfo.Visible = false;
            }
        }

        private void dateJoined_ValueChanged(object sender, EventArgs e)
        {
            dateJoined.Format = DateTimePickerFormat.Short;// Refreshes the date

        }

        private void dateRejoin_ValueChanged(object sender, EventArgs e)
        {
            dateRejoin.Format = DateTimePickerFormat.Short;//Refreshes the date
        }

        private void dateDOB_ValueChanged(object sender, EventArgs e)
        {
            dateDOB.Format = DateTimePickerFormat.Short;
        }

        private void btnRecapByDate_Click(object sender, EventArgs e)
        {
            new FrmPrintByDate().ShowDialog();
        }

        private void btnAllRecaps_Click(object sender, EventArgs e)
        {
            Print.printAllMembers();
        }

        private void btnLabels_Click(object sender, EventArgs e)
        {
            FrmLabelPrint labels = new FrmLabelPrint();
            labels.ShowDialog();
        }
        /// <summary>
        /// This action event assigns current form as currFrmMemberData in FrmMains' global 
        /// Variable property when leaving form. This allows the program to later check whether FrmMemberData has 
        /// been changed without saving.
        /// /// </summary>
        /// /// <param name="sender"></param>
        /// /// <param name="e"></param>
        private void FrmMemberData_Leave(object sender, EventArgs e)
        {
            ((FrmMain)MdiParent).currFrmMemberData = this;
        }

        /// <summary>
        /// checks whether form data has been changed and not saved
        /// </summary>
        /// <returns>true if frmData is saved and false if form data has been changed and not saved. </returns>
        public Boolean IsSavedData()
        {
            bool isMember = false;

            foreach (Member mem in ((FrmMain)MdiParent)._membersList)
            {
                if (mem.Id == (currentMem.Id))
                {
                    isMember = true;
                }
            }
            if(currentMem.State == null)
            {
                currentMem.State = "";
            }
            if (currentMem.City == null)
            {
                currentMem.City = "";
            }
            if (currentMem.Email == null)
            {
                currentMem.Email = "";
            }
            if (currentMem.Street == null)
            {
                currentMem.Street= "";
            }
            if (currentMem.Referrals == null)
            {
                txtReferrals.Text = null;
            }
            if (currentMem.PrimaryPhone == null)
            {
                currentMem.PrimaryPhone = "";
            }
            if (currentMem.PrimaryPhone == null)
            {
                currentMem.PrimaryPhone = "";
            }
            if (currentMem.SecondaryPhone == null)
            {
                currentMem.SecondaryPhone = "";
            }
            if (currentMem.SSN == null)
            {
                currentMem.SSN = " - -";
            }
            if (currentMem.PostalCode == null)
            {
                currentMem.PostalCode = "";
            }
            if (currentMem.Average == null)
            {
                txtAverage.Text = null ;
            }
            if (!isMember ||
                txtLastName.Text != currentMem.LastName.ToString() ||
                txtFirstName.Text != currentMem.FirstName.ToString() ||
                txtMiddleInitial.Text != currentMem.MiddleInitial.ToString() ||
                txtNotes.Text != currentMem.Notes.ToString() ||
                txtState.Text != currentMem.State.ToString() ||
                txtCity.Text != currentMem.City.ToString() ||
                txtEmail.Text != currentMem.Email.ToString() ||
                txtAddress.Text != currentMem.Street.ToString() ||
                txtReferrals.Text != currentMem.Referrals.ToString() ||
                mtxtBoxPhone.Text != currentMem.PrimaryPhone.ToString() ||
                mtxtBoxPhone2.Text != currentMem.SecondaryPhone.ToString() ||
                mtxtBoxSSN.Text.Trim() != currentMem.SSN.ToString().Trim() ||
                mtxtBoxZip.Text != currentMem.PostalCode.ToString() ||
                txtAverage.Text != currentMem.StartAvg.ToString() ||
                // checks radio buttons active Member
                (currentMem.IsActive == true && rdoActive.Checked == false) ||
                (currentMem.IsActive == false && rdoActive.Checked == true)
                )

            {
                return false;
            }

            else
            {
                return true;
            }
        }
        public double LeagueAverage (Member mem)
        {
            double sum = 0;
            double average = 0;
            var db = new NineTapDb();
            var temp = (
                        
                        from p in db.Participants
                        join m in db.Members on p.Member.Id equals m.Id
                        join g in db.Games on p.Game.Id equals g.Id
                        join t in db.Tournaments on p.Tournament.Id equals t.Id
                        where mem.Id == m.Id
                        orderby t.Date descending
                        select new
                        {
                            t.Date,
                            g.Game1,
                            g.Game2,
                            g.Game3,
                            g.Game4,
                            Average = (g.Game1 + g.Game2 + g.Game3 + g.Game4) / 4

                        }).Take(30).ToList();
            if(temp.Count > 0)
            {
                foreach (var item in temp)
                {
                    sum += Convert.ToDouble(item.Average);
                }
                return (average = sum / temp.Count());
            }
            return 0 ;
        }

  
    }
}

