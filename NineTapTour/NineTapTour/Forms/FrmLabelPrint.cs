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
using NineTapTour.Models;

namespace NineTapTour.Forms
{
    public partial class FrmLabelPrint : Form
    {
        int RegionID;
        List<Member> AllMems;
        List<Member> ActiveMems;
        List<Member> Labels;

        public FrmLabelPrint(int RegionID)
        {
            InitializeComponent();
            this.RegionID = RegionID;
        }

        private void FrmLabelPrint_Load(object sender, EventArgs e)
        {         
            AllMems = MemberDB.GetMemberList(RegionID);
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

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            foreach (Member m in lbxMemberList.SelectedItems)
            {
                Labels.Add(m);
            }
            UpdatePrintListBox();
        }

        private void BtnRemove_Click(object sender, EventArgs e)
        {
            foreach (Member m in lbxPrintList.SelectedItems)
            {
                Labels.Remove(m);
            }
            UpdatePrintListBox();
        }

        private void BtnClearAll_Click(object sender, EventArgs e)
        {
            Labels.Clear();
            UpdatePrintListBox();
        }

        private void BtnPrint_Click(object sender, EventArgs e)
        {
            // Set up components for printing
            PrintDialog printDialog = new PrintDialog();
            PrintDocument printDocument = new PrintDocument();
            // Add the document to the dialog box
            printDialog.Document = printDocument;
            // Add the event handler that will do the printing
            printDocument.PrintPage += new PrintPageEventHandler(PrintLabels);

            DialogResult result = printDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                printDocument.Print();
            }
        }
        #region Print Labels


        int currPage = 0;
        const int PageSize = 30;
        public void PrintLabels(object sender, PrintPageEventArgs e)
        {
            // Grab the next 30 members
            List<Member> nextMemberLabels = Labels.Skip((currPage) * PageSize).Take(PageSize).ToList();

            // If more than 30 members remaining another page will be printed
            e.HasMorePages = (currPage * PageSize + PageSize >= Labels.Count) ? false : true;

            // Print out 1 sheet of members, e.HasMorePages = true will cause print to be triggered again automatically
            PrintLabelSheetOf10(nextMemberLabels, e);
            currPage++;
        }

        private void PrintLabelSheetOf10(List<Member> memberLabel, PrintPageEventArgs e)
        {
            // This is what prints the data
            Graphics graphic = e.Graphics;

            // Default font to use for printing labels. Arial font will monospace the digits
            Font font = new Font("Arial", 16, FontStyle.Bold, GraphicsUnit.Pixel);

            // Sets default brush to use when printing
            SolidBrush dBrush = new SolidBrush(Color.Black);

            int startX = 55;
            int startY = 55;
            int offsetX = 0;
            int offsetY = 0;

            int start = 0;
            if(tbStartWhere.Text != "")
            {
                start = Convert.ToInt32(tbStartWhere.Text) - 1;
            }
            
            for (int i = 0; i < (memberLabel.Count + start); i++)
            {
                if(i < start)
                {
                    continue;
                }
                
                if (i % 3 == 0)
                    offsetX = 0;
                else if (i % 3 == 1)
                    offsetX = 270;
                else
                    offsetX = 540;
                offsetY = (i / 3) * 100;

                graphic.DrawString(memberLabel[i-start].FirstName.ToString() + " " + memberLabel[i - start].LastName.ToString(), font, dBrush, startX + offsetX, startY + offsetY);
                offsetY += 16;
                graphic.DrawString(memberLabel[i - start].Street, font, dBrush, startX + offsetX, startY + offsetY);
                offsetY += 16;
                graphic.DrawString(memberLabel[i - start].City + ", " + memberLabel[i - start].State + " " + memberLabel[i - start].PostalCode, font, dBrush, startX + offsetX, startY + offsetY);
            }
        }
        #endregion

        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void CbxShowInactive_CheckedChanged(object sender, EventArgs e)
        {
            ActiveMems.Clear();
            LoadLists();
        }

        public void UpdatePrintListBox()
        {
            lbxPrintList.DataSource = null;
            lbxPrintList.DataSource = Labels;
        }

        private void LbxMemberList_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            foreach (Member m in lbxMemberList.SelectedItems)
            {
                Labels.Add(m);
            }
            UpdatePrintListBox();
        }

        private void LbxPrintList_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            foreach (Member m in lbxPrintList.SelectedItems)
            {
                Labels.Remove(m);
            }
            UpdatePrintListBox();
        }


        private DateTime lastKeyPressed;

        private String searchWho;

        private void LbxMemberList_KeyPress(object sender, KeyPressEventArgs e)
        {
            var newDate = DateTime.Now;
            var diff = newDate - lastKeyPressed;
            if (diff.TotalSeconds >= 1.5)
            {
                searchWho = string.Empty;
            }
            searchWho += e.KeyChar;

            var found = lbxMemberList.Items.Cast<object>().Select(t => t.ToString()).Where(item => item.ToLower().StartsWith(searchWho)).FirstOrDefault();
            if (!String.IsNullOrEmpty(found))
            {
                lbxMemberList.SelectedItem = found;
            }

            lastKeyPressed = newDate;
            e.Handled = true;
        }
    }

}
