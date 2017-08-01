using Member_Import_Test.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;
using NineTapTour.Database;
using System.Text.RegularExpressions;
using System.Globalization;
using NineTapTour.Forms;
namespace Member_Import_Test
{
    public partial class frmMain : Form
    {

        public frmMain()
        {
            InitializeComponent();
            new NineTapDb();
        }

        //MEMBER INFO STATIC INTS
        static int MemNumSpace = 6; //Member Number
        static int DJoinedSpace = 8; //Date Joined
        static int LNameSpace = 20; //Last Name
        static int FNameSpace = 20; //First Name
        static int MISpace = 2;     //Middle Initial
        static int EPhoneSpace = 15;//Evening Phone
        static int DPhoneSpace = 15;//Day Phone
        static int CPhoneSpace = 15;//Cell Phone
        static int StreetSpace = 40;//Street Address
        static int EmailSpace = 40;//Email Address
        static int CitySpace = 20;//City
        static int StateSpace = 2;//State
        static int ZipSpace = 10;//Zip
        static int NotesSpace = 200;//Notes
        static int AVGSpace = 3;//Average
        static int HCSpace = 2;//Handicap
        static int BSpace = 2;//Bonus
        static int LastBSpace = 8;//Last Bowled
        static int YearEndTSpace = 2;//Year End Tournaments
        static int MoneyESpace = 10;//Money Earned
        static int RejoinDSpace = 8;//Rejoin Date;
        static int ReferalSpace = 2;//ReferalSpace;
        static int SSSpace = 11;//Social Security
        static int CBSpace = 5; //Check Box Spaceing, there are 7 total, only 5 are actually checked for information, repeated 7 times in Spaces array.
        static int DOBSpace = 8;// Date Of Birth.

        //PIN FILE STATIC INTS
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










        public List<Member> validMembers = new List<Member>(); //list of valid members
        public List<Member> invalidMembers = new List<Member>();//list of invalid members
       //public List<string> QBSTournamentList = new List<string>(); //list of qualified by squad tournaments
        public List<PlayerHistory> PlayerHistoryList = new List<PlayerHistory>();
        int GameIdint = 1;






        //Create array of spaces
        int[] Spaces = new int[] { MemNumSpace, DJoinedSpace, LNameSpace, FNameSpace, MISpace, EPhoneSpace, DPhoneSpace, CPhoneSpace,
                                   StreetSpace, EmailSpace, CitySpace, StateSpace, ZipSpace, NotesSpace, AVGSpace, HCSpace, BSpace,
                                   LastBSpace, YearEndTSpace, MoneyESpace, RejoinDSpace, ReferalSpace, SSSpace, CBSpace, CBSpace, CBSpace, CBSpace, CBSpace, CBSpace, CBSpace, DOBSpace};

        int[] PinSpaces = new int[] {PinFileMemNumSpace, PinFileLastName, PinFileFirstName, PinFileMiddleName, PinFileScratchScore1, PinFileScratchScore2, PinFileScratchScore3, PinFileScratchScore4 , PinFileScratchScoreTotal,
                                     PinFileHandicapScore1, PinFileHandicapScore2, PinFileHandicapScore3, PinFileHandicapScore4, PinFileHandicapScoreTotal , PinFileNotes, morsecodeslot, morsecodeslot, morsecodeslot, morsecodeslot, morsecodeslot, morsecodeslot
                                      ,morsecodeslot, morsecodeslot, morsecodeslot , morsecodeslot, morsecodeslot, morsecodeslot, morsecodeslot, morsecodeslot, morsecodeslot, morsecodeslot, morsecodeslot};

        private static List<ExcelRow> ALLEXCELDATAFROMALLPLAYERS = new List<ExcelRow>();
        private static List<Tournament> TournamentList = new List<Tournament>();

        private void btnOpenFile_Click(object sender, EventArgs e)
        {
            //Filter to limit the types of files that can be opened with the file open dialog
            ofdOpen.Filter = "Data Files (*.dat)|*.dat|Text Files (*.txt)|*.txt";
            ofdOpen.Title = "Please Select a member file to open";
            if (ofdOpen.ShowDialog() == DialogResult.OK)
            {
                System.IO.StreamReader sr = new System.IO.StreamReader(ofdOpen.FileName);
                //MessageBox.Show(sr.ReadToEnd()); //for debug purpose
                String File = sr.ReadToEnd(); //it's easier to read into a string and work with the file rather than a streamreader, which has no direct position "index" access.
                sr.Close();
                Member newMem = new Member(); // might not need this here, may move it.
                int currentIndex = 0; //starting index
                //List<String> memberInfo = new List<String>(); //for testing
                int i; //needs to be declared outside for to be used for switch
                int validCount = 0; //count of valid members added
                int invalidCount = 0; //count of invalid members added
                int MemberCount = 1; //number of current member


                while (currentIndex >= 0)
                {
                    do  // A do while to substring from the main string
                    {
                        bool validMember = true; // to determin if goes on seperate list
                        bool genderSelected = false; //check if gender has been selected 
                        bool status = false; // check if status has been selected
                        currentIndex = File.IndexOf(Convert.ToString(MemberCount), currentIndex);
                        if (currentIndex == -1)
                        {
                            break;
                        }
                        for (i = 0; i < Spaces.Length; i++)
                        {
                            switch (i)
                            {
                                case 0://Member Number
                                    if (!String.IsNullOrWhiteSpace(File.Substring(currentIndex, Spaces[i]).Trim()))
                                    {
                                        newMem.Number = Convert.ToInt32(File.Substring(currentIndex, Spaces[i]).Trim());
                                    }
                                    break;
                                case 1://Date Joined
                                    if (!String.IsNullOrWhiteSpace(File.Substring(currentIndex, Spaces[i]).Trim()))
                                    {
                                        newMem.JoinDate = Convert.ToDateTime(File.Substring(currentIndex, Spaces[i]).Trim());
                                    }
                                    break;
                                case 2://Last Name
                                    if (!String.IsNullOrWhiteSpace(File.Substring(currentIndex, Spaces[i]).Trim()))
                                    {
                                        newMem.LastName = (File.Substring(currentIndex, Spaces[i]).Trim());
                                    }
                                    else
                                    {
                                        //validMember = false;
                                    }

                                    break;
                                case 3://First Name
                                    if (!String.IsNullOrWhiteSpace(File.Substring(currentIndex, Spaces[i]).Trim()))
                                    {

                                        //newMem.FirstName = (File.Substring(currentIndex, Spaces[i]).Trim());

                                        //Idea #1: Using String.Replace\\
                                        //Simple method, but would have to account for all possible cases of extra data

                                        string [] notapartofname = {"life", "gst", "(Haw.)","pa","yk","hj","lg","mv" };


                                        string fName = File.Substring(currentIndex, Spaces[i]).Trim();

                                        newMem.FirstName = fName;

                                        for(int d = 0; d < notapartofname.Length; d++)
                                        {
                                            if (fName.Contains(notapartofname[d]))
                                            {
                                                newMem.FirstName = fName.Substring(0, fName.IndexOf(notapartofname[d])).Trim();
                                            }
                                        }

                                      

                                        //Idea #2 Using String.Split\\
                                        //Issue if name contains space, would have to check for additional parts of name

                                        //string fName = File.Substring(currentIndex, Spaces[i]).Trim();
                                        //string[] split = fName.Split(' ');
                                        //newMem.FirstName = split[0];

                                        //Idea #3 Using String.Substring\\
                                        //Issue arises if spaces between names.

                                        //string fName = File.Substring(currentIndex, Spaces[i]).Trim();
                                        //if(fName.Contains(' '))
                                        //{
                                        //   fName = fName.Substring(0, fName.LastIndexOf(' ')).Trim();                  
                                        //}
                                        //newMem.FirstName = fName;

                                        //Idea #4: Using String.Contains\\
                                        //Could improve this with an array to check for each indivual possibilty of extra data, to be able
                                        //then to use the the indexOf whatever data it found.

                                        //string fName = File.Substring(currentIndex, Spaces[i]).Trim();
                                        //if (fName.ToLower().Contains("life") || fName.ToLower().Contains("gst") || fName.ToLower().Contains("(haw.)"))
                                        //{
                                        //    fName = fName.Substring(0, fName.LastIndexOf(' ')).Trim();
                                        //}
                                        //newMem.FirstName = fName;

                                        //Idea #5 Using String.EndsWith

                                        //string fName = File.Substring(currentIndex, Spaces[i]).Trim();
                                        //if(fName.ToLower().EndsWith("life") || fName.ToLower().EndsWith("gst") || fName.ToLower().EndsWith(")"))
                                        //{
                                        //    fName = fName.Substring(0, fName.LastIndexOf(' ')).Trim();
                                        //}
                                        //newMem.FirstName = fName;
                                    }
                                    else
                                    {
                                        ////validMember = false;
                                    }
                                    break;
                                case 4://Middle Initial
                                    newMem.MiddleInitial = (File.Substring(currentIndex, Spaces[i]).Trim());
                                    break;
                                case 5://Primary Phone
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
                                case 7://Cell Phone
                                    newMem.SecondaryPhone = (File.Substring(currentIndex, Spaces[i]).Trim());
                                    break;
                                case 8://Street Address
                                    if (!String.IsNullOrWhiteSpace(File.Substring(currentIndex, Spaces[i]).Trim()))
                                    {
                                        newMem.Street = (File.Substring(currentIndex, Spaces[i]).Trim());
                                    }
                                    else
                                    {
                                        //validMember = false;
                                    }
                                    break;
                                case 9://Email Address
                                    if (!String.IsNullOrWhiteSpace(File.Substring(currentIndex, Spaces[i]).Trim()))
                                    {
                                        newMem.Email = (File.Substring(currentIndex, Spaces[i]).Trim());
                                    }
                                    else
                                    {
                                        //validMember = false;
                                    }
                                    break;
                                case 10://City
                                    if (!String.IsNullOrWhiteSpace(File.Substring(currentIndex, Spaces[i]).Trim()))
                                    {
                                        newMem.City = (File.Substring(currentIndex, Spaces[i]).Trim());
                                    }
                                    else
                                    {
                                        //validMember = false;
                                    }
                                    break;
                                case 11://State
                                    if (!String.IsNullOrWhiteSpace(File.Substring(currentIndex, Spaces[i]).Trim()))
                                    {
                                        newMem.State = (File.Substring(currentIndex, Spaces[i]).Trim());
                                    }
                                    else
                                    {
                                        //validMember = false;
                                    }
                                    break;
                                case 12://Zip
                                    if (!String.IsNullOrWhiteSpace(File.Substring(currentIndex, Spaces[i]).Trim()))
                                    {
                                        newMem.PostalCode = (File.Substring(currentIndex, Spaces[i]).Trim());
                                    }
                                    else
                                    {
                                        //validMember = false;
                                    }
                                    break;
                                case 13://Notes
                                    newMem.Notes = (File.Substring(currentIndex, Spaces[i]).Trim());
                                    break;
                                case 14://Average
                                    if (!String.IsNullOrWhiteSpace(File.Substring(currentIndex, Spaces[i]).Trim()))
                                    {
                                        newMem.StartAvg = Convert.ToInt32((File.Substring(currentIndex, Spaces[i]).Trim()));
                                        newMem.Average = newMem.StartAvg;
                                    }
                                    break;
                                case 15://Handicap
                                    if (!String.IsNullOrWhiteSpace(File.Substring(currentIndex, Spaces[i]).Trim()))
                                    {
                                        newMem.Handicap = Convert.ToInt32((File.Substring(currentIndex, Spaces[i]).Trim()));
                                    }
                                    break;
                                case 16://Bonus
                                    if (!String.IsNullOrWhiteSpace(File.Substring(currentIndex, Spaces[i]).Trim()))
                                    {
                                        newMem.Bonus = Convert.ToInt32((File.Substring(currentIndex, Spaces[i]).Trim()));
                                    }
                                    break;
                                case 17://Date Last Bowled
                                    if (!String.IsNullOrWhiteSpace(File.Substring(currentIndex, Spaces[i]).Trim()))
                                    {
                                        newMem.LastBowled = Convert.ToDateTime((File.Substring(currentIndex, Spaces[i]).Trim()));
                                    }
                                    break;
                                /*case 18:
                                    This is the year end tournaments which currently are not stored/not being used*/
                                case 19://Money Earned
                                    if (!String.IsNullOrWhiteSpace(File.Substring(currentIndex, Spaces[i]).Trim()))
                                    {
                                        newMem.MoneyEarned = Convert.ToDecimal((File.Substring(currentIndex, Spaces[i]).Trim()));
                                    }
                                    break;
                                case 20://Rejoin Date
                                    if (!String.IsNullOrWhiteSpace(File.Substring(currentIndex, Spaces[i]).Trim()))
                                    {
                                        newMem.RejoinDate = Convert.ToDateTime((File.Substring(currentIndex, Spaces[i]).Trim()));
                                    }
                                    break;
                                case 21://Referrals
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
                                            newMem.Referrals = Convert.ToInt16(File.Substring(currentIndex, Spaces[i]).Trim());
                                            //validMember = false;
                                        }

                                    }
                                    else
                                    {
                                        newMem.Referrals = null;
                                    }
                                    break;
                                case 22://Social Security Number
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
                                case 24://Active Member
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
                                case 26://Inactive Member
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
                                case 27://Senior
                                    if (!String.IsNullOrWhiteSpace(File.Substring(currentIndex, Spaces[i]).Trim()))
                                    {
                                        if (Convert.ToInt32(File.Substring(currentIndex, Spaces[i]).Trim()) == 1)
                                        {
                                            newMem.IsSenior = true;
                                        }
                                    }
                                    break;
                                case 28://Gender Female
                                    if (!String.IsNullOrWhiteSpace(File.Substring(currentIndex, Spaces[i]).Trim()))
                                    {
                                        if (Convert.ToInt32(File.Substring(currentIndex, Spaces[i]).Trim()) == 1)
                                        {
                                            newMem.Gender = MemberGenders.Female;
                                            genderSelected = true;
                                        }
                                    }
                                    break;
                                case 29://Gender Male
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
                                case 30://Birth Date
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
                                            catch (Exception ex)
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
                            //valid count to show user at end
                            validCount++;
                            //add good members to valid list to add to database once done reading file
                            validMembers.Add(newMem);
                        }
                        else
                        {
                            //invalid count to show the user at the end
                            invalidCount++;
                            //add invalid members to invalid list to be edited by user before adding to the database.
                            invalidMembers.Add(newMem);
                        }
                        MemberCount++;
                        newMem = new Member();
                    } while (currentIndex <= File.Length);

                    // go through the members on the valid list and add them to the database
                    for (int j = 0; j < validMembers.Count; j++)
                    {
                        //only add the member after checking if the memeber isn't already in the database.
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
                            NineTapTour.Database.MemberDb.AddMember(validMembers[j]);
                        }
                    }
                    //show the results to the user
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
                btnPinFileSelect.Enabled = true;
            }
            if (ALLEXCELDATAFROMALLPLAYERS.Count > 0) //&& TournamentList.Count > 0)
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
                //open the memberdata copied from main project in order to edit the invalid user information
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
        }

        private void GetAndProcessFolderWithExcelFiles()
        {
            using (var fbd = new FolderBrowserDialog())
            {
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





            string[] PlayerFinalFirstAndMiddle = { "", "" };
            string[] PlayersFinalLastAndMiddle = { "", "" };


            string playerFullName = Convert.ToString((range.Cells[1, 2] as Excel.Range).Value2);
            string playerLastName = playerFullName.Substring(0, playerFullName.IndexOf(","));


            string firstAndMiddle = playerFullName.Substring(playerFullName.IndexOf(",") + 2);
            string[] first0middle1 = firstAndMiddle.Split(' ');

            if (playerFullName.Contains("Sr")) //catches the "sr " and sets their last name to only there last name
            {
                try
                {
                    playerLastName = playerFullName.Substring(0, playerFullName.IndexOf(" "));
                }
                catch
                {
                    PlayerFinalFirstAndMiddle[0] = first0middle1[0];
                }
            }
            else
            {
                playerLastName = playerFullName.Substring(0, playerFullName.IndexOf(","));
            }

            int playerOrgAVG;
            for (int i = 0; i < first0middle1.Length; i++)
            {
                PlayerFinalFirstAndMiddle[i] = first0middle1[0];
            }
            try
            {
                playerOrgAVG = Convert.ToInt32((range.Cells[1, 10] as Excel.Range).Value2);
            }
            catch (Exception NotAValidNumber)
            {
                playerOrgAVG = -1;
            }

            String playerNumber = (range.Cells[1, 14] as Excel.Range).Value2;
            String[] playerNumberAfterSplit;
            int playerNumberAsInt = 0;
            int.TryParse(playerNumber, out playerNumberAsInt);
            if (playerNumberAsInt != 0)
            {
                playerNumberAsInt = Convert.ToInt32((range.Cells[1, 14] as Excel.Range).Value2);
            }
            else if (playerNumberAsInt == 0) // if player has more then one member number, set it to their latest
            {
                playerNumberAfterSplit = playerNumber.Split('/');
                playerNumberAsInt = Convert.ToInt32(playerNumberAfterSplit[playerNumberAfterSplit.Length - 1]);
            }

          
                for (int sheetNum = 1; sheetNum <= xlWorkBook.Worksheets.Count; sheetNum++)
                {

                    xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(sheetNum);
                    range = xlWorkSheet.UsedRange;
                for (int row = 3; row <= range.Rows.Count; row++)
                {
                    try
                    {
                        if (Convert.ToInt32((range.Cells[row, 3] as Excel.Range).Value2) == 0
                       && Convert.ToInt32((range.Cells[row, 4] as Excel.Range).Value2) == 0
                       && Convert.ToInt32((range.Cells[row, 5] as Excel.Range).Value2) == 0
                       && Convert.ToInt32((range.Cells[row, 6] as Excel.Range).Value2) == 0)
                        {
                            continue;
                        }
                    }
                    catch (Exception ex)
                    {
                        continue;
                    }
                    ExcelRow temp = new ExcelRow();
                    PlayerHistory playerH = new PlayerHistory();
                    Game GameHistory = new Game();
                    temp.PlayerFirstName = PlayerFinalFirstAndMiddle[0];
                    temp.PlayerMiddleName = PlayerFinalFirstAndMiddle[1];
                    temp.PlayerLastName = playerLastName;
                    temp.PlayerOrginalAVG = playerOrgAVG;
                    temp.PlayerNumber = playerNumberAsInt;
                    playerH.MemberNumber = temp.PlayerNumber;

                    for (int validmember = 0; validmember < validMembers.Count; validmember++)
                    {
                        if (validMembers[validmember].FirstName == temp.PlayerFirstName && validMembers[validmember].LastName == temp.PlayerLastName)
                        {//only process file if they have been added as a member first
                            try
                            {
                                temp.GameTotal = Convert.ToInt32((range.Cells[row, 1] as Excel.Range).Value2);
                                playerH.GamesPlayed = Convert.ToInt32((range.Cells[row, 1] as Excel.Range).Value2);
                                if (temp.GameTotal > 4)
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
                                playerH.TournamentDate = DateTime.FromOADate(Convert.ToDouble((range.Cells[row, 2] as Excel.Range).Value2));
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
                                temp.Cash = Convert.ToDouble((range.Cells[row, 15] as Excel.Range).Value2);
                                GameHistory.MoneyWon = Convert.ToDecimal(temp.Cash);
                                playerH.MoneyWon = Convert.ToDecimal(temp.Cash);
                            }
                            catch
                            {
                                temp.Cash = 0;
                            }
                            temp.Notes = Convert.ToString((range.Cells[row, 16] as Excel.Range).Value2);
                            GameHistory.Notes = temp.Notes;
                            playerH.Notes = temp.Notes;
                            PlayerHistoryDB.AddGame(GameHistory);
                            playerH.GameID = GameHistory.Id;
                            PlayerHistoryList.Add(playerH);
                            returnMe.Add(temp);
                            progressBar2.Increment(1);
                        }

                    }

                }
            }

            xlWorkBook.Close(false);
            xlApp.Quit();


            Marshal.ReleaseComObject(xlWorkSheet);
            Marshal.ReleaseComObject(xlWorkBook);
            Marshal.ReleaseComObject(xlApp);


         
            return returnMe;
        }

        private void btnPinFileSelect_Click(object sender, EventArgs e)
        {

            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();
                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    string[] files = Directory.GetFiles(fbd.SelectedPath);
                    for (int i = 0; i < files.Length; i++)
                    {
                        if (Path.GetExtension(files[i]) != ".pin")
                        {
                            continue;
                        }
                        if (files[i].Contains("#"))
                        {
                            //QBSTournamentList.Add(files[i]);
                        }
                        else
                        {
                            ProcessPinFile(files[i]);

                        }

                    }
                }
                MessageBox.Show(TournamentList.Count + " tournaments were imported.");
            }
            checkSpaces();
        }

        private void ProcessPinFile(string PinFileName)
        {
            Tournament currentTournament = new Tournament();
            List<Participant> listOfParticipants;
            //GETS DATE OUT OF FILE NAME
            string tournament = Path.GetFileNameWithoutExtension(PinFileName.Trim());
            string[] tournamentAfterSplit = tournament.Split(' ');
            string TournamentName = "";
            bool threeofFour = false;
            bool doubles = false;
            DateTime dt = DateTime.Today; //date extracted from the file name is put here
            //Getting Tournament date from file name
            string[] regexArray = new string[]    { @"\d{4}-\d{2}-\d{2}", // regex's used for valid dates in the fileName
                                                    @"\d{4}-\d{2}-\d{1}",
                                                    @"\d{4}-\d{1}-\d{2}",
                                                    @"\d{4}-\d{1}-\d{1}",
                                                    @"\d{2}-\d{2}-\d{4}",
                                                    @"\d{1}-\d{2}-\d{4}",
                                                    @"\d{2}-\d{1}-\d{4}",
                                                    @"\d{1}-\d{1}-\d{4}",
                                                    @"\d{2}-\d{2}-\d{2}",
                                                    @"\d{2}-\d{1}-\d{2}",
                                                    @"\d{1}-\d{2}-\d{2}",
                                                    @"\d{1}-\d{1}-\d{2}",



                                                   };
            string[] CorrectFormat = new string[]
                                                 {
                                                     "yyyy-MM-dd",         // valid formats for Date Times (corresponds to the regexArray) ex. regexArray[0] = CorrectFormat[0]
                                                     "yyyy-MM-d" ,
                                                     "yyyy-M-dd" ,
                                                     "yyyy-M-d"  ,
                                                     "MM-dd-yyyy",
                                                     "M-dd-yyyy" ,
                                                     "MM-d-yyyy" ,
                                                     "M-d-yyyy"  ,
                                                     "MM-dd-yy"  ,
                                                     "MM-d-yy"   ,
                                                     "M-dd-yy"   ,
                                                     "M-d-yy"    ,
                                                 };

            //for loop to check what date format is being used in the file name.
            for (int n = 0; n < regexArray.Length; n++)
            {
                var regex = new Regex(regexArray[n]);  // sets the regex to a regex in the regexArray list to check if a valid date is in the file name.
                {
                    Match m = regex.Match(tournament);
                    if (m.Success) //PROCESS INFO THAT HAS A VALID DATE
                    {
                        try
                        {
                            dt = DateTime.ParseExact(m.Value, CorrectFormat[n], CultureInfo.InvariantCulture);
                            break;
                        }
                        catch
                        {
                            break;
                        }
                    }
                    else if (n >= regexArray.Length - 1) //PROCESS INFO THAT DOESNT HAVE A VALID DATE TIME IN THE TITLE
                    {
                        dt = DateTime.Today; //sets defualt dt to the current date
                        break;
                    }

                }

            }
            //Adds everything before the date as part of the tournament name 
            for (int i = 0; i < tournamentAfterSplit.Length; i++)
            {
                DateTime dt2;
                if (DateTime.TryParse(tournamentAfterSplit[i], out dt2) == false)
                {
                    TournamentName += tournamentAfterSplit[i] + " ";
                }
            }
            if (tournament.Contains("3of4"))
            {

            }
            if (tournament.Contains("doubles"))
            {
                currentTournament.Squads = 8;
                doubles = true;
            }
            else
            {
                currentTournament.Squads = 4;
            }

            currentTournament.Date = dt;
            currentTournament.Location = TournamentName.TrimEnd();
            currentTournament.ThreeOutOf4 = threeofFour;
            currentTournament.Doubles = doubles;
            currentTournament.Id = TournamentList.Count + 1;
            // listOfParticipants =  ReadPinFile(PinFileName, currentTournament); 
            // currentTournament.Participant = listOfParticipants;

            TournamentList.Add(currentTournament);


        }

        //USED TO PROCESS PIN FILES INCASE HE WANTS TO ADD OLD TOURNAMENTS
        private List<Participant> ReadPinFile(string pinFileName, Tournament currentTournament)
        {
            List<Participant> partList = new List<Participant>();
            Participant pinFileParticipant = new Participant();
            Game newGame = new Game();
            System.IO.StreamReader sr = new System.IO.StreamReader(pinFileName);
            String File = sr.ReadToEnd();
            sr.Close();
            int CurrentIndex = 0;
            int i;


            while (CurrentIndex >= 0) // for loop with switch that grabs information not stored in the member class (Original Tournament scores and their bowling score)
            {

                if (CurrentIndex == -1 || CurrentIndex >= File.Length)
                {
                    break;
                }
                else
                {
                    for (i = 0; i < PinSpaces.Length; i++)
                    {
                        int ZeroOrOne;
                        switch (i)
                        {
                            case 0:
                                if (!String.IsNullOrWhiteSpace(File.Substring(CurrentIndex, PinSpaces[i])))
                                {
                                    int memberNumber = Convert.ToInt32(File.Substring(CurrentIndex, PinSpaces[i]));
                                    pinFileParticipant.Member = validMembers[memberNumber - 1]; //valid member array starts at 0
                                }
                                break;

                            case 1: //would grab last name but already stored in member class (see case 0);
                                break;
                            case 2: //would grab first name but already grabbed by member class (see case 0);
                                break;
                            case 3: //would grab middle initial but allready grabbed by member class(see case 0);
                                break;
                            case 4:
                                if (!String.IsNullOrWhiteSpace(File.Substring(CurrentIndex, PinSpaces[i]))) //game1
                                {
                                    newGame.Game1 = Convert.ToInt32(File.Substring(CurrentIndex, PinSpaces[i]));
                                }
                                break;
                            case 5:
                                if (!String.IsNullOrWhiteSpace(File.Substring(CurrentIndex, PinSpaces[i])))
                                {
                                    newGame.Game2 = Convert.ToInt32(File.Substring(CurrentIndex, PinSpaces[i]));
                                }
                                break;
                            case 6:
                                if (!String.IsNullOrWhiteSpace(File.Substring(CurrentIndex, PinSpaces[i])))
                                {
                                    newGame.Game3 = Convert.ToInt32(File.Substring(CurrentIndex, PinSpaces[i]));
                                }
                                break;
                            case 7:
                                if (!String.IsNullOrWhiteSpace(File.Substring(CurrentIndex, PinSpaces[i])))
                                {
                                    newGame.Game4 = Convert.ToInt32(File.Substring(CurrentIndex, PinSpaces[i]));
                                }
                                break;
                            case 8: // would grab scratch score total but is calculated using the 4 scratch scores (see case 4 - 7)
                                break;
                            case 9: // would grab handicap score 1 but is calculated using the 4 scratch scores (see case 4 - 7)
                                break;
                            case 10: // would grab handicap score 2 but is calculated using the 4 scratch scores (see case 4 - 7)
                                break;
                            case 11: // would grab handicap score 3 but is calculated using the 4 scratch scores (see case 4 - 7)
                                break;
                            case 12: // would grab handicap score 4 but is calculated using the 4 scratch scores (see case 4 - 7)
                                break;
                            case 13: // would grab handicap score total but is calculated using the 4 scratch scores (see case 4 - 7)
                                break;
                            case 14: //skips over notes (already stored in Member.Notes)
                                break;
                            case 15: //start of morsecode
                                break;
                            case 16:
                                break;
                            case 17:
                                break;
                            case 18:
                                break;
                            case 19:
                                break;
                            case 20:
                                break;
                            case 21:
                                break;
                            case 22:
                                if (!String.IsNullOrWhiteSpace(File.Substring(CurrentIndex, PinSpaces[i])))
                                {
                                    ZeroOrOne = Convert.ToInt32(File.Substring(CurrentIndex, PinSpaces[i]));
                                    if (ZeroOrOne == 1)
                                    {
                                        pinFileParticipant.Squad = 1;
                                    }
                                }
                                break;
                            case 23:
                                if (!String.IsNullOrWhiteSpace(File.Substring(CurrentIndex, PinSpaces[i])))
                                {
                                    ZeroOrOne = Convert.ToInt32(File.Substring(CurrentIndex, PinSpaces[i]));
                                    if (ZeroOrOne == 1)
                                    {
                                        pinFileParticipant.Squad = 2;
                                    }
                                }
                                break;
                            case 24:
                                if (!String.IsNullOrWhiteSpace(File.Substring(CurrentIndex, PinSpaces[i])))
                                {
                                    ZeroOrOne = Convert.ToInt32(File.Substring(CurrentIndex, PinSpaces[i]));
                                    if (ZeroOrOne == 1)
                                    {
                                        pinFileParticipant.Squad = 3;
                                    }
                                }
                                break;
                            case 25:
                                if (!String.IsNullOrWhiteSpace(File.Substring(CurrentIndex, PinSpaces[i])))
                                {
                                    ZeroOrOne = Convert.ToInt32(File.Substring(CurrentIndex, PinSpaces[i]));
                                    if (ZeroOrOne == 1)
                                    {
                                        pinFileParticipant.Squad = 4;
                                    }
                                }
                                break;
                            case 26:
                                break;
                            case 27:
                                break;
                            case 28:
                                break;
                            case 29:
                                break;
                            case 30:
                                break;
                            case 31:
                                break;
                            case 32:
                                break;

                        }
                        CurrentIndex += PinSpaces[i];
                    }

                    pinFileParticipant.Game = newGame;
                    partList.Add(pinFileParticipant);
                    pinFileParticipant.Tournament = currentTournament;
                    pinFileParticipant = new Participant();
                    newGame = new Game();

                }

            }

            return partList;
        }





        private void btn_FinalizeData_Click(object sender, EventArgs e)
        {
            for (int members = 0; members < validMembers.Count; members++)
            {
                for (int ExcelFileSlot = 0; ExcelFileSlot < ALLEXCELDATAFROMALLPLAYERS.Count; ExcelFileSlot++)

                {
                    //if for some reason the member number on the DAT file and the member number on the excel file do not match, set the new permanent member number to that of the DAT file
                    if (validMembers[members].FirstName == ALLEXCELDATAFROMALLPLAYERS[ExcelFileSlot].PlayerFirstName && validMembers[members].LastName == ALLEXCELDATAFROMALLPLAYERS[ExcelFileSlot].PlayerLastName
                       && validMembers[members].Number != ALLEXCELDATAFROMALLPLAYERS[ExcelFileSlot].PlayerNumber)
                    {
                        ALLEXCELDATAFROMALLPLAYERS[ExcelFileSlot].PlayerNumber = validMembers[members].Number;
                        PlayerHistoryList[ExcelFileSlot].MemberNumber = validMembers[members].Number;
                        validMembers[members].Id = validMembers[members].Number;
                    }
                    else if ((validMembers[members].FirstName == ALLEXCELDATAFROMALLPLAYERS[ExcelFileSlot].PlayerFirstName && validMembers[members].LastName == ALLEXCELDATAFROMALLPLAYERS[ExcelFileSlot].PlayerLastName
                       && validMembers[members].Number == ALLEXCELDATAFROMALLPLAYERS[ExcelFileSlot].PlayerNumber))
                    {
                        PlayerHistoryList[ExcelFileSlot].MemberNumber = validMembers[members].Number;
                    }


                    //if Current selected member has an excel file 
                    if (validMembers[members].Number == ALLEXCELDATAFROMALLPLAYERS[ExcelFileSlot].PlayerNumber)
                    {
                        validMembers[members].StartAvg = ALLEXCELDATAFROMALLPLAYERS[ExcelFileSlot].PlayerOrginalAVG;
                    }
                }
            }

            frmPleaseWait please = new frmPleaseWait();
            please.Show();
            updateMembers(validMembers);
            updatePlayerHistory(PlayerHistoryList);
            please.Close();
       

            MessageBox.Show($"{validMembers.Count} members have been imported and all their bowling history has been added to the database");

            this.Close();
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
            foreach(var m in members)
            {
                MemberDb.AddMember(m);
            }
        }

   
        





    }


}
    





