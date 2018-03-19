using NineTapTour.Database;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;
using System.Drawing;


using System.Data.Entity;
using NineTapTour.Migrations;

namespace NineTapTour.Forms
{
    public partial class FrmMain : Form
    {

       
        public IOrderedEnumerable<Member> _membersList { get; set; }
        public List<Tournament> _tournamentList { get; set; }
        public ToolStripItem activeItem;
        public FrmMemberData currFrmMemberData { get; set; }

        public frmMemberScores currfrmScoresdata { get; set; }

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
            activeItem = menMain.Items[0];
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
            currfrmScoresdata.UpdateTourneyComboBox();
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

        /// <summary>
        /// This method calls a FrmMemberData method to determine if data is saved.
        /// if data not saved prompts user whether they still want to leave without saving data.
        /// </summary>
        /// <returns>returns true if data is saved or if data is not saved and user wants to continue without
        /// saving changes. Returns false if data is not saved and user does want to save changes.</returns>
        private bool FrmMemberIsSavedData()
        {
            if (currFrmMemberData.IsSavedData())
            {
                return true;
            }

            else
            {
                if (memberDataIsActive == true)
                {
                    var confirm = MessageBox.Show(@"Are you sure you want to leave without saving changes?", @"Member Data Not Saved", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (confirm == DialogResult.No)
                    {

                        return false;

                    }
                    else
                    {
                        //prevents the message box from showing up when member data form is not active
                        memberDataIsActive = false;
                        return true;
                        
                    }
                }
                else {
                    return true;
                }
            }
        }


    }
}
