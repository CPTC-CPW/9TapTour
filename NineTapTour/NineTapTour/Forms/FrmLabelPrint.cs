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
        List<Member> AllMems = MemberDb.GetMemberList();
        List<Member> ActiveMems = new List<Member>();
        List<Member> Labels = new List<Member>();

        public FrmLabelPrint()
        {
            InitializeComponent();
        }

        private void FrmLabelPrint_Load(object sender, EventArgs e)
        {
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
        public void printLabels(object sender, PrintPageEventArgs e)
        {
            //This is what prints the data
            Graphics graphic = e.Graphics;

            //default font to use, should use a mono space font so the spaces line up.
            Font font = new Font("Arial", 24, FontStyle.Bold, GraphicsUnit.Pixel);

            //Sets defult brush to use when printing
            SolidBrush dBrush = new SolidBrush(Color.Black);

            int startX = 75;
            int startY = 100;
            int offsetY = 0;
            int offsetX = 0;
            int totalCount = Labels.Count();

            foreach (Member m in Labels)
            {
                if (totalCount > 10)
                {
                    e.HasMorePages = true;
                }
                else
                {
                    e.HasMorePages = false;
                }

                if (Labels.IndexOf(m) % 10 < 2)
                {
                    graphic.DrawString(m.FirstName.ToString() + " " + m.LastName.ToString(), font, dBrush, startX + offsetX, startY + offsetY);
                    offsetY += 24;
                    graphic.DrawString(m.Street, font, dBrush, startX + offsetX, startY + offsetY);
                    offsetY += 24;
                    graphic.DrawString(m.City + ", " + m.State + " " + m.PostalCode, font, dBrush, startX + offsetX, startY + offsetY);
                    offsetX = 425;
                    offsetY = 0;
                    totalCount -= 1;
                }
                else if ((Labels.IndexOf(m) % 10 >= 2 && Labels.IndexOf(m) % 10 < 4))
                {
                    offsetY = 200;
                    if (Labels.IndexOf(m) % 10 == 2)
                    {
                        offsetX = 0;
                    }
                    else
                    {
                        offsetX = 425;
                    }
                    graphic.DrawString(m.FirstName.ToString() + " " + m.LastName.ToString(), font, dBrush, startX + offsetX, startY + offsetY);
                    offsetY += 24;
                    graphic.DrawString(m.Street, font, dBrush, startX + offsetX, startY + offsetY);
                    offsetY += 24;
                    graphic.DrawString(m.City + ", " + m.State + " " + m.PostalCode, font, dBrush, startX + offsetX, startY + offsetY);
                    totalCount -= 1;
                }
                else if ((Labels.IndexOf(m) % 10 >= 4 && Labels.IndexOf(m) % 10 < 6))
                {
                    offsetY = 400;
                    if (Labels.IndexOf(m) % 10 == 4)
                    {
                        offsetX = 0;
                    }
                    else
                    {
                        offsetX = 425;
                    }
                    graphic.DrawString(m.FirstName.ToString() + " " + m.LastName.ToString(), font, dBrush, startX + offsetX, startY + offsetY);
                    offsetY += 24;
                    graphic.DrawString(m.Street, font, dBrush, startX + offsetX, startY + offsetY);
                    offsetY += 24;
                    graphic.DrawString(m.City + ", " + m.State + " " + m.PostalCode, font, dBrush, startX + offsetX, startY + offsetY);
                    totalCount -= 1;
                }
                else if ((Labels.IndexOf(m) % 10 >= 6 && Labels.IndexOf(m) % 10 < 8))
                {
                    offsetY = 600;
                    if (Labels.IndexOf(m) % 10 == 6)
                    {
                        offsetX = 0;
                    }
                    else
                    {
                        offsetX = 425;
                    }
                    graphic.DrawString(m.FirstName.ToString() + " " + m.LastName.ToString(), font, dBrush, startX + offsetX, startY + offsetY);
                    offsetY += 24;
                    graphic.DrawString(m.Street, font, dBrush, startX + offsetX, startY + offsetY);
                    offsetY += 24;
                    graphic.DrawString(m.City + ", " + m.State + " " + m.PostalCode, font, dBrush, startX + offsetX, startY + offsetY);
                    totalCount -= 1;
                }
                else if (Labels.IndexOf(m) % 10 >= 8)
                {
                    offsetY = 800;
                    if (Labels.IndexOf(m) % 10 == 8)
                    {
                        offsetX = 0;
                    }
                    else
                    {
                        offsetX = 425;
                    }
                    graphic.DrawString(m.FirstName.ToString() + " " + m.LastName.ToString(), font, dBrush, startX + offsetX, startY + offsetY);
                    offsetY += 24;
                    graphic.DrawString(m.Street, font, dBrush, startX + offsetX, startY + offsetY);
                    offsetY += 24;
                    graphic.DrawString(m.City + ", " + m.State + " " + m.PostalCode, font, dBrush, startX + offsetX, startY + offsetY);
                    totalCount -= 1;
                }
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
