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
            this.lblMemNumber.Location = new System.Drawing.Point(16, 38);
            this.lblMemNumber.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblMemNumber.Name = "lblMemNumber";
            this.lblMemNumber.Size = new System.Drawing.Size(66, 17);
            this.lblMemNumber.TabIndex = 0;
            this.lblMemNumber.Text = "Number: ";
            // 
            // txtMemNumber
            // 
            this.txtMemNumber.Location = new System.Drawing.Point(90, 35);
            this.txtMemNumber.Margin = new System.Windows.Forms.Padding(4);
            this.txtMemNumber.MaxLength = 5;
            this.txtMemNumber.Name = "txtMemNumber";
            this.txtMemNumber.Size = new System.Drawing.Size(68, 22);
            this.txtMemNumber.TabIndex = 1;
            // 
            // btnSearch
            // 
            this.btnSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSearch.Location = new System.Drawing.Point(869, 162);
            this.btnSearch.Margin = new System.Windows.Forms.Padding(4);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(100, 28);
            this.btnSearch.TabIndex = 8;
            this.btnSearch.Text = "Search";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(835, 513);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(4);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 28);
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
            this.dtagrdResults.Location = new System.Drawing.Point(12, 197);
            this.dtagrdResults.MultiSelect = false;
            this.dtagrdResults.Name = "dtagrdResults";
            this.dtagrdResults.ReadOnly = true;
            this.dtagrdResults.RowTemplate.Height = 24;
            this.dtagrdResults.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dtagrdResults.Size = new System.Drawing.Size(958, 293);
            this.dtagrdResults.TabIndex = 5;
            this.dtagrdResults.TabStop = false;
            this.dtagrdResults.DoubleClick += new System.EventHandler(this.btnSelect_Click);
            // 
            // lblFirstName
            // 
            this.lblFirstName.AutoSize = true;
            this.lblFirstName.Location = new System.Drawing.Point(203, 38);
            this.lblFirstName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblFirstName.Name = "lblFirstName";
            this.lblFirstName.Size = new System.Drawing.Size(80, 17);
            this.lblFirstName.TabIndex = 6;
            this.lblFirstName.Text = "First Name:";
            // 
            // lblLastName
            // 
            this.lblLastName.AutoSize = true;
            this.lblLastName.Location = new System.Drawing.Point(544, 38);
            this.lblLastName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblLastName.Name = "lblLastName";
            this.lblLastName.Size = new System.Drawing.Size(80, 17);
            this.lblLastName.TabIndex = 7;
            this.lblLastName.Text = "Last Name:";
            // 
            // txtFirstName
            // 
            this.txtFirstName.Location = new System.Drawing.Point(291, 35);
            this.txtFirstName.Margin = new System.Windows.Forms.Padding(4);
            this.txtFirstName.Name = "txtFirstName";
            this.txtFirstName.Size = new System.Drawing.Size(213, 22);
            this.txtFirstName.TabIndex = 2;
            // 
            // txtLastName
            // 
            this.txtLastName.Location = new System.Drawing.Point(632, 35);
            this.txtLastName.Margin = new System.Windows.Forms.Padding(4);
            this.txtLastName.Name = "txtLastName";
            this.txtLastName.Size = new System.Drawing.Size(213, 22);
            this.txtLastName.TabIndex = 3;
            // 
            // btnSelect
            // 
            this.btnSelect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSelect.Location = new System.Drawing.Point(708, 513);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(100, 28);
            this.btnSelect.TabIndex = 11;
            this.btnSelect.Text = "Select";
            this.btnSelect.UseVisualStyleBackColor = true;
            this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
            // 
            // chkAdvancedView
            // 
            this.chkAdvancedView.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkAdvancedView.AutoSize = true;
            this.chkAdvancedView.Location = new System.Drawing.Point(32, 517);
            this.chkAdvancedView.Name = "chkAdvancedView";
            this.chkAdvancedView.Size = new System.Drawing.Size(126, 21);
            this.chkAdvancedView.TabIndex = 9;
            this.chkAdvancedView.Text = "Advanced View";
            this.chkAdvancedView.UseVisualStyleBackColor = true;
            this.chkAdvancedView.CheckStateChanged += new System.EventHandler(this.chkAdvancedView_CheckStateChanged);
            // 
            // lblAverage
            // 
            this.lblAverage.AutoSize = true;
            this.lblAverage.Location = new System.Drawing.Point(16, 145);
            this.lblAverage.Name = "lblAverage";
            this.lblAverage.Size = new System.Drawing.Size(65, 17);
            this.lblAverage.TabIndex = 12;
            this.lblAverage.Text = "Average:";
            // 
            // txtAverage
            // 
            this.txtAverage.Location = new System.Drawing.Point(97, 142);
            this.txtAverage.MaxLength = 10;
            this.txtAverage.Name = "txtAverage";
            this.txtAverage.Size = new System.Drawing.Size(80, 22);
            this.txtAverage.TabIndex = 5;
            // 
            // lblHandicap
            // 
            this.lblHandicap.AutoSize = true;
            this.lblHandicap.Location = new System.Drawing.Point(288, 145);
            this.lblHandicap.Name = "lblHandicap";
            this.lblHandicap.Size = new System.Drawing.Size(72, 17);
            this.lblHandicap.TabIndex = 15;
            this.lblHandicap.Text = "Handicap:";
            // 
            // lblBonus
            // 
            this.lblBonus.AutoSize = true;
            this.lblBonus.Location = new System.Drawing.Point(562, 145);
            this.lblBonus.Name = "lblBonus";
            this.lblBonus.Size = new System.Drawing.Size(52, 17);
            this.lblBonus.TabIndex = 16;
            this.lblBonus.Text = "Bonus:";
            // 
            // txtHandicap
            // 
            this.txtHandicap.Location = new System.Drawing.Point(366, 142);
            this.txtHandicap.MaxLength = 10;
            this.txtHandicap.Name = "txtHandicap";
            this.txtHandicap.Size = new System.Drawing.Size(80, 22);
            this.txtHandicap.TabIndex = 6;
            // 
            // txtBonus
            // 
            this.txtBonus.Location = new System.Drawing.Point(620, 142);
            this.txtBonus.MaxLength = 10;
            this.txtBonus.Name = "txtBonus";
            this.txtBonus.Size = new System.Drawing.Size(80, 22);
            this.txtBonus.TabIndex = 7;
            // 
            // rdoActiveEither
            // 
            this.rdoActiveEither.AutoSize = true;
            this.rdoActiveEither.Checked = true;
            this.rdoActiveEither.Location = new System.Drawing.Point(6, 22);
            this.rdoActiveEither.Name = "rdoActiveEither";
            this.rdoActiveEither.Size = new System.Drawing.Size(66, 21);
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
            this.grpIsActive.Location = new System.Drawing.Point(19, 75);
            this.grpIsActive.Name = "grpIsActive";
            this.grpIsActive.Size = new System.Drawing.Size(196, 50);
            this.grpIsActive.TabIndex = 4;
            this.grpIsActive.TabStop = false;
            this.grpIsActive.Text = "Is Active?";
            // 
            // rdoActiveNo
            // 
            this.rdoActiveNo.AutoSize = true;
            this.rdoActiveNo.Location = new System.Drawing.Point(137, 22);
            this.rdoActiveNo.Name = "rdoActiveNo";
            this.rdoActiveNo.Size = new System.Drawing.Size(47, 21);
            this.rdoActiveNo.TabIndex = 22;
            this.rdoActiveNo.TabStop = true;
            this.rdoActiveNo.Text = "No";
            this.rdoActiveNo.UseVisualStyleBackColor = true;
            // 
            // rdoActiveYes
            // 
            this.rdoActiveYes.AutoSize = true;
            this.rdoActiveYes.Location = new System.Drawing.Point(78, 22);
            this.rdoActiveYes.Name = "rdoActiveYes";
            this.rdoActiveYes.Size = new System.Drawing.Size(53, 21);
            this.rdoActiveYes.TabIndex = 21;
            this.rdoActiveYes.TabStop = true;
            this.rdoActiveYes.Text = "Yes";
            this.rdoActiveYes.UseVisualStyleBackColor = true;
            // 
            // btnClear
            // 
            this.btnClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClear.Location = new System.Drawing.Point(583, 513);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(100, 28);
            this.btnClear.TabIndex = 10;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // FrmSearch
            // 
            this.AcceptButton = this.btnSearch;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(982, 553);
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
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MinimumSize = new System.Drawing.Size(900, 400);
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