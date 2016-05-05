namespace NineTapTour.Forms
{
    partial class FrmTournamentStats
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
            this.lblTournamentLocation = new System.Windows.Forms.Label();
            this.lblTournamentDate = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblTournamentName
            // 
            this.lblTournamentName.AutoSize = true;
            this.lblTournamentName.Location = new System.Drawing.Point(31, 26);
            this.lblTournamentName.Name = "lblTournamentName";
            this.lblTournamentName.Size = new System.Drawing.Size(70, 25);
            this.lblTournamentName.TabIndex = 0;
            this.lblTournamentName.Text = "label1";
            // 
            // lblTournamentLocation
            // 
            this.lblTournamentLocation.AutoSize = true;
            this.lblTournamentLocation.Location = new System.Drawing.Point(341, 25);
            this.lblTournamentLocation.Name = "lblTournamentLocation";
            this.lblTournamentLocation.Size = new System.Drawing.Size(70, 25);
            this.lblTournamentLocation.TabIndex = 1;
            this.lblTournamentLocation.Text = "label1";
            // 
            // lblTournamentDate
            // 
            this.lblTournamentDate.AutoSize = true;
            this.lblTournamentDate.Location = new System.Drawing.Point(641, 25);
            this.lblTournamentDate.Name = "lblTournamentDate";
            this.lblTournamentDate.Size = new System.Drawing.Size(70, 25);
            this.lblTournamentDate.TabIndex = 2;
            this.lblTournamentDate.Text = "label1";
            // 
            // FrmTournamentStats
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1394, 1308);
            this.Controls.Add(this.lblTournamentDate);
            this.Controls.Add(this.lblTournamentLocation);
            this.Controls.Add(this.lblTournamentName);
            this.Name = "FrmTournamentStats";
            this.Text = "FrmTournamentStats";
            this.Load += new System.EventHandler(this.FrmTournamentStats_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTournamentName;
        private System.Windows.Forms.Label lblTournamentLocation;
        private System.Windows.Forms.Label lblTournamentDate;
    }
}