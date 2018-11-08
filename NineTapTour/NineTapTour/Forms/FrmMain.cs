using NineTapTour.Database;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;
using System.Drawing;


using System.Data.Entity;
using NineTapTour.Migrations;
using NineTapTour.Models;

namespace NineTapTour.Forms
{
    public partial class FrmMain : Form
    {
        public IOrderedEnumerable<Member> _membersList { get; set; }
        public List<Tournament> _tournamentList { get; set; }
        public ToolStripMenuItem activeItem;
        public FrmMemberData currFrmMemberData { get; set; }

        public frmMemberScores currfrmScoresdata { get; set; }

        /// <summary>
        /// If this property is set to true, the application will not prompt the user to cancel a close in progress.
        /// Currently this is used to ensure the application restarts after restoring the database.
        /// </summary>
        private bool AppMustClose { get; set; }

        public MainMenu mainmenu { get; set; }
        public int RegionID { get; set; }
        public Size MaxWorkAreaScreenSize { get; set; }
        //initializes a bool var for handling if the memberdata form is active so it has proper scope for handling the save data popup showing up on the wrong forms
        bool memberDataIsActive = false;
        /// <summary>
        /// Opens Main form 
        /// Retrieves information from the database in order.
        /// </summary>

        public FrmMain()
        {
            InitializeComponent();
            //this size is the height and width of the primary screen minus the start bar (if the user has a start bar)
            MaxWorkAreaScreenSize = new Size( Screen.PrimaryScreen.WorkingArea.Width, Screen.PrimaryScreen.WorkingArea.Height );
            //run any pending database migrations on start
            NineTapDb db = new NineTapDb();
            System.Data.Entity.Database.SetInitializer<NineTapDb>(new MigrateDatabaseToLatestVersion<NineTapDb, Configuration>());

            _membersList = MemberDb.GetMemberList(RegionID).OrderBy(m => m.Number);
            _tournamentList = TournamentDb.GetTournamentList(RegionID);

            var newfrmStart = new MainMenu {MdiParent = this};
            //sets the height and width of the parent form... this can not be resized later... all child forms must 
            //fit in its bounds... the only exception is using a scrollbar on the side or bottom...
            setHeightAndWidth(MaxWorkAreaScreenSize);
            
            
            //on start up make sure regionID is set 
            var mainMenu = Application.OpenForms["MainMenu"] as MainMenu;
            OpenOrDisplayForm(ref mainMenu);
            RegionID = mainMenu.getRegionID();
            mainmenu = mainMenu;



            //sets the first item of the menu bar to the active item and highlights it.
            activeItem = (ToolStripMenuItem)menMain.Items[0];
            activeItem.BackColor = SystemColors.ActiveCaption;
            newfrmStart.Show();
            newfrmStart.WindowState = FormWindowState.Maximized;
            
        }

        /// <summary>
        ///     this methoud takes in a Size (width and height) and will set the application to that size
        ///     this size should be set to the working area of the primary monitior...
        /// </summary>
        /// <param name="workingArea">The working array of the primary monitor</param>
        private void setHeightAndWidth(Size workingArea)
        {
            this.Size = workingArea;
        }

        /// <summary>
        /// Opens/Displays the specified form. Ensures the form is on top when selected.
        /// </summary>
        /// <typeparam name="T">forms that have already been opened(?)</typeparam>
        /// <param name="form">forms that haven't been opened yet(?)</param>
        public void OpenOrDisplayForm<T>(ref T form) where T : Form, new()
        {
           
            bool isSavedData = true;

            if (isSavedData) //checks to see if you are leaving page without saved data.
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
                }
                form.WindowState = FormWindowState.Maximized;
                form.Show();
            }
            
        }

        //method to highlight menu item to show user which page they have open
        //also to disable button to current page
        private void menMain_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (activeItem != null)
            {
                activeItem.BackColor = SystemColors.Control;
            }
            activeItem = (ToolStripMenuItem)e.ClickedItem;
            if (!activeItem.HasDropDownItems)
            {
                activeItem.BackColor = SystemColors.ActiveCaption;
            }

            MenuStrip currentMenu = sender as MenuStrip;
            for (int i = 0; i < currentMenu.Items.Count; i++)
            {
                // sets enabled to true for all items in currentMenu
                // unless item is the clickedItem(activeItem)
                // or clicked item has a drop down list
                if (!activeItem.HasDropDownItems)
                {
                    if (activeItem == currentMenu.Items[i])
                    {
                        currentMenu.Items[i].Enabled = false;
                    }
                    else
                    {
                        currentMenu.Items[i].Enabled = true;
                    }
                }
            }

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
                    activeItem = (ToolStripMenuItem)menMain.Items[i];
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
            currFrmMemberData = newfrmMemberData;
            // sets bool var to true so the save data message will show up
            memberDataIsActive = true;
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
            currfrmScoresdata = newfrmMemberScores;
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
            //update the regionID
            RegionID = mainmenu.getRegionID();
            var UpdatefrmActiveMem = new FrmUpdateActiveMem(RegionID);          
            UpdatefrmActiveMem.Show();
        }

        private void BackupDatabaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();
            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                if (DatabaseManagement.BackupDatabase(folderDialog.SelectedPath))
                {
                    MessageBox.Show("Database successfully backed up!");
                }
            }
        }

        private void RestoreDatabaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Backup Files (*.bak)|*.bak";
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                if (MessageBox.Show("Restoring the database will restart the application.", "Warning", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    if (DatabaseManagement.RestoreDatabase(fileDialog.FileName))
                    {
                        MessageBox.Show("Database successfully restored from backup!");
                        AppMustClose = true;
                        Application.Restart();
                    }
                }
            }
        }

        private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Check the AppMustClose boolean to see if we need to bypass the user check.
            if (!AppMustClose)
            {
                //IF the Member Data Form has been activated and isn't null
                if (currFrmMemberData != null)
                {
                    //IF all the data on the Member Data Form IS valid
                    //Go ahead and close the application
                    if (currFrmMemberData.isValid().Count == 0)
                    {
                        currFrmMemberData.SaveMemberData();
                    }
                    //IF the data on the Member Data From is NOT Valid
                    else
                    {
                        e.Cancel = true;
                    }
                }
            }
        }
    }
}
