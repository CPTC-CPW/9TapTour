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

namespace Member_Import_Test
{
    public partial class frmMain : Form
    {
        
        public frmMain()
        {
            InitializeComponent();
            new NineTapDb();
        }
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

        List<Member> validMembers = new List<Member>(); //list of valid members
        public List<Member> invalidMembers = new List<Member>();//list of invalid members

        //Create array of spaces
        int[] Spaces = new int[] { MemNumSpace, DJoinedSpace, LNameSpace, FNameSpace, MISpace, EPhoneSpace, DPhoneSpace, CPhoneSpace,
                                   StreetSpace, EmailSpace, CitySpace, StateSpace, ZipSpace, NotesSpace, AVGSpace, HCSpace, BSpace,
                                   LastBSpace, YearEndTSpace, MoneyESpace, RejoinDSpace, ReferalSpace, SSSpace, CBSpace, CBSpace, CBSpace, CBSpace, CBSpace, CBSpace, CBSpace, DOBSpace};

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
                        if(currentIndex == -1)
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
                                        validMember = false;
                                    }

                                    break;
                                case 3://First Name
                                    if (!String.IsNullOrWhiteSpace(File.Substring(currentIndex, Spaces[i]).Trim()))
                                    {

                                        //newMem.FirstName = (File.Substring(currentIndex, Spaces[i]).Trim());

                                        //Idea #1: Using String.Replace\\
                                        //Simple method, but would have to account for all possible cases of extra data

                                        string fName = File.Substring(currentIndex, Spaces[i]).Trim();
                                        fName = fName.Replace("life", " ").Trim();
                                        fName = fName.Replace("gst", " ").Trim();
                                        fName = fName.Replace("(Haw.)", " ").Trim();
                                        newMem.FirstName = fName;

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
                                        validMember = false;
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
                                        validMember = false;
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
                                        validMember = false;
                                    }
                                    break;
                                case 9://Email Address
                                    if (!String.IsNullOrWhiteSpace(File.Substring(currentIndex, Spaces[i]).Trim()))
                                    {
                                        newMem.Email = (File.Substring(currentIndex, Spaces[i]).Trim());
                                    }
                                    else
                                    {
                                        validMember = false;
                                    }
                                    break;
                                case 10://City
                                    if (!String.IsNullOrWhiteSpace(File.Substring(currentIndex, Spaces[i]).Trim()))
                                    {
                                        newMem.City = (File.Substring(currentIndex, Spaces[i]).Trim());
                                    }
                                    else
                                    {
                                        validMember = false;
                                    }
                                    break;
                                case 11://State
                                    if (!String.IsNullOrWhiteSpace(File.Substring(currentIndex, Spaces[i]).Trim()))
                                    {
                                        newMem.State = (File.Substring(currentIndex, Spaces[i]).Trim());
                                    }
                                    else
                                    {
                                        validMember = false;
                                    }
                                    break;
                                case 12://Zip
                                    if (!String.IsNullOrWhiteSpace(File.Substring(currentIndex, Spaces[i]).Trim()))
                                    {
                                        newMem.PostalCode = (File.Substring(currentIndex, Spaces[i]).Trim());
                                    }
                                    else
                                    {
                                        validMember = false;
                                    }
                                    break;
                                case 13://Notes
                                    newMem.Notes = (File.Substring(currentIndex, Spaces[i]).Trim());
                                    break;
                                case 14://Average
                                    if (!String.IsNullOrWhiteSpace(File.Substring(currentIndex, Spaces[i]).Trim()))
                                    {
                                        newMem.Average = Convert.ToInt32((File.Substring(currentIndex, Spaces[i]).Trim()));
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
                                            validMember = false;
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
                                        validMember = false;
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
                                            validMember = false;
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
                                            validMember = false;
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
                                        else
                                        {
                                            Console.WriteLine(File.Substring(currentIndex, Spaces[i]).Trim());
                                            newMem.DateOfBirth = Convert.ToDateTime(File.Substring(currentIndex, Spaces[i]).Trim());
                                        }

                                    }
                                    else
                                    {
                                        validMember = false;
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
                        if (!DBQueries.MemberExists(validMembers[j]))
                        {
                            DBQueries.AddMember(validMembers[j]);
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
            if(invalidMembers.Count > 0 || validMembers.Count > 0)
            {
                btnSelectExcelFolder.Enabled = true;
                btnPinFileSelect.Enabled = true;
            }
            if(ALLEXCELDATAFROMALLPLAYERS.Count > 0 && TournamentList.Count > 0)
            {
                btn_FinalizeData.Enabled = true;
            }
        }

        private void btnInvalid_Click(object sender, EventArgs e)
        {
            if(invalidMembers.Count <=0)
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
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();
                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath)) {
                    string[] files = Directory.GetFiles(fbd.SelectedPath);
                    GetAllExcelData(files);
                }
            }
            checkSpaces();
        }

        private static List<ExcelRow> GetAllExcelData(string[] files)
        {
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
            }
            MessageBox.Show("All of the Member Excel Files have been Imported.");
            return ALLEXCELDATAFROMALLPLAYERS;
        }

        private static List<ExcelRow> ProcessExcelFile(string PathAndFileName)
        {
            Excel.Application xlApp = new Excel.Application();
            Excel.Workbook xlWorkBook = xlApp.Workbooks.Open(PathAndFileName, 0, true, 5, "", "", true, Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
            Excel.Worksheet xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);
            Excel.Range range = xlWorkSheet.UsedRange;
            List<ExcelRow> returnMe = new List<ExcelRow>();
            string[] PlayerFinalFirstAndMiddle = { "", "" };
            string playerFullName = Convert.ToString((range.Cells[1, 2] as Excel.Range).Value2);
            string playerLastName = playerFullName.Substring(0, playerFullName.IndexOf(","));
            string firstAndMiddle = playerFullName.Substring(playerFullName.IndexOf(",") + 2);
            string[] first0middle1 = firstAndMiddle.Split(' ');
            for (int i = 0; i < first0middle1.Length; i++)
            {
                PlayerFinalFirstAndMiddle[i] = first0middle1[0];
            }
            int playerOrgAVG = Convert.ToInt32((range.Cells[1, 10] as Excel.Range).Value2);
            int playerNumber = Convert.ToInt32((range.Cells[1, 14] as Excel.Range).Value2);

            for (int sheetNum = 1; sheetNum < xlWorkBook.Worksheets.Count; sheetNum++)
            {
                xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(sheetNum);
                range = xlWorkSheet.UsedRange;
                for (int row = 3; row <= range.Rows.Count; row++)
                {
                    

                    if (Convert.ToInt32((range.Cells[row, 3] as Excel.Range).Value2) == 0
                         && Convert.ToInt32((range.Cells[row, 4] as Excel.Range).Value2) == 0
                         && Convert.ToInt32((range.Cells[row, 5] as Excel.Range).Value2) == 0
                         && Convert.ToInt32((range.Cells[row, 6] as Excel.Range).Value2) == 0)
                    {
                        continue;
                    }
                    ExcelRow temp = new ExcelRow();
                    temp.PlayerFirstName = PlayerFinalFirstAndMiddle[0];
                    temp.PlayerMiddleName = PlayerFinalFirstAndMiddle[1];
                    temp.PlayerLastName = playerLastName;
                    temp.PlayerOrginalAVG = playerOrgAVG;
                    temp.PlayerNumber = playerNumber;
                    try
                    {
                        temp.GameTotal = Convert.ToInt32((range.Cells[row, 1] as Excel.Range).Value2);
                    }
                    catch {
                        temp.GameTotal = -1;
                    }
                    try
                    {
                        temp.Date = DateTime.FromOADate(Convert.ToDouble((range.Cells[row, 2] as Excel.Range).Value2));
                    }
                    catch {
                        temp.Date = new DateTime();
                    }
                    try
                    {
                        temp.Game1 = Convert.ToInt32((range.Cells[row, 3] as Excel.Range).Value2);
                    }
                    catch {
                        temp.Game1 = -1;
                    }
                    try
                    {
                        temp.Game2 = Convert.ToInt32((range.Cells[row, 4] as Excel.Range).Value2);
                    }
                    catch {
                        temp.Game2 = -1;
                    }
                    try
                    {
                        temp.Game3 = Convert.ToInt32((range.Cells[row, 5] as Excel.Range).Value2);
                    }
                    catch {
                        temp.Game3 = -1;
                    }
                    try
                    {
                        temp.Game4 = Convert.ToInt32((range.Cells[row, 6] as Excel.Range).Value2);
                    }
                    catch {
                        temp.Game4 = -1;
                    }
                    try
                    {
                        temp.Total = Convert.ToInt32((range.Cells[row, 7] as Excel.Range).Value2);
                    }
                    catch {
                        temp.Total = -1;
                    }
                    try
                    {
                        temp.AverageOfRow = Convert.ToDouble((range.Cells[row, 8] as Excel.Range).Value2);
                    }
                    catch {
                        temp.AverageOfRow = -1;
                    }
                    try
                    {
                        temp.TrueAverage = Convert.ToDouble((range.Cells[row, 9] as Excel.Range).Value2);
                    }
                    catch {
                        temp.TrueAverage = -1;
                    }
                    try
                    {
                        temp.AVG = Convert.ToInt32((range.Cells[row, 10] as Excel.Range).Value2);
                    }
                    catch {
                        temp.AVG = -1;
                    }
                    try
                    {
                        temp.Bonus = Convert.ToInt32((range.Cells[row, 11] as Excel.Range).Value2);
                    }
                    catch {
                        temp.Bonus = -1000;
                    }
                    try
                    {
                        temp.HandyCap = Convert.ToInt32((range.Cells[row, 12] as Excel.Range).Value2);
                    }
                    catch {
                        temp.HandyCap = -1;
                    }
                    temp.PotPro = Convert.ToString((range.Cells[row, 13] as Excel.Range).Value2);
                    temp.FinPPHG = Convert.ToString((range.Cells[row, 14] as Excel.Range).Value2);
                    try
                    {
                        temp.Cash = Convert.ToDecimal((range.Cells[row, 15] as Excel.Range).Value2);
                    }
                    catch {
                        temp.Cash = 0;
                    }
                    temp.Notes = Convert.ToString((range.Cells[row, 16] as Excel.Range).Value2);

                    returnMe.Add(temp);
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
                        ProcessPinFile(files[i]);
                    }
                }
                MessageBox.Show(TournamentList.Count + " tournaments were imported.");
            }
            checkSpaces();
        }

        private void ProcessPinFile(string PinFileName)
        {
            Tournament currentTournament = new Tournament();
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
                        dt = DateTime.ParseExact(m.Value, CorrectFormat[n], CultureInfo.InvariantCulture);
                        break;
                    }
                    else if (n >= regexArray.Length - 1) //PROCESS INFO THAT DOESNT HAVE A VALID DATE TIME IN THE TITLE
                    {
                        dt = DateTime.Today; //sets defualt dt to the current date
                        break;
                    }

                }
               
            }
            //Adds everything before the date as part of the tournament name 
            for(int i = 0; i < tournamentAfterSplit.Length; i++)
            {
                DateTime dt2;
                if (DateTime.TryParse(tournamentAfterSplit[i], out dt2) == false)
                {
                    TournamentName += tournamentAfterSplit[i] + " "; 
                }
            }
            if(tournament.Contains("3of4"))
            {
                threeofFour = true;
            }
            if (tournament.Contains("doubles"))
            {
                doubles = true;
            }
            currentTournament.Date = dt;
            currentTournament.Location = TournamentName.TrimEnd();
            currentTournament.ThreeOutOf4 = threeofFour;
            currentTournament.Doubles = doubles;
            TournamentList.Add(currentTournament);
            currentTournament.Id = TournamentList.Count;
        }


        private void btn_FinalizeData_Click(object sender, EventArgs e)
        {
            for(int pinFileSlot = 0; pinFileSlot < TournamentList.Count; pinFileSlot++)//CHANGE TO VALID MEMBERS LIST ON LAUNCH. INVALID USED FOR TESTING
            {
                
                List<Participant> ParticipantsForTournament = new List<Participant>();
               
              
                for (int members = 0; members < invalidMembers.Count; members++)
                {
                    int squadnumber = 0;
                    for (int ExcelFileSlot = 0; ExcelFileSlot < ALLEXCELDATAFROMALLPLAYERS.Count; ExcelFileSlot++)
                    {
                      
                        //if Current selected member has an excel file and their excel file has a date == tournament date
                        if(invalidMembers[members].Number == ALLEXCELDATAFROMALLPLAYERS[ExcelFileSlot].PlayerNumber //CHANGE TO VALID MEMBERS LIST ON LAUNCH. INVALID USED FOR TESTING
                        && ALLEXCELDATAFROMALLPLAYERS[ExcelFileSlot].Date == TournamentList[pinFileSlot].Date)
                        {
                           
                            squadnumber++;
                            //MessageBox.Show(invalidMembers[members].FirstName + " played in a tournament at " + TournamentList[pinFileSlot].Location + " on " + ALLEXCELDATAFROMALLPLAYERS[ExcelFileSlot].Date); TEST MESSAGE BOX
                            Participant currentParticipant = new Participant();
                            currentParticipant.Tournament = TournamentList[pinFileSlot];
                            currentParticipant.Member = invalidMembers[members];
                            currentParticipant.Squad = squadnumber;
                            

                            Game currentGame = new Game();
                            currentGame.Id = squadnumber;
                            currentGame.Game1 = ALLEXCELDATAFROMALLPLAYERS[ExcelFileSlot].Game1;
                            currentGame.Game2 = ALLEXCELDATAFROMALLPLAYERS[ExcelFileSlot].Game2;
                            currentGame.Game3 = ALLEXCELDATAFROMALLPLAYERS[ExcelFileSlot].Game3;
                            currentGame.Game4 = ALLEXCELDATAFROMALLPLAYERS[ExcelFileSlot].Game4;
                            
                            currentGame.Handicap = ALLEXCELDATAFROMALLPLAYERS[ExcelFileSlot].HandyCap;
                            currentGame.Bonus = ALLEXCELDATAFROMALLPLAYERS[ExcelFileSlot].Bonus;
                            currentGame.InputtedAvg = ALLEXCELDATAFROMALLPLAYERS[ExcelFileSlot].AVG; //comeback
                            currentGame.MoneyWon = Convert.ToDecimal(ALLEXCELDATAFROMALLPLAYERS[ExcelFileSlot].Cash);
                            currentGame.Notes = ALLEXCELDATAFROMALLPLAYERS[ExcelFileSlot].Notes;

                            currentParticipant.Game = currentGame;
                            ParticipantsForTournament.Add(currentParticipant);
                            currentParticipant.Id = ParticipantsForTournament.Count;
                        }
                    }
                }
              
                TournamentList[pinFileSlot].Participant = ParticipantsForTournament;
            }
            



            populateTournements(TournamentList);
        }
        private void populateTournements(List<Tournament> tournements)
        {
            
            foreach (Tournament t in tournements)
            {
                TournamentDb.AddTournament(t);
            }
        }
    }
}

