using NineTapTour.Database;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NineTapTour.Forms
{
    public partial class FrmLabelPrint : Form
    {
        int RegionID;
        List<Member> AllMems; //= MemberDb.GetMemberList();
        List<Member> ActiveMems;// = new List<Member>();
        List<Member> Labels; // = new List<Member>();

        public FrmLabelPrint(int RegionID)
        {
            InitializeComponent();
            this.RegionID = RegionID;
        }

        private void FrmLabelPrint_Load(object sender, EventArgs e)
        {         
            AllMems = MemberDb.GetMemberList(RegionID);
            ActiveMems = new List<Member>();
            Labels = new List<Member>();

            LoadLists();
        }

        public void LoadLists()
        {
            try
            {
                if (!cbxShowInactive.Checked)
                {
                    foreach (Member m in AllMems)
                    {
                        if (m.IsActive)
                        {
                            ActiveMems.Add(m);
                        }
                    }

                    lbxMemberList.DataSource = ActiveMems;
                }
                else
                {
                    lbxMemberList.DataSource = AllMems;
                }
            }
            catch (NullReferenceException nex)
            {
                Console.WriteLine("Error Number : " + nex.Message);
                if (!cbxShowInactive.Checked)
                {
                    MessageBox.Show("There are no active members to show; check inactive to load all members.");
                }
                else
                {
                    MessageBox.Show("There are no members to show.");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error Number : " + ex.Message);
                MessageBox.Show("An error occured. Please reload the form.");
            }
        }

        #region indexchanged
        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        #endregion

        private void btnAdd_Click(object sender, EventArgs e)
        {
            foreach (Member m in lbxMemberList.SelectedItems)
            {
                Labels.Add(m);
            }
            UpdatePrintListBox();
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            foreach (Member m in lbxPrintList.SelectedItems)
            {
                Labels.Remove(m);
            }
            UpdatePrintListBox();
        }

        private void btnClearAll_Click(object sender, EventArgs e)
        {
            Labels.Clear();
            UpdatePrintListBox();
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            //Set up compenents for printing
            PrintDialog printDialog = new PrintDialog();
            PrintDocument printDocument = new PrintDocument();
            //add the document to the dialog box
            printDialog.Document = printDocument;
            //add the event handler that will do the printing
            printDocument.PrintPage += new PrintPageEventHandler(printLabels);

            DialogResult result = printDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                printDocument.Print();
            }
        }
        #region Print Labels

        //TODO need to reset current page or refactor and avoid fields
        int currPage = 0;
        const int PageSize = 30;
        public void printLabels(object sender, PrintPageEventArgs e)
        {
            //grab the next 30 members
            List<Member> nextMemberLabels = Labels.Skip((currPage) * PageSize).Take(PageSize).ToList();

            //if more than 30 members remaining another page will be printed
            e.HasMorePages = (currPage * PageSize + PageSize >= Labels.Count) ? false : true;

            //print out 1 sheet of members, e.HasMorePages = true will cause print to be triggered again automatically
            PrintLabelSheetOf10(nextMemberLabels, e);
            currPage++;

        }

        private void PrintLabelSheetOf10(List<Member> memberLabel, PrintPageEventArgs e)
        {
            //This is what prints the data
            Graphics graphic = e.Graphics;

            //default font to use, should use a mono space font so the spaces line up.
            Font font = new Font("Arial", 16, FontStyle.Bold, GraphicsUnit.Pixel);

            //Sets defult brush to use when printing
            SolidBrush dBrush = new SolidBrush(Color.Black);

            int startX = 55;
            int startY = 55;
            int offsetX = 0;
            int offsetY = 0;
            for (int i = 0; i < memberLabel.Count; i++)
            {
                if (i % 3 == 0)
                    offsetX = 0;
                else if (i % 3 == 1)
                    offsetX = 270;
                else
                    offsetX = 540;
                offsetY = (i / 3) * 100;

                graphic.DrawString(memberLabel[i].FirstName.ToString() + " " + memberLabel[i].LastName.ToString(), font, dBrush, startX + offsetX, startY + offsetY);
                offsetY += 16;
                graphic.DrawString(memberLabel[i].Street, font, dBrush, startX + offsetX, startY + offsetY);
                offsetY += 16;
                graphic.DrawString(memberLabel[i].City + ", " + memberLabel[i].State + " " + memberLabel[i].PostalCode, font, dBrush, startX + offsetX, startY + offsetY);
            }
        }
        #endregion

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cbxShowInactive_CheckedChanged(object sender, EventArgs e)
        {
            ActiveMems.Clear();
            LoadLists();
        }

        public void UpdatePrintListBox()
        {
            lbxPrintList.DataSource = null;
            lbxPrintList.DataSource = Labels;
        }
    }
}
