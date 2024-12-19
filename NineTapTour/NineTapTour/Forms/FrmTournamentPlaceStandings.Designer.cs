namespace NineTapTour.Forms
{
    partial class FrmTournamentPlaceStandings
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmTournamentPlaceStandings));
            this.dgvTournamentStandings = new System.Windows.Forms.DataGridView();
            this.lblTournamentPlaceStandings = new System.Windows.Forms.Label();
            this.lblTournamentName = new System.Windows.Forms.Label();
            this.btnPrint = new System.Windows.Forms.Button();
            this.printDialog1 = new System.Windows.Forms.PrintDialog();
            this.printDocument1 = new System.Drawing.Printing.PrintDocument();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTournamentStandings)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvTournamentStandings
            // 
            this.dgvTournamentStandings.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTournamentStandings.Location = new System.Drawing.Point(0, 205);
            this.dgvTournamentStandings.Margin = new System.Windows.Forms.Padding(1);
            this.dgvTournamentStandings.Name = "dgvTournamentStandings";
            this.dgvTournamentStandings.RowTemplate.Height = 40;
            this.dgvTournamentStandings.Size = new System.Drawing.Size(1263, 397);
            this.dgvTournamentStandings.TabIndex = 0;
            // 
            // lblTournamentPlaceStandings
            // 
            this.lblTournamentPlaceStandings.AutoSize = true;
            this.lblTournamentPlaceStandings.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTournamentPlaceStandings.Location = new System.Drawing.Point(59, 24);
            this.lblTournamentPlaceStandings.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.lblTournamentPlaceStandings.Name = "lblTournamentPlaceStandings";
            this.lblTournamentPlaceStandings.Size = new System.Drawing.Size(308, 31);
            this.lblTournamentPlaceStandings.TabIndex = 1;
            this.lblTournamentPlaceStandings.Text = "Tournament Standings";
            // 
            // lblTournamentName
            // 
            this.lblTournamentName.AutoSize = true;
            this.lblTournamentName.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTournamentName.Location = new System.Drawing.Point(596, 34);
            this.lblTournamentName.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.lblTournamentName.Name = "lblTournamentName";
            this.lblTournamentName.Size = new System.Drawing.Size(57, 20);
            this.lblTournamentName.TabIndex = 2;
            this.lblTournamentName.Text = "label1";
            // 
            // btnPrint
            // 
            this.btnPrint.Location = new System.Drawing.Point(1187, 679);
            this.btnPrint.Margin = new System.Windows.Forms.Padding(1);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(97, 27);
            this.btnPrint.TabIndex = 3;
            this.btnPrint.Text = "Print";
            this.btnPrint.UseVisualStyleBackColor = true;
            this.btnPrint.Click += new System.EventHandler(this.BtnPrint_Click);
            // 
            // printDialog1
            // 
            this.printDialog1.UseEXDialog = true;
            // 
            // TournamentPlaceStandings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1264, 606);
            this.Controls.Add(this.btnPrint);
            this.Controls.Add(this.lblTournamentName);
            this.Controls.Add(this.lblTournamentPlaceStandings);
            this.Controls.Add(this.dgvTournamentStandings);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(1);
            this.Name = "TournamentPlaceStandings";
            this.Text = "Tournament Place Standings";
            this.Load += new System.EventHandler(this.TournamentPlaceStandings_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvTournamentStandings)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvTournamentStandings;
        private System.Windows.Forms.Label lblTournamentPlaceStandings;
        private System.Windows.Forms.Label lblTournamentName;
        private System.Windows.Forms.Button btnPrint;
        private System.Windows.Forms.PrintDialog printDialog1;
        private System.Drawing.Printing.PrintDocument printDocument1;
    }
}