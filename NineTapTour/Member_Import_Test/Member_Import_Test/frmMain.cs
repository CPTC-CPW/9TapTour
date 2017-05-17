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

namespace Member_Import_Test
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
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

        private void btnOpenFile_Click(object sender, EventArgs e)
        {
            //Filter to limit the types of files that can be opened with the file open dialog
            ofdOpen.Filter = "Text Files (*.txt)|*.txt|Data Files (*.dat)|*.dat";
            ofdOpen.Title = "Please Select a member file to open";
            if (ofdOpen.ShowDialog() == DialogResult.OK)
            {
                System.IO.StreamReader sr = new System.IO.StreamReader(ofdOpen.FileName);
                //MessageBox.Show(sr.ReadToEnd()); //for debug purpose
                String File = sr.ReadToEnd().Trim(); //it's easier to read into a string and work with the file rather than a streamreader, which has no direct position "index" access.
                sr.Close();
                Member newMem = new Member(); // might not need this here, may move it.
                int currentIndex = 0; //starting index
                //List<String> memberInfo = new List<String>(); //for testing
                int i; //needs to be declared outside for to be used for switch
                int validCount = 0; //count of valid members added
                int invalidCount = 0; //count of invalid members added
                int MemberCount = 1; //number of current member
                do  // A do while to substring from the main string
                {
                    bool validMember = true; // to determin if goes on seperate list
                    bool genderSelected = false; //check if gender has been selected 
                    bool status = false; // check if status has been selected
                    currentIndex = File.IndexOf(Convert.ToString(MemberCount), currentIndex);
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
                                    if(isNum)
                                    {
                                        newMem.Referrals = (File.Substring(currentIndex, Spaces[i]).Trim());
                                    }
                                    else
                                    {
                                        newMem.Referrals = (File.Substring(currentIndex, Spaces[i]).Trim());
                                        validMember = false;
                                    }
                                    
                                }
                                else
                                {
                                    newMem.Referrals = "";
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
                                if(!String.IsNullOrWhiteSpace(File.Substring(currentIndex, Spaces[i]).Trim()))
                                {
                                    if(status && Convert.ToInt32(File.Substring(currentIndex, Spaces[i]).Trim()) == 1)
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
                                    if(genderSelected && Convert.ToInt32(File.Substring(currentIndex, Spaces[i]).Trim()) == 1)
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
                                    if(File.Length - currentIndex < 8)
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
                    if(validMember)
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
                    for (int i = 0; i < files.Length; i++ )
                    {
                        if (Path.GetExtension(files[i]) != ".xls")
                        {
                            continue;
                        }
                        ProcessExcelFile(files[i]);
                    }
                }
            }
        }

        private static void ProcessExcelFile(string PathAndFileName)
        {
            //coment out later Diagnostic line!
            MessageBox.Show(PathAndFileName);
            
            Excel.Application xlApp = new Excel.Application();
            Excel.Workbook xlWorkBook = xlApp.Workbooks.Open(PathAndFileName, 0, true, 5, "", "", true, Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
            Excel.Worksheet xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);
            Excel.Range range = xlWorkSheet.UsedRange;


            //***********************************************************************************************
            //      This will message box each row and column in the excel doc...
            //      In plane english it will show you what is in each cell of an excel doc... Because we
            //          will be working with a univercial excel files we can just grab the data we want 
            //          from the cells that we want... :)
            //***********************************************************************************************
            string DataFromCell = "";
            for (int row = 1; row <= range.Rows.Count; row++)
            {
                for (int col = 1; col <= range.Columns.Count; col++)
                {
                    DataFromCell += "\nRow/col: " + row + "/" + col + "\n" + Convert.ToString( (range.Cells[row, col] as Excel.Range).Value2 );
                }
            }
            MessageBox.Show(DataFromCell);
            //***********************************************************************************************
            //***********************************************************************************************


            xlWorkBook.Close(false);
            xlApp.Quit();

            Marshal.ReleaseComObject(xlWorkSheet);
            Marshal.ReleaseComObject(xlWorkBook);
            Marshal.ReleaseComObject(xlApp);
        }
    }
}
