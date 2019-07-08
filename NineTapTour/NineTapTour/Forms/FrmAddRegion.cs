using NineTapTour.Database;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NineTapTour.Models;

namespace NineTapTour.Forms
{
    public partial class FrmAddRegion : Form
    {
        private List<NineTapRegion> nList;
        private int RegionID;
        public FrmAddRegion(int RegionID)
        {
            InitializeComponent();
            this.RegionID = RegionID;     
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
           
            if(tbEntry != null)
            {
                NineTapRegion n = new NineTapRegion();
                nList = NineTapRegionDB.GetRegionList();
                n.NineTapRegionID = nList.Count + 1;
                n.NineTapRegionName = tbEntry.Text;

                NineTapRegionDB.AddRegion(n);
                this.Close();         
            }
        }
    }
}
