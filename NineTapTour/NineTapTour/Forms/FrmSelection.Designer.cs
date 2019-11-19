namespace NineTapTour.Forms
{
    partial class FrmSelection
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
            this.cbxTournaments = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnSelectTournament = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // cbxTournaments
            // 
            this.cbxTournaments.FormattingEnabled = true;
            this.cbxTournaments.Location = new System.Drawing.Point(75, 25);
            this.cbxTournaments.Name = "cbxTournaments";
            this.cbxTournaments.Size = new System.Drawing.Size(211, 21);
            this.cbxTournaments.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(141, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(69, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Tournaments";
            // 
            // btnSelectTournament
            // 
            this.btnSelectTournament.Location = new System.Drawing.Point(118, 52);
            this.btnSelectTournament.Name = "btnSelectTournament";
            this.btnSelectTournament.Size = new System.Drawing.Size(112, 23);
            this.btnSelectTournament.TabIndex = 2;
            this.btnSelectTournament.Text = "Select Tournament";
            this.btnSelectTournament.UseVisualStyleBackColor = true;
            this.btnSelectTournament.Click += new System.EventHandler(this.btnSelectTournament_Click);
            // 
            // FrmSelection
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(343, 92);
            this.Controls.Add(this.btnSelectTournament);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cbxTournaments);
            this.Name = "FrmSelection";
            this.Text = "Select Tournament";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cbxTournaments;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnSelectTournament;
    }
}