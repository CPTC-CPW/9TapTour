namespace NineTapTour.Forms
{
    partial class FrmMainMenu
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmMainMenu));
            btnMemberData = new System.Windows.Forms.Button();
            btnMemberScores = new System.Windows.Forms.Button();
            btnAbout = new System.Windows.Forms.Button();
            Exit = new System.Windows.Forms.Button();
            btnDropDataBase1 = new System.Windows.Forms.Button();
            SuspendLayout();
            // 
            // btnMemberData
            // 
            btnMemberData.BackColor = System.Drawing.Color.Transparent;
            btnMemberData.BackgroundImage = Properties.Resources.BowlingPin2;
            btnMemberData.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            btnMemberData.FlatAppearance.BorderSize = 0;
            btnMemberData.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnMemberData.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            btnMemberData.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            btnMemberData.Location = new System.Drawing.Point(-13, 52);
            btnMemberData.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnMemberData.Name = "btnMemberData";
            btnMemberData.Padding = new System.Windows.Forms.Padding(29, 0, 0, 0);
            btnMemberData.Size = new System.Drawing.Size(282, 106);
            btnMemberData.TabIndex = 0;
            btnMemberData.Text = "Member Info";
            btnMemberData.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            btnMemberData.UseVisualStyleBackColor = false;
            btnMemberData.Click += btnMemberData_Click;
            // 
            // btnMemberScores
            // 
            btnMemberScores.BackColor = System.Drawing.Color.Transparent;
            btnMemberScores.BackgroundImage = Properties.Resources.BowlingPin2;
            btnMemberScores.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            btnMemberScores.FlatAppearance.BorderSize = 0;
            btnMemberScores.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnMemberScores.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            btnMemberScores.Location = new System.Drawing.Point(-13, 164);
            btnMemberScores.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnMemberScores.Name = "btnMemberScores";
            btnMemberScores.Padding = new System.Windows.Forms.Padding(29, 0, 0, 0);
            btnMemberScores.Size = new System.Drawing.Size(282, 112);
            btnMemberScores.TabIndex = 0;
            btnMemberScores.Text = "Member Scores";
            btnMemberScores.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            btnMemberScores.UseVisualStyleBackColor = false;
            btnMemberScores.Click += btnMemberScores_Click;
            // 
            // btnAbout
            // 
            btnAbout.BackColor = System.Drawing.Color.Transparent;
            btnAbout.BackgroundImage = Properties.Resources.BowlingPin2;
            btnAbout.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            btnAbout.FlatAppearance.BorderSize = 0;
            btnAbout.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnAbout.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F);
            btnAbout.Location = new System.Drawing.Point(-13, 282);
            btnAbout.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnAbout.Name = "btnAbout";
            btnAbout.Padding = new System.Windows.Forms.Padding(0, 0, 44, 0);
            btnAbout.Size = new System.Drawing.Size(282, 106);
            btnAbout.TabIndex = 0;
            btnAbout.Text = "About";
            btnAbout.UseVisualStyleBackColor = false;
            btnAbout.Click += btnAbout_Click;
            // 
            // Exit
            // 
            Exit.BackColor = System.Drawing.Color.Transparent;
            Exit.BackgroundImage = Properties.Resources.BowlingPin2;
            Exit.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            Exit.FlatAppearance.BorderSize = 0;
            Exit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            Exit.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F);
            Exit.Location = new System.Drawing.Point(-13, 393);
            Exit.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Exit.Name = "Exit";
            Exit.Padding = new System.Windows.Forms.Padding(0, 0, 44, 0);
            Exit.Size = new System.Drawing.Size(282, 114);
            Exit.TabIndex = 0;
            Exit.Text = "Exit";
            Exit.UseVisualStyleBackColor = false;
            Exit.Click += Exit_Click;
            // 
            // btnDropDataBase1
            // 
            btnDropDataBase1.Location = new System.Drawing.Point(58, 580);
            btnDropDataBase1.Margin = new System.Windows.Forms.Padding(2);
            btnDropDataBase1.Name = "btnDropDataBase1";
            btnDropDataBase1.Size = new System.Drawing.Size(141, 27);
            btnDropDataBase1.TabIndex = 6;
            btnDropDataBase1.Text = "Delete All Data";
            btnDropDataBase1.UseVisualStyleBackColor = true;
            btnDropDataBase1.Click += btnDropDataBase1_Click_1;
            // 
            // FrmMainMenu
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.FromArgb(0, 0, 102);
            BackgroundImage = Properties.Resources._9taplogo1;
            BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            ClientSize = new System.Drawing.Size(1148, 628);
            Controls.Add(btnDropDataBase1);
            Controls.Add(Exit);
            Controls.Add(btnAbout);
            Controls.Add(btnMemberScores);
            Controls.Add(btnMemberData);
            DoubleBuffered = true;
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Name = "FrmMainMenu";
            Text = "MainMenu";
            Paint += MainMenu_Paint;
            ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnMemberData;
        private System.Windows.Forms.Button btnMemberScores;
        private System.Windows.Forms.Button btnAbout;
        private System.Windows.Forms.Button Exit;
		private System.Windows.Forms.Button btnDropDataBase1;
	}
}