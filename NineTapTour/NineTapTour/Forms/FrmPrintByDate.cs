using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Printing;
using NineTapTour.Database;

namespace NineTapTour.Forms
{
    public partial class FrmPrintByDate : Form
    {
        public FrmPrintByDate()
        {
            InitializeComponent();
        }

        private void dateTimeStart_ValueChanged(object sender, EventArgs e)
        {
            if (dateTimeStart.Value > dateTimeEnd.Value)
            {
                dateTimeStart.Value = dateTimeEnd.Value;
            }
        }

        private void dateTimeEnd_ValueChanged(object sender, EventArgs e)
        {
            if (dateTimeEnd.Value > dateTimeStart.Value)
            {
                dateTimeEnd.Value = dateTimeStart.Value;
            }
        }

        List<Member> mems = new List<Member>();
        private void btnPrint_Click(object sender, EventArgs e)
        {
            //Set up compenents for printing
            PrintDialog printDialog = new PrintDialog();
            PrintDocument printDocument = new PrintDocument();
            //add the document to the dialog box
            printDialog.Document = printDocument;
            //add the event handler that will do the printing
            printDocument.PrintPage += new PrintPageEventHandler(print);
            
            

            DialogResult result = printDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                printDocument.Print();
            }
        }

        private void print(object sender, PrintPageEventArgs e)
        {
            using (NineTapDb db = new NineTapDb())
            {
                List<Tournament> a = (from t in db.Tournaments
                         orderby t.Date descending
                         where t.Date >= dateTimeStart.Value && t.Date <= dateTimeEnd.Value
                         select t).ToList();
            }
            
        }
    }
}
