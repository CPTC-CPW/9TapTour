namespace NineTapTour.Forms
{
    partial class FrmTournaments
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
            this.label1 = new System.Windows.Forms.Label();
            this.dgvAllTournaments = new System.Windows.Forms.DataGridView();
            this.tournamentsBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this._NineTapTour_NineTapDbDataSet = new NineTapTour._NineTapTour_NineTapDbDataSet();
            this.tournamentsTableAdapter = new NineTapTour._NineTapTour_NineTapDbDataSetTableAdapters.TournamentsTableAdapter();
            this.btnTournamentInfo = new System.Windows.Forms.Button();
            this.cbxYearsForTournamentSearch = new System.Windows.Forms.ComboBox();
            this.lblSelectYear = new System.Windows.Forms.Label();
            this.btnYearSelectedForTourneys = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAllTournaments)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tournamentsBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._NineTapTour_NineTapDbDataSet)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(83, 45);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(265, 46);
            this.label1.TabIndex = 0;
            this.label1.Text = "Tournaments";
            // 
            // dgvAllTournaments
            // 
            this.dgvAllTournaments.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvAllTournaments.Location = new System.Drawing.Point(49, 110);
            this.dgvAllTournaments.Name = "dgvAllTournaments";
            this.dgvAllTournaments.RowTemplate.Height = 33;
            this.dgvAllTournaments.Size = new System.Drawing.Size(1253, 1020);
            this.dgvAllTournaments.TabIndex = 1;
            this.dgvAllTournaments.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvAllTournaments_CellContentClick);
            // 
            // tournamentsBindingSource
            // 
            this.tournamentsBindingSource.DataMember = "Tournaments";
            this.tournamentsBindingSource.DataSource = this._NineTapTour_NineTapDbDataSet;
            // 
            // _NineTapTour_NineTapDbDataSet
            // 
            this._NineTapTour_NineTapDbDataSet.DataSetName = "_NineTapTour_NineTapDbDataSet";
            this._NineTapTour_NineTapDbDataSet.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // tournamentsTableAdapter
            // 
            this.tournamentsTableAdapter.ClearBeforeFill = true;
            // 
            // btnTournamentInfo
            // 
            this.btnTournamentInfo.Enabled = false;
            this.btnTournamentInfo.Location = new System.Drawing.Point(139, 1184);
            this.btnTournamentInfo.Name = "btnTournamentInfo";
            this.btnTournamentInfo.Size = new System.Drawing.Size(239, 86);
            this.btnTournamentInfo.TabIndex = 2;
            this.btnTournamentInfo.Text = "Tournament Info";
            this.btnTournamentInfo.UseVisualStyleBackColor = true;
            this.btnTournamentInfo.Click += new System.EventHandler(this.btnTournamentInfo_Click);
            // 
            // cbxYearsForTournamentSearch
            // 
            this.cbxYearsForTournamentSearch.FormattingEnabled = true;
            this.cbxYearsForTournamentSearch.Location = new System.Drawing.Point(632, 45);
            this.cbxYearsForTournamentSearch.Name = "cbxYearsForTournamentSearch";
            this.cbxYearsForTournamentSearch.Size = new System.Drawing.Size(167, 33);
            this.cbxYearsForTournamentSearch.TabIndex = 3;
            // 
            // lblSelectYear
            // 
            this.lblSelectYear.AutoSize = true;
            this.lblSelectYear.Location = new System.Drawing.Point(449, 45);
            this.lblSelectYear.Name = "lblSelectYear";
            this.lblSelectYear.Size = new System.Drawing.Size(130, 25);
            this.lblSelectYear.TabIndex = 4;
            this.lblSelectYear.Text = "Select Year:";
            // 
            // btnYearSelectedForTourneys
            // 
            this.btnYearSelectedForTourneys.Location = new System.Drawing.Point(822, 26);
            this.btnYearSelectedForTourneys.Name = "btnYearSelectedForTourneys";
            this.btnYearSelectedForTourneys.Size = new System.Drawing.Size(222, 65);
            this.btnYearSelectedForTourneys.TabIndex = 5;
            this.btnYearSelectedForTourneys.Text = "Select Tournaments for Year selected";
            this.btnYearSelectedForTourneys.UseVisualStyleBackColor = true;
            this.btnYearSelectedForTourneys.Click += new System.EventHandler(this.btnYearSelectedForTourneys_Click);
            // 
            // FrmTournaments
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1779, 1344);
            this.Controls.Add(this.btnYearSelectedForTourneys);
            this.Controls.Add(this.lblSelectYear);
            this.Controls.Add(this.cbxYearsForTournamentSearch);
            this.Controls.Add(this.btnTournamentInfo);
            this.Controls.Add(this.dgvAllTournaments);
            this.Controls.Add(this.label1);
            this.Name = "FrmTournaments";
            this.Text = "List of Tournaments";
            this.Load += new System.EventHandler(this.FrmTournamentStats_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvAllTournaments)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tournamentsBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._NineTapTour_NineTapDbDataSet)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView dgvAllTournaments;
        private _NineTapTour_NineTapDbDataSet _NineTapTour_NineTapDbDataSet;
        private System.Windows.Forms.BindingSource tournamentsBindingSource;
        private _NineTapTour_NineTapDbDataSetTableAdapters.TournamentsTableAdapter tournamentsTableAdapter;
        private System.Windows.Forms.Button btnTournamentInfo;
        private System.Windows.Forms.ComboBox cbxYearsForTournamentSearch;
        private System.Windows.Forms.Label lblSelectYear;
        private System.Windows.Forms.Button btnYearSelectedForTourneys;
    }
}