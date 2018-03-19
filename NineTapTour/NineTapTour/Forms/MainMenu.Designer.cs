namespace NineTapTour.Forms
{
    partial class MainMenu
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainMenu));
			this.btnMemberData = new System.Windows.Forms.Button();
			this.btnMemberScores = new System.Windows.Forms.Button();
			this.btnAbout = new System.Windows.Forms.Button();
			this.Exit = new System.Windows.Forms.Button();
			this.cbxRegionSelect = new System.Windows.Forms.ComboBox();
			this.btnAdd = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.btnDropDataBase1 = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// btnMemberData
			// 
			this.btnMemberData.BackgroundImage = global::NineTapTour.Properties.Resources.BowlingPin1;
			this.btnMemberData.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.btnMemberData.FlatAppearance.BorderSize = 0;
			this.btnMemberData.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnMemberData.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnMemberData.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
			this.btnMemberData.Location = new System.Drawing.Point(24, 98);
			this.btnMemberData.Margin = new System.Windows.Forms.Padding(6);
			this.btnMemberData.Name = "btnMemberData";
			this.btnMemberData.Padding = new System.Windows.Forms.Padding(50, 0, 0, 0);
			this.btnMemberData.Size = new System.Drawing.Size(484, 177);
			this.btnMemberData.TabIndex = 0;
			this.btnMemberData.Text = "Member Info";
			this.btnMemberData.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.btnMemberData.UseVisualStyleBackColor = true;
			this.btnMemberData.Click += new System.EventHandler(this.btnMemberData_Click);
			// 
			// btnMemberScores
			// 
			this.btnMemberScores.BackgroundImage = global::NineTapTour.Properties.Resources.BowlingPin1;
			this.btnMemberScores.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.btnMemberScores.FlatAppearance.BorderSize = 0;
			this.btnMemberScores.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnMemberScores.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnMemberScores.Location = new System.Drawing.Point(24, 275);
			this.btnMemberScores.Margin = new System.Windows.Forms.Padding(6);
			this.btnMemberScores.Name = "btnMemberScores";
			this.btnMemberScores.Padding = new System.Windows.Forms.Padding(50, 0, 0, 0);
			this.btnMemberScores.Size = new System.Drawing.Size(484, 187);
			this.btnMemberScores.TabIndex = 0;
			this.btnMemberScores.Text = "Member Scores";
			this.btnMemberScores.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.btnMemberScores.UseVisualStyleBackColor = true;
			this.btnMemberScores.Click += new System.EventHandler(this.btnMemberScores_Click);
			// 
			// btnAbout
			// 
			this.btnAbout.BackgroundImage = global::NineTapTour.Properties.Resources.BowlingPin1;
			this.btnAbout.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.btnAbout.FlatAppearance.BorderSize = 0;
			this.btnAbout.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnAbout.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F);
			this.btnAbout.Location = new System.Drawing.Point(24, 445);
			this.btnAbout.Margin = new System.Windows.Forms.Padding(6);
			this.btnAbout.Name = "btnAbout";
			this.btnAbout.Padding = new System.Windows.Forms.Padding(0, 0, 75, 0);
			this.btnAbout.Size = new System.Drawing.Size(484, 177);
			this.btnAbout.TabIndex = 0;
			this.btnAbout.Text = "About";
			this.btnAbout.UseVisualStyleBackColor = true;
			this.btnAbout.Click += new System.EventHandler(this.btnAbout_Click);
			// 
			// Exit
			// 
			this.Exit.BackgroundImage = global::NineTapTour.Properties.Resources.BowlingPin1;
			this.Exit.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.Exit.FlatAppearance.BorderSize = 0;
			this.Exit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.Exit.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F);
			this.Exit.Location = new System.Drawing.Point(24, 610);
			this.Exit.Margin = new System.Windows.Forms.Padding(6);
			this.Exit.Name = "Exit";
			this.Exit.Padding = new System.Windows.Forms.Padding(0, 0, 75, 0);
			this.Exit.Size = new System.Drawing.Size(484, 190);
			this.Exit.TabIndex = 0;
			this.Exit.Text = "Exit";
			this.Exit.UseVisualStyleBackColor = true;
			this.Exit.Click += new System.EventHandler(this.Exit_Click);
			// 
			// cbxRegionSelect
			// 
			this.cbxRegionSelect.FormattingEnabled = true;
			this.cbxRegionSelect.Location = new System.Drawing.Point(99, 847);
			this.cbxRegionSelect.Margin = new System.Windows.Forms.Padding(6);
			this.cbxRegionSelect.Name = "cbxRegionSelect";
			this.cbxRegionSelect.Size = new System.Drawing.Size(238, 33);
			this.cbxRegionSelect.TabIndex = 3;
			this.cbxRegionSelect.SelectedIndexChanged += new System.EventHandler(this.cbxRegionSelect_SelectedIndexChanged);
			// 
			// btnAdd
			// 
			this.btnAdd.Location = new System.Drawing.Point(99, 899);
			this.btnAdd.Margin = new System.Windows.Forms.Padding(6);
			this.btnAdd.Name = "btnAdd";
			this.btnAdd.Size = new System.Drawing.Size(242, 44);
			this.btnAdd.TabIndex = 4;
			this.btnAdd.Text = "Add Region";
			this.btnAdd.UseVisualStyleBackColor = true;
			this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.BackColor = System.Drawing.Color.White;
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(515, 1327);
			this.label1.Name = "label1";
			this.label1.Padding = new System.Windows.Forms.Padding(10);
			this.label1.Size = new System.Drawing.Size(1023, 51);
			this.label1.TabIndex = 5;
			this.label1.Text = "To report issues with this software, please email Rob Victor at rob1@9taptour.com" +
    "";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// btnDropDataBase1
			// 
			this.btnDropDataBase1.Location = new System.Drawing.Point(99, 967);
			this.btnDropDataBase1.Name = "btnDropDataBase1";
			this.btnDropDataBase1.Size = new System.Drawing.Size(242, 44);
			this.btnDropDataBase1.TabIndex = 6;
			this.btnDropDataBase1.Text = "Delete Region Data";
			this.btnDropDataBase1.UseVisualStyleBackColor = true;
			this.btnDropDataBase1.Click += new System.EventHandler(this.btnDropDataBase1_Click_1);
			// 
			// MainMenu
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(102)))));
			this.BackgroundImage = global::NineTapTour.Properties.Resources._9taplogo1;
			this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.ClientSize = new System.Drawing.Size(1968, 1425);
			this.Controls.Add(this.btnDropDataBase1);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.btnAdd);
			this.Controls.Add(this.cbxRegionSelect);
			this.Controls.Add(this.Exit);
			this.Controls.Add(this.btnAbout);
			this.Controls.Add(this.btnMemberScores);
			this.Controls.Add(this.btnMemberData);
			this.DoubleBuffered = true;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Margin = new System.Windows.Forms.Padding(6);
			this.Name = "MainMenu";
			this.Text = "MainMenu";
			this.Load += new System.EventHandler(this.MainMenu_Load);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.MainMenu_Paint);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnMemberData;
        private System.Windows.Forms.Button btnMemberScores;
        private System.Windows.Forms.Button btnAbout;
        private System.Windows.Forms.Button Exit;
        private System.Windows.Forms.ComboBox cbxRegionSelect;
        private System.Windows.Forms.Button btnAdd;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button btnDropDataBase1;
	}
}