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
            this.pbAboutFormLogo = new System.Windows.Forms.PictureBox();
            this.rtbFrmAboutText = new System.Windows.Forms.RichTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.pbAboutFormLogo)).BeginInit();
            this.SuspendLayout();
            // 
            // pbAboutFormLogo
            // 
            this.pbAboutFormLogo.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.pbAboutFormLogo.Image = global::NineTapTour.Properties.Resources._9taplogo1;
            this.pbAboutFormLogo.Location = new System.Drawing.Point(297, 12);
            this.pbAboutFormLogo.Name = "pbAboutFormLogo";
            this.pbAboutFormLogo.Size = new System.Drawing.Size(266, 159);
            this.pbAboutFormLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbAboutFormLogo.TabIndex = 1;
            this.pbAboutFormLogo.TabStop = false;
            // 
            // rtbFrmAboutText
            // 
            this.rtbFrmAboutText.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.rtbFrmAboutText.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(102)))));
            this.rtbFrmAboutText.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtbFrmAboutText.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbFrmAboutText.ForeColor = System.Drawing.Color.Black;
            this.rtbFrmAboutText.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.rtbFrmAboutText.Location = new System.Drawing.Point(96, 225);
            this.rtbFrmAboutText.Name = "rtbFrmAboutText";
            this.rtbFrmAboutText.ShortcutsEnabled = false;
            this.rtbFrmAboutText.Size = new System.Drawing.Size(662, 240);
            this.rtbFrmAboutText.TabIndex = 0;
            this.rtbFrmAboutText.TabStop = false;
            this.rtbFrmAboutText.Text = "";
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
            this.Text = "About";
            ((System.ComponentModel.ISupportInitialize)(this.pbAboutFormLogo)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.PictureBox pbAboutFormLogo;
        private System.Windows.Forms.RichTextBox rtbFrmAboutText;
    }
}