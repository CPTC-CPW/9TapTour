namespace NineTapTour.Database
{
    partial class PlayerHistoryForm
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
            this.lblFullName = new System.Windows.Forms.Label();
            this.lblMemberNumber = new System.Windows.Forms.Label();
            this.lblMemberSrartAvg = new System.Windows.Forms.Label();
            this.dtvPlayerHistory = new System.Windows.Forms.DataGridView();
            this._NineTapTour_NineTapDbDataSet = new NineTapTour._NineTapTour_NineTapDbDataSet();
            this.tournamentsBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.tournamentsTableAdapter = new NineTapTour._NineTapTour_NineTapDbDataSetTableAdapters.TournamentsTableAdapter();
            this.fKdboParticipantsdboTournamentsTournamentIdBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.nineTapTourNineTapDbDataSetBindingSource = new System.Windows.Forms.BindingSource(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.dtvPlayerHistory)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._NineTapTour_NineTapDbDataSet)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tournamentsBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fKdboParticipantsdboTournamentsTournamentIdBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nineTapTourNineTapDbDataSetBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // lblFullName
            // 
            this.lblFullName.AutoSize = true;
            this.lblFullName.Font = new System.Drawing.Font("Calibri", 30F, System.Drawing.FontStyle.Bold);
            this.lblFullName.Location = new System.Drawing.Point(22, 9);
            this.lblFullName.Name = "lblFullName";
            this.lblFullName.Size = new System.Drawing.Size(141, 49);
            this.lblFullName.TabIndex = 3;
            this.lblFullName.Text = "#Name";
            // 
            // lblMemberNumber
            // 
            this.lblMemberNumber.AutoSize = true;
            this.lblMemberNumber.Font = new System.Drawing.Font("Calibri", 20F, System.Drawing.FontStyle.Bold);
            this.lblMemberNumber.Location = new System.Drawing.Point(25, 68);
            this.lblMemberNumber.Name = "lblMemberNumber";
            this.lblMemberNumber.Size = new System.Drawing.Size(205, 33);
            this.lblMemberNumber.TabIndex = 4;
            this.lblMemberNumber.Text = "MemberNumber";
            // 
            // lblMemberSrartAvg
            // 
            this.lblMemberSrartAvg.AutoSize = true;
            this.lblMemberSrartAvg.Font = new System.Drawing.Font("Calibri", 20F, System.Drawing.FontStyle.Bold);
            this.lblMemberSrartAvg.Location = new System.Drawing.Point(25, 111);
            this.lblMemberSrartAvg.Name = "lblMemberSrartAvg";
            this.lblMemberSrartAvg.Size = new System.Drawing.Size(206, 33);
            this.lblMemberSrartAvg.TabIndex = 5;
            this.lblMemberSrartAvg.Text = "MemberStartavg";
            // 
            // dtvPlayerHistory
            // 
            this.dtvPlayerHistory.AllowUserToDeleteRows = false;
            this.dtvPlayerHistory.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dtvPlayerHistory.Location = new System.Drawing.Point(31, 164);
            this.dtvPlayerHistory.Name = "dtvPlayerHistory";
            this.dtvPlayerHistory.ReadOnly = true;
            this.dtvPlayerHistory.Size = new System.Drawing.Size(643, 243);
            this.dtvPlayerHistory.TabIndex = 6;
            // 
            // _NineTapTour_NineTapDbDataSet
            // 
            this._NineTapTour_NineTapDbDataSet.DataSetName = "_NineTapTour_NineTapDbDataSet";
            this._NineTapTour_NineTapDbDataSet.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // tournamentsBindingSource
            // 
            this.tournamentsBindingSource.DataMember = "Tournaments";
            this.tournamentsBindingSource.DataSource = this._NineTapTour_NineTapDbDataSet;
            // 
            // tournamentsTableAdapter
            // 
            this.tournamentsTableAdapter.ClearBeforeFill = true;
            // 
            // fKdboParticipantsdboTournamentsTournamentIdBindingSource
            // 
            this.fKdboParticipantsdboTournamentsTournamentIdBindingSource.DataMember = "FK_dbo.Participants_dbo.Tournaments_Tournament_Id";
            this.fKdboParticipantsdboTournamentsTournamentIdBindingSource.DataSource = this.tournamentsBindingSource;
            // 
            // nineTapTourNineTapDbDataSetBindingSource
            // 
            this.nineTapTourNineTapDbDataSetBindingSource.DataSource = this._NineTapTour_NineTapDbDataSet;
            this.nineTapTourNineTapDbDataSetBindingSource.Position = 0;
            // 
            // PlayerHistoryForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(709, 419);
            this.Controls.Add(this.dtvPlayerHistory);
            this.Controls.Add(this.lblMemberSrartAvg);
            this.Controls.Add(this.lblMemberNumber);
            this.Controls.Add(this.lblFullName);
            this.Name = "PlayerHistoryForm";
            this.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Text = "Player History";
            this.Load += new System.EventHandler(this.PlayerHistoryForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dtvPlayerHistory)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._NineTapTour_NineTapDbDataSet)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tournamentsBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fKdboParticipantsdboTournamentsTournamentIdBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nineTapTourNineTapDbDataSetBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblFullName;
        private System.Windows.Forms.Label lblMemberNumber;
        private System.Windows.Forms.Label lblMemberSrartAvg;
        private System.Windows.Forms.DataGridView dtvPlayerHistory;
        private _NineTapTour_NineTapDbDataSet _NineTapTour_NineTapDbDataSet;
        private System.Windows.Forms.BindingSource tournamentsBindingSource;
        private _NineTapTour_NineTapDbDataSetTableAdapters.TournamentsTableAdapter tournamentsTableAdapter;
        private System.Windows.Forms.BindingSource fKdboParticipantsdboTournamentsTournamentIdBindingSource;
        private System.Windows.Forms.BindingSource nineTapTourNineTapDbDataSetBindingSource;
    }
}