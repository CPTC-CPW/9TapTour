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
        /// <summary>
        /// Opens the "Main Menu" form.
        /// </summary>
        public MainMenu()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Closes the "Main Menu" form when the "Exit" button is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Exit_Click(object sender, EventArgs e)
        {
            this.MdiParent.Close();
        }
        /// <summary>
        /// Brings up a message box explaining what the 9-Tap Tour is about when the "About" button is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAbout_Click(object sender, EventArgs e)
        {
            // TODO finish 
            MessageBox.Show("9-tap tour inc. is a unique, fun, and professionally run tournament. Members enjoy our Beat the board format, with four games per squad. Yet the fun, big payouts, special pots and the 9 tap version of bowling itself, brings new excitement to tournaments. \n Approximately one bowler in every 5 entries will cash. Other optional ways to cash are: 9 tap Jackpot, Progressive Pot, high game pots, brackets, scratch game and series pot, and more depending on where and when you bowl these Side Pots may vary from time to time. 9 Tap Tour also has BIG added tournaments. Each quarterly Tournament may have eligibility requirements for members who bowl during that Quarter. \n" );
        }
        /// <summary>
        /// Brings up the "Member Data" form when the "Member Data" button is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnMemberData_Click(object sender, EventArgs e)
        {
            ((FrmMain)MdiParent).menuHighlight(btnMemberData.Text);
            ((FrmMain)MdiParent).memberToolStripMenuItem_Click(sender, e);
        }
        /// <summary>
        /// Brings up the "Member Scores" form when the "Member Scores" button is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnMemberScores_Click(object sender, EventArgs e)
        {
            ((FrmMain)MdiParent).menuHighlight(btnMemberScores.Text);
            ((FrmMain)MdiParent).tournamentToolStripMenuItem_Click(sender, e);
        }

        private void MainMenu_Paint(object sender, PaintEventArgs e)
        {
#if DEBUG
            Graphics g = e.Graphics;
            Font drawFont = new Font("Arial", 12);
            SolidBrush drawBrush = new SolidBrush(Color.Red);
            PointF drawPoint = new PointF(10, 2);
            g.DrawString("DEVELOPMENT VERSION NOT FOR PRODUCTION", drawFont, drawBrush, drawPoint);
#endif
        }

        private void MainMenu_Resize(object sender, EventArgs e)
        {
            Console.WriteLine("noot");
        }
    }
}
