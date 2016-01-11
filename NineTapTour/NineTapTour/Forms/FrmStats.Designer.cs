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
            this.button1 = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.lblName = new System.Windows.Forms.Label();
            this.lblMemberNumber = new System.Windows.Forms.Label();
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
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(2, 191);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "Refresh";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToOrderColumns = true;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(83, 85);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(1036, 722);
            this.dataGridView1.TabIndex = 3;
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
            // FrmStats
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1158, 849);
            this.Controls.Add(this.lblMemberNumber);
            this.Controls.Add(this.lblName);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.button1);
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
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.Label lblMemberNumber;
    }
}