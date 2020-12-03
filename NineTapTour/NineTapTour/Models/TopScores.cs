using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineTapTour.Models
{
    /// <summary>
    /// This class could populate Filter by Current High Series 
    /// of the 3rd text box in Series [Member No.] -- (Name)
    /// 
    /// Will add more code later to fix the 3rd Label box in FrmMemberScores
    /// </summary>
    public class TopScores
    {
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

        
    }
}
