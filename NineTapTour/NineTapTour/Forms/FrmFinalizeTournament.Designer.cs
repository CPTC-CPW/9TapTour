namespace NineTapTour.Forms
{
    partial class FrmFinalizeTournament
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmFinalizeTournament));
            this.TournamentEntriesGrid = new System.Windows.Forms.DataGridView();
            this.btnFinalize = new System.Windows.Forms.Button();
            this.playerTournamentHistoryGrid = new System.Windows.Forms.DataGridView();
            this.lblMemberNumber = new System.Windows.Forms.Label();
            this.lblName = new System.Windows.Forms.Label();
            this.lblStartAvg = new System.Windows.Forms.Label();
            this.lblMemNum = new System.Windows.Forms.Label();
            this.lblAveNum = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.TournamentEntriesGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.playerTournamentHistoryGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // TournamentEntriesGrid
            // 
            this.TournamentEntriesGrid.AllowUserToAddRows = false;
            this.TournamentEntriesGrid.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.LightGray;
            this.TournamentEntriesGrid.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.TournamentEntriesGrid.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TournamentEntriesGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.TournamentEntriesGrid.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.TournamentEntriesGrid.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.TournamentEntriesGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.TournamentEntriesGrid.DefaultCellStyle = dataGridViewCellStyle3;
            this.TournamentEntriesGrid.Location = new System.Drawing.Point(14, 40);
            this.TournamentEntriesGrid.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.TournamentEntriesGrid.Name = "TournamentEntriesGrid";
            this.TournamentEntriesGrid.Size = new System.Drawing.Size(1447, 322);
            this.TournamentEntriesGrid.TabIndex = 0;
            this.TournamentEntriesGrid.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.TournamentEntriesGrid_CellClick);
            this.TournamentEntriesGrid.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.TournamentEntriesGrid_CellContentClick);
            this.TournamentEntriesGrid.CellMouseUp += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.DataGridView1_OnCellMouseUp);
            this.TournamentEntriesGrid.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.TournamentEntriesGrid_CellValueChanged);
            this.TournamentEntriesGrid.Sorted += new System.EventHandler(this.DataGridView1_Sorted);
            // 
            // btnFinalize
            // 
            this.btnFinalize.BackColor = System.Drawing.Color.White;
            this.btnFinalize.FlatAppearance.BorderColor = System.Drawing.Color.Red;
            this.btnFinalize.FlatAppearance.BorderSize = 2;
            this.btnFinalize.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFinalize.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnFinalize.Location = new System.Drawing.Point(1200, 1);
            this.btnFinalize.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnFinalize.Name = "btnFinalize";
            this.btnFinalize.Size = new System.Drawing.Size(209, 32);
            this.btnFinalize.TabIndex = 1;
            this.btnFinalize.Text = "Finalize Tournament";
            this.btnFinalize.UseVisualStyleBackColor = false;
            this.btnFinalize.Click += new System.EventHandler(this.BtnFinalize_Click);
            // 
            // playerTournamentHistoryGrid
            // 
            this.playerTournamentHistoryGrid.AllowUserToAddRows = false;
            this.playerTournamentHistoryGrid.AllowUserToDeleteRows = false;
            this.playerTournamentHistoryGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.playerTournamentHistoryGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.playerTournamentHistoryGrid.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.playerTournamentHistoryGrid.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.playerTournamentHistoryGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.playerTournamentHistoryGrid.DefaultCellStyle = dataGridViewCellStyle5;
            this.playerTournamentHistoryGrid.Location = new System.Drawing.Point(14, 411);
            this.playerTournamentHistoryGrid.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.playerTournamentHistoryGrid.Name = "playerTournamentHistoryGrid";
            this.playerTournamentHistoryGrid.ReadOnly = true;
            this.playerTournamentHistoryGrid.Size = new System.Drawing.Size(1447, 375);
            this.playerTournamentHistoryGrid.TabIndex = 2;
            // 
            // lblMemberNumber
            // 
            this.lblMemberNumber.AutoSize = true;
            this.lblMemberNumber.Font = new System.Drawing.Font("Calibri", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblMemberNumber.Location = new System.Drawing.Point(251, 366);
            this.lblMemberNumber.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblMemberNumber.Name = "lblMemberNumber";
            this.lblMemberNumber.Size = new System.Drawing.Size(99, 36);
            this.lblMemberNumber.TabIndex = 6;
            this.lblMemberNumber.Text = "______";
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Font = new System.Drawing.Font("Calibri", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblName.Location = new System.Drawing.Point(489, 363);
            this.lblName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(87, 36);
            this.lblName.TabIndex = 7;
            this.lblName.Text = "Name";
            // 
            // lblStartAvg
            // 
            this.lblStartAvg.AutoSize = true;
            this.lblStartAvg.Font = new System.Drawing.Font("Calibri", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblStartAvg.Location = new System.Drawing.Point(1374, 366);
            this.lblStartAvg.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblStartAvg.Name = "lblStartAvg";
            this.lblStartAvg.Size = new System.Drawing.Size(30, 36);
            this.lblStartAvg.TabIndex = 29;
            this.lblStartAvg.Text = "0";
            // 
            // lblMemNum
            // 
            this.lblMemNum.AutoSize = true;
            this.lblMemNum.Font = new System.Drawing.Font("Calibri", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblMemNum.Location = new System.Drawing.Point(144, 366);
            this.lblMemNum.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblMemNum.Name = "lblMemNum";
            this.lblMemNum.Size = new System.Drawing.Size(93, 36);
            this.lblMemNum.TabIndex = 30;
            this.lblMemNum.Text = "Mem#";
            // 
            // lblAveNum
            // 
            this.lblAveNum.AutoSize = true;
            this.lblAveNum.Font = new System.Drawing.Font("Calibri", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblAveNum.Location = new System.Drawing.Point(1259, 367);
            this.lblAveNum.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblAveNum.Name = "lblAveNum";
            this.lblAveNum.Size = new System.Drawing.Size(67, 36);
            this.lblAveNum.TabIndex = 31;
            this.lblAveNum.Text = "AVG";
            // 
            // FrmFinalizeTournament
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1475, 800);
            this.Controls.Add(this.btnFinalize);
            this.Controls.Add(this.lblAveNum);
            this.Controls.Add(this.lblMemNum);
            this.Controls.Add(this.lblStartAvg);
            this.Controls.Add(this.lblName);
            this.Controls.Add(this.lblMemberNumber);
            this.Controls.Add(this.playerTournamentHistoryGrid);
            this.Controls.Add(this.TournamentEntriesGrid);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "FrmFinalizeTournament";
            this.Text = "Finalize Tournament Results";
            this.Load += new System.EventHandler(this.FrmFinalizeTournament_Load);
            ((System.ComponentModel.ISupportInitialize)(this.TournamentEntriesGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.playerTournamentHistoryGrid)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView TournamentEntriesGrid;
        private System.Windows.Forms.Button btnFinalize;
        private System.Windows.Forms.DataGridView playerTournamentHistoryGrid;
        private System.Windows.Forms.Label lblMemberNumber;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.Label lblStartAvg;
        private System.Windows.Forms.Label lblMemNum;
        private System.Windows.Forms.Label lblAveNum;
    }
}