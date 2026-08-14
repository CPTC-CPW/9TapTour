using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;
using NineTapTour.Core.Abstractions;
using NineTapTour.Core.Entities;
using NineTapTour.Core.Models;
using NineTapTour.Core.Printing;
using static NineTapTour.Core.Calculations.ReportHelper;

namespace NineTapTour.Database
{
    /// <summary>
    /// Draws print content computed by <see cref="PrintContentBuilder"/>:
    /// this class owns the PrintDocument/PrintDialog handling, fonts,
    /// Graphics coordinates, and page-event state only.
    /// </summary>
    static class Print
    {
        public static void SinglePrint(RecapCardContent card, PrintPageEventArgs e)
        {
            // The handicap is drawn once per game before the total
            int AmtOfTimesHandicapApplied = 4;

            //This is what prints the data
            Graphics graphic = e.Graphics;

            //default font to use, should use a mono space font so the spaces line up.
            Font font = new("Arial", 16, FontStyle.Bold, GraphicsUnit.Pixel);

            //Sets default brush to use when printing
            SolidBrush dBrush = new(Color.Black);

            int startX = 10;
            int startY = 50;

            //draw handicap and average
            graphic.DrawString(card.AverageText, font, dBrush, startX + 490, startY - 8);
            graphic.DrawString(card.HandicapText, font, dBrush, startX + 605, startY - 8);
            graphic.DrawString(card.BonusText, font, dBrush, startX + 730, startY - 8);

            //draw the 4 handicaps for the game section of the card and the total handicap
            float offset = 39.55f;
            for (int i = 1; i <= 5; i++)
            {
                //this prints the handicap 4 times.
                if (i <= AmtOfTimesHandicapApplied)
                {
                    graphic.DrawString(card.HandicapText, font, dBrush, startX + 540, startY + 31 + i * offset);
                }
                //this prints the total handicap after it prints the handicap 4 separate times
                if (i == AmtOfTimesHandicapApplied + 1)
                {
                    graphic.DrawString(card.TotalHandicapText, font, dBrush, startX + 540, (startY + 50 + i * offset) - 1);
                }
            }
            //draw name string
            graphic.DrawString(card.NameLine, font, dBrush, startX + 5, startY + 80);
            //draw city string
            graphic.DrawString(card.CityLine, font, dBrush, startX + 5, startY + 121);
            //draw member number string
            graphic.DrawString(card.MemberNumberText, font, dBrush, startX + 80, startY + 238);
        }

        /// <summary>
        /// For Printing the Report Sections
        /// </summary>
        public static void ReportPrint(MemberReportContent content, int pageIndex, PrintPageEventArgs e)
        {
            //This is what prints the data
            Graphics graphic = e.Graphics;

            //default font to use, should use a mono space font so the spaces line up.
            Font font = new("Arial", 16, FontStyle.Bold, GraphicsUnit.Pixel);
            Font starFont = new("Arial", 16.5f, FontStyle.Bold, GraphicsUnit.Pixel);
            Font bigFont = new("Arial", 25, FontStyle.Bold, GraphicsUnit.Pixel);

            //Sets default brush to use when printing
            SolidBrush dBrush = new(Color.Black);

            int startX = 15;
            int startY = 50;

            // drawing the location and date(Month, Day, Year, e.g. May 13th 2019 = 5-13-2019
            graphic.DrawString(content.TournamentLine, font, dBrush, startX + 10, startY - 19);

            //The 'Through squad x' header is only drawn for Series Reports
            if (content.SeriesSubtitle != null)
            {
                graphic.DrawString(content.SeriesSubtitle, bigFont, dBrush, startX + 250, startY + 70);
            }

            // drawing the report title
            graphic.DrawString(content.Title, bigFont, dBrush, startX + 10, startY + 27);

            // drawing the header of the data
            graphic.DrawString(content.ColumnHeaderLine, font, dBrush, startX + 8, startY + 133);
            graphic.DrawString(" **************************************************************************************************", starFont, dBrush, startX + 1, startY + 152);

            ReportPageContent page = content.Pages[pageIndex];
            for (int i = 0; i < page.Rows.Count; i++)
            {
                ReportRowContent row = page.Rows[i];

                //draw number for what place they are
                graphic.DrawString(row.Placing, font, dBrush, startX + 6, startY + 173 + (i * 19));

                //draw Score
                graphic.DrawString(row.Score, font, dBrush, startX + 48, startY + 173 + (i * 19));

                //draw the member number
                graphic.DrawString(row.MemberNumber, font, dBrush, startX + 120, startY + 173 + (i * 19));

                //draw name string
                graphic.DrawString(row.Name, font, dBrush, startX + 230, startY + 173 + (i * 19));

                //draw Membership Paid Through Column
                graphic.DrawString(row.DuesText, font, dBrush, startX + 500, startY + 173 + (i * 19));

                // Print the cutoff line after the last row of money-winning members
                if (page.CutoffAfterRowIndex == i)
                {
                    PrintCutoffLine(graphic, startX, startY, i);
                }
            }
        }

        private static void PrintCutoffLine(Graphics graphic, int startX, int startY, int i)
        {
            int x1 = startX;
            int y1 = startY + 173 + ((i + 1) * 19);
            int x2 = 800;
            int y2 = y1;

            Pen redPen = new(Brushes.Red, 3);
            graphic.DrawLine(redPen, x1, y1, x2, y2);
        }

        static public void PrintMemberReport(List<MemberScores> temp, Tournament selectedTournament, ReportType reportTypeNum, int currentSquad, List<int> squadList, bool printDues, int? manualCutoff)
        {
            reportContent = PrintContentBuilder.BuildMemberReport(temp, selectedTournament, reportTypeNum, currentSquad, squadList, printDues, manualCutoff);

            // Set up components for printing
            PrintDialog printDialog = new();
            PrintDocument printDocument = new();
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
            ReportPrint(reportContent, index, e);
            index++;
            e.HasMorePages = (index < reportContent.Pages.Count);
        }

        static MemberReportContent reportContent;//for High score
        /************************************************************************/

        static List<Member> mems = [];
        static int index = 0;

        // This is for the printbytourney button. The caller supplies the
        // tournament's unique members so this class stays free of data access.
        static public void PrintByTour(Tournament tour, List<Member> tourMembers)
        {
            //Set up components for printing
            PrintDialog printDialog = new();
            PrintDocument printDocument = new();
            //add the document to the dialog box
            printDialog.Document = printDocument;

            //add the event handler that will do the printing
            printDocument.PrintPage += new PrintPageEventHandler(PrintTourRecaps);

            // Client wants the recaps ordered by last name first
            mems = PrintContentBuilder.OrderMembersForRecaps(tourMembers);

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
            PrintDialog printDialog = new();
            PrintDocument printDocument = new();
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

        static public void PrintByActiveMembers(List<Member> members, IMessageService messageService)
        {
            PrintDialog printDialog = new();
            PrintDocument printDocument = new();
            printDialog.Document = printDocument;

            printDocument.PrintPage += new PrintPageEventHandler(PrintActiveRecaps);
            mems = members;

            if (mems.Count > 0)
            {
                bool confirmed = messageService.Confirm(
                    $"You are about to print {mems.Count} active members! Are you sure you want to continue?",
                    "Confirming Prints");
                if (confirmed)
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
            SinglePrint(PrintContentBuilder.BuildRecapCard(mems[index]), e);
            index++;
            e.HasMorePages = (index < mems.Count);
        }

        static private void PrintTourRecaps(object sender, PrintPageEventArgs e)
        {
            SinglePrint(PrintContentBuilder.BuildRecapCard(mems[index]), e);
            index++;
            e.HasMorePages = (index < mems.Count);
        }
    }
}
