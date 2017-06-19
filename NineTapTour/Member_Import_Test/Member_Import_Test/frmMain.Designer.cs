namespace Member_Import_Test
{
    partial class frmMain
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
            this.ofdOpen = new System.Windows.Forms.OpenFileDialog();
            this.btnOpenFile = new System.Windows.Forms.Button();
            this.btnInvalid = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnSelectExcelFolder = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.btnPinFileSelect = new System.Windows.Forms.Button();
            this.btn_FinalizeData = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.progressBar2 = new System.Windows.Forms.ProgressBar();
            this.LabelCurrentFileWorkingOn = new System.Windows.Forms.Label();
            this.OverAllProcessingExcel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnOpenFile
            // 
            this.btnOpenFile.Location = new System.Drawing.Point(75, 25);
            this.btnOpenFile.Name = "btnOpenFile";
            this.btnOpenFile.Size = new System.Drawing.Size(97, 23);
            this.btnOpenFile.TabIndex = 0;
            this.btnOpenFile.Text = "Open File";
            this.btnOpenFile.UseVisualStyleBackColor = true;
            this.btnOpenFile.Click += new System.EventHandler(this.btnOpenFile_Click);
            // 
            // btnInvalid
            // 
            this.btnInvalid.Location = new System.Drawing.Point(75, 54);
            this.btnInvalid.Name = "btnInvalid";
            this.btnInvalid.Size = new System.Drawing.Size(97, 23);
            this.btnInvalid.TabIndex = 1;
            this.btnInvalid.Text = "View Invalid List";
            this.btnInvalid.UseVisualStyleBackColor = true;
            this.btnInvalid.Click += new System.EventHandler(this.btnInvalid_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(65, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(114, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Import Member .dat file";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(65, 132);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(121, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Import Members .xls files";
            // 
            // btnSelectExcelFolder
            // 
            this.btnSelectExcelFolder.Enabled = false;
            this.btnSelectExcelFolder.Location = new System.Drawing.Point(62, 148);
            this.btnSelectExcelFolder.Name = "btnSelectExcelFolder";
            this.btnSelectExcelFolder.Size = new System.Drawing.Size(118, 23);
            this.btnSelectExcelFolder.TabIndex = 3;
            this.btnSelectExcelFolder.Text = "Select .xls Folder";
            this.btnSelectExcelFolder.UseVisualStyleBackColor = true;
            this.btnSelectExcelFolder.Click += new System.EventHandler(this.button1_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(53, 80);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(140, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Import Tournament .pin Files";
            // 
            // btnPinFileSelect
            // 
            this.btnPinFileSelect.Enabled = false;
            this.btnPinFileSelect.Location = new System.Drawing.Point(68, 96);
            this.btnPinFileSelect.Name = "btnPinFileSelect";
            this.btnPinFileSelect.Size = new System.Drawing.Size(112, 23);
            this.btnPinFileSelect.TabIndex = 2;
            this.btnPinFileSelect.Text = "Select .pin Folder";
            this.btnPinFileSelect.UseVisualStyleBackColor = true;
            this.btnPinFileSelect.Click += new System.EventHandler(this.btnPinFileSelect_Click);
            // 
            // btn_FinalizeData
            // 
            this.btn_FinalizeData.Enabled = false;
            this.btn_FinalizeData.Location = new System.Drawing.Point(56, 300);
            this.btn_FinalizeData.Name = "btn_FinalizeData";
            this.btn_FinalizeData.Size = new System.Drawing.Size(75, 23);
            this.btn_FinalizeData.TabIndex = 4;
            this.btn_FinalizeData.Text = "Finalize";
            this.btn_FinalizeData.UseVisualStyleBackColor = true;
            this.btn_FinalizeData.Click += new System.EventHandler(this.btn_FinalizeData_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(56, 189);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(293, 23);
            this.progressBar1.TabIndex = 7;
            // 
            // progressBar2
            // 
            this.progressBar2.Location = new System.Drawing.Point(56, 235);
            this.progressBar2.Name = "progressBar2";
            this.progressBar2.Size = new System.Drawing.Size(293, 23);
            this.progressBar2.TabIndex = 8;
            // 
            // LabelCurrentFileWorkingOn
            // 
            this.LabelCurrentFileWorkingOn.AutoSize = true;
            this.LabelCurrentFileWorkingOn.Location = new System.Drawing.Point(53, 220);
            this.LabelCurrentFileWorkingOn.Name = "LabelCurrentFileWorkingOn";
            this.LabelCurrentFileWorkingOn.Size = new System.Drawing.Size(0, 13);
            this.LabelCurrentFileWorkingOn.TabIndex = 9;
            // 
            // OverAllProcessingExcel
            // 
            this.OverAllProcessingExcel.AutoSize = true;
            this.OverAllProcessingExcel.Location = new System.Drawing.Point(53, 173);
            this.OverAllProcessingExcel.Name = "OverAllProcessingExcel";
            this.OverAllProcessingExcel.Size = new System.Drawing.Size(0, 13);
            this.OverAllProcessingExcel.TabIndex = 10;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(405, 335);
            this.Controls.Add(this.OverAllProcessingExcel);
            this.Controls.Add(this.LabelCurrentFileWorkingOn);
            this.Controls.Add(this.progressBar2);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.btn_FinalizeData);
            this.Controls.Add(this.btnPinFileSelect);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnSelectExcelFolder);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnInvalid);
            this.Controls.Add(this.btnOpenFile);
            this.Name = "frmMain";
            this.Text = "Import Test";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog ofdOpen;
        private System.Windows.Forms.Button btnOpenFile;
        private System.Windows.Forms.Button btnInvalid;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnSelectExcelFolder;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnPinFileSelect;
        private System.Windows.Forms.Button btn_FinalizeData;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.ProgressBar progressBar2;
        private System.Windows.Forms.Label LabelCurrentFileWorkingOn;
        private System.Windows.Forms.Label OverAllProcessingExcel;
    }
}

