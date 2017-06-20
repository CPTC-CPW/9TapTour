namespace NineTapTour.Database
{
    partial class PlayerHistory
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
            this.lblFullName = new System.Windows.Forms.Label();
            this.lblMemberNumber = new System.Windows.Forms.Label();
            this.lblMemberSrartAvg = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblFullName
            // 
            this.lblFullName.AutoSize = true;
            this.lblFullName.Font = new System.Drawing.Font("Calibri", 30F, System.Drawing.FontStyle.Bold);
            this.lblFullName.Location = new System.Drawing.Point(22, 9);
            this.lblFullName.Name = "lblFullName";
            this.lblFullName.Size = new System.Drawing.Size(141, 49);
            this.lblFullName.TabIndex = 3;
            this.lblFullName.Text = "#Name";
            this.lblFullName.Click += new System.EventHandler(this.lblFullName_Click);
            // 
            // lblMemberNumber
            // 
            this.lblMemberNumber.AutoSize = true;
            this.lblMemberNumber.Font = new System.Drawing.Font("Calibri", 20F, System.Drawing.FontStyle.Bold);
            this.lblMemberNumber.Location = new System.Drawing.Point(25, 68);
            this.lblMemberNumber.Name = "lblMemberNumber";
            this.lblMemberNumber.Size = new System.Drawing.Size(205, 33);
            this.lblMemberNumber.TabIndex = 4;
            this.lblMemberNumber.Text = "MemberNumber";
            // 
            // lblMemberSrartAvg
            // 
            this.lblMemberSrartAvg.AutoSize = true;
            this.lblMemberSrartAvg.Font = new System.Drawing.Font("Calibri", 20F, System.Drawing.FontStyle.Bold);
            this.lblMemberSrartAvg.Location = new System.Drawing.Point(25, 111);
            this.lblMemberSrartAvg.Name = "lblMemberSrartAvg";
            this.lblMemberSrartAvg.Size = new System.Drawing.Size(206, 33);
            this.lblMemberSrartAvg.TabIndex = 5;
            this.lblMemberSrartAvg.Text = "MemberStartavg";
            // 
            // PlayerHistory
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(709, 419);
            this.Controls.Add(this.lblMemberSrartAvg);
            this.Controls.Add(this.lblMemberNumber);
            this.Controls.Add(this.lblFullName);
            this.Name = "PlayerHistory";
            this.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Text = "Player History";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblFullName;
        private System.Windows.Forms.Label lblMemberNumber;
        private System.Windows.Forms.Label lblMemberSrartAvg;
    }
}