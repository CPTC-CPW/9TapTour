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
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Core;
using Excel = Microsoft.Office.Interop.Excel;
using Member_Import_Test.Classes;

namespace NineTapTour.Forms
{
    public partial class FrmMemberData : Form
    {

        //IOrderedEnumerable<Member> _membersList;
        int _memberId;
        Member currentMem;
        private int _memberNum;
        int RegionID;
        int AllGames;
        
        


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

            RegionID = ((FrmMain)MdiParent).RegionID;

            List<Member> ListOfMembers = MemberDb.GetMemberList(RegionID);

            updateOnload(ListOfMembers);

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
            RegionID = ((FrmMain)MdiParent).RegionID;
            List<Member> ListOfMembers = MemberDb.GetMemberList(RegionID);

            
           


            //set txtMemberNumber.Text back to one if there is no one in the the current selected region added yet
            if (MemberDb.GetMemberList(RegionID).Count == 0)
            {
                txtMemberNumber.Text = "1";
            }
            // if last region selected had more members then current selected region, set txtmemberNumber.Text to
            // its highest member count for the selcted region
            else if(Convert.ToInt16(txtMemberNumber.Text) > MemberDb.GetMemberList(RegionID).Count)
            {
                txtMemberNumber.Text = MemberDb.GetMemberList(RegionID).Count.ToString();
            }

            _memberNum = Convert.ToInt32(txtMemberNumber.Text);
            if (searchMem == null)
            {
                currentMem = MemberDb.GetMember(_memberNum,RegionID);
                List<PlayerHistory> last5 = PlayerHistoryDB.getLastFiveFromPlayerhistory(currentMem.Number, RegionID);
                if (last5.Count >= 1)
                {
                    txtAverage.Text = currentMem.StartAvg.ToString(); //whatever the bowler director decides his average to be is right. dont pull from the player hstory page
                    currentMem.StartAvg = Convert.ToInt16(txtAverage.Text);
                    txtTournAvg.Text = Convert.ToInt16(last5[0].trueAVG).ToString();
                    currentMem.Average = Convert.ToInt32(last5[0].trueAVG);

                    txtBonus.Text = last5[0].Bonus.ToString();
                    currentMem.Bonus = Convert.ToInt16(txtBonus.Text);
                }
                else
                {
                    txtAverage.Text = currentMem.StartAvg.ToString();
                    txtTournAvg.Text = 0.ToString();
                    txtBonus.Text = 0.ToString();
                    currentMem.Bonus = Convert.ToInt16(txtBonus.Text);
                }
            }
            else
            {
                currentMem = searchMem;
                _memberNum = currentMem.Number;
            }

            if (currentMem.Id == 0)
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

      
              
          

                /********************************************************************************
                updates the form's handicap even when the finalize tournament button is clicked
                -also updates the currentMem's handicap, so when the tournamnent gets it, it is the right handicap
                *********************************************************************************/
                try
                {
                    currentMem.Handicap = Calculations.Calculations.CalculateHandicapPins((currentMem.StartAvg.Value));
                }
                catch
                {
                    currentMem.Handicap = Calculations.Calculations.CalculateHandicapPins((0));
                }
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
                txtMoneyEarned.Text = currentMem.MoneyEarned.ToString("C");
                decimal moneySum = 0;
                var result = (from p in db.PlayerHistory
                              where p.MemberNumber == currentMem.Id
                              orderby p.TournamentDate descending
                              select new
                              {
                                  p.MoneyWon
                              }).ToArray();
                foreach(var v in result)
                {
                    moneySum += v.MoneyWon;
                }
               

                txtMoneyEarned.Text = String.Format("{0:C}", moneySum);
                currentMem.MoneyEarned = moneySum;
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

                moneySum = 0;
                db = new NineTapDb();
                result = (from p in db.PlayerHistory
                              where p.MemberNumber == currentMem.Number && p.regionID == RegionID
                              orderby p.TournamentDate descending
                              select new
                              {
                                  p.MoneyWon
                              }).ToArray();
                foreach (var v in result)
                {
                    moneySum += v.MoneyWon;
                }
                currentMem.MoneyEarned = moneySum;

                txtMoneyEarned.Text = currentMem.MoneyEarned.ToString("C");

                MemberDb.AddMember(currentMem);





                MemberDb.AddMember(currentMem);

     
                
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
            //// check if Active radio button is checked
            //if (!rdoActive.Checked && !rdoInActive.Checked)
            //{
            //    MessageBox.Show("Member must be checked active or inactive.");
            //    return false;
            //}
            //// check if gender radio button is checked
            //if (!rdoMale.Checked && !rdoFemale.Checked)
            //{
            //    MessageBox.Show("A gender must be chosen.");
            //    return false;
            //}
            ////use better regex expression that includes spaces and hyphens
            //if (!Regex.IsMatch(txtLastName.Text, "^[-a-zA-Z]+$"))
            //{
            //    MessageBox.Show("Last Name is required.");
            //    txtLastName.Clear();
            //    return false;
            //}

            //if (!Regex.IsMatch(txtFirstName.Text, "^[a-zA-Z]+$"))
            //{
            //    MessageBox.Show("First Name is required.");
            //    txtFirstName.Clear();
            //    return false;
            //}

            //if (dateDOB.Format == DateTimePickerFormat.Custom)
            //{
            //    MessageBox.Show("DOB field cannot be blank.");
            //    return false;
            //}
            //if (!Regex.IsMatch(mtxtBoxSSN.Text, "   -  -"))
            //{
            //    if (!Regex.IsMatch(mtxtBoxSSN.Text, "^\\d{3}-?\\d{2}-?\\d{4}$"))
            //    {
            //        MessageBox.Show("Invalid Social Security field.");
            //        mtxtBoxSSN.Clear();
            //        return false;
            //    }
            //}
            //var db = new NineTapDb();
            //var id = Convert.ToInt32(txtMemberNumber.Text);
            //var ssnList = (from p in db.Members
            //               where p.Number != id
            //               select p.SSN                        
            //               ).ToList();
            //if (ssnList.Contains(mtxtBoxSSN.Text) && mtxtBoxSSN.Text != null)
            //{
            //    MessageBox.Show("Member with same SSN already exists");
            //    mtxtBoxSSN.Clear();
            //    mtxtBoxSSN.Focus();
            //    return false;
                
            //}
            //if (string.IsNullOrWhiteSpace(txtAddress.Text.Trim()))
            //{
            //    MessageBox.Show("Address field cannot be null.");
            //    txtAddress.Clear();
            //    return false;
            //}

            //if (string.IsNullOrWhiteSpace(txtCity.Text.Trim()))
            //{
            //    MessageBox.Show("City field cannot be null");
            //    txtCity.Clear();
            //    return false;
            //}

            //if (string.IsNullOrWhiteSpace(txtState.Text.Trim()))
            //{
            //    MessageBox.Show("State field cannot be blank.");
            //    txtState.Clear();
            //    return false;
            //}

            //if (!Regex.IsMatch(mtxtBoxZip.Text, "^\\d{5}(?:[-\\s]\\d{4})?$"))
            //{
            //    MessageBox.Show("Invalid zip code field.");
            //    mtxtBoxZip.Clear();
            //    return false;
            //}
            //if (!Regex.IsMatch(mtxtBoxPhone.Text, "^(\\+\\d{1,2}\\s)?\\(?\\d{3}\\)?[\\s.-]\\d{3}[\\s.-]\\d{4}$"))
            //{
            //    MessageBox.Show("Invalid Primary Phone field.");
            //    mtxtBoxPhone.Clear();
            //    return false;
            //}

            //// I think the block following this might make this redundant so I'm commenting it out to see what happens -- Cody
            ///*
            //if (string.IsNullOrWhiteSpace(txtEmail.Text.Trim()))
            //{
            //    MessageBox.Show("Email field cannot be blank.");
            //    txtEmail.Clear();
            //    return false;
            //}
            //*/
            //// email validation
            //// Author: Toby Fortuner
            //if (!(new EmailAddressAttribute().IsValid(txtEmail.Text) || string.IsNullOrWhiteSpace(txtEmail.Text)))
            //{
            //    MessageBox.Show("Email field must be a valid email address.");
            //    txtEmail.Clear();
            //    return false;
            //}
            //if(dateJoined.Value != null)
            //{
            //    if(dateRejoin.Value != null && dateRejoin.Value < dateJoined.Value)
            //    {
            //        MessageBox.Show("Rejoin Date before Join Date");
            //        dateRejoin.Focus();
            //        return false;
            //    }
            //    else if (dateRejoin.Value != null && dateRejoin.Value == dateJoined.Value)
            //    {
            //        MessageBox.Show("Rejoin Date same as Join Date");
            //        dateRejoin.Focus();
            //        return false;
            //    }
            //}
            ///********************************************************************************************************
            //League average should only be between 125 - 210
            //*********************************************************************************************************/
            //if (txtAverage.Text == "" || Convert.ToInt32(txtAverage.Text) < 125 || Convert.ToInt32(txtAverage.Text) > 210)
            //{
            //    MessageBox.Show("For your League Average, you should only input between 125 to 210.");
            //    txtAverage.Focus();
            //    return false;
            //}
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

            ////use existing memberId if present or select the member id from the form
            //int memId = (_memberId != -1) ? _memberId : Convert.ToInt32(txtMemberNumber.Text);

            //checks to see if MemberID exists 
            int memId;
               Member temp = new Member();

          
            temp.Number = Convert.ToInt32(txtMemberNumber.Text);
            temp.IsActive = rdoActive.Checked;
            temp.JoinDate = dateJoined.Value;

            #region Personal Info
            temp.LastName = txtLastName.Text;
            temp.FirstName = txtFirstName.Text;
            temp.MiddleInitial = txtMiddleInitial.Text;
            temp.DateOfBirth = dateDOB.Value;
            temp.SSN = mtxtBoxSSN.Text;
            temp.IsSenior = chbSenior.Checked;
            temp.Gender = (rdoFemale.Checked) ? MemberGenders.Female : MemberGenders.Male;
            #endregion

            #region Postal Address
            temp.Street = txtAddress.Text;
            temp.City = txtCity.Text;
            temp.State = txtState.Text;
            temp.PostalCode = mtxtBoxZip.Text;
            #endregion

            #region Contact Info
            temp.Email = txtEmail.Text;
            temp.PrimaryPhone = mtxtBoxPhone.Text;
            temp.SecondaryPhone = mtxtBoxPhone2.Text;
            #endregion

            #region Score Info
            /*************************************************************************************
            used to say Average = 0; which is always making the average in the database 0
            **************************************************************************************/
            double avg = 0;
            try
            {
               avg = Convert.ToDouble(txtTournAvg.Text);
            }
            catch
            {
               
            }
            temp.Average = (txtTournAvg.Text == string.Empty) ? 0 : Convert.ToInt16(avg);
            /*************************************************************************************/
           
            temp.Handicap = Calculations.Calculations.CalculateHandicapPins((temp.Average.Value));


           
            #endregion

            #region Misc. Info
            temp.RejoinDate = (dateRejoin.Format == DateTimePickerFormat.Custom) ? (DateTime?)null : dateRejoin.Value;
            temp.LastBowled = (dateLastBowled.Format == DateTimePickerFormat.Custom) ? (DateTime?)null : dateLastBowled.Value;
            temp.MoneyEarned = currentMem.MoneyEarned;
            //MoneyEarned = (txtMoneyEarned.Text == string.Empty) ? 0 : Convert.ToDecimal(txtMoneyEarned.Text),

            temp.Notes = txtNotes.Text;
            temp.Referrals = (txtReferrals.Text) == string.Empty ? 0 : Convert.ToInt16(txtReferrals.Text);
            #endregion
            temp.LastPayment = (datePaid.Format == DateTimePickerFormat.Custom) ? (DateTime?)null : datePaid.Value;
            temp.IsLifetimeMember = chbLifetime.Checked;
            temp.NineTapRegionID = RegionID;

            //check to see if memberId exists before putting it in current selected regions database
            if(MemberDb.GetMember(temp.Number,RegionID).Id > 0)
            {
                memId = MemberDb.GetMember(temp.Number, RegionID).Id;
            }
            else
            {
                memId = MemberDb.GetALLMembersList().Count + 1;
            }
            temp.Id = memId;

            List<PlayerHistory> last5 = PlayerHistoryDB.getLastFiveFromPlayerhistory(currentMem.Number, RegionID);
            if (last5.Count >= 1)
            { // sets the average to that of their last adjusted average
                if (Convert.ToInt32(txtAverage.Text) == last5[0].AVG)
                {
                    txtAverage.Text = last5[0].AVG.ToString();
                    temp.StartAvg = last5[0].AVG;

                    txtTournAvg.Text = last5[0].trueAVG.ToString();
                    temp.Average = Convert.ToInt16(last5[0].trueAVG);


                    txtBonus.Text = last5[0].Bonus.ToString();
                    temp.Bonus = (txtBonus.Text == string.Empty) ? 0 : Convert.ToInt16(txtBonus.Text);
                }
                else //catches if director wants to change there average manually regardless of there player history
                {
                    temp.StartAvg = Convert.ToInt32(txtAverage.Text);
                    txtTournAvg.Text = last5[0].trueAVG.ToString();
                    temp.Average = Convert.ToInt16(last5[0].trueAVG);
                    txtBonus.Text = last5[0].Bonus.ToString();
                    temp.Bonus = (txtBonus.Text == string.Empty) ? 0 : Convert.ToInt16(txtBonus.Text);
                }
            }
            else if (txtAverage.Text == "")
            {
                txtAverage.Text = 0.ToString();
                txtTournAvg.Text = 0.ToString();
                temp.Average = 0;
                temp.StartAvg = 0;
                temp.Bonus = (txtBonus.Text == string.Empty) ? 0 : Convert.ToInt16(txtBonus.Text);
            }
            else
            {
                temp.StartAvg = Convert.ToInt16(txtAverage.Text);
                temp.Average = 0;
                txtAverage.Text = temp.StartAvg.ToString();
                txtTournAvg.Text = 0.ToString();
                temp.Bonus = (txtBonus.Text == string.Empty) ? 0 : Convert.ToInt16(txtBonus.Text);
            }


            // Adds Member to Database

            try
                    {
                        MemberDb.AddMember(temp);


                        //_membersList = MemberDb.GetMemberList().OrderBy(m => m.Number);
                        ((FrmMain)MdiParent)._membersList = MemberDb.GetMemberList(RegionID).OrderBy(m => m.Number);
                        //_membersList = ((FrmMain)MdiParent)._membersList;
                        UpdateMemberInfo();
                    }
                    catch (MemberTableException ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
                //catch (FormatException fe)
                //{
                //    Console.WriteLine("Error Number : " + fe.Message);
                //    //TODO - this field is a catch all for errors in fields that require numbers 
                //    //League Score, Handicap, and referrals
                //   // MessageBox.Show("Referrals must be an integer number value.");
                //}
            //}
        

        /// <summary>
        /// Displays the previous "Member Number"'s information when the left arrow button is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnArrowLeft_Click(object sender, EventArgs e)
        {
            List<Member> m = MemberDb.GetMemberList(RegionID);
            if (MemberDb.GetMemberList(RegionID).Count == 0 || currentMem.Number <= m[0].Number)
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
            if (MemberDb.GetMemberList(RegionID).Count == 0 || currentMem.Number >= MemberDb.GetMemberList(RegionID).Count)
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
                // int nextMemberNumber = ((FrmMain)MdiParent)._membersList.Any() ? (((FrmMain)MdiParent)._membersList.Last().Number + 1) : 1;
                int nextMemberNumber = MemberDb.GetMemberList(RegionID).Count + 1;
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
            txtMemberNumber.Text = MemberDb.GetMemberList(RegionID)[0].Number.ToString();
            UpdateMemberInfo();
        }

        /// <summary>
        /// Brings up the last "Member Number".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLastRecord_Click(object sender, EventArgs e)
        {
            txtMemberNumber.Text = MemberDb.GetMemberList(RegionID).Count.ToString();
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
            FrmSearch SearchForm = new FrmSearch(RegionID);
            SearchForm.ShowDialog();

            if (SearchForm.searchResult > 0)
            {
                txtMemberNumber.Text = SearchForm.searchResult.ToString();
                UpdateMemberInfo();
            }
        }

        private void btnStats_Click(object sender, EventArgs e)
        {
            List<PlayerHistory> nothing = new List<PlayerHistory>();
            FrmStats p = new FrmStats(currentMem.Number, currentMem.FirstName + currentMem.LastName + currentMem.MiddleInitial, currentMem, nothing, RegionID);
            p.ShowDialog();
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
            FrmLabelPrint labels = new FrmLabelPrint(RegionID);
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



        private void updateOnload(List<Member> temp)
        {
            foreach(var m in temp)
            {
                MemberDb.AddMember(m);
            }
        }



        /// <summary>
        /// checks whether form data has been changed and not saved
        /// </summary>
        /// <returns>true if frmData is saved and false if form data has been changed and not saved. </returns>
        public Boolean IsSavedData()
        {
            bool isMember = false;

            foreach (Member mem in MemberDb.GetALLMembersList())
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

        public double LeagueAvgFromPlayerHistory(Member mem)
        {
            double sum = 0;
            double avg = 0;
            var db = new NineTapDb();
            var temp = (from p in db.PlayerHistory
                        where p.MemberNumber == mem.Number
                        orderby p.TournamentDate descending, p.hisID descending
                        select new
                        {
                            p.TournamentDate,
                            p.Game1,
                            p.Game2,
                            p.Game3,
                            p.Game4,
                            p.AverageForGame,
                            p.trueAVG,
                        }).Take(30).ToList();
            if (temp.Count > 0)
            {
                foreach (var item in temp)
                {
                    sum += Convert.ToDouble(item.AverageForGame);
                }
                return (avg = sum / temp.Count());
            }
            return 0;
        }

      
        private void btnImportData_Click(object sender, EventArgs e)
        {
           
            List<ExcelRow> CurrentExcelData = new List<ExcelRow>();
            OpenFileDialog ofdOpen = new OpenFileDialog();
            ofdOpen.Filter = "Excel Files (*.xls)|*.xls";
            if(ofdOpen.ShowDialog() == DialogResult.OK)
            {

                List<ExcelRow> rows = new List<ExcelRow>();
               
                List<PlayerHistory> AlreadyImportedPH = PlayerHistoryDB.getMemberPlayerHistory(currentMem.Number, RegionID);
                bool wait = true;
                string fileName = ofdOpen.FileName;
                while (wait == true)
                {
                    frmPleaseWait please = new frmPleaseWait();
                    please.Show();
                    AllGames = PlayerHistoryDB.getNumberOfAllGames();
                    if (AlreadyImportedPH.Count > 0)
                    {
                        for(int delete = 0; delete < AlreadyImportedPH.Count; delete++)
                        {
                            Game game = FinalizeTempDB.getGame(AlreadyImportedPH[delete].GameID);
                            PlayerHistoryDB.DeleteGame(game);
                            PlayerHistoryDB.DeletePlayerHistory(AlreadyImportedPH[delete]);

                        }
                    }
            
                    rows = ProcessExcelFile(fileName);

                 

  
                    
                    wait = false;
                    please.Close();
                }
                foreach(var r in rows)
                {
                    CurrentExcelData.Add(r);
                }

                List<PlayerHistory> reset = PlayerHistoryDB.getLastFiveFromPlayerhistory(currentMem.Number, RegionID);
                currentMem.StartAvg = reset[0].AVG;
                currentMem.Average = Convert.ToInt32(reset[0].trueAVG);
                currentMem.Handicap = Calculations.Calculations.CalculateHandicapPins(Convert.ToInt32(currentMem.StartAvg));
                currentMem.Bonus = reset[0].Bonus;
               
                txtAverage.Text = currentMem.StartAvg.ToString();
                txtTournAvg.Text = currentMem.Average.ToString();
                txtHandicap.Text = currentMem.Handicap.ToString();
                txtBonus.Text = currentMem.Bonus.ToString();


              


                decimal moneySum = 0;
                var db = new NineTapDb();
                var result = (from p in db.PlayerHistory
                              where p.MemberNumber == currentMem.Number && p.regionID == RegionID
                              orderby p.TournamentDate descending
                              select new
                              {
                                  p.MoneyWon
                              }).ToArray();
                foreach (var v in result)
                {
                    moneySum += v.MoneyWon;
                }
                currentMem.MoneyEarned = moneySum;

                txtMoneyEarned.Text = currentMem.MoneyEarned.ToString("C");

                MemberDb.AddMember(currentMem);






            }
        }


        private List<ExcelRow> ProcessExcelFile(string PathAndFileName)
        {
         
            List<ExcelRow> returnMe = new List<ExcelRow>();
           
        

            Excel.Application xlApp = new Excel.Application();
            Excel.Workbook xlWorkBook = xlApp.Workbooks.Open(PathAndFileName, 0, true, 5, "", "", true, Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
            Excel.Worksheet xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);
            Excel.Range range = xlWorkSheet.UsedRange;

            string[] PlayerFinalFirstAndMiddle = { "", "" };
            string[] PlayersFinalLastAndMiddle = { "", "" };


            string playerFullName = Convert.ToString((range.Cells[1, 2] as Excel.Range).Value2);
            string playerLastName = playerFullName.Substring(0, playerFullName.IndexOf(","));


            string firstAndMiddle = playerFullName.Substring(playerFullName.IndexOf(",") + 2);
            string[] first0middle1 = firstAndMiddle.Split(' ');

            if (playerFullName.Contains("Sr")) //catches the "sr " and sets their last name to only there last name
            {
                try
                {
                    playerLastName = playerFullName.Substring(0, playerFullName.IndexOf(" "));
                }
                catch
                {
                    PlayerFinalFirstAndMiddle[0] = first0middle1[0];
                }
            }
            else
            {
                playerLastName = playerFullName.Substring(0, playerFullName.IndexOf(","));
            }

            int playerOrgAVG;
            for (int i = 0; i < first0middle1.Length; i++)
            {
                PlayerFinalFirstAndMiddle[i] = first0middle1[0];
            }
            try
            {
                playerOrgAVG = Convert.ToInt32((range.Cells[1, 10] as Excel.Range).Value2);
            }
            catch (Exception NotAValidNumber)
            {
                playerOrgAVG = -1;
            }

            String playerNumber = (range.Cells[1, 14] as Excel.Range).Value2;
            String[] playerNumberAfterSplit;
            int playerNumberAsInt = 0;
            int.TryParse(playerNumber, out playerNumberAsInt);
            if (playerNumberAsInt != 0)
            {
                playerNumberAsInt = Convert.ToInt32((range.Cells[1, 14] as Excel.Range).Value2);
            }
            else if (playerNumberAsInt == 0) // if player has more then one member number, set it to their latest
            {
                playerNumberAfterSplit = playerNumber.Split('/');
                playerNumberAsInt = Convert.ToInt32(playerNumberAfterSplit[playerNumberAfterSplit.Length - 1]);
            }


            for (int sheetNum = 1; sheetNum <= xlWorkBook.Worksheets.Count; sheetNum++)
            {

                xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(sheetNum);
                range = xlWorkSheet.UsedRange;
                for (int row = 3; row <= range.Rows.Count; row++)
                {
                    try
                    {
                        if (Convert.ToInt32((range.Cells[row, 3] as Excel.Range).Value2) == 0
                       && Convert.ToInt32((range.Cells[row, 4] as Excel.Range).Value2) == 0
                       && Convert.ToInt32((range.Cells[row, 5] as Excel.Range).Value2) == 0
                       && Convert.ToInt32((range.Cells[row, 6] as Excel.Range).Value2) == 0)
                        {
                            continue;
                        }
                    }
                    catch (Exception ex)
                    {
                        continue;
                    }
                    ExcelRow temp = new ExcelRow();
                    PlayerHistory playerH = new PlayerHistory();
                    Game GameHistory = new Game();
                    GameHistory.gameRegionID = RegionID;
                    temp.PlayerFirstName = PlayerFinalFirstAndMiddle[0];
                    temp.PlayerMiddleName = PlayerFinalFirstAndMiddle[1];
                    temp.PlayerLastName = playerLastName;
                    temp.PlayerOrginalAVG = playerOrgAVG;
                    temp.PlayerNumber = currentMem.Number;
                    playerH.MemberNumber = currentMem.Number;
                    playerH.regionID = RegionID;
                    if (currentMem.Number == temp.PlayerNumber)
                    {//only process file if they have been added as a member first 
                        try
                        {
                            temp.GameTotal = Convert.ToInt32((range.Cells[row, 1] as Excel.Range).Value2);
                            playerH.GamesPlayed = Convert.ToInt32((range.Cells[row, 1] as Excel.Range).Value2);
                            DateTime compare = DateTime.FromOADate(Convert.ToDouble((range.Cells[row, 2] as Excel.Range).Value2));
                            if (temp.GameTotal > 4 && compare == Convert.ToDateTime("12/30/1899 12:00:00 AM"))
                            {
                                break;
                            }
                        }
                        catch
                        {
                            temp.GameTotal = -1;
                        }
                        try
                        {
                            temp.Date = DateTime.FromOADate(Convert.ToDouble((range.Cells[row, 2] as Excel.Range).Value2));
                            playerH.TournamentDate = temp.Date;
                        }
                        catch
                        {
                            temp.Date = new DateTime();
                        }
                        try
                        {
                            temp.Game1 = Convert.ToInt32((range.Cells[row, 3] as Excel.Range).Value2);
                            GameHistory.Game1 = temp.Game1;
                            playerH.Game1 = temp.Game1;
                        }
                        catch
                        {
                            temp.Game1 = -1;
                        }
                        try
                        {
                            temp.Game2 = Convert.ToInt32((range.Cells[row, 4] as Excel.Range).Value2);
                            GameHistory.Game2 = temp.Game2;
                            playerH.Game2 = temp.Game2;
                        }
                        catch
                        {
                            temp.Game2 = -1;
                        }
                        try
                        {
                            temp.Game3 = Convert.ToInt32((range.Cells[row, 5] as Excel.Range).Value2);
                            GameHistory.Game3 = temp.Game3;
                            playerH.Game3 = temp.Game3;
                        }
                        catch
                        {
                            temp.Game3 = -1;
                        }
                        try
                        {
                            temp.Game4 = Convert.ToInt32((range.Cells[row, 6] as Excel.Range).Value2);
                            GameHistory.Game4 = temp.Game4;
                            playerH.Game4 = temp.Game4;
                        }
                        catch
                        {
                            temp.Game4 = -1;
                        }
                        try
                        {
                            temp.Total = Convert.ToInt32((range.Cells[row, 7] as Excel.Range).Value2);
                            GameHistory.TotalScore = temp.Total;
                            playerH.TotalScore = temp.Total;
                        }
                        catch
                        {
                            temp.Total = -1;
                        }
                        try
                        {
                            temp.AverageOfRow = Convert.ToDouble((range.Cells[row, 8] as Excel.Range).Value2);
                            playerH.AverageForGame = temp.AverageOfRow;
                        }
                        catch
                        {
                            temp.AverageOfRow = -1;
                        }
                        try
                        {
                            temp.TrueAverage = Convert.ToDouble((range.Cells[row, 9] as Excel.Range).Value2);
                            playerH.trueAVG = temp.TrueAverage;
                        }
                        catch
                        {
                            temp.TrueAverage = -1;
                        }
                        try
                        {
                            temp.AVG = Convert.ToInt32((range.Cells[row, 10] as Excel.Range).Value2);
                            playerH.AVG = temp.AVG;

                        }
                        catch
                        {
                            temp.AVG = -1;
                        }
                        try
                        {
                            temp.HandyCap = Convert.ToInt32((range.Cells[row, 11] as Excel.Range).Value2);
                            GameHistory.Handicap = temp.HandyCap;
                            playerH.HandiCap = temp.HandyCap;
                        }
                        catch
                        {
                            temp.Bonus = -1;
                        }
                        try
                        {
                            temp.Bonus = Convert.ToInt32((range.Cells[row, 12] as Excel.Range).Value2);
                            GameHistory.Bonus = temp.Bonus;
                            playerH.Bonus = temp.Bonus;
                        }
                        catch
                        {
                            temp.HandyCap = -1000;
                        }
                        temp.PotPro = Convert.ToString((range.Cells[row, 13] as Excel.Range).Value2);
                        playerH.ProPot = temp.PotPro;
                        temp.FinPPHG = Convert.ToString((range.Cells[row, 14] as Excel.Range).Value2);
                        playerH.PPHG = temp.FinPPHG;
                       

                        try
                        {
                            temp.Cash = Convert.ToDouble((range.Cells[row, 15] as Excel.Range).Value2);
                            GameHistory.MoneyWon = Convert.ToDecimal(temp.Cash);
                            playerH.MoneyWon = Convert.ToDecimal(temp.Cash);
                        }
                        catch
                        {
                            temp.Cash = 0;
                        }
                        temp.Notes = Convert.ToString((range.Cells[row, 16] as Excel.Range).Value2);
                        GameHistory.Notes = temp.Notes;
                        playerH.Notes = temp.Notes;
                        playerH.PPHG = temp.FinPPHG;
                        GameHistory.Id = AllGames + 1;
                        AllGames++;
                        playerH.GameID = GameHistory.Id;
                        PlayerHistoryDB.AddGame(GameHistory);
                        PlayerHistoryDB.AddPlayerHistory(playerH);
                        returnMe.Add(temp);
                   
                    }

                }

            }
        


        xlWorkBook.Close(0);
            xlApp.Quit();




            Marshal.ReleaseComObject(range);
            Marshal.ReleaseComObject(xlWorkSheet);
            Marshal.ReleaseComObject(xlWorkBook);
            Marshal.ReleaseComObject(xlApp);

            System.Diagnostics.Process[] process = System.Diagnostics.Process.GetProcessesByName("Excel");
            foreach (System.Diagnostics.Process p in process)
            {

                try
                {
                    p.Kill();
                }
                catch { }
            }

            return returnMe;
        }
        
    

        private void chbSocial_CheckedChanged(object sender, EventArgs e)
        {
          mtxtBoxSSN.PasswordChar = chbSocial.Checked ? '\0' : '*';
        }
    }
}



