namespace NineTapTour.Forms
{
    partial class FrmMemberScoresReports
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmMemberScoresReports));
            this.btnPrint = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtNumberOfMembers = new System.Windows.Forms.TextBox();
            this.cbPrintDues = new System.Windows.Forms.CheckBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.lblSave = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnPrint
            // 
            this.btnPrint.Location = new System.Drawing.Point(56, 102);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(75, 23);
            this.btnPrint.TabIndex = 1;
            this.btnPrint.Text = "Print";
            this.btnPrint.UseVisualStyleBackColor = true;
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(77, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(140, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Enter Amount of Top Places";
            // 
            // txtNumberOfMembers
            // 
            this.txtNumberOfMembers.BackColor = System.Drawing.SystemColors.Control;
            this.txtNumberOfMembers.Location = new System.Drawing.Point(94, 43);
            this.txtNumberOfMembers.Name = "txtNumberOfMembers";
            this.txtNumberOfMembers.Size = new System.Drawing.Size(100, 20);
            this.txtNumberOfMembers.TabIndex = 0;
            // 
            // cbPrintDues
            // 
            this.cbPrintDues.AutoSize = true;
            this.cbPrintDues.Location = new System.Drawing.Point(56, 79);
            this.cbPrintDues.Name = "cbPrintDues";
            this.cbPrintDues.Size = new System.Drawing.Size(75, 17);
            this.cbPrintDues.TabIndex = 2;
            this.cbPrintDues.Text = "Print Dues";
            this.cbPrintDues.UseVisualStyleBackColor = true;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(160, 102);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 3;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // lblSave
            // 
            this.lblSave.AutoSize = true;
            this.lblSave.Location = new System.Drawing.Point(157, 80);
            this.lblSave.Name = "lblSave";
            this.lblSave.Size = new System.Drawing.Size(91, 13);
            this.lblSave.TabIndex = 4;
            this.lblSave.Text = "Save To Desktop";
            // 
            // FrmMemberScoresReports
            // 
            this.AcceptButton = this.btnPrint;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(284, 152);
            this.Controls.Add(this.lblSave);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.cbPrintDues);
            this.Controls.Add(this.txtNumberOfMembers);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnPrint);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FrmMemberScoresReports";
            this.Text = "Member Scores Report";
            this.Load += new System.EventHandler(this.FrmMemberScoresReports_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnPrint;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtNumberOfMembers;
        private System.Windows.Forms.CheckBox cbPrintDues;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Label lblSave;
    }
}