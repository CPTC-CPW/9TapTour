using NineTapTour.Database;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace NineTapTour.Forms
{
    public partial class FrmMain : Form
    {

        public List<Member> _membersList { get; set; }

        public FrmMain()
        {
            InitializeComponent();
            _membersList = MemberDb.GetMemberList();
            var newfrmStart = new MainMenu {MdiParent = this};
            //newStart.Dock = DockStyle.Fill;
            Width = newfrmStart.Width;
            Height = newfrmStart.Height + 20;
            newfrmStart.Show();
            newfrmStart.WindowState = FormWindowState.Maximized;
        }

        public void OpenOrDisplayForm<T>(ref T form) where T : Form, new()
        {
            if (form != null)
            {
                form.BringToFront();
                form.Activate();
            }
            else
            {
                form = new T
                {
                    MdiParent = this,
                    Dock = DockStyle.Fill
                };
                Width = form.Width;
                Height = form.Height + 20;
            }

            form.Show();
            form.WindowState = FormWindowState.Maximized;
        }

        public void mainMenuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var mainMenu = Application.OpenForms["MainMenu"] as MainMenu;
            OpenOrDisplayForm(ref mainMenu);
        }

        public void memberToolStripMenuItem_Click(object sender, EventArgs e)
        {   

            var newfrmMemberData = Application.OpenForms["FrmMemberData"] as FrmMemberData;

            OpenOrDisplayForm(ref newfrmMemberData);

        }

        public void tournamentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var newfrmMemberScores = Application.OpenForms["frmMemberScores"] as FrmMemberScores;

            OpenOrDisplayForm(ref newfrmMemberScores);
        }

        private void menMain_ItemAdded(object sender, ToolStripItemEventArgs e)
        {
            var s = e.Item.GetType().ToString();
            if (s == "System.Windows.Forms.MdiControlStrip+ControlBoxMenuItem")
            {
                e.Item.Visible = false;
            }

            if (e.Item.Text == "")
            {
                e.Item.Visible = false;
            }
        }
    }
}
