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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.ofdOpen = new System.Windows.Forms.OpenFileDialog();
            this.btnOpenFile = new System.Windows.Forms.Button();
            this.btnInvalid = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnSelectExcelFolder = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.btn_FinalizeData = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.progressBar2 = new System.Windows.Forms.ProgressBar();
            this.LabelCurrentFileWorkingOn = new System.Windows.Forms.Label();
            this.OverAllProcessingExcel = new System.Windows.Forms.Label();
            this.cbxRegionSelect = new System.Windows.Forms.ComboBox();
            this.cbHaw = new System.Windows.Forms.CheckBox();
            this.progressBarFinalize = new System.Windows.Forms.ProgressBar();
            this.label4 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnOpenFile
            // 
            this.btnOpenFile.Location = new System.Drawing.Point(150, 100);
            this.btnOpenFile.Name = "btnOpenFile";
            this.btnOpenFile.Size = new System.Drawing.Size(97, 23);
            this.btnOpenFile.TabIndex = 0;
            this.btnOpenFile.Text = "Open File";
            this.btnOpenFile.UseVisualStyleBackColor = true;
            this.btnOpenFile.Click += new System.EventHandler(this.btnOpenFile_Click);
            // 
            // btnInvalid
            // 
            this.btnInvalid.Location = new System.Drawing.Point(150, 129);
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
            this.label1.Location = new System.Drawing.Point(142, 84);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(114, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Import Member .dat file";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(135, 173);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(121, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Import Members .xls files";
            // 
            // btnSelectExcelFolder
            // 
            this.btnSelectExcelFolder.Enabled = false;
            this.btnSelectExcelFolder.Location = new System.Drawing.Point(150, 189);
            this.btnSelectExcelFolder.Name = "btnSelectExcelFolder";
            this.btnSelectExcelFolder.Size = new System.Drawing.Size(97, 23);
            this.btnSelectExcelFolder.TabIndex = 3;
            this.btnSelectExcelFolder.Text = "Select .xls Folder";
            this.btnSelectExcelFolder.UseVisualStyleBackColor = true;
            this.btnSelectExcelFolder.Click += new System.EventHandler(this.button1_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(163, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(74, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Region Select";
            // 
            // btn_FinalizeData
            // 
            this.btn_FinalizeData.Enabled = false;
            this.btn_FinalizeData.Location = new System.Drawing.Point(150, 300);
            this.btn_FinalizeData.Name = "btn_FinalizeData";
            this.btn_FinalizeData.Size = new System.Drawing.Size(97, 23);
            this.btn_FinalizeData.TabIndex = 4;
            this.btn_FinalizeData.Text = "Finalize";
            this.btn_FinalizeData.UseVisualStyleBackColor = true;
            this.btn_FinalizeData.Click += new System.EventHandler(this.btn_FinalizeData_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(56, 220);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(293, 23);
            this.progressBar1.TabIndex = 7;
            // 
            // progressBar2
            // 
            this.progressBar2.Location = new System.Drawing.Point(56, 255);
            this.progressBar2.Name = "progressBar2";
            this.progressBar2.Size = new System.Drawing.Size(293, 23);
            this.progressBar2.TabIndex = 8;
            // 
            // LabelCurrentFileWorkingOn
            // 
            this.LabelCurrentFileWorkingOn.AutoSize = true;
            this.LabelCurrentFileWorkingOn.Location = new System.Drawing.Point(53, 255);
            this.LabelCurrentFileWorkingOn.Name = "LabelCurrentFileWorkingOn";
            this.LabelCurrentFileWorkingOn.Size = new System.Drawing.Size(0, 13);
            this.LabelCurrentFileWorkingOn.TabIndex = 9;
            // 
            // OverAllProcessingExcel
            // 
            this.OverAllProcessingExcel.AutoSize = true;
            this.OverAllProcessingExcel.Location = new System.Drawing.Point(53, 220);
            this.OverAllProcessingExcel.Name = "OverAllProcessingExcel";
            this.OverAllProcessingExcel.Size = new System.Drawing.Size(0, 13);
            this.OverAllProcessingExcel.TabIndex = 10;
            // 
            // cbxRegionSelect
            // 
            this.cbxRegionSelect.FormattingEnabled = true;
            this.cbxRegionSelect.Location = new System.Drawing.Point(138, 28);
            this.cbxRegionSelect.Name = "cbxRegionSelect";
            this.cbxRegionSelect.Size = new System.Drawing.Size(121, 21);
            this.cbxRegionSelect.TabIndex = 11;
            this.cbxRegionSelect.SelectedIndexChanged += new System.EventHandler(this.cbxRegionSelect_SelectedIndexChanged);
            // 
            // cbHaw
            // 
            this.cbHaw.AutoSize = true;
            this.cbHaw.Location = new System.Drawing.Point(166, 56);
            this.cbHaw.Name = "cbHaw";
            this.cbHaw.Size = new System.Drawing.Size(67, 17);
            this.cbHaw.TabIndex = 12;
            this.cbHaw.Text = "Hawaii ?";
            this.cbHaw.UseVisualStyleBackColor = true;
            // 
            // progressBarFinalize
            // 
            this.progressBarFinalize.Location = new System.Drawing.Point(56, 335);
            this.progressBarFinalize.Name = "progressBarFinalize";
            this.progressBarFinalize.Size = new System.Drawing.Size(293, 23);
            this.progressBarFinalize.TabIndex = 13;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(53, 335);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(35, 13);
            this.label4.TabIndex = 14;
            this.label4.Text = "label4";
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(405, 370);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.progressBarFinalize);
            this.Controls.Add(this.cbHaw);
            this.Controls.Add(this.cbxRegionSelect);
            this.Controls.Add(this.OverAllProcessingExcel);
            this.Controls.Add(this.LabelCurrentFileWorkingOn);
            this.Controls.Add(this.progressBar2);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.btn_FinalizeData);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnSelectExcelFolder);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnInvalid);
            this.Controls.Add(this.btnOpenFile);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
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
        private System.Windows.Forms.Button btn_FinalizeData;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.ProgressBar progressBar2;
        private System.Windows.Forms.Label LabelCurrentFileWorkingOn;
        private System.Windows.Forms.Label OverAllProcessingExcel;
        private System.Windows.Forms.ComboBox cbxRegionSelect;
        private System.Windows.Forms.CheckBox cbHaw;
        private System.Windows.Forms.ProgressBar progressBarFinalize;
        private System.Windows.Forms.Label label4;
    }
}

