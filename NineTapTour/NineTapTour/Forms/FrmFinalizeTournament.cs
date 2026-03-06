using NineTapTour.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace NineTapTour.Forms
{
    public partial class FrmFinalizeTournament : Form
    {
        private Tournament selectedTournament;
        private int regionID;

        public FrmFinalizeTournament(Tournament selectedTournament, int regionID)
        {
            this.selectedTournament = selectedTournament;
            this.regionID = regionID;

            InitializeComponent();
        }
    }
}
