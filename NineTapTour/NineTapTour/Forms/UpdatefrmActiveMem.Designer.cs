namespace NineTapTour.Forms
{
    partial class UpdatefrmActiveMem
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
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.btnUpdateActive = new System.Windows.Forms.Button();
            this.checkedListBox1 = new System.Windows.Forms.CheckedListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnCheckInactive = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.Location = new System.Drawing.Point(36, 39);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(200, 20);
            this.dateTimePicker1.TabIndex = 0;
            this.dateTimePicker1.ValueChanged += new System.EventHandler(this.dateTimePicker1_ValueChanged);
            // 
            // btnUpdateActive
            // 
            this.btnUpdateActive.Location = new System.Drawing.Point(49, 423);
            this.btnUpdateActive.Name = "btnUpdateActive";
            this.btnUpdateActive.Size = new System.Drawing.Size(75, 23);
            this.btnUpdateActive.TabIndex = 1;
            this.btnUpdateActive.Text = "Update";
            this.btnUpdateActive.UseVisualStyleBackColor = true;
            this.btnUpdateActive.Click += new System.EventHandler(this.btnUpdateActive_Click);
            // 
            // checkedListBox1
            // 
            this.checkedListBox1.FormattingEnabled = true;
            this.checkedListBox1.Location = new System.Drawing.Point(36, 77);
            this.checkedListBox1.Name = "checkedListBox1";
            this.checkedListBox1.Size = new System.Drawing.Size(205, 334);
            this.checkedListBox1.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(33, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "label1";
            // 
            // btnCheckInactive
            // 
            this.btnCheckInactive.Location = new System.Drawing.Point(143, 423);
            this.btnCheckInactive.Name = "btnCheckInactive";
            this.btnCheckInactive.Size = new System.Drawing.Size(75, 23);
            this.btnCheckInactive.TabIndex = 4;
            this.btnCheckInactive.Text = "Check All";
            this.btnCheckInactive.UseVisualStyleBackColor = true;
            this.btnCheckInactive.Click += new System.EventHandler(this.btnCheckInactive_Click);
            // 
            // UpdatefrmActiveMem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 458);
            this.Controls.Add(this.btnCheckInactive);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.checkedListBox1);
            this.Controls.Add(this.btnUpdateActive);
            this.Controls.Add(this.dateTimePicker1);
            this.Name = "UpdatefrmActiveMem";
            this.Text = "Update Inactive Members";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DateTimePicker dateTimePicker1;
        private System.Windows.Forms.Button btnUpdateActive;
        private System.Windows.Forms.CheckedListBox checkedListBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnCheckInactive;
    }
}