namespace Member_Import_Test
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
            this.txtrejoinDate = new System.Windows.Forms.TextBox();
            this.txtdateJoined = new System.Windows.Forms.TextBox();
            this.mtxtBoxDOB = new System.Windows.Forms.MaskedTextBox();
            this.mtxtBoxZip = new System.Windows.Forms.MaskedTextBox();
            this.mtxtBoxSSN = new System.Windows.Forms.MaskedTextBox();
            this.mtxtBoxPhone2 = new System.Windows.Forms.MaskedTextBox();
            this.mtxtBoxPhone = new System.Windows.Forms.MaskedTextBox();
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
            this.btnSave = new System.Windows.Forms.Button();
            this.grpRecordNumber = new System.Windows.Forms.GroupBox();
            this.btnLastRecord = new System.Windows.Forms.Button();
            this.btnFirstRecord = new System.Windows.Forms.Button();
            this.btnRightArrow = new System.Windows.Forms.Button();
            this.btnArrowLeft = new System.Windows.Forms.Button();
            this.rdoFemale = new System.Windows.Forms.RadioButton();
            this.rdoActive = new System.Windows.Forms.RadioButton();
            this.rdoInActive = new System.Windows.Forms.RadioButton();
            this.rdoMale = new System.Windows.Forms.RadioButton();
            this.grpGender = new System.Windows.Forms.GroupBox();
            this.grpStatus = new System.Windows.Forms.GroupBox();
            this.chbSenior = new System.Windows.Forms.CheckBox();
            this.txtlastBowled = new System.Windows.Forms.TextBox();
            this.grpMemberInfo.SuspendLayout();
            this.groupRecord.SuspendLayout();
            this.grpRecordNumber.SuspendLayout();
            this.grpGender.SuspendLayout();
            this.grpStatus.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtLastName
            // 
            this.txtLastName.BackColor = System.Drawing.Color.LightPink;
            this.txtLastName.Font = new System.Drawing.Font("Calibri", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtLastName.Location = new System.Drawing.Point(226, 31);
            this.txtLastName.MaxLength = 50;
            this.txtLastName.Multiline = true;
            this.txtLastName.Name = "txtLastName";
            this.txtLastName.Size = new System.Drawing.Size(226, 38);
            this.txtLastName.TabIndex = 1;
            this.txtLastName.TextChanged += new System.EventHandler(this.InputRequired);
            // 
            // txtFirstName
            // 
            this.txtFirstName.BackColor = System.Drawing.Color.LightPink;
            this.txtFirstName.Font = new System.Drawing.Font("Calibri", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtFirstName.Location = new System.Drawing.Point(477, 31);
            this.txtFirstName.MaxLength = 50;
            this.txtFirstName.Multiline = true;
            this.txtFirstName.Name = "txtFirstName";
            this.txtFirstName.Size = new System.Drawing.Size(232, 38);
            this.txtFirstName.TabIndex = 2;
            this.txtFirstName.TextChanged += new System.EventHandler(this.InputRequired);
            // 
            // txtMiddleInitial
            // 
            this.txtMiddleInitial.Font = new System.Drawing.Font("Calibri", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtMiddleInitial.Location = new System.Drawing.Point(731, 31);
            this.txtMiddleInitial.MaxLength = 10;
            this.txtMiddleInitial.Multiline = true;
            this.txtMiddleInitial.Name = "txtMiddleInitial";
            this.txtMiddleInitial.Size = new System.Drawing.Size(119, 38);
            this.txtMiddleInitial.TabIndex = 3;
            this.txtMiddleInitial.TextChanged += new System.EventHandler(this.InputRequired);
            // 
            // lblLastName
            // 
            this.lblLastName.AutoSize = true;
            this.lblLastName.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLastName.Location = new System.Drawing.Point(222, 9);
            this.lblLastName.Name = "lblLastName";
            this.lblLastName.Size = new System.Drawing.Size(80, 19);
            this.lblLastName.TabIndex = 3;
            this.lblLastName.Text = "Last Name";
            // 
            // lblFirstName
            // 
            this.lblFirstName.AutoSize = true;
            this.lblFirstName.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFirstName.Location = new System.Drawing.Point(473, 9);
            this.lblFirstName.Name = "lblFirstName";
            this.lblFirstName.Size = new System.Drawing.Size(82, 19);
            this.lblFirstName.TabIndex = 2;
            this.lblFirstName.Text = "First Name";
            // 
            // lblMiddleIntial
            // 
            this.lblMiddleIntial.AutoSize = true;
            this.lblMiddleIntial.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMiddleIntial.Location = new System.Drawing.Point(727, 9);
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
            this.txtBonus.Location = new System.Drawing.Point(761, 438);
            this.txtBonus.MaxLength = 20;
            this.txtBonus.Multiline = true;
            this.txtBonus.Name = "txtBonus";
            this.txtBonus.Size = new System.Drawing.Size(89, 54);
            this.txtBonus.TabIndex = 25;
            this.txtBonus.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtHandicap
            // 
            this.txtHandicap.Font = new System.Drawing.Font("Calibri", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtHandicap.Location = new System.Drawing.Point(573, 438);
            this.txtHandicap.MaxLength = 20;
            this.txtHandicap.Multiline = true;
            this.txtHandicap.Name = "txtHandicap";
            this.txtHandicap.Size = new System.Drawing.Size(89, 54);
            this.txtHandicap.TabIndex = 24;
            this.txtHandicap.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lblAverage
            // 
            this.lblAverage.AutoSize = true;
            this.lblAverage.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAverage.Location = new System.Drawing.Point(389, 420);
            this.lblAverage.Name = "lblAverage";
            this.lblAverage.Size = new System.Drawing.Size(50, 15);
            this.lblAverage.TabIndex = 17;
            this.lblAverage.Text = "Average";
            // 
            // lblHandicap
            // 
            this.lblHandicap.AutoSize = true;
            this.lblHandicap.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHandicap.Location = new System.Drawing.Point(570, 420);
            this.lblHandicap.Name = "lblHandicap";
            this.lblHandicap.Size = new System.Drawing.Size(60, 15);
            this.lblHandicap.TabIndex = 17;
            this.lblHandicap.Text = "Handicap";
            // 
            // lblBonus
            // 
            this.lblBonus.AutoSize = true;
            this.lblBonus.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBonus.Location = new System.Drawing.Point(758, 420);
            this.lblBonus.Name = "lblBonus";
            this.lblBonus.Size = new System.Drawing.Size(41, 15);
            this.lblBonus.TabIndex = 17;
            this.lblBonus.Text = "Bonus";
            // 
            // txtNotes
            // 
            this.txtNotes.Location = new System.Drawing.Point(38, 319);
            this.txtNotes.MaxLength = 750;
            this.txtNotes.Multiline = true;
            this.txtNotes.Name = "txtNotes";
            this.txtNotes.Size = new System.Drawing.Size(298, 218);
            this.txtNotes.TabIndex = 22;
            this.txtNotes.TabStop = false;
            // 
            // txtAverage
            // 
            this.txtAverage.Font = new System.Drawing.Font("Calibri", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAverage.Location = new System.Drawing.Point(392, 438);
            this.txtAverage.MaxLength = 20;
            this.txtAverage.Multiline = true;
            this.txtAverage.Name = "txtAverage";
            this.txtAverage.Size = new System.Drawing.Size(99, 54);
            this.txtAverage.TabIndex = 23;
            this.txtAverage.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lblNotes
            // 
            this.lblNotes.AutoSize = true;
            this.lblNotes.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNotes.Location = new System.Drawing.Point(36, 297);
            this.lblNotes.Name = "lblNotes";
            this.lblNotes.Size = new System.Drawing.Size(49, 19);
            this.lblNotes.TabIndex = 22;
            this.lblNotes.Text = "Notes";
            // 
            // grpMemberInfo
            // 
            this.grpMemberInfo.Controls.Add(this.txtrejoinDate);
            this.grpMemberInfo.Controls.Add(this.txtdateJoined);
            this.grpMemberInfo.Controls.Add(this.mtxtBoxDOB);
            this.grpMemberInfo.Controls.Add(this.mtxtBoxZip);
            this.grpMemberInfo.Controls.Add(this.mtxtBoxSSN);
            this.grpMemberInfo.Controls.Add(this.mtxtBoxPhone2);
            this.grpMemberInfo.Controls.Add(this.mtxtBoxPhone);
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
            this.grpMemberInfo.TabIndex = 7;
            this.grpMemberInfo.TabStop = false;
            this.grpMemberInfo.Text = "Member Information";
            // 
            // txtrejoinDate
            // 
            this.txtrejoinDate.Location = new System.Drawing.Point(344, 186);
            this.txtrejoinDate.Name = "txtrejoinDate";
            this.txtrejoinDate.Size = new System.Drawing.Size(100, 23);
            this.txtrejoinDate.TabIndex = 19;
            // 
            // txtdateJoined
            // 
            this.txtdateJoined.BackColor = System.Drawing.Color.LightPink;
            this.txtdateJoined.Location = new System.Drawing.Point(344, 138);
            this.txtdateJoined.Name = "txtdateJoined";
            this.txtdateJoined.Size = new System.Drawing.Size(100, 23);
            this.txtdateJoined.TabIndex = 15;
            // 
            // mtxtBoxDOB
            // 
            this.mtxtBoxDOB.BackColor = System.Drawing.Color.LightPink;
            this.mtxtBoxDOB.Location = new System.Drawing.Point(25, 41);
            this.mtxtBoxDOB.Mask = "00/00/0000";
            this.mtxtBoxDOB.Name = "mtxtBoxDOB";
            this.mtxtBoxDOB.Size = new System.Drawing.Size(100, 23);
            this.mtxtBoxDOB.TabIndex = 9;
            this.mtxtBoxDOB.ValidatingType = typeof(System.DateTime);
            // 
            // mtxtBoxZip
            // 
            this.mtxtBoxZip.BackColor = System.Drawing.Color.LightPink;
            this.mtxtBoxZip.HideSelection = false;
            this.mtxtBoxZip.Location = new System.Drawing.Point(215, 137);
            this.mtxtBoxZip.Mask = "00000";
            this.mtxtBoxZip.Name = "mtxtBoxZip";
            this.mtxtBoxZip.ShortcutsEnabled = false;
            this.mtxtBoxZip.Size = new System.Drawing.Size(100, 23);
            this.mtxtBoxZip.TabIndex = 14;
            // 
            // mtxtBoxSSN
            // 
            this.mtxtBoxSSN.BackColor = System.Drawing.Color.White;
            this.mtxtBoxSSN.Location = new System.Drawing.Point(344, 41);
            this.mtxtBoxSSN.Mask = "000-00-0000";
            this.mtxtBoxSSN.Name = "mtxtBoxSSN";
            this.mtxtBoxSSN.PasswordChar = '*';
            this.mtxtBoxSSN.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.mtxtBoxSSN.Size = new System.Drawing.Size(100, 23);
            this.mtxtBoxSSN.TabIndex = 10;
            // 
            // mtxtBoxPhone2
            // 
            this.mtxtBoxPhone2.Location = new System.Drawing.Point(155, 185);
            this.mtxtBoxPhone2.Mask = "(999) 000-0000";
            this.mtxtBoxPhone2.Name = "mtxtBoxPhone2";
            this.mtxtBoxPhone2.Size = new System.Drawing.Size(100, 23);
            this.mtxtBoxPhone2.TabIndex = 17;
            // 
            // mtxtBoxPhone
            // 
            this.mtxtBoxPhone.BackColor = System.Drawing.Color.LightPink;
            this.mtxtBoxPhone.Location = new System.Drawing.Point(25, 185);
            this.mtxtBoxPhone.Mask = "(999) 000-0000";
            this.mtxtBoxPhone.Name = "mtxtBoxPhone";
            this.mtxtBoxPhone.Size = new System.Drawing.Size(100, 23);
            this.mtxtBoxPhone.TabIndex = 16;
            // 
            // lblEmail
            // 
            this.lblEmail.AutoSize = true;
            this.lblEmail.Location = new System.Drawing.Point(22, 215);
            this.lblEmail.Name = "lblEmail";
            this.lblEmail.Size = new System.Drawing.Size(38, 15);
            this.lblEmail.TabIndex = 10;
            this.lblEmail.Text = "Email";
            // 
            // lblDOB
            // 
            this.lblDOB.AutoSize = true;
            this.lblDOB.Location = new System.Drawing.Point(22, 23);
            this.lblDOB.Name = "lblDOB";
            this.lblDOB.Size = new System.Drawing.Size(107, 15);
            this.lblDOB.TabIndex = 31;
            this.lblDOB.Text = "D.O.B. (mm/dd/yy)";
            // 
            // lblRefferals
            // 
            this.lblRefferals.AutoSize = true;
            this.lblRefferals.Location = new System.Drawing.Point(341, 215);
            this.lblRefferals.Name = "lblRefferals";
            this.lblRefferals.Size = new System.Drawing.Size(57, 15);
            this.lblRefferals.TabIndex = 6;
            this.lblRefferals.Text = "Referrals";
            // 
            // lblPhoneNumber2
            // 
            this.lblPhoneNumber2.AutoSize = true;
            this.lblPhoneNumber2.Location = new System.Drawing.Point(152, 167);
            this.lblPhoneNumber2.Name = "lblPhoneNumber2";
            this.lblPhoneNumber2.Size = new System.Drawing.Size(97, 15);
            this.lblPhoneNumber2.TabIndex = 8;
            this.lblPhoneNumber2.Text = "Phone Number 2";
            // 
            // lblPhoneNumber
            // 
            this.lblPhoneNumber.AutoSize = true;
            this.lblPhoneNumber.Location = new System.Drawing.Point(22, 167);
            this.lblPhoneNumber.Name = "lblPhoneNumber";
            this.lblPhoneNumber.Size = new System.Drawing.Size(87, 15);
            this.lblPhoneNumber.TabIndex = 7;
            this.lblPhoneNumber.Text = "Phone Number";
            // 
            // lblDateJoined
            // 
            this.lblDateJoined.AutoSize = true;
            this.lblDateJoined.Location = new System.Drawing.Point(341, 119);
            this.lblDateJoined.Name = "lblDateJoined";
            this.lblDateJoined.Size = new System.Drawing.Size(70, 15);
            this.lblDateJoined.TabIndex = 5;
            this.lblDateJoined.Text = "Date Joined";
            // 
            // lblRejoinDate
            // 
            this.lblRejoinDate.AutoSize = true;
            this.lblRejoinDate.Location = new System.Drawing.Point(341, 167);
            this.lblRejoinDate.Name = "lblRejoinDate";
            this.lblRejoinDate.Size = new System.Drawing.Size(70, 15);
            this.lblRejoinDate.TabIndex = 4;
            this.lblRejoinDate.Text = "Rejoin Date";
            // 
            // lblZip
            // 
            this.lblZip.AutoSize = true;
            this.lblZip.Location = new System.Drawing.Point(212, 119);
            this.lblZip.Name = "lblZip";
            this.lblZip.Size = new System.Drawing.Size(24, 15);
            this.lblZip.TabIndex = 2;
            this.lblZip.Text = "Zip";
            // 
            // lblState
            // 
            this.lblState.AutoSize = true;
            this.lblState.Location = new System.Drawing.Point(152, 119);
            this.lblState.Name = "lblState";
            this.lblState.Size = new System.Drawing.Size(34, 15);
            this.lblState.TabIndex = 2;
            this.lblState.Text = "State";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 119);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(28, 15);
            this.label1.TabIndex = 2;
            this.label1.Text = "City";
            // 
            // txtReferrals
            // 
            this.txtReferrals.Location = new System.Drawing.Point(344, 233);
            this.txtReferrals.MaxLength = 50;
            this.txtReferrals.Name = "txtReferrals";
            this.txtReferrals.Size = new System.Drawing.Size(54, 23);
            this.txtReferrals.TabIndex = 21;
            // 
            // lblAddress
            // 
            this.lblAddress.AutoSize = true;
            this.lblAddress.Location = new System.Drawing.Point(22, 71);
            this.lblAddress.Name = "lblAddress";
            this.lblAddress.Size = new System.Drawing.Size(51, 15);
            this.lblAddress.TabIndex = 3;
            this.lblAddress.Text = "Address";
            // 
            // lblSSN
            // 
            this.lblSSN.AutoSize = true;
            this.lblSSN.Location = new System.Drawing.Point(341, 23);
            this.lblSSN.Name = "lblSSN";
            this.lblSSN.Size = new System.Drawing.Size(27, 15);
            this.lblSSN.TabIndex = 3;
            this.lblSSN.Text = "SSN";
            // 
            // txtState
            // 
            this.txtState.BackColor = System.Drawing.Color.LightPink;
            this.txtState.Location = new System.Drawing.Point(155, 137);
            this.txtState.MaxLength = 20;
            this.txtState.Name = "txtState";
            this.txtState.Size = new System.Drawing.Size(32, 23);
            this.txtState.TabIndex = 13;
            // 
            // txtCity
            // 
            this.txtCity.BackColor = System.Drawing.Color.LightPink;
            this.txtCity.Location = new System.Drawing.Point(25, 137);
            this.txtCity.MaxLength = 20;
            this.txtCity.Name = "txtCity";
            this.txtCity.Size = new System.Drawing.Size(100, 23);
            this.txtCity.TabIndex = 12;
            // 
            // txtAddress
            // 
            this.txtAddress.BackColor = System.Drawing.Color.LightPink;
            this.txtAddress.Location = new System.Drawing.Point(25, 89);
            this.txtAddress.MaxLength = 75;
            this.txtAddress.Name = "txtAddress";
            this.txtAddress.Size = new System.Drawing.Size(290, 23);
            this.txtAddress.TabIndex = 11;
            // 
            // txtEmail
            // 
            this.txtEmail.BackColor = System.Drawing.Color.LightPink;
            this.txtEmail.Location = new System.Drawing.Point(25, 233);
            this.txtEmail.MaxLength = 50;
            this.txtEmail.Name = "txtEmail";
            this.txtEmail.Size = new System.Drawing.Size(290, 23);
            this.txtEmail.TabIndex = 20;
            // 
            // txtYearEndT
            // 
            this.txtYearEndT.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtYearEndT.Location = new System.Drawing.Point(392, 517);
            this.txtYearEndT.MaxLength = 20;
            this.txtYearEndT.Name = "txtYearEndT";
            this.txtYearEndT.Size = new System.Drawing.Size(124, 23);
            this.txtYearEndT.TabIndex = 26;
            // 
            // txtMoneyEarned
            // 
            this.txtMoneyEarned.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtMoneyEarned.Location = new System.Drawing.Point(573, 517);
            this.txtMoneyEarned.Name = "txtMoneyEarned";
            this.txtMoneyEarned.Size = new System.Drawing.Size(100, 23);
            this.txtMoneyEarned.TabIndex = 27;
            // 
            // lblYET
            // 
            this.lblYET.AutoSize = true;
            this.lblYET.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblYET.Location = new System.Drawing.Point(389, 499);
            this.lblYET.Name = "lblYET";
            this.lblYET.Size = new System.Drawing.Size(127, 15);
            this.lblYET.TabIndex = 25;
            this.lblYET.Text = "Year End Tournaments";
            // 
            // lblMoneyEarned
            // 
            this.lblMoneyEarned.AutoSize = true;
            this.lblMoneyEarned.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMoneyEarned.Location = new System.Drawing.Point(570, 499);
            this.lblMoneyEarned.Name = "lblMoneyEarned";
            this.lblMoneyEarned.Size = new System.Drawing.Size(85, 15);
            this.lblMoneyEarned.TabIndex = 26;
            this.lblMoneyEarned.Text = "Money Earned";
            // 
            // lblLastBowled
            // 
            this.lblLastBowled.AutoSize = true;
            this.lblLastBowled.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLastBowled.Location = new System.Drawing.Point(744, 499);
            this.lblLastBowled.Name = "lblLastBowled";
            this.lblLastBowled.Size = new System.Drawing.Size(72, 15);
            this.lblLastBowled.TabIndex = 27;
            this.lblLastBowled.Text = "Last Bowled";
            // 
            // groupRecord
            // 
            this.groupRecord.Controls.Add(this.btnSave);
            this.groupRecord.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupRecord.Location = new System.Drawing.Point(23, 583);
            this.groupRecord.Name = "groupRecord";
            this.groupRecord.Size = new System.Drawing.Size(108, 100);
            this.groupRecord.TabIndex = 29;
            this.groupRecord.TabStop = false;
            this.groupRecord.Text = "Record";
            // 
            // btnSave
            // 
            this.btnSave.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSave.Location = new System.Drawing.Point(15, 48);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 29;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.BtnSave_Click);
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
            this.btnLastRecord.Click += new System.EventHandler(this.BtnLastRecord_Click);
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
            this.btnFirstRecord.Click += new System.EventHandler(this.BtnFirstRecord_Click);
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
            this.btnRightArrow.Click += new System.EventHandler(this.BtnRightArrow_Click);
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
            this.btnArrowLeft.Click += new System.EventHandler(this.BtnArrowLeft_Click);
            // 
            // rdoFemale
            // 
            this.rdoFemale.AutoSize = true;
            this.rdoFemale.Location = new System.Drawing.Point(11, 19);
            this.rdoFemale.Name = "rdoFemale";
            this.rdoFemale.Size = new System.Drawing.Size(64, 19);
            this.rdoFemale.TabIndex = 7;
            this.rdoFemale.Text = "Female";
            this.rdoFemale.UseVisualStyleBackColor = true;
            // 
            // rdoActive
            // 
            this.rdoActive.AutoSize = true;
            this.rdoActive.Location = new System.Drawing.Point(11, 23);
            this.rdoActive.Name = "rdoActive";
            this.rdoActive.Size = new System.Drawing.Size(57, 19);
            this.rdoActive.TabIndex = 4;
            this.rdoActive.Text = "Active";
            this.rdoActive.UseVisualStyleBackColor = true;
            // 
            // rdoInActive
            // 
            this.rdoInActive.AutoSize = true;
            this.rdoInActive.Location = new System.Drawing.Point(11, 46);
            this.rdoInActive.Name = "rdoInActive";
            this.rdoInActive.Size = new System.Drawing.Size(68, 19);
            this.rdoInActive.TabIndex = 4;
            this.rdoInActive.Text = "InActive";
            this.rdoInActive.UseVisualStyleBackColor = true;
            // 
            // rdoMale
            // 
            this.rdoMale.AutoSize = true;
            this.rdoMale.Location = new System.Drawing.Point(11, 42);
            this.rdoMale.Name = "rdoMale";
            this.rdoMale.Size = new System.Drawing.Size(53, 19);
            this.rdoMale.TabIndex = 8;
            this.rdoMale.Text = "Male";
            this.rdoMale.UseVisualStyleBackColor = true;
            // 
            // grpGender
            // 
            this.grpGender.Controls.Add(this.rdoFemale);
            this.grpGender.Controls.Add(this.rdoMale);
            this.grpGender.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpGender.Location = new System.Drawing.Point(236, 164);
            this.grpGender.Name = "grpGender";
            this.grpGender.Size = new System.Drawing.Size(101, 71);
            this.grpGender.TabIndex = 6;
            this.grpGender.TabStop = false;
            this.grpGender.Text = "Gender";
            // 
            // grpStatus
            // 
            this.grpStatus.Controls.Add(this.rdoInActive);
            this.grpStatus.Controls.Add(this.rdoActive);
            this.grpStatus.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpStatus.Location = new System.Drawing.Point(38, 156);
            this.grpStatus.Name = "grpStatus";
            this.grpStatus.Size = new System.Drawing.Size(101, 79);
            this.grpStatus.TabIndex = 4;
            this.grpStatus.TabStop = false;
            this.grpStatus.Text = "Status";
            // 
            // chbSenior
            // 
            this.chbSenior.AutoSize = true;
            this.chbSenior.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chbSenior.Location = new System.Drawing.Point(156, 179);
            this.chbSenior.Name = "chbSenior";
            this.chbSenior.Size = new System.Drawing.Size(61, 19);
            this.chbSenior.TabIndex = 5;
            this.chbSenior.TabStop = false;
            this.chbSenior.Text = "Senior";
            this.chbSenior.UseVisualStyleBackColor = true;
            // 
            // txtlastBowled
            // 
            this.txtlastBowled.BackColor = System.Drawing.Color.LightPink;
            this.txtlastBowled.Location = new System.Drawing.Point(747, 519);
            this.txtlastBowled.Name = "txtlastBowled";
            this.txtlastBowled.Size = new System.Drawing.Size(100, 21);
            this.txtlastBowled.TabIndex = 28;
            // 
            // FrmMemberData
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(884, 712);
            this.Controls.Add(this.txtlastBowled);
            this.Controls.Add(this.chbSenior);
            this.Controls.Add(this.grpStatus);
            this.Controls.Add(this.grpGender);
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
            this.MinimumSize = new System.Drawing.Size(900, 750);
            this.Name = "FrmMemberData";
            this.Text = "Member Info";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FrmMemberData_FormClosed);
            this.Load += new System.EventHandler(this.MemberDataForm_Load);
            this.grpMemberInfo.ResumeLayout(false);
            this.grpMemberInfo.PerformLayout();
            this.groupRecord.ResumeLayout(false);
            this.grpRecordNumber.ResumeLayout(false);
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
        private System.Windows.Forms.Button btnLastRecord;
        private System.Windows.Forms.Button btnFirstRecord;
        private System.Windows.Forms.Button btnRightArrow;
        private System.Windows.Forms.Button btnArrowLeft;
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
        private System.Windows.Forms.MaskedTextBox mtxtBoxPhone;
        private System.Windows.Forms.MaskedTextBox mtxtBoxPhone2;
        private System.Windows.Forms.MaskedTextBox mtxtBoxSSN;
        private System.Windows.Forms.MaskedTextBox mtxtBoxZip;
        private System.Windows.Forms.MaskedTextBox mtxtBoxDOB;
        private System.Windows.Forms.TextBox txtrejoinDate;
        private System.Windows.Forms.TextBox txtdateJoined;
        private System.Windows.Forms.TextBox txtlastBowled;
    }
}

