using NineTapTour.Database;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace NineTapTour.Forms
{
    public partial class frmMemberScores : Form
    {

        //IOrderedEnumerable<Member> _membersList;
        Member currentMem;
        TextBox[] scratchArray = new TextBox[4];
        TextBox[] handicappArray = new TextBox[4];
        //Count for record counting
        int count = 0;
        int totalCount = 0;
        Participant player = new Participant();

        public frmMemberScores()
        {
            InitializeComponent();
        }

        private void FrmMemberScores_Load(object sender, EventArgs e)
        {
            //_membersList = ((FrmMain)MdiParent)._membersList;
            scratchArray = new TextBox[4] { txtScratchScore1, txtScratchScore2, txtScratchScore3, txtScratchScore4 };
            handicappArray = new TextBox[4] { txtHandicapScore1, txtHandicapScore2, txtHandicapScore3, txtHandicapScore4 };
        }

        /// <summary>
        /// entering a member number clears members data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtMemberNum_TextChanged(object sender, EventArgs e)
        {
            if (currentMem == null || ((TextBox)sender).Text == "")
            {
                txtLastName.Clear();
                txtFirstName.Clear();
                txtMiddleInitial.Clear();
                txtHandicap.Clear();
                txtBonusPins.Clear();
                MemberStatus("", Color.Black, SystemColors.Control, true);
            }
        }

        /// <summary>
        /// clears the forms member scores
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmMemberScores_Activated(object sender, EventArgs e)
        {
            txtMemberNum.Clear();
            txtLastName.Clear();
            txtFirstName.Clear();
            txtMiddleInitial.Clear();
            txtHandicap.Clear();
            txtBonusPins.Clear();
            MemberStatus("", Color.Black, SystemColors.Control, true);
            cbxTourneyDropDown.DataSource = ((FrmMain)MdiParent)._tournamentList;
            cbxTourneyDropDown.DisplayMember = "TourneyNameDate";
            cbxTourneyDropDown.ValueMember = "Id";
        }

        /// <summary>
        /// Gets the members information based on the member number
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GetMember(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
                return;

            string searchNumber = txtMemberNum.Text;
            //if(searchNumber.Trim()=="") return;
            for (int i = 0; i < searchNumber.Length; i++)
            {
                if (!char.IsNumber(searchNumber[i]))
                {
                    MessageBox.Show("Please input numbers only.", "Your Attention Please.");
                    txtMemberNum.Clear();
                    return;
                }
            }
            if (searchNumber.Trim() != "")
            {
                int memberNumber = Convert.ToInt16(txtMemberNum.Text);
                currentMem = ((FrmMain)MdiParent)._membersList.FirstOrDefault(m => m.Number == memberNumber);
                if (currentMem != null)
                {
                    if (currentMem.IsActive)
                    {
                        MemberStatus("Active", Color.Green, Color.Lime, false);
                    }
                    else
                    {
                        //lblMemberStatus.Text = "Inactive";
                        //lblMemberStatus.ForeColor = System.Drawing.Color.Red;
                        //pnlMemStat.BackColor = System.Drawing.Color.Pink;
                        ////Will change later, just for presentation
                        //txtScratchScore1.ReadOnly = true;
                        //txtScratchScore2.ReadOnly = true;
                        //txtScratchScore3.ReadOnly = true;
                        //txtScratchScore4.ReadOnly = true;
                        MemberStatus("Inactive", Color.Red, Color.Pink, true);
                    }
                    txtLastName.Text = currentMem.LastName;
                    txtFirstName.Text = currentMem.FirstName;
                    txtMiddleInitial.Text = currentMem.MiddleInitial;
                    txtHandicap.Text = currentMem.Handicap.ToString();
                    txtBonusPins.Text = currentMem.Bonus.ToString();
                    
                }
                else
                {
                    MessageBox.Show(string.Format("A member with the number {0} does not exist", txtMemberNum.Text), "Your Attention Please.");
                    txtMemberNum.Clear();
                }
            }
        }

        private void FillMember()
        {
            string searchNumber = count.ToString();
            //if(searchNumber.Trim()=="") return;
            for (int i = 0; i < searchNumber.Length; i++)
            {
                if (!char.IsNumber(searchNumber[i]))
                {
                    MessageBox.Show("Please input numbers only.", "Your Attention Please.");
                    txtMemberNum.Clear();
                    return;
                }
            }
            if (searchNumber.Trim() != "")
            {
                int memberNumber = count;
                currentMem = ((FrmMain)MdiParent)._membersList.FirstOrDefault(m => m.Number == memberNumber);
                if (currentMem != null)
                {
                    if (currentMem.IsActive)
                    {
                        MemberStatus("Active", Color.Green, Color.Lime, false);
                    }
                    else
                    {
                        //lblMemberStatus.Text = "Inactive";
                        //lblMemberStatus.ForeColor = System.Drawing.Color.Red;
                        //pnlMemStat.BackColor = System.Drawing.Color.Pink;
                        ////Will change later, just for presentation
                        //txtScratchScore1.ReadOnly = true;
                        //txtScratchScore2.ReadOnly = true;
                        //txtScratchScore3.ReadOnly = true;
                        //txtScratchScore4.ReadOnly = true;
                        MemberStatus("Inactive", Color.Red, Color.Pink, true);
                    }
                    txtLastName.Text = currentMem.LastName;
                    txtFirstName.Text = currentMem.FirstName;
                    txtMiddleInitial.Text = currentMem.MiddleInitial;
                    txtHandicap.Text = currentMem.Handicap.ToString();
                    txtBonusPins.Text = currentMem.Bonus.ToString();
                    txtScratchScore1.Text = Convert.ToString(player.Game.Game1);
                    txtScratchScore2.Text = Convert.ToString(player.Game.Game2);
                    txtScratchScore3.Text = Convert.ToString(player.Game.Game3);
                    txtScratchScore4.Text = Convert.ToString(player.Game.Game4);
                }
                else
                {
                    MessageBox.Show(string.Format("A member with the number {0} does not exist", txtMemberNum.Text), "Your Attention Please.");
                    txtMemberNum.Clear();
                }
            }
        }

        public int GetParticipant()
        {
            NineTapDb db = new NineTapDb();
            int totalCount = (from t in db.Tournaments
                              select t.Participant).Count();
            return totalCount;
        }
        
        /// <summary>
        /// method to set the member status colors on lblMemberStatus forecolor and pnlMemStat background color
        /// </summary>
        /// <param name="text"></param>
        /// <param name="forColor"></param>
        /// <param name="backColor"></param>
        /// <param name="active"></param>
        public void MemberStatus(string text, Color forColor, Color backColor, bool active)
        {
            lblMemberStatus.Text = text;
            lblMemberStatus.ForeColor = forColor;
            pnlMemStat.BackColor = backColor;
            foreach (TextBox scratch in scratchArray)
            {
                scratch.Clear();
                scratch.ReadOnly = active;
            }
        }

        /// <summary>
        /// txtScratchScore 1, 2 ,3, 4 textboxes are added. the result is put into the txtScratchTotal textbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void scratchTotal(object sender, EventArgs e)
        {
            int scratchTotal = 0;
            int cScore = 0;
            string id;
            foreach (TextBox score in scratchArray)
            {
                id = Regex.Match(score.Name, @"\d+").Value;
                if (int.TryParse(score.Text, out cScore))
                {
                    if (cScore >= 0 && cScore <= 300)
                    {
                        scratchTotal += cScore;
                        handicapTotal(id, cScore);
                    }
                    else
                    {
                        MessageBox.Show("Score out of range.", "Error");
                        score.Clear();
                    }
                }
                else
                {
                    score.Clear();
                    handicapTotal(id, cScore);
                }
                txtScratchTotal.Text = scratchTotal.ToString();
            }
            
            //auto tab to the next textbox when textbox1's length is 3.
            if (scratchArray[0].Text.Length == 3)
            {
                scratchArray[1].Focus();
            }
            if (scratchArray[1].Text.Length == 3)
            {
                scratchArray[2].Focus();
            }
            if (scratchArray[2].Text.Length == 3)
            {
                scratchArray[3].Focus();
            }
            if (scratchArray[3].Text.Length == 3)
            {
                btnNew.Focus();
            }
        }

        /// <summary>
        /// finds the handycap score
        /// </summary>
        /// <param name="id"></param>
        /// <param name="score"></param>
        private void handicapTotal(string id, int score)
        {
            int totalScore = 0;
            foreach (TextBox hScore in handicappArray)
            {
                if (hScore.Name.Contains(id))
                {
                    if (score != 0 && txtHandicap.Text != "" && txtBonusPins.Text != "")
                    {
                        hScore.Text = Convert.ToString(score + Convert.ToInt32(txtHandicap.Text) + Convert.ToInt32(txtBonusPins.Text));
                    }
                    else
                    {
                        hScore.Clear();
                    }
                }
                if (hScore.Text != "")
                {
                    totalScore += Convert.ToInt32(hScore.Text);
                }
            }
            txtHandicapTotal.Text = Convert.ToString(totalScore);
        }
        
        /// <summary>
        /// enter a tournamnet participant into a specific tournament
        /// save scores and info in database
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void newRecap(object sender, EventArgs e)
        {
            
            player.Game = new Game();
            player.Game.Score = new List<int> { 
                Convert.ToInt16(scratchArray[0].Text), 
                Convert.ToInt16(scratchArray[1].Text), 
                Convert.ToInt16(scratchArray[2].Text), 
                Convert.ToInt16(scratchArray[3].Text) 
            };
            
            //selects the ID of the combobox of tournaments and stores the
            //tournament property within the participants class.
            int selectedTournamentId = Convert.ToInt32(cbxTourneyDropDown.SelectedValue);
            Tournament selectedTourney = GetTournamentById(selectedTournamentId);
            player.Tournament = selectedTourney;
            player.Game.Member = currentMem;
            player.Game.Game1 = Convert.ToInt16(scratchArray[0].Text);
            player.Game.Game2 = Convert.ToInt16(scratchArray[1].Text);
            player.Game.Game3 = Convert.ToInt16(scratchArray[2].Text);
            player.Game.Game4 = Convert.ToInt16(scratchArray[3].Text);


            #region radio button
            if (rdoSquadOne.Checked)
            {
                player.Squad = 1;
            }
            else if (rdoSquadTwo.Checked)
            {
                player.Squad = 2;
            }
            else if (rdoSquadThree.Checked)
            {
                player.Squad = 3;
            }
            else
            {
                player.Squad = 4;
            }
            #endregion

            player.Member = currentMem;

            try
            {
                player.Member = currentMem;
                TournamentDb.AddMemberToTournament(player);
#if DEBUG
                MessageBox.Show(@"Bowler Added Successfully to Tournament!");
#endif
                count++;

                NineTapDb db = new NineTapDb();
                selectedTournamentId = Convert.ToInt32(cbxTourneyDropDown.SelectedValue);
                totalCount = (from r in db.Tournaments
                              where r.Id == selectedTournamentId
                              select r.Participant).Count();
                lblRecord.Text = "Record " + count + " / " + totalCount;

            }
            catch (MemberAccessException ex)
            {
                MessageBox.Show(ex.Message);

            }
            clear();

        }

        /// <summary>
        /// get a tournament by selected id
        /// </summary>
        /// <param name="selectedTournamentId"></param>
        /// <returns></returns>
        private static Tournament GetTournamentById(int selectedTournamentId)
        {
            Tournament selectedTournament = (from t in TournamentDb.GetTournamentList()
                                             where t.Id == selectedTournamentId
                                             select t
                                                 ).Single();
            return selectedTournament;
        }
        
        /// <summary>
        /// clears txtScratchScores textboxes
        /// </summary>
        private void clear()
        {
            txtScratchScore1.Clear();
            txtScratchScore2.Clear();
            txtScratchScore3.Clear();
            txtScratchScore4.Clear();
            txtMemberNum.Clear();

        }
        
        /// <summary>
        /// increments to the next participant in the tournament
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRightArrow_Click(object sender, EventArgs e)
        {
            totalCount = GetParticipant();
            if (count >= totalCount)
            {
                MessageBox.Show("There are no more players to go to!");
            }
            else
            {
                count++;
                lblRecord.Text = "Record " + count + " / " + totalCount;
                FillMember();
            }

        }

        /// <summary>
        /// decrements to the previous participant in the tournament
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLeftArrow_Click(object sender, EventArgs e)
        {
            totalCount = GetParticipant();
            if (count <= 0)
            {
                MessageBox.Show("There are no more players to go back to!");
            }
            else
            {
                count--;
                lblRecord.Text = "Record " + count + " / " + totalCount;
                FillMember();
            }
        }

        /// <summary>
        /// opens the new tournament form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnNewTournament_Click(object sender, EventArgs e)
        {
            var newfrmNewTournament = Application.OpenForms["frmNewTournament"] as frmNewTournament;
            ((FrmMain)MdiParent).OpenOrDisplayForm(ref newfrmNewTournament);
            newfrmNewTournament.Dock = DockStyle.None;
            rdoSquadOne.Checked = true;
        }
    }
}
