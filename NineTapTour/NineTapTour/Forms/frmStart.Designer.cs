namespace NineTapTour.Forms
{
    partial class FrmStart
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
            this.lblUpdates = new System.Windows.Forms.Label();
            this.lblUpdatetext = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblUpdates
            // 
            this.lblUpdates.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblUpdates.AutoSize = true;
            this.lblUpdates.BackColor = System.Drawing.Color.Transparent;
            this.lblUpdates.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUpdates.ForeColor = System.Drawing.Color.White;
            this.lblUpdates.Location = new System.Drawing.Point(340, 53);
            this.lblUpdates.Name = "lblUpdates";
            this.lblUpdates.Size = new System.Drawing.Size(225, 55);
            this.lblUpdates.TabIndex = 5;
            this.lblUpdates.Text = "Updates!";
            // 
            // lblUpdatetext
            // 
            this.lblUpdatetext.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblUpdatetext.BackColor = System.Drawing.Color.Transparent;
            this.lblUpdatetext.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblUpdatetext.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUpdatetext.ForeColor = System.Drawing.Color.White;
            this.lblUpdatetext.Location = new System.Drawing.Point(253, 108);
            this.lblUpdatetext.Name = "lblUpdatetext";
            this.lblUpdatetext.Size = new System.Drawing.Size(412, 415);
            this.lblUpdatetext.TabIndex = 6;
            this.lblUpdatetext.Text = "Updates and Notices would be pulled from the database will be put in this box so " +
    "users can see upcoming events or important notices.\r\n\r\n";
            this.lblUpdatetext.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // FrmStart
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DodgerBlue;
            this.BackgroundImage = global::NineTapTour.Properties.Resources._9tap;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.ClientSize = new System.Drawing.Size(919, 546);
            this.Controls.Add(this.lblUpdatetext);
            this.Controls.Add(this.lblUpdates);
            this.DoubleBuffered = true;
            this.Name = "FrmStart";
            this.Text = "Nine Tap Tour Notices!";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblUpdates;
        private System.Windows.Forms.Label lblUpdatetext;
    }
}