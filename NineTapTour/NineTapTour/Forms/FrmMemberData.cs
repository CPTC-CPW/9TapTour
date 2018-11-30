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
            //finds all Controls and change BackColor of each control color when 
            //the control is on focus
            foreach (Control ctrl in this.Controls)
            {
                ChangeBackColorOnFocus(ctrl);                
            }

            RegionID = ((FrmMain)MdiParent).RegionID;
            List<Member> ListOfMembers = MemberDb.GetMemberList(RegionID);
            toolTip1.IsBalloon = true;
            txtDateJoined.MaskInputRejected += new MaskInputRejectedEventHandler(DateMaskTextBoxInput_MaskInputRejected);
            txtDateJoined.KeyDown += new KeyEventHandler(mtxtBoxDOB_KeyDown);
            txtRejoinDate.MaskInputRejected += new MaskInputRejectedEventHandler(DateMaskTextBoxInput_MaskInputRejected);
            txtRejoinDate.KeyDown += new KeyEventHandler(mtxtBoxRejoinDate_KeyDown);
            txtLastBowled.MaskInputRejected += new MaskInputRejectedEventHandler(DateMaskTextBoxInput_MaskInputRejected);
            txtLastBowled.KeyDown += new KeyEventHandler(mtxtBoxLastBowled_KeyDown);
            txtLastPayment.MaskInputRejected += new MaskInputRejectedEventHandler(DateMaskTextBoxInput_MaskInputRejected);
            txtLastPayment.KeyDown += new KeyEventHandler(MtxtBoxLastPayment_KeyDown);
            txtDOB.MaskInputRejected += new MaskInputRejectedEventHandler(DateMaskTextBoxInput_MaskInputRejected);
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

        /// <summary>
        /// Finds "Member Number" in the database and populates the "Member Data" form.
        /// If that "Member Number" is not assigned then display error box.
        /// </summary>
        /// <param name="searchMem"></param>
        public void UpdateMemberInfo(Member searchMem = null)
        {
            RemoveValidation();
            RegionID = ((FrmMain)MdiParent).RegionID;  
            
            //set all member info group control background colors
            foreach(Control c in grpMemberInfo.Controls)
            {
                c.BackColor = Color.White;
            }

			foreach(Control d in panel5.Controls)
			{
				d.BackColor = Color.LightGray;
			}

            int memberCount = MemberDb.GetMemberListCount(RegionID);

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
                currentMem = MemberDb.GetMember(_memberNum,RegionID);
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
                txtDOB.Text = "";
                txtDOB.KeyDown += new KeyEventHandler(mtxtBoxDOB_KeyDown);
                toolTip1.IsBalloon = true;
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
                txtDateJoined.KeyDown += new KeyEventHandler(mtxtBoxDateJoined_KeyDown);
                toolTip1.IsBalloon = true;
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

                // Misc. Info
                //TODO: Pull datetime from database correctly 
                if (currentMem.DateOfBirth.HasValue)
                {
                    txtDOB.Text = currentMem.DateOfBirth.Value.ToString("MM/dd/yyyy");
                }
                else
                {
                    txtDOB.Text = "";
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
                    checkPayment();
                }
                else
                {
                    txtLastPayment.Text = "";
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
                txtAverage.BackColor = Color.LightPink;
                valid = false;
            }

            // validate lastname textbox
            if (String.IsNullOrWhiteSpace(txtLastName.Text))
            {
                lblLastNameValidation.Visible = true;
                txtLastName.Clear();
                txtLastName.BackColor = Color.LightPink;
                valid = false;
            }

            // validate firstname textbox
            if (String.IsNullOrWhiteSpace(txtFirstName.Text))
            {
                lblFirstNameValidation.Visible = true;
                txtFirstName.Clear();
                txtFirstName.BackColor = Color.LightPink;
                valid = false;
            }

			// validate dateJoined textbox
            if (!FormHelper.IsDateTimeValid(txtDateJoined.Text))
            {
                lblDateJoinedValidation.Visible = true;
                txtDateJoined.BackColor = Color.LightPink;
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
                RemoveValidation();

                //checks to see if MemberID exists 
                int memId;
                Member temp = new Member();
                temp.Number = Convert.ToInt32(txtMemberNumber.Text);
                temp.IsActive = rdoActive.Checked;

                if (!String.IsNullOrWhiteSpace(txtDateJoined.Text))
                {
                    DateTime date;
                    if(DateTime.TryParse(txtDateJoined.Text, out date))
                    {
                        temp.JoinDate = date;
                    }
                }
                else
                {
                    temp.JoinDate = null;
                }

                // Personal Info
                temp.LastName = txtLastName.Text;
                temp.FirstName = txtFirstName.Text;
                temp.MiddleInitial = txtMiddleInitial.Text;

                if (!String.IsNullOrWhiteSpace(txtDOB.Text))
                {
                    DateTime date;
                    if (DateTime.TryParse(txtDOB.Text, out date))
                    {
                        temp.DateOfBirth = date;
                    }
                }
                else
                {
                    temp.DateOfBirth = null;
                }

                DateTime senior = DateTime.Now.AddYears(-50);

                if (senior >= temp.DateOfBirth)
                {
                    temp.IsSenior = true;
                    chbSenior.Checked = true;
                }
                else
                {
                    temp.IsSenior = false;
                    chbSenior.Checked = false;
                }

                temp.SSN = txtSSN.Text;
                temp.IsSenior = chbSenior.Checked;
                temp.Gender = (rdoFemale.Checked) ? MemberGenders.Female : MemberGenders.Male;

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
                if(MemberDb.MemberExists(temp))
                {
                    memId = MemberDb.GetMemberIdByNumber(temp.Number, RegionID, new NineTapDb());
                }
                else
                {
                    memId = MemberDb.GetMemberListCount(RegionID) + 1;
                }

                temp.Id = memId;

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
                    MemberDb.AddMember(temp);
#if DEBUG
                    MessageBox.Show("Member saved");
#endif
                    ((FrmMain)MdiParent)._membersList = 
                        MemberDb.GetMemberList(RegionID).OrderBy(m => m.Number);
                    UpdateMemberInfo();
                }
                catch (MemberTableException ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void RemoveValidation()
        {
            Label[] validationLabels =
            {
                lblLastNameValidation,
                lblFirstNameValidation,
                lblDOBValidation,
                lblSSNValidation,
                lblDateJoinedValidation,
                lblStateValidation,
                lblReferralsValidation,
                lblAverageValidation
            };

            for (int i = 0; i < validationLabels.Length; i++)
            {
                validationLabels[i].Visible = false;
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
            List<Member> m = MemberDb.GetMemberList(RegionID);
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
            int memberCount = MemberDb.GetMemberListCount(RegionID);
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
            txtDOB.Text = "";
            txtDOB.Mask = "00/00/0000";
            _memberId = -1;

            //get latest member number, or set to 1 if no members in database
            int nextMemberNumber = MemberDb.GetMemberListCount(RegionID) + 1;
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
                txtMemberNumber.Text = MemberDb.GetMemberList(RegionID)[0].Number.ToString();
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
            txtMemberNumber.Text = MemberDb.GetMemberListCount(RegionID).ToString();
            UpdateMemberInfo();
        }
        
        /// <summary>
        /// Turns textbox pink when text is erased
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InputRequired(object sender, EventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null)
            {
                textBox.BackColor = textBox.Text == string.Empty ? Color.LightPink : Color.White;
            }
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

        // takes a list of no player history, this list would stack on 
        // top of thew orginal data on the form finalize page
        private void btnStats_Click(object sender, EventArgs e)
        {
            FrmStats p = new FrmStats(currentMem.Number, currentMem.FirstName + 
                currentMem.LastName + currentMem.MiddleInitial, currentMem, RegionID);
            p.ShowDialog();
        }

        private void btnThisRecap_Click(object sender, EventArgs e)
        {
            if (IsValidTextboxes())
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
                    txtLastName.Text, 
                    txtAverage.Text, 
                    Convert.ToInt32(txtBonus.Text)), 
                    e);
        }

        private void chbLifetime_CheckedChanged(object sender, EventArgs e)
        {
            if (chbLifetime.Checked)
            {
                lblPaymentInfo.Visible = false;
                txtLastPayment.Enabled = false;
            }
            else
            {
                txtLastPayment.Enabled = true;
                checkPayment();
            }
        }

        private void datePaid_ValueChanged(object sender, EventArgs e)
        {
            txtLastPayment.Text = "";
            checkPayment();
        }

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
        /// <returns>true if frmData is saved and false if form data has 
        /// been changed and not saved. </returns>
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

            if (currentMem.Notes == null)
            {
                currentMem.Notes = "";
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
                txtPhone.Text != currentMem.PrimaryPhone.ToString() ||
                txtPhone2.Text != currentMem.SecondaryPhone.ToString() ||
                txtSSN.Text.Trim() != currentMem.SSN.ToString().Trim() ||
                txtZip.Text != currentMem.PostalCode.ToString() ||
                txtAverage.Text != currentMem.StartAvg.ToString() ||
                // checks radio buttons active Member
                (currentMem.IsActive == true && rdoActive.Checked == false) ||
                (currentMem.IsActive == false && rdoActive.Checked == true))
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

                List<PlayerHistory> AlreadyImportedPH = 
                    PlayerHistoryDB.getMemberPlayerHistory(currentMem.Number, RegionID);

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

                currentMem.MoneyEarned = moneySum;

                txtMoneyEarned.Text = currentMem.MoneyEarned.ToString("C");

                MemberDb.AddMember(currentMem);
            }
        }

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
            string playerFullName = Convert.ToString((range.Cells[1, 2] as Excel.Range).Value2);
            string playerLastName = playerFullName.Substring(0, playerFullName.IndexOf(","));
            string firstAndMiddle = playerFullName.Substring(playerFullName.IndexOf(",") + 2);
            string[] first0middle1 = firstAndMiddle.Split(' ');
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
                catch (Exception e)
                {
                    MessageBox.Show(e.ToString());
                }
            }
            return returnMe;
        }    

        private void chbSocial_CheckedChanged(object sender, EventArgs e)
        {
            txtSSN.PasswordChar = chbSocial.Checked ? '\0' : '*';
        }

        private void mtxtBoxDOB_KeyDown(object sender, KeyEventArgs e)
        {
            toolTip1.Hide(txtDOB);
        }

        private void mtxtBoxDateJoined_KeyDown(object sender, KeyEventArgs e)
        {
            toolTip1.Hide(txtDateJoined);
        }

        private void mtxtBoxRejoinDate_KeyDown(object sender, KeyEventArgs e)
        {
            toolTip1.Hide(txtDateJoined);
        }

        private void mtxtBoxLastBowled_KeyDown(object sender, KeyEventArgs e)
        {
            toolTip1.Hide(txtLastBowled);
        }

        private void MtxtBoxLastPayment_KeyDown(object sender, KeyEventArgs e)
        {
            toolTip1.Hide(txtLastPayment);
        }

        private void DateMaskTextBoxInput_MaskInputRejected(object sender, MaskInputRejectedEventArgs e)
        {
            MaskedTextBox box = sender as MaskedTextBox;

            if (box.MaskFull)
            {
                toolTip1.ToolTipTitle = "Input Rejected - Too Much Data";
                toolTip1.Show("You cannot enter any more data into the date field. " +
                    "Delete some characters in order to insert more data.", box, 0, -20, 5000);
            }
            else if (e.Position == box.Mask.Length)
            {
                toolTip1.ToolTipTitle = "Input Rejected - End of Field";
                toolTip1.Show("You cannot add extra characters to the end " +
                    "of this date field.", box, 0, -20, 5000);
            }
            else
            {
                toolTip1.ToolTipTitle = "Input Rejected";
                toolTip1.Show("You can only add numeric characters (0-9) " +
                    "into this date field.", box, 0, -20, 5000);
            }
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
    }
}