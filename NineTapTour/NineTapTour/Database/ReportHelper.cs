using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineTapTour.Database
{
    public class ReportHelper
    {
        public enum ReportType
        {
            HighGameHandicapGameSenior, HighGame, HighSeriesScratch, HighSeriesHandicap
        }
    }

    /// <summary>
    /// Holds data for a single bowler's entry in a tournament
    /// </summary>
    /// <param name="Placing">Place the bowler placed in a given tournament</param>
    /// <param name="Score">Score to display for the report</param>
    /// <param name="MemberNumber">The bowler's member number</param>
    /// <param name="FullName">The bowlers last name followed by first name. ex. Smith, Jane</param>
    public record ReportEntry(int Placing, int Score, int MemberNumber, string FullName);
}
