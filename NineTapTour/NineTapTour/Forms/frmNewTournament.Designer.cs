namespace NineTapTour.Forms
{
    partial class FrmNewTournament
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmNewTournament));
            lblDate = new System.Windows.Forms.Label();
            lblLocation = new System.Windows.Forms.Label();
            lblEvent = new System.Windows.Forms.Label();
            lblSponors = new System.Windows.Forms.Label();
            lblNotes = new System.Windows.Forms.Label();
            dtpDate = new System.Windows.Forms.DateTimePicker();
            txtLocation = new System.Windows.Forms.TextBox();
            txtEvent = new System.Windows.Forms.TextBox();
            txtSponsors = new System.Windows.Forms.TextBox();
            rtxtNotes = new System.Windows.Forms.RichTextBox();
            btnSubmit = new System.Windows.Forms.Button();
            btnCancel = new System.Windows.Forms.Button();
            btnEditTour = new System.Windows.Forms.Button();
            btnClear = new System.Windows.Forms.Button();
            lblEdit = new System.Windows.Forms.Label();
            lblsquads = new System.Windows.Forms.Label();
            txtSquads = new System.Windows.Forms.TextBox();
            rdo3OutOf4 = new System.Windows.Forms.RadioButton();
            rdoDoubles = new System.Windows.Forms.RadioButton();
            rdoSingles = new System.Windows.Forms.RadioButton();
            rdoThreeGame = new System.Windows.Forms.RadioButton();
            chkTwoDay = new System.Windows.Forms.CheckBox();
            SuspendLayout();
            // 
            // lblDate
            // 
            lblDate.AutoSize = true;
            lblDate.Location = new System.Drawing.Point(22, 82);
            lblDate.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblDate.Name = "lblDate";
            lblDate.Size = new System.Drawing.Size(101, 15);
            lblDate.TabIndex = 0;
            lblDate.Text = "Tournament Date:";
            // 
            // lblLocation
            // 
            lblLocation.AutoSize = true;
            lblLocation.Location = new System.Drawing.Point(22, 115);
            lblLocation.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblLocation.Name = "lblLocation";
            lblLocation.Size = new System.Drawing.Size(56, 15);
            lblLocation.TabIndex = 1;
            lblLocation.Text = "Location:";
            // 
            // lblEvent
            // 
            lblEvent.AutoSize = true;
            lblEvent.Location = new System.Drawing.Point(22, 152);
            lblEvent.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblEvent.Name = "lblEvent";
            lblEvent.Size = new System.Drawing.Size(39, 15);
            lblEvent.TabIndex = 2;
            lblEvent.Text = "Event:";
            // 
            // lblSponors
            // 
            lblSponors.AutoSize = true;
            lblSponors.Location = new System.Drawing.Point(22, 189);
            lblSponors.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblSponors.Name = "lblSponors";
            lblSponors.Size = new System.Drawing.Size(58, 15);
            lblSponors.TabIndex = 3;
            lblSponors.Text = "Sponsors:";
            // 
            // lblNotes
            // 
            lblNotes.AutoSize = true;
            lblNotes.Location = new System.Drawing.Point(22, 325);
            lblNotes.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblNotes.Name = "lblNotes";
            lblNotes.Size = new System.Drawing.Size(108, 15);
            lblNotes.TabIndex = 4;
            lblNotes.Text = "Tournament Notes:";
            // 
            // dtpDate
            // 
            dtpDate.Checked = false;
            dtpDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            dtpDate.Location = new System.Drawing.Point(172, 75);
            dtpDate.Margin = new System.Windows.Forms.Padding(4);
            dtpDate.Name = "dtpDate";
            dtpDate.Size = new System.Drawing.Size(143, 23);
            dtpDate.TabIndex = 5;
            // 
            // txtLocation
            // 
            txtLocation.BackColor = System.Drawing.SystemColors.Control;
            txtLocation.Location = new System.Drawing.Point(172, 112);
            txtLocation.Margin = new System.Windows.Forms.Padding(4);
            txtLocation.Name = "txtLocation";
            txtLocation.Size = new System.Drawing.Size(143, 23);
            txtLocation.TabIndex = 6;
            txtLocation.TextChanged += txtLocation_TextChanged;
            // 
            // txtEvent
            // 
            txtEvent.BackColor = System.Drawing.SystemColors.Control;
            txtEvent.Location = new System.Drawing.Point(172, 152);
            txtEvent.Margin = new System.Windows.Forms.Padding(4);
            txtEvent.Name = "txtEvent";
            txtEvent.Size = new System.Drawing.Size(143, 23);
            txtEvent.TabIndex = 7;
            txtEvent.TextChanged += txtEvent_TextChanged;
            // 
            // txtSponsors
            // 
            txtSponsors.BackColor = System.Drawing.SystemColors.Control;
            txtSponsors.Location = new System.Drawing.Point(172, 186);
            txtSponsors.Margin = new System.Windows.Forms.Padding(4);
            txtSponsors.Name = "txtSponsors";
            txtSponsors.Size = new System.Drawing.Size(143, 23);
            txtSponsors.TabIndex = 8;
            txtSponsors.TextChanged += txtSponsors_TextChanged;
            // 
            // rtxtNotes
            // 
            rtxtNotes.BackColor = System.Drawing.SystemColors.Control;
            rtxtNotes.Location = new System.Drawing.Point(21, 344);
            rtxtNotes.Margin = new System.Windows.Forms.Padding(4);
            rtxtNotes.Name = "rtxtNotes";
            rtxtNotes.Size = new System.Drawing.Size(280, 110);
            rtxtNotes.TabIndex = 9;
            rtxtNotes.Text = "";
            rtxtNotes.TextChanged += rtxtNotes_TextChanged;
            // 
            // btnSubmit
            // 
            btnSubmit.Enabled = false;
            btnSubmit.Location = new System.Drawing.Point(21, 461);
            btnSubmit.Margin = new System.Windows.Forms.Padding(4);
            btnSubmit.Name = "btnSubmit";
            btnSubmit.Size = new System.Drawing.Size(158, 26);
            btnSubmit.TabIndex = 10;
            btnSubmit.Text = "Create Tournament";
            btnSubmit.UseVisualStyleBackColor = true;
            btnSubmit.Click += btnSubmit_Click;
            // 
            // btnCancel
            // 
            btnCancel.Location = new System.Drawing.Point(214, 461);
            btnCancel.Margin = new System.Windows.Forms.Padding(4);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new System.Drawing.Size(88, 26);
            btnCancel.TabIndex = 11;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += btnCancel_Click;
            // 
            // btnEditTour
            // 
            btnEditTour.Location = new System.Drawing.Point(25, 25);
            btnEditTour.Margin = new System.Windows.Forms.Padding(4);
            btnEditTour.Name = "btnEditTour";
            btnEditTour.Size = new System.Drawing.Size(181, 26);
            btnEditTour.TabIndex = 14;
            btnEditTour.Text = "Edit Existing Tournament...";
            btnEditTour.UseVisualStyleBackColor = true;
            btnEditTour.Click += btnEditTour_Click;
            // 
            // btnClear
            // 
            btnClear.Enabled = false;
            btnClear.Location = new System.Drawing.Point(228, 25);
            btnClear.Margin = new System.Windows.Forms.Padding(4);
            btnClear.Name = "btnClear";
            btnClear.Size = new System.Drawing.Size(88, 26);
            btnClear.TabIndex = 15;
            btnClear.Text = "Clear";
            btnClear.UseVisualStyleBackColor = true;
            btnClear.Click += btnClear_Click;
            // 
            // lblEdit
            // 
            lblEdit.AutoSize = true;
            lblEdit.Location = new System.Drawing.Point(22, 10);
            lblEdit.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblEdit.Name = "lblEdit";
            lblEdit.Size = new System.Drawing.Size(0, 15);
            lblEdit.TabIndex = 16;
            // 
            // lblsquads
            // 
            lblsquads.AutoSize = true;
            lblsquads.Location = new System.Drawing.Point(22, 292);
            lblsquads.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblsquads.Name = "lblsquads";
            lblsquads.Size = new System.Drawing.Size(106, 15);
            lblsquads.TabIndex = 19;
            lblsquads.Text = "Number of Squads";
            // 
            // txtSquads
            // 
            txtSquads.BackColor = System.Drawing.SystemColors.Control;
            txtSquads.Location = new System.Drawing.Point(186, 289);
            txtSquads.Margin = new System.Windows.Forms.Padding(4);
            txtSquads.Name = "txtSquads";
            txtSquads.Size = new System.Drawing.Size(116, 23);
            txtSquads.TabIndex = 20;
            // 
            // rdo3OutOf4
            // 
            rdo3OutOf4.AutoSize = true;
            rdo3OutOf4.Location = new System.Drawing.Point(25, 248);
            rdo3OutOf4.Margin = new System.Windows.Forms.Padding(4);
            rdo3OutOf4.Name = "rdo3OutOf4";
            rdo3OutOf4.Size = new System.Drawing.Size(142, 19);
            rdo3OutOf4.TabIndex = 21;
            rdo3OutOf4.TabStop = true;
            rdo3OutOf4.Text = "3 out of 4 Tournament";
            rdo3OutOf4.UseVisualStyleBackColor = true;
            rdo3OutOf4.CheckedChanged += rdo3OutOf4_CheckedChanged;
            // 
            // rdoDoubles
            // 
            rdoDoubles.AutoSize = true;
            rdoDoubles.Location = new System.Drawing.Point(172, 221);
            rdoDoubles.Margin = new System.Windows.Forms.Padding(4);
            rdoDoubles.Name = "rdoDoubles";
            rdoDoubles.Size = new System.Drawing.Size(135, 19);
            rdoDoubles.TabIndex = 22;
            rdoDoubles.TabStop = true;
            rdoDoubles.Text = "Doubles Tournament";
            rdoDoubles.UseVisualStyleBackColor = true;
            rdoDoubles.CheckedChanged += rdoDoubles_CheckedChanged;
            // 
            // rdoSingles
            // 
            rdoSingles.AutoSize = true;
            rdoSingles.Location = new System.Drawing.Point(25, 221);
            rdoSingles.Margin = new System.Windows.Forms.Padding(4);
            rdoSingles.Name = "rdoSingles";
            rdoSingles.Size = new System.Drawing.Size(129, 19);
            rdoSingles.TabIndex = 23;
            rdoSingles.TabStop = true;
            rdoSingles.Text = "Singles Tournament";
            rdoSingles.UseVisualStyleBackColor = true;
            // 
            // rdoThreeGame
            // 
            rdoThreeGame.AutoSize = true;
            rdoThreeGame.Location = new System.Drawing.Point(172, 248);
            rdoThreeGame.Name = "rdoThreeGame";
            rdoThreeGame.Size = new System.Drawing.Size(132, 19);
            rdoThreeGame.TabIndex = 24;
            rdoThreeGame.TabStop = true;
            rdoThreeGame.Text = "3 Game Tournament";
            rdoThreeGame.UseVisualStyleBackColor = true;
            // 
            // chkTwoDay
            // 
            chkTwoDay.AutoSize = true;
            chkTwoDay.Location = new System.Drawing.Point(25, 272);
            chkTwoDay.Margin = new System.Windows.Forms.Padding(4);
            chkTwoDay.Name = "chkTwoDay";
            chkTwoDay.Size = new System.Drawing.Size(130, 19);
            chkTwoDay.TabIndex = 25;
            chkTwoDay.Text = "2-Day Tournament";
            chkTwoDay.UseVisualStyleBackColor = true;
            chkTwoDay.CheckedChanged += chkTwoDay_CheckedChanged;
            // 
            // FrmNewTournament
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.White;
            ClientSize = new System.Drawing.Size(368, 502);
            Controls.Add(rdoThreeGame);
            Controls.Add(chkTwoDay);
            Controls.Add(rdoSingles);
            Controls.Add(rdoDoubles);
            Controls.Add(rdo3OutOf4);
            Controls.Add(txtSquads);
            Controls.Add(lblsquads);
            Controls.Add(lblEdit);
            Controls.Add(btnClear);
            Controls.Add(btnEditTour);
            Controls.Add(btnCancel);
            Controls.Add(btnSubmit);
            Controls.Add(rtxtNotes);
            Controls.Add(txtSponsors);
            Controls.Add(txtEvent);
            Controls.Add(txtLocation);
            Controls.Add(dtpDate);
            Controls.Add(lblNotes);
            Controls.Add(lblSponors);
            Controls.Add(lblEvent);
            Controls.Add(lblLocation);
            Controls.Add(lblDate);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            Margin = new System.Windows.Forms.Padding(4);
            Name = "FrmNewTournament";
            Text = "New Tournament";
            TopMost = true;
            Load += FrmNewTournament_Load;
            ResumeLayout(false);
            PerformLayout();
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
        private System.Windows.Forms.Button btnEditTour;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Label lblEdit;
        private System.Windows.Forms.Label lblsquads;
        private System.Windows.Forms.TextBox txtSquads;
        private System.Windows.Forms.RadioButton rdo3OutOf4;
        private System.Windows.Forms.RadioButton rdoDoubles;
        private System.Windows.Forms.RadioButton rdoSingles;
        private System.Windows.Forms.RadioButton rdoThreeGame;
        private System.Windows.Forms.CheckBox chkTwoDay;
    }
}