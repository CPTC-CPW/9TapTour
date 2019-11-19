namespace NineTapTour.Forms
{
    partial class FrmSearch
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmSearch));
            this.lblMemNumber = new System.Windows.Forms.Label();
            this.txtMemNumber = new System.Windows.Forms.TextBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.dtagrdResults = new System.Windows.Forms.DataGridView();
            this.lblFirstName = new System.Windows.Forms.Label();
            this.lblLastName = new System.Windows.Forms.Label();
            this.txtFirstName = new System.Windows.Forms.TextBox();
            this.txtLastName = new System.Windows.Forms.TextBox();
            this.btnSelect = new System.Windows.Forms.Button();
            this.chkAdvancedView = new System.Windows.Forms.CheckBox();
            this.lblAverage = new System.Windows.Forms.Label();
            this.txtAverage = new System.Windows.Forms.TextBox();
            this.lblHandicap = new System.Windows.Forms.Label();
            this.lblBonus = new System.Windows.Forms.Label();
            this.txtHandicap = new System.Windows.Forms.TextBox();
            this.txtBonus = new System.Windows.Forms.TextBox();
            this.rdoActiveEither = new System.Windows.Forms.RadioButton();
            this.grpIsActive = new System.Windows.Forms.GroupBox();
            this.rdoActiveNo = new System.Windows.Forms.RadioButton();
            this.rdoActiveYes = new System.Windows.Forms.RadioButton();
            this.btnClear = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dtagrdResults)).BeginInit();
            this.grpIsActive.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblMemNumber
            // 
            this.lblMemNumber.AutoSize = true;
            this.lblMemNumber.Location = new System.Drawing.Point(12, 31);
            this.lblMemNumber.Name = "lblMemNumber";
            this.lblMemNumber.Size = new System.Drawing.Size(50, 13);
            this.lblMemNumber.TabIndex = 0;
            this.lblMemNumber.Text = "Number: ";
            // 
            // txtMemNumber
            // 
            this.txtMemNumber.BackColor = System.Drawing.SystemColors.Control;
            this.txtMemNumber.Location = new System.Drawing.Point(68, 28);
            this.txtMemNumber.MaxLength = 5;
            this.txtMemNumber.Name = "txtMemNumber";
            this.txtMemNumber.Size = new System.Drawing.Size(52, 20);
            this.txtMemNumber.TabIndex = 1;
            // 
            // btnSearch
            // 
            this.btnSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSearch.Location = new System.Drawing.Point(652, 132);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 23);
            this.btnSearch.TabIndex = 8;
            this.btnSearch.Text = "Search";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(626, 417);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 12;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // dtagrdResults
            // 
            this.dtagrdResults.AllowUserToAddRows = false;
            this.dtagrdResults.AllowUserToDeleteRows = false;
            this.dtagrdResults.AllowUserToOrderColumns = true;
            this.dtagrdResults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dtagrdResults.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dtagrdResults.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dtagrdResults.Location = new System.Drawing.Point(9, 160);
            this.dtagrdResults.Margin = new System.Windows.Forms.Padding(2);
            this.dtagrdResults.MultiSelect = false;
            this.dtagrdResults.Name = "dtagrdResults";
            this.dtagrdResults.ReadOnly = true;
            this.dtagrdResults.RowTemplate.Height = 24;
            this.dtagrdResults.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dtagrdResults.Size = new System.Drawing.Size(718, 238);
            this.dtagrdResults.TabIndex = 5;
            this.dtagrdResults.TabStop = false;
            this.dtagrdResults.DoubleClick += new System.EventHandler(this.btnSelect_Click);
            // 
            // lblFirstName
            // 
            this.lblFirstName.AutoSize = true;
            this.lblFirstName.Location = new System.Drawing.Point(152, 31);
            this.lblFirstName.Name = "lblFirstName";
            this.lblFirstName.Size = new System.Drawing.Size(60, 13);
            this.lblFirstName.TabIndex = 6;
            this.lblFirstName.Text = "First Name:";
            // 
            // lblLastName
            // 
            this.lblLastName.AutoSize = true;
            this.lblLastName.Location = new System.Drawing.Point(408, 31);
            this.lblLastName.Name = "lblLastName";
            this.lblLastName.Size = new System.Drawing.Size(61, 13);
            this.lblLastName.TabIndex = 7;
            this.lblLastName.Text = "Last Name:";
            // 
            // txtFirstName
            // 
            this.txtFirstName.BackColor = System.Drawing.SystemColors.Control;
            this.txtFirstName.Location = new System.Drawing.Point(218, 28);
            this.txtFirstName.Name = "txtFirstName";
            this.txtFirstName.Size = new System.Drawing.Size(161, 20);
            this.txtFirstName.TabIndex = 2;
            // 
            // txtLastName
            // 
            this.txtLastName.BackColor = System.Drawing.SystemColors.Control;
            this.txtLastName.Location = new System.Drawing.Point(474, 28);
            this.txtLastName.Name = "txtLastName";
            this.txtLastName.Size = new System.Drawing.Size(161, 20);
            this.txtLastName.TabIndex = 3;
            // 
            // btnSelect
            // 
            this.btnSelect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSelect.Location = new System.Drawing.Point(531, 417);
            this.btnSelect.Margin = new System.Windows.Forms.Padding(2);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(75, 23);
            this.btnSelect.TabIndex = 11;
            this.btnSelect.Text = "Select";
            this.btnSelect.UseVisualStyleBackColor = true;
            this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
            // 
            // chkAdvancedView
            // 
            this.chkAdvancedView.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkAdvancedView.AutoSize = true;
            this.chkAdvancedView.Location = new System.Drawing.Point(24, 420);
            this.chkAdvancedView.Margin = new System.Windows.Forms.Padding(2);
            this.chkAdvancedView.Name = "chkAdvancedView";
            this.chkAdvancedView.Size = new System.Drawing.Size(101, 17);
            this.chkAdvancedView.TabIndex = 9;
            this.chkAdvancedView.Text = "Advanced View";
            this.chkAdvancedView.UseVisualStyleBackColor = true;
            this.chkAdvancedView.CheckStateChanged += new System.EventHandler(this.chkAdvancedView_CheckStateChanged);
            // 
            // lblAverage
            // 
            this.lblAverage.AutoSize = true;
            this.lblAverage.Location = new System.Drawing.Point(12, 118);
            this.lblAverage.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblAverage.Name = "lblAverage";
            this.lblAverage.Size = new System.Drawing.Size(50, 13);
            this.lblAverage.TabIndex = 12;
            this.lblAverage.Text = "Average:";
            // 
            // txtAverage
            // 
            this.txtAverage.BackColor = System.Drawing.SystemColors.Control;
            this.txtAverage.Location = new System.Drawing.Point(73, 115);
            this.txtAverage.Margin = new System.Windows.Forms.Padding(2);
            this.txtAverage.MaxLength = 10;
            this.txtAverage.Name = "txtAverage";
            this.txtAverage.Size = new System.Drawing.Size(61, 20);
            this.txtAverage.TabIndex = 5;
            // 
            // lblHandicap
            // 
            this.lblHandicap.AutoSize = true;
            this.lblHandicap.Location = new System.Drawing.Point(216, 118);
            this.lblHandicap.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblHandicap.Name = "lblHandicap";
            this.lblHandicap.Size = new System.Drawing.Size(56, 13);
            this.lblHandicap.TabIndex = 15;
            this.lblHandicap.Text = "Handicap:";
            // 
            // lblBonus
            // 
            this.lblBonus.AutoSize = true;
            this.lblBonus.Location = new System.Drawing.Point(422, 118);
            this.lblBonus.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblBonus.Name = "lblBonus";
            this.lblBonus.Size = new System.Drawing.Size(40, 13);
            this.lblBonus.TabIndex = 16;
            this.lblBonus.Text = "Bonus:";
            // 
            // txtHandicap
            // 
            this.txtHandicap.BackColor = System.Drawing.SystemColors.Control;
            this.txtHandicap.Location = new System.Drawing.Point(274, 115);
            this.txtHandicap.Margin = new System.Windows.Forms.Padding(2);
            this.txtHandicap.MaxLength = 10;
            this.txtHandicap.Name = "txtHandicap";
            this.txtHandicap.Size = new System.Drawing.Size(61, 20);
            this.txtHandicap.TabIndex = 6;
            // 
            // txtBonus
            // 
            this.txtBonus.BackColor = System.Drawing.SystemColors.Control;
            this.txtBonus.Location = new System.Drawing.Point(465, 115);
            this.txtBonus.Margin = new System.Windows.Forms.Padding(2);
            this.txtBonus.MaxLength = 10;
            this.txtBonus.Name = "txtBonus";
            this.txtBonus.Size = new System.Drawing.Size(61, 20);
            this.txtBonus.TabIndex = 7;
            // 
            // rdoActiveEither
            // 
            this.rdoActiveEither.AutoSize = true;
            this.rdoActiveEither.Checked = true;
            this.rdoActiveEither.Location = new System.Drawing.Point(4, 18);
            this.rdoActiveEither.Margin = new System.Windows.Forms.Padding(2);
            this.rdoActiveEither.Name = "rdoActiveEither";
            this.rdoActiveEither.Size = new System.Drawing.Size(52, 17);
            this.rdoActiveEither.TabIndex = 19;
            this.rdoActiveEither.TabStop = true;
            this.rdoActiveEither.Text = "Either";
            this.rdoActiveEither.UseVisualStyleBackColor = true;
            // 
            // grpIsActive
            // 
            this.grpIsActive.Controls.Add(this.rdoActiveNo);
            this.grpIsActive.Controls.Add(this.rdoActiveEither);
            this.grpIsActive.Controls.Add(this.rdoActiveYes);
            this.grpIsActive.Location = new System.Drawing.Point(14, 61);
            this.grpIsActive.Margin = new System.Windows.Forms.Padding(2);
            this.grpIsActive.Name = "grpIsActive";
            this.grpIsActive.Padding = new System.Windows.Forms.Padding(2);
            this.grpIsActive.Size = new System.Drawing.Size(147, 41);
            this.grpIsActive.TabIndex = 4;
            this.grpIsActive.TabStop = false;
            this.grpIsActive.Text = "Is Active?";
            // 
            // rdoActiveNo
            // 
            this.rdoActiveNo.AutoSize = true;
            this.rdoActiveNo.Location = new System.Drawing.Point(103, 18);
            this.rdoActiveNo.Margin = new System.Windows.Forms.Padding(2);
            this.rdoActiveNo.Name = "rdoActiveNo";
            this.rdoActiveNo.Size = new System.Drawing.Size(39, 17);
            this.rdoActiveNo.TabIndex = 22;
            this.rdoActiveNo.Text = "No";
            this.rdoActiveNo.UseVisualStyleBackColor = true;
            // 
            // rdoActiveYes
            // 
            this.rdoActiveYes.AutoSize = true;
            this.rdoActiveYes.Location = new System.Drawing.Point(58, 18);
            this.rdoActiveYes.Margin = new System.Windows.Forms.Padding(2);
            this.rdoActiveYes.Name = "rdoActiveYes";
            this.rdoActiveYes.Size = new System.Drawing.Size(43, 17);
            this.rdoActiveYes.TabIndex = 21;
            this.rdoActiveYes.Text = "Yes";
            this.rdoActiveYes.UseVisualStyleBackColor = true;
            // 
            // btnClear
            // 
            this.btnClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClear.Location = new System.Drawing.Point(437, 417);
            this.btnClear.Margin = new System.Windows.Forms.Padding(2);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(75, 23);
            this.btnClear.TabIndex = 10;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // FrmSearch
            // 
            this.AcceptButton = this.btnSearch;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(736, 449);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.grpIsActive);
            this.Controls.Add(this.txtBonus);
            this.Controls.Add(this.txtHandicap);
            this.Controls.Add(this.lblBonus);
            this.Controls.Add(this.lblHandicap);
            this.Controls.Add(this.txtAverage);
            this.Controls.Add(this.lblAverage);
            this.Controls.Add(this.chkAdvancedView);
            this.Controls.Add(this.btnSelect);
            this.Controls.Add(this.txtLastName);
            this.Controls.Add(this.txtFirstName);
            this.Controls.Add(this.lblLastName);
            this.Controls.Add(this.lblFirstName);
            this.Controls.Add(this.dtagrdResults);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.txtMemNumber);
            this.Controls.Add(this.lblMemNumber);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(679, 332);
            this.Name = "FrmSearch";
            this.Text = "Search Members";
            this.Load += new System.EventHandler(this.FrmSearch_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dtagrdResults)).EndInit();
            this.grpIsActive.ResumeLayout(false);
            this.grpIsActive.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblMemNumber;
        private System.Windows.Forms.TextBox txtMemNumber;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.DataGridView dtagrdResults;
        private System.Windows.Forms.Label lblFirstName;
        private System.Windows.Forms.Label lblLastName;
        private System.Windows.Forms.TextBox txtFirstName;
        private System.Windows.Forms.TextBox txtLastName;
        private System.Windows.Forms.Button btnSelect;
        private System.Windows.Forms.CheckBox chkAdvancedView;
        private System.Windows.Forms.Label lblAverage;
        private System.Windows.Forms.TextBox txtAverage;
        private System.Windows.Forms.Label lblHandicap;
        private System.Windows.Forms.Label lblBonus;
        private System.Windows.Forms.TextBox txtHandicap;
        private System.Windows.Forms.TextBox txtBonus;
        private System.Windows.Forms.RadioButton rdoActiveEither;
        private System.Windows.Forms.GroupBox grpIsActive;
        private System.Windows.Forms.RadioButton rdoActiveNo;
        private System.Windows.Forms.RadioButton rdoActiveYes;
        private System.Windows.Forms.Button btnClear;
    }
}