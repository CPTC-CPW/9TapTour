using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;
using System.Data.Entity;
using NineTapTour.Models;
using static NineTapTour.Database.ReportHelper;

namespace NineTapTour.Database
{
    static class Print
    {
        public static void SinglePrint(MemberPrintObj mem, PrintPageEventArgs e)
        {
            //get the total handicap to display on the card when printed
            int AmtOfTimesHandicapApplied = 4;
            int totalHandicap = mem.Handicap * AmtOfTimesHandicapApplied; 

            //This is what prints the data
            Graphics graphic = e.Graphics;

            //default font to use, should use a mono space font so the spaces line up.
            Font font = new Font("Arial", 16, FontStyle.Bold, GraphicsUnit.Pixel);

            //Sets default brush to use when printing
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
                if (i <= AmtOfTimesHandicapApplied)
                {
                    graphic.DrawString(mem.Handicap.ToString(), font, dBrush, startX + 540, startY + 31 + i * offset);
                }
                //this prints the total handicap after it prints the handicap 4 separate times
                if (i == AmtOfTimesHandicapApplied + 1)
                {
                    graphic.DrawString(totalHandicap.ToString(), font, dBrush, startX + 540, (startY + 50 + i * offset) - 1);
                }
            }
            //create name string containing lastname, firstname.
            string nameString = mem.LastName + ", " + mem.FirstName;
            //draw name string
            graphic.DrawString(nameString, font, dBrush, startX + 5, startY + 80);
            //draw city string
            graphic.DrawString(mem.City, font, dBrush, startX + 5, startY + 121);
            //draw member number string
            graphic.DrawString(mem.Number.ToString(), font, dBrush, startX + 80, startY + 238);
        }

        /// <summary>
        /// For Printing the Report Sections
        /// </summary>
        public static void ReportPrint(List<Models.MemberScores> tempMemberList, Tournament selectedTournament, ReportType reportTypeNum, PrintPageEventArgs e, int? manualCutoff = null)
        {
            int numToPrint = 40;
            // This var is used to draw a line after the rows of money-winning members are printed
            int winningPlaces;
            if (tempMemberList.Count() < 5)
            {
                winningPlaces = 5;
            }
            else
            {
                winningPlaces = tempMemberList.Count() / 5;
            }
            //This is what prints the data
            Graphics graphic = e.Graphics;

            //default font to use, should use a mono space font so the spaces line up.
            Font font = new Font("Arial", 16, FontStyle.Bold, GraphicsUnit.Pixel);
            Font starFont = new Font("Arial", 16.5f, FontStyle.Bold, GraphicsUnit.Pixel);
            Font bigFont = new Font("Arial", 25, FontStyle.Bold, GraphicsUnit.Pixel);

            //Sets default brush to use when printing
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

            // drawing the location and date(Month, Day, Year, e.g. May 13th 2019 = 5-13-2019
            graphic.DrawString(selectedTournament.Location + " " + tournamentType + string.Format("{0:M-d-yyyy}", selectedTournament.Date), font, dBrush, startX + 10, startY - 19);

            string header = "9 Tap Tour High - ";

            string reportType = "";

            // for drawing the report type using the reportTypeNum
            if (reportTypeNum == ReportType.HighGameSenior)
            {
                reportType = "Game Senior";
            }
            else if (reportTypeNum == ReportType.HighGame)
            {
                reportType = "Game";
            }
            else if(reportTypeNum == ReportType.HighSeries)
            {
                //The 'Through squad x' header is only drawn for Series Reports
                if (squadList[0] == 0) //'All Squads' is checked
                {
                    graphic.DrawString("Final", bigFont, dBrush, startX + 250, startY + 70);
                }
                else //A different squad is checked.
                {
                    //create helper ints and bool
                    int min = squadList[0];
                    int max = squadList[squadList.Count - 1];
                    string list = string.Join(",", squadList.ToArray());
                    bool consective = true;
                    
                    
                    
                    if(squadList.Count == 1) //if one squad
                    {
                        if(min == 1) // checks for squad 1 is test for progression based filter
                        {
                            graphic.DrawString("Through Squad " + min, bigFont, dBrush, startX + 250, startY + 70);
                        }
                        else
                        {
                            graphic.DrawString("Squad " + min, bigFont, dBrush, startX + 250, startY + 70);
                        }
                    }
                    else //if more then one squad
                    {
                        //test to see if squads giving are consecutive
                        for(int i = 1; i < squadList.Count; i++)
                        {
                            if(squadList[i] - squadList[i-1] != 1)
                            {
                                consective = false;
                            }
                        }

                        if(squadList.Count == 2) //if filtering two squads
                        {
                            if(consective) //Calls if bool consecutive is true
                            {
                                if(min == 1)
                                {
                                    graphic.DrawString("Through Squad " + max, bigFont, dBrush, startX + 250, startY + 70);
                                }
                                else
                                {
                                graphic.DrawString("Squads " + min + " Through " + max, bigFont, dBrush, startX + 250, startY + 70);
                                }
                            }
                            else // if bool not true
                            {
                                graphic.DrawString("Squad " + min + " and " + max, bigFont, dBrush, startX + 250, startY + 70);
                            }
                        }
                        else // if three or more squads being filtered
                        {
                            if(consective)
                            {
                                if(min == 1)
                                {
                                    graphic.DrawString("Through squad" + max, bigFont, dBrush, startX + 250, startY + 70);
                                }
                                else
                                {
                                     graphic.DrawString("Squads " + min + " Through " + max, bigFont, dBrush, startX + 250, startY + 70);
                                }
                            }
                            else
                            {
                                graphic.DrawString("Squads " + list, bigFont, dBrush, startX + 250, startY + 70); //how to print a list?
                            }
                        }

                    }
                }
                reportType = "Series";
            }

            //If Series button was clicked, should not say final based on qual by squad, rather by Filter Series by Squad. Still shows qual by squad filters on the listed players.
            if (currentSquad == 0 && string.Equals(reportType, "Series"))
            {
                graphic.DrawString(header + reportType + " Standings", bigFont, dBrush, startX + 10, startY + 27);
            }
            // drawing the report title
            else if (currentSquad == 0)
            {
                graphic.DrawString(header + reportType + " Final Standings", bigFont, dBrush, startX + 10, startY + 27);
            }
            else
            {
                graphic.DrawString(header + reportType + "     Squad "  + currentSquad + " Standings " , bigFont, dBrush, startX + 10, startY + 27);
            }


            if (reportTypeNum == 0)
            {
                reportType = "Game";
            }

            // drawing the header of the data
            if (printDues)
            {
                graphic.DrawString("       " + reportType + "     Mem No       Name                                  Membership Paid To", font, dBrush, startX + 8, startY + 133);
            }
            else {
                graphic.DrawString("       " + reportType + "     Mem No       Name", font, dBrush, startX + 8, startY + 133);
            }
            graphic.DrawString(" **************************************************************************************************", starFont, dBrush, startX + 1, startY + 152);

            for (int i = 0; i < tempMemberList.Count - (index * 40) && i < numToPrint; i++)
            {
                //draw number for what place they are
                graphic.DrawString((tempMemberList[i + (index * 40)].placing).ToString(), font, dBrush, startX + 6, startY + 173 + (i * 19));

                //draw Score
                graphic.DrawString(tempMemberList[i + (index * 40)].Score.ToString(), font, dBrush, startX + 48, startY + 173 + (i * 19));

                //draw the member number
                graphic.DrawString(tempMemberList[i + (index * 40)].MemberId.ToString(), font, dBrush, startX + 120, startY + 173 + (i * 19));

                // Decides if the last date the member paid their dues prints on the page
                string unpaid = string.Empty;

                // Gets lastPaymentYear, and adds one year
                string lastPaymentYear = tempMemberList[i + (index * 40)].LastPaymentYear;
                int year;
                int.TryParse(lastPaymentYear, out year);
                year += 1;

                //handle members that don't have payment information
                if (printDues && string.IsNullOrWhiteSpace(tempMemberList[i +(index * 40)].LastPaymentYear))
                {
                    unpaid = "N/A";
                }
                else if(printDues && lastPaymentYear.Equals("life "))
                {
                    unpaid = tempMemberList[i + (index * 40)].LastPaymentYear;
                } else if(printDues)
                {
                    unpaid = Convert.ToString(year);
                }

                //create name string containing lastname, firstname, and last payment
                //Changed: instead of showing last payment every time it shows the year as the "unpaid"
                string nameString = tempMemberList[i + (index * 40)].LastName + ", " + tempMemberList[i + (index * 40)].FirstName;

                //draw name string
                graphic.DrawString(nameString, font, dBrush, startX + 200, startY + 173 + (i * 19));

                //draw Membership Paid Through Column
                graphic.DrawString(unpaid, font, dBrush, startX + 500, startY + 173 + (i * 19));

                // Print a line after 20 percent of the members have been printed.
                if ((manualCutoff.HasValue && i == manualCutoff) || (!manualCutoff.HasValue && i == winningPlaces))
                {
                    int x1 = startX;
                    int y1 = startY + 173 + (i * 19);
                    int x2 = 800;
                    int y2 = y1;
                    
                    Pen redPen = new Pen(Brushes.Red, 3);
                    graphic.DrawLine(redPen, x1, y1, x2, y2);
                }
            }
        }

        static public void PrintMemberReport(List<Models.MemberScores> temp, Tournament selectedTournament, ReportType reportTypeNum, int currentSquad, List<int> squadList, bool printDues, int? manualCutoff)
        {
            Print.temp = temp;
            Print.selectedTournament = selectedTournament;
            Print.reportTypeNum = reportTypeNum;
            Print.currentSquad = currentSquad;
            Print.squadList = squadList;
            Print.printDues = printDues;
            Print.manualCutoff = manualCutoff;

            // Set up components for printing
            PrintDialog printDialog = new PrintDialog();
            PrintDocument printDocument = new PrintDocument();
            //add the document to the dialog box
            printDialog.Document = printDocument;

            //add the event handler that will do the printing
            printDocument.PrintPage += new PrintPageEventHandler(PrintReport);

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

        static private void PrintReport(object sender, PrintPageEventArgs e)
        {
            ReportPrint(temp, selectedTournament, reportTypeNum, e, manualCutoff);
            index++;
            e.HasMorePages = ((index * 40) < temp.Count);
        }

        static List<MemberScores> temp = new List<MemberScores>();//for High score
        static Tournament selectedTournament;
        static ReportType reportTypeNum;
        static int currentSquad;
        static List<int> squadList;
        static bool printDues;
        static int? manualCutoff;
        /************************************************************************/

        static List<Member> mems = new List<Member>();
        static int index = 0;

        // This is for the printbytourney button
        static public void PrintByTour(Tournament tour)
        {
            //Set up components for printing
            PrintDialog printDialog = new PrintDialog();
            PrintDocument printDocument = new PrintDocument();
            //add the document to the dialog box
            printDialog.Document = printDocument;

            //add the event handler that will do the printing
            printDocument.PrintPage += new PrintPageEventHandler(PrintTourRecaps);
            mems = TournamentDB.GetUniqueTourMembers(tour);

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
            //Set up components for printing
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

        static public void PrintAllMembers()
        {
            // Set up components for printing
            PrintDialog printDialog = new PrintDialog();
            PrintDocument printDocument = new PrintDocument();
            //add the document to the dialog box
            printDialog.Document = printDocument;

            //add the event handler that will do the printing
            printDocument.PrintPage += new PrintPageEventHandler(PrintTourRecaps);

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

        static public void PrintByMemberList(List<Member> members)
        {
            //Set up components for printing
            PrintDialog printDialog = new PrintDialog();
            PrintDocument printDocument = new PrintDocument();
            //add the document to the dialog box
            printDialog.Document = printDocument;

            //add the event handler that will do the printing
            printDocument.PrintPage += new PrintPageEventHandler(PrintTourRecaps);

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

        static public void PrintByActiveMembers(List<Member> members)
        {
            PrintDialog printDialog = new PrintDialog();
            PrintDocument printDocument = new PrintDocument();
            printDialog.Document = printDocument;

            printDocument.PrintPage += new PrintPageEventHandler(PrintActiveRecaps);
            mems = members;

            if (mems.Count > 0)
            {
                DialogResult mboxResult =
                        MessageBox.Show($"You are about to print {mems.Count} active members! Are you sure you want to continue?",
                                            "Confirming Prints", MessageBoxButtons.YesNo);
                if (mboxResult == DialogResult.Yes)
                {
                    DialogResult result = printDialog.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        printDocument.Print();
                    }
                }
            }
            index = 0;
        }

        static private void PrintActiveRecaps(object sender, PrintPageEventArgs e)
        {
            SinglePrint(new MemberPrintObj(mems[index]), e);
            index++;
            e.HasMorePages = (index < mems.Count);
        }

        static private void PrintTourRecaps(object sender, PrintPageEventArgs e)
        {
            SinglePrint(new MemberPrintObj(mems[index]), e);
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
            //Bonus = (mem.Bonus != null) ? mem.Bonus.Value : 0;//mem.Bonus;
            Bonus = mem.Bonus;//mem.Bonus;
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
