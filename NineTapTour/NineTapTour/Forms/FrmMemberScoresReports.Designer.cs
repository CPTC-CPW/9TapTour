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
            btnPrint = new System.Windows.Forms.Button();
            label1 = new System.Windows.Forms.Label();
            txtNumberOfMembers = new System.Windows.Forms.TextBox();
            cbPrintDues = new System.Windows.Forms.CheckBox();
            btnSave = new System.Windows.Forms.Button();
            lblSave = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            txtCutoffLine = new System.Windows.Forms.MaskedTextBox();
            SuspendLayout();
            // 
            // btnPrint
            // 
            btnPrint.Location = new System.Drawing.Point(65, 118);
            btnPrint.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnPrint.Name = "btnPrint";
            btnPrint.Size = new System.Drawing.Size(88, 27);
            btnPrint.TabIndex = 5;
            btnPrint.Text = "Print";
            btnPrint.UseVisualStyleBackColor = true;
            btnPrint.Click += BtnPrint_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(90, 21);
            label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(154, 15);
            label1.TabIndex = 0;
            label1.Text = "Enter Amount of Top Places";
            // 
            // txtNumberOfMembers
            // 
            txtNumberOfMembers.BackColor = System.Drawing.SystemColors.Control;
            txtNumberOfMembers.Location = new System.Drawing.Point(110, 50);
            txtNumberOfMembers.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            txtNumberOfMembers.Name = "txtNumberOfMembers";
            txtNumberOfMembers.Size = new System.Drawing.Size(116, 23);
            txtNumberOfMembers.TabIndex = 1;
            // 
            // cbPrintDues
            // 
            cbPrintDues.AutoSize = true;
            cbPrintDues.Location = new System.Drawing.Point(65, 91);
            cbPrintDues.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            cbPrintDues.Name = "cbPrintDues";
            cbPrintDues.Size = new System.Drawing.Size(80, 19);
            cbPrintDues.TabIndex = 4;
            cbPrintDues.Text = "Print Dues";
            cbPrintDues.UseVisualStyleBackColor = true;
            // 
            // btnSave
            // 
            btnSave.Location = new System.Drawing.Point(187, 118);
            btnSave.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnSave.Name = "btnSave";
            btnSave.Size = new System.Drawing.Size(88, 27);
            btnSave.TabIndex = 7;
            btnSave.Text = "Save";
            btnSave.UseVisualStyleBackColor = true;
            btnSave.Click += BtnSave_Click;
            // 
            // lblSave
            // 
            lblSave.AutoSize = true;
            lblSave.Location = new System.Drawing.Point(183, 92);
            lblSave.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblSave.Name = "lblSave";
            lblSave.Size = new System.Drawing.Size(93, 15);
            lblSave.TabIndex = 6;
            lblSave.Text = "Save To Desktop";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(299, 10);
            label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(263, 30);
            label2.TabIndex = 2;
            label2.Text = "Manual Cutoff Line\r\n(Overrides calculated number of winning places)";
            // 
            // txtCutoffLine
            // 
            txtCutoffLine.BackColor = System.Drawing.SystemColors.Control;
            txtCutoffLine.Location = new System.Drawing.Point(302, 50);
            txtCutoffLine.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            txtCutoffLine.Mask = "00";
            txtCutoffLine.Name = "txtCutoffLine";
            txtCutoffLine.Size = new System.Drawing.Size(116, 23);
            txtCutoffLine.TabIndex = 3;
            txtCutoffLine.ValidatingType = typeof(int);
            txtCutoffLine.Click += TxtCutoffLine_Click;
            // 
            // FrmMemberScoresReports
            // 
            AcceptButton = btnPrint;
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.White;
            ClientSize = new System.Drawing.Size(581, 250);
            Controls.Add(txtCutoffLine);
            Controls.Add(label2);
            Controls.Add(lblSave);
            Controls.Add(btnSave);
            Controls.Add(cbPrintDues);
            Controls.Add(txtNumberOfMembers);
            Controls.Add(label1);
            Controls.Add(btnPrint);
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Name = "FrmMemberScoresReports";
            Text = "Member Scores Report";
            Load += FrmMemberScoresReports_Load;
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnPrint;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtNumberOfMembers;
        private System.Windows.Forms.CheckBox cbPrintDues;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Label lblSave;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.MaskedTextBox txtCutoffLine;
    }
}