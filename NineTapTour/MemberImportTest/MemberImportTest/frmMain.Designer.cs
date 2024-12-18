namespace MemberImportTest
{
    partial class FrmMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmMain));
            ofdOpen = new System.Windows.Forms.OpenFileDialog();
            btnOpenFile = new System.Windows.Forms.Button();
            btnInvalid = new System.Windows.Forms.Button();
            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            btnSelectExcelFolder = new System.Windows.Forms.Button();
            label3 = new System.Windows.Forms.Label();
            btn_FinalizeData = new System.Windows.Forms.Button();
            progressBar1 = new System.Windows.Forms.ProgressBar();
            progressBar2 = new System.Windows.Forms.ProgressBar();
            LabelCurrentFileWorkingOn = new System.Windows.Forms.Label();
            OverAllProcessingExcel = new System.Windows.Forms.Label();
            cbxRegionSelect = new System.Windows.Forms.ComboBox();
            cbHaw = new System.Windows.Forms.CheckBox();
            progressBarFinalize = new System.Windows.Forms.ProgressBar();
            lblFinalizeStatus = new System.Windows.Forms.Label();
            SuspendLayout();
            // 
            // btnOpenFile
            // 
            btnOpenFile.Location = new System.Drawing.Point(175, 115);
            btnOpenFile.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnOpenFile.Name = "btnOpenFile";
            btnOpenFile.Size = new System.Drawing.Size(113, 27);
            btnOpenFile.TabIndex = 0;
            btnOpenFile.Text = "Open File";
            btnOpenFile.UseVisualStyleBackColor = true;
            btnOpenFile.Click += BtnOpenFile_Click;
            // 
            // btnInvalid
            // 
            btnInvalid.Location = new System.Drawing.Point(175, 149);
            btnInvalid.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnInvalid.Name = "btnInvalid";
            btnInvalid.Size = new System.Drawing.Size(113, 27);
            btnInvalid.TabIndex = 1;
            btnInvalid.Text = "View Invalid List";
            btnInvalid.UseVisualStyleBackColor = true;
            btnInvalid.Click += BtnInvalid_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(166, 97);
            label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(133, 15);
            label1.TabIndex = 3;
            label1.Text = "Import Member .dat file";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(158, 200);
            label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(150, 15);
            label2.TabIndex = 4;
            label2.Text = "Import Members Excel files";
            // 
            // btnSelectExcelFolder
            // 
            btnSelectExcelFolder.Enabled = false;
            btnSelectExcelFolder.Location = new System.Drawing.Point(138, 218);
            btnSelectExcelFolder.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnSelectExcelFolder.Name = "btnSelectExcelFolder";
            btnSelectExcelFolder.Size = new System.Drawing.Size(181, 27);
            btnSelectExcelFolder.TabIndex = 3;
            btnSelectExcelFolder.Text = "Select Excel files Folder";
            btnSelectExcelFolder.UseVisualStyleBackColor = true;
            btnSelectExcelFolder.Click += Button1_Click;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(190, 10);
            label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(78, 15);
            label3.TabIndex = 6;
            label3.Text = "Region Select";
            // 
            // btn_FinalizeData
            // 
            btn_FinalizeData.Enabled = false;
            btn_FinalizeData.Location = new System.Drawing.Point(175, 346);
            btn_FinalizeData.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btn_FinalizeData.Name = "btn_FinalizeData";
            btn_FinalizeData.Size = new System.Drawing.Size(113, 27);
            btn_FinalizeData.TabIndex = 4;
            btn_FinalizeData.Text = "Finalize";
            btn_FinalizeData.UseVisualStyleBackColor = true;
            btn_FinalizeData.Click += Btn_FinalizeData_Click;
            // 
            // progressBar1
            // 
            progressBar1.Location = new System.Drawing.Point(65, 254);
            progressBar1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new System.Drawing.Size(342, 27);
            progressBar1.TabIndex = 7;
            // 
            // progressBar2
            // 
            progressBar2.Location = new System.Drawing.Point(65, 294);
            progressBar2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            progressBar2.Name = "progressBar2";
            progressBar2.Size = new System.Drawing.Size(342, 27);
            progressBar2.TabIndex = 8;
            // 
            // LabelCurrentFileWorkingOn
            // 
            LabelCurrentFileWorkingOn.AutoSize = true;
            LabelCurrentFileWorkingOn.Location = new System.Drawing.Point(62, 294);
            LabelCurrentFileWorkingOn.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            LabelCurrentFileWorkingOn.Name = "LabelCurrentFileWorkingOn";
            LabelCurrentFileWorkingOn.Size = new System.Drawing.Size(0, 15);
            LabelCurrentFileWorkingOn.TabIndex = 9;
            // 
            // OverAllProcessingExcel
            // 
            OverAllProcessingExcel.AutoSize = true;
            OverAllProcessingExcel.Location = new System.Drawing.Point(62, 254);
            OverAllProcessingExcel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            OverAllProcessingExcel.Name = "OverAllProcessingExcel";
            OverAllProcessingExcel.Size = new System.Drawing.Size(0, 15);
            OverAllProcessingExcel.TabIndex = 10;
            // 
            // cbxRegionSelect
            // 
            cbxRegionSelect.FormattingEnabled = true;
            cbxRegionSelect.Location = new System.Drawing.Point(161, 32);
            cbxRegionSelect.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            cbxRegionSelect.Name = "cbxRegionSelect";
            cbxRegionSelect.Size = new System.Drawing.Size(140, 23);
            cbxRegionSelect.TabIndex = 11;
            cbxRegionSelect.SelectedIndexChanged += CbxRegionSelect_SelectedIndexChanged;
            // 
            // cbHaw
            // 
            cbHaw.AutoSize = true;
            cbHaw.Location = new System.Drawing.Point(194, 65);
            cbHaw.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            cbHaw.Name = "cbHaw";
            cbHaw.Size = new System.Drawing.Size(70, 19);
            cbHaw.TabIndex = 12;
            cbHaw.Text = "Hawaii ?";
            cbHaw.UseVisualStyleBackColor = true;
            // 
            // progressBarFinalize
            // 
            progressBarFinalize.Location = new System.Drawing.Point(65, 387);
            progressBarFinalize.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            progressBarFinalize.Name = "progressBarFinalize";
            progressBarFinalize.Size = new System.Drawing.Size(342, 27);
            progressBarFinalize.TabIndex = 13;
            // 
            // lblFinalizeStatus
            // 
            lblFinalizeStatus.AutoSize = true;
            lblFinalizeStatus.Location = new System.Drawing.Point(62, 387);
            lblFinalizeStatus.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblFinalizeStatus.Name = "lblFinalizeStatus";
            lblFinalizeStatus.Size = new System.Drawing.Size(0, 15);
            lblFinalizeStatus.TabIndex = 14;
            // 
            // frmMain
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.White;
            ClientSize = new System.Drawing.Size(472, 427);
            Controls.Add(lblFinalizeStatus);
            Controls.Add(progressBarFinalize);
            Controls.Add(cbHaw);
            Controls.Add(cbxRegionSelect);
            Controls.Add(OverAllProcessingExcel);
            Controls.Add(LabelCurrentFileWorkingOn);
            Controls.Add(progressBar2);
            Controls.Add(progressBar1);
            Controls.Add(btn_FinalizeData);
            Controls.Add(label3);
            Controls.Add(btnSelectExcelFolder);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(btnInvalid);
            Controls.Add(btnOpenFile);
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Name = "frmMain";
            Text = "Import Test";
            Paint += FrmMain_Paint;
            ResumeLayout(false);
            PerformLayout();
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
        private System.Windows.Forms.Label lblFinalizeStatus;
    }
}

