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
            this.nineTapTourNineTapDbDataSetBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this._NineTapTour_NineTapDbDataSet = new NineTapTour._NineTapTour_NineTapDbDataSet();
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
            this.txtGameTotal = new System.Windows.Forms.TextBox();
            this.txtAveragePerGame = new System.Windows.Forms.TextBox();
            this.txtAverageOnFile = new System.Windows.Forms.TextBox();
            this.txtHandicap = new System.Windows.Forms.TextBox();
            this.txtBonus = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.nineTapTourNineTapDbDataSetBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._NineTapTour_NineTapDbDataSet)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
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
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToOrderColumns = true;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(193, 190);
            this.dataGridView1.Margin = new System.Windows.Forms.Padding(7, 7, 7, 7);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(3382, 1611);
            this.dataGridView1.TabIndex = 3;
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Font = new System.Drawing.Font("Calibri", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblName.Location = new System.Drawing.Point(507, 67);
            this.lblName.Margin = new System.Windows.Forms.Padding(7, 0, 7, 0);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(198, 81);
            this.lblName.TabIndex = 4;
            this.lblName.Text = "Name";
            // 
            // lblMemberNumber
            // 
            this.lblMemberNumber.AutoSize = true;
            this.lblMemberNumber.Font = new System.Drawing.Font("Calibri", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMemberNumber.Location = new System.Drawing.Point(179, 67);
            this.lblMemberNumber.Margin = new System.Windows.Forms.Padding(7, 0, 7, 0);
            this.lblMemberNumber.Name = "lblMemberNumber";
            this.lblMemberNumber.Size = new System.Drawing.Size(304, 81);
            this.lblMemberNumber.TabIndex = 5;
            this.lblMemberNumber.Text = "Member#";
            // 
            // btnPrint
            // 
            this.btnPrint.Location = new System.Drawing.Point(3207, 1629);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(281, 155);
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
            this.txtGame1.Location = new System.Drawing.Point(1482, 1566);
            this.txtGame1.Name = "txtGame1";
            this.txtGame1.Size = new System.Drawing.Size(226, 35);
            this.txtGame1.TabIndex = 7;
            // 
            // txtGame2
            // 
            this.txtGame2.Location = new System.Drawing.Point(1712, 1566);
            this.txtGame2.Name = "txtGame2";
            this.txtGame2.Size = new System.Drawing.Size(226, 35);
            this.txtGame2.TabIndex = 8;
            // 
            // txtGame3
            // 
            this.txtGame3.Location = new System.Drawing.Point(1942, 1566);
            this.txtGame3.Name = "txtGame3";
            this.txtGame3.Size = new System.Drawing.Size(226, 35);
            this.txtGame3.TabIndex = 9;
            // 
            // txtGame4
            // 
            this.txtGame4.Location = new System.Drawing.Point(2172, 1566);
            this.txtGame4.Name = "txtGame4";
            this.txtGame4.Size = new System.Drawing.Size(226, 35);
            this.txtGame4.TabIndex = 10;
            // 
            // txtGameTotal
            // 
            this.txtGameTotal.Location = new System.Drawing.Point(2402, 1566);
            this.txtGameTotal.Name = "txtGameTotal";
            this.txtGameTotal.Size = new System.Drawing.Size(226, 35);
            this.txtGameTotal.TabIndex = 11;
            // 
            // txtAveragePerGame
            // 
            this.txtAveragePerGame.Location = new System.Drawing.Point(2633, 1566);
            this.txtAveragePerGame.Name = "txtAveragePerGame";
            this.txtAveragePerGame.Size = new System.Drawing.Size(226, 35);
            this.txtAveragePerGame.TabIndex = 12;
            // 
            // txtAverageOnFile
            // 
            this.txtAverageOnFile.Location = new System.Drawing.Point(2863, 1566);
            this.txtAverageOnFile.Name = "txtAverageOnFile";
            this.txtAverageOnFile.Size = new System.Drawing.Size(226, 35);
            this.txtAverageOnFile.TabIndex = 13;
            // 
            // txtHandicap
            // 
            this.txtHandicap.Location = new System.Drawing.Point(3093, 1566);
            this.txtHandicap.Name = "txtHandicap";
            this.txtHandicap.Size = new System.Drawing.Size(226, 35);
            this.txtHandicap.TabIndex = 14;
            // 
            // txtBonus
            // 
            this.txtBonus.Location = new System.Drawing.Point(3323, 1566);
            this.txtBonus.Name = "txtBonus";
            this.txtBonus.Size = new System.Drawing.Size(226, 35);
            this.txtBonus.TabIndex = 15;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(1545, 1516);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(97, 29);
            this.label1.TabIndex = 16;
            this.label1.Text = "Game 1";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(1775, 1516);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(97, 29);
            this.label2.TabIndex = 17;
            this.label2.Text = "Game 2";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(2005, 1516);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(97, 29);
            this.label3.TabIndex = 18;
            this.label3.Text = "Game 3";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(2235, 1516);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(97, 29);
            this.label4.TabIndex = 19;
            this.label4.Text = "Game 4";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(2444, 1516);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(139, 29);
            this.label5.TabIndex = 20;
            this.label5.Text = "Game Total";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(2665, 1516);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(156, 29);
            this.label6.TabIndex = 21;
            this.label6.Text = "AvgPerGame";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(2911, 1516);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(126, 29);
            this.label7.TabIndex = 22;
            this.label7.Text = "AvgOnFile";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(3146, 1516);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(115, 29);
            this.label8.TabIndex = 23;
            this.label8.Text = "Handicap";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(3394, 1516);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(81, 29);
            this.label9.TabIndex = 24;
            this.label9.Text = "Bonus";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(1324, 1571);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(114, 29);
            this.label10.TabIndex = 25;
            this.label10.Text = "Averages";
            // 
            // FrmStats
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(14F, 29F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(3619, 1894);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtBonus);
            this.Controls.Add(this.txtHandicap);
            this.Controls.Add(this.txtAverageOnFile);
            this.Controls.Add(this.txtAveragePerGame);
            this.Controls.Add(this.txtGameTotal);
            this.Controls.Add(this.txtGame4);
            this.Controls.Add(this.txtGame3);
            this.Controls.Add(this.txtGame2);
            this.Controls.Add(this.txtGame1);
            this.Controls.Add(this.btnPrint);
            this.Controls.Add(this.lblMemberNumber);
            this.Controls.Add(this.lblName);
            this.Controls.Add(this.dataGridView1);
            this.Margin = new System.Windows.Forms.Padding(7, 7, 7, 7);
            this.Name = "FrmStats";
            this.Text = "FrmStats";
            this.Load += new System.EventHandler(this.FrmStats_Load);
            ((System.ComponentModel.ISupportInitialize)(this.nineTapTourNineTapDbDataSetBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._NineTapTour_NineTapDbDataSet)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
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
        private System.Windows.Forms.TextBox txtGameTotal;
        private System.Windows.Forms.TextBox txtAveragePerGame;
        private System.Windows.Forms.TextBox txtAverageOnFile;
        private System.Windows.Forms.TextBox txtHandicap;
        private System.Windows.Forms.TextBox txtBonus;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
    }
}