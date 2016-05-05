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
            this.idDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dateDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.locationDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.eventDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.notesDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.sponsorsDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tournamentsBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this._NineTapTour_NineTapDbDataSet = new NineTapTour._NineTapTour_NineTapDbDataSet();
            this.tournamentsTableAdapter = new NineTapTour._NineTapTour_NineTapDbDataSetTableAdapters.TournamentsTableAdapter();
            this.btnTournamentInfo = new System.Windows.Forms.Button();
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
            this.dgvAllTournaments.AutoGenerateColumns = false;
            this.dgvAllTournaments.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvAllTournaments.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.idDataGridViewTextBoxColumn,
            this.dateDataGridViewTextBoxColumn,
            this.locationDataGridViewTextBoxColumn,
            this.eventDataGridViewTextBoxColumn,
            this.notesDataGridViewTextBoxColumn,
            this.sponsorsDataGridViewTextBoxColumn});
            this.dgvAllTournaments.DataSource = this.tournamentsBindingSource;
            this.dgvAllTournaments.Location = new System.Drawing.Point(49, 110);
            this.dgvAllTournaments.Name = "dgvAllTournaments";
            this.dgvAllTournaments.RowTemplate.Height = 33;
            this.dgvAllTournaments.Size = new System.Drawing.Size(1580, 1020);
            this.dgvAllTournaments.TabIndex = 1;
            this.dgvAllTournaments.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvAllTournaments_CellContentClick);
            // 
            // idDataGridViewTextBoxColumn
            // 
            this.idDataGridViewTextBoxColumn.DataPropertyName = "Id";
            this.idDataGridViewTextBoxColumn.HeaderText = "Id";
            this.idDataGridViewTextBoxColumn.Name = "idDataGridViewTextBoxColumn";
            this.idDataGridViewTextBoxColumn.ReadOnly = true;
            this.idDataGridViewTextBoxColumn.Width = 80;
            // 
            // dateDataGridViewTextBoxColumn
            // 
            this.dateDataGridViewTextBoxColumn.DataPropertyName = "Date";
            this.dateDataGridViewTextBoxColumn.HeaderText = "Date";
            this.dateDataGridViewTextBoxColumn.Name = "dateDataGridViewTextBoxColumn";
            this.dateDataGridViewTextBoxColumn.Width = 170;
            // 
            // locationDataGridViewTextBoxColumn
            // 
            this.locationDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.locationDataGridViewTextBoxColumn.DataPropertyName = "Location";
            this.locationDataGridViewTextBoxColumn.HeaderText = "Location";
            this.locationDataGridViewTextBoxColumn.Name = "locationDataGridViewTextBoxColumn";
            // 
            // eventDataGridViewTextBoxColumn
            // 
            this.eventDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.eventDataGridViewTextBoxColumn.DataPropertyName = "Event";
            this.eventDataGridViewTextBoxColumn.HeaderText = "Event";
            this.eventDataGridViewTextBoxColumn.Name = "eventDataGridViewTextBoxColumn";
            // 
            // notesDataGridViewTextBoxColumn
            // 
            this.notesDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.notesDataGridViewTextBoxColumn.DataPropertyName = "Notes";
            this.notesDataGridViewTextBoxColumn.HeaderText = "Notes";
            this.notesDataGridViewTextBoxColumn.Name = "notesDataGridViewTextBoxColumn";
            // 
            // sponsorsDataGridViewTextBoxColumn
            // 
            this.sponsorsDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.sponsorsDataGridViewTextBoxColumn.DataPropertyName = "Sponsors";
            this.sponsorsDataGridViewTextBoxColumn.HeaderText = "Sponsors";
            this.sponsorsDataGridViewTextBoxColumn.Name = "sponsorsDataGridViewTextBoxColumn";
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
            // 
            // FrmTournaments
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1779, 1344);
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
        private System.Windows.Forms.DataGridViewTextBoxColumn idDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn dateDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn locationDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn eventDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn notesDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn sponsorsDataGridViewTextBoxColumn;
        private System.Windows.Forms.Button btnTournamentInfo;
    }
}