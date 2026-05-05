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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmTournamentResults));
            lblTournamentResults = new System.Windows.Forms.Label();
            lblTournamentName = new System.Windows.Forms.Label();
            btnExportToExcel = new System.Windows.Forms.Button();
            dgvTournamentResults = new System.Windows.Forms.DataGridView();
            lblClientRequestCount = new System.Windows.Forms.Label();
            tbClientInputCount = new System.Windows.Forms.TextBox();
            btnPaste = new System.Windows.Forms.Button();
            lblHB = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)dgvTournamentResults).BeginInit();
            SuspendLayout();
            // 
            // lblTournamentResults
            // 
            lblTournamentResults.AutoSize = true;
            lblTournamentResults.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            lblTournamentResults.Location = new System.Drawing.Point(24, 31);
            lblTournamentResults.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblTournamentResults.Name = "lblTournamentResults";
            lblTournamentResults.Size = new System.Drawing.Size(277, 31);
            lblTournamentResults.TabIndex = 0;
            lblTournamentResults.Text = "Tournament Results";
            // 
            // lblTournamentName
            // 
            lblTournamentName.AutoSize = true;
            lblTournamentName.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            lblTournamentName.Location = new System.Drawing.Point(31, 90);
            lblTournamentName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblTournamentName.Name = "lblTournamentName";
            lblTournamentName.Size = new System.Drawing.Size(156, 20);
            lblTournamentName.TabIndex = 1;
            lblTournamentName.Text = "Tournament Name";
            // 
            // btnExportToExcel
            // 
            btnExportToExcel.Location = new System.Drawing.Point(590, 28);
            btnExportToExcel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnExportToExcel.Name = "btnExportToExcel";
            btnExportToExcel.Size = new System.Drawing.Size(154, 42);
            btnExportToExcel.TabIndex = 2;
            btnExportToExcel.Text = "Export to Excel";
            btnExportToExcel.UseVisualStyleBackColor = true;
            btnExportToExcel.Click += BtnExportToExcel_Click;
            // 
            // dgvTournamentResults
            // 
            dgvTournamentResults.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            dgvTournamentResults.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvTournamentResults.Location = new System.Drawing.Point(34, 159);
            dgvTournamentResults.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            dgvTournamentResults.Name = "dgvTournamentResults";
            dgvTournamentResults.Size = new System.Drawing.Size(710, 483);
            dgvTournamentResults.TabIndex = 0;
            dgvTournamentResults.CellEnter += DgvTournamentResults_CellEnter;
            // 
            // lblClientRequestCount
            // 
            lblClientRequestCount.AutoSize = true;
            lblClientRequestCount.Location = new System.Drawing.Point(372, 132);
            lblClientRequestCount.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblClientRequestCount.Name = "lblClientRequestCount";
            lblClientRequestCount.Size = new System.Drawing.Size(248, 15);
            lblClientRequestCount.TabIndex = 3;
            lblClientRequestCount.Text = "How many winners would you like places for?";
            // 
            // tbClientInputCount
            // 
            tbClientInputCount.Location = new System.Drawing.Point(628, 129);
            tbClientInputCount.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tbClientInputCount.Name = "tbClientInputCount";
            tbClientInputCount.Size = new System.Drawing.Size(116, 23);
            tbClientInputCount.TabIndex = 4;
            tbClientInputCount.KeyDown += TbClientInputCount_KeyDown;
            // 
            // btnPaste
            // 
            btnPaste.Location = new System.Drawing.Point(223, 129);
            btnPaste.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnPaste.Name = "btnPaste";
            btnPaste.Size = new System.Drawing.Size(120, 27);
            btnPaste.TabIndex = 5;
            btnPaste.Text = "Paste Earnings";
            btnPaste.UseVisualStyleBackColor = true;
            btnPaste.Click += BtnPaste_Click;
            // 
            // lblHB
            // 
            lblHB.AutoSize = true;
            lblHB.Location = new System.Drawing.Point(587, 655);
            lblHB.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblHB.Name = "lblHB";
            lblHB.Size = new System.Drawing.Size(148, 15);
            lblHB.TabIndex = 6;
            lblHB.Text = "* H/B = Handicap + Bonus";
            // 
            // FrmTournamentResults
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(903, 681);
            Controls.Add(lblHB);
            Controls.Add(btnPaste);
            Controls.Add(tbClientInputCount);
            Controls.Add(lblClientRequestCount);
            Controls.Add(dgvTournamentResults);
            Controls.Add(btnExportToExcel);
            Controls.Add(lblTournamentName);
            Controls.Add(lblTournamentResults);
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Name = "FrmTournamentResults";
            Text = "Tournament Results";
            Load += FrmTournamentResults_Load;
            ((System.ComponentModel.ISupportInitialize)dgvTournamentResults).EndInit();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTournamentResults;
        private System.Windows.Forms.Label lblTournamentName;
        private System.Windows.Forms.Button btnExportToExcel;
        private System.Windows.Forms.DataGridView dgvTournamentResults;
        private System.Windows.Forms.Label lblClientRequestCount;
        private System.Windows.Forms.TextBox tbClientInputCount;
        private System.Windows.Forms.Button btnPaste;
        private System.Windows.Forms.Label lblHB;
    }
}