namespace NineTapTour.Forms
{
    partial class FrmTournamentResults
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
            this.lblTournamentResults = new System.Windows.Forms.Label();
            this.lblTournamentName = new System.Windows.Forms.Label();
            this.btnExportToExcel = new System.Windows.Forms.Button();
            this.dgvTournamentResults = new System.Windows.Forms.DataGridView();
            this.btnSaveChanges = new System.Windows.Forms.Button();
            this.txtTotalEntries = new System.Windows.Forms.Label();
            this.txtCalculatedCashedWinners = new System.Windows.Forms.Label();
            this.txtCompEntries = new System.Windows.Forms.Label();
            this.txtAdjustedEntries = new System.Windows.Forms.Label();
            this.txtActualCashedWinners = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTournamentResults)).BeginInit();
            this.SuspendLayout();
            // 
            // lblTournamentResults
            // 
            this.lblTournamentResults.AutoSize = true;
            this.lblTournamentResults.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTournamentResults.Location = new System.Drawing.Point(21, 9);
            this.lblTournamentResults.Name = "lblTournamentResults";
            this.lblTournamentResults.Size = new System.Drawing.Size(277, 31);
            this.lblTournamentResults.TabIndex = 0;
            this.lblTournamentResults.Text = "Tournament Results";
            // 
            // lblTournamentName
            // 
            this.lblTournamentName.AutoSize = true;
            this.lblTournamentName.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTournamentName.Location = new System.Drawing.Point(32, 59);
            this.lblTournamentName.Name = "lblTournamentName";
            this.lblTournamentName.Size = new System.Drawing.Size(156, 20);
            this.lblTournamentName.TabIndex = 1;
            this.lblTournamentName.Text = "Tournament Name";
            // 
            // btnExportToExcel
            // 
            this.btnExportToExcel.Location = new System.Drawing.Point(550, 63);
            this.btnExportToExcel.Name = "btnExportToExcel";
            this.btnExportToExcel.Size = new System.Drawing.Size(123, 36);
            this.btnExportToExcel.TabIndex = 2;
            this.btnExportToExcel.Text = "Export to Excel";
            this.btnExportToExcel.UseVisualStyleBackColor = true;
            this.btnExportToExcel.Click += new System.EventHandler(this.btnExportToExcel_Click);
            // 
            // dgvTournamentResults
            // 
            this.dgvTournamentResults.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvTournamentResults.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTournamentResults.Location = new System.Drawing.Point(35, 242);
            this.dgvTournamentResults.Name = "dgvTournamentResults";
            this.dgvTournamentResults.Size = new System.Drawing.Size(625, 313);
            this.dgvTournamentResults.TabIndex = 0;
            this.dgvTournamentResults.CellEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvTournamentResults_CellEnter);
            // 
            // btnSaveChanges
            // 
            this.btnSaveChanges.Location = new System.Drawing.Point(550, 12);
            this.btnSaveChanges.Name = "btnSaveChanges";
            this.btnSaveChanges.Size = new System.Drawing.Size(123, 36);
            this.btnSaveChanges.TabIndex = 1;
            this.btnSaveChanges.Text = "Save Changes";
            this.btnSaveChanges.UseVisualStyleBackColor = true;
            this.btnSaveChanges.Click += new System.EventHandler(this.btnSaveChanges_Click);
            // 
            // txtTotalEntries
            // 
            this.txtTotalEntries.AutoSize = true;
            this.txtTotalEntries.Location = new System.Drawing.Point(33, 95);
            this.txtTotalEntries.Name = "txtTotalEntries";
            this.txtTotalEntries.Size = new System.Drawing.Size(72, 13);
            this.txtTotalEntries.TabIndex = 3;
            this.txtTotalEntries.Text = "Total Entries: ";
            // 
            // txtCalculatedCashedWinners
            // 
            this.txtCalculatedCashedWinners.AutoSize = true;
            this.txtCalculatedCashedWinners.Location = new System.Drawing.Point(33, 187);
            this.txtCalculatedCashedWinners.Name = "txtCalculatedCashedWinners";
            this.txtCalculatedCashedWinners.Size = new System.Drawing.Size(135, 13);
            this.txtCalculatedCashedWinners.TabIndex = 4;
            this.txtCalculatedCashedWinners.Text = "Adjusted Cashed Winners: ";
            // 
            // txtCompEntries
            // 
            this.txtCompEntries.AutoSize = true;
            this.txtCompEntries.Location = new System.Drawing.Point(33, 117);
            this.txtCompEntries.Name = "txtCompEntries";
            this.txtCompEntries.Size = new System.Drawing.Size(72, 13);
            this.txtCompEntries.TabIndex = 5;
            this.txtCompEntries.Text = "Comp Entries:";
            // 
            // txtAdjustedEntries
            // 
            this.txtAdjustedEntries.AutoSize = true;
            this.txtAdjustedEntries.Location = new System.Drawing.Point(33, 139);
            this.txtAdjustedEntries.Name = "txtAdjustedEntries";
            this.txtAdjustedEntries.Size = new System.Drawing.Size(86, 13);
            this.txtAdjustedEntries.TabIndex = 6;
            this.txtAdjustedEntries.Text = "Adjusted Entries:";
            // 
            // txtActualCashedWinners
            // 
            this.txtActualCashedWinners.AutoSize = true;
            this.txtActualCashedWinners.Location = new System.Drawing.Point(33, 208);
            this.txtActualCashedWinners.Name = "txtActualCashedWinners";
            this.txtActualCashedWinners.Size = new System.Drawing.Size(124, 13);
            this.txtActualCashedWinners.TabIndex = 7;
            this.txtActualCashedWinners.Text = "Actual Cashed Winners: ";
            // 
            // FrmTournamentResults
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(696, 579);
            this.Controls.Add(this.txtActualCashedWinners);
            this.Controls.Add(this.txtAdjustedEntries);
            this.Controls.Add(this.txtCompEntries);
            this.Controls.Add(this.txtCalculatedCashedWinners);
            this.Controls.Add(this.txtTotalEntries);
            this.Controls.Add(this.btnSaveChanges);
            this.Controls.Add(this.dgvTournamentResults);
            this.Controls.Add(this.btnExportToExcel);
            this.Controls.Add(this.lblTournamentName);
            this.Controls.Add(this.lblTournamentResults);
            this.Name = "FrmTournamentResults";
            this.Text = "Tournament Results";
            this.Load += new System.EventHandler(this.FrmTournamentResults_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvTournamentResults)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTournamentResults;
        private System.Windows.Forms.Label lblTournamentName;
        private System.Windows.Forms.Button btnExportToExcel;
        private System.Windows.Forms.DataGridView dgvTournamentResults;
        private System.Windows.Forms.Button btnSaveChanges;
        private System.Windows.Forms.Label txtTotalEntries;
        private System.Windows.Forms.Label txtCalculatedCashedWinners;
        private System.Windows.Forms.Label txtCompEntries;
        private System.Windows.Forms.Label txtAdjustedEntries;
        private System.Windows.Forms.Label txtActualCashedWinners;
    }
}