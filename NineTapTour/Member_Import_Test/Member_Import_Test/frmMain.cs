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
                                    newMem.FirstName = (File.Substring(currentIndex, Spaces[i]).Trim());
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
                            case 6://Secondary Phone
                                newMem.SecondaryPhone = (File.Substring(currentIndex, Spaces[i]).Trim());
                                break;
                            /*case 7:
                                    This is the cell phone from the old form*/
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
                                        newMem.Referrals = Convert.ToInt32((File.Substring(currentIndex, Spaces[i]).Trim()));
                                    }
                                    else
                                    {
                                        validMember = false;
                                    }
                                    
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
                        validCount++;
                        validMembers.Add(newMem);
                    }
                    else
                    {
                        invalidCount++;
                        invalidMembers.Add(newMem);
                    }
                    MemberCount++;
                    newMem = new Member();
                } while (currentIndex <= File.Length);

                for(int j = 0; j < validMembers.Count; j++)
                {
                    if(!DBQueries.MemberExists(validMembers[j]))
                    {
                        DBQueries.AddMember(validMembers[j]);
                    }
                    
                }
                MessageBox.Show(validCount + " valid members processed, " + invalidCount + " invalid members processed.", "Results", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnValid_Click(object sender, EventArgs e)
        {
            var message = string.Join(Environment.NewLine, validMembers);
            MessageBox.Show(message);
        }

        private void btnInvalid_Click(object sender, EventArgs e)
        {
            if(invalidMembers.Count <=0)
            {
                MessageBox.Show("No invalid members processed.");
            }
            else
            {
                var md = new FrmMemberData(invalidMembers);
                md.Show();
                this.Hide();
            }
        }
    }
}
