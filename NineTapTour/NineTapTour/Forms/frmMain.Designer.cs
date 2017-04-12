namespace NineTapTour.Forms
{
    partial class FrmMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmMain));
            this.menMain = new System.Windows.Forms.MenuStrip();
            this.mainMenuToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.memberToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tournamentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.updateInactiveMembersToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.updateInactiveMembersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // menMain
            // 
            this.menMain.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.menMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mainMenuToolStripMenuItem,
            this.memberToolStripMenuItem,
            this.tournamentToolStripMenuItem,
            this.toolsToolStripMenuItem});
            this.menMain.Location = new System.Drawing.Point(0, 0);
            this.menMain.Name = "menMain";
            this.menMain.Size = new System.Drawing.Size(347, 24);
            this.menMain.TabIndex = 1;
            this.menMain.Text = "menuStrip1";
            this.menMain.ItemAdded += new System.Windows.Forms.ToolStripItemEventHandler(this.menMain_ItemAdded);
            this.menMain.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.menMain_ItemClicked);
            // 
            // mainMenuToolStripMenuItem
            // 
            this.mainMenuToolStripMenuItem.BackColor = System.Drawing.SystemColors.Control;
            this.mainMenuToolStripMenuItem.Name = "mainMenuToolStripMenuItem";
            this.mainMenuToolStripMenuItem.Size = new System.Drawing.Size(52, 20);
            this.mainMenuToolStripMenuItem.Text = "Home";
            this.mainMenuToolStripMenuItem.Click += new System.EventHandler(this.mainMenuToolStripMenuItem_Click);
            // 
            // memberToolStripMenuItem
            // 
            this.memberToolStripMenuItem.BackColor = System.Drawing.SystemColors.Control;
            this.memberToolStripMenuItem.Name = "memberToolStripMenuItem";
            this.memberToolStripMenuItem.Size = new System.Drawing.Size(88, 20);
            this.memberToolStripMenuItem.Text = "Member Info";
            this.memberToolStripMenuItem.Click += new System.EventHandler(this.memberToolStripMenuItem_Click);
            // 
            // tournamentToolStripMenuItem
            // 
            this.tournamentToolStripMenuItem.BackColor = System.Drawing.SystemColors.Control;
            this.tournamentToolStripMenuItem.Name = "tournamentToolStripMenuItem";
            this.tournamentToolStripMenuItem.Size = new System.Drawing.Size(101, 20);
            this.tournamentToolStripMenuItem.Text = "Member Scores";
            this.tournamentToolStripMenuItem.Click += new System.EventHandler(this.tournamentToolStripMenuItem_Click);
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.updateInactiveMembersToolStripMenuItem1});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.toolsToolStripMenuItem.Text = "Tools";
            // 
            // updateInactiveMembersToolStripMenuItem1
            // 
            this.updateInactiveMembersToolStripMenuItem1.Name = "updateInactiveMembersToolStripMenuItem1";
            this.updateInactiveMembersToolStripMenuItem1.Size = new System.Drawing.Size(209, 22);
            this.updateInactiveMembersToolStripMenuItem1.Text = "Update Inactive Members";
            this.updateInactiveMembersToolStripMenuItem1.Click += new System.EventHandler(this.updateInactiveMembersToolStripMenuItem1_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.updateInactiveMembersToolStripMenuItem});
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(32, 19);
            this.toolStripMenuItem1.Text = "Tools";
            // 
            // updateInactiveMembersToolStripMenuItem
            // 
            this.updateInactiveMembersToolStripMenuItem.Name = "updateInactiveMembersToolStripMenuItem";
            this.updateInactiveMembersToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
            this.updateInactiveMembersToolStripMenuItem.Text = "Update Inactive Members";
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(347, 196);
            this.Controls.Add(this.menMain);
            this.DoubleBuffered = true;
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.IsMdiContainer = true;
            this.MainMenuStrip = this.menMain;
            this.Name = "FrmMain";
            this.Text = "9 Tap Tour";
            this.menMain.ResumeLayout(false);
            this.menMain.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menMain;
        private System.Windows.Forms.ToolStripMenuItem memberToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tournamentToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mainMenuToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem updateInactiveMembersToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem updateInactiveMembersToolStripMenuItem;
    }
}