namespace NineTapTour.Forms
{
    partial class TournamentStats
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
            this.lblTournamentName = new System.Windows.Forms.Label();
            this.dgvTournamentStats = new System.Windows.Forms.DataGridView();
            this.printDialog1 = new System.Windows.Forms.PrintDialog();
            this.printDocument1 = new System.Drawing.Printing.PrintDocument();
            this.btnPrint = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTournamentStats)).BeginInit();
            this.SuspendLayout();
            // 
            // lblTournamentName
            // 
            this.lblTournamentName.AutoSize = true;
            this.lblTournamentName.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTournamentName.Location = new System.Drawing.Point(14, 21);
            this.lblTournamentName.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.lblTournamentName.Name = "lblTournamentName";
            this.lblTournamentName.Size = new System.Drawing.Size(156, 20);
            this.lblTournamentName.TabIndex = 0;
            this.lblTournamentName.Text = "Tournament Name";
            // 
            // dgvTournamentStats
            // 
            this.dgvTournamentStats.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTournamentStats.Location = new System.Drawing.Point(-2, 91);
            this.dgvTournamentStats.Margin = new System.Windows.Forms.Padding(1, 1, 1, 1);
            this.dgvTournamentStats.Name = "dgvTournamentStats";
            this.dgvTournamentStats.RowTemplate.Height = 37;
            this.dgvTournamentStats.Size = new System.Drawing.Size(1265, 501);
            this.dgvTournamentStats.TabIndex = 1;
            // 
            // printDialog1
            // 
            this.printDialog1.UseEXDialog = true;
            // 
            // printDocument1
            // 
            this.printDocument1.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(this.printDocument1_PrintPage);
            // 
            // btnPrint
            // 
            this.btnPrint.Location = new System.Drawing.Point(1173, 550);
            this.btnPrint.Margin = new System.Windows.Forms.Padding(1, 1, 1, 1);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(91, 38);
            this.btnPrint.TabIndex = 2;
            this.btnPrint.Text = "Print";
            this.btnPrint.UseVisualStyleBackColor = true;
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // TournamentStats
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1264, 602);
            this.Controls.Add(this.btnPrint);
            this.Controls.Add(this.dgvTournamentStats);
            this.Controls.Add(this.lblTournamentName);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Margin = new System.Windows.Forms.Padding(1, 1, 1, 1);
            this.Name = "TournamentStats";
            this.Text = "Tournament Statistics";
            this.Load += new System.EventHandler(this.TournamentStats_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvTournamentStats)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTournamentName;
        private System.Windows.Forms.DataGridView dgvTournamentStats;
        private System.Windows.Forms.PrintDialog printDialog1;
        private System.Drawing.Printing.PrintDocument printDocument1;
        private System.Windows.Forms.Button btnPrint;
    }
}