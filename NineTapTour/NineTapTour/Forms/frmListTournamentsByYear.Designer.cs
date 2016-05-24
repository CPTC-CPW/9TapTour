namespace NineTapTour
{
    partial class FrmListTournamentsByYear
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
            this.dgvAllTournaments = new System.Windows.Forms.DataGridView();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cbxYear = new System.Windows.Forms.ComboBox();
            this.btnSearch = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAllTournaments)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvAllTournaments
            // 
            this.dgvAllTournaments.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvAllTournaments.Location = new System.Drawing.Point(68, 197);
            this.dgvAllTournaments.Name = "dgvAllTournaments";
            this.dgvAllTournaments.RowTemplate.Height = 40;
            this.dgvAllTournaments.Size = new System.Drawing.Size(1717, 1183);
            this.dgvAllTournaments.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(106, 64);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(531, 58);
            this.label1.TabIndex = 1;
            this.label1.Text = "Tournaments By Year";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(889, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(185, 32);
            this.label2.TabIndex = 2;
            this.label2.Text = "Select a Year";
            // 
            // cbxYear
            // 
            this.cbxYear.FormattingEnabled = true;
            this.cbxYear.Location = new System.Drawing.Point(878, 64);
            this.cbxYear.Name = "cbxYear";
            this.cbxYear.Size = new System.Drawing.Size(211, 39);
            this.cbxYear.TabIndex = 3;
            this.cbxYear.SelectedIndexChanged += new System.EventHandler(this.cbxYear_SelectedIndexChanged);
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(878, 122);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(211, 47);
            this.btnSearch.TabIndex = 4;
            this.btnSearch.Text = "Search Year";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // FrmListTournamentsByYear
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(16F, 31F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1858, 1419);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.cbxYear);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dgvAllTournaments);
            this.Name = "FrmListTournamentsByYear";
            this.Text = "List of Tournaments By Year";
            this.Load += new System.EventHandler(this.frmListTournamentsByYear_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvAllTournaments)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvAllTournaments;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cbxYear;
        private System.Windows.Forms.Button btnSearch;
    }
}