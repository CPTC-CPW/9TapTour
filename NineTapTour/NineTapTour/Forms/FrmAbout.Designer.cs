namespace NineTapTour.Forms {
    partial class FrmAbout {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
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
        private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmAbout));
            this.rtbFrmAboutText = new System.Windows.Forms.RichTextBox();
            this.pbAboutFormLogo = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pbAboutFormLogo)).BeginInit();
            this.SuspendLayout();
            // 
            // rtbAboutFormText
            // 
            this.rtbFrmAboutText.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(102)))));
            this.rtbFrmAboutText.ForeColor = System.Drawing.Color.White;
            this.rtbFrmAboutText.Location = new System.Drawing.Point(81, 188);
            this.rtbFrmAboutText.Name = "rtbAboutFormText";
            this.rtbFrmAboutText.ReadOnly = true;
            this.rtbFrmAboutText.Size = new System.Drawing.Size(662, 240);
            this.rtbFrmAboutText.TabIndex = 0;
            this.rtbFrmAboutText.Text = "";
            // 
            // pbAboutFormLogo
            // 
            this.pbAboutFormLogo.Image = global::NineTapTour.Properties.Resources._9taplogo1;
            this.pbAboutFormLogo.Location = new System.Drawing.Point(297, 12);
            this.pbAboutFormLogo.Name = "pbAboutFormLogo";
            this.pbAboutFormLogo.Size = new System.Drawing.Size(266, 159);
            this.pbAboutFormLogo.TabIndex = 1;
            this.pbAboutFormLogo.TabStop = false;
            // 
            // FrmAbout
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(102)))));
            this.ClientSize = new System.Drawing.Size(849, 450);
            this.Controls.Add(this.pbAboutFormLogo);
            this.Controls.Add(this.rtbFrmAboutText);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FrmAbout";
            this.Text = "FrmAbout";
            ((System.ComponentModel.ISupportInitialize)(this.pbAboutFormLogo)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox rtbFrmAboutText;
        private System.Windows.Forms.PictureBox pbAboutFormLogo;
    }
}