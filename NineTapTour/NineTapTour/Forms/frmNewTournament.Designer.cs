namespace NineTapTour.Forms
{
    partial class frmNewTournament
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
            this.lblDate = new System.Windows.Forms.Label();
            this.lblLocation = new System.Windows.Forms.Label();
            this.lblEvent = new System.Windows.Forms.Label();
            this.lblSponors = new System.Windows.Forms.Label();
            this.lblNotes = new System.Windows.Forms.Label();
            this.dtpDate = new System.Windows.Forms.DateTimePicker();
            this.txtLocation = new System.Windows.Forms.TextBox();
            this.txtEvent = new System.Windows.Forms.TextBox();
            this.txtSponsors = new System.Windows.Forms.TextBox();
            this.rtxtNotes = new System.Windows.Forms.RichTextBox();
            this.btnSubmit = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lblDoubles = new System.Windows.Forms.Label();
            this.ckbxDoubles = new System.Windows.Forms.CheckBox();
            this.btnEditTour = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.lblEdit = new System.Windows.Forms.Label();
            this.ckbx3outOf4 = new System.Windows.Forms.CheckBox();
            this.lbl3OutOf4 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblDate
            // 
            this.lblDate.AutoSize = true;
            this.lblDate.Location = new System.Drawing.Point(77, 158);
            this.lblDate.Margin = new System.Windows.Forms.Padding(7, 0, 7, 0);
            this.lblDate.Name = "lblDate";
            this.lblDate.Size = new System.Drawing.Size(205, 29);
            this.lblDate.TabIndex = 0;
            this.lblDate.Text = "Tournament Date:";
            // 
            // lblLocation
            // 
            this.lblLocation.AutoSize = true;
            this.lblLocation.Location = new System.Drawing.Point(77, 228);
            this.lblLocation.Margin = new System.Windows.Forms.Padding(7, 0, 7, 0);
            this.lblLocation.Name = "lblLocation";
            this.lblLocation.Size = new System.Drawing.Size(110, 29);
            this.lblLocation.TabIndex = 1;
            this.lblLocation.Text = "Location:";
            // 
            // lblEvent
            // 
            this.lblEvent.AutoSize = true;
            this.lblEvent.Location = new System.Drawing.Point(77, 297);
            this.lblEvent.Margin = new System.Windows.Forms.Padding(7, 0, 7, 0);
            this.lblEvent.Name = "lblEvent";
            this.lblEvent.Size = new System.Drawing.Size(79, 29);
            this.lblEvent.TabIndex = 2;
            this.lblEvent.Text = "Event:";
            // 
            // lblSponors
            // 
            this.lblSponors.AutoSize = true;
            this.lblSponors.Location = new System.Drawing.Point(77, 366);
            this.lblSponors.Margin = new System.Windows.Forms.Padding(7, 0, 7, 0);
            this.lblSponors.Name = "lblSponors";
            this.lblSponors.Size = new System.Drawing.Size(122, 29);
            this.lblSponors.TabIndex = 3;
            this.lblSponors.Text = "Sponsors:";
            // 
            // lblNotes
            // 
            this.lblNotes.AutoSize = true;
            this.lblNotes.Location = new System.Drawing.Point(44, 549);
            this.lblNotes.Margin = new System.Windows.Forms.Padding(7, 0, 7, 0);
            this.lblNotes.Name = "lblNotes";
            this.lblNotes.Size = new System.Drawing.Size(219, 29);
            this.lblNotes.TabIndex = 4;
            this.lblNotes.Text = "Tournament Notes:";
            // 
            // dtpDate
            // 
            this.dtpDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpDate.Location = new System.Drawing.Point(317, 145);
            this.dtpDate.Margin = new System.Windows.Forms.Padding(7, 7, 7, 7);
            this.dtpDate.Name = "dtpDate";
            this.dtpDate.Size = new System.Drawing.Size(282, 35);
            this.dtpDate.TabIndex = 5;
            // 
            // txtLocation
            // 
            this.txtLocation.Location = new System.Drawing.Point(317, 216);
            this.txtLocation.Margin = new System.Windows.Forms.Padding(7, 7, 7, 7);
            this.txtLocation.Name = "txtLocation";
            this.txtLocation.Size = new System.Drawing.Size(282, 35);
            this.txtLocation.TabIndex = 6;
            this.txtLocation.TextChanged += new System.EventHandler(this.txtLocation_TextChanged);
            // 
            // txtEvent
            // 
            this.txtEvent.Location = new System.Drawing.Point(317, 288);
            this.txtEvent.Margin = new System.Windows.Forms.Padding(7, 7, 7, 7);
            this.txtEvent.Name = "txtEvent";
            this.txtEvent.Size = new System.Drawing.Size(282, 35);
            this.txtEvent.TabIndex = 7;
            this.txtEvent.TextChanged += new System.EventHandler(this.txtEvent_TextChanged);
            // 
            // txtSponsors
            // 
            this.txtSponsors.Location = new System.Drawing.Point(317, 359);
            this.txtSponsors.Margin = new System.Windows.Forms.Padding(7, 7, 7, 7);
            this.txtSponsors.Name = "txtSponsors";
            this.txtSponsors.Size = new System.Drawing.Size(282, 35);
            this.txtSponsors.TabIndex = 8;
            this.txtSponsors.TextChanged += new System.EventHandler(this.txtSponsors_TextChanged);
            // 
            // rtxtNotes
            // 
            this.rtxtNotes.Location = new System.Drawing.Point(42, 607);
            this.rtxtNotes.Margin = new System.Windows.Forms.Padding(7, 7, 7, 7);
            this.rtxtNotes.Name = "rtxtNotes";
            this.rtxtNotes.Size = new System.Drawing.Size(557, 209);
            this.rtxtNotes.TabIndex = 9;
            this.rtxtNotes.Text = "";
            this.rtxtNotes.TextChanged += new System.EventHandler(this.rtxtNotes_TextChanged);
            // 
            // btnSubmit
            // 
            this.btnSubmit.Enabled = false;
            this.btnSubmit.Location = new System.Drawing.Point(35, 857);
            this.btnSubmit.Margin = new System.Windows.Forms.Padding(7, 7, 7, 7);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Size = new System.Drawing.Size(317, 51);
            this.btnSubmit.TabIndex = 10;
            this.btnSubmit.Text = "Create Tournament";
            this.btnSubmit.UseVisualStyleBackColor = true;
            this.btnSubmit.Click += new System.EventHandler(this.btnSubmit_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(429, 857);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(7, 7, 7, 7);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(175, 51);
            this.btnCancel.TabIndex = 11;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // lblDoubles
            // 
            this.lblDoubles.AutoSize = true;
            this.lblDoubles.Location = new System.Drawing.Point(77, 442);
            this.lblDoubles.Margin = new System.Windows.Forms.Padding(7, 0, 7, 0);
            this.lblDoubles.Name = "lblDoubles";
            this.lblDoubles.Size = new System.Drawing.Size(109, 29);
            this.lblDoubles.TabIndex = 12;
            this.lblDoubles.Text = "Doubles:";
            // 
            // ckbxDoubles
            // 
            this.ckbxDoubles.AutoSize = true;
            this.ckbxDoubles.Location = new System.Drawing.Point(317, 442);
            this.ckbxDoubles.Margin = new System.Windows.Forms.Padding(7, 7, 7, 7);
            this.ckbxDoubles.Name = "ckbxDoubles";
            this.ckbxDoubles.Size = new System.Drawing.Size(87, 33);
            this.ckbxDoubles.TabIndex = 13;
            this.ckbxDoubles.Text = "Yes";
            this.ckbxDoubles.UseVisualStyleBackColor = true;
            this.ckbxDoubles.CheckedChanged += new System.EventHandler(this.ckbxDoubles_CheckedChanged);
            // 
            // btnEditTour
            // 
            this.btnEditTour.Location = new System.Drawing.Point(42, 78);
            this.btnEditTour.Margin = new System.Windows.Forms.Padding(7, 7, 7, 7);
            this.btnEditTour.Name = "btnEditTour";
            this.btnEditTour.Size = new System.Drawing.Size(362, 51);
            this.btnEditTour.TabIndex = 14;
            this.btnEditTour.Text = "Edit Existing Tournament...";
            this.btnEditTour.UseVisualStyleBackColor = true;
            this.btnEditTour.Click += new System.EventHandler(this.btnEditTour_Click);
            // 
            // btnClear
            // 
            this.btnClear.Enabled = false;
            this.btnClear.Location = new System.Drawing.Point(429, 78);
            this.btnClear.Margin = new System.Windows.Forms.Padding(7, 7, 7, 7);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(175, 51);
            this.btnClear.TabIndex = 15;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // lblEdit
            // 
            this.lblEdit.AutoSize = true;
            this.lblEdit.Location = new System.Drawing.Point(44, 20);
            this.lblEdit.Margin = new System.Windows.Forms.Padding(7, 0, 7, 0);
            this.lblEdit.Name = "lblEdit";
            this.lblEdit.Size = new System.Drawing.Size(0, 29);
            this.lblEdit.TabIndex = 16;
            // 
            // ckbx3outOf4
            // 
            this.ckbx3outOf4.AutoSize = true;
            this.ckbx3outOf4.Location = new System.Drawing.Point(317, 495);
            this.ckbx3outOf4.Name = "ckbx3outOf4";
            this.ckbx3outOf4.Size = new System.Drawing.Size(87, 33);
            this.ckbx3outOf4.TabIndex = 17;
            this.ckbx3outOf4.Text = "Yes";
            this.ckbx3outOf4.UseVisualStyleBackColor = true;
            this.ckbx3outOf4.CheckedChanged += new System.EventHandler(this.ckbx3outOf4_CheckedChanged);
            // 
            // lbl3OutOf4
            // 
            this.lbl3OutOf4.AutoSize = true;
            this.lbl3OutOf4.Location = new System.Drawing.Point(77, 495);
            this.lbl3OutOf4.Name = "lbl3OutOf4";
            this.lbl3OutOf4.Size = new System.Drawing.Size(115, 29);
            this.lbl3OutOf4.TabIndex = 18;
            this.lbl3OutOf4.Text = "3 Out of 4";
            // 
            // frmNewTournament
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(14F, 29F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightBlue;
            this.ClientSize = new System.Drawing.Size(665, 970);
            this.Controls.Add(this.lbl3OutOf4);
            this.Controls.Add(this.ckbx3outOf4);
            this.Controls.Add(this.lblEdit);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.btnEditTour);
            this.Controls.Add(this.ckbxDoubles);
            this.Controls.Add(this.lblDoubles);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSubmit);
            this.Controls.Add(this.rtxtNotes);
            this.Controls.Add(this.txtSponsors);
            this.Controls.Add(this.txtEvent);
            this.Controls.Add(this.txtLocation);
            this.Controls.Add(this.dtpDate);
            this.Controls.Add(this.lblNotes);
            this.Controls.Add(this.lblSponors);
            this.Controls.Add(this.lblEvent);
            this.Controls.Add(this.lblLocation);
            this.Controls.Add(this.lblDate);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(7, 7, 7, 7);
            this.Name = "frmNewTournament";
            this.Text = "New Tournament";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblDate;
        private System.Windows.Forms.Label lblLocation;
        private System.Windows.Forms.Label lblEvent;
        private System.Windows.Forms.Label lblSponors;
        private System.Windows.Forms.Label lblNotes;
        private System.Windows.Forms.DateTimePicker dtpDate;
        private System.Windows.Forms.TextBox txtLocation;
        private System.Windows.Forms.TextBox txtEvent;
        private System.Windows.Forms.TextBox txtSponsors;
        private System.Windows.Forms.RichTextBox rtxtNotes;
        private System.Windows.Forms.Button btnSubmit;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label lblDoubles;
        private System.Windows.Forms.CheckBox ckbxDoubles;
        private System.Windows.Forms.Button btnEditTour;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Label lblEdit;
        private System.Windows.Forms.CheckBox ckbx3outOf4;
        private System.Windows.Forms.Label lbl3OutOf4;
    }
}