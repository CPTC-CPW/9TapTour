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
            this.SuspendLayout();
            // 
            // btnOpenFile
            // 
            this.btnOpenFile.Location = new System.Drawing.Point(75, 75);
            this.btnOpenFile.Name = "btnOpenFile";
            this.btnOpenFile.Size = new System.Drawing.Size(97, 23);
            this.btnOpenFile.TabIndex = 0;
            this.btnOpenFile.Text = "Open File";
            this.btnOpenFile.UseVisualStyleBackColor = true;
            this.btnOpenFile.Click += new System.EventHandler(this.btnOpenFile_Click);
            // 
            // btnInvalid
            // 
            this.btnInvalid.Location = new System.Drawing.Point(75, 135);
            this.btnInvalid.Name = "btnInvalid";
            this.btnInvalid.Size = new System.Drawing.Size(97, 23);
            this.btnInvalid.TabIndex = 2;
            this.btnInvalid.Text = "View Invalid List";
            this.btnInvalid.UseVisualStyleBackColor = true;
            this.btnInvalid.Click += new System.EventHandler(this.btnInvalid_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(72, 40);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(114, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Import Member .dat file";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(251, 40);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(121, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Import Members .xls files";
            // 
            // btnSelectExcelFolder
            // 
            this.btnSelectExcelFolder.Location = new System.Drawing.Point(254, 75);
            this.btnSelectExcelFolder.Name = "btnSelectExcelFolder";
            this.btnSelectExcelFolder.Size = new System.Drawing.Size(118, 23);
            this.btnSelectExcelFolder.TabIndex = 5;
            this.btnSelectExcelFolder.Text = "Select .xls Folder";
            this.btnSelectExcelFolder.UseVisualStyleBackColor = true;
            this.btnSelectExcelFolder.Click += new System.EventHandler(this.button1_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(473, 40);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(140, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Import Tournament .pin Files";
            // 
            // btnPinFileSelect
            // 
            this.btnPinFileSelect.Location = new System.Drawing.Point(486, 75);
            this.btnPinFileSelect.Name = "btnPinFileSelect";
            this.btnPinFileSelect.Size = new System.Drawing.Size(112, 23);
            this.btnPinFileSelect.TabIndex = 7;
            this.btnPinFileSelect.Text = "Select .pin Folder";
            this.btnPinFileSelect.UseVisualStyleBackColor = true;
            this.btnPinFileSelect.Click += new System.EventHandler(this.btnPinFileSelect_Click);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(707, 235);
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
    }
}

