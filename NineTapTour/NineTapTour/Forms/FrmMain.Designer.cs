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
            menMain = new System.Windows.Forms.MenuStrip();
            mainMenuToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            memberToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            tournamentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            AboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            BackupDatabaseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            RestoreDatabaseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            updateInactiveMembersToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            labelPrintToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            updateInactiveMembersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            menMain.SuspendLayout();
            SuspendLayout();
            // 
            // menMain
            // 
            menMain.AllowMerge = false;
            menMain.BackColor = System.Drawing.Color.White;
            menMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { mainMenuToolStripMenuItem, memberToolStripMenuItem, tournamentToolStripMenuItem, AboutToolStripMenuItem, toolsToolStripMenuItem });
            menMain.Location = new System.Drawing.Point(0, 0);
            menMain.Name = "menMain";
            menMain.Padding = new System.Windows.Forms.Padding(7, 2, 0, 2);
            menMain.Size = new System.Drawing.Size(420, 24);
            menMain.TabIndex = 1;
            menMain.Text = "menuStrip1";
            menMain.ItemClicked += MainMenuToolStrip_ItemClicked;
            // 
            // mainMenuToolStripMenuItem
            // 
            mainMenuToolStripMenuItem.BackColor = System.Drawing.SystemColors.Control;
            mainMenuToolStripMenuItem.Name = "mainMenuToolStripMenuItem";
            mainMenuToolStripMenuItem.Size = new System.Drawing.Size(52, 20);
            mainMenuToolStripMenuItem.Text = "Home";
            mainMenuToolStripMenuItem.Click += MainMenuToolStripMenuItem_Click;
            // 
            // memberToolStripMenuItem
            // 
            memberToolStripMenuItem.BackColor = System.Drawing.SystemColors.Control;
            memberToolStripMenuItem.Name = "memberToolStripMenuItem";
            memberToolStripMenuItem.Size = new System.Drawing.Size(88, 20);
            memberToolStripMenuItem.Text = "Member Info";
            memberToolStripMenuItem.Click += MemberDataToolStripMenuItem_Click;
            // 
            // tournamentToolStripMenuItem
            // 
            tournamentToolStripMenuItem.BackColor = System.Drawing.SystemColors.Control;
            tournamentToolStripMenuItem.Name = "tournamentToolStripMenuItem";
            tournamentToolStripMenuItem.Size = new System.Drawing.Size(101, 20);
            tournamentToolStripMenuItem.Text = "Member Scores";
            tournamentToolStripMenuItem.Click += TournamentToolStripMenuItem_Click;
            // 
            // AboutToolStripMenuItem
            // 
            AboutToolStripMenuItem.BackColor = System.Drawing.SystemColors.Control;
            AboutToolStripMenuItem.Name = "AboutToolStripMenuItem";
            AboutToolStripMenuItem.Size = new System.Drawing.Size(52, 20);
            AboutToolStripMenuItem.Text = "About";
            AboutToolStripMenuItem.Click += AboutToolStripMenuItem_Click;
            // 
            // toolsToolStripMenuItem
            // 
            toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { BackupDatabaseToolStripMenuItem, RestoreDatabaseToolStripMenuItem, updateInactiveMembersToolStripMenuItem1, labelPrintToolStripMenuItem });
            toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            toolsToolStripMenuItem.Size = new System.Drawing.Size(47, 20);
            toolsToolStripMenuItem.Text = "Tools";
            // 
            // BackupDatabaseToolStripMenuItem
            // 
            BackupDatabaseToolStripMenuItem.Name = "BackupDatabaseToolStripMenuItem";
            BackupDatabaseToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
            BackupDatabaseToolStripMenuItem.Text = "Backup Database";
            BackupDatabaseToolStripMenuItem.Click += BackupDatabaseToolStripMenuItem_Click;
            // 
            // RestoreDatabaseToolStripMenuItem
            // 
            RestoreDatabaseToolStripMenuItem.Name = "RestoreDatabaseToolStripMenuItem";
            RestoreDatabaseToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
            RestoreDatabaseToolStripMenuItem.Text = "Restore Database";
            RestoreDatabaseToolStripMenuItem.Click += RestoreDatabaseToolStripMenuItem_Click;
            // 
            // updateInactiveMembersToolStripMenuItem1
            // 
            updateInactiveMembersToolStripMenuItem1.Name = "updateInactiveMembersToolStripMenuItem1";
            updateInactiveMembersToolStripMenuItem1.Size = new System.Drawing.Size(209, 22);
            updateInactiveMembersToolStripMenuItem1.Text = "Update Inactive Members";
            updateInactiveMembersToolStripMenuItem1.Click += UpdateInactiveMembersToolStripMenuItem1_Click;
            // 
            // labelPrintToolStripMenuItem
            // 
            labelPrintToolStripMenuItem.Name = "labelPrintToolStripMenuItem";
            labelPrintToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
            labelPrintToolStripMenuItem.Text = "Print Member Labels";
            labelPrintToolStripMenuItem.Click += LabelPrintToolStripMenuItem_Click;
            // 
            // toolStripMenuItem1
            // 
            toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { updateInactiveMembersToolStripMenuItem });
            toolStripMenuItem1.Name = "toolStripMenuItem1";
            toolStripMenuItem1.Size = new System.Drawing.Size(32, 19);
            toolStripMenuItem1.Text = "Tools";
            // 
            // updateInactiveMembersToolStripMenuItem
            // 
            updateInactiveMembersToolStripMenuItem.Name = "updateInactiveMembersToolStripMenuItem";
            updateInactiveMembersToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
            updateInactiveMembersToolStripMenuItem.Text = "Update Inactive Members";
            // 
            // FrmMain
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            AutoSize = true;
            BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            ClientSize = new System.Drawing.Size(420, 226);
            Controls.Add(menMain);
            DoubleBuffered = true;
            ForeColor = System.Drawing.SystemColors.ControlText;
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            IsMdiContainer = true;
            MainMenuStrip = menMain;
            Name = "FrmMain";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "9 Tap Tour";
            menMain.ResumeLayout(false);
            menMain.PerformLayout();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menMain;
        public System.Windows.Forms.ToolStripMenuItem memberToolStripMenuItem;
        public System.Windows.Forms.ToolStripMenuItem tournamentToolStripMenuItem;
        public System.Windows.Forms.ToolStripMenuItem AboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem updateInactiveMembersToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem updateInactiveMembersToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem BackupDatabaseToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem RestoreDatabaseToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mainMenuToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem labelPrintToolStripMenuItem;
    }
}