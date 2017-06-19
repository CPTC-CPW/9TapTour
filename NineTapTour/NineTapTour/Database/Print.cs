using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;
using System.Data.Entity;

namespace NineTapTour.Database
{
    static class Print
    {
        public static void SinglePrint(MemberPrintObj mem, PrintPageEventArgs e)
        {
            //get the total handicap to display on the card when printed
            int totalHandicap = mem.Handicap * 4;


            //This is what prints the data
            Graphics graphic = e.Graphics;

            //default font to use, should use a mono space font so the spaces line up.
            Font font = new Font("Arial", 16, FontStyle.Bold, GraphicsUnit.Pixel);

            //Sets defult brush to use when printing
            SolidBrush dBrush = new SolidBrush(Color.Black);

            int startX = 10;
            int startY = 50;

            //draw handicap and average
            graphic.DrawString(mem.Average, font, dBrush, startX + 490, startY - 8);
            graphic.DrawString(mem.Handicap.ToString(), font, dBrush, startX + 605, startY - 8);
            graphic.DrawString(mem.Bonus.ToString(), font, dBrush, startX + 730, startY - 8);

            //draw the 4 handicaps for the game section of the card and the total handicap
            float offset = 39.55f;
            for (int i = 1; i <= 5; i++)
            {
                //this prints the handicap 4 times.
                if (i <= 4)
                {
                    graphic.DrawString(mem.Handicap.ToString(), font, dBrush, startX + 540, startY + 31 + i * offset);
                }
                //this prints the total handicap after it prints the handicap 4 seperate times
                if (i == 5)
                {
                    graphic.DrawString(totalHandicap.ToString(), font, dBrush, startX + 540, (startY + 50 + i * offset) - 1);
                }
            }
            //create name string containg lastname, firstname.
            string nameString = mem.LastName + ", " + mem.FirstName;
            //draw name string
            graphic.DrawString(nameString, font, dBrush, startX + 5, startY + 80);
            //draw city string
            graphic.DrawString(mem.City, font, dBrush, startX + 5, startY + 121);
            //draw member number string
            graphic.DrawString(mem.Number.ToString(), font, dBrush, startX + 80, startY + 238);
        }

        /************************************************************************
        For Printing the Report Sections
        ************************************************************************/
        public static void ReportPrint(List<Forms.frmMemberScores.MemberScores> temp, Database.Tournament selectedTournament, int reportTypeNum, PrintPageEventArgs e)
        {
            int numToPrint = 40;
            //This is what prints the data
            Graphics graphic = e.Graphics;

            //default font to use, should use a mono space font so the spaces line up.
            Font font = new Font("Arial", 16, FontStyle.Bold, GraphicsUnit.Pixel);
            Font starFont = new Font("Arial", 16.5f, FontStyle.Bold, GraphicsUnit.Pixel);
            Font bigFont = new Font("Arial", 25, FontStyle.Bold, GraphicsUnit.Pixel);

            //Sets defult brush to use when printing
            SolidBrush dBrush = new SolidBrush(Color.Black);

            int startX = 15;
            int startY = 50;

            string tournamentType = "";

            if (selectedTournament.ThreeOutOf4)
            {
                tournamentType = "3of4 ";
            }
            /***********************************************************
             if doubles is working and is needed, uncomment the code below
            ***********************************************************/
            //else if(selectedTournament.Doubles)
            //{
            //    tournamentType = "doubles ";
            //}
            /************************************************************/

            // drawing the location and date
            graphic.DrawString(selectedTournament.Location + " " + tournamentType + string.Format("{0:d-M-yyyy}", selectedTournament.Date), font, dBrush, startX + 10, startY - 19);

            string header = "9 Tap Tour High - ";

            string reportType = "";

            // for drawing the report type using the reportTypeNum
            if (reportTypeNum == 0)
            {
                reportType = "Game Senior";
            }
            else if (reportTypeNum == 1)
            {
                reportType = "Game";
            }
            else
            {
                reportType = "Series";
            }

            // drawing the report title
            graphic.DrawString(header + reportType + " Finals", bigFont, dBrush, startX + 10, startY + 27);

            if (reportTypeNum == 0)
            {
                reportType = "Game";
            }

            // drawing the header of the data
            graphic.DrawString("       " + reportType + "     Mem No       Name", font, dBrush, startX + 8, startY + 133);
            graphic.DrawString(" ***********************************************************", starFont, dBrush, startX + 1, startY + 152);

            for (int i = 0; i < temp.Count - (index * 40) && i < numToPrint; i++)
            {
                //draw number for what place they are
                graphic.DrawString((i + 1 + (index * 40)).ToString(), font, dBrush, startX + 6, startY + 173 + (i * 19));

                //draw Score
                graphic.DrawString(temp[i + (index * 40)].Score.ToString(), font, dBrush, startX + 48, startY + 173 + (i * 19));

                //draw the member number
                graphic.DrawString(temp[i + (index * 40)].MemberNo.ToString(), font, dBrush, startX + 120, startY + 173 + (i * 19));

                string unpaid = "";
                if(!temp[i + (index * 40)].Paid)
                {
                    unpaid = "X";
                }

                //create name string containg lastname, firstname, and last payment
                string nameString = temp[i + (index * 40)].LastName + ", " + temp[i + (index * 40)].FirstName + "     " + temp[i + (index * 40)].LastPaymentYear + " " + unpaid;

                //draw name string
                graphic.DrawString(nameString, font, dBrush, startX + 200, startY + 173 + (i * 19));
            }
        }

        static public void printMemberReport(List<Forms.frmMemberScores.MemberScores> temp, Database.Tournament selectedTournament, int reportTypeNum)
        {
            Print.temp = temp;
            Print.selectedTournament = selectedTournament;
            Print.reportTypeNum = reportTypeNum;

            // Set up compenents for printing
            PrintDialog printDialog = new PrintDialog();
            PrintDocument printDocument = new PrintDocument();
            //add the document to the dialog box
            printDialog.Document = printDocument;

            //add the event handler that will do the printing
            printDocument.PrintPage += new PrintPageEventHandler(printReport);

            if (temp.Count > 0)
            {
                DialogResult result = printDialog.ShowDialog();
                if (result == DialogResult.OK)
                {
                    printDocument.Print();
                }
            }
            index = 0;
        }

        static private void printReport(object sender, PrintPageEventArgs e)
        {
            ReportPrint(temp, selectedTournament, reportTypeNum, e);
            index++;
            e.HasMorePages = ((index * 40) < temp.Count);
        }

        static List<Forms.frmMemberScores.MemberScores> temp = new List<Forms.frmMemberScores.MemberScores>();//for High score
        static Tournament selectedTournament;
        static int reportTypeNum;
        /************************************************************************/

        static List<Member> mems = new List<Member>();
        static int index = 0;

        // This is for the printbytourney button
        static public void printByTour(Tournament tour)
        {
            //Set up compenents for printing
            PrintDialog printDialog = new PrintDialog();
            PrintDocument printDocument = new PrintDocument();
            //add the document to the dialog box
            printDialog.Document = printDocument;

            //add the event handler that will do the printing
            printDocument.PrintPage += new PrintPageEventHandler(printTourRecaps);
            mems = TournamentDb.GetUniqueTourMembers(tour);

            if (mems.Count > 0)
            {
                DialogResult result = printDialog.ShowDialog();
                if (result == DialogResult.OK)
                {
                    printDocument.Print();
                }
            }
            index = 0;
        }

        // The fetching was moved to the print button, but I'll leave this code here in case it's ever needed.
        /*
        static public void printByTourDate(DateTime start, DateTime end)
        {
            //Set up compenents for printing
            PrintDialog printDialog = new PrintDialog();
            PrintDocument printDocument = new PrintDocument();
            //add the document to the dialog box
            printDialog.Document = printDocument;

            //add the event handler that will do the printing
            printDocument.PrintPage += new PrintPageEventHandler(printTourRecaps);

            mems = TournamentDb.GetUniqueTourMembersByDate(start, end);
            if (mems.Count > 0)
            {
                DialogResult result = printDialog.ShowDialog();
                if (result == DialogResult.OK)
                {
                    printDocument.Print();
                }
            }
            index = 0;
        }
        */

        static public void printAllMembers()
        {
            // Set up compenents for printing
            PrintDialog printDialog = new PrintDialog();
            PrintDocument printDocument = new PrintDocument();
            //add the document to the dialog box
            printDialog.Document = printDocument;

            //add the event handler that will do the printing
            printDocument.PrintPage += new PrintPageEventHandler(printTourRecaps);

            using (NineTapDb db = new NineTapDb())
            {
                mems = (from m in db.Members
                        orderby m.LastName descending
                        select m).Take(1).ToList();
            }

            if (mems.Count > 0)
            {
                DialogResult result = printDialog.ShowDialog();
                if (result == DialogResult.OK)
                {
                    printDocument.Print();
                }
            }

            index = 0;
        }

        static public void printByMemberList(List<Member> members)
        {
            //Set up compenents for printing
            PrintDialog printDialog = new PrintDialog();
            PrintDocument printDocument = new PrintDocument();
            //add the document to the dialog box
            printDialog.Document = printDocument;

            //add the event handler that will do the printing
            printDocument.PrintPage += new PrintPageEventHandler(printTourRecaps);

            mems = members;

            if (mems.Count > 0)
            {
                DialogResult result = printDialog.ShowDialog();
                if (result == DialogResult.OK)
                {
                    printDocument.Print();
                }
            }
            index = 0;
        }

        static private void printTourRecaps(object sender, PrintPageEventArgs e)
        {
            NineTapTour.Database.Print.SinglePrint(new MemberPrintObj(mems[index]), e);
            index++;
            e.HasMorePages = (index < mems.Count);
        }
    }

    class MemberPrintObj
    {
        public MemberPrintObj(int handicap, int memberNumber, string city, string firstName, string lastName, string average, int bonus)
        {
            Handicap = handicap;
            Number = memberNumber.ToString();
            City = city;
            FirstName = firstName;
            LastName = lastName;
            Average = average;
            Bonus = bonus;
        }

        public MemberPrintObj(Member mem)
        {
            Handicap = (mem.Handicap != null) ? (int) mem.Handicap : 0;
            Number = mem.Number.ToString();
            City = mem.City;
            FirstName = mem.FirstName;
            LastName = mem.LastName;
            /**************************************************************
            edited this part because it used to say Average = (mem.Average != null) ? mem.Handicap.ToString() : "";

            and added the bonus because there was no code for it(still not sure if I should add it)
            Check if it is a good code
            ***************************************************************/
            Average = (mem.Average != null) ? mem.Average.ToString() : "";
            Bonus = (mem.Bonus != null) ? mem.Bonus.Value : 0;//mem.Bonus;
            /*************************************************************/
            //Bonus pins default to 0 on the recap for all recaps printed.

        }

        public int Handicap;
        public string Number;
        public string City;
        public string LastName;
        public string FirstName;
        public string Average;
        public int Bonus;
    }
}
