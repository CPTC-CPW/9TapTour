using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineTapTour.Models
{
    public class TopScores
    {
        /// <summary>
        /// Class used to populate 3rd RichTextBox
        /// </summary>
        
            public TopScores()
            {

            }
            #region Properties

            public int memberID { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public int Placing { get; set; }
            public int? ScratchTotal { get; set; }
            public int? HandicapScore { get; set; }
            public int? Top3ScratchScore { get; set; }
            public int? Top3HandiScores { get; set; }
            public int? Game1 { get; set; }
            public int? Game2 { get; set; }
            public int? Game3 { get; set; }
            public int? Game4 { get; set; }
            public int? Handicap { get; set; }
            public int Bonus { get; set; }
            public int GameID { get; set; }
            #endregion
            public List<int?> allGameScores()
            {
                var newList = new List<int?>();
                newList.Add(Game1);
                newList.Add(Game2);
                newList.Add(Game3);
                newList.Add(Game4);
                return newList.Where(sc => sc.HasValue).ToList();
            }
        
    }
}
