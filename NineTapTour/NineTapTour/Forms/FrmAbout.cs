using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NineTapTour.Forms {

    public partial class FrmAbout : Form {
        public FrmAbout() {
            InitializeComponent();

            rtbFrmAboutText.Enabled = true; 
            rtbFrmAboutText.TabStop = false;

            /* Fills the rtbFrmAbout text box with the information below */
            rtbFrmAboutText.Text = "9 -Tap Tour Inc. is a unique, fun, and professionally run tournament. " +
                "Members enjoy our \"Beat the Board\" format, with four games per squad. The fun, big payouts, " +
                "special pots, and the 9-Tap version of bowling itself, brings new excitement to tournaments. \n \n " +
                "Approximately one bowler in every 5 entries will cash out. Other optional ways to cash are: 9-Tap " +
                "Jackpots, Progressive Pots, High Game Pots, Brackets, Scratch Games, Series Pots, and more; " +
                "Depending on where and when you bowl, these Side Pots may vary from time to time. 9-Tap Tour " +
                "also has BIG added tournaments. Each quarterly tournament may have eligibility requirements for " +
                "members who bowl during that quarter. \n";
            rtbFrmAboutText.ReadOnly = true;
        }

    }
}
