namespace NineTapTour.Forms
{
    partial class FrmMemberScores
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmMemberScores));
            this.grpMemberNum = new System.Windows.Forms.GroupBox();
            this.txtMemberNum = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rdoSquadFour = new System.Windows.Forms.RadioButton();
            this.rdoSquad3 = new System.Windows.Forms.RadioButton();
            this.rdoSquadTwo = new System.Windows.Forms.RadioButton();
            this.rdoSquadOne = new System.Windows.Forms.RadioButton();
            this.txtLastName = new System.Windows.Forms.TextBox();
            this.txtFirstName = new System.Windows.Forms.TextBox();
            this.txtMiddleInitial = new System.Windows.Forms.TextBox();
            this.lblLastName = new System.Windows.Forms.Label();
            this.lblFirstName = new System.Windows.Forms.Label();
            this.lblMiddleInitial = new System.Windows.Forms.Label();
            this.grpMemberStatus = new System.Windows.Forms.GroupBox();
            this.rdoActive = new System.Windows.Forms.RadioButton();
            this.txtScratchScore1 = new System.Windows.Forms.TextBox();
            this.txtHandicapScore1 = new System.Windows.Forms.TextBox();
            this.txtScratchScore2 = new System.Windows.Forms.TextBox();
            this.txtHandicapScore2 = new System.Windows.Forms.TextBox();
            this.txtScratchScore3 = new System.Windows.Forms.TextBox();
            this.txtHandicapScore3 = new System.Windows.Forms.TextBox();
            this.txtScratchScore4 = new System.Windows.Forms.TextBox();
            this.txtHandicapScore4 = new System.Windows.Forms.TextBox();
            this.txtScratchTotal = new System.Windows.Forms.TextBox();
            this.txtHandicapTotal = new System.Windows.Forms.TextBox();
            this.grpRecord = new System.Windows.Forms.GroupBox();
            this.btnRightArrow = new System.Windows.Forms.Button();
            this.btnLeftArrow = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.btnLastFile = new System.Windows.Forms.Button();
            this.btnNew = new System.Windows.Forms.Button();
            this.lblStratchScores = new System.Windows.Forms.Label();
            this.lblHandiCap = new System.Windows.Forms.Label();
            this.grpStats = new System.Windows.Forms.GroupBox();
            this.lblBonusPins = new System.Windows.Forms.Label();
            this.lblCap = new System.Windows.Forms.Label();
            this.txtBonusPins = new System.Windows.Forms.TextBox();
            this.txtHandicap = new System.Windows.Forms.TextBox();
            this.grpTournamentFile = new System.Windows.Forms.GroupBox();
            this.txtTournamentFile = new System.Windows.Forms.RichTextBox();
            this.grpLeaders = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lblHighGame = new System.Windows.Forms.Label();
            this.btnRefresh3 = new System.Windows.Forms.Button();
            this.btnRefresh2 = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.richTextBox3 = new System.Windows.Forms.RichTextBox();
            this.richTextBox2 = new System.Windows.Forms.RichTextBox();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.lblSeries = new System.Windows.Forms.Label();
            this.lblGameSenior = new System.Windows.Forms.Label();
            this.grpReports = new System.Windows.Forms.GroupBox();
            this.btnSeries = new System.Windows.Forms.Button();
            this.btnGame = new System.Windows.Forms.Button();
            this.btnSenior = new System.Windows.Forms.Button();
            this.grpComments = new System.Windows.Forms.GroupBox();
            this.rtxtComments = new System.Windows.Forms.RichTextBox();
            this.grpMemberNum.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.grpMemberStatus.SuspendLayout();
            this.grpRecord.SuspendLayout();
            this.grpStats.SuspendLayout();
            this.grpTournamentFile.SuspendLayout();
            this.grpLeaders.SuspendLayout();
            this.grpReports.SuspendLayout();
            this.grpComments.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpMemberNum
            // 
            this.grpMemberNum.Controls.Add(this.txtMemberNum);
            this.grpMemberNum.Location = new System.Drawing.Point(21, 23);
            this.grpMemberNum.Name = "grpMemberNum";
            this.grpMemberNum.Size = new System.Drawing.Size(179, 57);
            this.grpMemberNum.TabIndex = 0;
            this.grpMemberNum.TabStop = false;
            this.grpMemberNum.Text = "Enter Member Number";
            // 
            // txtMemberNum
            // 
            this.txtMemberNum.Location = new System.Drawing.Point(6, 19);
            this.txtMemberNum.Name = "txtMemberNum";
            this.txtMemberNum.Size = new System.Drawing.Size(86, 20);
            this.txtMemberNum.TabIndex = 0;
            this.txtMemberNum.TextChanged += new System.EventHandler(this.txtMemberNum_TextChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rdoSquadFour);
            this.groupBox1.Controls.Add(this.rdoSquad3);
            this.groupBox1.Controls.Add(this.rdoSquadTwo);
            this.groupBox1.Controls.Add(this.rdoSquadOne);
            this.groupBox1.Location = new System.Drawing.Point(21, 85);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(179, 73);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Squad Number";
            // 
            // rdoSquadFour
            // 
            this.rdoSquadFour.AutoSize = true;
            this.rdoSquadFour.Location = new System.Drawing.Point(108, 43);
            this.rdoSquadFour.Name = "rdoSquadFour";
            this.rdoSquadFour.Size = new System.Drawing.Size(65, 17);
            this.rdoSquadFour.TabIndex = 0;
            this.rdoSquadFour.TabStop = true;
            this.rdoSquadFour.Text = "Squad 4";
            this.rdoSquadFour.UseVisualStyleBackColor = true;
            // 
            // rdoSquad3
            // 
            this.rdoSquad3.AutoSize = true;
            this.rdoSquad3.Location = new System.Drawing.Point(6, 43);
            this.rdoSquad3.Name = "rdoSquad3";
            this.rdoSquad3.Size = new System.Drawing.Size(65, 17);
            this.rdoSquad3.TabIndex = 0;
            this.rdoSquad3.TabStop = true;
            this.rdoSquad3.Text = "Squad 3";
            this.rdoSquad3.UseVisualStyleBackColor = true;
            // 
            // rdoSquadTwo
            // 
            this.rdoSquadTwo.AutoSize = true;
            this.rdoSquadTwo.Location = new System.Drawing.Point(108, 20);
            this.rdoSquadTwo.Name = "rdoSquadTwo";
            this.rdoSquadTwo.Size = new System.Drawing.Size(65, 17);
            this.rdoSquadTwo.TabIndex = 0;
            this.rdoSquadTwo.TabStop = true;
            this.rdoSquadTwo.Text = "Squad 2";
            this.rdoSquadTwo.UseVisualStyleBackColor = true;
            // 
            // rdoSquadOne
            // 
            this.rdoSquadOne.AutoSize = true;
            this.rdoSquadOne.Location = new System.Drawing.Point(6, 20);
            this.rdoSquadOne.Name = "rdoSquadOne";
            this.rdoSquadOne.Size = new System.Drawing.Size(65, 17);
            this.rdoSquadOne.TabIndex = 0;
            this.rdoSquadOne.TabStop = true;
            this.rdoSquadOne.Text = "Squad 1";
            this.rdoSquadOne.UseVisualStyleBackColor = true;
            // 
            // txtLastName
            // 
            this.txtLastName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtLastName.Enabled = false;
            this.txtLastName.Font = new System.Drawing.Font("Calibri", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtLastName.Location = new System.Drawing.Point(218, 27);
            this.txtLastName.Multiline = true;
            this.txtLastName.Name = "txtLastName";
            this.txtLastName.ReadOnly = true;
            this.txtLastName.Size = new System.Drawing.Size(206, 38);
            this.txtLastName.TabIndex = 2;
            // 
            // txtFirstName
            // 
            this.txtFirstName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtFirstName.Enabled = false;
            this.txtFirstName.Font = new System.Drawing.Font("Calibri", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtFirstName.Location = new System.Drawing.Point(452, 27);
            this.txtFirstName.Multiline = true;
            this.txtFirstName.Name = "txtFirstName";
            this.txtFirstName.ReadOnly = true;
            this.txtFirstName.Size = new System.Drawing.Size(206, 38);
            this.txtFirstName.TabIndex = 2;
            // 
            // txtMiddleInitial
            // 
            this.txtMiddleInitial.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtMiddleInitial.Enabled = false;
            this.txtMiddleInitial.Font = new System.Drawing.Font("Calibri", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtMiddleInitial.Location = new System.Drawing.Point(686, 27);
            this.txtMiddleInitial.Multiline = true;
            this.txtMiddleInitial.Name = "txtMiddleInitial";
            this.txtMiddleInitial.ReadOnly = true;
            this.txtMiddleInitial.Size = new System.Drawing.Size(86, 38);
            this.txtMiddleInitial.TabIndex = 3;
            // 
            // lblLastName
            // 
            this.lblLastName.AutoSize = true;
            this.lblLastName.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold);
            this.lblLastName.Location = new System.Drawing.Point(218, 8);
            this.lblLastName.Name = "lblLastName";
            this.lblLastName.Size = new System.Drawing.Size(80, 19);
            this.lblLastName.TabIndex = 4;
            this.lblLastName.Text = "Last Name";
            // 
            // lblFirstName
            // 
            this.lblFirstName.AutoSize = true;
            this.lblFirstName.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold);
            this.lblFirstName.Location = new System.Drawing.Point(452, 8);
            this.lblFirstName.Name = "lblFirstName";
            this.lblFirstName.Size = new System.Drawing.Size(82, 19);
            this.lblFirstName.TabIndex = 4;
            this.lblFirstName.Text = "First Name";
            // 
            // lblMiddleInitial
            // 
            this.lblMiddleInitial.AutoSize = true;
            this.lblMiddleInitial.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold);
            this.lblMiddleInitial.Location = new System.Drawing.Point(680, 8);
            this.lblMiddleInitial.Name = "lblMiddleInitial";
            this.lblMiddleInitial.Size = new System.Drawing.Size(99, 19);
            this.lblMiddleInitial.TabIndex = 4;
            this.lblMiddleInitial.Text = "Middle Initial";
            // 
            // grpMemberStatus
            // 
            this.grpMemberStatus.Controls.Add(this.rdoActive);
            this.grpMemberStatus.Location = new System.Drawing.Point(225, 101);
            this.grpMemberStatus.Name = "grpMemberStatus";
            this.grpMemberStatus.Size = new System.Drawing.Size(110, 61);
            this.grpMemberStatus.TabIndex = 5;
            this.grpMemberStatus.TabStop = false;
            this.grpMemberStatus.Text = "Member Status";
            // 
            // rdoActive
            // 
            this.rdoActive.AutoSize = true;
            this.rdoActive.Location = new System.Drawing.Point(18, 27);
            this.rdoActive.Name = "rdoActive";
            this.rdoActive.Size = new System.Drawing.Size(55, 17);
            this.rdoActive.TabIndex = 0;
            this.rdoActive.TabStop = true;
            this.rdoActive.Text = "Active";
            this.rdoActive.UseVisualStyleBackColor = true;
            // 
            // txtScratchScore1
            // 
            this.txtScratchScore1.Font = new System.Drawing.Font("Calibri", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtScratchScore1.Location = new System.Drawing.Point(21, 213);
            this.txtScratchScore1.Multiline = true;
            this.txtScratchScore1.Name = "txtScratchScore1";
            this.txtScratchScore1.Size = new System.Drawing.Size(57, 49);
            this.txtScratchScore1.TabIndex = 6;
            // 
            // txtHandicapScore1
            // 
            this.txtHandicapScore1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtHandicapScore1.Enabled = false;
            this.txtHandicapScore1.Font = new System.Drawing.Font("Calibri", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtHandicapScore1.Location = new System.Drawing.Point(124, 213);
            this.txtHandicapScore1.Multiline = true;
            this.txtHandicapScore1.Name = "txtHandicapScore1";
            this.txtHandicapScore1.ReadOnly = true;
            this.txtHandicapScore1.Size = new System.Drawing.Size(57, 49);
            this.txtHandicapScore1.TabIndex = 6;
            // 
            // txtScratchScore2
            // 
            this.txtScratchScore2.Font = new System.Drawing.Font("Calibri", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtScratchScore2.Location = new System.Drawing.Point(21, 268);
            this.txtScratchScore2.Multiline = true;
            this.txtScratchScore2.Name = "txtScratchScore2";
            this.txtScratchScore2.Size = new System.Drawing.Size(57, 49);
            this.txtScratchScore2.TabIndex = 6;
            // 
            // txtHandicapScore2
            // 
            this.txtHandicapScore2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtHandicapScore2.Enabled = false;
            this.txtHandicapScore2.Font = new System.Drawing.Font("Calibri", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtHandicapScore2.Location = new System.Drawing.Point(124, 268);
            this.txtHandicapScore2.Multiline = true;
            this.txtHandicapScore2.Name = "txtHandicapScore2";
            this.txtHandicapScore2.ReadOnly = true;
            this.txtHandicapScore2.Size = new System.Drawing.Size(57, 49);
            this.txtHandicapScore2.TabIndex = 6;
            // 
            // txtScratchScore3
            // 
            this.txtScratchScore3.Font = new System.Drawing.Font("Calibri", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtScratchScore3.Location = new System.Drawing.Point(21, 323);
            this.txtScratchScore3.Multiline = true;
            this.txtScratchScore3.Name = "txtScratchScore3";
            this.txtScratchScore3.Size = new System.Drawing.Size(57, 49);
            this.txtScratchScore3.TabIndex = 6;
            // 
            // txtHandicapScore3
            // 
            this.txtHandicapScore3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtHandicapScore3.Enabled = false;
            this.txtHandicapScore3.Font = new System.Drawing.Font("Calibri", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtHandicapScore3.Location = new System.Drawing.Point(124, 323);
            this.txtHandicapScore3.Multiline = true;
            this.txtHandicapScore3.Name = "txtHandicapScore3";
            this.txtHandicapScore3.ReadOnly = true;
            this.txtHandicapScore3.Size = new System.Drawing.Size(57, 49);
            this.txtHandicapScore3.TabIndex = 6;
            // 
            // txtScratchScore4
            // 
            this.txtScratchScore4.Font = new System.Drawing.Font("Calibri", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtScratchScore4.Location = new System.Drawing.Point(21, 378);
            this.txtScratchScore4.Multiline = true;
            this.txtScratchScore4.Name = "txtScratchScore4";
            this.txtScratchScore4.Size = new System.Drawing.Size(57, 49);
            this.txtScratchScore4.TabIndex = 6;
            // 
            // txtHandicapScore4
            // 
            this.txtHandicapScore4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtHandicapScore4.Enabled = false;
            this.txtHandicapScore4.Font = new System.Drawing.Font("Calibri", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtHandicapScore4.Location = new System.Drawing.Point(124, 378);
            this.txtHandicapScore4.Multiline = true;
            this.txtHandicapScore4.Name = "txtHandicapScore4";
            this.txtHandicapScore4.ReadOnly = true;
            this.txtHandicapScore4.Size = new System.Drawing.Size(57, 49);
            this.txtHandicapScore4.TabIndex = 6;
            // 
            // txtScratchTotal
            // 
            this.txtScratchTotal.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtScratchTotal.Enabled = false;
            this.txtScratchTotal.Font = new System.Drawing.Font("Calibri", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtScratchTotal.Location = new System.Drawing.Point(21, 433);
            this.txtScratchTotal.Multiline = true;
            this.txtScratchTotal.Name = "txtScratchTotal";
            this.txtScratchTotal.ReadOnly = true;
            this.txtScratchTotal.Size = new System.Drawing.Size(83, 55);
            this.txtScratchTotal.TabIndex = 7;
            // 
            // txtHandicapTotal
            // 
            this.txtHandicapTotal.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtHandicapTotal.Enabled = false;
            this.txtHandicapTotal.Font = new System.Drawing.Font("Calibri", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtHandicapTotal.Location = new System.Drawing.Point(124, 433);
            this.txtHandicapTotal.Multiline = true;
            this.txtHandicapTotal.Name = "txtHandicapTotal";
            this.txtHandicapTotal.ReadOnly = true;
            this.txtHandicapTotal.Size = new System.Drawing.Size(83, 55);
            this.txtHandicapTotal.TabIndex = 7;
            // 
            // grpRecord
            // 
            this.grpRecord.Controls.Add(this.btnRightArrow);
            this.grpRecord.Controls.Add(this.btnLeftArrow);
            this.grpRecord.Controls.Add(this.btnDelete);
            this.grpRecord.Controls.Add(this.button5);
            this.grpRecord.Controls.Add(this.btnLastFile);
            this.grpRecord.Controls.Add(this.btnNew);
            this.grpRecord.Location = new System.Drawing.Point(12, 494);
            this.grpRecord.Name = "grpRecord";
            this.grpRecord.Size = new System.Drawing.Size(200, 180);
            this.grpRecord.TabIndex = 8;
            this.grpRecord.TabStop = false;
            this.grpRecord.Text = "Record";
            // 
            // btnRightArrow
            // 
            this.btnRightArrow.Location = new System.Drawing.Point(103, 50);
            this.btnRightArrow.Name = "btnRightArrow";
            this.btnRightArrow.Size = new System.Drawing.Size(72, 29);
            this.btnRightArrow.TabIndex = 1;
            this.btnRightArrow.Text = ">";
            this.btnRightArrow.UseVisualStyleBackColor = true;
            // 
            // btnLeftArrow
            // 
            this.btnLeftArrow.Location = new System.Drawing.Point(19, 50);
            this.btnLeftArrow.Name = "btnLeftArrow";
            this.btnLeftArrow.Size = new System.Drawing.Size(72, 29);
            this.btnLeftArrow.TabIndex = 1;
            this.btnLeftArrow.Text = "<";
            this.btnLeftArrow.UseVisualStyleBackColor = true;
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(19, 147);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(156, 25);
            this.btnDelete.TabIndex = 0;
            this.btnDelete.Text = "Delete";
            this.btnDelete.UseVisualStyleBackColor = true;
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(19, 116);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(156, 25);
            this.button5.TabIndex = 0;
            this.button5.Text = "Stats";
            this.button5.UseVisualStyleBackColor = true;
            // 
            // btnLastFile
            // 
            this.btnLastFile.Location = new System.Drawing.Point(19, 85);
            this.btnLastFile.Name = "btnLastFile";
            this.btnLastFile.Size = new System.Drawing.Size(156, 25);
            this.btnLastFile.TabIndex = 0;
            this.btnLastFile.Text = "Last File";
            this.btnLastFile.UseVisualStyleBackColor = true;
            // 
            // btnNew
            // 
            this.btnNew.Location = new System.Drawing.Point(19, 19);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(156, 25);
            this.btnNew.TabIndex = 0;
            this.btnNew.Text = "New";
            this.btnNew.UseVisualStyleBackColor = true;
            // 
            // lblStratchScores
            // 
            this.lblStratchScores.AutoSize = true;
            this.lblStratchScores.Location = new System.Drawing.Point(12, 187);
            this.lblStratchScores.Name = "lblStratchScores";
            this.lblStratchScores.Size = new System.Drawing.Size(80, 13);
            this.lblStratchScores.TabIndex = 9;
            this.lblStratchScores.Text = "Scratch Scores";
            // 
            // lblHandiCap
            // 
            this.lblHandiCap.AutoSize = true;
            this.lblHandiCap.Location = new System.Drawing.Point(110, 187);
            this.lblHandiCap.Name = "lblHandiCap";
            this.lblHandiCap.Size = new System.Drawing.Size(90, 13);
            this.lblHandiCap.TabIndex = 10;
            this.lblHandiCap.Text = "HandiCap Scores";
            // 
            // grpStats
            // 
            this.grpStats.Controls.Add(this.lblBonusPins);
            this.grpStats.Controls.Add(this.lblCap);
            this.grpStats.Controls.Add(this.txtBonusPins);
            this.grpStats.Controls.Add(this.txtHandicap);
            this.grpStats.Location = new System.Drawing.Point(225, 349);
            this.grpStats.Name = "grpStats";
            this.grpStats.Size = new System.Drawing.Size(147, 151);
            this.grpStats.TabIndex = 11;
            this.grpStats.TabStop = false;
            this.grpStats.Text = "Stats";
            // 
            // lblBonusPins
            // 
            this.lblBonusPins.AutoSize = true;
            this.lblBonusPins.Location = new System.Drawing.Point(50, 86);
            this.lblBonusPins.Name = "lblBonusPins";
            this.lblBonusPins.Size = new System.Drawing.Size(60, 13);
            this.lblBonusPins.TabIndex = 8;
            this.lblBonusPins.Text = "Bonus Pins";
            // 
            // lblCap
            // 
            this.lblCap.AutoSize = true;
            this.lblCap.Location = new System.Drawing.Point(50, 16);
            this.lblCap.Name = "lblCap";
            this.lblCap.Size = new System.Drawing.Size(54, 13);
            this.lblCap.TabIndex = 7;
            this.lblCap.Text = "HandiCap";
            // 
            // txtBonusPins
            // 
            this.txtBonusPins.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtBonusPins.Enabled = false;
            this.txtBonusPins.Font = new System.Drawing.Font("Calibri", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBonusPins.Location = new System.Drawing.Point(40, 102);
            this.txtBonusPins.Multiline = true;
            this.txtBonusPins.Name = "txtBonusPins";
            this.txtBonusPins.ReadOnly = true;
            this.txtBonusPins.Size = new System.Drawing.Size(76, 35);
            this.txtBonusPins.TabIndex = 6;
            this.txtBonusPins.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtHandicap
            // 
            this.txtHandicap.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtHandicap.Enabled = false;
            this.txtHandicap.Font = new System.Drawing.Font("Calibri", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtHandicap.Location = new System.Drawing.Point(40, 33);
            this.txtHandicap.Multiline = true;
            this.txtHandicap.Name = "txtHandicap";
            this.txtHandicap.ReadOnly = true;
            this.txtHandicap.Size = new System.Drawing.Size(76, 35);
            this.txtHandicap.TabIndex = 6;
            this.txtHandicap.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // grpTournamentFile
            // 
            this.grpTournamentFile.Controls.Add(this.txtTournamentFile);
            this.grpTournamentFile.Location = new System.Drawing.Point(225, 525);
            this.grpTournamentFile.Name = "grpTournamentFile";
            this.grpTournamentFile.Size = new System.Drawing.Size(201, 148);
            this.grpTournamentFile.TabIndex = 12;
            this.grpTournamentFile.TabStop = false;
            this.grpTournamentFile.Text = "Tournament File";
            // 
            // txtTournamentFile
            // 
            this.txtTournamentFile.Enabled = false;
            this.txtTournamentFile.Location = new System.Drawing.Point(18, 28);
            this.txtTournamentFile.Name = "txtTournamentFile";
            this.txtTournamentFile.Size = new System.Drawing.Size(164, 97);
            this.txtTournamentFile.TabIndex = 0;
            this.txtTournamentFile.Text = "";
            // 
            // grpLeaders
            // 
            this.grpLeaders.Controls.Add(this.label3);
            this.grpLeaders.Controls.Add(this.label2);
            this.grpLeaders.Controls.Add(this.label1);
            this.grpLeaders.Controls.Add(this.lblHighGame);
            this.grpLeaders.Controls.Add(this.btnRefresh3);
            this.grpLeaders.Controls.Add(this.btnRefresh2);
            this.grpLeaders.Controls.Add(this.btnRefresh);
            this.grpLeaders.Controls.Add(this.richTextBox3);
            this.grpLeaders.Controls.Add(this.richTextBox2);
            this.grpLeaders.Controls.Add(this.richTextBox1);
            this.grpLeaders.Controls.Add(this.lblSeries);
            this.grpLeaders.Controls.Add(this.lblGameSenior);
            this.grpLeaders.Location = new System.Drawing.Point(412, 102);
            this.grpLeaders.Name = "grpLeaders";
            this.grpLeaders.Size = new System.Drawing.Size(361, 395);
            this.grpLeaders.TabIndex = 13;
            this.grpLeaders.TabStop = false;
            this.grpLeaders.Text = "Leaders";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(22, 290);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(149, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Series [Member No.] -- (Name)";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(144, 274);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(61, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "High Series";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 167);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(200, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Game [Member No.] -- (Name) *=Pre paid";
            // 
            // lblHighGame
            // 
            this.lblHighGame.AutoSize = true;
            this.lblHighGame.Location = new System.Drawing.Point(151, 148);
            this.lblHighGame.Name = "lblHighGame";
            this.lblHighGame.Size = new System.Drawing.Size(60, 13);
            this.lblHighGame.TabIndex = 4;
            this.lblHighGame.Text = "High Game";
            // 
            // btnRefresh3
            // 
            this.btnRefresh3.Location = new System.Drawing.Point(25, 353);
            this.btnRefresh3.Name = "btnRefresh3";
            this.btnRefresh3.Size = new System.Drawing.Size(317, 23);
            this.btnRefresh3.TabIndex = 3;
            this.btnRefresh3.Text = "Refresh";
            this.btnRefresh3.UseVisualStyleBackColor = true;
            // 
            // btnRefresh2
            // 
            this.btnRefresh2.Location = new System.Drawing.Point(25, 233);
            this.btnRefresh2.Name = "btnRefresh2";
            this.btnRefresh2.Size = new System.Drawing.Size(317, 23);
            this.btnRefresh2.TabIndex = 3;
            this.btnRefresh2.Text = "Refresh";
            this.btnRefresh2.UseVisualStyleBackColor = true;
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(25, 110);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(317, 23);
            this.btnRefresh.TabIndex = 3;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.UseVisualStyleBackColor = true;
            // 
            // richTextBox3
            // 
            this.richTextBox3.Enabled = false;
            this.richTextBox3.Location = new System.Drawing.Point(25, 306);
            this.richTextBox3.Name = "richTextBox3";
            this.richTextBox3.Size = new System.Drawing.Size(317, 47);
            this.richTextBox3.TabIndex = 2;
            this.richTextBox3.Text = "";
            // 
            // richTextBox2
            // 
            this.richTextBox2.Enabled = false;
            this.richTextBox2.Location = new System.Drawing.Point(25, 183);
            this.richTextBox2.Name = "richTextBox2";
            this.richTextBox2.Size = new System.Drawing.Size(317, 44);
            this.richTextBox2.TabIndex = 2;
            this.richTextBox2.Text = "";
            // 
            // richTextBox1
            // 
            this.richTextBox1.Enabled = false;
            this.richTextBox1.Location = new System.Drawing.Point(25, 60);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(317, 47);
            this.richTextBox1.TabIndex = 2;
            this.richTextBox1.Text = "";
            // 
            // lblSeries
            // 
            this.lblSeries.AutoSize = true;
            this.lblSeries.Location = new System.Drawing.Point(22, 44);
            this.lblSeries.Name = "lblSeries";
            this.lblSeries.Size = new System.Drawing.Size(149, 13);
            this.lblSeries.TabIndex = 1;
            this.lblSeries.Text = "Series [Member No.] -- (Name)";
            // 
            // lblGameSenior
            // 
            this.lblGameSenior.AutoSize = true;
            this.lblGameSenior.Location = new System.Drawing.Point(130, 16);
            this.lblGameSenior.Name = "lblGameSenior";
            this.lblGameSenior.Size = new System.Drawing.Size(93, 13);
            this.lblGameSenior.TabIndex = 0;
            this.lblGameSenior.Text = "High Game Senior";
            // 
            // grpReports
            // 
            this.grpReports.Controls.Add(this.btnSeries);
            this.grpReports.Controls.Add(this.btnGame);
            this.grpReports.Controls.Add(this.btnSenior);
            this.grpReports.Location = new System.Drawing.Point(443, 503);
            this.grpReports.Name = "grpReports";
            this.grpReports.Size = new System.Drawing.Size(123, 171);
            this.grpReports.TabIndex = 0;
            this.grpReports.TabStop = false;
            this.grpReports.Text = "Reports";
            // 
            // btnSeries
            // 
            this.btnSeries.Location = new System.Drawing.Point(18, 114);
            this.btnSeries.Name = "btnSeries";
            this.btnSeries.Size = new System.Drawing.Size(82, 37);
            this.btnSeries.TabIndex = 0;
            this.btnSeries.Text = "Series";
            this.btnSeries.UseVisualStyleBackColor = true;
            // 
            // btnGame
            // 
            this.btnGame.Location = new System.Drawing.Point(18, 71);
            this.btnGame.Name = "btnGame";
            this.btnGame.Size = new System.Drawing.Size(82, 37);
            this.btnGame.TabIndex = 0;
            this.btnGame.Text = "Game";
            this.btnGame.UseVisualStyleBackColor = true;
            // 
            // btnSenior
            // 
            this.btnSenior.Location = new System.Drawing.Point(18, 23);
            this.btnSenior.Name = "btnSenior";
            this.btnSenior.Size = new System.Drawing.Size(82, 37);
            this.btnSenior.TabIndex = 0;
            this.btnSenior.Text = "Senior";
            this.btnSenior.UseVisualStyleBackColor = true;
            // 
            // grpComments
            // 
            this.grpComments.Controls.Add(this.rtxtComments);
            this.grpComments.Location = new System.Drawing.Point(572, 525);
            this.grpComments.Name = "grpComments";
            this.grpComments.Size = new System.Drawing.Size(201, 148);
            this.grpComments.TabIndex = 16;
            this.grpComments.TabStop = false;
            this.grpComments.Text = "Comments";
            // 
            // rtxtComments
            // 
            this.rtxtComments.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.rtxtComments.Enabled = false;
            this.rtxtComments.Location = new System.Drawing.Point(19, 28);
            this.rtxtComments.Name = "rtxtComments";
            this.rtxtComments.ReadOnly = true;
            this.rtxtComments.Size = new System.Drawing.Size(164, 97);
            this.rtxtComments.TabIndex = 0;
            this.rtxtComments.Text = "";
            // 
            // FrmMemberScores
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(790, 686);
            this.Controls.Add(this.grpComments);
            this.Controls.Add(this.grpReports);
            this.Controls.Add(this.grpLeaders);
            this.Controls.Add(this.grpTournamentFile);
            this.Controls.Add(this.grpStats);
            this.Controls.Add(this.lblHandiCap);
            this.Controls.Add(this.lblStratchScores);
            this.Controls.Add(this.grpRecord);
            this.Controls.Add(this.txtHandicapTotal);
            this.Controls.Add(this.txtScratchTotal);
            this.Controls.Add(this.txtHandicapScore4);
            this.Controls.Add(this.txtScratchScore4);
            this.Controls.Add(this.txtHandicapScore3);
            this.Controls.Add(this.txtScratchScore3);
            this.Controls.Add(this.txtHandicapScore2);
            this.Controls.Add(this.txtScratchScore2);
            this.Controls.Add(this.txtHandicapScore1);
            this.Controls.Add(this.txtScratchScore1);
            this.Controls.Add(this.grpMemberStatus);
            this.Controls.Add(this.lblMiddleInitial);
            this.Controls.Add(this.lblFirstName);
            this.Controls.Add(this.lblLastName);
            this.Controls.Add(this.txtMiddleInitial);
            this.Controls.Add(this.txtFirstName);
            this.Controls.Add(this.txtLastName);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.grpMemberNum);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FrmMemberScores";
            this.Text = "Member Scores";
            this.Activated += new System.EventHandler(this.FrmMemberScores_Activated);
            this.Load += new System.EventHandler(this.FrmMemberScores_Load);
            this.grpMemberNum.ResumeLayout(false);
            this.grpMemberNum.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.grpMemberStatus.ResumeLayout(false);
            this.grpMemberStatus.PerformLayout();
            this.grpRecord.ResumeLayout(false);
            this.grpStats.ResumeLayout(false);
            this.grpStats.PerformLayout();
            this.grpTournamentFile.ResumeLayout(false);
            this.grpLeaders.ResumeLayout(false);
            this.grpLeaders.PerformLayout();
            this.grpReports.ResumeLayout(false);
            this.grpComments.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox grpMemberNum;
        private System.Windows.Forms.TextBox txtMemberNum;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rdoSquadOne;
        private System.Windows.Forms.RadioButton rdoSquadFour;
        private System.Windows.Forms.RadioButton rdoSquad3;
        private System.Windows.Forms.RadioButton rdoSquadTwo;
        private System.Windows.Forms.TextBox txtLastName;
        private System.Windows.Forms.TextBox txtFirstName;
        private System.Windows.Forms.TextBox txtMiddleInitial;
        private System.Windows.Forms.Label lblLastName;
        private System.Windows.Forms.Label lblFirstName;
        private System.Windows.Forms.Label lblMiddleInitial;
        private System.Windows.Forms.GroupBox grpMemberStatus;
        private System.Windows.Forms.TextBox txtScratchScore1;
        private System.Windows.Forms.TextBox txtHandicapScore1;
        private System.Windows.Forms.TextBox txtScratchScore2;
        private System.Windows.Forms.TextBox txtHandicapScore2;
        private System.Windows.Forms.TextBox txtScratchScore3;
        private System.Windows.Forms.TextBox txtHandicapScore3;
        private System.Windows.Forms.TextBox txtScratchScore4;
        private System.Windows.Forms.TextBox txtHandicapScore4;
        private System.Windows.Forms.TextBox txtScratchTotal;
        private System.Windows.Forms.TextBox txtHandicapTotal;
        private System.Windows.Forms.GroupBox grpRecord;
        private System.Windows.Forms.Button btnRightArrow;
        private System.Windows.Forms.Button btnLeftArrow;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button btnLastFile;
        private System.Windows.Forms.Button btnNew;
        private System.Windows.Forms.Label lblStratchScores;
        private System.Windows.Forms.Label lblHandiCap;
        private System.Windows.Forms.GroupBox grpStats;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Label lblBonusPins;
        private System.Windows.Forms.Label lblCap;
        private System.Windows.Forms.TextBox txtBonusPins;
        private System.Windows.Forms.TextBox txtHandicap;
        private System.Windows.Forms.GroupBox grpTournamentFile;
        private System.Windows.Forms.GroupBox grpLeaders;
        private System.Windows.Forms.RichTextBox txtTournamentFile;
        private System.Windows.Forms.GroupBox grpReports;
        private System.Windows.Forms.Button btnSeries;
        private System.Windows.Forms.Button btnGame;
        private System.Windows.Forms.Button btnSenior;
        private System.Windows.Forms.Button btnRefresh3;
        private System.Windows.Forms.Button btnRefresh2;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.RichTextBox richTextBox3;
        private System.Windows.Forms.RichTextBox richTextBox2;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Label lblSeries;
        private System.Windows.Forms.Label lblGameSenior;
        private System.Windows.Forms.Label lblHighGame;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton rdoActive;
        private System.Windows.Forms.GroupBox grpComments;
        private System.Windows.Forms.RichTextBox rtxtComments;
    }
}