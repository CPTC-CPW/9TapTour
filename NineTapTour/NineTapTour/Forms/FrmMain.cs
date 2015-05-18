using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NineTapTour.Forms
{
    public partial class FrmMain : Form
    {
        public FrmMain()
        {
            InitializeComponent();
            FrmStart newfrmStart = Application.OpenForms["frmStart"] as FrmStart;
            newfrmStart = new FrmStart();
            newfrmStart.MdiParent = this;
            //newStart.Dock = DockStyle.Fill;
            this.Width = newfrmStart.Width;
            this.Height = newfrmStart.Height + 20;
            newfrmStart.Show();
            newfrmStart.WindowState = FormWindowState.Maximized;
        }
        private void mainMenuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmStart newfrmStart = Application.OpenForms["frmStart"] as FrmStart;

            if (newfrmStart != null)
            {
                newfrmStart.WindowState = FormWindowState.Maximized;
                newfrmStart.BringToFront();
                newfrmStart.Activate();
            }
            else
            {
                newfrmStart = new FrmStart();
                newfrmStart.MdiParent = this;
                //newStart.Dock = DockStyle.Fill;
                this.Width = newfrmStart.Width;
                this.Height = newfrmStart.Height + 20;
                newfrmStart.Show();
                newfrmStart.WindowState = FormWindowState.Maximized;
            }
        }
        private void memberToolStripMenuItem_Click(object sender, EventArgs e)
        {   

            FrmMemberData newfrmMemberData = Application.OpenForms["FrmMemberData"] as FrmMemberData;
            
            if(newfrmMemberData != null)
            {
                newfrmMemberData.WindowState = FormWindowState.Maximized;
                newfrmMemberData.BringToFront();
                newfrmMemberData.Activate();
            }
            else
            {
                newfrmMemberData = new FrmMemberData();
                newfrmMemberData.MdiParent = this;
                //newfrmMemberData.Dock = DockStyle.Fill;
                this.Width = newfrmMemberData.Width;
                this.Height = newfrmMemberData.Height + 20;
                newfrmMemberData.Show();
                newfrmMemberData.WindowState = FormWindowState.Maximized;
            }
            
        }

        private void tournamentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmMemberScores newfrmMemberScores = Application.OpenForms["frmMemberScores"] as FrmMemberScores;
            
            if(newfrmMemberScores != null)
            {
                newfrmMemberScores.WindowState = FormWindowState.Maximized;
                newfrmMemberScores.BringToFront();
                newfrmMemberScores.Activate();
            }
            else
            {
                newfrmMemberScores = new FrmMemberScores();
                newfrmMemberScores.MdiParent = this;
                //newfrmMemberScores.Dock = DockStyle.Fill;
                this.Width = newfrmMemberScores.Width;
                this.Height = newfrmMemberScores.Height + 20;
                newfrmMemberScores.Show();
                newfrmMemberScores.WindowState = FormWindowState.Maximized;
            }
        }
    }
}
