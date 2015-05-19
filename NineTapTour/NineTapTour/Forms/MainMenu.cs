using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NineTapTour.Forms
{
    public partial class MainMenu : Form
    {
        public MainMenu()
        {
            InitializeComponent();
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnAbout_Click(object sender, EventArgs e)
        {
            // TODO finish 
            MessageBox.Show("9-tap tour inc. is a unique, fun, and professionally run tournament. Members enjoy our Beat the board format, with four games per squad. Yet the fun, big payouts, special pots and the 9 tap version of bowling itself, brings new excitement to tournaments. \n Approximately one bowler in every 5 entries will cash. Other optional ways to cash are: 9 tap Jackpot, Progressive Pot, high game pots, brackets, scratch game and series pot, and more depending on where and when you bowl these Side Pots may vary from time to time. 9 Tap Tour also has BIG added tournaments. Each quarterly Tournament may have eligibility requirements for members who bowl during that Quarter. \n" );
        }

        private void btnMemberData_Click(object sender, EventArgs e)
        {
            this.Hide();
            var form = new FrmMemberData();
            form.Show();
        }

        private void btnMemberScores_Click(object sender, EventArgs e)
        {
            this.Hide();
            var form = new FrmMemberScores();
            form.Show();
            
        }  
    }
}
