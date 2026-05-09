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
            menMain.BackColor = System.Drawing.Color.White;
            menMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { mainMenuToolStripMenuItem, memberToolStripMenuItem, tournamentToolStripMenuItem, AboutToolStripMenuItem, toolsToolStripMenuItem });
            menMain.Location = new System.Drawing.Point(0, 0);
            menMain.Name = "menMain";
            menMain.Padding = new System.Windows.Forms.Padding(13, 5, 0, 5);
            menMain.Size = new System.Drawing.Size(780, 48);
            menMain.TabIndex = 1;
            menMain.Text = "menuStrip1";
            menMain.ItemAdded += menMain_ItemAdded;
            menMain.ItemClicked += menMain_ItemClicked;
            // 
            // mainMenuToolStripMenuItem
            // 
            mainMenuToolStripMenuItem.BackColor = System.Drawing.SystemColors.Control;
            mainMenuToolStripMenuItem.Name = "mainMenuToolStripMenuItem";
            mainMenuToolStripMenuItem.Size = new System.Drawing.Size(99, 38);
            mainMenuToolStripMenuItem.Text = "Home";
            mainMenuToolStripMenuItem.Click += mainMenuToolStripMenuItem_Click;
            // 
            // memberToolStripMenuItem
            // 
            memberToolStripMenuItem.BackColor = System.Drawing.SystemColors.Control;
            memberToolStripMenuItem.Name = "memberToolStripMenuItem";
            memberToolStripMenuItem.Size = new System.Drawing.Size(174, 38);
            memberToolStripMenuItem.Text = "Member Info";
            memberToolStripMenuItem.Click += memberToolStripMenuItem_Click;
            // 
            // tournamentToolStripMenuItem
            // 
            tournamentToolStripMenuItem.BackColor = System.Drawing.SystemColors.Control;
            tournamentToolStripMenuItem.Name = "tournamentToolStripMenuItem";
            tournamentToolStripMenuItem.Size = new System.Drawing.Size(201, 38);
            tournamentToolStripMenuItem.Text = "Member Scores";
            tournamentToolStripMenuItem.Click += tournamentToolStripMenuItem_Click;
            // 
            // AboutToolStripMenuItem
            // 
            AboutToolStripMenuItem.BackColor = System.Drawing.SystemColors.Control;
            AboutToolStripMenuItem.Name = "AboutToolStripMenuItem";
            AboutToolStripMenuItem.Size = new System.Drawing.Size(99, 38);
            AboutToolStripMenuItem.Text = "About";
            AboutToolStripMenuItem.Click += AboutToolStripMenuItem_Click;
            // 
            // toolsToolStripMenuItem
            // 
            toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { BackupDatabaseToolStripMenuItem, RestoreDatabaseToolStripMenuItem, updateInactiveMembersToolStripMenuItem1, labelPrintToolStripMenuItem });
            toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            toolsToolStripMenuItem.Size = new System.Drawing.Size(89, 38);
            toolsToolStripMenuItem.Text = "Tools";
            // 
            // BackupDatabaseToolStripMenuItem
            // 
            BackupDatabaseToolStripMenuItem.Name = "BackupDatabaseToolStripMenuItem";
            BackupDatabaseToolStripMenuItem.Size = new System.Drawing.Size(421, 44);
            BackupDatabaseToolStripMenuItem.Text = "Backup Database";
            BackupDatabaseToolStripMenuItem.Click += BackupDatabaseToolStripMenuItem_Click;
            // 
            // RestoreDatabaseToolStripMenuItem
            // 
            RestoreDatabaseToolStripMenuItem.Name = "RestoreDatabaseToolStripMenuItem";
            RestoreDatabaseToolStripMenuItem.Size = new System.Drawing.Size(421, 44);
            RestoreDatabaseToolStripMenuItem.Text = "Restore Database";
            RestoreDatabaseToolStripMenuItem.Click += RestoreDatabaseToolStripMenuItem_Click;
            // 
            // updateInactiveMembersToolStripMenuItem1
            // 
            updateInactiveMembersToolStripMenuItem1.Name = "updateInactiveMembersToolStripMenuItem1";
            updateInactiveMembersToolStripMenuItem1.Size = new System.Drawing.Size(421, 44);
            updateInactiveMembersToolStripMenuItem1.Text = "Update Inactive Members";
            updateInactiveMembersToolStripMenuItem1.Click += updateInactiveMembersToolStripMenuItem1_Click;
            // 
            // labelPrintToolStripMenuItem
            // 
            labelPrintToolStripMenuItem.Name = "labelPrintToolStripMenuItem";
            labelPrintToolStripMenuItem.Size = new System.Drawing.Size(421, 44);
            labelPrintToolStripMenuItem.Text = "Print Member Labels";
            labelPrintToolStripMenuItem.Click += labelPrintToolStripMenuItem_Click;
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
            updateInactiveMembersToolStripMenuItem.Size = new System.Drawing.Size(421, 44);
            updateInactiveMembersToolStripMenuItem.Text = "Update Inactive Members";
            // 
            // FrmMain
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(13F, 32F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            AutoSize = true;
            BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            ClientSize = new System.Drawing.Size(780, 482);
            Controls.Add(menMain);
            DoubleBuffered = true;
            ForeColor = System.Drawing.SystemColors.ControlText;
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            IsMdiContainer = true;
            MainMenuStrip = menMain;
            Margin = new System.Windows.Forms.Padding(6, 7, 6, 7);
            Name = "FrmMain";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "9 Tap Tour";
            Load += FrmMain_Load;
            menMain.ResumeLayout(false);
            menMain.PerformLayout();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menMain;
        private System.Windows.Forms.ToolStripMenuItem memberToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tournamentToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem AboutToolStripMenuItem;
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