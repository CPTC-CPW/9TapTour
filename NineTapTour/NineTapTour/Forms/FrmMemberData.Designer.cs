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
            this.lblTournAvg = new System.Windows.Forms.Label();
            this.lblBonus = new System.Windows.Forms.Label();
            this.txtNotes = new System.Windows.Forms.TextBox();
            this.txtAverage = new System.Windows.Forms.TextBox();
            this.lblNotes = new System.Windows.Forms.Label();
            this.grpMemberInfo = new System.Windows.Forms.GroupBox();
            this.chbSocial = new System.Windows.Forms.CheckBox();
            this.dateDOB = new System.Windows.Forms.DateTimePicker();
            this.mtxtBoxZip = new System.Windows.Forms.MaskedTextBox();
            this.mtxtBoxSSN = new System.Windows.Forms.MaskedTextBox();
            this.mtxtBoxPhone2 = new System.Windows.Forms.MaskedTextBox();
            this.mtxtBoxPhone = new System.Windows.Forms.MaskedTextBox();
            this.dateRejoin = new System.Windows.Forms.DateTimePicker();
            this.dateJoined = new System.Windows.Forms.DateTimePicker();
            this.lblEmail = new System.Windows.Forms.Label();
            this.lblDOB = new System.Windows.Forms.Label();
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
            this.txtState = new System.Windows.Forms.TextBox();
            this.txtCity = new System.Windows.Forms.TextBox();
            this.txtAddress = new System.Windows.Forms.TextBox();
            this.txtEmail = new System.Windows.Forms.TextBox();
            this.txtYearEndT = new System.Windows.Forms.TextBox();
            this.txtMoneyEarned = new System.Windows.Forms.TextBox();
            this.lblYET = new System.Windows.Forms.Label();
            this.lblMoneyEarned = new System.Windows.Forms.Label();
            this.lblLastBowled = new System.Windows.Forms.Label();
            this.groupRecord = new System.Windows.Forms.GroupBox();
            this.btnImportData = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnNew = new System.Windows.Forms.Button();
            this.grpRecordNumber = new System.Windows.Forms.GroupBox();
            this.btnLastRecord = new System.Windows.Forms.Button();
            this.btnFirstRecord = new System.Windows.Forms.Button();
            this.btnRightArrow = new System.Windows.Forms.Button();
            this.btnArrowLeft = new System.Windows.Forms.Button();
            this.grpStats = new System.Windows.Forms.GroupBox();
            this.btnStats = new System.Windows.Forms.Button();
            this.grpSearchBy = new System.Windows.Forms.GroupBox();
            this.btnMemberSearch = new System.Windows.Forms.Button();
            this.grpPrinter = new System.Windows.Forms.GroupBox();
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
            this.chbLifetime = new System.Windows.Forms.CheckBox();
            this.datePaid = new System.Windows.Forms.DateTimePicker();
            this.lblLastPaid = new System.Windows.Forms.Label();
            this.lblPaymentInfo = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtTournAvg = new System.Windows.Forms.TextBox();
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
            this.txtLastName.BackColor = System.Drawing.SystemColors.ControlLight;
            this.txtLastName.Font = new System.Drawing.Font("Calibri", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtLastName.Location = new System.Drawing.Point(226, 31);
            this.txtLastName.MaxLength = 50;
            this.txtLastName.Name = "txtLastName";
            this.txtLastName.Size = new System.Drawing.Size(226, 37);
            this.txtLastName.TabIndex = 3;
            this.txtLastName.TextChanged += new System.EventHandler(this.InputRequired);
            // 
            // txtFirstName
            // 
            this.txtFirstName.BackColor = System.Drawing.SystemColors.ControlLight;
            this.txtFirstName.Font = new System.Drawing.Font("Calibri", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtFirstName.Location = new System.Drawing.Point(477, 31);
            this.txtFirstName.MaxLength = 50;
            this.txtFirstName.Name = "txtFirstName";
            this.txtFirstName.Size = new System.Drawing.Size(232, 37);
            this.txtFirstName.TabIndex = 5;
            this.txtFirstName.TextChanged += new System.EventHandler(this.InputRequired);
            // 
            // txtMiddleInitial
            // 
            this.txtMiddleInitial.BackColor = System.Drawing.SystemColors.ControlLight;
            this.txtMiddleInitial.Font = new System.Drawing.Font("Calibri", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtMiddleInitial.Location = new System.Drawing.Point(731, 31);
            this.txtMiddleInitial.MaxLength = 10;
            this.txtMiddleInitial.Name = "txtMiddleInitial";
            this.txtMiddleInitial.Size = new System.Drawing.Size(119, 37);
            this.txtMiddleInitial.TabIndex = 7;
            this.txtMiddleInitial.TextChanged += new System.EventHandler(this.InputRequired);
            // 
            // lblLastName
            // 
            this.lblLastName.AutoSize = true;
            this.lblLastName.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLastName.Location = new System.Drawing.Point(222, 9);
            this.lblLastName.Name = "lblLastName";
            this.lblLastName.Size = new System.Drawing.Size(80, 19);
            this.lblLastName.TabIndex = 2;
            this.lblLastName.Text = "Last Name";
            // 
            // lblFirstName
            // 
            this.lblFirstName.AutoSize = true;
            this.lblFirstName.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFirstName.Location = new System.Drawing.Point(473, 9);
            this.lblFirstName.Name = "lblFirstName";
            this.lblFirstName.Size = new System.Drawing.Size(82, 19);
            this.lblFirstName.TabIndex = 4;
            this.lblFirstName.Text = "First Name";
            // 
            // lblMiddleIntial
            // 
            this.lblMiddleIntial.AutoSize = true;
            this.lblMiddleIntial.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMiddleIntial.Location = new System.Drawing.Point(727, 9);
            this.lblMiddleIntial.Name = "lblMiddleIntial";
            this.lblMiddleIntial.Size = new System.Drawing.Size(103, 19);
            this.lblMiddleIntial.TabIndex = 6;
            this.lblMiddleIntial.Text = "Middle Initial ";
            // 
            // lblMemberNum
            // 
            this.lblMemberNum.AutoSize = true;
            this.lblMemberNum.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMemberNum.Location = new System.Drawing.Point(34, 102);
            this.lblMemberNum.Name = "lblMemberNum";
            this.lblMemberNum.Size = new System.Drawing.Size(127, 19);
            this.lblMemberNum.TabIndex = 1;
            this.lblMemberNum.Text = "Member Number";
            // 
            // txtMemberNumber
            // 
            this.txtMemberNumber.Font = new System.Drawing.Font("Calibri", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtMemberNumber.Location = new System.Drawing.Point(12, 26);
            this.txtMemberNumber.Multiline = true;
            this.txtMemberNumber.Name = "txtMemberNumber";
            this.txtMemberNumber.ReadOnly = true;
            this.txtMemberNumber.Size = new System.Drawing.Size(186, 73);
            this.txtMemberNumber.TabIndex = 0;
            this.txtMemberNumber.TabStop = false;
            this.txtMemberNumber.Text = "1";
            this.txtMemberNumber.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtBonus
            // 
            this.txtBonus.BackColor = System.Drawing.SystemColors.ControlLight;
            this.txtBonus.Enabled = false;
            this.txtBonus.Font = new System.Drawing.Font("Calibri", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBonus.Location = new System.Drawing.Point(761, 438);
            this.txtBonus.MaxLength = 20;
            this.txtBonus.Name = "txtBonus";
            this.txtBonus.ReadOnly = true;
            this.txtBonus.Size = new System.Drawing.Size(89, 47);
            this.txtBonus.TabIndex = 40;
            this.txtBonus.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtHandicap
            // 
            this.txtHandicap.BackColor = System.Drawing.SystemColors.ControlLight;
            this.txtHandicap.Enabled = false;
            this.txtHandicap.Font = new System.Drawing.Font("Calibri", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtHandicap.Location = new System.Drawing.Point(638, 438);
            this.txtHandicap.MaxLength = 20;
            this.txtHandicap.Name = "txtHandicap";
            this.txtHandicap.ReadOnly = true;
            this.txtHandicap.Size = new System.Drawing.Size(89, 47);
            this.txtHandicap.TabIndex = 38;
            this.txtHandicap.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lblAverage
            // 
            this.lblAverage.AutoSize = true;
            this.lblAverage.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAverage.Location = new System.Drawing.Point(405, 420);
            this.lblAverage.Name = "lblAverage";
            this.lblAverage.Size = new System.Drawing.Size(50, 15);
            this.lblAverage.TabIndex = 33;
            this.lblAverage.Text = "Average\r\n";
            // 
            // lblTournAvg
            // 
            this.lblTournAvg.AutoSize = true;
            this.lblTournAvg.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTournAvg.Location = new System.Drawing.Point(649, 420);
            this.lblTournAvg.Name = "lblTournAvg";
            this.lblTournAvg.Size = new System.Drawing.Size(60, 15);
            this.lblTournAvg.TabIndex = 37;
            this.lblTournAvg.Text = "Handicap";
            // 
            // lblBonus
            // 
            this.lblBonus.AutoSize = true;
            this.lblBonus.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBonus.Location = new System.Drawing.Point(785, 420);
            this.lblBonus.Name = "lblBonus";
            this.lblBonus.Size = new System.Drawing.Size(41, 15);
            this.lblBonus.TabIndex = 39;
            this.lblBonus.Text = "Bonus";
            // 
            // txtNotes
            // 
            this.txtNotes.BackColor = System.Drawing.SystemColors.Control;
            this.txtNotes.Location = new System.Drawing.Point(38, 319);
            this.txtNotes.MaxLength = 750;
            this.txtNotes.Multiline = true;
            this.txtNotes.Name = "txtNotes";
            this.txtNotes.Size = new System.Drawing.Size(298, 218);
            this.txtNotes.TabIndex = 59;
            this.txtNotes.TabStop = false;
            // 
            // txtAverage
            // 
            this.txtAverage.BackColor = System.Drawing.SystemColors.Control;
            this.txtAverage.Font = new System.Drawing.Font("Calibri", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAverage.Location = new System.Drawing.Point(382, 438);
            this.txtAverage.MaxLength = 20;
            this.txtAverage.Name = "txtAverage";
            this.txtAverage.Size = new System.Drawing.Size(99, 47);
            this.txtAverage.TabIndex = 34;
            this.txtAverage.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lblNotes
            // 
            this.lblNotes.AutoSize = true;
            this.lblNotes.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNotes.Location = new System.Drawing.Point(36, 297);
            this.lblNotes.Name = "lblNotes";
            this.lblNotes.Size = new System.Drawing.Size(49, 19);
            this.lblNotes.TabIndex = 58;
            this.lblNotes.Text = "Notes";
            // 
            // grpMemberInfo
            // 
            this.grpMemberInfo.Controls.Add(this.chbSocial);
            this.grpMemberInfo.Controls.Add(this.dateDOB);
            this.grpMemberInfo.Controls.Add(this.mtxtBoxZip);
            this.grpMemberInfo.Controls.Add(this.mtxtBoxSSN);
            this.grpMemberInfo.Controls.Add(this.mtxtBoxPhone2);
            this.grpMemberInfo.Controls.Add(this.mtxtBoxPhone);
            this.grpMemberInfo.Controls.Add(this.dateRejoin);
            this.grpMemberInfo.Controls.Add(this.dateJoined);
            this.grpMemberInfo.Controls.Add(this.lblEmail);
            this.grpMemberInfo.Controls.Add(this.lblDOB);
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
            this.grpMemberInfo.Controls.Add(this.txtState);
            this.grpMemberInfo.Controls.Add(this.txtCity);
            this.grpMemberInfo.Controls.Add(this.txtAddress);
            this.grpMemberInfo.Controls.Add(this.txtEmail);
            this.grpMemberInfo.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpMemberInfo.Location = new System.Drawing.Point(382, 121);
            this.grpMemberInfo.Name = "grpMemberInfo";
            this.grpMemberInfo.Size = new System.Drawing.Size(469, 277);
            this.grpMemberInfo.TabIndex = 8;
            this.grpMemberInfo.TabStop = false;
            this.grpMemberInfo.Text = "Member Information";
            // 
            // chbSocial
            // 
            this.chbSocial.AutoSize = true;
            this.chbSocial.Location = new System.Drawing.Point(323, 45);
            this.chbSocial.Name = "chbSocial";
            this.chbSocial.Size = new System.Drawing.Size(15, 14);
            this.chbSocial.TabIndex = 33;
            this.chbSocial.UseVisualStyleBackColor = true;
            this.chbSocial.CheckedChanged += new System.EventHandler(this.chbSocial_CheckedChanged);
            // 
            // dateDOB
            // 
            this.dateDOB.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dateDOB.Location = new System.Drawing.Point(25, 41);
            this.dateDOB.Name = "dateDOB";
            this.dateDOB.Size = new System.Drawing.Size(104, 23);
            this.dateDOB.TabIndex = 10;
            this.dateDOB.ValueChanged += new System.EventHandler(this.dateDOB_ValueChanged);
            // 
            // mtxtBoxZip
            // 
            this.mtxtBoxZip.BackColor = System.Drawing.SystemColors.Control;
            this.mtxtBoxZip.HideSelection = false;
            this.mtxtBoxZip.Location = new System.Drawing.Point(215, 137);
            this.mtxtBoxZip.Mask = "00000";
            this.mtxtBoxZip.Name = "mtxtBoxZip";
            this.mtxtBoxZip.ShortcutsEnabled = false;
            this.mtxtBoxZip.Size = new System.Drawing.Size(100, 23);
            this.mtxtBoxZip.TabIndex = 20;
            // 
            // mtxtBoxSSN
            // 
            this.mtxtBoxSSN.BackColor = System.Drawing.SystemColors.Control;
            this.mtxtBoxSSN.Location = new System.Drawing.Point(344, 41);
            this.mtxtBoxSSN.Mask = "000-00-0000";
            this.mtxtBoxSSN.Name = "mtxtBoxSSN";
            this.mtxtBoxSSN.PasswordChar = '*';
            this.mtxtBoxSSN.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.mtxtBoxSSN.Size = new System.Drawing.Size(100, 23);
            this.mtxtBoxSSN.TabIndex = 12;
            // 
            // mtxtBoxPhone2
            // 
            this.mtxtBoxPhone2.BackColor = System.Drawing.SystemColors.Control;
            this.mtxtBoxPhone2.Location = new System.Drawing.Point(155, 185);
            this.mtxtBoxPhone2.Mask = "(999) 000-0000";
            this.mtxtBoxPhone2.Name = "mtxtBoxPhone2";
            this.mtxtBoxPhone2.Size = new System.Drawing.Size(100, 23);
            this.mtxtBoxPhone2.TabIndex = 24;
            // 
            // mtxtBoxPhone
            // 
            this.mtxtBoxPhone.BackColor = System.Drawing.SystemColors.Control;
            this.mtxtBoxPhone.Location = new System.Drawing.Point(25, 185);
            this.mtxtBoxPhone.Mask = "(999) 000-0000";
            this.mtxtBoxPhone.Name = "mtxtBoxPhone";
            this.mtxtBoxPhone.Size = new System.Drawing.Size(100, 23);
            this.mtxtBoxPhone.TabIndex = 22;
            // 
            // dateRejoin
            // 
            this.dateRejoin.Checked = false;
            this.dateRejoin.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dateRejoin.Location = new System.Drawing.Point(344, 185);
            this.dateRejoin.Name = "dateRejoin";
            this.dateRejoin.Size = new System.Drawing.Size(103, 23);
            this.dateRejoin.TabIndex = 30;
            this.dateRejoin.CloseUp += new System.EventHandler(this.ApplyCalendarForm);
            this.dateRejoin.ValueChanged += new System.EventHandler(this.dateRejoin_ValueChanged);
            this.dateRejoin.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ClearCalendar);
            // 
            // dateJoined
            // 
            this.dateJoined.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dateJoined.Location = new System.Drawing.Point(344, 137);
            this.dateJoined.Name = "dateJoined";
            this.dateJoined.Size = new System.Drawing.Size(103, 23);
            this.dateJoined.TabIndex = 28;
            this.dateJoined.CloseUp += new System.EventHandler(this.ApplyCalendarForm);
            this.dateJoined.ValueChanged += new System.EventHandler(this.dateJoined_ValueChanged);
            // 
            // lblEmail
            // 
            this.lblEmail.AutoSize = true;
            this.lblEmail.Location = new System.Drawing.Point(22, 215);
            this.lblEmail.Name = "lblEmail";
            this.lblEmail.Size = new System.Drawing.Size(38, 15);
            this.lblEmail.TabIndex = 25;
            this.lblEmail.Text = "Email";
            // 
            // lblDOB
            // 
            this.lblDOB.AutoSize = true;
            this.lblDOB.Location = new System.Drawing.Point(22, 23);
            this.lblDOB.Name = "lblDOB";
            this.lblDOB.Size = new System.Drawing.Size(107, 15);
            this.lblDOB.TabIndex = 9;
            this.lblDOB.Text = "D.O.B. (mm/dd/yy)";
            // 
            // lblRefferals
            // 
            this.lblRefferals.AutoSize = true;
            this.lblRefferals.Location = new System.Drawing.Point(341, 215);
            this.lblRefferals.Name = "lblRefferals";
            this.lblRefferals.Size = new System.Drawing.Size(57, 15);
            this.lblRefferals.TabIndex = 31;
            this.lblRefferals.Text = "Referrals";
            // 
            // lblPhoneNumber2
            // 
            this.lblPhoneNumber2.AutoSize = true;
            this.lblPhoneNumber2.Location = new System.Drawing.Point(152, 167);
            this.lblPhoneNumber2.Name = "lblPhoneNumber2";
            this.lblPhoneNumber2.Size = new System.Drawing.Size(97, 15);
            this.lblPhoneNumber2.TabIndex = 23;
            this.lblPhoneNumber2.Text = "Phone Number 2";
            // 
            // lblPhoneNumber
            // 
            this.lblPhoneNumber.AutoSize = true;
            this.lblPhoneNumber.Location = new System.Drawing.Point(22, 167);
            this.lblPhoneNumber.Name = "lblPhoneNumber";
            this.lblPhoneNumber.Size = new System.Drawing.Size(87, 15);
            this.lblPhoneNumber.TabIndex = 21;
            this.lblPhoneNumber.Text = "Phone Number";
            // 
            // lblDateJoined
            // 
            this.lblDateJoined.AutoSize = true;
            this.lblDateJoined.Location = new System.Drawing.Point(341, 119);
            this.lblDateJoined.Name = "lblDateJoined";
            this.lblDateJoined.Size = new System.Drawing.Size(70, 15);
            this.lblDateJoined.TabIndex = 27;
            this.lblDateJoined.Text = "Date Joined";
            // 
            // lblRejoinDate
            // 
            this.lblRejoinDate.AutoSize = true;
            this.lblRejoinDate.Location = new System.Drawing.Point(341, 167);
            this.lblRejoinDate.Name = "lblRejoinDate";
            this.lblRejoinDate.Size = new System.Drawing.Size(70, 15);
            this.lblRejoinDate.TabIndex = 29;
            this.lblRejoinDate.Text = "Rejoin Date";
            // 
            // lblZip
            // 
            this.lblZip.AutoSize = true;
            this.lblZip.Location = new System.Drawing.Point(212, 119);
            this.lblZip.Name = "lblZip";
            this.lblZip.Size = new System.Drawing.Size(24, 15);
            this.lblZip.TabIndex = 19;
            this.lblZip.Text = "Zip";
            // 
            // lblState
            // 
            this.lblState.AutoSize = true;
            this.lblState.Location = new System.Drawing.Point(152, 119);
            this.lblState.Name = "lblState";
            this.lblState.Size = new System.Drawing.Size(34, 15);
            this.lblState.TabIndex = 17;
            this.lblState.Text = "State";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 119);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(28, 15);
            this.label1.TabIndex = 15;
            this.label1.Text = "City";
            // 
            // txtReferrals
            // 
            this.txtReferrals.BackColor = System.Drawing.SystemColors.Control;
            this.txtReferrals.Location = new System.Drawing.Point(344, 233);
            this.txtReferrals.MaxLength = 50;
            this.txtReferrals.Name = "txtReferrals";
            this.txtReferrals.Size = new System.Drawing.Size(54, 23);
            this.txtReferrals.TabIndex = 32;
            // 
            // lblAddress
            // 
            this.lblAddress.AutoSize = true;
            this.lblAddress.Location = new System.Drawing.Point(22, 71);
            this.lblAddress.Name = "lblAddress";
            this.lblAddress.Size = new System.Drawing.Size(51, 15);
            this.lblAddress.TabIndex = 13;
            this.lblAddress.Text = "Address";
            // 
            // lblSSN
            // 
            this.lblSSN.AutoSize = true;
            this.lblSSN.Location = new System.Drawing.Point(341, 23);
            this.lblSSN.Name = "lblSSN";
            this.lblSSN.Size = new System.Drawing.Size(27, 15);
            this.lblSSN.TabIndex = 11;
            this.lblSSN.Text = "SSN";
            // 
            // txtState
            // 
            this.txtState.BackColor = System.Drawing.SystemColors.Control;
            this.txtState.Location = new System.Drawing.Point(155, 137);
            this.txtState.MaxLength = 2;
            this.txtState.Name = "txtState";
            this.txtState.Size = new System.Drawing.Size(32, 23);
            this.txtState.TabIndex = 18;
            this.txtState.TextChanged += new System.EventHandler(this.InputRequired);
            // 
            // txtCity
            // 
            this.txtCity.BackColor = System.Drawing.SystemColors.Control;
            this.txtCity.Location = new System.Drawing.Point(25, 137);
            this.txtCity.MaxLength = 20;
            this.txtCity.Name = "txtCity";
            this.txtCity.Size = new System.Drawing.Size(100, 23);
            this.txtCity.TabIndex = 16;
            this.txtCity.TextChanged += new System.EventHandler(this.InputRequired);
            // 
            // txtAddress
            // 
            this.txtAddress.BackColor = System.Drawing.SystemColors.Control;
            this.txtAddress.Location = new System.Drawing.Point(25, 89);
            this.txtAddress.MaxLength = 75;
            this.txtAddress.Name = "txtAddress";
            this.txtAddress.Size = new System.Drawing.Size(290, 23);
            this.txtAddress.TabIndex = 14;
            this.txtAddress.TextChanged += new System.EventHandler(this.InputRequired);
            // 
            // txtEmail
            // 
            this.txtEmail.BackColor = System.Drawing.SystemColors.Control;
            this.txtEmail.Location = new System.Drawing.Point(25, 233);
            this.txtEmail.MaxLength = 50;
            this.txtEmail.Name = "txtEmail";
            this.txtEmail.Size = new System.Drawing.Size(290, 23);
            this.txtEmail.TabIndex = 26;
            // 
            // txtYearEndT
            // 
            this.txtYearEndT.BackColor = System.Drawing.SystemColors.Control;
            this.txtYearEndT.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtYearEndT.Location = new System.Drawing.Point(392, 517);
            this.txtYearEndT.MaxLength = 20;
            this.txtYearEndT.Name = "txtYearEndT";
            this.txtYearEndT.Size = new System.Drawing.Size(124, 23);
            this.txtYearEndT.TabIndex = 42;
            // 
            // txtMoneyEarned
            // 
            this.txtMoneyEarned.BackColor = System.Drawing.SystemColors.Control;
            this.txtMoneyEarned.Enabled = false;
            this.txtMoneyEarned.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtMoneyEarned.Location = new System.Drawing.Point(573, 517);
            this.txtMoneyEarned.Name = "txtMoneyEarned";
            this.txtMoneyEarned.Size = new System.Drawing.Size(100, 23);
            this.txtMoneyEarned.TabIndex = 44;
            this.txtMoneyEarned.TabStop = false;
            // 
            // lblYET
            // 
            this.lblYET.AutoSize = true;
            this.lblYET.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblYET.Location = new System.Drawing.Point(389, 499);
            this.lblYET.Name = "lblYET";
            this.lblYET.Size = new System.Drawing.Size(127, 15);
            this.lblYET.TabIndex = 41;
            this.lblYET.Text = "Year End Tournaments";
            // 
            // lblMoneyEarned
            // 
            this.lblMoneyEarned.AutoSize = true;
            this.lblMoneyEarned.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMoneyEarned.Location = new System.Drawing.Point(570, 499);
            this.lblMoneyEarned.Name = "lblMoneyEarned";
            this.lblMoneyEarned.Size = new System.Drawing.Size(85, 15);
            this.lblMoneyEarned.TabIndex = 43;
            this.lblMoneyEarned.Text = "Money Earned";
            // 
            // lblLastBowled
            // 
            this.lblLastBowled.AutoSize = true;
            this.lblLastBowled.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLastBowled.Location = new System.Drawing.Point(744, 499);
            this.lblLastBowled.Name = "lblLastBowled";
            this.lblLastBowled.Size = new System.Drawing.Size(72, 15);
            this.lblLastBowled.TabIndex = 45;
            this.lblLastBowled.Text = "Last Bowled";
            // 
            // groupRecord
            // 
            this.groupRecord.Controls.Add(this.btnImportData);
            this.groupRecord.Controls.Add(this.btnSave);
            this.groupRecord.Controls.Add(this.btnClear);
            this.groupRecord.Controls.Add(this.btnNew);
            this.groupRecord.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupRecord.Location = new System.Drawing.Point(23, 583);
            this.groupRecord.Name = "groupRecord";
            this.groupRecord.Size = new System.Drawing.Size(108, 126);
            this.groupRecord.TabIndex = 60;
            this.groupRecord.TabStop = false;
            this.groupRecord.Text = "Record";
            // 
            // btnImportData
            // 
            this.btnImportData.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnImportData.Location = new System.Drawing.Point(17, 97);
            this.btnImportData.Name = "btnImportData";
            this.btnImportData.Size = new System.Drawing.Size(75, 23);
            this.btnImportData.TabIndex = 64;
            this.btnImportData.TabStop = false;
            this.btnImportData.Text = "Import";
            this.btnImportData.UseVisualStyleBackColor = true;
            this.btnImportData.Click += new System.EventHandler(this.btnImportData_Click);
            // 
            // btnSave
            // 
            this.btnSave.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSave.Location = new System.Drawing.Point(17, 45);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 62;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnClear
            // 
            this.btnClear.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClear.Location = new System.Drawing.Point(17, 71);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(75, 23);
            this.btnClear.TabIndex = 63;
            this.btnClear.TabStop = false;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnNew
            // 
            this.btnNew.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnNew.Location = new System.Drawing.Point(17, 19);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(75, 23);
            this.btnNew.TabIndex = 61;
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
            this.grpRecordNumber.Location = new System.Drawing.Point(149, 583);
            this.grpRecordNumber.Name = "grpRecordNumber";
            this.grpRecordNumber.Size = new System.Drawing.Size(195, 100);
            this.grpRecordNumber.TabIndex = 64;
            this.grpRecordNumber.TabStop = false;
            this.grpRecordNumber.Text = "Record Number";
            // 
            // btnLastRecord
            // 
            this.btnLastRecord.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLastRecord.Location = new System.Drawing.Point(98, 62);
            this.btnLastRecord.Name = "btnLastRecord";
            this.btnLastRecord.Size = new System.Drawing.Size(89, 25);
            this.btnLastRecord.TabIndex = 68;
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
            this.btnFirstRecord.TabIndex = 67;
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
            this.btnRightArrow.TabIndex = 66;
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
            this.btnArrowLeft.TabIndex = 65;
            this.btnArrowLeft.TabStop = false;
            this.btnArrowLeft.Text = "<";
            this.btnArrowLeft.UseVisualStyleBackColor = true;
            this.btnArrowLeft.Click += new System.EventHandler(this.btnArrowLeft_Click);
            // 
            // grpStats
            // 
            this.grpStats.Controls.Add(this.btnStats);
            this.grpStats.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpStats.Location = new System.Drawing.Point(376, 583);
            this.grpStats.Name = "grpStats";
            this.grpStats.Size = new System.Drawing.Size(79, 94);
            this.grpStats.TabIndex = 69;
            this.grpStats.TabStop = false;
            this.grpStats.Text = "Stats";
            // 
            // btnStats
            // 
            this.btnStats.Location = new System.Drawing.Point(6, 41);
            this.btnStats.Name = "btnStats";
            this.btnStats.Size = new System.Drawing.Size(66, 34);
            this.btnStats.TabIndex = 70;
            this.btnStats.TabStop = false;
            this.btnStats.Text = "Stats";
            this.btnStats.UseVisualStyleBackColor = true;
            this.btnStats.Click += new System.EventHandler(this.btnStats_Click);
            // 
            // grpSearchBy
            // 
            this.grpSearchBy.Controls.Add(this.btnMemberSearch);
            this.grpSearchBy.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpSearchBy.Location = new System.Drawing.Point(496, 583);
            this.grpSearchBy.Name = "grpSearchBy";
            this.grpSearchBy.Size = new System.Drawing.Size(115, 100);
            this.grpSearchBy.TabIndex = 71;
            this.grpSearchBy.TabStop = false;
            this.grpSearchBy.Text = "Search";
            // 
            // btnMemberSearch
            // 
            this.btnMemberSearch.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnMemberSearch.Location = new System.Drawing.Point(0, 33);
            this.btnMemberSearch.Name = "btnMemberSearch";
            this.btnMemberSearch.Size = new System.Drawing.Size(109, 52);
            this.btnMemberSearch.TabIndex = 72;
            this.btnMemberSearch.TabStop = false;
            this.btnMemberSearch.Text = "Member Search";
            this.btnMemberSearch.UseVisualStyleBackColor = true;
            this.btnMemberSearch.Click += new System.EventHandler(this.btnMemberSearch_Click);
            // 
            // grpPrinter
            // 
            this.grpPrinter.Controls.Add(this.btnLabels);
            this.grpPrinter.Controls.Add(this.btnRecapByDate);
            this.grpPrinter.Controls.Add(this.btnThisRecap);
            this.grpPrinter.Controls.Add(this.btnAllRecaps);
            this.grpPrinter.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpPrinter.Location = new System.Drawing.Point(643, 585);
            this.grpPrinter.Name = "grpPrinter";
            this.grpPrinter.Size = new System.Drawing.Size(229, 85);
            this.grpPrinter.TabIndex = 73;
            this.grpPrinter.TabStop = false;
            this.grpPrinter.Text = "Print Recaps";
            // 
            // btnLabels
            // 
            this.btnLabels.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLabels.Location = new System.Drawing.Point(105, 49);
            this.btnLabels.Name = "btnLabels";
            this.btnLabels.Size = new System.Drawing.Size(102, 23);
            this.btnLabels.TabIndex = 76;
            this.btnLabels.TabStop = false;
            this.btnLabels.Text = "Labels";
            this.btnLabels.UseVisualStyleBackColor = true;
            this.btnLabels.Click += new System.EventHandler(this.btnLabels_Click);
            // 
            // btnRecapByDate
            // 
            this.btnRecapByDate.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRecapByDate.Location = new System.Drawing.Point(105, 23);
            this.btnRecapByDate.Name = "btnRecapByDate";
            this.btnRecapByDate.Size = new System.Drawing.Size(102, 23);
            this.btnRecapByDate.TabIndex = 75;
            this.btnRecapByDate.TabStop = false;
            this.btnRecapByDate.Text = "Print By Date";
            this.btnRecapByDate.UseVisualStyleBackColor = true;
            this.btnRecapByDate.Click += new System.EventHandler(this.btnRecapByDate_Click);
            // 
            // btnThisRecap
            // 
            this.btnThisRecap.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnThisRecap.Location = new System.Drawing.Point(18, 23);
            this.btnThisRecap.Name = "btnThisRecap";
            this.btnThisRecap.Size = new System.Drawing.Size(84, 23);
            this.btnThisRecap.TabIndex = 74;
            this.btnThisRecap.TabStop = false;
            this.btnThisRecap.Text = "Print single";
            this.btnThisRecap.UseVisualStyleBackColor = true;
            this.btnThisRecap.Click += new System.EventHandler(this.btnThisRecap_Click);
            // 
            // btnAllRecaps
            // 
            this.btnAllRecaps.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAllRecaps.Location = new System.Drawing.Point(18, 49);
            this.btnAllRecaps.Name = "btnAllRecaps";
            this.btnAllRecaps.Size = new System.Drawing.Size(84, 23);
            this.btnAllRecaps.TabIndex = 75;
            this.btnAllRecaps.TabStop = false;
            this.btnAllRecaps.Text = "All Recaps";
            this.btnAllRecaps.UseVisualStyleBackColor = true;
            this.btnAllRecaps.Click += new System.EventHandler(this.btnAllRecaps_Click);
            // 
            // rdoFemale
            // 
            this.rdoFemale.AutoSize = true;
            this.rdoFemale.Location = new System.Drawing.Point(11, 19);
            this.rdoFemale.Name = "rdoFemale";
            this.rdoFemale.Size = new System.Drawing.Size(64, 19);
            this.rdoFemale.TabIndex = 52;
            this.rdoFemale.Text = "Female";
            this.rdoFemale.UseVisualStyleBackColor = true;
            // 
            // rdoActive
            // 
            this.rdoActive.AutoSize = true;
            this.rdoActive.Location = new System.Drawing.Point(11, 23);
            this.rdoActive.Name = "rdoActive";
            this.rdoActive.Size = new System.Drawing.Size(57, 19);
            this.rdoActive.TabIndex = 48;
            this.rdoActive.Text = "Active";
            this.rdoActive.UseVisualStyleBackColor = true;
            // 
            // rdoInActive
            // 
            this.rdoInActive.AutoSize = true;
            this.rdoInActive.Location = new System.Drawing.Point(11, 46);
            this.rdoInActive.Name = "rdoInActive";
            this.rdoInActive.Size = new System.Drawing.Size(68, 19);
            this.rdoInActive.TabIndex = 49;
            this.rdoInActive.Text = "InActive";
            this.rdoInActive.UseVisualStyleBackColor = true;
            // 
            // rdoMale
            // 
            this.rdoMale.AutoSize = true;
            this.rdoMale.Location = new System.Drawing.Point(11, 42);
            this.rdoMale.Name = "rdoMale";
            this.rdoMale.Size = new System.Drawing.Size(53, 19);
            this.rdoMale.TabIndex = 53;
            this.rdoMale.Text = "Male";
            this.rdoMale.UseVisualStyleBackColor = true;
            // 
            // grpGender
            // 
            this.grpGender.Controls.Add(this.rdoFemale);
            this.grpGender.Controls.Add(this.rdoMale);
            this.grpGender.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpGender.Location = new System.Drawing.Point(235, 128);
            this.grpGender.Name = "grpGender";
            this.grpGender.Size = new System.Drawing.Size(101, 71);
            this.grpGender.TabIndex = 51;
            this.grpGender.TabStop = false;
            this.grpGender.Text = "Gender";
            // 
            // grpStatus
            // 
            this.grpStatus.Controls.Add(this.rdoInActive);
            this.grpStatus.Controls.Add(this.rdoActive);
            this.grpStatus.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpStatus.Location = new System.Drawing.Point(38, 128);
            this.grpStatus.Name = "grpStatus";
            this.grpStatus.Size = new System.Drawing.Size(101, 79);
            this.grpStatus.TabIndex = 47;
            this.grpStatus.TabStop = false;
            this.grpStatus.Text = "Status";
            // 
            // chbSenior
            // 
            this.chbSenior.AutoSize = true;
            this.chbSenior.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chbSenior.Location = new System.Drawing.Point(156, 162);
            this.chbSenior.Name = "chbSenior";
            this.chbSenior.Size = new System.Drawing.Size(61, 19);
            this.chbSenior.TabIndex = 50;
            this.chbSenior.TabStop = false;
            this.chbSenior.Text = "Senior";
            this.chbSenior.UseVisualStyleBackColor = true;
            // 
            // dateLastBowled
            // 
            this.dateLastBowled.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dateLastBowled.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dateLastBowled.Location = new System.Drawing.Point(747, 517);
            this.dateLastBowled.Name = "dateLastBowled";
            this.dateLastBowled.Size = new System.Drawing.Size(103, 23);
            this.dateLastBowled.TabIndex = 46;
            this.dateLastBowled.TabStop = false;
            this.dateLastBowled.CloseUp += new System.EventHandler(this.ApplyCalendarForm);
            this.dateLastBowled.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ClearCalendar);
            // 
            // chbLifetime
            // 
            this.chbLifetime.AutoSize = true;
            this.chbLifetime.Font = new System.Drawing.Font("Calibri", 9.75F);
            this.chbLifetime.Location = new System.Drawing.Point(38, 216);
            this.chbLifetime.Name = "chbLifetime";
            this.chbLifetime.Size = new System.Drawing.Size(116, 19);
            this.chbLifetime.TabIndex = 54;
            this.chbLifetime.Text = "Lifetime Member";
            this.chbLifetime.UseVisualStyleBackColor = true;
            this.chbLifetime.CheckedChanged += new System.EventHandler(this.chbLifetime_CheckedChanged);
            // 
            // datePaid
            // 
            this.datePaid.Location = new System.Drawing.Point(146, 241);
            this.datePaid.Name = "datePaid";
            this.datePaid.Size = new System.Drawing.Size(190, 21);
            this.datePaid.TabIndex = 56;
            this.datePaid.ValueChanged += new System.EventHandler(this.datePaid_ValueChanged);
            // 
            // lblLastPaid
            // 
            this.lblLastPaid.AutoSize = true;
            this.lblLastPaid.Font = new System.Drawing.Font("Calibri", 9.75F);
            this.lblLastPaid.Location = new System.Drawing.Point(35, 245);
            this.lblLastPaid.Name = "lblLastPaid";
            this.lblLastPaid.Size = new System.Drawing.Size(79, 15);
            this.lblLastPaid.TabIndex = 55;
            this.lblLastPaid.Text = "Last Payment";
            // 
            // lblPaymentInfo
            // 
            this.lblPaymentInfo.AutoSize = true;
            this.lblPaymentInfo.Font = new System.Drawing.Font("Calibri", 9.75F);
            this.lblPaymentInfo.ForeColor = System.Drawing.Color.Red;
            this.lblPaymentInfo.Location = new System.Drawing.Point(35, 266);
            this.lblPaymentInfo.Name = "lblPaymentInfo";
            this.lblPaymentInfo.Size = new System.Drawing.Size(129, 15);
            this.lblPaymentInfo.TabIndex = 57;
            this.lblPaymentInfo.Text = "Yearly Payment is due.";
            this.lblPaymentInfo.Visible = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(510, 420);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(101, 15);
            this.label2.TabIndex = 35;
            this.label2.Text = "30 Game Average";
            // 
            // txtTournAvg
            // 
            this.txtTournAvg.BackColor = System.Drawing.SystemColors.ControlLight;
            this.txtTournAvg.Enabled = false;
            this.txtTournAvg.Font = new System.Drawing.Font("Calibri", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtTournAvg.Location = new System.Drawing.Point(515, 438);
            this.txtTournAvg.MaxLength = 20;
            this.txtTournAvg.Name = "txtTournAvg";
            this.txtTournAvg.ReadOnly = true;
            this.txtTournAvg.Size = new System.Drawing.Size(89, 47);
            this.txtTournAvg.TabIndex = 36;
            this.txtTournAvg.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // FrmMemberData
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoScrollMinSize = new System.Drawing.Size(100, 100);
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(959, 741);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lblPaymentInfo);
            this.Controls.Add(this.lblLastPaid);
            this.Controls.Add(this.txtTournAvg);
            this.Controls.Add(this.chbLifetime);
            this.Controls.Add(this.datePaid);
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
            this.Controls.Add(this.lblTournAvg);
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
            this.HelpButton = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "FrmMemberData";
            this.Text = "Member Info";
            this.Load += new System.EventHandler(this.MemberDataForm_Load);
            this.Leave += new System.EventHandler(this.FrmMemberData_Leave);
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
        private System.Windows.Forms.Label lblTournAvg;
        private System.Windows.Forms.Label lblBonus;
        private System.Windows.Forms.TextBox txtNotes;
        private System.Windows.Forms.TextBox txtAverage;
        private System.Windows.Forms.Label lblNotes;
        private System.Windows.Forms.GroupBox grpMemberInfo;
        private System.Windows.Forms.TextBox txtState;
        private System.Windows.Forms.TextBox txtCity;
        private System.Windows.Forms.TextBox txtEmail;
        private System.Windows.Forms.TextBox txtAddress;
        private System.Windows.Forms.Label lblAddress;
        private System.Windows.Forms.Label lblZip;
        private System.Windows.Forms.Label lblState;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtReferrals;
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
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnNew;
        private System.Windows.Forms.Button btnLastRecord;
        private System.Windows.Forms.Button btnFirstRecord;
        private System.Windows.Forms.Button btnRightArrow;
        private System.Windows.Forms.Button btnArrowLeft;
        private System.Windows.Forms.Button btnStats;
        private System.Windows.Forms.Button btnMemberSearch;
        private System.Windows.Forms.Button btnLabels;
        private System.Windows.Forms.Button btnRecapByDate;
        private System.Windows.Forms.Button btnThisRecap;
        private System.Windows.Forms.Button btnAllRecaps;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Label lblEmail;
        private System.Windows.Forms.RadioButton rdoFemale;
        private System.Windows.Forms.RadioButton rdoActive;
        private System.Windows.Forms.RadioButton rdoInActive;
        private System.Windows.Forms.RadioButton rdoMale;
        private System.Windows.Forms.Label lblDOB;
        private System.Windows.Forms.GroupBox grpGender;
        private System.Windows.Forms.GroupBox grpStatus;
        private System.Windows.Forms.CheckBox chbSenior;
        private System.Windows.Forms.DateTimePicker dateRejoin;
        private System.Windows.Forms.DateTimePicker dateJoined;
        private System.Windows.Forms.DateTimePicker dateLastBowled;
        private System.Windows.Forms.MaskedTextBox mtxtBoxPhone;
        private System.Windows.Forms.MaskedTextBox mtxtBoxPhone2;
        private System.Windows.Forms.MaskedTextBox mtxtBoxSSN;
        private System.Windows.Forms.MaskedTextBox mtxtBoxZip;
        private System.Windows.Forms.CheckBox chbLifetime;
        private System.Windows.Forms.DateTimePicker datePaid;
        private System.Windows.Forms.Label lblLastPaid;
        private System.Windows.Forms.Label lblPaymentInfo;
        private System.Windows.Forms.DateTimePicker dateDOB;
        private System.Windows.Forms.TextBox txtTournAvg;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnImportData;
        private System.Windows.Forms.OpenFileDialog ofdOpen;
        private System.Windows.Forms.CheckBox chbSocial;
    }
}
