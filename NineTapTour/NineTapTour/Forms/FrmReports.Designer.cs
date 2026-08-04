namespace NineTapTour.Forms
{
    partial class FrmReports
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
            grpScope = new System.Windows.Forms.GroupBox();
            cmbMember = new System.Windows.Forms.ComboBox();
            rbIndividual = new System.Windows.Forms.RadioButton();
            rbTourWide = new System.Windows.Forms.RadioButton();
            grpPeriod = new System.Windows.Forms.GroupBox();
            cmbYearTo = new System.Windows.Forms.ComboBox();
            lblTo = new System.Windows.Forms.Label();
            cmbYearFrom = new System.Windows.Forms.ComboBox();
            cmbYear = new System.Windows.Forms.ComboBox();
            rbYearRange = new System.Windows.Forms.RadioButton();
            rbYear = new System.Windows.Forms.RadioButton();
            rbCareer = new System.Windows.Forms.RadioButton();
            grpReport = new System.Windows.Forms.GroupBox();
            chkIncludeSidePots = new System.Windows.Forms.CheckBox();
            lblTopNHint = new System.Windows.Forms.Label();
            txtTopN = new System.Windows.Forms.TextBox();
            lblTopN = new System.Windows.Forms.Label();
            cmbCategory = new System.Windows.Forms.ComboBox();
            lblCategory = new System.Windows.Forms.Label();
            btnRunReport = new System.Windows.Forms.Button();
            btnExport = new System.Windows.Forms.Button();
            dgvReport = new System.Windows.Forms.DataGridView();
            grpScope.SuspendLayout();
            grpPeriod.SuspendLayout();
            grpReport.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvReport).BeginInit();
            SuspendLayout();
            //
            // grpScope
            //
            grpScope.Controls.Add(cmbMember);
            grpScope.Controls.Add(rbIndividual);
            grpScope.Controls.Add(rbTourWide);
            grpScope.Location = new System.Drawing.Point(12, 12);
            grpScope.Name = "grpScope";
            grpScope.Size = new System.Drawing.Size(255, 130);
            grpScope.TabIndex = 0;
            grpScope.TabStop = false;
            grpScope.Text = "Report Scope";
            //
            // cmbMember
            //
            cmbMember.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            cmbMember.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            cmbMember.Enabled = false;
            cmbMember.FormattingEnabled = true;
            cmbMember.Location = new System.Drawing.Point(15, 84);
            cmbMember.Name = "cmbMember";
            cmbMember.Size = new System.Drawing.Size(225, 23);
            cmbMember.TabIndex = 2;
            //
            // rbIndividual
            //
            rbIndividual.AutoSize = true;
            rbIndividual.Location = new System.Drawing.Point(15, 54);
            rbIndividual.Name = "rbIndividual";
            rbIndividual.Size = new System.Drawing.Size(128, 19);
            rbIndividual.TabIndex = 1;
            rbIndividual.Text = "Individual Member:";
            rbIndividual.UseVisualStyleBackColor = true;
            //
            // rbTourWide
            //
            rbTourWide.AutoSize = true;
            rbTourWide.Checked = true;
            rbTourWide.Location = new System.Drawing.Point(15, 25);
            rbTourWide.Name = "rbTourWide";
            rbTourWide.Size = new System.Drawing.Size(80, 19);
            rbTourWide.TabIndex = 0;
            rbTourWide.TabStop = true;
            rbTourWide.Text = "Tour-Wide";
            rbTourWide.UseVisualStyleBackColor = true;
            rbTourWide.CheckedChanged += RbScope_CheckedChanged;
            //
            // grpPeriod
            //
            grpPeriod.Controls.Add(cmbYearTo);
            grpPeriod.Controls.Add(lblTo);
            grpPeriod.Controls.Add(cmbYearFrom);
            grpPeriod.Controls.Add(cmbYear);
            grpPeriod.Controls.Add(rbYearRange);
            grpPeriod.Controls.Add(rbYear);
            grpPeriod.Controls.Add(rbCareer);
            grpPeriod.Location = new System.Drawing.Point(280, 12);
            grpPeriod.Name = "grpPeriod";
            grpPeriod.Size = new System.Drawing.Size(340, 130);
            grpPeriod.TabIndex = 1;
            grpPeriod.TabStop = false;
            grpPeriod.Text = "Time Period";
            //
            // cmbYearTo
            //
            cmbYearTo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cmbYearTo.Enabled = false;
            cmbYearTo.FormattingEnabled = true;
            cmbYearTo.Location = new System.Drawing.Point(255, 82);
            cmbYearTo.Name = "cmbYearTo";
            cmbYearTo.Size = new System.Drawing.Size(70, 23);
            cmbYearTo.TabIndex = 6;
            //
            // lblTo
            //
            lblTo.AutoSize = true;
            lblTo.Location = new System.Drawing.Point(230, 86);
            lblTo.Name = "lblTo";
            lblTo.Size = new System.Drawing.Size(19, 15);
            lblTo.TabIndex = 5;
            lblTo.Text = "to";
            //
            // cmbYearFrom
            //
            cmbYearFrom.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cmbYearFrom.Enabled = false;
            cmbYearFrom.FormattingEnabled = true;
            cmbYearFrom.Location = new System.Drawing.Point(155, 82);
            cmbYearFrom.Name = "cmbYearFrom";
            cmbYearFrom.Size = new System.Drawing.Size(70, 23);
            cmbYearFrom.TabIndex = 4;
            //
            // cmbYear
            //
            cmbYear.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cmbYear.Enabled = false;
            cmbYear.FormattingEnabled = true;
            cmbYear.Location = new System.Drawing.Point(155, 52);
            cmbYear.Name = "cmbYear";
            cmbYear.Size = new System.Drawing.Size(70, 23);
            cmbYear.TabIndex = 3;
            //
            // rbYearRange
            //
            rbYearRange.AutoSize = true;
            rbYearRange.Location = new System.Drawing.Point(15, 84);
            rbYearRange.Name = "rbYearRange";
            rbYearRange.Size = new System.Drawing.Size(105, 19);
            rbYearRange.TabIndex = 2;
            rbYearRange.Text = "Multiple Years:";
            rbYearRange.UseVisualStyleBackColor = true;
            rbYearRange.CheckedChanged += RbPeriod_CheckedChanged;
            //
            // rbYear
            //
            rbYear.AutoSize = true;
            rbYear.Location = new System.Drawing.Point(15, 54);
            rbYear.Name = "rbYear";
            rbYear.Size = new System.Drawing.Size(98, 19);
            rbYear.TabIndex = 1;
            rbYear.Text = "Specific Year:";
            rbYear.UseVisualStyleBackColor = true;
            rbYear.CheckedChanged += RbPeriod_CheckedChanged;
            //
            // rbCareer
            //
            rbCareer.AutoSize = true;
            rbCareer.Checked = true;
            rbCareer.Location = new System.Drawing.Point(15, 25);
            rbCareer.Name = "rbCareer";
            rbCareer.Size = new System.Drawing.Size(114, 19);
            rbCareer.TabIndex = 0;
            rbCareer.TabStop = true;
            rbCareer.Text = "Career (All Years)";
            rbCareer.UseVisualStyleBackColor = true;
            rbCareer.CheckedChanged += RbPeriod_CheckedChanged;
            //
            // grpReport
            //
            grpReport.Controls.Add(chkIncludeSidePots);
            grpReport.Controls.Add(lblTopNHint);
            grpReport.Controls.Add(txtTopN);
            grpReport.Controls.Add(lblTopN);
            grpReport.Controls.Add(cmbCategory);
            grpReport.Controls.Add(lblCategory);
            grpReport.Location = new System.Drawing.Point(633, 12);
            grpReport.Name = "grpReport";
            grpReport.Size = new System.Drawing.Size(280, 130);
            grpReport.TabIndex = 2;
            grpReport.TabStop = false;
            grpReport.Text = "Report";
            //
            // chkIncludeSidePots
            //
            chkIncludeSidePots.AutoSize = true;
            chkIncludeSidePots.Location = new System.Drawing.Point(15, 90);
            chkIncludeSidePots.Name = "chkIncludeSidePots";
            chkIncludeSidePots.Size = new System.Drawing.Size(190, 19);
            chkIncludeSidePots.TabIndex = 5;
            chkIncludeSidePots.Text = "Include side pots in earnings";
            chkIncludeSidePots.UseVisualStyleBackColor = true;
            //
            // lblTopNHint
            //
            lblTopNHint.AutoSize = true;
            lblTopNHint.Location = new System.Drawing.Point(150, 60);
            lblTopNHint.Name = "lblTopNHint";
            lblTopNHint.Size = new System.Drawing.Size(66, 15);
            lblTopNHint.TabIndex = 4;
            lblTopNHint.Text = "(blank = all)";
            //
            // txtTopN
            //
            txtTopN.Location = new System.Drawing.Point(90, 56);
            txtTopN.Name = "txtTopN";
            txtTopN.Size = new System.Drawing.Size(50, 23);
            txtTopN.TabIndex = 3;
            txtTopN.Text = "25";
            //
            // lblTopN
            //
            lblTopN.AutoSize = true;
            lblTopN.Location = new System.Drawing.Point(15, 60);
            lblTopN.Name = "lblTopN";
            lblTopN.Size = new System.Drawing.Size(62, 15);
            lblTopN.TabIndex = 2;
            lblTopN.Text = "Show Top:";
            //
            // cmbCategory
            //
            cmbCategory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cmbCategory.FormattingEnabled = true;
            cmbCategory.Location = new System.Drawing.Point(90, 24);
            cmbCategory.Name = "cmbCategory";
            cmbCategory.Size = new System.Drawing.Size(175, 23);
            cmbCategory.TabIndex = 1;
            //
            // lblCategory
            //
            lblCategory.AutoSize = true;
            lblCategory.Location = new System.Drawing.Point(15, 28);
            lblCategory.Name = "lblCategory";
            lblCategory.Size = new System.Drawing.Size(58, 15);
            lblCategory.TabIndex = 0;
            lblCategory.Text = "Category:";
            //
            // btnRunReport
            //
            btnRunReport.Location = new System.Drawing.Point(930, 30);
            btnRunReport.Name = "btnRunReport";
            btnRunReport.Size = new System.Drawing.Size(130, 35);
            btnRunReport.TabIndex = 3;
            btnRunReport.Text = "Run Report";
            btnRunReport.UseVisualStyleBackColor = true;
            btnRunReport.Click += BtnRunReport_Click;
            //
            // btnExport
            //
            btnExport.Location = new System.Drawing.Point(930, 80);
            btnExport.Name = "btnExport";
            btnExport.Size = new System.Drawing.Size(130, 35);
            btnExport.TabIndex = 4;
            btnExport.Text = "Export to Excel";
            btnExport.UseVisualStyleBackColor = true;
            btnExport.Click += BtnExport_Click;
            //
            // dgvReport
            //
            dgvReport.AllowUserToAddRows = false;
            dgvReport.AllowUserToDeleteRows = false;
            dgvReport.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            dgvReport.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvReport.Location = new System.Drawing.Point(12, 155);
            dgvReport.Name = "dgvReport";
            dgvReport.ReadOnly = true;
            dgvReport.RowHeadersVisible = false;
            dgvReport.Size = new System.Drawing.Size(1060, 440);
            dgvReport.TabIndex = 5;
            //
            // FrmReports
            //
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1084, 607);
            Controls.Add(dgvReport);
            Controls.Add(btnExport);
            Controls.Add(btnRunReport);
            Controls.Add(grpReport);
            Controls.Add(grpPeriod);
            Controls.Add(grpScope);
            Name = "FrmReports";
            Text = "Reports";
            Load += FrmReports_Load;
            grpScope.ResumeLayout(false);
            grpScope.PerformLayout();
            grpPeriod.ResumeLayout(false);
            grpPeriod.PerformLayout();
            grpReport.ResumeLayout(false);
            grpReport.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvReport).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.GroupBox grpScope;
        private System.Windows.Forms.RadioButton rbIndividual;
        private System.Windows.Forms.RadioButton rbTourWide;
        private System.Windows.Forms.ComboBox cmbMember;
        private System.Windows.Forms.GroupBox grpPeriod;
        private System.Windows.Forms.RadioButton rbCareer;
        private System.Windows.Forms.RadioButton rbYear;
        private System.Windows.Forms.RadioButton rbYearRange;
        private System.Windows.Forms.ComboBox cmbYear;
        private System.Windows.Forms.ComboBox cmbYearFrom;
        private System.Windows.Forms.Label lblTo;
        private System.Windows.Forms.ComboBox cmbYearTo;
        private System.Windows.Forms.GroupBox grpReport;
        private System.Windows.Forms.Label lblCategory;
        private System.Windows.Forms.ComboBox cmbCategory;
        private System.Windows.Forms.Label lblTopN;
        private System.Windows.Forms.TextBox txtTopN;
        private System.Windows.Forms.Label lblTopNHint;
        private System.Windows.Forms.CheckBox chkIncludeSidePots;
        private System.Windows.Forms.Button btnRunReport;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.DataGridView dgvReport;
    }
}
