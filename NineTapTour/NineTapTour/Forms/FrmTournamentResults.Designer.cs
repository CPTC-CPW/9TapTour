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
            this.lblTournamentResults = new System.Windows.Forms.Label();
            this.lblTournamentName = new System.Windows.Forms.Label();
            this.btnExportToExcel = new System.Windows.Forms.Button();
            this.dgvTournamentResults = new System.Windows.Forms.DataGridView();
            this.lblClientRequestCount = new System.Windows.Forms.Label();
            this.txtClientInputCount = new System.Windows.Forms.TextBox();
            this.btnPaste = new System.Windows.Forms.Button();
            this.lblHB = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTournamentResults)).BeginInit();
            this.SuspendLayout();
            // 
            // lblTournamentResults
            // 
            this.lblTournamentResults.AutoSize = true;
            this.lblTournamentResults.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTournamentResults.Location = new System.Drawing.Point(28, 33);
            this.lblTournamentResults.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTournamentResults.Name = "lblTournamentResults";
            this.lblTournamentResults.Size = new System.Drawing.Size(341, 39);
            this.lblTournamentResults.TabIndex = 0;
            this.lblTournamentResults.Text = "Tournament Results";
            // 
            // lblTournamentName
            // 
            this.lblTournamentName.AutoSize = true;
            this.lblTournamentName.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTournamentName.Location = new System.Drawing.Point(36, 96);
            this.lblTournamentName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTournamentName.Name = "lblTournamentName";
            this.lblTournamentName.Size = new System.Drawing.Size(190, 25);
            this.lblTournamentName.TabIndex = 1;
            this.lblTournamentName.Text = "Tournament Name";
            // 
            // btnExportToExcel
            // 
            this.btnExportToExcel.Location = new System.Drawing.Point(675, 30);
            this.btnExportToExcel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnExportToExcel.Name = "btnExportToExcel";
            this.btnExportToExcel.Size = new System.Drawing.Size(176, 44);
            this.btnExportToExcel.TabIndex = 2;
            this.btnExportToExcel.Text = "Export to Excel";
            this.btnExportToExcel.UseVisualStyleBackColor = true;
            this.btnExportToExcel.Click += new System.EventHandler(this.btnExportToExcel_Click);
            // 
            // dgvTournamentResults
            // 
            this.dgvTournamentResults.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvTournamentResults.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTournamentResults.Location = new System.Drawing.Point(39, 170);
            this.dgvTournamentResults.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.dgvTournamentResults.Name = "dgvTournamentResults";
            this.dgvTournamentResults.RowHeadersWidth = 51;
            this.dgvTournamentResults.Size = new System.Drawing.Size(812, 516);
            this.dgvTournamentResults.TabIndex = 0;
            this.dgvTournamentResults.CellEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvTournamentResults_CellEnter);
            // 
            // lblClientRequestCount
            // 
            this.lblClientRequestCount.AutoSize = true;
            this.lblClientRequestCount.Location = new System.Drawing.Point(415, 142);
            this.lblClientRequestCount.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblClientRequestCount.Name = "lblClientRequestCount";
            this.lblClientRequestCount.Size = new System.Drawing.Size(291, 17);
            this.lblClientRequestCount.TabIndex = 3;
            this.lblClientRequestCount.Text = "How many winners would you like places for?";
            // 
            // txtClientInputCount
            // 
            this.txtClientInputCount.Location = new System.Drawing.Point(717, 138);
            this.txtClientInputCount.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtClientInputCount.Name = "txtClientInputCount";
            this.txtClientInputCount.Size = new System.Drawing.Size(132, 22);
            this.txtClientInputCount.TabIndex = 4;
            this.txtClientInputCount.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TxtClientInputCount_KeyDown);
            // 
            // btnPaste
            // 
            this.btnPaste.Location = new System.Drawing.Point(255, 138);
            this.btnPaste.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnPaste.Name = "btnPaste";
            this.btnPaste.Size = new System.Drawing.Size(137, 28);
            this.btnPaste.TabIndex = 5;
            this.btnPaste.Text = "Paste Earnings";
            this.btnPaste.UseVisualStyleBackColor = true;
            this.btnPaste.Click += new System.EventHandler(this.btnPaste_Click);
            // 
            // lblHB
            // 
            this.lblHB.AutoSize = true;
            this.lblHB.Location = new System.Drawing.Point(671, 699);
            this.lblHB.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblHB.Name = "lblHB";
            this.lblHB.Size = new System.Drawing.Size(172, 17);
            this.lblHB.TabIndex = 6;
            this.lblHB.Text = "* H/B = Handicap + Bonus";
            // 
            // FrmTournamentResults
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(889, 726);
            this.Controls.Add(this.lblHB);
            this.Controls.Add(this.btnPaste);
            this.Controls.Add(this.txtClientInputCount);
            this.Controls.Add(this.lblClientRequestCount);
            this.Controls.Add(this.dgvTournamentResults);
            this.Controls.Add(this.btnExportToExcel);
            this.Controls.Add(this.lblTournamentName);
            this.Controls.Add(this.lblTournamentResults);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
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
        private System.Windows.Forms.Label lblClientRequestCount;
        private System.Windows.Forms.TextBox txtClientInputCount;
        private System.Windows.Forms.Button btnPaste;
        private System.Windows.Forms.Label lblHB;
    }
}