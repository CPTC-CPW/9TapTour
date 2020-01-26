using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using NineTapTour.Database;
using NineTapTour.Exceptions;
using System.Drawing.Printing;
using System.Data;
using System.Runtime.InteropServices;
using NineTapTour.Models;
using Excel = Microsoft.Office.Interop.Excel;
using System.Text.RegularExpressions;

namespace NineTapTour.Forms
{
    public partial class FrmMemberData : Form
    {
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
            this.WindowState = FormWindowState.Maximized;

            //finds all Controls and change BackColor of each control color when 
            //the control is on focus
            foreach (Control ctrl in this.Controls)
            {
                ChangeBackColorOnFocus(ctrl);                
            }

            RegionID = ((FrmMain)MdiParent).RegionID;
            List<Member> ListOfMembers = MemberDB.GetMemberList(RegionID);
            UpdateMemberInfo();
        }
        
        /// <summary>
        /// finds all Controls and change BackColor of each control color when the control is on 
        /// focus and checks if that control has a child and changes the child contol color onFocus
        /// and changes back to origin back color when LostFocus
        /// </summary>
        /// <param name="ctrl"></param>
        private void ChangeBackColorOnFocus(Control ctrl)
        {
            ctrl.GotFocus += Ctrl_GotFocus;
            ctrl.LostFocus += Ctrl_LostFocus;
            if (ctrl.HasChildren)
            {
                foreach (Control childCtrl in ctrl.Controls)
                {
                    ChangeBackColorOnFocus(childCtrl);
                }
            }
        }

        /// <summary>
        /// The controller back color is set back to origin color set in the properties
        /// when LostFocus method is called
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Ctrl_LostFocus(object sender, EventArgs e)
        {
            var ctrl = sender as Control;
            if (ctrl.Tag is Color)
                ctrl.BackColor = (Color)ctrl.Tag;
        }

        /// <summary>
        /// Method to change controller BackColor when GotFocus method is called
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Ctrl_GotFocus(object sender, EventArgs e)
        {
            var ctrl = sender as Control;
            ctrl.Tag = ctrl.BackColor;            
            ctrl.BackColor = Color.Yellow;
        }

        public void RemovePlaceholderText(object sender, EventArgs e)
        {
            if (txtDOB.Text == "MM/DD/YYYY")
            {
                txtDOB.Text = "";
            }
        }

        public void AddPlaceholderText(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtDOB.Text))
            {
                txtDOB.Text = "MM/DD/YYYY";
            }
        }

        /// <summary>
        /// Finds "Member Number" in the database and populates the "Member Data" form.
        /// If that "Member Number" is not assigned then display error box.
        /// </summary>
        public void UpdateMemberInfo()
        {
            RegionID = ((FrmMain)MdiParent).RegionID;
            Member searchMem = null;

            lblLastNameValidation.Visible = false;
            lblFirstNameValidation.Visible = false;
            lblAverageValidation.Visible = false;
            lblDateJoinedValidation.Visible = false;

            //set all member info group control background colors
            foreach (Control c in grpMemberInfo.Controls)
            {
                c.BackColor = Color.White;
            }

			foreach(Control d in panel5.Controls)
			{
				d.BackColor = Color.LightGray;
			}

            int memberCount = MemberDB.GetMemberListCount(RegionID);

            // set txtMemberNumber.Text back to one if there is no one in the the 
            // current selected region added yet
            if (memberCount == 0)
            {
                txtMemberNumber.Text = "1";
            }
            // if last region selected had more members then current selected 
            // region, set txtmemberNumber.Text to its highest member count for the selcted region
            else if(Convert.ToInt16(txtMemberNumber.Text) > memberCount)
            {
                txtMemberNumber.Text = memberCount.ToString();
            }

            _memberNum = Convert.ToInt32(txtMemberNumber.Text);

            if (searchMem == null)
            {
                currentMem = MemberDB.GetMember(_memberNum,RegionID);
                List<PlayerHistory> last5 = PlayerHistoryDB.GetLastFiveTournaments(currentMem.Number, RegionID);
                if (last5.Count >= 1)
                {   //whatever the bowler director decides his average to be is right. 
                    // dont pull from the player hstory page
                    txtAverage.Text = currentMem.StartAvg.ToString(); 
                    currentMem.StartAvg = Convert.ToInt16(txtAverage.Text);
                    txt30GameAvg.Text = Convert.ToInt16(last5[0].trueAVG).ToString();
                    currentMem.Average = Convert.ToInt32(last5[0].trueAVG);
                    txtBonus.Text = currentMem.Bonus.ToString();
                }
                else
                {
                    txtAverage.Text = currentMem.StartAvg.ToString();
                    txt30GameAvg.Text = 0.ToString();
                    txtBonus.Text = currentMem.Bonus.ToString();
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
                
                txtMemberNumber.Text = _memberNum.ToString();
                _memberId = -1;

                // Personal Info
                txtMemberNumber.Text = currentMem.Number.ToString();
                txtLastName.Text = "";
                txtFirstName.Text = "";
                txtMiddleInitial.Text = "";
                txtDOB.Text = "MM/DD/YYYY";
                txtSSN.Text = "";

                // Postal Address
                txtAddress.Text = "";
                txtCity.Text = "";
                txtState.Text = "";
                txtZip.Text = "";

                // Contact Info
                txtEmail.Text = "";
                txtPhone.Text = "";
                txtPhone2.Text = "";

                // Score Info
                txtAverage.Text = "";
                txtHandicap.Text = "";
                txtBonus.Text = "";

                // Misc. Info
                txtDateJoined.Text = "";
                txtDateJoined.Text = "";
                txtMoneyEarned.Text = "";
                txtNotes.Text = "";
                txtReferrals.Text = "";

        
                foreach (var check in grpStatus.Controls.OfType<RadioButton>())
                {
                    check.Checked = false;
                }

                foreach (var check in grpGender.Controls.OfType<RadioButton>())
                {
                    check.Checked = false;
                }

                chbLifetime.Checked = false;
                txtLastPayment.Text = "";
                txtPaidTo.Text = "";
            }
            else
            {
                var db = new NineTapDb();

                // Personal Info
                _memberId = currentMem.Id;
                txtMemberNumber.Text = currentMem.Number.ToString();
                txtLastName.Text = currentMem.LastName;
                txtFirstName.Text = currentMem.FirstName;
                txtMiddleInitial.Text = currentMem.MiddleInitial;

                if(currentMem.DateOfBirth != null)
                {
                    txtDOB.Text = currentMem.DateOfBirth.Value.ToString("MM/dd/yyyy");
                }

                txtSSN.Text = currentMem.SSN;

                // Postal Address
                txtAddress.Text = currentMem.Street;
                txtCity.Text = currentMem.City;
                txtState.Text = currentMem.State;
                txtZip.Text = currentMem.PostalCode;

                // Contact Info
                txtEmail.Text = currentMem.Email;
                txtPhone.Text = currentMem.PrimaryPhone;
                txtPhone2.Text = currentMem.SecondaryPhone;

                // Score Info         
                /********************************************************************************
                updates the form's handicap even when the finalize tournament button is clicked
                -also updates the currentMem's handicap, so when the tournnament gets it, it is the right handicap
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

                // Misc. Info
                //TODO: Pull datetime from database correctly 
                if (currentMem.DateOfBirth.HasValue)
                {
                    txtDOB.Text = currentMem.DateOfBirth.Value.ToString("MM/dd/yyyy");
                }
                else
                {
                    txtDOB.Text = "MM/DD/YYYY";
                }

                if (currentMem.JoinDate.HasValue)
                {
                    txtDateJoined.Text = currentMem.JoinDate.Value.ToString("MM/dd/yyyy");
                }
                else
                {
                    txtDateJoined.Text = "";
                }

                if (currentMem.RejoinDate.HasValue)
                {
                    txtRejoinDate.Text = currentMem.RejoinDate.Value.ToString("MM/dd/yyyy");
                }
                else
                {
                    txtRejoinDate.Text = "";
                }

                if (currentMem.LastBowled.HasValue)
                {
                    txtLastBowled.Text = currentMem.LastBowled.Value.ToString("MM/dd/yyyy");
                }
                else
                {
                    txtLastBowled.Text = "";
                }

                txtMoneyEarned.Text = currentMem.MoneyEarned.ToString("C");
                txtNotes.Text = currentMem.Notes;

                if (currentMem.Referrals == null)
                {
                    txtReferrals.Text = "0";
                }
                else
                {
                    txtReferrals.Text = currentMem.Referrals.ToString();
                }

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

                chbLifetime.Checked = currentMem.IsLifetimeMember;

                if (currentMem.LastPayment.HasValue)
                {
                    txtLastPayment.Text = 
                        currentMem.LastPayment.Value.ToString("MM/dd/yyyy");
                    txtPaidTo.Text =
                        currentMem.LastPayment.Value.AddYears(1).ToString("yyyy");

                    checkPayment();
                }
                else
                {
                    txtLastPayment.Text = "";
                    txtPaidTo.Text = "";
                    lblPaymentInfo.Visible = false;
                }                

                currentMem.MoneyEarned = 
                    PlayerHistoryDB.GetTotalMoneyWon(currentMem.Number, RegionID);

                txtMoneyEarned.Text = currentMem.MoneyEarned.ToString("C");
            }
        }           

        /// <summary>
		/// This method creates validation for the input boxes on the Member Info page
        /// by highlighting required fields.
		/// </summary>
		/// <returns></returns>
        public bool IsValidTextboxes()
        {
			bool valid = true;

            // validate average textbox for being between 1-300
            if (!FormHelper.IsAverageValid(txtAverage.Text))
            {
                lblAverageValidation.Visible = true;
                txtAverage.Clear();
                //txtAverage.BackColor = Color.LightPink;
                valid = false;
            }

            // validate lastname textbox
            if (String.IsNullOrWhiteSpace(txtLastName.Text))
            {
                lblLastNameValidation.Visible = true;
                txtLastName.Clear();
                //txtLastName.BackColor = Color.LightPink;
                valid = false;
            }

            // validate firstname textbox
            if (String.IsNullOrWhiteSpace(txtFirstName.Text))
            {
                lblFirstNameValidation.Visible = true;
                txtFirstName.Clear();
                //txtFirstName.BackColor = Color.LightPink;
                valid = false;
            }

			// validate dateJoined textbox
            if (!FormHelper.IsDateTimeValid(txtDateJoined.Text))
            {
                lblDateJoinedValidation.Visible = true;
                txtDateJoined.BackColor = Color.LightPink;
                valid = false;
            }

            // validate DOB textbox
            if (!FormHelper.IsDateTimeValid(txtDOB.Text))
                {
                lblDOBValidation.Visible = true;
                //txtDOB.BackColor = Color.LightPink;
                valid = false;
            }

            return valid;
        }

        /// <summary>
        /// Saves the information entered in the "Member Data" form.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveMemberData();
        }

        public void SaveMemberData()
        {      
            // checks validation then runs the rest of the 
            // btnSave_Click and adds a member into the database.
            if (IsValidTextboxes())
            {
                //create temporary member for validation
                Member temp = new Member();
                temp.Number = Convert.ToInt32(txtMemberNumber.Text);
                temp.IsActive = rdoActive.Checked;
                temp.JoinDate = DateTime.Parse(txtDateJoined.Text);

                // Personal Info
                temp.LastName = txtLastName.Text;
                temp.FirstName = txtFirstName.Text;
                temp.MiddleInitial = txtMiddleInitial.Text;                              
                temp.DateOfBirth = DateTime.Parse(txtDOB.Text);
                temp.SSN = txtSSN.Text;
                temp.Gender = (rdoFemale.Checked) ? MemberGenders.Female : MemberGenders.Male;

                //if member was born more than 50 years ago, then member is a senior. If member is a senior, check the isSenior checkbox and set temp.IsSenior to true
                DateTime senior = DateTime.Now.AddYears(-50);
                if (senior >= temp.DateOfBirth)
                {
                    chbSenior.Checked = true;
                }
                else
                {
                    chbSenior.Checked = false;
                }
                temp.IsSenior = chbSenior.Checked;

            

                // Postal Address
                temp.Street = txtAddress.Text;
                temp.City = txtCity.Text;
                temp.State = txtState.Text;
                temp.PostalCode = txtZip.Text;

                // Contact Info
                temp.Email = txtEmail.Text;
                temp.PrimaryPhone = txtPhone.Text;
                temp.SecondaryPhone = txtPhone2.Text;

                // Score Info
                /****************************************************************************
                / This used to say Average = 0; which will make the average in the database 0.
                / This code block assigns txt30GameAvg.Text to temp.Average.
                *****************************************************************************/
                if (Int32.TryParse(txt30GameAvg.Text, out int thirtyGameAverage))
                {
                    temp.Average = thirtyGameAverage;
                }
                else
                {
                    temp.Average = 0;
                }

                /****************************************************************************/
                temp.Handicap = 
                    Calculations.Calculations.CalculateHandicapPins(temp.Average.Value);
                
                // Misc. Info
                if (!String.IsNullOrWhiteSpace(txtRejoinDate.Text))
                {
                    DateTime date;
                    if (DateTime.TryParse(txtRejoinDate.Text, out date))
                    {
                        temp.RejoinDate = date;
                    }
                }
                else
                {
                    temp.RejoinDate = null;
                }

                if (!String.IsNullOrWhiteSpace(txtLastBowled.Text))
                {
                    DateTime date;
                    if (DateTime.TryParse(txtLastBowled.Text, out date))
                    {
                        temp.LastBowled = date;
                    }
                }
                else
                {
                    temp.LastBowled = null;
                }

                temp.MoneyEarned = currentMem.MoneyEarned;
                temp.Notes = txtNotes.Text;

                temp.Referrals = (txtReferrals.Text) == string.Empty ? 0 : 
                    Convert.ToInt16(txtReferrals.Text);

                if (!String.IsNullOrWhiteSpace(txtLastPayment.Text))
                {
                    DateTime date;
                    if (DateTime.TryParse(txtLastPayment.Text, out date))
                    {
                        temp.LastPayment = date;
                    }
                }
                else
                {
                    temp.LastPayment = null;
                }

                temp.IsLifetimeMember = chbLifetime.Checked;
                temp.NineTapRegionID = RegionID;

                // check to see if memberId exists before putting it in 
                // current selected regions database
                int memId;
                if (MemberDB.MemberExists(temp))
                {
                    memId = MemberDB.GetMemberIdByNumber(temp.Number, RegionID);
                }
                else
                {
                    memId = MemberDB.GetMemberListCount(RegionID) + 1;
                }

                temp.Id = memId;

                //Set average for the new member
                List<PlayerHistory> last5 = PlayerHistoryDB.GetLastFiveTournaments(currentMem.Number, RegionID);
                if (last5.Count >= 1)
                {   // sets the average to that of their last adjusted average
                    if (Convert.ToInt32(txtAverage.Text) == last5[0].AVG)
                    {
                        txtAverage.Text = last5[0].AVG.ToString();
                        temp.StartAvg = last5[0].AVG;

                        txt30GameAvg.Text = last5[0].trueAVG.ToString();
                        temp.Average = Convert.ToInt16(last5[0].trueAVG);

                        temp.Bonus = (txtBonus.Text == string.Empty) ? 0 : 
                            Convert.ToInt16(txtBonus.Text);
                    }
                    else
                    {   // catches if director wants to change their average 
                        // manually regardless of there player history
                        temp.StartAvg = Convert.ToInt32(txtAverage.Text);
                        txt30GameAvg.Text = last5[0].trueAVG.ToString();
                        temp.Average = Convert.ToInt16(last5[0].trueAVG);
                        temp.Bonus = (txtBonus.Text == string.Empty) ? 0 : 
                            Convert.ToInt16(txtBonus.Text);
                    }
                }
                else if (txtAverage.Text == "")
                {
                    txtAverage.Text = 0.ToString();
                    txt30GameAvg.Text = 0.ToString();
                    temp.Average = 0;
                    temp.StartAvg = 0;
                    temp.Bonus = (txtBonus.Text == string.Empty) ? 0 : 
                        Convert.ToInt16(txtBonus.Text);
                }
                else
                {
                    temp.StartAvg = Convert.ToInt16(txtAverage.Text);
                    temp.Average = 0;
                    txtAverage.Text = temp.StartAvg.ToString();
                    txt30GameAvg.Text = 0.ToString();
                    temp.Bonus = (txtBonus.Text == string.Empty) ? 0 : 
                        Convert.ToInt16(txtBonus.Text);
                }

                // Adds Member to Database
                try
                {
                    int tempBonusPins = Convert.ToInt16(txtBonus.Text);
                    if (tempBonusPins <= 5)
                    {
                        // Left blank because this is simply making sure it is going to import 
                        // correct data into [dbo].[Members]
                        temp.Bonus = tempBonusPins;
                    }
                    else
                    {
                        throw new InvalidMemberImportationException("Maximum allowed bonus pins is 5");
                    }
                    MemberDB.AddOrUpdateMember(temp);
#if DEBUG
                    MessageBox.Show("Member saved");
#endif
                    ((FrmMain)MdiParent)._membersList =
                        MemberDB.GetMemberList(RegionID).OrderBy(m => m.Number);
                    UpdateMemberInfo();
                }
                catch (MemberTableException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                catch (InvalidMemberImportationException exMember)
                {
                    MessageBox.Show(exMember.Message, "Uh-Oh!",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// Displays the previous "Member Number"'s information when 
        /// the left arrow button is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnArrowLeft_Click(object sender, EventArgs e)
        {
            //cursor begins when arrow is clicked
            Cursor.Current = Cursors.WaitCursor;
            List<Member> m = MemberDB.GetMemberList(RegionID);
            if (m.Count == 0 || currentMem.Number <= m[0].Number)
            {
                //turns loading cursor off.
                Cursor.Current = Cursors.Default;
                MessageBox.Show(@"Beginning of file.", @"Notice");
            }
            else
            {
                txtMemberNumber.Text = (currentMem.Number - 1).ToString();
                UpdateMemberInfo();

                //turns loading cursor off
                Cursor.Current = Cursors.Default;
            }
        }

        /// <summary>
        /// Displays the next "Member Number"'s information when the right 
        /// arrow button is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRightArrow_Click(object sender, EventArgs e)
        {
            //turns on a loading cursor while new bowler is loaded.
            Cursor.Current = Cursors.WaitCursor;
            int memberCount = MemberDB.GetMemberListCount(RegionID);
            if (memberCount == 0 ||
                currentMem.Number >= memberCount)
            {
                //turns loading cursor off.
                Cursor.Current = Cursors.Default;
                MessageBox.Show(@"End of file.", @"Notice");
            }
            else
            {
                txtMemberNumber.Text = (currentMem.Number + 1).ToString();
                UpdateMemberInfo();
                //turns loading cursor off.
                Cursor.Current = Cursors.Default;
            }
        }

        //After the InitializeComponent(); call, the dateRejoin 
        // Format & dateLastBowled are reused.
        /// <summary>
        /// Adds a new "Member Number".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnNew_Click(object sender, EventArgs e)
        {
            SaveMemberData();
            Controls.Clear();
            InitializeComponent();

            //finds all Controls and change BackColor of each control color when 
            //the control is on focus
            foreach (Control ctrl in this.Controls)
            {
                ChangeBackColorOnFocus(ctrl);
            }

            txtRejoinDate.Text = "";
            txtRejoinDate.Mask = "00/00/0000";
            txtDateJoined.Text = DateTime.Now.ToString("MM/dd/yyyy");
            txtDateJoined.Mask = "00/00/0000";
            txtLastBowled.Text = DateTime.Now.ToString("MM/dd/yyyy");
            txtLastBowled.Mask = "00/00/0000";
            txtLastPayment.Text = "";
            txtLastPayment.Mask = "00/00/0000";
            txtDOB.Text = "MM/DD/YYYY";
            _memberId = -1;

            //removes placeholder text when DOB textBox is clicked
            txtDOB.GotFocus += RemovePlaceholderText;
            //adds placeholder text when DOB textBox is clicked away from with a date
            txtDOB.LostFocus += AddPlaceholderText;

            //get latest member number, or set to 1 if no members in database
            int nextMemberNumber = MemberDB.GetMemberListCount(RegionID) + 1;
            txtMemberNumber.Text = nextMemberNumber.ToString();

            currentMem = new Member
            {
                Number = nextMemberNumber
            };

            // on new player button select this focuses on the last name texbox 
            // that way user does not have to use the mouse to reclick when 
            // adding a new player
            txtLastName.Focus();
        }

        /// <summary>
        /// Brings up the first "Member Number".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnFirstRecord_Click(object sender, EventArgs e)
        {
            try
            {
                txtMemberNumber.Text = MemberDB.GetMemberList(RegionID)[0].Number.ToString();
                UpdateMemberInfo();
            }
            catch
            {
                MessageBox.Show("There are no Members yet");
            }
        }

        /// <summary>
        /// Brings up the last "Member Number".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLastRecord_Click(object sender, EventArgs e)
        {
            txtMemberNumber.Text = MemberDB.GetMemberListCount(RegionID).ToString();
            UpdateMemberInfo();
        }
        

        /// <summary>
        /// Opens SearchForm to search members. If member is found, updates Member Form to display that member's info
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        // takes a list of no player history, this list would stack on 
        // top of thew original data on the form finalize page
        private void btnStats_Click(object sender, EventArgs e)
        {
            FrmStats p = new FrmStats(currentMem.Number, currentMem.FirstName + 
                currentMem.LastName + currentMem.MiddleInitial, currentMem, RegionID);
            p.ShowDialog();
        }

        /// <summary>
        /// Prints Average, Handicap and Bonus of a single member
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnThisRecap_Click(object sender, EventArgs e)
        {
            if (IsValidTextboxes())
            {
                //Set up components for printing
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

        /// <summary>
        /// Will print all members that are marked as active in the database
        /// </summary>
        private void btnPrintActive_Click(object sender, EventArgs e)
        {
            Print.printByActiveMembers(TournamentDB.GetAllActiveMembers());
        }

        /// <summary>
        /// Gets Member data (Name, Member Number, City, Average, Handicap and Bonus) of a single member to print
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void singlePrint(object sender, PrintPageEventArgs e) 
        {
            NineTapTour.Database.Print.SinglePrint(
                new MemberPrintObj(Convert.ToInt32(txtHandicap.Text), 
                    Convert.ToInt32(txtMemberNumber.Text), 
                    txtCity.Text,
                    txtFirstName.Text, 
                    txtLastName.Text, 
                    txtAverage.Text, 
                    Convert.ToInt32(txtBonus.Text)), 
                    e);
        }

        /// <summary>
        /// Checks if lifetime member checkbox is checked. If it is, the "year membership will end" fields are hidden. If it's not, show "year membership will end" fields, and check is payment was made more than a year ago
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chbLifetime_CheckedChanged(object sender, EventArgs e)
        {
            if (chbLifetime.Checked)
            {
                lblPaymentInfo.Visible = false;
                txtLastPayment.Enabled = false;
                txtPaidTo.Visible = false;
                lblPaidTo.Visible = false;
            }
            else
            {
                txtLastPayment.Enabled = true;
                txtPaidTo.Visible = true;
                lblPaidTo.Visible = true;
                checkPayment();
            }
        }

        /// <summary>
        /// If "year membership will end" field is changed, check if payment was made more than a year ago
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void datePaid_ValueChanged(object sender, EventArgs e)
        {
            txtLastPayment.Text = "";
            txtPaidTo.Text = "";
            checkPayment();
        }

        /// <summary>
        /// Checks if last payment was made more than a year ago. If it was, show warning label that payment is due. 
        /// </summary>
        private void checkPayment()
        {
            /********************************************************************************
            added '&& chbLifetime.Checked == false' so when the member is a lifetime member, 
            the lblPaymentInfo will not be visible even if their last payment was due before
            *********************************************************************************/
            lblPaymentInfo.Visible = true;
            if (chbLifetime.Checked || (DateTime.TryParse(txtLastPayment.Text, 
                out DateTime lastPayment) && lastPayment >= DateTime.Now.AddYears(-1)))
            {
                lblPaymentInfo.Visible = false;
            }
        }
            
        /// <summary>
        /// This action event assigns current form as currFrmMemberData in
        /// FrmMains' global Variable property when leaving form. This allows 
        /// the program to later check whether FrmMemberData has 
        /// been changed without saving.
        /// /// </summary>
        /// /// <param name="sender"></param>
        /// /// <param name="e"></param>
        private void FrmMemberData_Leave(object sender, EventArgs e)
        {
            ((FrmMain)MdiParent).currFrmMemberData = this;
        }

        /// <summary>
        /// This button imports member data from excel file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnImportData_Click(object sender, EventArgs e)
        {
            List<ExcelRow> CurrentExcelData = new List<ExcelRow>();
            OpenFileDialog ofdOpen = new OpenFileDialog();
            ofdOpen.Filter = "Excel Files (*.xls)|*.xls";

            if(ofdOpen.ShowDialog() == DialogResult.OK)
            {
                List<ExcelRow> rows = new List<ExcelRow>();

                List<PlayerHistory> AlreadyImportedPH = 
                    PlayerHistoryDB.GetMemberPlayerHistory(currentMem.Number, RegionID);

                bool wait = true;
                string fileName = ofdOpen.FileName;

                while (wait == true)
                {
                    frmPleaseWait please = new frmPleaseWait();
                    please.Show();
                    AllGames = PlayerHistoryDB.GetNumberOfAllGames();

                    if (AlreadyImportedPH.Count > 0)
                    {
                        for(int delete = 0; delete < AlreadyImportedPH.Count; delete++)
                        {
                            Game game = GameDB.GetGame(AlreadyImportedPH[delete].GameID);
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

                List<PlayerHistory> reset = PlayerHistoryDB.GetLastFiveTournaments(currentMem.Number, RegionID);
                currentMem.StartAvg = reset[0].AVG;
                currentMem.Average = Convert.ToInt32(reset[0].trueAVG);
                currentMem.Handicap = Calculations.Calculations
                    .CalculateHandicapPins(Convert.ToInt32(currentMem.StartAvg));

                currentMem.Bonus = reset[0].Bonus;
                txtAverage.Text = currentMem.StartAvg.ToString();
                txt30GameAvg.Text = currentMem.Average.ToString();
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

                currentMem.MoneyEarned += moneySum; 

                txtMoneyEarned.Text = currentMem.MoneyEarned.ToString("C");

                MemberDB.AddOrUpdateMember(currentMem);
            }
        }

        /// <summary>
        /// Processes excel file for member data import
        /// </summary>
        /// <param name="PathAndFileName"></param>
        /// <returns></returns>
        private List<ExcelRow> ProcessExcelFile(string PathAndFileName)
        {
            List<ExcelRow> returnMe = new List<ExcelRow>();        
            Excel.Application xlApp = new Excel.Application();

            Excel.Workbook xlWorkBook = xlApp.Workbooks.Open(PathAndFileName, 0, true, 5, "", "", 
                true, Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);

            Excel.Worksheet xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);
            Excel.Range range = xlWorkSheet.UsedRange;

            string[] PlayerFinalFirstAndMiddle = { "", "" };
            string[] PlayersFinalLastAndMiddle = { "", "" };
            string playerLastName = "";
            string firstAndMiddle = "";
            string playerFullName = Convert.ToString((range.Cells[1, 2] as Excel.Range).Value2);
            if (playerFullName.Contains(","))
            {
                playerLastName = playerFullName.Substring(0, playerFullName.IndexOf(","));
                firstAndMiddle = playerFullName.Substring(playerFullName.IndexOf(",") + 2);
            }
            // Checks to see if a period instead of a comma was accidentally placed in member name. (Rob's Request)
            else if (playerFullName.Contains("."))
            {
                playerLastName = playerFullName.Substring(0, playerFullName.IndexOf("."));
                firstAndMiddle = playerFullName.Substring(playerFullName.IndexOf(".") + 2);
            }
                        
            string[] first0middle1 = firstAndMiddle.Split(' ');
            int playerOrgAVG;

            for (int i = 0; i < first0middle1.Length; i++)
            {
                PlayerFinalFirstAndMiddle[i] = first0middle1[0];
            }

            if ( Int32.TryParse( ( ( range.Cells[1, 10] as Excel.Range ).Value2 ), out int result ) )
            {
                playerOrgAVG = result;
            }
            else
            {
                playerOrgAVG = -1;
            }

            String playerNumber = (range.Cells[1, 14] as Excel.Range).Value2;
            bool isRegionHawaii = (cbHaw.Checked); // checks to see if Region is Hawaii
            
            if(isRegionHawaii) 
            {
                playerNumber = Regex.Replace(playerNumber, "[^0-9]", "");  // strip the member number to straight number
            }
            String[] playerNumberAfterSplit;
            int playerNumberAsInt = 0;
            int.TryParse(playerNumber, out playerNumberAsInt);

            // hawaii numbers are not 234 they have H  or H- in front need to address that by removing the h 
            // used regex to remove any non numeric expressions from player number be it a letter or a - 
            if (playerNumberAsInt != 0)
            {
                playerNumberAsInt = Convert.ToInt32(Regex.Replace(playerNumber, "[^0-9]", "")); 
            }
            else if (playerNumberAsInt == 0) // if player has more then one member number, set it to their latest
            {
                playerNumberAfterSplit = playerNumber.Split('/');
                playerNumberAsInt = Convert.ToInt32( Regex.Replace(playerNumberAfterSplit[playerNumberAfterSplit.Length - 1] , "[^0-9]", ""));
            }

            for (int sheetNum = 1; sheetNum <= xlWorkBook.Worksheets.Count; sheetNum++)
            {
                xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(sheetNum);
                range = xlWorkSheet.UsedRange;
                double noGameMoneyWon = 0;
                int rowNum;
                
                if(isRegionHawaii)
                {
                    rowNum = 4;
                }
                else
                {
                    rowNum = 3;
                }

                for (int row = rowNum; row <= range.Rows.Count; row++)
                {

                    ExcelRow temp = new ExcelRow();
                    PlayerHistory playerH = new PlayerHistory();
                    Game GameHistory = new Game();

                    string game1 = Convert.ToString((range.Cells[row, 3] as Excel.Range).Value2);
                    string game2 = Convert.ToString((range.Cells[row, 4] as Excel.Range).Value2);
                    string game3 = Convert.ToString((range.Cells[row, 5] as Excel.Range).Value2);
                    string game4 = Convert.ToString((range.Cells[row, 6] as Excel.Range).Value2);
                    string testFin = Convert.ToString((range.Cells[row, 14] as Excel.Range).Value2);

                    // handles when legacy excel files have 0 in the games total column
                    if (!string.IsNullOrWhiteSpace(Convert.ToString((range.Cells[row, 1] as Excel.Range).Value2)))
                    {
                        if (Convert.ToInt32((range.Cells[row, 1] as Excel.Range).Value2) == 0)
                        {
                            continue;
                        }
                    }
                    
                    if ( // if no date or cash then continue to the next line
                        string.IsNullOrWhiteSpace(Convert.ToString((range.Cells[row, 2] as Excel.Range).Value2)) &&
                        string.IsNullOrWhiteSpace(Convert.ToString((range.Cells[row, 15] as Excel.Range).Value2))
                        )
                    {
                        continue;
                    }

                    if( // if the four games have no data AKA no games bowled and there is a finish place then add the cash to moneywon
                        string.IsNullOrWhiteSpace(game1) &&
                        string.IsNullOrWhiteSpace(game2) &&
                        string.IsNullOrWhiteSpace(game3) &&
                        string.IsNullOrWhiteSpace(game4) &&
                        !string.IsNullOrWhiteSpace(testFin)
                    )
                    {
                        noGameMoneyWon += Convert.ToDouble((range.Cells[row, 15] as Excel.Range).Value2);
                        continue;
                    }
               

                    GameHistory.GameRegionID = RegionID;
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
                            if (compare == Convert.ToDateTime("12/30/1899 12:00:00 AM"))
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
                            playerH.AverageForEntry = temp.AverageOfRow;
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
                            //THIS WILL CATCH SUBTOTALS THAT MAY HAVE BEEN ADDED ON LINE 46 OF THE EXCEL FILES
                            //only grab the money earned from tournament if they placed in tournament
                            if (temp.FinPPHG.ToString() != "") 
                            {
                                temp.Cash = Convert.ToDouble((range.Cells[row, 15] as Excel.Range).Value2);
                                GameHistory.MoneyWon = Convert.ToDecimal(temp.Cash);
                                playerH.MoneyWon = Convert.ToDecimal(temp.Cash);
                            }
                            else
                            {
                                temp.Cash = 0;
                                GameHistory.MoneyWon = 0;
                                playerH.MoneyWon = 0;
                            }
                        }
                        catch
                        {
                            temp.Cash = 0;
                        }
                        playerH.MoneyWon += Convert.ToDecimal(noGameMoneyWon);

                        temp.Notes = Convert.ToString((range.Cells[row, 16] as Excel.Range).Value2);
                        GameHistory.Notes = temp.Notes;
                        playerH.Notes = temp.Notes;
                        playerH.PPHG = temp.FinPPHG;
                        GameHistory.Id = AllGames + 1;
                        AllGames++;
                        playerH.GameID = GameHistory.Id;

                        GameDB.AddOrUpdateGame(GameHistory);
                        PlayerHistoryDB.AddPlayerHistory(playerH);
                        returnMe.Add(temp);
                        noGameMoneyWon = 0;
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
                catch (Exception e)
                {
                    MessageBox.Show(e.ToString());
                }
            }
            return returnMe;
        }

        /// <summary>
        /// If SSN checkbox is checked, the social security number is shown, if not the social secuity number is masked with '*'
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chbSocial_CheckedChanged(object sender, EventArgs e)
        {
            txtSSN.PasswordChar = chbSocial.Checked ? '\0' : '*';
        }

        
        /// <summary>
        /// The resize event for the form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmMemberData_Resize(object sender, EventArgs e)
        {
            FormHelper.SetFlowDirection(this, flpMemberData, 1080, 730);
        }

        /// <summary>
        /// After the size of the form has been changed, it checks the pixel
        /// width and height to determine whether there needs to be scroll bars
        /// or not.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void flpMemberScores_SizeChanged(object sender, EventArgs e)
        {
            FormHelper.SetFlowControlScrollBars(this, flpMemberData, 1080, 600);
        }

        private void mtxtBox_Click(object sender, EventArgs e)
        {
            FormHelper.GoToFirstIndexInTextboxIfEmpty(sender as TextBoxBase);
        }

        /// <summary>
        /// The Bonus Pins textbox is allowed to be changed in the
        /// members form
        /// </summary>
        /// <return>a new value of bonus pins</return>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtBonus_TextChanged(object sender, EventArgs e)
        {
            if (Int32.TryParse(txtBonus.Text, out int newBonusPins))
            {
                txtBonus.Text = Convert.ToInt32(newBonusPins).ToString();
            }
        }

        /// <summary>
        /// Checks IsSenior checkbox if any key is pressed (helps with data entry when tabbing through form)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chbSenior_KeyDown(object sender, KeyEventArgs e)
        {
            CheckBox currentCheckBox = sender as CheckBox;
            currentCheckBox.Checked = true;
        }

        /// <summary>
        /// Checks female radio button if any key is pressed (helps with checking radio buttons when tabbing through form)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rdoFemale_KeyDown(object sender, KeyEventArgs e)
        {
            RadioButton currentRadioButton = sender as RadioButton;
            currentRadioButton.Checked = true;
        }

        private void txtDOB_TextChanged(object sender, EventArgs e)
        {
            lblDOBValidation.Visible = false;                     
        }

        private void txtAverage_TextChanged(object sender, EventArgs e)
        {
            lblAverageValidation.Visible = false;    
        }

        private void txtLastName_TextChanged(object sender, EventArgs e)
        {
            lblLastNameValidation.Visible = false;          
        }

        private void txtFirstName_TextChanged(object sender, EventArgs e)
        {
            lblFirstNameValidation.Visible = false;
        }
    }
}