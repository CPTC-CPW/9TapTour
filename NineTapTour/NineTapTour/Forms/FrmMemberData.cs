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
        }
        /// <summary>
        /// Updates information in the "Member Data" form.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MemberDataForm_Load(object sender, EventArgs e)
        {
            //_membersList = ((FrmMain)MdiParent)._membersList;
            dateRejoin.Format = DateTimePickerFormat.Custom;
            dateRejoin.CustomFormat = @" ";

            dateLastBowled.Format = DateTimePickerFormat.Custom;
            dateLastBowled.CustomFormat = @" ";

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

                foreach (var check in grpStatus.Controls.OfType<RadioButton>())
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
                mtxtBoxDOB.Text = currentMem.DateOfBirth.ToString("MM/dd/yyyy");
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

                chbLifetime.Checked = currentMem.IsLifetimeMember;
                if (!currentMem.IsLifetimeMember && currentMem.LastPayment.HasValue)
                {
                    datePaid.Value = (DateTime)currentMem.LastPayment;
                    checkPayment();
                }
                else
                {
                    datePaid.Value = DateTime.Now;
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
                MessageBox.Show("member must be checked active or inactive");
                return false;
            }
            // check if gender radio button is checked
            if (!rdoMale.Checked && !rdoFemale.Checked)
            {
                MessageBox.Show("a gender must be chosen");
                return false;
            }
            if (!Regex.IsMatch(txtFirstName.Text, "^[a-zA-Z]+$"))
            {
                MessageBox.Show("field cannot be blank");
                txtFirstName.Clear();
                return false;
            }
            //use better regex expression that includes spaces and hyphens
            if (!Regex.IsMatch(txtLastName.Text, "^[-a-zA-Z]+$"))
            {
                MessageBox.Show("field cannot be blank");
                txtLastName.Clear();
                return false;
            }
            if (!Regex.IsMatch(mtxtBoxZip.Text, "^\\d{5}(?:[-\\s]\\d{4})?$"))
            {
                MessageBox.Show("Invalid zip code field");
                mtxtBoxZip.Clear();
                return false;
            }
            if (!Regex.IsMatch(mtxtBoxPhone.Text, "^(\\+\\d{1,2}\\s)?\\(?\\d{3}\\)?[\\s.-]\\d{3}[\\s.-]\\d{4}$"))
            {
                MessageBox.Show("Invalid Primary Phone field");
                mtxtBoxPhone.Clear();
                return false;
            }
            if (!Regex.IsMatch(mtxtBoxSSN.Text, "^\\d{3}-?\\d{2}-?\\d{4}$"))
            {
                MessageBox.Show("Invalid Social Security field");
                mtxtBoxSSN.Clear();
                return false;
            }

            if (mtxtBoxSSN.Text == "")
            {
                MessageBox.Show(" DOB field cannot be null");
                mtxtBoxDOB.Clear();
                return false;
            }
            if (txtAddress.Text == "")
            {
                MessageBox.Show("Address field cannot be null");
                txtAddress.Clear();
                return false;
            }
            if (txtCity.Text == "")
            {
                MessageBox.Show(" City field cannot be null");
                txtCity.Clear();
                return false;
            }
            if (txtEmail.Text == "")
            {
                MessageBox.Show("email field cannot be null");
                txtEmail.Clear();
                return false;
            }
            // email validation
            // Author: Toby Fortuner
            if (!(new EmailAddressAttribute().IsValid(txtEmail.Text)))
            {
                MessageBox.Show("email field must be a valid email address");
                txtEmail.Clear();
                return false;
            }
            if (txtState.Text == "")
            {
                MessageBox.Show("state field cannot be null");
                txtState.Clear();
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

                //use existing memberId if present or select the member id from the form
                int memId = (_memberId != -1) ? _memberId : Convert.ToInt32(txtMemberNumber.Text);

                Member temp = new Member
                {
                    Id = memId,
                    Number = Convert.ToInt32(txtMemberNumber.Text),
                    IsActive = rdoActive.Checked,
                    JoinDate = DateTime.Now,

                    #region Personal Info
                    LastName = txtLastName.Text,
                    FirstName = txtFirstName.Text,
                    MiddleInitial = txtMiddleInitial.Text,
                    DateOfBirth = Convert.ToDateTime(mtxtBoxDOB.Text),
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
                    Referrals = txtReferrals.Text == string.Empty ? 0 : Convert.ToInt16(txtReferrals.Text),
                    #endregion
                    LastPayment = dateJoined.Value,
                    IsLifetimeMember = chbLifetime.Checked
                    
                };

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

                dateLastBowled.Format = DateTimePickerFormat.Custom;
                dateLastBowled.CustomFormat = @" ";
                _memberId = -1;

                //get latest member number, or set to 1 if no members in database
                int nextMemberNumber = ((FrmMain)MdiParent)._membersList.Any() ? (((FrmMain)MdiParent)._membersList.Last().Number + 1) : 1;
                txtMemberNumber.Text = nextMemberNumber.ToString();
                currentMem = new Member
                {
                    Number = nextMemberNumber
                };
            }
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
        /// Removes a bowler's information from the database.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (isValid())
            {
                var confirm = MessageBox.Show(@"Are You Sure?", @"Confirm Save", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (confirm == DialogResult.No) return;
                try
                {
                    MemberDb.DeleteMember(currentMem);

                    MessageBox.Show(@"Bowler Removed Successfully.");
                    ((FrmMain)MdiParent)._membersList = MemberDb.GetMemberList().OrderBy(m => m.Number);
                    if (((FrmMain)MdiParent)._membersList.Count() > 0)
                    {
                        UpdateMemberInfo();
                    }
                }
                catch (MemberTableException ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void btnMemberNumber_Click(object sender, EventArgs e)
        {
            var newfrmStart = new FrmSearch();
            Width = newfrmStart.Width;
            Height = newfrmStart.Height + 20;
            newfrmStart.Show();

        }

        private void btnStats_Click(object sender, EventArgs e)
        {
            var newfrmStart = new FrmStats(Convert.ToInt32(txtMemberNumber.Text), (txtFirstName.Text + " " + txtLastName.Text), currentMem);
            newfrmStart.populateStats();            
            newfrmStart.Show();
        }

        private void btnThisRecap_Click(object sender, EventArgs e)
        {
            //Set up compenents for printing
            PrintDialog printDialog = new PrintDialog();
            PrintDocument printDocument = new PrintDocument();
            //add the document to the dialog box
            printDialog.Document = printDocument;
            //add the event handler that will do the printing
            printDocument.PrintPage += new PrintPageEventHandler(singlePrint);

            DialogResult result = printDialog.ShowDialog();

            if(result == DialogResult.OK)
            {
                printDocument.Print();
            }
        }

        public void singlePrint(object sender, PrintPageEventArgs e)
        {
            //get the total handicap to display on the card when printed
            int totalHandicap = 0;
            if(txtHandicap.Text != "")
            {
                totalHandicap = Convert.ToInt32(txtHandicap.Text) * 4;
            }

            //This is what prints the data
            Graphics graphic = e.Graphics;

            //default font to use, should use a mono space font so the spaces line up.
            Font font = new Font("Arial", 16, FontStyle.Bold, GraphicsUnit.Pixel);

            //Sets defult brush to use when printing
            SolidBrush dBrush = new SolidBrush(Color.Black);

            int startX = 10;
            int startY = 50;
            //removed due to just adding an extra variable to positioning
            //int offSet = 40;

            //draw handicap and average
            graphic.DrawString(txtAverage.Text, font, dBrush, startX + 490, startY - 5);
            graphic.DrawString(txtHandicap.Text, font, dBrush, startX + 590, startY - 5);

            //draw the 4 handicaps for the game section of the card and the total handicap

            for(int i = 1; i <=5; i++)
            {
                //this prints the handicap 4 times.
                if(i <=4)
                {
                    graphic.DrawString(txtHandicap.Text, font, dBrush, startX + 530, startY + 30 + i * 40);
                }
                //this prints the total handicap after it prints the handicap 4 seperate times
                if(i == 5)
                {
                    graphic.DrawString(totalHandicap.ToString(), font, dBrush, startX + 530, startY + 50 + i * 40);
                }
            }
            //create name string containg lastname, firstname.
            string nameString = txtLastName.Text + ", " + txtFirstName.Text;
            //draw name string
            graphic.DrawString(nameString, font, dBrush, startX + 5, startY + 80);
            //draw city string
            graphic.DrawString(txtCity.Text, font, dBrush, startX + 5, startY + 122);
            //draw member number string
            graphic.DrawString(txtMemberNumber.Text, font, dBrush, startX + 80, startY + 215);

        }

        private void chbLifetime_CheckedChanged(object sender, EventArgs e)
        {
            if (chbLifetime.Checked)
            {
                lblPaymentInfo.Visible = false;
                datePaid.Enabled = false;
            } else
            {
                datePaid.Enabled = true;
                checkPayment();
            }
        }

        private void datePaid_ValueChanged(object sender, EventArgs e)
        {
            checkPayment();
        }
        private void checkPayment()
        {
            if (datePaid.Value != null && datePaid.Value <= DateTime.Now.AddYears(-1))
            {
                lblPaymentInfo.Visible = true;
            } else
            {
                lblPaymentInfo.Visible = false;
            }
        }
    }
}

