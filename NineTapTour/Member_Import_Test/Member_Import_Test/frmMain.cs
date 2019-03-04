using Member_Import_Test.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;
using NineTapTour.Database;
using System.Text.RegularExpressions;
using System.Globalization;
using NineTapTour.Forms;
using NineTapTour.Models;

namespace Member_Import_Test
{
    public partial class frmMain : Form
    {

        public frmMain()
        {
            InitializeComponent();
            List<NineTapRegion> r = NineTapRegionDB.GetRegionList();
            cbxRegionSelect.DataSource = r;
            cbxRegionSelect.DisplayMember = "NineTapRegionName";
            RegionID = r[cbxRegionSelect.SelectedIndex].NineTapRegionID;
            
        }
        #region Member Info Static Ints
        //MEMBER INFO STATIC INTS
        static int MemNumSpace = 6;     // Member Number
        static int DJoinedSpace = 8;    // Date Joined
        static int LNameSpace = 20;     // Last Name
        static int FNameSpace = 20;     // First Name
        static int MISpace = 2;         // Middle Initial
        static int EPhoneSpace = 15;    // Evening Phone
        static int DPhoneSpace = 15;    // Day Phone
        static int CPhoneSpace = 15;    // Cell Phone
        static int StreetSpace = 40;    // Street Address
        static int EmailSpace = 40;     // Email Address
        static int CitySpace = 20;      // City
        static int StateSpace = 2;      // State
        static int ZipSpace = 10;       // Zip
        static int NotesSpace = 200;    // Notes
        static int AVGSpace = 3;        // Average
        static int HCSpace = 2;         // Handicap
        static int BSpace = 2;          // Bonus
        static int LastBSpace = 8;      // Last Bowled
        static int YearEndTSpace = 2;   // Year End Tournaments
        static int MoneyESpace = 10;    // Money Earned
        static int RejoinDSpace = 8;    // Rejoin Date;
        static int ReferalSpace = 2;    // ReferalSpace;
        static int SSSpace = 11;        // Social Security
        static int CBSpace = 5;         // Check Box Spaceing, there are 7 total, only 5 are actually checked for information, repeated 7 times in Spaces array.
        static int DOBSpace = 8;        // Date Of Birth.
        #endregion
        #region PinFile Static Ints
        // PIN FILE STATIC INTS
        static int PinFileMemNumSpace = 6;
        static int PinFileLastName = 20;
        static int PinFileFirstName = 20;
        static int PinFileMiddleName = 2;
        static int PinFileScratchScore1 = 3;
        static int PinFileScratchScore2 = 3;
        static int PinFileScratchScore3 = 3;
        static int PinFileScratchScore4 = 3;
        static int PinFileScratchScoreTotal = 4;
        static int PinFileHandicapScore1 = 3;
        static int PinFileHandicapScore2 = 3;
        static int PinFileHandicapScore3 = 3;
        static int PinFileHandicapScore4 = 3;
        static int PinFileHandicapScoreTotal = 4;
        static int PinFileNotes = 207; //notes + spaces to skip to get to the 0's ans 1's that control the squads
        static int morsecodeslot = 5; // all the 0 and 1s at the end of a players pin record. these series of 0s and 1s indicate their active or inactive status, male or female, senior. and bowling squad.
        #endregion

        public int RegionID;
        public int allGames;

        public List<Member> validMembers = new List<Member>();      // Makes list of valid members
        public List<Member> invalidMembers = new List<Member>();    // Makes list of invalid members
        public List<PlayerHistory> PlayerHistoryList = new List<PlayerHistory>();

        // Create array of spaces
        int[] Spaces = new int[] { MemNumSpace, DJoinedSpace, LNameSpace, FNameSpace, MISpace, EPhoneSpace, DPhoneSpace, CPhoneSpace,
                                   StreetSpace, EmailSpace, CitySpace, StateSpace, ZipSpace, NotesSpace, AVGSpace, HCSpace, BSpace,
                                   LastBSpace, YearEndTSpace, MoneyESpace, RejoinDSpace, ReferalSpace, SSSpace, CBSpace, CBSpace, CBSpace, CBSpace, CBSpace, CBSpace, CBSpace, DOBSpace};

        int[] PinSpaces = new int[] {PinFileMemNumSpace, PinFileLastName, PinFileFirstName, PinFileMiddleName, PinFileScratchScore1, PinFileScratchScore2, PinFileScratchScore3, PinFileScratchScore4 , PinFileScratchScoreTotal,
                                     PinFileHandicapScore1, PinFileHandicapScore2, PinFileHandicapScore3, PinFileHandicapScore4, PinFileHandicapScoreTotal , PinFileNotes, morsecodeslot, morsecodeslot, morsecodeslot, morsecodeslot, morsecodeslot, morsecodeslot
                                      ,morsecodeslot, morsecodeslot, morsecodeslot , morsecodeslot, morsecodeslot, morsecodeslot, morsecodeslot, morsecodeslot, morsecodeslot, morsecodeslot, morsecodeslot};

        private static List<ExcelRow> ALLEXCELDATAFROMALLPLAYERS = new List<ExcelRow>();
        private static List<Tournament> TournamentList = new List<Tournament>();
        private static List<Game> GameImport = new List<Game>();

        private void btnOpenFile_Click(object sender, EventArgs e)
        {
            //Filter to limit the types of files that can be opened with the file open dialog
            ofdOpen.Filter = "Data Files (*.dat)|*.dat|Text Files (*.txt)|*.txt";
            ofdOpen.Title = "Please Select a member file to open";
            if (ofdOpen.ShowDialog() == DialogResult.OK)
            {
                System.IO.StreamReader sr = new System.IO.StreamReader(ofdOpen.FileName);
                //MessageBox.Show(sr.ReadToEnd()); //for debug purpose
                String File = sr.ReadToEnd(); // It's easier to read into a string and work with the file rather than a streamreader, which has no direct position "index" access.
                sr.Close();
                Member newMem = new Member(); // Might not need this here, may move it.
                int currentIndex = 0;         // Starting index
                //List<String> memberInfo = new List<String>(); //for testing
                int i;                        // Needs to be declared outside for to be used for switch
                int validCount = 0;           // Count of valid members added
                int invalidCount = 0;         // Count of invalid members added
                int MemberCount = 1;          // Number of current member

                while (currentIndex >= 0)
                {
                    do  // A do while to substring from the main string
                    {
                        bool validMember = true; // to determin if goes on seperate list
                        bool genderSelected = false; //check if gender has been selected 
                        bool status = false; // check if status has been selected
                        newMem.NineTapRegionID = RegionID; //sets the region id to the current selected region
                        currentIndex = File.IndexOf(Convert.ToString(MemberCount), currentIndex);
                        if (currentIndex == -1)
                        {
                            break;
                        }
                        for (i = 0; i < Spaces.Length; i++)
                        {
                            switch (i)
                            {
                                case 0: // Member Number
                                    if (!String.IsNullOrWhiteSpace(File.Substring(currentIndex, Spaces[i]).Trim()))
                                    {
                                        newMem.Number = Convert.ToInt32(File.Substring(currentIndex, Spaces[i]).Trim());
                                    }
                                    break;
                                case 1: // Date Joined
                                    if (!String.IsNullOrWhiteSpace(File.Substring(currentIndex, Spaces[i]).Trim()))
                                    {
                                        try
                                        {
                                            newMem.JoinDate = Convert.ToDateTime(File.Substring(currentIndex, Spaces[i]).Trim());
                                        }
                                        catch
                                        {
                                            newMem.JoinDate = DateTime.Today;
                                        }
                                    }
                                    break;
                                case 2: // Last Name
                                    if (!String.IsNullOrWhiteSpace(File.Substring(currentIndex, Spaces[i]).Trim()))
                                    {
                                        newMem.LastName = (File.Substring(currentIndex, Spaces[i]).Trim());
                                    }
                                    else
                                    {
                                        //validMember = false;
                                    }

                                    break;
                                case 3: // First Name
                                    if (!String.IsNullOrWhiteSpace(File.Substring(currentIndex, Spaces[i]).Trim()))
                                    {
                                        string fName = File.Substring(currentIndex, Spaces[i]).Trim();
                                        string[] split = fName.Split(' ');
                                        newMem.FirstName = split[0];
                                    }
                                    else
                                    {
                                        ////validMember = false;
                                    }
                                    break;
                                case 4: // Middle Initial
                                    newMem.MiddleInitial = (File.Substring(currentIndex, Spaces[i]).Trim());
                                    break;
                                case 5: // Primary Phone
                                    if (!String.IsNullOrWhiteSpace(File.Substring(currentIndex, Spaces[i]).Trim()))
                                    {
                                        newMem.PrimaryPhone = (File.Substring(currentIndex, Spaces[i]).Trim());
                                    }
                                    else
                                    {
                                        ////validMember = false;
                                    }
                                    break;
                                //case 6://Secondary Phone
                                //    newMem.SecondaryPhone = (File.Substring(currentIndex, Spaces[i]).Trim());
                                //    break;
                                case 7: // Cell Phone
                                    newMem.SecondaryPhone = (File.Substring(currentIndex, Spaces[i]).Trim());
                                    break;
                                case 8: // Street Address
                                    if (!String.IsNullOrWhiteSpace(File.Substring(currentIndex, Spaces[i]).Trim()))
                                    {
                                        newMem.Street = (File.Substring(currentIndex, Spaces[i]).Trim());
                                    }
                                    else
                                    {
                                        //validMember = false;
                                    }
                                    break;
                                case 9: // Email Address
                                    if (!String.IsNullOrWhiteSpace(File.Substring(currentIndex, Spaces[i]).Trim()))
                                    {
                                        newMem.Email = (File.Substring(currentIndex, Spaces[i]).Trim());
                                    }
                                    else
                                    {
                                        //validMember = false;
                                    }
                                    break;
                                case 10: // City
                                    if (!String.IsNullOrWhiteSpace(File.Substring(currentIndex, Spaces[i]).Trim()))
                                    {
                                        newMem.City = (File.Substring(currentIndex, Spaces[i]).Trim());
                                    }
                                    else
                                    {
                                        //validMember = false;
                                    }
                                    break;
                                case 11: // State
                                    if (!String.IsNullOrWhiteSpace(File.Substring(currentIndex, Spaces[i]).Trim()))
                                    {
                                        newMem.State = (File.Substring(currentIndex, Spaces[i]).Trim());
                                    }
                                    else
                                    {
                                        //validMember = false;
                                    }
                                    break;
                                case 12: // Zip
                                    if (!String.IsNullOrWhiteSpace(File.Substring(currentIndex, Spaces[i]).Trim()))
                                    {
                                        newMem.PostalCode = (File.Substring(currentIndex, Spaces[i]).Trim());
                                    }
                                    else
                                    {
                                        //validMember = false;
                                    }
                                    break;
                                case 13: // Notes
                                    newMem.Notes = (File.Substring(currentIndex, Spaces[i]).Trim());
                                    break;
                                case 14: // Average
                                    if (!String.IsNullOrWhiteSpace(File.Substring(currentIndex, Spaces[i]).Trim()))
                                    {
                                        newMem.StartAvg = Convert.ToInt32((File.Substring(currentIndex, Spaces[i]).Trim()));
                                        newMem.Average = newMem.StartAvg;
                                    }
                                    break;
                                case 15: // Handicap
                                    if (!String.IsNullOrWhiteSpace(File.Substring(currentIndex, Spaces[i]).Trim()))
                                    {
                                        newMem.Handicap = Convert.ToInt32((File.Substring(currentIndex, Spaces[i]).Trim()));
                                    }
                                    break;
                                case 16: // Bonus
                                    if (!String.IsNullOrWhiteSpace(File.Substring(currentIndex, Spaces[i]).Trim()))
                                    {
                                        newMem.Bonus = Convert.ToInt32((File.Substring(currentIndex, Spaces[i]).Trim()));
                                    }
                                    break;
                                case 17: // Date Last Bowled
                                    if (!String.IsNullOrWhiteSpace(File.Substring(currentIndex, Spaces[i]).Trim()))
                                    {
                                        newMem.LastBowled = Convert.ToDateTime((File.Substring(currentIndex, Spaces[i]).Trim()));
                                    }
                                    break;
                                /*case 18:
                                    This is the year end tournaments which currently are not stored/not being used*/
                                case 19: // Money Earned
                                    if (!String.IsNullOrWhiteSpace(File.Substring(currentIndex, Spaces[i]).Trim()))
                                    {
                                        newMem.MoneyEarned = Convert.ToDecimal((File.Substring(currentIndex, Spaces[i]).Trim()));
                                    }
                                    break;
                                case 20: // Rejoin Date
                                    if (!String.IsNullOrWhiteSpace(File.Substring(currentIndex, Spaces[i]).Trim()))
                                    {
                                        try
                                        {
                                            newMem.RejoinDate = Convert.ToDateTime((File.Substring(currentIndex, Spaces[i]).Trim()));
                                        }
                                        catch
                                        {
                                            newMem.RejoinDate = null;
                                        }
                                    }
                                    break;
                                case 21: // Referrals
                                    if (!String.IsNullOrWhiteSpace(File.Substring(currentIndex, Spaces[i]).Trim()))
                                    {
                                        Console.WriteLine(File.Substring(currentIndex, Spaces[i]).Trim());
                                        string str = File.Substring(currentIndex, Spaces[i]).Trim();
                                        int num;
                                        bool isNum = int.TryParse(str, out num);
                                        if (isNum)
                                        {
                                            newMem.Referrals = Convert.ToInt16(File.Substring(currentIndex, Spaces[i]).Trim());
                                        }
                                        else
                                        {
                                            //newMem.Referrals = Convert.ToInt16(File.Substring(currentIndex, Spaces[i]).Trim());
                                            //validMember = false;
                                        }

                                    }
                                    else
                                    {
                                        newMem.Referrals = null;
                                    }
                                    break;
                                case 22: // Social Security Number
                                    if (!String.IsNullOrWhiteSpace(File.Substring(currentIndex, Spaces[i]).Trim()))
                                    {
                                        newMem.SSN = File.Substring(currentIndex, Spaces[i]).Trim();
                                    }
                                    else
                                    {
                                        //validMember = false;
                                    }
                                    break;
                                /*case 23:
                                    Unused member for life checkbox*/
                                case 24: // Active Member
                                    if (!String.IsNullOrWhiteSpace(File.Substring(currentIndex, Spaces[i]).Trim()))
                                    {
                                        if (Convert.ToInt32(File.Substring(currentIndex, Spaces[i]).Trim()) == 1)
                                        {
                                            newMem.IsActive = true;
                                            status = true;
                                        }
                                    }
                                    break;
                                /*case 25:
                                    Unused pre paid checkbox from original form*/
                                case 26: // Inactive Member
                                    if (!String.IsNullOrWhiteSpace(File.Substring(currentIndex, Spaces[i]).Trim()))
                                    {
                                        if (status && Convert.ToInt32(File.Substring(currentIndex, Spaces[i]).Trim()) == 1)
                                        {
                                            //validMember = false;
                                            break;
                                        }
                                        if (Convert.ToInt32(File.Substring(currentIndex, Spaces[i]).Trim()) == 1)
                                        {
                                            newMem.IsActive = false;
                                        }
                                    }
                                    break;
                                case 27: // Senior
                                    if (!String.IsNullOrWhiteSpace(File.Substring(currentIndex, Spaces[i]).Trim()))
                                    {
                                        if (Convert.ToInt32(File.Substring(currentIndex, Spaces[i]).Trim()) == 1)
                                        {
                                            newMem.IsSenior = true;
                                        }
                                    }
                                    break;
                                case 28: // Gender Female
                                    if (!String.IsNullOrWhiteSpace(File.Substring(currentIndex, Spaces[i]).Trim()))
                                    {
                                        if (Convert.ToInt32(File.Substring(currentIndex, Spaces[i]).Trim()) == 1)
                                        {
                                            newMem.Gender = MemberGenders.Female;
                                            genderSelected = true;
                                        }
                                    }
                                    break;
                                case 29: // Gender Male
                                    if (!String.IsNullOrWhiteSpace(File.Substring(currentIndex, Spaces[i]).Trim()))
                                    {
                                        if (genderSelected && Convert.ToInt32(File.Substring(currentIndex, Spaces[i]).Trim()) == 1)
                                        {
                                            //validMember = false;
                                            break;
                                        }
                                        if (Convert.ToInt32(File.Substring(currentIndex, Spaces[i]).Trim()) == 1)
                                        {
                                            newMem.Gender = MemberGenders.Male;
                                        }
                                    }
                                    break;
                                case 30: // Birth Date
                                    if (File.Length - currentIndex < 8 || !String.IsNullOrWhiteSpace(File.Substring(currentIndex, Spaces[i]).Trim()))
                                    {
                                        if (File.Length - currentIndex < 8)
                                        {
                                            Console.WriteLine(File.Substring(currentIndex));
                                            newMem.DateOfBirth = Convert.ToDateTime(File.Substring(currentIndex));
                                        }
                                        else if (File.Length - currentIndex > 8)
                                        {
                                            Console.WriteLine(File.Substring(currentIndex, Spaces[i]).Trim());
                                            try
                                            {
                                                newMem.DateOfBirth = Convert.ToDateTime(File.Substring(currentIndex, Spaces[i]).Trim());
                                            }
                                            catch (Exception)
                                            {
                                                newMem.DateOfBirth = DateTime.Today;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        //validMember = false;
                                    }
                                    break;
                            }
                            currentIndex += Spaces[i];
                        }
                        if (validMember)
                        {
                            // Valid count to show user at end
                            validCount++;
                            // Add good members to valid list to add to database once done reading file
                            validMembers.Add(newMem);
                        }
                        else
                        {
                            // Invalid count to show the user at the end
                            invalidCount++;
                            // Add invalid members to invalid list to be edited by user before adding to the database.
                            invalidMembers.Add(newMem);
                        }
                        MemberCount++;
                        newMem = new Member();
                    } while (currentIndex <= File.Length);

                    // Go through the members on the valid list and add them to the database
                    for (int j = 0; j < validMembers.Count; j++)
                    {
                        // Only add the member after checking if the memeber isn't already in the database.
                        if (!NineTapTour.Database.MemberDb.MemberExists(validMembers[j]))
                        {
                            if (validMembers[j].DateOfBirth < Convert.ToDateTime("1 / 1 / 1753 12:00:00 AM"))
                            {
                                validMembers[j].DateOfBirth = Convert.ToDateTime("1 / 1 / 1753 12:00:00 AM");
                            }
                            if (validMembers[j].JoinDate < Convert.ToDateTime("1 / 1 / 1753 12:00:00 AM"))
                            {
                                validMembers[j].JoinDate = Convert.ToDateTime("1 / 1 / 1753 12:00:00 AM");
                            }
                            if (validMembers[j].RejoinDate < Convert.ToDateTime("1 / 1 / 1753 12:00:00 AM"))
                            {
                                validMembers[j].RejoinDate = Convert.ToDateTime("1 / 1 / 1753 12:00:00 AM");
                            }
                            NineTapTour.Database.MemberDb.AddOrUpdateMember(validMembers[j]);
                        }
                    }
                    // Show the results to the user
                    MessageBox.Show(validCount + " valid members processed, " + invalidCount + " invalid members processed.", "Results", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    checkSpaces();
                }
            }
        }

        private void checkSpaces()
        {
            if (invalidMembers.Count > 0 || validMembers.Count > 0)
            {
                btnSelectExcelFolder.Enabled = true;
            }
            if (ALLEXCELDATAFROMALLPLAYERS.Count > 0)
            {
                btn_FinalizeData.Enabled = true;
            }
        }

        private void btnInvalid_Click(object sender, EventArgs e)
        {
            if (invalidMembers.Count <= 0)
            {
                MessageBox.Show("No invalid members processed.");
            }
            else
            {
                // Open the memberdata copied from main project in order to edit the invalid user information
                var md = new FrmMemberData(invalidMembers, this);
                md.Show();
                Hide();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            GetAndProcessFolderWithExcelFiles();
            while (MessageBox.Show("Do You have more Excel Files to import?", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                GetAndProcessFolderWithExcelFiles();
            }

            // Ensure second progress bar is filled to show completion
            progressBar2.Value = progressBar2.Maximum;
            LabelCurrentFileWorkingOn.Text = "Complete";
        }

        private void GetAndProcessFolderWithExcelFiles()
        {
            using (var fbd = new FolderBrowserDialog())
            {
                allGames = PlayerHistoryDB.getNumberOfAllGames();
                DialogResult result = fbd.ShowDialog();
                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    string[] files = Directory.GetFiles(fbd.SelectedPath);
                    progressBar1.Minimum = 0;
                    progressBar1.Maximum = files.Length;
                    progressBar1.Value = 0;
                    GetAllExcelData(files);
                }
            }
            checkSpaces();
        }

        private List<ExcelRow> GetAllExcelData(string[] files)
        {
            OverAllProcessingExcel.Text = "Over All Process:";
            for (int i = 0; i < files.Length; i++)
            {
                if (Path.GetExtension(files[i]) != ".xls")
                {
                    continue;
                }
                
                List<ExcelRow> rows = ProcessExcelFile(files[i]);
                foreach (ExcelRow r in rows)
                {
                    ALLEXCELDATAFROMALLPLAYERS.Add(r);
                }
                progressBar1.Increment(1);
            }
            OverAllProcessingExcel.Text = "Complete";
            progressBar1.Increment(100);
            progressBar2.Increment(100);
            return ALLEXCELDATAFROMALLPLAYERS;
        }

        private List<ExcelRow> ProcessExcelFile(string PathAndFileName)
        {
            progressBar2.Minimum = 0;
            progressBar2.Maximum = 347;
            progressBar2.Value = 0;
            LabelCurrentFileWorkingOn.Text = "Current File Being Processed:   " + Path.GetFileName(PathAndFileName);
            Excel.Application xlApp = new Excel.Application();
            Excel.Workbook xlWorkBook = xlApp.Workbooks.Open(PathAndFileName, 0, true, 5, "", "", true, Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
            Excel.Worksheet xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);
            Excel.Range range = xlWorkSheet.UsedRange;
            List<ExcelRow> returnMe = new List<ExcelRow>();

            char[] splitters = { '/', '-' };
            string[] PlayerFinalFirstAndMiddle = { "", "" };
            string[] PlayersFinalLastAndMiddle = { "", "" };
            string playerLastName = "";
            string firstAndMiddle = "";
            string playerFullName = Convert.ToString((range.Cells[1, 2] as Excel.Range).Value2);
            if (playerFullName.Contains(","))
            {
                playerLastName = playerFullName.Substring(0, playerFullName.IndexOf(","));
                firstAndMiddle = playerFullName.Substring(playerFullName.IndexOf(",") + 2);
            }
            // Checks to see if a period instead of a comma was accidentally placed in member name. (Rob's Request)
            else if (playerFullName.Contains("."))
            {
                playerLastName = playerFullName.Substring(0, playerFullName.IndexOf("."));
                firstAndMiddle = playerFullName.Substring(playerFullName.IndexOf(".") + 2);
            }

            string[] first0middle1 = firstAndMiddle.Split(' ');
            int playerOrgAVG;

            for (int i = 0; i < first0middle1.Length; i++)
            {
                PlayerFinalFirstAndMiddle[i] = first0middle1[0];
            }
            try
            {
                playerOrgAVG = Convert.ToInt32((range.Cells[1, 10] as Excel.Range).Value2);
            }

            catch (Exception)
            {
                string[] aftersplit;
                string orgstring;
                try
                {
                    orgstring = ((range.Cells[1, 10] as Excel.Range).Value2);
                    aftersplit = orgstring.Split('-');
                    playerOrgAVG = Convert.ToInt32(aftersplit[0]);
                }
                catch
                {
                    try
                    {
                        orgstring = ((range.Cells[1, 10] as Excel.Range).Value2);
                        aftersplit = orgstring.Split('*');
                        playerOrgAVG = Convert.ToInt32(aftersplit[0]);
                    }
                    catch
                    {
                        try
                        {
                            orgstring = ((range.Cells[1, 10] as Excel.Range).Value2);
                            aftersplit = orgstring.Split('L');
                            playerOrgAVG = Convert.ToInt32(aftersplit[0]);
                        }
                        catch
                        {
                            playerOrgAVG = -1;
                        }
                    }
                }
            }
          
            String playerNumber = (range.Cells[1, 14] as Excel.Range).Value2;
            bool isRegionHawaii = (cbHaw.Checked); // checks to see if Region is Hawaii

            if (isRegionHawaii)
            {
                playerNumber = Regex.Replace(playerNumber, "[^0-9]", "");  // strip the member number to straight number
            }

            String[] playerNumberAfterSplit;
            int playerNumberAsInt = 0;
            int.TryParse(playerNumber, out playerNumberAsInt);

            if (playerNumberAsInt != 0)
            {
                playerNumberAsInt = Convert.ToInt32(Regex.Replace(playerNumber, "[^0-9]", ""));
            }
            else if (playerNumberAsInt == 0) // if player has more then one member number, set it to their latest
            {
                for (int i = 0; i < splitters.Length; i++)
                {
                    try
                    {
                        playerNumberAfterSplit = playerNumber.Split(splitters[i]);
                        playerNumberAsInt = Convert.ToInt32(Regex.Replace(playerNumberAfterSplit[playerNumberAfterSplit.Length - 1], "[^0-9]", ""));
                    }
                    catch 
                    {

                    }
                }
            }

            for (int sheetNum = 1; sheetNum <= xlWorkBook.Worksheets.Count; sheetNum++)
            {
                xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(sheetNum);
                range = xlWorkSheet.UsedRange;

                double noGameMoneyWon = 0;

                for (int row = 3; row <= range.Rows.Count; row++)
                {
                    ExcelRow temp = new ExcelRow();
                    PlayerHistory playerH = new PlayerHistory();
                    Game GameHistory = new Game();

                    string game1 = Convert.ToString((range.Cells[row, 3] as Excel.Range).Value2);
                    string game2 = Convert.ToString((range.Cells[row, 4] as Excel.Range).Value2);
                    string game3 = Convert.ToString((range.Cells[row, 5] as Excel.Range).Value2);
                    string game4 = Convert.ToString((range.Cells[row, 6] as Excel.Range).Value2);
                    string testFin = Convert.ToString((range.Cells[row, 14] as Excel.Range).Value2);

                    if ( // if no date or cash then continue to the next line
                        string.IsNullOrWhiteSpace(Convert.ToString((range.Cells[row, 2] as Excel.Range).Value2)) &&
                        string.IsNullOrWhiteSpace(Convert.ToString((range.Cells[row, 15] as Excel.Range).Value2))
                        )
                    {
                        continue;
                    }

                    if ( // if the four games have no data AKA no games bowled and there is a finish place then add the cash to moneywon
                        string.IsNullOrWhiteSpace(game1) &&
                        string.IsNullOrWhiteSpace(game2) &&
                        string.IsNullOrWhiteSpace(game3) &&
                        string.IsNullOrWhiteSpace(game4) &&
                        !string.IsNullOrWhiteSpace(testFin)
                    )
                    {
                        noGameMoneyWon += Convert.ToDouble((range.Cells[row, 15] as Excel.Range).Value2);
                        continue;
                    }

                    GameHistory.gameRegionID = RegionID;
                    temp.PlayerFirstName = PlayerFinalFirstAndMiddle[0];
                    temp.PlayerMiddleName = PlayerFinalFirstAndMiddle[1];
                    temp.PlayerLastName = playerLastName;
                    temp.PlayerOrginalAVG = playerOrgAVG;
                    temp.PlayerNumber = playerNumberAsInt;
                    playerH.MemberNumber = temp.PlayerNumber;
                    playerH.regionID = RegionID;
                    //only process file if they have been added as a member first and are active ?
                    if (MemberDb.GetMember(temp.PlayerNumber, RegionID).IsActive == true)
                    {
                        try
                        {
                            temp.GameTotal = Convert.ToInt32((range.Cells[row, 1] as Excel.Range).Value2);
                            playerH.GamesPlayed = Convert.ToInt32((range.Cells[row, 1] as Excel.Range).Value2);
                            DateTime compare = DateTime.FromOADate(Convert.ToDouble((range.Cells[row, 2] as Excel.Range).Value2));
                            if (compare == Convert.ToDateTime("12/30/1899 12:00:00 AM"))
                            {
                                break;
                            }
                        }
                        catch
                        {
                            temp.GameTotal = -1;
                        }
                        try
                        {
                            temp.Date = DateTime.FromOADate(Convert.ToDouble((range.Cells[row, 2] as Excel.Range).Value2));
                            playerH.TournamentDate = temp.Date;
                        }
                        catch
                        {
                            temp.Date = new DateTime();
                        }
                        try
                        {
                            temp.Game1 = Convert.ToInt32((range.Cells[row, 3] as Excel.Range).Value2);
                            GameHistory.Game1 = temp.Game1;
                            playerH.Game1 = temp.Game1;
                        }
                        catch
                        {
                            temp.Game1 = -1;
                        }
                        try
                        {
                            temp.Game2 = Convert.ToInt32((range.Cells[row, 4] as Excel.Range).Value2);
                            GameHistory.Game2 = temp.Game2;
                            playerH.Game2 = temp.Game2;
                        }
                        catch
                        {
                            temp.Game2 = -1;
                        }
                        try
                        {
                            temp.Game3 = Convert.ToInt32((range.Cells[row, 5] as Excel.Range).Value2);
                            GameHistory.Game3 = temp.Game3;
                            playerH.Game3 = temp.Game3;
                        }
                        catch
                        {
                            temp.Game3 = -1;
                        }
                        try
                        {
                            temp.Game4 = Convert.ToInt32((range.Cells[row, 6] as Excel.Range).Value2);
                            GameHistory.Game4 = temp.Game4;
                            playerH.Game4 = temp.Game4;
                        }
                        catch
                        {
                            temp.Game4 = -1;
                        }
                        try
                        {
                            temp.Total = Convert.ToInt32((range.Cells[row, 7] as Excel.Range).Value2);
                            GameHistory.TotalScore = temp.Total;
                            playerH.TotalScore = temp.Total;
                        }
                        catch
                        {
                            temp.Total = -1;
                        }
                        try
                        {
                            temp.AverageOfRow = Convert.ToDouble((range.Cells[row, 8] as Excel.Range).Value2);
                            playerH.AverageForGame = temp.AverageOfRow;
                        }
                        catch
                        {
                            temp.AverageOfRow = -1;
                        }
                        try
                        {
                            temp.TrueAverage = Convert.ToDouble((range.Cells[row, 9] as Excel.Range).Value2);
                            playerH.trueAVG = temp.TrueAverage;
                        }
                        catch
                        {
                            temp.TrueAverage = -1;
                        }
                        try
                        {
                            temp.AVG = Convert.ToInt32((range.Cells[row, 10] as Excel.Range).Value2);
                            playerH.AVG = temp.AVG;

                        }
                        catch
                        {
                            temp.AVG = -1;
                        }
                        try
                        {
                            temp.HandyCap = Convert.ToInt32((range.Cells[row, 11] as Excel.Range).Value2);
                            GameHistory.Handicap = temp.HandyCap;
                            playerH.HandiCap = temp.HandyCap;
                        }
                        catch
                        {
                            temp.Bonus = -1;
                        }
                        try
                        {
                            temp.Bonus = Convert.ToInt32((range.Cells[row, 12] as Excel.Range).Value2);
                            GameHistory.Bonus = temp.Bonus;
                            playerH.Bonus = temp.Bonus;
                        }
                        catch
                        {
                            temp.HandyCap = -1000;
                        }
                        temp.PotPro = Convert.ToString((range.Cells[row, 13] as Excel.Range).Value2);
                        playerH.ProPot = temp.PotPro;
                        temp.FinPPHG = Convert.ToString((range.Cells[row, 14] as Excel.Range).Value2);
                        playerH.PPHG = temp.FinPPHG;

                        try
                        {
                            // THIS WILL CATCH SUBTOTALS THAT MAY HAVE BEEN ADDED ON LINE 46 OF THE EXCEL FILES
                            if (temp.FinPPHG.ToString() != "") // Only grab the money earned from tournament if they placed in tournament
                            {
                                temp.Cash = Convert.ToDouble((range.Cells[row, 15] as Excel.Range).Value2);
                                GameHistory.MoneyWon = Convert.ToDecimal(temp.Cash);
                                playerH.MoneyWon = Convert.ToDecimal(temp.Cash);
                            }
                            else
                            {
                                temp.Cash = 0;
                                GameHistory.MoneyWon = 0;
                                playerH.MoneyWon = 0;
                            }
                        }
                        catch
                        {
                            temp.Cash = 0;
                        }
                        playerH.MoneyWon += Convert.ToDecimal(noGameMoneyWon); 

                        temp.Notes = Convert.ToString((range.Cells[row, 16] as Excel.Range).Value2);
                        GameHistory.Notes = temp.Notes;
                        playerH.Notes = temp.Notes;
                        playerH.PPHG = temp.FinPPHG;
                        GameHistory.Id = allGames + 1;
                        allGames++;
                        playerH.GameID = GameHistory.Id;
                        GameImport.Add(GameHistory);
                        PlayerHistoryList.Add(playerH);
                        returnMe.Add(temp);
                        noGameMoneyWon = 0; 
                        progressBar2.Increment(1);
                    }
                }
            }

            xlWorkBook.Close(0);
            xlApp.Quit();

            Marshal.ReleaseComObject(range);
            Marshal.ReleaseComObject(xlWorkSheet);
            Marshal.ReleaseComObject(xlWorkBook);
            Marshal.ReleaseComObject(xlApp);

            System.Diagnostics.Process[] process = System.Diagnostics.Process.GetProcessesByName("Excel");
            foreach (System.Diagnostics.Process p in process)
            {
                try
                {
                    p.Kill();
                }
                catch { }
            }
            return returnMe;
        }

        private void btn_FinalizeData_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            IncrementFinalizeBar(0, "Step 1: Adding player histories to the database.");
            updatePlayerHistory(PlayerHistoryList);
            IncrementFinalizeBar(25, "Step 2: Player histories updated, beginning games import.");

            GameDB.AddOrUpdateSomeGames(GameImport);
            IncrementFinalizeBar(25, "Step 3: Games updated. Setting averages and bonus pins.");

            for (int i = 0; i < validMembers.Count; i++)
            {
                List<PlayerHistory> list = PlayerHistoryDB.GetLastFiveTournaments(validMembers[i].Number, RegionID);
                if (list.Count > 0)
                {
                    validMembers[i].StartAvg = list[0].AVG; //set new avg to last bowled adjusted avg
                    validMembers[i].Average = Convert.ToInt32(list[0].trueAVG); //last 30 game avg
                    validMembers[i].Bonus = list[0].Bonus; //last adjusted bonus pin
                }
            }
            IncrementFinalizeBar(25, "Step 4: Averages and bonus pins set. Updating all members.");
            updateMembers(validMembers);
            IncrementFinalizeBar(25, "Members updated");
            Cursor.Current = Cursors.Default;
            MessageBox.Show($"{validMembers.Count} members have been imported and all their bowling history has been added to the database");
            this.Close();
        }

        private void IncrementFinalizeBar(int increment, string msg)
        {
            progressBarFinalize.Increment(increment);
            lblFinalizeStatus.Text = msg;
            progressBarFinalize.Refresh();
            lblFinalizeStatus.Refresh();
        }

        private void updatePlayerHistory(List<PlayerHistory> playerHistory)
        {
            foreach (var ph in playerHistory)
            {
                PlayerHistoryDB.AddPlayerHistory(ph);
            }
        }

        private void updateMembers(List<Member> members)
        {
            for(int i = 0; i < members.Count; i++)
            {
                if (MemberDb.MemberExists(members[i]) == false)
                {
                    MemberDb.AddOrUpdateMember(members[i]);
                }
            }
        }
        
        /// <summary>
        /// Allows user to change region for where they would like to import the member data to.
        /// </summary>
        private void cbxRegionSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            List<NineTapRegion> r = NineTapRegionDB.GetRegionList();
            RegionID = r[cbxRegionSelect.SelectedIndex].NineTapRegionID;
        }
    }
}
    





