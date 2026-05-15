using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using NineTapTour.Database;
using System.Drawing.Printing;
using System.Data;
using NineTapTour.Models;
using ClosedXML.Excel;
using NineTapTour.Core.Models;

namespace NineTapTour.Forms;

public partial class FrmMemberData : Form
{
    private Member currentMem;
    
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

        UpdateMemberInfo();
    }
    
    /// <summary>
    /// finds all Controls and change BackColor of each control color when the control is on 
    /// focus and checks if that control has a child and changes the child control color onFocus
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
        if (ctrl.Tag is Color color)
            ctrl.BackColor = color;
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

        int highestMemberNumber = MemberDB.GetLastMemberNumber();

        // set txtMemberNumber.Text back to one if there is no one
        if (highestMemberNumber == 0)
        {
            txtMemberNumber.Text = "1";
        }

        currentMem = MemberDB.GetMember(Convert.ToInt32(txtMemberNumber.Text));
        List<PlayerHistoryViewModel> last5 = PlayerHistoryDB.GetLastFiveTournaments(currentMem.Number);
        if (last5.Count >= 1)
        {   //whatever the bowler director decides his average to be is right. 
            // don't pull from the player history page
            txtAverage.Text = (currentMem.Average ?? 0).ToString();
            txt30GameAvg.Text = Convert.ToInt16(last5[0].trueAVG).ToString();
            txtBonus.Text = currentMem.Bonus.ToString();
        }
        else
        {
            txtAverage.Text = (currentMem.Average ?? 0).ToString();
            txt30GameAvg.Text = 0.ToString();
            txtBonus.Text = currentMem.Bonus.ToString();
        }

        if (currentMem.Id == 0)
        {
            currentMem = new Member
            {
                Number = Convert.ToInt32(txtMemberNumber.Text)
            };

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
            // Personal Info
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
                currentMem.Handicap = Calculations.Calculations.CalculateHandicapPins((currentMem.Average.Value));
            }
            catch
            {
                currentMem.Handicap = Calculations.Calculations.CalculateHandicapPins((0));
            }

            txtHandicap.Text = currentMem.Handicap.ToString(); 

            /********************************************************************************/
            txtBonus.Text = currentMem.Bonus.ToString();

            // Misc. Info
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

                CheckPayment();
            }
            else
            {
                txtLastPayment.Text = "";
                txtPaidTo.Text = "";
                lblPaymentInfo.Visible = false;
            }                

            currentMem.MoneyEarned = 
                PlayerHistoryDB.GetTotalMoneyWon(currentMem.Number);

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
        else
        {
            txtAverage.BackColor = SystemColors.Control;
        }

        // validate lastname textbox
        if (String.IsNullOrWhiteSpace(txtLastName.Text))
        {
            lblLastNameValidation.Visible = true;
            txtLastName.Clear();
            txtLastName.BackColor = Color.LightPink;
            valid = false;
        }
        else
        {
            txtLastName.BackColor = SystemColors.Control;
        }

        // validate first name textbox
        if (String.IsNullOrWhiteSpace(txtFirstName.Text))
        {
            lblFirstNameValidation.Visible = true;
            txtFirstName.Clear();
            txtFirstName.BackColor = Color.LightPink;
            valid = false;
        }
        else
        {
            txtFirstName.BackColor = SystemColors.Control;
        }

        // validate dateJoined textbox
        if (!FormHelper.IsDateTimeValid(txtDateJoined.Text))
        {
            lblDateJoinedValidation.Visible = true;
            txtDateJoined.BackColor = Color.LightPink;
            valid = false;
        }

        // validate DOB textbox
        if(String.IsNullOrEmpty(txtDOB.Text) || txtDOB.Text.Contains("MM/DD/YYYY"))
        {
            txtDOB.BackColor = SystemColors.Control;
        }
        else if(!FormHelper.IsDateTimeValid(txtDOB.Text))
        {
            lblDOBValidation.Visible = true;
            txtDOB.BackColor = Color.LightPink;
            valid = false;
        }
        return valid;
    }

    /// <summary>
    /// Saves the information entered in the "Member Data" form.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void BtnSave_Click(object sender, EventArgs e)
    {
        SaveMemberData();
    }

    /// <summary>
    /// If data is valid, member is saved to the database.
    /// If data is invalid, invalid data is highlighted on the form
    /// </summary>
    /// <returns>Returns true if all required field are filled out</returns>
    public bool SaveMemberData()
    {
        bool isValid = IsValidTextboxes();
        // checks validation then runs the rest of the 
        // btnSave_Click and adds a member into the database.
        if (isValid)
        {
            //create temporary member for validation
            Member temp = new()
            {
                Number = Convert.ToInt32(txtMemberNumber.Text),
                IsActive = rdoActive.Checked,
                JoinDate = DateTime.Parse(txtDateJoined.Text),

                // Personal Info
                LastName = txtLastName.Text,
                FirstName = txtFirstName.Text,
                MiddleInitial = txtMiddleInitial.Text
            };
            if (!String.IsNullOrEmpty(txtDOB.Text) && !txtDOB.Text.Contains("MM/DD/YYYY"))
            {
                temp.DateOfBirth = DateTime.Parse(txtDOB.Text);
            }
            temp.SSN = txtSSN.Text;
            temp.Gender = (rdoFemale.Checked) ? MemberGenders.Female : MemberGenders.Male;

            //if member was born more than 50 years ago, then member is a senior. If member is a senior, check the isSenior checkbox and set temp.IsSenior to true
            if (temp.DateOfBirth.HasValue)
            {
                DateTime senior = DateTime.Now.AddYears(-50);
                chbSenior.Checked = senior >= temp.DateOfBirth.Value;
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
            if (Int32.TryParse(txtAverage.Text, out int leagueAverage))
            {
                temp.Average = leagueAverage;
            }
            else
            {
                temp.Average = 0;
            }

            temp.Handicap = 
                Calculations.Calculations.CalculateHandicapPins(temp.Average.Value);
            
            // Misc. Info
            if (!String.IsNullOrWhiteSpace(txtRejoinDate.Text))
            {
                if (DateTime.TryParse(txtRejoinDate.Text, out DateTime date))
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
                if (DateTime.TryParse(txtLastBowled.Text, out DateTime date))
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
                Convert.ToInt32(txtReferrals.Text);

            if (!String.IsNullOrWhiteSpace(txtLastPayment.Text))
            {
                if (DateTime.TryParse(txtLastPayment.Text, out DateTime date))
                {
                    temp.LastPayment = date;
                }
            }
            else
            {
                temp.LastPayment = null;
            }

            temp.IsLifetimeMember = chbLifetime.Checked;

            // check to see if memberId exists before putting it in 
            // current selected regions database
            if (MemberDB.MemberExists(temp))
            {
                temp.Id = MemberDB.GetMemberIdByNumber(temp.Number);
            }
            else
            {
                temp.Number = MemberDB.GetLastMemberNumber() + 1;
                txtMemberNumber.Text = temp.Number.ToString();
            }

            //Set average for the new member
            List<PlayerHistoryViewModel> last5 = PlayerHistoryDB.GetLastFiveTournaments(currentMem.Number);
            if (last5.Count >= 1)
            {   // sets the average to that of their last adjusted average
                if (Convert.ToInt32(txtAverage.Text) == last5[0].AVG)
                {
                    txtAverage.Text = last5[0].AVG.ToString();
                    temp.Average = last5[0].AVG;       
                }
                else
                {   // catches if director wants to change their average 
                    // manually regardless of there player history
                    temp.Average = Convert.ToInt32(txtAverage.Text);
                }
                txt30GameAvg.Text = last5[0].trueAVG.ToString();
            }
            else if (txtAverage.Text == "")
            {
                txtAverage.Text = 0.ToString();
                txt30GameAvg.Text = 0.ToString();
                temp.Average = 0;
            }
            else
            {
                temp.Average = Convert.ToInt16(txtAverage.Text);
                txtAverage.Text = temp.Average.ToString();
                txt30GameAvg.Text = 0.ToString();
            }
            temp.Bonus = (txtBonus.Text == string.Empty) ? 0 :
                    Convert.ToInt16(txtBonus.Text);

            // Adds Member to Database
            if (!Int32.TryParse(txtBonus.Text, out int tempBonusPins)) {
                tempBonusPins = 0;
            }

            if (tempBonusPins >= 0 && tempBonusPins <= 5)
            {
                temp.Bonus = tempBonusPins;
            }
            else
            {
                isValid = false;
                MessageBox.Show("Bonus pins is invalid!");
                txtBonus.BackColor = Color.LightPink;
            }

            if (isValid)
            {
                MemberDB.AddOrUpdateMember(temp);
                #if DEBUG
                    MessageBox.Show("Member saved");
                #endif
                ((FrmMain)MdiParent).MembersList =
                    MemberDB.GetMemberList().OrderBy(m => m.Number);
                UpdateMemberInfo();
            }
            
        }

        return isValid;
    }

    /// <summary>
    /// Displays the previous "Member Number"'s information when 
    /// the left arrow button is clicked.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void BtnArrowLeft_Click(object sender, EventArgs e)
    {
        //cursor begins when arrow is clicked
        Cursor.Current = Cursors.WaitCursor;
        List<Member> m = MemberDB.GetMemberList();
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
    private void BtnRightArrow_Click(object sender, EventArgs e)
    {
        //turns on a loading cursor while new bowler is loaded.
        Cursor.Current = Cursors.WaitCursor;
        int memberCount = MemberDB.GetLastMemberNumber();
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

    /// <summary>
    /// If "New" record button is clicked with invalid data for current member, UI will show error messages
    /// and form will not be cleared. If data is valid for current member, the form will be cleared
    /// and the new bowler can be entered
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void BtnNew_Click(object sender, EventArgs e)
    {
        if (SaveMemberData())
        {
            // Form controls are destroyed and recreated
            Controls.Clear();
            InitializeComponent();

            //finds all Controls and change BackColor of each control color when 
            //the control is on focus
            foreach (Control ctrl in this.Controls)
            {
                ChangeBackColorOnFocus(ctrl);
            }

            txtRejoinDate.Text = "";
            txtDateJoined.Text = DateTime.Now.ToString("MM/dd/yyyy");
            txtLastBowled.Text = DateTime.Now.ToString("MM/dd/yyyy");
            txtLastPayment.Text = "";
            txtDOB.Text = "MM/DD/YYYY";

            //removes placeholder text when DOB textBox is clicked
            txtDOB.GotFocus += RemovePlaceholderText;
            //adds placeholder text when DOB textBox is clicked away from with a date
            txtDOB.LostFocus += AddPlaceholderText;

            //get latest member number, or set to 1 if no members in database
            int nextMemberNumber = MemberDB.GetLastMemberNumber() + 1;
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
    }

    /// <summary>
    /// Brings up the first "Member Number".
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void BtnFirstRecord_Click(object sender, EventArgs e)
    {
        try
        {
            txtMemberNumber.Text = MemberDB.GetMemberList()[0].Number.ToString();
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
    private void BtnLastRecord_Click(object sender, EventArgs e)
    {
        txtMemberNumber.Text = MemberDB.GetLastMemberNumber().ToString();
        UpdateMemberInfo();
    }
    

    /// <summary>
    /// Opens SearchForm to search members. If member is found, updates Member Form to display that member's info
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void BtnMemberSearch_Click(object sender, EventArgs e)
    {
        FrmSearch SearchForm = new();
        SearchForm.ShowDialog();

        if (SearchForm.searchResult > 0)
        {
            txtMemberNumber.Text = SearchForm.searchResult.ToString();
            UpdateMemberInfo();
        }
    }

    // takes a list of no player history, this list would stack on 
    // top of thew original data on the form finalize page
    private void BtnStats_Click(object sender, EventArgs e)
    {
        FrmStats p = new(currentMem.Number, currentMem.FirstName + 
            currentMem.LastName + currentMem.MiddleInitial, currentMem);
        p.ShowDialog();
    }

    /// <summary>
    /// Prints Average, Handicap and Bonus of a single member
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void BtnThisRecap_Click(object sender, EventArgs e)
    {
        if (IsValidTextboxes())
        {
            //Set up components for printing
            PrintDialog printDialog = new();
            PrintDocument printDocument = new();

            //add the document to the dialog box
            printDialog.Document = printDocument;

            //add the event handler that will do the printing
            printDocument.PrintPage += new PrintPageEventHandler(SinglePrint);
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
    private void BtnPrintActive_Click(object sender, EventArgs e)
    {
        Print.PrintByActiveMembers(TournamentDB.GetAllActiveMembers());
    }

    /// <summary>
    /// Gets Member data (Name, Member Number, City, Average, Handicap and Bonus) of a single member to print
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void SinglePrint(object sender, PrintPageEventArgs e) 
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
    private void ChbLifetime_CheckedChanged(object sender, EventArgs e)
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
            CheckPayment();
        }
    }

    /// <summary>
    /// If "year membership will end" field is changed, check if payment was made more than a year ago
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void DatePaid_ValueChanged(object sender, EventArgs e)
    {
        txtLastPayment.Text = "";
        txtPaidTo.Text = "";
        CheckPayment();
    }

    /// <summary>
    /// Checks if last payment was made more than a year ago. If it was, show warning label that payment is due. 
    /// </summary>
    private void CheckPayment()
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
        ((FrmMain)MdiParent).CurrFrmMemberData = this;
    }

    /// <summary>
    /// This button imports member data from excel file
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void BtnImportData_Click(object sender, EventArgs e)
    {
        List<ExcelRow> CurrentExcelData = [];
        OpenFileDialog ofdOpen = new()
        {
            Filter = FileHelper.GetExcelFilterStringForFileDialogs()
        };

        if (ofdOpen.ShowDialog() == DialogResult.OK)
        {
            List<PlayerHistoryViewModel> AlreadyImportedPH = 
                PlayerHistoryDB.GetMemberPlayerHistory(currentMem.Number);

            if (AlreadyImportedPH.Count > 0)
            {
                MessageBox.Show("Member history has already been imported");
                return;
            }

            string fileName = ofdOpen.FileName;

            frmPleaseWait please = new();
            please.Show();

            // Process the Excel file and create tournaments/participants
            List<ExcelRow> rows = ProcessExcelFile(fileName); 
            
            please.Close();

            foreach(var r in rows)
            {
                CurrentExcelData.Add(r);
            }

            // Update member's averages after import
            List<PlayerHistoryViewModel> reset = PlayerHistoryDB.GetLastFiveTournaments(currentMem.Number);
            if (reset.Count > 0)
            {
                currentMem.Average = reset[0].AVG;
                currentMem.Handicap = Calculations.Calculations
                    .CalculateHandicapPins(Convert.ToInt32(currentMem.Average));

                currentMem.Bonus = reset[0].Bonus;
                txtAverage.Text = currentMem.Average.ToString();
                txt30GameAvg.Text = Convert.ToInt32(reset[0].trueAVG).ToString();
                txtHandicap.Text = currentMem.Handicap.ToString();
                txtBonus.Text = currentMem.Bonus.ToString();
            }

            // Grabs the total money won by the member
            decimal moneySum = PlayerHistoryDB.GetTotalMoneyWon(currentMem.Number);

            currentMem.MoneyEarned += moneySum; 

            txtMoneyEarned.Text = currentMem.MoneyEarned.ToString("C");

            MemberDB.AddOrUpdateMember(currentMem);
            
            MessageBox.Show($"Import completed. {rows.Count} games imported across multiple tournaments.");
        }
    }

    /// <summary>
    /// Processes excel file for member data import
    /// Creates tournaments for each unique date and links games to member through participants
    /// </summary>
    /// <param name="PathAndFileName"></param>
    /// <returns></returns>
    private List<ExcelRow> ProcessExcelFile(string PathAndFileName)
    {
        List<ExcelRow> returnMe = [];
        // Dictionary to track tournaments by date
        Dictionary<DateTime, Tournament> tournamentsCache = new();
        
        using (var workbook = new XLWorkbook(PathAndFileName))
        {
            var ws = workbook.Worksheet(1);
            string[] PlayerFinalFirstAndMiddle = ["", ""];
            string playerLastName = "";
            string firstAndMiddle = "";
            string playerFullName = ws.Cell(1, 2).GetString();
            SplitName(ref playerLastName, ref firstAndMiddle, playerFullName);
            string[] first0middle1 = firstAndMiddle.Split(' ');
            int playerOrgAVG = ws.Cell(1, 10).GetValue<int?>() ?? -1;
            string playerNumber = ws.Cell(1, 14).GetString();
            bool isRegionHawaii = (cbHaw.Checked);
            if (isRegionHawaii)
            {
                playerNumber = RegexHelpers.StripNonNumericRegex().Replace(playerNumber, string.Empty);
            }
            int.TryParse(playerNumber, out int playerNumberAsInt);
            if (playerNumberAsInt != 0)
            {
                playerNumberAsInt = Convert.ToInt32(RegexHelpers.StripNonNumericRegex().Replace(playerNumber, string.Empty));
            }
            else if (playerNumberAsInt == 0)
            {
                var playerNumberAfterSplit = playerNumber.Split('/');
                playerNumberAsInt = Convert.ToInt32(RegexHelpers.StripNonNumericRegex().Replace(playerNumberAfterSplit[^1], string.Empty));
            }
            
            // Data rows start at row 3 (or 4 for Hawaii)
            int rowNum = isRegionHawaii ? 4 : 3;
            int lastRow = ws.LastRowUsed().RowNumber();
            
            for (int row = rowNum; row <= lastRow; row++)
            {
                ExcelRow temp = new();
                temp.PlayerFirstName = PlayerFinalFirstAndMiddle[0];
                temp.PlayerMiddleName = PlayerFinalFirstAndMiddle.Length > 1 ? PlayerFinalFirstAndMiddle[1] : "";
                temp.PlayerLastName = playerLastName;
                temp.PlayerOrginalAVG = playerOrgAVG;
                temp.PlayerNumber = playerNumberAsInt;
                temp.GameTotal = ws.Cell(row, 1).GetValue<int?>() ?? -1;
                temp.Date = ws.Cell(row, 2).GetDateTime();
                temp.Game1 = ws.Cell(row, 3).GetValue<int?>() ?? -1;
                temp.Game2 = ws.Cell(row, 4).GetValue<int?>() ?? -1;
                temp.Game3 = ws.Cell(row, 5).GetValue<int?>() ?? -1;
                temp.Game4 = ws.Cell(row, 6).GetValue<int?>() ?? -1;
                temp.Total = ws.Cell(row, 7).GetValue<int?>() ?? -1;
                temp.AverageOfRow = ws.Cell(row, 8).GetValue<double?>() ?? -1;
                temp.TrueAverage = ws.Cell(row, 9).GetValue<double?>() ?? -1;
                temp.AVG = ws.Cell(row, 10).GetValue<int?>() ?? -1;
                temp.HandyCap = ws.Cell(row, 11).GetValue<int?>() ?? -1000;
                temp.Bonus = ws.Cell(row, 12).GetValue<int?>() ?? -1;
                temp.FinPPHG = ws.Cell(row, 14).GetString();
                temp.Cash = ws.Cell(row, 15).GetValue<double?>() ?? 0;
                temp.Notes = ws.Cell(row, 16).GetString();
                
                // Create or get tournament for this date
                Tournament tournament;
                DateTime tournamentDate = temp.Date.Date; // Normalize to date only
                
                if (!tournamentsCache.ContainsKey(tournamentDate))
                {
                    // Check if tournament already exists in database
                    List<Tournament> existingTournaments = TournamentDB.GetTournamentList()
                        .Where(t => t.Date.Date == tournamentDate).ToList();
                    
                    if (existingTournaments.Count > 0)
                    {
                        tournament = existingTournaments[0];
                    }
                    else
                    {
                        // Create new tournament for this date
                        tournament = new Tournament
                        {
                            Date = tournamentDate,
                            Location = $"Imported - {tournamentDate:yyyy-MM-dd}",
                            Event = "Legacy Data Import",
                            Notes = "Tournament created from legacy data import",
                            Squads = 1,
                            Doubles = false,
                            ThreeOutOf4 = false,
                            IsOnlyThreeGames = false,
                            IsTournamentFinalized = false
                        };
                        
                        // Add tournament to database
                        TournamentDB.AddTournament(tournament);
                    }
                    
                    tournamentsCache[tournamentDate] = tournament;
                }
                else
                {
                    tournament = tournamentsCache[tournamentDate];
                }
                
                // Create Game entity
                Game game = new Game
                {
                    Game1 = temp.Game1 >= 0 ? temp.Game1 : null,
                    Game2 = temp.Game2 >= 0 ? temp.Game2 : null,
                    Game3 = temp.Game3 >= 0 ? temp.Game3 : null,
                    Game4 = temp.Game4 >= 0 ? temp.Game4 : null,
                    Handicap = temp.HandyCap >= 0 ? temp.HandyCap : null,
                    Bonus = temp.Bonus >= 0 ? temp.Bonus : null,
                    MoneyWon = temp.Cash > 0 ? Convert.ToDecimal(temp.Cash) : null,
                    Notes = temp.Notes,
                    IsFinalized = true, // Mark as finalized since it's legacy data
                    AdjustedAvg = temp.AVG,
                    LeagueAverage = temp.TrueAverage,
                    UseGame1 = temp.Game1 >= 0,
                    UseGame2 = temp.Game2 >= 0,
                    UseGame3 = temp.Game3 >= 0,
                    UseGame4 = temp.Game4 >= 0
                };
                
                // Create Participant linking member, game, and tournament
                Participant participant = new Participant
                {
                    Member = currentMem,
                    Game = game,
                    Tournament = tournament,
                    Squad = 1 // Default squad for imported data
                };
                
                // Add participant (which will also save the game)
                TournamentDB.AddMemberToTournament(participant);
                
                returnMe.Add(temp);
            }
        }
        return returnMe;
    }

    /// <summary>
    /// Takes the imported excel row for playerFullName and splits it into playerLastName and firstAndMiddle strings
    /// </summary>
    /// <param name="playerLastName"></param>
    /// <param name="firstAndMiddle"></param>
    /// <param name="playerFullName"></param>
    private static void SplitName(ref string playerLastName, ref string firstAndMiddle, string playerFullName)
    {
        if (playerFullName.Contains(','))
        {
            playerLastName = playerFullName[..playerFullName.IndexOf(',')];
            firstAndMiddle = playerFullName[(playerFullName.IndexOf(',') + 2)..];
        }
        // Checks to see if a period instead of a comma was accidentally placed in member name. (Rob's Request)
        else if (playerFullName.Contains('.'))
        {
            playerLastName = playerFullName[..playerFullName.IndexOf('.')];
            firstAndMiddle = playerFullName[(playerFullName.IndexOf('.') + 2)..];
        }
    }

    /// <summary>
    /// If SSN checkbox is checked, the social security number is shown, if not the social secuity number is masked with '*'
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ChbSocial_CheckedChanged(object sender, EventArgs e)
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
    private void FlpMemberScores_SizeChanged(object sender, EventArgs e)
    {
        FormHelper.SetFlowControlScrollBars(this, flpMemberData, 1080, 600);
    }

    private void MtxtBox_Click(object sender, EventArgs e)
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
    private void TxtBonus_TextChanged(object sender, EventArgs e)
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
    private void ChbSenior_KeyDown(object sender, KeyEventArgs e)
    {
        CheckBox currentCheckBox = sender as CheckBox;
        currentCheckBox.Checked = true;
    }

    private void TxtLastName_TextChanged(object sender, EventArgs e)
    {
        lblLastNameValidation.Visible = false;
    }

    private void TxtFirstName_TextChanged(object sender, EventArgs e)
    {
        lblFirstNameValidation.Visible = false;
    }

    private void TxtDOB_TextChanged(object sender, EventArgs e)
    {
        lblDOBValidation.Visible = false;
    }

    private void TxtAverage_TextChanged(object sender, EventArgs e)
    {
        lblAverageValidation.Visible = false;
    }

    private void RdoFemale_KeyDown(object sender, KeyEventArgs e)
    {
        RadioButton currentRadioButton = sender as RadioButton;
        currentRadioButton.Checked = true;
    }
}