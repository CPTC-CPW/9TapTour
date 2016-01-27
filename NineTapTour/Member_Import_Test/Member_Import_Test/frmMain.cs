using Member_Import_Test.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
        static int MISpace = 1;     //Middle Initial
        static int EPhoneSpace = 15;//Evening Phone
        static int DPhoneSpace = 15;//Day Phone
        static int CPhoneSpace = 15;//Cell Phone
        static int StreetSpace = 40;//Street Address
        static int EmailSpace = 40;//Email Address
        static int CitySpace = 20;//City
        static int StateSpace = 2;//State
        static int ZipSpace = 10;//Zip
        static int NotesSpace = 199;//Notes
        static int AVGSpace = 3;//Average
        static int HCSpace = 2;//Handicap
        static int BSpace = 2;//Bonus
        static int LastBSpace = 8;//Last Bowled
        static int YearEndTSpace = 2;//Year End Tournaments
        static int MoneyESpace = 10;//Money Earned
        static int RejoinDSpace = 8;//Rejoin Date;
        static int ReferalSpace = 2;//ReferalSpace;
        static int SSSpace = 10;//Social Security
        static int CBSpace = 4; //Check Box Spaceing, there are 7 total, only 5 are actually checked for information, repeated 7 times in Spaces array.
        static int DOBSpace = 8;// Date Of Birth.

        List<Member> validMembers = new List<Member>(); //list of valid members
        List<Member> invalidMembers = new List<Member>();//list of invalid members

        //Create array of spaces
        int[] Spaces = new int[] { MemNumSpace, DJoinedSpace, LNameSpace, FNameSpace, MISpace, EPhoneSpace, DPhoneSpace, CPhoneSpace,
                                   StreetSpace, EmailSpace, CitySpace, StateSpace, ZipSpace, NotesSpace, AVGSpace, HCSpace, BSpace,
                                   LastBSpace, YearEndTSpace, MoneyESpace, RejoinDSpace, ReferalSpace, SSSpace, CBSpace, CBSpace, CBSpace, CBSpace, CBSpace, CBSpace, CBSpace, DOBSpace};

        private void btnOpenFile_Click(object sender, EventArgs e)
        {
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
                do  // A do while to substring from the main string
                {
                    bool validMember = true; // to determin if goes on seperate list
                    bool genderSelected = false; //check if gender has been selected 
                    bool status = false; // check if status has been selected
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
                                newMem.LastName = (File.Substring(currentIndex, Spaces[i]).Trim());
                                break;
                            case 3://First Name
                                newMem.FirstName = (File.Substring(currentIndex, Spaces[i]).Trim());
                                break;
                            case 4://Middle Initial
                                newMem.MiddleInitial = (File.Substring(currentIndex, Spaces[i]).Trim());
                                break;
                            case 5://Primary Phone
                                newMem.PrimaryPhone = (File.Substring(currentIndex, Spaces[i]).Trim());
                                break;
                            case 6://Secondary Phone
                                newMem.SecondaryPhone = (File.Substring(currentIndex, Spaces[i]).Trim());
                                break;
                            /*case 7:
                                    This is the cell phone from the old form*/
                            case 8://Street Address
                                newMem.Street = (File.Substring(currentIndex, Spaces[i]).Trim());
                                break;
                            case 9://Email Address
                                newMem.Email = (File.Substring(currentIndex, Spaces[i]).Trim());
                                break;
                            case 10://City
                                newMem.City = (File.Substring(currentIndex, Spaces[i]).Trim());
                                break;
                            case 11://State
                                newMem.State = (File.Substring(currentIndex, Spaces[i]).Trim());
                                break;
                            case 12://Zip
                                newMem.PostalCode = (File.Substring(currentIndex, Spaces[i]).Trim());
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
                                    newMem.Referrals = Convert.ToInt32((File.Substring(currentIndex, Spaces[i]).Trim()));
                                }
                                break;
                            case 22://Social Security Number
                                newMem.SSN = File.Substring(currentIndex, Spaces[i]).Trim();
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
                                    if(status)
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
                                    if(genderSelected)
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
                                if (!String.IsNullOrWhiteSpace(File.Substring(currentIndex, Spaces[i]).Trim()))
                                {
                                    newMem.DateOfBirth = Convert.ToDateTime(File.Substring(currentIndex, Spaces[i]).Trim());
                                }
                                    
                                break;
                        }
                        currentIndex += Spaces[i];
                    }
                    if(validMember)
                    {
                        validMembers.Add(newMem);
                    }
                    else
                    {
                        invalidMembers.Add(newMem);
                    }
                    newMem = new Member();
                } while (currentIndex >= File.Length);

                MessageBox.Show("Complete.");

                //MessageBox.Show(String.Join(",", memberInfo));

            }
        }
    }
}
