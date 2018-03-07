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
            this.btnDropDataBase1 = new System.Windows.Forms.Button();
            this.cbxRegionSelect = new System.Windows.Forms.ComboBox();
            this.btnAdd = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnMemberData
            // 
            this.btnMemberData.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnMemberData.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.btnMemberData.Location = new System.Drawing.Point(44, 207);
            this.btnMemberData.Margin = new System.Windows.Forms.Padding(11, 10, 11, 10);
            this.btnMemberData.Name = "btnMemberData";
            this.btnMemberData.Size = new System.Drawing.Size(433, 210);
            this.btnMemberData.TabIndex = 0;
            this.btnMemberData.Text = "Member Info";
            this.btnMemberData.UseVisualStyleBackColor = true;
            this.btnMemberData.Click += new System.EventHandler(this.btnMemberData_Click);
            // 
            // btnMemberScores
            // 
            this.btnMemberScores.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnMemberScores.Location = new System.Drawing.Point(44, 565);
            this.btnMemberScores.Margin = new System.Windows.Forms.Padding(11, 10, 11, 10);
            this.btnMemberScores.Name = "btnMemberScores";
            this.btnMemberScores.Size = new System.Drawing.Size(444, 210);
            this.btnMemberScores.TabIndex = 0;
            this.btnMemberScores.Text = "Member Scores";
            this.btnMemberScores.UseVisualStyleBackColor = true;
            this.btnMemberScores.Click += new System.EventHandler(this.btnMemberScores_Click);
            // 
            // btnAbout
            // 
            this.btnAbout.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F);
            this.btnAbout.Location = new System.Drawing.Point(44, 795);
            this.btnAbout.Margin = new System.Windows.Forms.Padding(11, 10, 11, 10);
            this.btnAbout.Name = "btnAbout";
            this.btnAbout.Size = new System.Drawing.Size(444, 210);
            this.btnAbout.TabIndex = 0;
            this.btnAbout.Text = "About";
            this.btnAbout.UseVisualStyleBackColor = true;
            this.btnAbout.Click += new System.EventHandler(this.btnAbout_Click);
            // 
            // Exit
            // 
            this.Exit.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F);
            this.Exit.Location = new System.Drawing.Point(44, 1024);
            this.Exit.Margin = new System.Windows.Forms.Padding(11, 10, 11, 10);
            this.Exit.Name = "Exit";
            this.Exit.Size = new System.Drawing.Size(444, 210);
            this.Exit.TabIndex = 0;
            this.Exit.Text = "Exit";
            this.Exit.UseVisualStyleBackColor = true;
            this.Exit.Click += new System.EventHandler(this.Exit_Click);
            // 
            // btnDropDataBase1
            // 
            this.btnDropDataBase1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDropDataBase1.Location = new System.Drawing.Point(44, 1470);
            this.btnDropDataBase1.Margin = new System.Windows.Forms.Padding(11, 10, 11, 10);
            this.btnDropDataBase1.Name = "btnDropDataBase1";
            this.btnDropDataBase1.Size = new System.Drawing.Size(444, 74);
            this.btnDropDataBase1.TabIndex = 2;
            this.btnDropDataBase1.Text = "Delete Region";
            this.btnDropDataBase1.UseVisualStyleBackColor = true;
            this.btnDropDataBase1.Click += new System.EventHandler(this.btnDropDataBase1_Click_1);
            // 
            // cbxRegionSelect
            // 
            this.cbxRegionSelect.FormattingEnabled = true;
            this.cbxRegionSelect.Location = new System.Drawing.Point(44, 1289);
            this.cbxRegionSelect.Margin = new System.Windows.Forms.Padding(11, 10, 11, 10);
            this.cbxRegionSelect.Name = "cbxRegionSelect";
            this.cbxRegionSelect.Size = new System.Drawing.Size(433, 50);
            this.cbxRegionSelect.TabIndex = 3;
            this.cbxRegionSelect.SelectedIndexChanged += new System.EventHandler(this.cbxRegionSelect_SelectedIndexChanged);
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(44, 1376);
            this.btnAdd.Margin = new System.Windows.Forms.Padding(11, 10, 11, 10);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(444, 74);
            this.btnAdd.TabIndex = 4;
            this.btnAdd.Text = "Add Region";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // MainMenu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(22F, 42F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.LightBlue;
            this.BackgroundImage = global::NineTapTour.Properties.Resources._9tap;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.ClientSize = new System.Drawing.Size(3608, 2086);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.cbxRegionSelect);
            this.Controls.Add(this.btnDropDataBase1);
            this.Controls.Add(this.Exit);
            this.Controls.Add(this.btnAbout);
            this.Controls.Add(this.btnMemberScores);
            this.Controls.Add(this.btnMemberData);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(11, 10, 11, 10);
            this.Name = "MainMenu";
            this.Text = "MainMenu";
            this.Load += new System.EventHandler(this.MainMenu_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.MainMenu_Paint);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnMemberData;
        private System.Windows.Forms.Button btnMemberScores;
        private System.Windows.Forms.Button btnAbout;
        private System.Windows.Forms.Button Exit;
        private System.Windows.Forms.Button btnDropDataBase1;
        private System.Windows.Forms.ComboBox cbxRegionSelect;
        private System.Windows.Forms.Button btnAdd;
    }
}