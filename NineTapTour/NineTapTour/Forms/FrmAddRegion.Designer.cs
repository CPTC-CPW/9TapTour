namespace NineTapTour.Forms
{
    partial class FrmAddRegion
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmAddRegion));
            this.btnSubmit = new System.Windows.Forms.Button();
            this.tbEntry = new System.Windows.Forms.TextBox();
            this.lblRegiontext = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnSubmit
            // 
            this.btnSubmit.Location = new System.Drawing.Point(101, 70);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Size = new System.Drawing.Size(75, 23);
            this.btnSubmit.TabIndex = 0;
            this.btnSubmit.Text = "Submit";
            this.btnSubmit.UseVisualStyleBackColor = true;
            this.btnSubmit.Click += new System.EventHandler(this.btnSubmit_Click);
            // 
            // tbEntry
            // 
            this.tbEntry.BackColor = System.Drawing.SystemColors.ControlLight;
            this.tbEntry.Location = new System.Drawing.Point(68, 44);
            this.tbEntry.Name = "tbEntry";
            this.tbEntry.Size = new System.Drawing.Size(138, 20);
            this.tbEntry.TabIndex = 1;
            // 
            // lblRegiontext
            // 
            this.lblRegiontext.AutoSize = true;
            this.lblRegiontext.Location = new System.Drawing.Point(54, 19);
            this.lblRegiontext.Name = "lblRegiontext";
            this.lblRegiontext.Size = new System.Drawing.Size(166, 13);
            this.lblRegiontext.TabIndex = 2;
            this.lblRegiontext.Text = "Select a name  for the new region";
            // 
            // FrmAddRegion
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(284, 95);
            this.Controls.Add(this.lblRegiontext);
            this.Controls.Add(this.tbEntry);
            this.Controls.Add(this.btnSubmit);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FrmAddRegion";
            this.Text = "Add Region";
            this.Load += new System.EventHandler(this.FrmAddRegion_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSubmit;
        private System.Windows.Forms.TextBox tbEntry;
        private System.Windows.Forms.Label lblRegiontext;
    }
}