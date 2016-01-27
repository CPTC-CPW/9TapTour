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

        static int MemNumSpace = 5;
        static int DJoinedSpace = 8;
        static int LNameSpace = 20;
        static int FNameSpace = 20;
        static int MISpace = 1;
        static int EPhoneSpace = 16;
        static int DPhoneSpace = 16;
        static int CPhoneSpace = 16;
        static int Street1Space = 40;
        static int Street2Space = 40;
        static int CitySpace = 20;
        static int StateSpace = 2;
        static int ZipSpace = 10;
        static int NotesSpace = 199;
        static int AVGSpace = 3;
        static int HCSpace = 2;
        static int BSpace = 2;
        static int LastBSpace = 8;
        static int MoneyESpace = 10;
        static int SSSpace = 10;
        static int CBSpace = 4; //There are 7 of these, 2 are not needed in current design.
        static int DOBSpace = 8;

        //Create array of spaces
        int[] Spaces = new int[] { MemNumSpace, DJoinedSpace, LNameSpace, FNameSpace, MISpace, EPhoneSpace, DPhoneSpace, CPhoneSpace,
                                   Street1Space, Street2Space, CitySpace, StateSpace, ZipSpace, NotesSpace, AVGSpace, HCSpace, BSpace,
                                   LastBSpace, MoneyESpace, SSSpace, CBSpace, CBSpace, CBSpace, CBSpace, CBSpace, CBSpace, CBSpace, DOBSpace};

        private void btnOpenFile_Click(object sender, EventArgs e)
        {
            if(ofdOpen.ShowDialog() == DialogResult.OK)
            {
                System.IO.StreamReader sr = new System.IO.StreamReader(ofdOpen.FileName);
                //MessageBox.Show(sr.ReadToEnd()); //for debug purpose
                String File = sr.ReadToEnd(); //it's easier to read into a string and work with the file rather than a streamreader, which has no direct position "index" access.
                sr.Close();
                Member newMem = new Member(); // might not need this here, may move it.
                int currentIndex = 0; //starting index
                List<String> memberInfo = new List<String>();
                do  // A do while to substring from the main string
                {
                    for (int i = 0; i < Spaces.Length; i++)
                    {
                        //if(!String.IsNullOrWhiteSpace(File.Substring(currentIndex, Spaces[i]))) //if condition to try and ignore empty entries.
                        //{
                        //    memberInfo.Add(File.Substring(currentIndex, Spaces[i]));
                        //}
                        memberInfo.Add(File.Substring(currentIndex, currentIndex + Spaces[i]));
                        currentIndex += Spaces[i];
                    }
                } while (currentIndex >= File.Length);

                MessageBox.Show(String.Join(",", memberInfo));
              
            }
        }
    }
}
