namespace NineTapTour.Forms
{
    partial class FrmSearch
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
            this.lblSearchMember = new System.Windows.Forms.Label();
            this.txtbxSearchMemberNumber = new System.Windows.Forms.TextBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.lbxMembersSearched = new System.Windows.Forms.ListBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblSearchMember
            // 
            this.lblSearchMember.AutoSize = true;
            this.lblSearchMember.Location = new System.Drawing.Point(12, 53);
            this.lblSearchMember.Name = "lblSearchMember";
            this.lblSearchMember.Size = new System.Drawing.Size(91, 13);
            this.lblSearchMember.TabIndex = 0;
            this.lblSearchMember.Text = "Member Number: ";
            // 
            // txtbxSearchMemberNumber
            // 
            this.txtbxSearchMemberNumber.Location = new System.Drawing.Point(109, 50);
            this.txtbxSearchMemberNumber.Name = "txtbxSearchMemberNumber";
            this.txtbxSearchMemberNumber.Size = new System.Drawing.Size(30, 20);
            this.txtbxSearchMemberNumber.TabIndex = 1;
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(15, 227);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 23);
            this.btnSearch.TabIndex = 2;
            this.btnSearch.Text = "Search";
            this.btnSearch.UseVisualStyleBackColor = true;
            // 
            // lbxMembersSearched
            // 
            this.lbxMembersSearched.FormattingEnabled = true;
            this.lbxMembersSearched.Items.AddRange(new object[] {
            "test"});
            this.lbxMembersSearched.Location = new System.Drawing.Point(15, 97);
            this.lbxMembersSearched.Name = "lbxMembersSearched";
            this.lbxMembersSearched.Size = new System.Drawing.Size(257, 95);
            this.lbxMembersSearched.TabIndex = 3;
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(197, 227);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // FrmSearch
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.lbxMembersSearched);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.txtbxSearchMemberNumber);
            this.Controls.Add(this.lblSearchMember);
            this.Name = "FrmSearch";
            this.Text = "FrmSearch";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblSearchMember;
        private System.Windows.Forms.TextBox txtbxSearchMemberNumber;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.ListBox lbxMembersSearched;
        private System.Windows.Forms.Button btnCancel;
    }
}