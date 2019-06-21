namespace NineTapTour.Forms
{
    partial class FrmLabelPrint
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmLabelPrint));
            this.lbxPrintList = new System.Windows.Forms.ListBox();
            this.lbxMemberList = new System.Windows.Forms.ListBox();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnRemove = new System.Windows.Forms.Button();
            this.btnClearAll = new System.Windows.Forms.Button();
            this.btnPrint = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.cbxShowInactive = new System.Windows.Forms.CheckBox();
            this.lblMemToPrint = new System.Windows.Forms.Label();
            this.lblMem = new System.Windows.Forms.Label();
            this.lblStartWhere = new System.Windows.Forms.Label();
            this.tbStartWhere = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // lbxPrintList
            // 
            this.lbxPrintList.BackColor = System.Drawing.SystemColors.ControlLight;
            this.lbxPrintList.FormattingEnabled = true;
            this.lbxPrintList.Location = new System.Drawing.Point(60, 71);
            this.lbxPrintList.Name = "lbxPrintList";
            this.lbxPrintList.Size = new System.Drawing.Size(180, 264);
            this.lbxPrintList.TabIndex = 0;
            this.lbxPrintList.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            this.lbxPrintList.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lbxPrintList_MouseDoubleClick);
            // 
            // lbxMemberList
            // 
            this.lbxMemberList.BackColor = System.Drawing.SystemColors.ControlLight;
            this.lbxMemberList.FormattingEnabled = true;
            this.lbxMemberList.Location = new System.Drawing.Point(360, 71);
            this.lbxMemberList.Name = "lbxMemberList";
            this.lbxMemberList.Size = new System.Drawing.Size(227, 264);
            this.lbxMemberList.TabIndex = 1;
            this.lbxMemberList.SelectedIndexChanged += new System.EventHandler(this.listBox2_SelectedIndexChanged);
            this.lbxMemberList.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lbxMemberList_MouseDoubleClick);
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(262, 81);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 23);
            this.btnAdd.TabIndex = 2;
            this.btnAdd.Text = "<<";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.Location = new System.Drawing.Point(262, 110);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(75, 23);
            this.btnRemove.TabIndex = 3;
            this.btnRemove.Text = ">>";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // btnClearAll
            // 
            this.btnClearAll.Location = new System.Drawing.Point(262, 238);
            this.btnClearAll.Name = "btnClearAll";
            this.btnClearAll.Size = new System.Drawing.Size(75, 23);
            this.btnClearAll.TabIndex = 4;
            this.btnClearAll.Text = "Clear All";
            this.btnClearAll.UseVisualStyleBackColor = true;
            this.btnClearAll.Click += new System.EventHandler(this.btnClearAll_Click);
            // 
            // btnPrint
            // 
            this.btnPrint.Location = new System.Drawing.Point(60, 369);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(107, 23);
            this.btnPrint.TabIndex = 5;
            this.btnPrint.Text = "Print Labels";
            this.btnPrint.UseVisualStyleBackColor = true;
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(465, 369);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 6;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // cbxShowInactive
            // 
            this.cbxShowInactive.AutoSize = true;
            this.cbxShowInactive.Location = new System.Drawing.Point(360, 341);
            this.cbxShowInactive.Name = "cbxShowInactive";
            this.cbxShowInactive.Size = new System.Drawing.Size(94, 17);
            this.cbxShowInactive.TabIndex = 7;
            this.cbxShowInactive.Text = "Show Inactive";
            this.cbxShowInactive.UseVisualStyleBackColor = true;
            this.cbxShowInactive.CheckedChanged += new System.EventHandler(this.cbxShowInactive_CheckedChanged);
            // 
            // lblMemToPrint
            // 
            this.lblMemToPrint.AutoSize = true;
            this.lblMemToPrint.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMemToPrint.Location = new System.Drawing.Point(58, 32);
            this.lblMemToPrint.Name = "lblMemToPrint";
            this.lblMemToPrint.Size = new System.Drawing.Size(182, 25);
            this.lblMemToPrint.TabIndex = 8;
            this.lblMemToPrint.Text = "Members To Print";
            // 
            // lblMem
            // 
            this.lblMem.AutoSize = true;
            this.lblMem.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMem.Location = new System.Drawing.Point(358, 32);
            this.lblMem.Name = "lblMem";
            this.lblMem.Size = new System.Drawing.Size(175, 25);
            this.lblMem.TabIndex = 9;
            this.lblMem.Text = "Region Members";
            // 
            // lblStartWhere
            // 
            this.lblStartWhere.AutoSize = true;
            this.lblStartWhere.Location = new System.Drawing.Point(60, 350);
            this.lblStartWhere.Name = "lblStartWhere";
            this.lblStartWhere.Size = new System.Drawing.Size(128, 13);
            this.lblStartWhere.TabIndex = 10;
            this.lblStartWhere.Text = "Start labels at what label?";
            // 
            // tbStartWhere
            // 
            this.tbStartWhere.Location = new System.Drawing.Point(195, 347);
            this.tbStartWhere.Name = "tbStartWhere";
            this.tbStartWhere.Size = new System.Drawing.Size(45, 20);
            this.tbStartWhere.TabIndex = 11;
            // 
            // FrmLabelPrint
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(599, 411);
            this.Controls.Add(this.tbStartWhere);
            this.Controls.Add(this.lblStartWhere);
            this.Controls.Add(this.lblMem);
            this.Controls.Add(this.lblMemToPrint);
            this.Controls.Add(this.cbxShowInactive);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnPrint);
            this.Controls.Add(this.btnClearAll);
            this.Controls.Add(this.btnRemove);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.lbxMemberList);
            this.Controls.Add(this.lbxPrintList);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FrmLabelPrint";
            this.Text = "Print Address Labels";
            this.Load += new System.EventHandler(this.FrmLabelPrint_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox lbxPrintList;
        private System.Windows.Forms.ListBox lbxMemberList;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Button btnClearAll;
        private System.Windows.Forms.Button btnPrint;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.CheckBox cbxShowInactive;
        private System.Windows.Forms.Label lblMemToPrint;
        private System.Windows.Forms.Label lblMem;
        private System.Windows.Forms.Label lblStartWhere;
        private System.Windows.Forms.TextBox tbStartWhere;
    }
}