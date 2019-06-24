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
        int currPage = 0;
        const int PageSize = 30;
        private String searchWho;
        private DateTime lastKeyPressed;
        List<Member> AllMems;       // = MemberDb.GetMemberLabelList();
        List<Member> ActiveMems;    // = new List<Member>();
        List<Member> Labels;        // = new List<Member>();
        
        #region FrmLabelPrint
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
        #endregion

        #region Buttons
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

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion

        #region CheckBoxs
        private void cbxShowInactive_CheckedChanged(object sender, EventArgs e)
        {
            ActiveMems.Clear();
            LoadLists();
        }
        #endregion

        #region ListBoxes


        private void listBox1_SelectedIndexChanged(object sender, EventArgs e) { }
        private void listBox2_SelectedIndexChanged(object sender, EventArgs e) { }

        private void lbxMemberList_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            foreach (Member m in lbxMemberList.SelectedItems)
            {
                Labels.Add(m);
            }
            UpdatePrintListBox();
        }

        private void lbxPrintList_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            foreach (Member m in lbxPrintList.SelectedItems)
            {
                Labels.Remove(m);
            }
            UpdatePrintListBox();
        }

        private void lbxMemberList_KeyPress(object sender, KeyPressEventArgs e)
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
        #endregion

        #region Methods
        /// <summary>
        /// Updates lbxPrintList.DataSource
        /// </summary>
        public void UpdatePrintListBox()
        {
            lbxPrintList.DataSource = null;
            lbxPrintList.DataSource = Labels;
        }

        /// <summary>
        /// Updates lbxMemberList.DataSource
        /// </summary>
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

        /// <summary>
        /// Prints all Members from Labels
        /// </summary>
        public void printLabels(object sender, PrintPageEventArgs e)
        {
            // grab the next 30 members
            List<Member> nextMemberLabels = Labels.Skip((currPage) * PageSize).Take(PageSize).ToList();

            // if more than 30 members remaining another page will be printed
            e.HasMorePages = (currPage * PageSize + PageSize >= Labels.Count) ? false : true;

            // print out 1 sheet of members, e.HasMorePages = true will cause print to be triggered again automatically
            PrintLabelSheetOf10(nextMemberLabels, e);
            currPage++;
        }

        /// <summary>
        /// Prints all Members in the list given, on pages with 10 Members per page
        /// </summary>
        private void PrintLabelSheetOf10(List<Member> memberLabel, PrintPageEventArgs e)
        {
            // This is what prints the data
            Graphics graphic = e.Graphics;

            // default font to use, should use a mono space font so the spaces line up.
            Font font = new Font("Arial", 16, FontStyle.Bold, GraphicsUnit.Pixel);

            // Sets defult brush to use when printing
            SolidBrush dBrush = new SolidBrush(Color.Black);

            int startX = 55;
            int startY = 55;
            int offsetX = 0;
            int offsetY = 0;

            int start = 0;
            if (tbStartWhere.Text != "")
            {
                start = Convert.ToInt32(tbStartWhere.Text) - 1;
            }

            for (int i = 0; i < (memberLabel.Count + start); i++)
            {
                if (i < start)
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

                graphic.DrawString(memberLabel[i - start].FirstName.ToString() + " " + memberLabel[i - start].LastName.ToString(), font, dBrush, startX + offsetX, startY + offsetY);
                offsetY += 16;
                graphic.DrawString(memberLabel[i - start].Street, font, dBrush, startX + offsetX, startY + offsetY);
                offsetY += 16;
                graphic.DrawString(memberLabel[i - start].City + ", " + memberLabel[i - start].State + " " + memberLabel[i - start].PostalCode, font, dBrush, startX + offsetX, startY + offsetY);
            }
        }
        #endregion
    }
}