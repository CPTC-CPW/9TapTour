using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;

namespace NineTapTour.Database
{
    static class Print
    {
        public static void SinglePrint(MemberPrintObj mem, PrintPageEventArgs e)
        {
            //get the total handicap to display on the card when printed
            int totalHandicap = mem.Handicap* 4;
            

            //This is what prints the data
            Graphics graphic = e.Graphics;

            //default font to use, should use a mono space font so the spaces line up.
            Font font = new Font("Arial", 16, FontStyle.Bold, GraphicsUnit.Pixel);

            //Sets defult brush to use when printing
            SolidBrush dBrush = new SolidBrush(Color.Black);

            int startX = 10;
            int startY = 50;

            //draw handicap and average
            graphic.DrawString(mem.Average, font, dBrush, startX + 490, startY - 5);
            graphic.DrawString(mem.Handicap.ToString(), font, dBrush, startX + 590, startY - 5);

            //draw the 4 handicaps for the game section of the card and the total handicap
            float offset = 39.55f;
            for (int i = 1; i <= 5; i++)
            {
                //this prints the handicap 4 times.
                if (i <= 4)
                {
                    graphic.DrawString(mem.Handicap.ToString(), font, dBrush, startX + 530, startY + 31 + i * offset);
                }
                //this prints the total handicap after it prints the handicap 4 seperate times
                if (i == 5)
                {
                    graphic.DrawString(totalHandicap.ToString(), font, dBrush, startX + 530, startY + 50 + i * offset);
                }
            }
            //create name string containg lastname, firstname.
            string nameString = mem.LastName + ", " + mem.FirstName;
            //draw name string
            graphic.DrawString(nameString, font, dBrush, startX + 5, startY + 80);
            //draw city string
            graphic.DrawString(mem.City, font, dBrush, startX + 5, startY + 122);
            //draw member number string
            graphic.DrawString(mem.Number.ToString(), font, dBrush, startX + 80, startY + 235);
        }

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

            List<Participant> tempParts = TournamentDb.GetTournamentMemberList(tour);

            bool add;
            foreach (Participant p in tempParts)
            {
                add = true;
                foreach (Member m in mems)
                {
                    if (m.Id == p.Member.Id)
                    {
                        add = false;
                        break;
                    }
                }
                if (add)
                {
                    mems.Add(p.Member);
                }
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

        static private void printTourRecaps(object sender, PrintPageEventArgs e)
        {
            NineTapTour.Database.Print.SinglePrint(new MemberPrintObj(mems[index]), e);
            index++;
            e.HasMorePages = (index < mems.Count);
        }
    }

    class MemberPrintObj
    {
        public MemberPrintObj(int handicap, int memberNumber, string city, string firstName, string lastName, string average)
        {
            Handicap = handicap;
            Number = memberNumber.ToString();
            City = city;
            FirstName = firstName;
            LastName = lastName;
            Average = average;
        }

        public MemberPrintObj(Member mem)
        {
            Handicap = (mem.Handicap != null) ? (int)mem.Handicap : 0;
            Number = mem.Number.ToString();
            City = mem.City;
            FirstName = mem.FirstName;
            LastName = mem.LastName;
            Average = (mem.Average != null) ? mem.Handicap.ToString() : "";
        }
        public int Handicap;
        public string Number;
        public string City;
        public string LastName;
        public string FirstName;
        public string Average;
    }
}
