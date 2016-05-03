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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.txtGame1 = new System.Windows.Forms.TextBox();
            this.txtGame2 = new System.Windows.Forms.TextBox();
            this.txtGame3 = new System.Windows.Forms.TextBox();
            this.txtGame4 = new System.Windows.Forms.TextBox();
            this.txtGameTotal = new System.Windows.Forms.TextBox();
            this.txtAveragePerGame = new System.Windows.Forms.TextBox();
            this.txtAverageOnFile = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.txtHandicap = new System.Windows.Forms.TextBox();
            this.txtBonus = new System.Windows.Forms.TextBox();
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
            this.dataGridView1.Location = new System.Drawing.Point(166, 163);
            this.dataGridView1.Margin = new System.Windows.Forms.Padding(6);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(2072, 1388);
            this.dataGridView1.TabIndex = 3;
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Font = new System.Drawing.Font("Calibri", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblName.Location = new System.Drawing.Point(434, 58);
            this.lblName.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(173, 71);
            this.lblName.TabIndex = 4;
            this.lblName.Text = "Name";
            // 
            // lblMemberNumber
            // 
            this.lblMemberNumber.AutoSize = true;
            this.lblMemberNumber.Font = new System.Drawing.Font("Calibri", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMemberNumber.Location = new System.Drawing.Point(154, 58);
            this.lblMemberNumber.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblMemberNumber.Name = "lblMemberNumber";
            this.lblMemberNumber.Size = new System.Drawing.Size(267, 71);
            this.lblMemberNumber.TabIndex = 5;
            this.lblMemberNumber.Text = "Member#";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(222, 1346);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(183, 25);
            this.label1.TabIndex = 6;
            this.label1.Text = "Overall Averages:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(544, 1296);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(87, 25);
            this.label2.TabIndex = 7;
            this.label2.Text = "Game 1";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(738, 1296);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(87, 25);
            this.label3.TabIndex = 8;
            this.label3.Text = "Game 2";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(932, 1296);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(87, 25);
            this.label4.TabIndex = 9;
            this.label4.Text = "Game 3";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(1126, 1296);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(87, 25);
            this.label5.TabIndex = 10;
            this.label5.Text = "Game 4";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(1302, 1296);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(123, 25);
            this.label6.TabIndex = 11;
            this.label6.Text = "Game Total";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(1504, 1296);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(107, 25);
            this.label7.TabIndex = 12;
            this.label7.Text = "Avr/Game";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(1681, 1296);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(141, 25);
            this.label8.TabIndex = 13;
            this.label8.Text = "AvrOnRecord";
            // 
            // txtGame1
            // 
            this.txtGame1.Location = new System.Drawing.Point(488, 1346);
            this.txtGame1.Name = "txtGame1";
            this.txtGame1.Size = new System.Drawing.Size(194, 31);
            this.txtGame1.TabIndex = 14;
            // 
            // txtGame2
            // 
            this.txtGame2.Location = new System.Drawing.Point(682, 1346);
            this.txtGame2.Name = "txtGame2";
            this.txtGame2.Size = new System.Drawing.Size(194, 31);
            this.txtGame2.TabIndex = 15;
            // 
            // txtGame3
            // 
            this.txtGame3.Location = new System.Drawing.Point(876, 1346);
            this.txtGame3.Name = "txtGame3";
            this.txtGame3.Size = new System.Drawing.Size(194, 31);
            this.txtGame3.TabIndex = 16;
            // 
            // txtGame4
            // 
            this.txtGame4.Location = new System.Drawing.Point(1070, 1346);
            this.txtGame4.Name = "txtGame4";
            this.txtGame4.Size = new System.Drawing.Size(194, 31);
            this.txtGame4.TabIndex = 17;
            // 
            // txtGameTotal
            // 
            this.txtGameTotal.Location = new System.Drawing.Point(1264, 1346);
            this.txtGameTotal.Name = "txtGameTotal";
            this.txtGameTotal.Size = new System.Drawing.Size(194, 31);
            this.txtGameTotal.TabIndex = 18;
            // 
            // txtAveragePerGame
            // 
            this.txtAveragePerGame.Location = new System.Drawing.Point(1458, 1346);
            this.txtAveragePerGame.Name = "txtAveragePerGame";
            this.txtAveragePerGame.Size = new System.Drawing.Size(194, 31);
            this.txtAveragePerGame.TabIndex = 19;
            // 
            // txtAverageOnFile
            // 
            this.txtAverageOnFile.Location = new System.Drawing.Point(1652, 1346);
            this.txtAverageOnFile.Name = "txtAverageOnFile";
            this.txtAverageOnFile.Size = new System.Drawing.Size(194, 31);
            this.txtAverageOnFile.TabIndex = 20;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(1977, 1399);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(242, 83);
            this.button1.TabIndex = 21;
            this.button1.Text = "Print";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // txtHandicap
            // 
            this.txtHandicap.Location = new System.Drawing.Point(1846, 1346);
            this.txtHandicap.Name = "txtHandicap";
            this.txtHandicap.Size = new System.Drawing.Size(194, 31);
            this.txtHandicap.TabIndex = 22;
            // 
            // txtBonus
            // 
            this.txtBonus.Location = new System.Drawing.Point(2040, 1346);
            this.txtBonus.Name = "txtBonus";
            this.txtBonus.Size = new System.Drawing.Size(194, 31);
            this.txtBonus.TabIndex = 24;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(1894, 1296);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(103, 25);
            this.label9.TabIndex = 25;
            this.label9.Text = "Handicap";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(2103, 1296);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(73, 25);
            this.label10.TabIndex = 26;
            this.label10.Text = "Bonus";
            // 
            // FrmStats
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(2316, 1633);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.txtBonus);
            this.Controls.Add(this.txtHandicap);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.txtAverageOnFile);
            this.Controls.Add(this.txtAveragePerGame);
            this.Controls.Add(this.txtGameTotal);
            this.Controls.Add(this.txtGame4);
            this.Controls.Add(this.txtGame3);
            this.Controls.Add(this.txtGame2);
            this.Controls.Add(this.txtGame1);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblMemberNumber);
            this.Controls.Add(this.lblName);
            this.Controls.Add(this.dataGridView1);
            this.Margin = new System.Windows.Forms.Padding(6);
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
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtGame1;
        private System.Windows.Forms.TextBox txtGame2;
        private System.Windows.Forms.TextBox txtGame3;
        private System.Windows.Forms.TextBox txtGame4;
        private System.Windows.Forms.TextBox txtGameTotal;
        private System.Windows.Forms.TextBox txtAveragePerGame;
        private System.Windows.Forms.TextBox txtAverageOnFile;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox txtHandicap;
        private System.Windows.Forms.TextBox txtBonus;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
    }
}