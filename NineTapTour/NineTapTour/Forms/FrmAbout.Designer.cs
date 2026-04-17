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
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmAbout));
            pbAboutFormLogo = new System.Windows.Forms.PictureBox();
            lblFrmAboutText = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)pbAboutFormLogo).BeginInit();
            SuspendLayout();
            // 
            // pbAboutFormLogo
            // 
            pbAboutFormLogo.Anchor = System.Windows.Forms.AnchorStyles.Top;
            pbAboutFormLogo.Image = Properties.Resources._9taplogo1;
            pbAboutFormLogo.Location = new System.Drawing.Point(346, 14);
            pbAboutFormLogo.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            pbAboutFormLogo.Name = "pbAboutFormLogo";
            pbAboutFormLogo.Size = new System.Drawing.Size(310, 183);
            pbAboutFormLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            pbAboutFormLogo.TabIndex = 1;
            pbAboutFormLogo.TabStop = false;
            // 
            // lblFrmAboutText
            // 
            lblFrmAboutText.Anchor = System.Windows.Forms.AnchorStyles.Top;
            lblFrmAboutText.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            lblFrmAboutText.ForeColor = System.Drawing.SystemColors.Control;
            lblFrmAboutText.Location = new System.Drawing.Point(152, 227);
            lblFrmAboutText.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblFrmAboutText.Name = "lblFrmAboutText";
            lblFrmAboutText.Size = new System.Drawing.Size(782, 404);
            lblFrmAboutText.TabIndex = 2;
            lblFrmAboutText.Text = resources.GetString("lblFrmAboutText.Text");
            // 
            // FrmAbout
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.FromArgb(0, 0, 102);
            ClientSize = new System.Drawing.Size(990, 519);
            Controls.Add(lblFrmAboutText);
            Controls.Add(pbAboutFormLogo);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Name = "FrmAbout";
            Text = "About";
            ((System.ComponentModel.ISupportInitialize)pbAboutFormLogo).EndInit();
            ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.PictureBox pbAboutFormLogo;
        private System.Windows.Forms.Label lblFrmAboutText;
    }
}