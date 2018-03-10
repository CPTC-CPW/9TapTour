namespace NineTapTour.Forms
{
    partial class FrmStats
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmStats));
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.lblName = new System.Windows.Forms.Label();
            this.lblMemberNumber = new System.Windows.Forms.Label();
            this.btnPrint = new System.Windows.Forms.Button();
            this.printDialog1 = new System.Windows.Forms.PrintDialog();
            this.printDocument1 = new System.Drawing.Printing.PrintDocument();
            this.txtGame1 = new System.Windows.Forms.TextBox();
            this.txtGame2 = new System.Windows.Forms.TextBox();
            this.txtGame3 = new System.Windows.Forms.TextBox();
            this.txtGame4 = new System.Windows.Forms.TextBox();
            this.txtScratchTotal = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.txtGameTotal = new System.Windows.Forms.TextBox();
            this.lblStartAvg = new System.Windows.Forms.Label();
            this.txtBonus = new System.Windows.Forms.TextBox();
            this.txtHandicap = new System.Windows.Forms.TextBox();
            this.txtAveragePerGame = new System.Windows.Forms.TextBox();
            this.nineTapTourNineTapDbDataSetBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this._NineTapTour_NineTapDbDataSet = new NineTapTour._NineTapTour_NineTapDbDataSet();
            this.btnSaveChanges = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nineTapTourNineTapDbDataSetBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._NineTapTour_NineTapDbDataSet)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(83, 85);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(988, 384);
            this.dataGridView1.StandardTab = true;
            this.dataGridView1.TabIndex = 3;
            this.dataGridView1.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dataGridView1_CellFormatting);
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Font = new System.Drawing.Font("Calibri", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblName.Location = new System.Drawing.Point(217, 30);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(87, 36);
            this.lblName.TabIndex = 4;
            this.lblName.Text = "Name";
            // 
            // lblMemberNumber
            // 
            this.lblMemberNumber.AutoSize = true;
            this.lblMemberNumber.Font = new System.Drawing.Font("Calibri", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMemberNumber.Location = new System.Drawing.Point(77, 30);
            this.lblMemberNumber.Name = "lblMemberNumber";
            this.lblMemberNumber.Size = new System.Drawing.Size(134, 36);
            this.lblMemberNumber.TabIndex = 5;
            this.lblMemberNumber.Text = "Member#";
            // 
            // btnPrint
            // 
            this.btnPrint.Location = new System.Drawing.Point(985, 561);
            this.btnPrint.Margin = new System.Windows.Forms.Padding(1);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(86, 22);
            this.btnPrint.TabIndex = 6;
            this.btnPrint.Text = "Print";
            this.btnPrint.UseVisualStyleBackColor = true;
            this.btnPrint.Click += new System.EventHandler(this.button1_Click);
            // 
            // printDialog1
            // 
            this.printDialog1.UseEXDialog = true;
            // 
            // printDocument1
            // 
            this.printDocument1.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(this.printDocument1_PrintPage);
            // 
            // txtGame1
            // 
            this.txtGame1.Location = new System.Drawing.Point(159, 506);
            this.txtGame1.Margin = new System.Windows.Forms.Padding(1);
            this.txtGame1.Name = "txtGame1";
            this.txtGame1.ReadOnly = true;
            this.txtGame1.Size = new System.Drawing.Size(99, 20);
            this.txtGame1.TabIndex = 7;
            // 
            // txtGame2
            // 
            this.txtGame2.Location = new System.Drawing.Point(258, 506);
            this.txtGame2.Margin = new System.Windows.Forms.Padding(1);
            this.txtGame2.Name = "txtGame2";
            this.txtGame2.ReadOnly = true;
            this.txtGame2.Size = new System.Drawing.Size(99, 20);
            this.txtGame2.TabIndex = 8;
            // 
            // txtGame3
            // 
            this.txtGame3.Location = new System.Drawing.Point(356, 506);
            this.txtGame3.Margin = new System.Windows.Forms.Padding(1);
            this.txtGame3.Name = "txtGame3";
            this.txtGame3.ReadOnly = true;
            this.txtGame3.Size = new System.Drawing.Size(99, 20);
            this.txtGame3.TabIndex = 9;
            // 
            // txtGame4
            // 
            this.txtGame4.Location = new System.Drawing.Point(455, 506);
            this.txtGame4.Margin = new System.Windows.Forms.Padding(1);
            this.txtGame4.Name = "txtGame4";
            this.txtGame4.ReadOnly = true;
            this.txtGame4.Size = new System.Drawing.Size(99, 20);
            this.txtGame4.TabIndex = 10;
            // 
            // txtScratchTotal
            // 
            this.txtScratchTotal.Location = new System.Drawing.Point(553, 506);
            this.txtScratchTotal.Margin = new System.Windows.Forms.Padding(1);
            this.txtScratchTotal.Name = "txtScratchTotal";
            this.txtScratchTotal.ReadOnly = true;
            this.txtScratchTotal.Size = new System.Drawing.Size(99, 20);
            this.txtScratchTotal.TabIndex = 11;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(186, 484);
            this.label1.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 13);
            this.label1.TabIndex = 16;
            this.label1.Text = "Game 1";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(285, 484);
            this.label2.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(44, 13);
            this.label2.TabIndex = 17;
            this.label2.Text = "Game 2";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(383, 484);
            this.label3.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(44, 13);
            this.label3.TabIndex = 18;
            this.label3.Text = "Game 3";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(482, 484);
            this.label4.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(44, 13);
            this.label4.TabIndex = 19;
            this.label4.Text = "Game 4";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(571, 484);
            this.label5.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(71, 13);
            this.label5.TabIndex = 20;
            this.label5.Text = "Scratch Total";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(91, 508);
            this.label10.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(52, 13);
            this.label10.TabIndex = 25;
            this.label10.Text = "Averages";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(650, 484);
            this.label7.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(114, 13);
            this.label7.TabIndex = 26;
            this.label7.Text = "Game Total  W/HDCP";
            // 
            // txtGameTotal
            // 
            this.txtGameTotal.Location = new System.Drawing.Point(653, 506);
            this.txtGameTotal.Margin = new System.Windows.Forms.Padding(1);
            this.txtGameTotal.Name = "txtGameTotal";
            this.txtGameTotal.Size = new System.Drawing.Size(99, 20);
            this.txtGameTotal.TabIndex = 27;
            // 
            // lblStartAvg
            // 
            this.lblStartAvg.AutoSize = true;
            this.lblStartAvg.Font = new System.Drawing.Font("Calibri", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStartAvg.Location = new System.Drawing.Point(800, 30);
            this.lblStartAvg.Name = "lblStartAvg";
            this.lblStartAvg.Size = new System.Drawing.Size(30, 36);
            this.lblStartAvg.TabIndex = 28;
            this.lblStartAvg.Text = "0";
            // 
            // txtBonus
            // 
            this.txtBonus.Location = new System.Drawing.Point(953, 506);
            this.txtBonus.Margin = new System.Windows.Forms.Padding(1);
            this.txtBonus.Name = "txtBonus";
            this.txtBonus.ReadOnly = true;
            this.txtBonus.Size = new System.Drawing.Size(99, 20);
            this.txtBonus.TabIndex = 15;
            // 
            // txtHandicap
            // 
            this.txtHandicap.Location = new System.Drawing.Point(852, 506);
            this.txtHandicap.Margin = new System.Windows.Forms.Padding(1);
            this.txtHandicap.Name = "txtHandicap";
            this.txtHandicap.ReadOnly = true;
            this.txtHandicap.Size = new System.Drawing.Size(99, 20);
            this.txtHandicap.TabIndex = 14;
            // 
            // txtAveragePerGame
            // 
            this.txtAveragePerGame.Location = new System.Drawing.Point(752, 506);
            this.txtAveragePerGame.Margin = new System.Windows.Forms.Padding(1);
            this.txtAveragePerGame.Name = "txtAveragePerGame";
            this.txtAveragePerGame.ReadOnly = true;
            this.txtAveragePerGame.Size = new System.Drawing.Size(99, 20);
            this.txtAveragePerGame.TabIndex = 12;
            // 
            // nineTapTourNineTapDbDataSetBindingSource
            // 
            this.nineTapTourNineTapDbDataSetBindingSource.DataSource = this._NineTapTour_NineTapDbDataSet;
            this.nineTapTourNineTapDbDataSetBindingSource.Position = 0;
            // 
            // _NineTapTour_NineTapDbDataSet
            // 
            this._NineTapTour_NineTapDbDataSet.DataSetName = "_NineTapTour_NineTapDbDataSet";
            this._NineTapTour_NineTapDbDataSet.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // btnSaveChanges
            // 
            this.btnSaveChanges.Location = new System.Drawing.Point(83, 561);
            this.btnSaveChanges.Name = "btnSaveChanges";
            this.btnSaveChanges.Size = new System.Drawing.Size(75, 23);
            this.btnSaveChanges.TabIndex = 29;
            this.btnSaveChanges.Text = "Save";
            this.btnSaveChanges.UseVisualStyleBackColor = true;
            this.btnSaveChanges.Click += new System.EventHandler(this.btnSaveChanges_Click);
            // 
            // FrmStats
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1118, 613);
            this.Controls.Add(this.btnSaveChanges);
            this.Controls.Add(this.lblStartAvg);
            this.Controls.Add(this.txtGameTotal);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtBonus);
            this.Controls.Add(this.txtHandicap);
            this.Controls.Add(this.txtAveragePerGame);
            this.Controls.Add(this.txtScratchTotal);
            this.Controls.Add(this.txtGame4);
            this.Controls.Add(this.txtGame3);
            this.Controls.Add(this.txtGame2);
            this.Controls.Add(this.txtGame1);
            this.Controls.Add(this.btnPrint);
            this.Controls.Add(this.lblMemberNumber);
            this.Controls.Add(this.lblName);
            this.Controls.Add(this.dataGridView1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FrmStats";
            this.Text = "FrmStats";
            this.Load += new System.EventHandler(this.FrmStats_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nineTapTourNineTapDbDataSetBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._NineTapTour_NineTapDbDataSet)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.BindingSource nineTapTourNineTapDbDataSetBindingSource;
        private _NineTapTour_NineTapDbDataSet _NineTapTour_NineTapDbDataSet;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.Label lblMemberNumber;
        private System.Windows.Forms.Button btnPrint;
        private System.Windows.Forms.PrintDialog printDialog1;
        private System.Drawing.Printing.PrintDocument printDocument1;
        private System.Windows.Forms.TextBox txtGame1;
        private System.Windows.Forms.TextBox txtGame2;
        private System.Windows.Forms.TextBox txtGame3;
        private System.Windows.Forms.TextBox txtGame4;
        private System.Windows.Forms.TextBox txtScratchTotal;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtGameTotal;
        private System.Windows.Forms.Label lblStartAvg;
        private System.Windows.Forms.TextBox txtBonus;
        private System.Windows.Forms.TextBox txtHandicap;
        private System.Windows.Forms.TextBox txtAveragePerGame;
        private System.Windows.Forms.Button btnSaveChanges;
    }
}