using NineTapTour.Database;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;

namespace NineTapTour.Forms
{
    public partial class FrmMain : Form
    {

        public IOrderedEnumerable<Member> _membersList { get; set; }
        public List<Tournament> _tournamentList { get; set; }

        public FrmMain()
        {
            InitializeComponent();
            _membersList = MemberDb.GetMemberList().OrderBy(m => m.Number);
            _tournamentList = TournamentDb.GetTournamentList();
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
                Width = form.Right + Math.Abs(form.Left) + 4;
                Height = form.Height + 28;
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
            form.WindowState = FormWindowState.Maximized;
            form.Show();
           
        }

        public void OpenOrDisplayTourneyForm<T>(ref T form) where T : Form, new()
        {
            if (form != null)
            {
                Width = form.Right + Math.Abs(form.Left) + 4;
                Height = form.Height + 28;
                form.BringToFront();
                form.Activate();
            }
            else
            {
                form = new T
                {
                    Dock = DockStyle.Fill
                };
                Width = form.Width;
                Height = form.Height + 20;
            }
            //form.WindowState = FormWindowState.Maximized;
            form.Show();

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
            var newfrmMemberScores = Application.OpenForms["frmMemberScores"] as frmMemberScores;

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
