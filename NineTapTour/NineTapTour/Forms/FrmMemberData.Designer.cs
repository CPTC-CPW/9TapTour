namespace NineTapTour.Forms
{
    partial class FrmMemberData
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmMemberData));
            this.txtLastName = new System.Windows.Forms.TextBox();
            this.txtFirstName = new System.Windows.Forms.TextBox();
            this.txtMiddleInitial = new System.Windows.Forms.TextBox();
            this.lblLastName = new System.Windows.Forms.Label();
            this.lblFirstName = new System.Windows.Forms.Label();
            this.lblMiddleIntial = new System.Windows.Forms.Label();
            this.lblMemberNum = new System.Windows.Forms.Label();
            this.txtMemberNumber = new System.Windows.Forms.TextBox();
            this.txtBonus = new System.Windows.Forms.TextBox();
            this.txtHandicap = new System.Windows.Forms.TextBox();
            this.lblAverage = new System.Windows.Forms.Label();
            this.lblHandicap = new System.Windows.Forms.Label();
            this.lblBonus = new System.Windows.Forms.Label();
            this.txtNotes = new System.Windows.Forms.TextBox();
            this.txtAverage = new System.Windows.Forms.TextBox();
            this.lblNotes = new System.Windows.Forms.Label();
            this.grpMemberInfo = new System.Windows.Forms.GroupBox();
            this.dateRejoin = new System.Windows.Forms.DateTimePicker();
            this.dateJoined = new System.Windows.Forms.DateTimePicker();
            this.lblEmail = new System.Windows.Forms.Label();
            this.txtPhoneNumber2 = new System.Windows.Forms.TextBox();
            this.lblDOB = new System.Windows.Forms.Label();
            this.txtPhoneNumber = new System.Windows.Forms.TextBox();
            this.txtDOB = new System.Windows.Forms.TextBox();
            this.lblRefferals = new System.Windows.Forms.Label();
            this.lblPhoneNumber2 = new System.Windows.Forms.Label();
            this.lblPhoneNumber = new System.Windows.Forms.Label();
            this.lblDateJoined = new System.Windows.Forms.Label();
            this.lblRejoinDate = new System.Windows.Forms.Label();
            this.lblZip = new System.Windows.Forms.Label();
            this.lblState = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtReferrals = new System.Windows.Forms.TextBox();
            this.lblAddress = new System.Windows.Forms.Label();
            this.lblSSN = new System.Windows.Forms.Label();
            this.txtZip = new System.Windows.Forms.TextBox();
            this.txtState = new System.Windows.Forms.TextBox();
            this.txtCity = new System.Windows.Forms.TextBox();
            this.txtAddress = new System.Windows.Forms.TextBox();
            this.txtSSN = new System.Windows.Forms.TextBox();
            this.txtEmail = new System.Windows.Forms.TextBox();
            this.txtYearEndT = new System.Windows.Forms.TextBox();
            this.txtMoneyEarned = new System.Windows.Forms.TextBox();
            this.lblYET = new System.Windows.Forms.Label();
            this.lblMoneyEarned = new System.Windows.Forms.Label();
            this.lblLastBowled = new System.Windows.Forms.Label();
            this.groupRecord = new System.Windows.Forms.GroupBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnNew = new System.Windows.Forms.Button();
            this.grpRecordNumber = new System.Windows.Forms.GroupBox();
            this.btnLastRecord = new System.Windows.Forms.Button();
            this.btnFirstRecord = new System.Windows.Forms.Button();
            this.btnRightArrow = new System.Windows.Forms.Button();
            this.btnArrowLeft = new System.Windows.Forms.Button();
            this.grpStats = new System.Windows.Forms.GroupBox();
            this.btnStats = new System.Windows.Forms.Button();
            this.grpSearchBy = new System.Windows.Forms.GroupBox();
            this.btnLastName = new System.Windows.Forms.Button();
            this.btnFirstName = new System.Windows.Forms.Button();
            this.btnMemberNumber = new System.Windows.Forms.Button();
            this.grpPrinter = new System.Windows.Forms.GroupBox();
            this.btnRecapByPin = new System.Windows.Forms.Button();
            this.btnLabels = new System.Windows.Forms.Button();
            this.btnRecapByDate = new System.Windows.Forms.Button();
            this.btnThisRecap = new System.Windows.Forms.Button();
            this.btnAllRecaps = new System.Windows.Forms.Button();
            this.rdoFemale = new System.Windows.Forms.RadioButton();
            this.rdoActive = new System.Windows.Forms.RadioButton();
            this.rdoInActive = new System.Windows.Forms.RadioButton();
            this.rdoMale = new System.Windows.Forms.RadioButton();
            this.grpGender = new System.Windows.Forms.GroupBox();
            this.grpStatus = new System.Windows.Forms.GroupBox();
            this.chbSenior = new System.Windows.Forms.CheckBox();
            this.dateLastBowled = new System.Windows.Forms.DateTimePicker();
            this.grpMemberInfo.SuspendLayout();
            this.groupRecord.SuspendLayout();
            this.grpRecordNumber.SuspendLayout();
            this.grpStats.SuspendLayout();
            this.grpSearchBy.SuspendLayout();
            this.grpPrinter.SuspendLayout();
            this.grpGender.SuspendLayout();
            this.grpStatus.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtLastName
            // 
            this.txtLastName.Font = new System.Drawing.Font("Calibri", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtLastName.Location = new System.Drawing.Point(179, 31);
            this.txtLastName.MaxLength = 50;
            this.txtLastName.Multiline = true;
            this.txtLastName.Name = "txtLastName";
            this.txtLastName.Size = new System.Drawing.Size(206, 38);
            this.txtLastName.TabIndex = 1;
            this.txtLastName.TextChanged += new System.EventHandler(this.InputRequired);
            // 
            // txtFirstName
            // 
            this.txtFirstName.Font = new System.Drawing.Font("Calibri", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtFirstName.Location = new System.Drawing.Point(414, 31);
            this.txtFirstName.MaxLength = 50;
            this.txtFirstName.Multiline = true;
            this.txtFirstName.Name = "txtFirstName";
            this.txtFirstName.Size = new System.Drawing.Size(202, 38);
            this.txtFirstName.TabIndex = 2;
            this.txtFirstName.TextChanged += new System.EventHandler(this.InputRequired);
            // 
            // txtMiddleInitial
            // 
            this.txtMiddleInitial.Font = new System.Drawing.Font("Calibri", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtMiddleInitial.Location = new System.Drawing.Point(639, 31);
            this.txtMiddleInitial.MaxLength = 10;
            this.txtMiddleInitial.Multiline = true;
            this.txtMiddleInitial.Name = "txtMiddleInitial";
            this.txtMiddleInitial.Size = new System.Drawing.Size(86, 38);
            this.txtMiddleInitial.TabIndex = 3;
            this.txtMiddleInitial.TextChanged += new System.EventHandler(this.InputRequired);
            // 
            // lblLastName
            // 
            this.lblLastName.AutoSize = true;
            this.lblLastName.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLastName.Location = new System.Drawing.Point(179, 9);
            this.lblLastName.Name = "lblLastName";
            this.lblLastName.Size = new System.Drawing.Size(80, 19);
            this.lblLastName.TabIndex = 3;
            this.lblLastName.Text = "Last Name";
            // 
            // lblFirstName
            // 
            this.lblFirstName.AutoSize = true;
            this.lblFirstName.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFirstName.Location = new System.Drawing.Point(414, 9);
            this.lblFirstName.Name = "lblFirstName";
            this.lblFirstName.Size = new System.Drawing.Size(82, 19);
            this.lblFirstName.TabIndex = 2;
            this.lblFirstName.Text = "First Name";
            // 
            // lblMiddleIntial
            // 
            this.lblMiddleIntial.AutoSize = true;
            this.lblMiddleIntial.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMiddleIntial.Location = new System.Drawing.Point(639, 9);
            this.lblMiddleIntial.Name = "lblMiddleIntial";
            this.lblMiddleIntial.Size = new System.Drawing.Size(103, 19);
            this.lblMiddleIntial.TabIndex = 5;
            this.lblMiddleIntial.Text = "Middle Initial ";
            // 
            // lblMemberNum
            // 
            this.lblMemberNum.AutoSize = true;
            this.lblMemberNum.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMemberNum.Location = new System.Drawing.Point(34, 102);
            this.lblMemberNum.Name = "lblMemberNum";
            this.lblMemberNum.Size = new System.Drawing.Size(127, 19);
            this.lblMemberNum.TabIndex = 6;
            this.lblMemberNum.Text = "Member Number";
            // 
            // txtMemberNumber
            // 
            this.txtMemberNumber.Font = new System.Drawing.Font("Calibri", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtMemberNumber.Location = new System.Drawing.Point(38, 26);
            this.txtMemberNumber.Multiline = true;
            this.txtMemberNumber.Name = "txtMemberNumber";
            this.txtMemberNumber.ReadOnly = true;
            this.txtMemberNumber.Size = new System.Drawing.Size(119, 73);
            this.txtMemberNumber.TabIndex = 0;
            this.txtMemberNumber.TabStop = false;
            this.txtMemberNumber.Text = "1";
            this.txtMemberNumber.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtBonus
            // 
            this.txtBonus.Font = new System.Drawing.Font("Calibri", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBonus.Location = new System.Drawing.Point(624, 327);
            this.txtBonus.MaxLength = 20;
            this.txtBonus.Multiline = true;
            this.txtBonus.Name = "txtBonus";
            this.txtBonus.Size = new System.Drawing.Size(89, 54);
            this.txtBonus.TabIndex = 7;
            this.txtBonus.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtHandicap
            // 
            this.txtHandicap.Font = new System.Drawing.Font("Calibri", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtHandicap.Location = new System.Drawing.Point(496, 327);
            this.txtHandicap.MaxLength = 20;
            this.txtHandicap.Multiline = true;
            this.txtHandicap.Name = "txtHandicap";
            this.txtHandicap.Size = new System.Drawing.Size(89, 54);
            this.txtHandicap.TabIndex = 6;
            this.txtHandicap.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lblAverage
            // 
            this.lblAverage.AutoSize = true;
            this.lblAverage.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAverage.Location = new System.Drawing.Point(353, 308);
            this.lblAverage.Name = "lblAverage";
            this.lblAverage.Size = new System.Drawing.Size(50, 15);
            this.lblAverage.TabIndex = 17;
            this.lblAverage.Text = "Average";
            // 
            // lblHandicap
            // 
            this.lblHandicap.AutoSize = true;
            this.lblHandicap.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHandicap.Location = new System.Drawing.Point(493, 311);
            this.lblHandicap.Name = "lblHandicap";
            this.lblHandicap.Size = new System.Drawing.Size(60, 15);
            this.lblHandicap.TabIndex = 17;
            this.lblHandicap.Text = "Handicap";
            // 
            // lblBonus
            // 
            this.lblBonus.AutoSize = true;
            this.lblBonus.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBonus.Location = new System.Drawing.Point(621, 311);
            this.lblBonus.Name = "lblBonus";
            this.lblBonus.Size = new System.Drawing.Size(41, 15);
            this.lblBonus.TabIndex = 17;
            this.lblBonus.Text = "Bonus";
            // 
            // txtNotes
            // 
            this.txtNotes.Location = new System.Drawing.Point(138, 155);
            this.txtNotes.MaxLength = 750;
            this.txtNotes.Multiline = true;
            this.txtNotes.Name = "txtNotes";
            this.txtNotes.Size = new System.Drawing.Size(196, 266);
            this.txtNotes.TabIndex = 18;
            this.txtNotes.TabStop = false;
            // 
            // txtAverage
            // 
            this.txtAverage.Font = new System.Drawing.Font("Calibri", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAverage.Location = new System.Drawing.Point(356, 324);
            this.txtAverage.MaxLength = 20;
            this.txtAverage.Multiline = true;
            this.txtAverage.Name = "txtAverage";
            this.txtAverage.Size = new System.Drawing.Size(99, 54);
            this.txtAverage.TabIndex = 5;
            this.txtAverage.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lblNotes
            // 
            this.lblNotes.AutoSize = true;
            this.lblNotes.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNotes.Location = new System.Drawing.Point(134, 133);
            this.lblNotes.Name = "lblNotes";
            this.lblNotes.Size = new System.Drawing.Size(49, 19);
            this.lblNotes.TabIndex = 22;
            this.lblNotes.Text = "Notes";
            // 
            // grpMemberInfo
            // 
            this.grpMemberInfo.Controls.Add(this.dateRejoin);
            this.grpMemberInfo.Controls.Add(this.dateJoined);
            this.grpMemberInfo.Controls.Add(this.lblEmail);
            this.grpMemberInfo.Controls.Add(this.txtPhoneNumber2);
            this.grpMemberInfo.Controls.Add(this.lblDOB);
            this.grpMemberInfo.Controls.Add(this.txtPhoneNumber);
            this.grpMemberInfo.Controls.Add(this.txtDOB);
            this.grpMemberInfo.Controls.Add(this.lblRefferals);
            this.grpMemberInfo.Controls.Add(this.lblPhoneNumber2);
            this.grpMemberInfo.Controls.Add(this.lblPhoneNumber);
            this.grpMemberInfo.Controls.Add(this.lblDateJoined);
            this.grpMemberInfo.Controls.Add(this.lblRejoinDate);
            this.grpMemberInfo.Controls.Add(this.lblZip);
            this.grpMemberInfo.Controls.Add(this.lblState);
            this.grpMemberInfo.Controls.Add(this.label1);
            this.grpMemberInfo.Controls.Add(this.txtReferrals);
            this.grpMemberInfo.Controls.Add(this.lblAddress);
            this.grpMemberInfo.Controls.Add(this.lblSSN);
            this.grpMemberInfo.Controls.Add(this.txtZip);
            this.grpMemberInfo.Controls.Add(this.txtState);
            this.grpMemberInfo.Controls.Add(this.txtCity);
            this.grpMemberInfo.Controls.Add(this.txtAddress);
            this.grpMemberInfo.Controls.Add(this.txtSSN);
            this.grpMemberInfo.Controls.Add(this.txtEmail);
            this.grpMemberInfo.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpMemberInfo.Location = new System.Drawing.Point(340, 86);
            this.grpMemberInfo.Name = "grpMemberInfo";
            this.grpMemberInfo.Size = new System.Drawing.Size(416, 216);
            this.grpMemberInfo.TabIndex = 4;
            this.grpMemberInfo.TabStop = false;
            this.grpMemberInfo.Text = "Member Information";
            // 
            // dateRejoin
            // 
            this.dateRejoin.Checked = false;
            this.dateRejoin.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dateRejoin.Location = new System.Drawing.Point(299, 143);
            this.dateRejoin.Name = "dateRejoin";
            this.dateRejoin.Size = new System.Drawing.Size(103, 23);
            this.dateRejoin.TabIndex = 35;
            this.dateRejoin.CloseUp += new System.EventHandler(this.ApplyCalendarForm);
            this.dateRejoin.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ClearCalendar);
            // 
            // dateJoined
            // 
            this.dateJoined.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dateJoined.Location = new System.Drawing.Point(299, 106);
            this.dateJoined.Name = "dateJoined";
            this.dateJoined.Size = new System.Drawing.Size(103, 23);
            this.dateJoined.TabIndex = 32;
            this.dateJoined.CloseUp += new System.EventHandler(this.ApplyCalendarForm);
            // 
            // lblEmail
            // 
            this.lblEmail.AutoSize = true;
            this.lblEmail.Location = new System.Drawing.Point(16, 165);
            this.lblEmail.Name = "lblEmail";
            this.lblEmail.Size = new System.Drawing.Size(38, 15);
            this.lblEmail.TabIndex = 10;
            this.lblEmail.Text = "Email";
            // 
            // txtPhoneNumber2
            // 
            this.txtPhoneNumber2.Location = new System.Drawing.Point(167, 143);
            this.txtPhoneNumber2.MaxLength = 20;
            this.txtPhoneNumber2.Name = "txtPhoneNumber2";
            this.txtPhoneNumber2.Size = new System.Drawing.Size(115, 23);
            this.txtPhoneNumber2.TabIndex = 7;
            // 
            // lblDOB
            // 
            this.lblDOB.AutoSize = true;
            this.lblDOB.Location = new System.Drawing.Point(16, 17);
            this.lblDOB.Name = "lblDOB";
            this.lblDOB.Size = new System.Drawing.Size(40, 15);
            this.lblDOB.TabIndex = 31;
            this.lblDOB.Text = "D.O.B.";
            // 
            // txtPhoneNumber
            // 
            this.txtPhoneNumber.Location = new System.Drawing.Point(16, 143);
            this.txtPhoneNumber.MaxLength = 20;
            this.txtPhoneNumber.Name = "txtPhoneNumber";
            this.txtPhoneNumber.Size = new System.Drawing.Size(115, 23);
            this.txtPhoneNumber.TabIndex = 6;
            this.txtPhoneNumber.TextChanged += new System.EventHandler(this.InputRequired);
            // 
            // txtDOB
            // 
            this.txtDOB.Location = new System.Drawing.Point(16, 32);
            this.txtDOB.MaxLength = 20;
            this.txtDOB.Name = "txtDOB";
            this.txtDOB.Size = new System.Drawing.Size(111, 23);
            this.txtDOB.TabIndex = 0;
            this.txtDOB.TextChanged += new System.EventHandler(this.InputRequired);
            // 
            // lblRefferals
            // 
            this.lblRefferals.AutoSize = true;
            this.lblRefferals.Location = new System.Drawing.Point(299, 165);
            this.lblRefferals.Name = "lblRefferals";
            this.lblRefferals.Size = new System.Drawing.Size(57, 15);
            this.lblRefferals.TabIndex = 6;
            this.lblRefferals.Text = "Referrals";
            // 
            // lblPhoneNumber2
            // 
            this.lblPhoneNumber2.AutoSize = true;
            this.lblPhoneNumber2.Location = new System.Drawing.Point(167, 128);
            this.lblPhoneNumber2.Name = "lblPhoneNumber2";
            this.lblPhoneNumber2.Size = new System.Drawing.Size(97, 15);
            this.lblPhoneNumber2.TabIndex = 8;
            this.lblPhoneNumber2.Text = "Phone Number 2";
            // 
            // lblPhoneNumber
            // 
            this.lblPhoneNumber.AutoSize = true;
            this.lblPhoneNumber.Location = new System.Drawing.Point(16, 128);
            this.lblPhoneNumber.Name = "lblPhoneNumber";
            this.lblPhoneNumber.Size = new System.Drawing.Size(87, 15);
            this.lblPhoneNumber.TabIndex = 7;
            this.lblPhoneNumber.Text = "Phone Number";
            // 
            // lblDateJoined
            // 
            this.lblDateJoined.AutoSize = true;
            this.lblDateJoined.Location = new System.Drawing.Point(299, 91);
            this.lblDateJoined.Name = "lblDateJoined";
            this.lblDateJoined.Size = new System.Drawing.Size(70, 15);
            this.lblDateJoined.TabIndex = 5;
            this.lblDateJoined.Text = "Date Joined";
            // 
            // lblRejoinDate
            // 
            this.lblRejoinDate.AutoSize = true;
            this.lblRejoinDate.Location = new System.Drawing.Point(299, 128);
            this.lblRejoinDate.Name = "lblRejoinDate";
            this.lblRejoinDate.Size = new System.Drawing.Size(70, 15);
            this.lblRejoinDate.TabIndex = 4;
            this.lblRejoinDate.Text = "Rejoin Date";
            // 
            // lblZip
            // 
            this.lblZip.AutoSize = true;
            this.lblZip.Location = new System.Drawing.Point(209, 91);
            this.lblZip.Name = "lblZip";
            this.lblZip.Size = new System.Drawing.Size(24, 15);
            this.lblZip.TabIndex = 2;
            this.lblZip.Text = "Zip";
            // 
            // lblState
            // 
            this.lblState.AutoSize = true;
            this.lblState.Location = new System.Drawing.Point(146, 91);
            this.lblState.Name = "lblState";
            this.lblState.Size = new System.Drawing.Size(34, 15);
            this.lblState.TabIndex = 2;
            this.lblState.Text = "State";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 91);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(28, 15);
            this.label1.TabIndex = 2;
            this.label1.Text = "City";
            // 
            // txtReferrals
            // 
            this.txtReferrals.Location = new System.Drawing.Point(299, 180);
            this.txtReferrals.MaxLength = 50;
            this.txtReferrals.Name = "txtReferrals";
            this.txtReferrals.Size = new System.Drawing.Size(54, 23);
            this.txtReferrals.TabIndex = 11;
            // 
            // lblAddress
            // 
            this.lblAddress.AutoSize = true;
            this.lblAddress.Location = new System.Drawing.Point(16, 54);
            this.lblAddress.Name = "lblAddress";
            this.lblAddress.Size = new System.Drawing.Size(51, 15);
            this.lblAddress.TabIndex = 3;
            this.lblAddress.Text = "Address";
            // 
            // lblSSN
            // 
            this.lblSSN.AutoSize = true;
            this.lblSSN.Location = new System.Drawing.Point(134, 17);
            this.lblSSN.Name = "lblSSN";
            this.lblSSN.Size = new System.Drawing.Size(27, 15);
            this.lblSSN.TabIndex = 3;
            this.lblSSN.Text = "SSN";
            // 
            // txtZip
            // 
            this.txtZip.Location = new System.Drawing.Point(209, 106);
            this.txtZip.MaxLength = 20;
            this.txtZip.Name = "txtZip";
            this.txtZip.Size = new System.Drawing.Size(73, 23);
            this.txtZip.TabIndex = 5;
            this.txtZip.TextChanged += new System.EventHandler(this.InputRequired);
            // 
            // txtState
            // 
            this.txtState.Location = new System.Drawing.Point(146, 106);
            this.txtState.MaxLength = 20;
            this.txtState.Name = "txtState";
            this.txtState.Size = new System.Drawing.Size(32, 23);
            this.txtState.TabIndex = 4;
            this.txtState.TextChanged += new System.EventHandler(this.InputRequired);
            // 
            // txtCity
            // 
            this.txtCity.Location = new System.Drawing.Point(16, 106);
            this.txtCity.MaxLength = 20;
            this.txtCity.Name = "txtCity";
            this.txtCity.Size = new System.Drawing.Size(100, 23);
            this.txtCity.TabIndex = 3;
            this.txtCity.TextChanged += new System.EventHandler(this.InputRequired);
            // 
            // txtAddress
            // 
            this.txtAddress.Location = new System.Drawing.Point(16, 69);
            this.txtAddress.MaxLength = 75;
            this.txtAddress.Name = "txtAddress";
            this.txtAddress.Size = new System.Drawing.Size(266, 23);
            this.txtAddress.TabIndex = 2;
            this.txtAddress.TextChanged += new System.EventHandler(this.InputRequired);
            // 
            // txtSSN
            // 
            this.txtSSN.Location = new System.Drawing.Point(134, 32);
            this.txtSSN.MaxLength = 11;
            this.txtSSN.Name = "txtSSN";
            this.txtSSN.Size = new System.Drawing.Size(75, 23);
            this.txtSSN.TabIndex = 1;
            this.txtSSN.TextChanged += new System.EventHandler(this.InputRequired);
            // 
            // txtEmail
            // 
            this.txtEmail.Location = new System.Drawing.Point(16, 180);
            this.txtEmail.MaxLength = 50;
            this.txtEmail.Name = "txtEmail";
            this.txtEmail.Size = new System.Drawing.Size(266, 23);
            this.txtEmail.TabIndex = 8;
            this.txtEmail.TextChanged += new System.EventHandler(this.InputRequired);
            // 
            // txtYearEndT
            // 
            this.txtYearEndT.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtYearEndT.Location = new System.Drawing.Point(387, 403);
            this.txtYearEndT.MaxLength = 20;
            this.txtYearEndT.Name = "txtYearEndT";
            this.txtYearEndT.Size = new System.Drawing.Size(35, 23);
            this.txtYearEndT.TabIndex = 8;
            // 
            // txtMoneyEarned
            // 
            this.txtMoneyEarned.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtMoneyEarned.Location = new System.Drawing.Point(489, 403);
            this.txtMoneyEarned.Name = "txtMoneyEarned";
            this.txtMoneyEarned.Size = new System.Drawing.Size(100, 23);
            this.txtMoneyEarned.TabIndex = 9;
            // 
            // lblYET
            // 
            this.lblYET.AutoSize = true;
            this.lblYET.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblYET.Location = new System.Drawing.Point(349, 390);
            this.lblYET.Name = "lblYET";
            this.lblYET.Size = new System.Drawing.Size(127, 15);
            this.lblYET.TabIndex = 25;
            this.lblYET.Text = "Year End Tournaments";
            // 
            // lblMoneyEarned
            // 
            this.lblMoneyEarned.AutoSize = true;
            this.lblMoneyEarned.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMoneyEarned.Location = new System.Drawing.Point(487, 390);
            this.lblMoneyEarned.Name = "lblMoneyEarned";
            this.lblMoneyEarned.Size = new System.Drawing.Size(85, 15);
            this.lblMoneyEarned.TabIndex = 26;
            this.lblMoneyEarned.Text = "Money Earned";
            // 
            // lblLastBowled
            // 
            this.lblLastBowled.AutoSize = true;
            this.lblLastBowled.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLastBowled.Location = new System.Drawing.Point(615, 390);
            this.lblLastBowled.Name = "lblLastBowled";
            this.lblLastBowled.Size = new System.Drawing.Size(72, 15);
            this.lblLastBowled.TabIndex = 27;
            this.lblLastBowled.Text = "Last Bowled";
            // 
            // groupRecord
            // 
            this.groupRecord.Controls.Add(this.btnSave);
            this.groupRecord.Controls.Add(this.btnDelete);
            this.groupRecord.Controls.Add(this.btnNew);
            this.groupRecord.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupRecord.Location = new System.Drawing.Point(24, 427);
            this.groupRecord.Name = "groupRecord";
            this.groupRecord.Size = new System.Drawing.Size(108, 100);
            this.groupRecord.TabIndex = 11;
            this.groupRecord.TabStop = false;
            this.groupRecord.Text = "Record";
            // 
            // btnSave
            // 
            this.btnSave.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSave.Location = new System.Drawing.Point(17, 45);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 0;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDelete.Location = new System.Drawing.Point(17, 71);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(75, 23);
            this.btnDelete.TabIndex = 2;
            this.btnDelete.TabStop = false;
            this.btnDelete.Text = "Delete";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnNew
            // 
            this.btnNew.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnNew.Location = new System.Drawing.Point(17, 19);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(75, 23);
            this.btnNew.TabIndex = 1;
            this.btnNew.TabStop = false;
            this.btnNew.Text = "New";
            this.btnNew.UseVisualStyleBackColor = true;
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
            // 
            // grpRecordNumber
            // 
            this.grpRecordNumber.Controls.Add(this.btnLastRecord);
            this.grpRecordNumber.Controls.Add(this.btnFirstRecord);
            this.grpRecordNumber.Controls.Add(this.btnRightArrow);
            this.grpRecordNumber.Controls.Add(this.btnArrowLeft);
            this.grpRecordNumber.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpRecordNumber.Location = new System.Drawing.Point(138, 427);
            this.grpRecordNumber.Name = "grpRecordNumber";
            this.grpRecordNumber.Size = new System.Drawing.Size(195, 100);
            this.grpRecordNumber.TabIndex = 29;
            this.grpRecordNumber.TabStop = false;
            this.grpRecordNumber.Text = "Record Number";
            // 
            // btnLastRecord
            // 
            this.btnLastRecord.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLastRecord.Location = new System.Drawing.Point(98, 62);
            this.btnLastRecord.Name = "btnLastRecord";
            this.btnLastRecord.Size = new System.Drawing.Size(89, 25);
            this.btnLastRecord.TabIndex = 2;
            this.btnLastRecord.TabStop = false;
            this.btnLastRecord.Text = "Last Record";
            this.btnLastRecord.UseVisualStyleBackColor = true;
            this.btnLastRecord.Click += new System.EventHandler(this.btnLastRecord_Click);
            // 
            // btnFirstRecord
            // 
            this.btnFirstRecord.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnFirstRecord.Location = new System.Drawing.Point(7, 62);
            this.btnFirstRecord.Name = "btnFirstRecord";
            this.btnFirstRecord.Size = new System.Drawing.Size(89, 25);
            this.btnFirstRecord.TabIndex = 1;
            this.btnFirstRecord.TabStop = false;
            this.btnFirstRecord.Text = "First Record";
            this.btnFirstRecord.UseVisualStyleBackColor = true;
            this.btnFirstRecord.Click += new System.EventHandler(this.btnFirstRecord_Click);
            // 
            // btnRightArrow
            // 
            this.btnRightArrow.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRightArrow.Location = new System.Drawing.Point(115, 25);
            this.btnRightArrow.Name = "btnRightArrow";
            this.btnRightArrow.Size = new System.Drawing.Size(55, 33);
            this.btnRightArrow.TabIndex = 0;
            this.btnRightArrow.TabStop = false;
            this.btnRightArrow.Text = ">";
            this.btnRightArrow.UseVisualStyleBackColor = true;
            this.btnRightArrow.Click += new System.EventHandler(this.btnRightArrow_Click);
            // 
            // btnArrowLeft
            // 
            this.btnArrowLeft.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnArrowLeft.Location = new System.Drawing.Point(24, 25);
            this.btnArrowLeft.Name = "btnArrowLeft";
            this.btnArrowLeft.Size = new System.Drawing.Size(55, 33);
            this.btnArrowLeft.TabIndex = 0;
            this.btnArrowLeft.TabStop = false;
            this.btnArrowLeft.Text = "<";
            this.btnArrowLeft.UseVisualStyleBackColor = true;
            this.btnArrowLeft.Click += new System.EventHandler(this.btnArrowLeft_Click);
            // 
            // grpStats
            // 
            this.grpStats.Controls.Add(this.btnStats);
            this.grpStats.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpStats.Location = new System.Drawing.Point(349, 427);
            this.grpStats.Name = "grpStats";
            this.grpStats.Size = new System.Drawing.Size(79, 100);
            this.grpStats.TabIndex = 29;
            this.grpStats.TabStop = false;
            this.grpStats.Text = "Stats";
            // 
            // btnStats
            // 
            this.btnStats.Location = new System.Drawing.Point(6, 41);
            this.btnStats.Name = "btnStats";
            this.btnStats.Size = new System.Drawing.Size(66, 34);
            this.btnStats.TabIndex = 0;
            this.btnStats.TabStop = false;
            this.btnStats.Text = "Stats";
            this.btnStats.UseVisualStyleBackColor = true;
            // 
            // grpSearchBy
            // 
            this.grpSearchBy.Controls.Add(this.btnLastName);
            this.grpSearchBy.Controls.Add(this.btnFirstName);
            this.grpSearchBy.Controls.Add(this.btnMemberNumber);
            this.grpSearchBy.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpSearchBy.Location = new System.Drawing.Point(439, 427);
            this.grpSearchBy.Name = "grpSearchBy";
            this.grpSearchBy.Size = new System.Drawing.Size(115, 111);
            this.grpSearchBy.TabIndex = 29;
            this.grpSearchBy.TabStop = false;
            this.grpSearchBy.Text = "Search by";
            // 
            // btnLastName
            // 
            this.btnLastName.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLastName.Location = new System.Drawing.Point(3, 75);
            this.btnLastName.Name = "btnLastName";
            this.btnLastName.Size = new System.Drawing.Size(109, 23);
            this.btnLastName.TabIndex = 30;
            this.btnLastName.TabStop = false;
            this.btnLastName.Text = "Last Name";
            this.btnLastName.UseVisualStyleBackColor = true;
            // 
            // btnFirstName
            // 
            this.btnFirstName.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnFirstName.Location = new System.Drawing.Point(3, 48);
            this.btnFirstName.Name = "btnFirstName";
            this.btnFirstName.Size = new System.Drawing.Size(109, 23);
            this.btnFirstName.TabIndex = 30;
            this.btnFirstName.TabStop = false;
            this.btnFirstName.Text = "First Name";
            this.btnFirstName.UseVisualStyleBackColor = true;
            // 
            // btnMemberNumber
            // 
            this.btnMemberNumber.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnMemberNumber.Location = new System.Drawing.Point(3, 19);
            this.btnMemberNumber.Name = "btnMemberNumber";
            this.btnMemberNumber.Size = new System.Drawing.Size(109, 23);
            this.btnMemberNumber.TabIndex = 30;
            this.btnMemberNumber.TabStop = false;
            this.btnMemberNumber.Text = "Member Number";
            this.btnMemberNumber.UseVisualStyleBackColor = true;
            // 
            // grpPrinter
            // 
            this.grpPrinter.Controls.Add(this.btnRecapByPin);
            this.grpPrinter.Controls.Add(this.btnLabels);
            this.grpPrinter.Controls.Add(this.btnRecapByDate);
            this.grpPrinter.Controls.Add(this.btnThisRecap);
            this.grpPrinter.Controls.Add(this.btnAllRecaps);
            this.grpPrinter.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpPrinter.Location = new System.Drawing.Point(560, 427);
            this.grpPrinter.Name = "grpPrinter";
            this.grpPrinter.Size = new System.Drawing.Size(207, 109);
            this.grpPrinter.TabIndex = 29;
            this.grpPrinter.TabStop = false;
            this.grpPrinter.Text = "Print";
            // 
            // btnRecapByPin
            // 
            this.btnRecapByPin.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRecapByPin.Location = new System.Drawing.Point(105, 49);
            this.btnRecapByPin.Name = "btnRecapByPin";
            this.btnRecapByPin.Size = new System.Drawing.Size(84, 23);
            this.btnRecapByPin.TabIndex = 31;
            this.btnRecapByPin.TabStop = false;
            this.btnRecapByPin.Text = "Recap By Pin";
            this.btnRecapByPin.UseVisualStyleBackColor = true;
            // 
            // btnLabels
            // 
            this.btnLabels.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLabels.Location = new System.Drawing.Point(18, 75);
            this.btnLabels.Name = "btnLabels";
            this.btnLabels.Size = new System.Drawing.Size(84, 23);
            this.btnLabels.TabIndex = 33;
            this.btnLabels.TabStop = false;
            this.btnLabels.Text = "Labels";
            this.btnLabels.UseVisualStyleBackColor = true;
            // 
            // btnRecapByDate
            // 
            this.btnRecapByDate.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRecapByDate.Location = new System.Drawing.Point(105, 23);
            this.btnRecapByDate.Name = "btnRecapByDate";
            this.btnRecapByDate.Size = new System.Drawing.Size(84, 23);
            this.btnRecapByDate.TabIndex = 30;
            this.btnRecapByDate.TabStop = false;
            this.btnRecapByDate.Text = "Recap Date";
            this.btnRecapByDate.UseVisualStyleBackColor = true;
            // 
            // btnThisRecap
            // 
            this.btnThisRecap.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnThisRecap.Location = new System.Drawing.Point(18, 23);
            this.btnThisRecap.Name = "btnThisRecap";
            this.btnThisRecap.Size = new System.Drawing.Size(84, 23);
            this.btnThisRecap.TabIndex = 34;
            this.btnThisRecap.TabStop = false;
            this.btnThisRecap.Text = "Print single";
            this.btnThisRecap.UseVisualStyleBackColor = true;
            // 
            // btnAllRecaps
            // 
            this.btnAllRecaps.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAllRecaps.Location = new System.Drawing.Point(18, 49);
            this.btnAllRecaps.Name = "btnAllRecaps";
            this.btnAllRecaps.Size = new System.Drawing.Size(84, 23);
            this.btnAllRecaps.TabIndex = 32;
            this.btnAllRecaps.TabStop = false;
            this.btnAllRecaps.Text = "All Recaps";
            this.btnAllRecaps.UseVisualStyleBackColor = true;
            // 
            // rdoFemale
            // 
            this.rdoFemale.AutoSize = true;
            this.rdoFemale.Location = new System.Drawing.Point(11, 19);
            this.rdoFemale.Name = "rdoFemale";
            this.rdoFemale.Size = new System.Drawing.Size(64, 19);
            this.rdoFemale.TabIndex = 30;
            this.rdoFemale.Text = "Female";
            this.rdoFemale.UseVisualStyleBackColor = true;
            // 
            // rdoActive
            // 
            this.rdoActive.AutoSize = true;
            this.rdoActive.Location = new System.Drawing.Point(11, 23);
            this.rdoActive.Name = "rdoActive";
            this.rdoActive.Size = new System.Drawing.Size(57, 19);
            this.rdoActive.TabIndex = 30;
            this.rdoActive.Text = "Active";
            this.rdoActive.UseVisualStyleBackColor = true;
            // 
            // rdoInActive
            // 
            this.rdoInActive.AutoSize = true;
            this.rdoInActive.Location = new System.Drawing.Point(11, 46);
            this.rdoInActive.Name = "rdoInActive";
            this.rdoInActive.Size = new System.Drawing.Size(68, 19);
            this.rdoInActive.TabIndex = 30;
            this.rdoInActive.Text = "InActive";
            this.rdoInActive.UseVisualStyleBackColor = true;
            // 
            // rdoMale
            // 
            this.rdoMale.AutoSize = true;
            this.rdoMale.Location = new System.Drawing.Point(11, 42);
            this.rdoMale.Name = "rdoMale";
            this.rdoMale.Size = new System.Drawing.Size(53, 19);
            this.rdoMale.TabIndex = 30;
            this.rdoMale.Text = "Male";
            this.rdoMale.UseVisualStyleBackColor = true;
            // 
            // grpGender
            // 
            this.grpGender.Controls.Add(this.rdoFemale);
            this.grpGender.Controls.Add(this.rdoMale);
            this.grpGender.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpGender.Location = new System.Drawing.Point(12, 258);
            this.grpGender.Name = "grpGender";
            this.grpGender.Size = new System.Drawing.Size(101, 71);
            this.grpGender.TabIndex = 31;
            this.grpGender.TabStop = false;
            this.grpGender.Text = "Gender";
            // 
            // grpStatus
            // 
            this.grpStatus.Controls.Add(this.rdoInActive);
            this.grpStatus.Controls.Add(this.rdoActive);
            this.grpStatus.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpStatus.Location = new System.Drawing.Point(12, 150);
            this.grpStatus.Name = "grpStatus";
            this.grpStatus.Size = new System.Drawing.Size(101, 79);
            this.grpStatus.TabIndex = 32;
            this.grpStatus.TabStop = false;
            this.grpStatus.Text = "Status";
            // 
            // chbSenior
            // 
            this.chbSenior.AutoSize = true;
            this.chbSenior.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chbSenior.Location = new System.Drawing.Point(23, 235);
            this.chbSenior.Name = "chbSenior";
            this.chbSenior.Size = new System.Drawing.Size(61, 19);
            this.chbSenior.TabIndex = 33;
            this.chbSenior.TabStop = false;
            this.chbSenior.Text = "Senior";
            this.chbSenior.UseVisualStyleBackColor = true;
            // 
            // dateLastBowled
            // 
            this.dateLastBowled.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dateLastBowled.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dateLastBowled.Location = new System.Drawing.Point(618, 403);
            this.dateLastBowled.Name = "dateLastBowled";
            this.dateLastBowled.Size = new System.Drawing.Size(103, 23);
            this.dateLastBowled.TabIndex = 34;
            this.dateLastBowled.CloseUp += new System.EventHandler(this.ApplyCalendarForm);
            this.dateLastBowled.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ClearCalendar);
            // 
            // FrmMemberData
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(779, 545);
            this.Controls.Add(this.dateLastBowled);
            this.Controls.Add(this.chbSenior);
            this.Controls.Add(this.grpStatus);
            this.Controls.Add(this.grpGender);
            this.Controls.Add(this.grpPrinter);
            this.Controls.Add(this.grpSearchBy);
            this.Controls.Add(this.grpStats);
            this.Controls.Add(this.grpRecordNumber);
            this.Controls.Add(this.groupRecord);
            this.Controls.Add(this.lblLastBowled);
            this.Controls.Add(this.lblMoneyEarned);
            this.Controls.Add(this.lblYET);
            this.Controls.Add(this.txtMoneyEarned);
            this.Controls.Add(this.txtYearEndT);
            this.Controls.Add(this.grpMemberInfo);
            this.Controls.Add(this.lblNotes);
            this.Controls.Add(this.txtNotes);
            this.Controls.Add(this.lblBonus);
            this.Controls.Add(this.lblHandicap);
            this.Controls.Add(this.lblAverage);
            this.Controls.Add(this.txtHandicap);
            this.Controls.Add(this.txtBonus);
            this.Controls.Add(this.txtAverage);
            this.Controls.Add(this.txtMemberNumber);
            this.Controls.Add(this.lblMemberNum);
            this.Controls.Add(this.lblMiddleIntial);
            this.Controls.Add(this.lblFirstName);
            this.Controls.Add(this.lblLastName);
            this.Controls.Add(this.txtMiddleInitial);
            this.Controls.Add(this.txtFirstName);
            this.Controls.Add(this.txtLastName);
            this.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FrmMemberData";
            this.Text = "Member Info";
            this.Load += new System.EventHandler(this.MemberDataForm_Load);
            this.grpMemberInfo.ResumeLayout(false);
            this.grpMemberInfo.PerformLayout();
            this.groupRecord.ResumeLayout(false);
            this.grpRecordNumber.ResumeLayout(false);
            this.grpStats.ResumeLayout(false);
            this.grpSearchBy.ResumeLayout(false);
            this.grpPrinter.ResumeLayout(false);
            this.grpGender.ResumeLayout(false);
            this.grpGender.PerformLayout();
            this.grpStatus.ResumeLayout(false);
            this.grpStatus.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtLastName;
        private System.Windows.Forms.TextBox txtFirstName;
        private System.Windows.Forms.TextBox txtMiddleInitial;
        private System.Windows.Forms.Label lblLastName;
        private System.Windows.Forms.Label lblFirstName;
        private System.Windows.Forms.Label lblMiddleIntial;
        private System.Windows.Forms.Label lblMemberNum;
        private System.Windows.Forms.TextBox txtMemberNumber;
        private System.Windows.Forms.TextBox txtBonus;
        private System.Windows.Forms.TextBox txtHandicap;
        private System.Windows.Forms.Label lblAverage;
        private System.Windows.Forms.Label lblHandicap;
        private System.Windows.Forms.Label lblBonus;
        private System.Windows.Forms.TextBox txtNotes;
        private System.Windows.Forms.TextBox txtAverage;
        private System.Windows.Forms.Label lblNotes;
        private System.Windows.Forms.GroupBox grpMemberInfo;
        private System.Windows.Forms.TextBox txtZip;
        private System.Windows.Forms.TextBox txtState;
        private System.Windows.Forms.TextBox txtCity;
        private System.Windows.Forms.TextBox txtEmail;
        private System.Windows.Forms.TextBox txtAddress;
        private System.Windows.Forms.Label lblAddress;
        private System.Windows.Forms.Label lblZip;
        private System.Windows.Forms.Label lblState;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtReferrals;
        private System.Windows.Forms.TextBox txtSSN;
        private System.Windows.Forms.TextBox txtPhoneNumber2;
        private System.Windows.Forms.TextBox txtPhoneNumber;
        private System.Windows.Forms.Label lblPhoneNumber2;
        private System.Windows.Forms.Label lblPhoneNumber;
        private System.Windows.Forms.Label lblRefferals;
        private System.Windows.Forms.Label lblDateJoined;
        private System.Windows.Forms.Label lblRejoinDate;
        private System.Windows.Forms.Label lblSSN;
        private System.Windows.Forms.TextBox txtYearEndT;
        private System.Windows.Forms.TextBox txtMoneyEarned;
        private System.Windows.Forms.Label lblYET;
        private System.Windows.Forms.Label lblMoneyEarned;
        private System.Windows.Forms.Label lblLastBowled;
        private System.Windows.Forms.GroupBox groupRecord;
        private System.Windows.Forms.GroupBox grpRecordNumber;
        private System.Windows.Forms.GroupBox grpStats;
        private System.Windows.Forms.GroupBox grpSearchBy;
        private System.Windows.Forms.GroupBox grpPrinter;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnNew;
        private System.Windows.Forms.Button btnLastRecord;
        private System.Windows.Forms.Button btnFirstRecord;
        private System.Windows.Forms.Button btnRightArrow;
        private System.Windows.Forms.Button btnArrowLeft;
        private System.Windows.Forms.Button btnStats;
        private System.Windows.Forms.Button btnLastName;
        private System.Windows.Forms.Button btnFirstName;
        private System.Windows.Forms.Button btnMemberNumber;
        private System.Windows.Forms.Button btnLabels;
        private System.Windows.Forms.Button btnRecapByDate;
        private System.Windows.Forms.Button btnThisRecap;
        private System.Windows.Forms.Button btnAllRecaps;
        private System.Windows.Forms.Button btnRecapByPin;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Label lblEmail;
        private System.Windows.Forms.RadioButton rdoFemale;
        private System.Windows.Forms.RadioButton rdoActive;
        private System.Windows.Forms.RadioButton rdoInActive;
        private System.Windows.Forms.RadioButton rdoMale;
        private System.Windows.Forms.Label lblDOB;
        private System.Windows.Forms.TextBox txtDOB;
        private System.Windows.Forms.GroupBox grpGender;
        private System.Windows.Forms.GroupBox grpStatus;
        private System.Windows.Forms.CheckBox chbSenior;
        private System.Windows.Forms.DateTimePicker dateRejoin;
        private System.Windows.Forms.DateTimePicker dateJoined;
        private System.Windows.Forms.DateTimePicker dateLastBowled;
    }
}

