using NineTapTour.Database;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;
using System.Drawing;

using NineTapTour.Migrations;
using System.Data.Entity;

namespace NineTapTour.Forms
{
    public partial class FrmMain : Form
    {

        public IOrderedEnumerable<Member> _membersList { get; set; }
        public List<Tournament> _tournamentList { get; set; }
        public ToolStripItem activeItem;
        public FrmMemberData currFrmMemberData { get; set; }

        /// <summary>
        /// Opens Main form 
        /// Retrieves information from the database in order.
        /// </summary>

        public FrmMain()
        {
            InitializeComponent();

            //run any pending database migrations on start
            NineTapDb db = new NineTapDb();
            System.Data.Entity.Database.SetInitializer<NineTapDb>(new MigrateDatabaseToLatestVersion<NineTapDb, Configuration>());

            _membersList = MemberDb.GetMemberList().OrderBy(m => m.Number);
            _tournamentList = TournamentDb.GetTournamentList();
            var newfrmStart = new MainMenu {MdiParent = this};
            //newStart.Dock = DockStyle.Fill;
            //Width and Height not needed here?
            //Width = newfrmStart.Width;
            //Height = newfrmStart.Height + 20;
            //sets the first item of the menu bar to the active item and highlights it.
            activeItem = menMain.Items[0];
            activeItem.BackColor = SystemColors.ActiveCaption;
            newfrmStart.Show();
            newfrmStart.WindowState = FormWindowState.Maximized;
        }

        /// <summary>
        /// Opens/Displays the specified form. Ensures the form is on top when selected.
        /// </summary>
        /// <typeparam name="T">forms that have already been opened(?)</typeparam>
        /// <param name="form">forms that haven't been opened yet(?)</param>
        public void OpenOrDisplayForm<T>(ref T form) where T : Form, new()
        {
            bool isSavedData = true;

            // determines whether FrmMemberData is saved before leaving and if calls 
            // FrmMemberIsSavedData to determine you want to leave without saving changes
            // else return you to FrmMemberData.
            if (currFrmMemberData != null)
            {
                if (!FrmMemberIsSavedData())
                {
                    isSavedData = false;
                    currFrmMemberData.BringToFront();
                    currFrmMemberData.Activate();
                    menuHighlight("Member Info");
                }
                else
                {
                    currFrmMemberData.UpdateMemberInfo();
                }
            }

            if (isSavedData) //checks to see if you are leaving page without saved data.
            {
                if (form != null)
                {
                    //Width and Height not needed here?
                    //Width = form.Right + Math.Abs(form.Left) + 4;
                    //Height = form.Height + 28;
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
                    //Width and Height not needed here?
                    //Width = form.Width;
                    //Height = form.Height + 20;
                }
                form.WindowState = FormWindowState.Maximized;
                form.Show();
            }
            
        }

        //method to highlight menu item to show user which page they have open
        private void menMain_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if(activeItem != null)
            {
                activeItem.BackColor = SystemColors.Control;
            }
            activeItem = e.ClickedItem;

            activeItem.BackColor = SystemColors.ActiveCaption;
        }
        //this method is for the buttons on the main form
        public void menuHighlight(string itemName)
        {
            if (activeItem != null)
            {
                activeItem.BackColor = SystemColors.Control;
            }
            for(int i = 0; i <= menMain.Items.Count; i++)
            {
                if (itemName == menMain.Items[i].Text)
                {
                    activeItem = menMain.Items[i];
                    break;
                }
            }
            activeItem.BackColor = SystemColors.ActiveCaption;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void mainMenuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var mainMenu = Application.OpenForms["MainMenu"] as MainMenu;
            OpenOrDisplayForm(ref mainMenu);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void memberToolStripMenuItem_Click(object sender, EventArgs e)
        {   
            var newfrmMemberData = Application.OpenForms["FrmMemberData"] as FrmMemberData;
            OpenOrDisplayForm(ref newfrmMemberData);

            //currFrmMemberData = newfrmMemberData;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void tournamentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var newfrmMemberScores = Application.OpenForms["frmMemberScores"] as frmMemberScores;
            OpenOrDisplayForm(ref newfrmMemberScores);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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


        private void updateInactiveMembersToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            var UpdatefrmActiveMem = new FrmUpdateActiveMem();
            Width = UpdatefrmActiveMem.Width;
            Height = UpdatefrmActiveMem.Height + 20;
            UpdatefrmActiveMem.Show();
        }

        /// <summary>
        /// This Method call a FrmMemberData method to determine if data is saved.
        /// if data not saved prompts user whether they still want to leave without saving data.
        /// </summary>
        /// <returns>return true if data is saved or if data is saved and user wants to delete any
        /// saved changes. Returns false if data is not saved and user does want to make changes.</returns>
        private bool FrmMemberIsSavedData() {
            if (currFrmMemberData.IsSavedData())
            {
                return true;
            }

            else
            {
                var confirm = MessageBox.Show(@"Are you sure you want to leave without saving changes?", @"Member Data Not Saved", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (confirm == DialogResult.No)
                {

                    return false;

                }
                else
                {
                    return true;
                }
            }
            
        }
    }
}
