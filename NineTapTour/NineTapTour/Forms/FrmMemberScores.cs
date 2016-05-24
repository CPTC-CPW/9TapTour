using NineTapTour.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Data.Entity;


namespace NineTapTour.Forms
{
    public partial class frmMemberScores : Form
    {
        //IOrderedEnumerable<Member> _membersList;
        Member currentMem;
        Member currentMem2;
        TextBox[] scratchArray = new TextBox[4];
        TextBox[] handicappArray = new TextBox[4];
        int currentIndex = 0;         //Count for record counting
        Participant player = new Participant();
        Participant player2 = new Participant();
        bool doubles = true;

        public frmMemberScores()
        {
            InitializeComponent();
        }

        private void FrmMemberScores_Load(object sender, EventArgs e)
        {
            txtMemberNum2.Visible = doubles;
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
            cbxTourneyDropDown.SelectedIndex = 0;
            List<Tournament> temp2 = TournamentDb.GetTournamentList();
            if (temp2.Count() > 0)
            {
                var item = temp2.Max(x => x.Id);
                cbxTourneyDropDown.SelectedValue = item;
            }
        }

        /// <summary>
        /// Gets the members information based on the member number
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GetMember(object sender, KeyEventArgs e)
        {
            Tournament currTourney = GetTournamentById(Convert.ToInt32(cbxTourneyDropDown.SelectedValue));
            if(currTourney.Doubles)
            {
                //if (e.KeyCode != Keys.Enter) // manually press Enter to populate Names
                //    return;

                string searchNumber = txtMemberNum.Text;
                for (int i = 0; i < searchNumber.Length; i++)
                {
                    if (!char.IsNumber(searchNumber[i]))
                    {
                        MessageBox.Show("Please input numbers only.", "Your Attention Please.");
                        txtMemberNum.Clear();
                        txtMemberNum2.Clear();
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
                            MemberStatus("Inactive", Color.Red, Color.Pink, true);
                        }
                        txtScratchScore1.Focus();
                        Game currentGame = GetScoresById(currentMem.Id);
                        if (currentGame != null)
                        {
                            currentGame.Bonus = currentMem.Bonus;
                            currentGame.Handicap = currentMem.Handicap;
                            txtScratchScore1.Text = Convert.ToString(currentGame.Game1);
                            txtScratchScore2.Text = Convert.ToString(currentGame.Game2);
                        }
                    }
                    else
                    {
                        MessageBox.Show(string.Format("A member with the number {0} does not exist", txtMemberNum.Text), "Your Attention Please.");
                        txtMemberNum.Clear();
                        txtMemberNum2.Clear();
                    }
                }

                string searchNumber2 = txtMemberNum2.Text;
                for (int i = 0; i < searchNumber2.Length; i++)
                {
                    if (!char.IsNumber(searchNumber2[i]))
                    {
                        MessageBox.Show("Please input numbers only.", "Your Attention Please.");
                        txtMemberNum2.Clear();
                        txtMemberNum2.Clear();
                        return;
                    }
                }
                if (searchNumber2.Trim() != "")
                {
                    int memberNumber2 = Convert.ToInt16(txtMemberNum2.Text);
                    currentMem2 = ((FrmMain)MdiParent)._membersList.FirstOrDefault(m => m.Number == memberNumber2);
                    if (currentMem2 != null)
                    {
                        if (currentMem2.IsActive)
                        {
                            MemberStatus("Active", Color.Green, Color.Lime, false);
                        }
                        else
                        {
                            MemberStatus("Inactive", Color.Red, Color.Pink, true);
                        }
                        Game currentGame2 = GetScoresById(currentMem2.Id);
                        if (currentGame2 != null)
                        {
                            currentGame2.Bonus = currentMem2.Bonus;
                            currentGame2.Handicap = currentMem2.Handicap;
                            txtScratchScore3.Text = Convert.ToString(currentGame2.Game1);
                            txtScratchScore4.Text = Convert.ToString(currentGame2.Game2);
                        }
                    }
                    else
                    {
                        MessageBox.Show(string.Format("A member with the number {0} does not exist", txtMemberNum.Text), "Your Attention Please.");
                        txtMemberNum.Clear();
                        txtMemberNum2.Clear();
                    }
                }
            }
            //IF the tournament is not DOUBLES
            else
            {
                //if (e.KeyCode != Keys.Enter) // manually press Enter to populate Names
                //    return;

                string searchNumber = txtMemberNum.Text;
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
                            MemberStatus("Inactive", Color.Red, Color.Pink, true);
                        } 
                        txtScratchScore1.Focus();

                        txtLastName.Text = currentMem.LastName;
                        txtFirstName.Text = currentMem.FirstName;
                        txtMiddleInitial.Text = currentMem.MiddleInitial;
                        txtHandicap.Text = currentMem.Handicap.ToString();
                        txtBonusPins.Text = currentMem.Bonus.ToString();
                        Game currentGame = GetScoresById(currentMem.Id);
                        if(currentGame != null)
                        {
                            currentGame.Bonus = currentMem.Bonus;
                            currentGame.Handicap = currentMem.Handicap;
                            txtScratchScore1.Text = Convert.ToString(currentGame.Game1);
                            txtScratchScore2.Text = Convert.ToString(currentGame.Game2);
                            txtScratchScore3.Text = Convert.ToString(currentGame.Game3);
                            txtScratchScore4.Text = Convert.ToString(currentGame.Game4);
                        }
                    
                    }
                    else
                    {
                        MessageBox.Show(string.Format("A member with the number {0} does not exist", txtMemberNum.Text), "Your Attention Please.");
                        txtMemberNum.Clear();
                    }
                }

            }
        }

        private void FillMember()
        {
            //listOfParticipants brings back a list of participants but does not carry over "member/tournament/game"
            //So I implemented a new query which brings back members and store it within the listOfParticipants
            var currTourney = GetTournamentById(Convert.ToInt32(cbxTourneyDropDown.SelectedValue));
            List<Participant> listOfParticipants = TournamentDb.GetTournamentMemberList(currTourney);
            var db = new NineTapDb();
            var temp = (from p in db.Participants
                    join m in db.Members on p.Member.Id equals m.Id
                    where p.Tournament.Id == currTourney.Id
                    select p.Member).ToList();
            
            string searchNumber = currentIndex.ToString();
            for (int i = 0; i < searchNumber.Length; i++)
            {
                if (!char.IsNumber(searchNumber[i]))
                {
                    MessageBox.Show("Please input numbers only.", "Your Attention Please.");
                    txtMemberNum.Clear();
                    txtMemberNum2.Clear();
                    return;
                }
            }
            if (searchNumber.Trim() != "")
            {
                if (currentIndex == 0)
                {
                    return;
                }
                listOfParticipants[currentIndex-1].Member = temp[currentIndex-1];
                int memberNumber = listOfParticipants[currentIndex-1].Member.Id;
                currentMem = ((FrmMain)MdiParent)._membersList.FirstOrDefault(m => m.Id == memberNumber);
                if (currentMem != null)
                {
                    if (currentMem.IsActive)
                    {
                        MemberStatus("Active", Color.Green, Color.Lime, false);
                    }
                    else
                    {
                        MemberStatus("Inactive", Color.Red, Color.Pink, true);
                    }
                    txtLastName.Text = currentMem.LastName;
                    txtFirstName.Text = currentMem.FirstName;
                    txtMiddleInitial.Text = currentMem.MiddleInitial;
                    txtHandicap.Text = currentMem.Handicap.ToString();
                    txtBonusPins.Text = currentMem.Bonus.ToString();

                    Game currentGame = GetScoresById(currentMem.Id);
                    if (currentGame != null)
                    {
                        currentGame.Bonus = currentMem.Bonus;
                        currentGame.Handicap = currentMem.Handicap;
                        txtScratchScore1.Text = Convert.ToString(currentGame.Game1);
                        txtScratchScore2.Text = Convert.ToString(currentGame.Game2);
                        txtScratchScore3.Text = Convert.ToString(currentGame.Game3);
                        txtScratchScore4.Text = Convert.ToString(currentGame.Game4);
                    }
                }
                else
                {
                    MessageBox.Show(string.Format("A member with the number {0} does not exist", txtMemberNum.Text), "Your Attention Please.");
                    txtMemberNum.Clear();
                    txtMemberNum2.Clear();
                }
            }
        }
        /// <summary>
        /// May be unused
        /// </summary>
        /// <returns></returns>
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
            TextBox tx = (TextBox)sender;
            //auto tab to the next textbox when textbox1's length is 3.
            if (tx.Text.Length == 3)
            {
                SendKeys.Send("{TAB}");
            }
        }

        /// <summary>
        /// finds the handicap score
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
            if (IsValid()) 
            {
                Tournament currTourney = GetTournamentById(Convert.ToInt32(cbxTourneyDropDown.SelectedValue));
                List<Participant> total = TournamentDb.GetTournamentMemberList(currTourney);
                //Doubles tournament
                if (currTourney.Doubles)
                {
                    player.Game = new Game();
                    player2.Game = new Game();
                    NineTapDb db = new NineTapDb();
                    int gameId = (from p in db.Participants
                                  where p.Member.Id == currentMem.Id
                                  && p.Tournament.Id == currTourney.Id
                                  select p.Game.Id).FirstOrDefault();
                    int gameId2 = (from p in db.Participants
                                  where p.Member.Id == currentMem2.Id
                                  && p.Tournament.Id == currTourney.Id
                                  select p.Game.Id).FirstOrDefault();
                    player.Game.Id = gameId;
                    //selects the ID of the combobox of tournaments and stores the
                    //tournament property within the participants class.
                    player.Tournament = currTourney;
                    player.Game.Game1 = IsEmpty(txtScratchScore1) ? null : (int?)Convert.ToInt32((scratchArray[0].Text));
                    player.Game.Game2 = IsEmpty(txtScratchScore2) ? null : (int?)Convert.ToInt32((scratchArray[1].Text));
                    player.Game.Bonus = currentMem.Bonus;
                    player.Game.Handicap = currentMem.Handicap;

                    player2.Game.Id = gameId2;
                    player2.Tournament = currTourney;
                    player2.Game.Game1 = IsEmpty(txtScratchScore1) ? null : (int?)Convert.ToInt32((scratchArray[2].Text));
                    player2.Game.Game2 = IsEmpty(txtScratchScore2) ? null : (int?)Convert.ToInt32((scratchArray[3].Text));
                    player2.Game.Bonus = currentMem2.Bonus;
                    player2.Game.Handicap = currentMem2.Handicap;

                    #region radio button
                    if (rdoSquadOne.Checked)
                    {
                        player.Squad = 1;
                        player2.Squad = 1;
                    }
                    else if (rdoSquadTwo.Checked)
                    {
                        player.Squad = 2;
                        player2.Squad = 2;
                    }
                    else if (rdoSquadThree.Checked)
                    {
                        player.Squad = 3;
                        player2.Squad = 3;
                    }
                    else
                    {
                        player.Squad = 4;
                        player2.Squad = 4;
                    }
                    #endregion

                    player.Member = currentMem;
                    player.Id = total.Count;
                    player2.Member = currentMem2;
                    player2.Id = total.Count + 1;
                    try
                    {
                        TournamentDb.AddMemberToTournament(player);
                        TournamentDb.AddMemberToTournament(player2);
#if DEBUG
                        MessageBox.Show(@"Bowlers Added Successfully to Tournament!");
#endif
                        RecordIndex(TournamentDb.GetTournamentMemberList(currTourney));

                    }
                    catch (MemberAccessException ex)
                    {
                        MessageBox.Show(ex.Message);

                    }
                    clear();
                    txtMemberNum.Focus();
                }
                //IF the tournament type is NOT a DOUBLES tournament
                else
                {
                    player.Game = new Game();
                    var db = new NineTapDb();
                    var gameId = (from p in db.Participants
                                    where p.Member.Id == currentMem.Id 
                                    && p.Tournament.Id == currTourney.Id
                                    select p.Game.Id).FirstOrDefault();

                    player.Game.Id = gameId;
                    //selects the ID of the combobox of tournaments and stores the
                    //tournament property within the participants class.
                    player.Tournament = currTourney;
                    if (string.IsNullOrEmpty(txtScratchScore1.Text.Trim()) || string.IsNullOrEmpty(txtScratchScore2.Text.Trim())
                        || string.IsNullOrEmpty(txtScratchScore3.Text.Trim()) || string.IsNullOrEmpty(txtScratchScore4.Text.Trim()))
                    {
                        MessageBox.Show("Please enter all scratch scores", "Blank Scores Not Allowed");
                    }
                    else if (!isNumeric(txtScratchScore1.Text.Trim()) || !isNumeric(txtScratchScore2.Text.Trim())
                        || !isNumeric(txtScratchScore3.Text.Trim()) || !isNumeric(txtScratchScore4.Text.Trim()))
                    {
                        MessageBox.Show("Please enter only numbers", "Non-Integer Scores Not Allowed");
                    }
                    else
                    {
                        player.Game.Game1 = IsEmpty(txtScratchScore1) ? null : (int?)Convert.ToInt32((scratchArray[0].Text));
                        player.Game.Game2 = IsEmpty(txtScratchScore2) ? null : (int?)Convert.ToInt32((scratchArray[1].Text));
                        player.Game.Game3 = IsEmpty(txtScratchScore3) ? null : (int?)Convert.ToInt32((scratchArray[2].Text));
                        player.Game.Game4 = IsEmpty(txtScratchScore4) ? null : (int?)Convert.ToInt32((scratchArray[3].Text));
                        player.Game.Bonus = currentMem.Bonus;
                        player.Game.Handicap = currentMem.Handicap;
                    }

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
                        TournamentDb.AddMemberToTournament(player);
        #if DEBUG
                        MessageBox.Show(@"Bowler Added Successfully to Tournament!");
#endif
                        RecordIndex(TournamentDb.GetTournamentMemberList(currTourney));
                    }
                    catch (MemberAccessException ex)
                    {
                        MessageBox.Show(ex.Message);

                    }
                    clear();
                    txtMemberNum.Focus();
                }
            }
            else 
            {
                MessageBox.Show("Please Fill out the Participants information!");
            }
        }
        /// <summary>
        /// Checks a string for numeric values
        /// true if all are numeric
        /// </summary>
        /// <param name="str"></param>
        /// <returns>isNum</returns>
        public bool isNumeric(string str)
        {
            int num;
            bool isNum = int.TryParse(str, out num);
            return isNum;
        }

        /// <summary>
        /// updates the index and total count of the record label
        /// </summary>
        /// <param name="players"> a list of participant objects </param>
        public void RecordIndex(List<Participant> players)
        {
            int temp;
            if (players.Count() <= 0)
            {
                temp = 0;
            }
            else
            {
                temp = players.IndexOf(players[currentIndex]);
            }
            lblRecord.Text = "Record " + (temp) + " / " + players.Count();
        }

        /// <summary>
        /// check for empty text box
        /// </summary>
        /// <param name="box"></param>
        /// <returns></returns>
        private bool IsEmpty(TextBox box)
        {
            if (string.IsNullOrEmpty(box.Text.Trim()))
            {
                return true;
            }
            return false;
        } 

        /// <summary>
        /// get a tournament by selected id
        /// </summary>
        /// <param name="selectedTournamentId"></param>    
        private static Tournament GetTournamentById(int selectedTournamentId)
        {
            Tournament selectedTournament = (from t in TournamentDb.GetTournamentList()
                                             where t.Id == selectedTournamentId
                                             select t).Single();
            return selectedTournament;
        }

        /// <summary>
        /// gets the scores from games table by joining participants and tourneys by id 
        /// where member id = participant.member ID and selectedtourney id = tourney id
        /// </summary>
        /// <param name="memberID"></param>
        /// <returns></returns>
        
        public Game GetScoresById(int memberID)
        {
            NineTapDb db = new NineTapDb();
            Game memScores =  new Game();
            int squad = 0;

            if (rdoSquadOne.Checked)
            {
                squad = 1;
            }
            else if (rdoSquadTwo.Checked)
            {
                squad = 2;
            }
            else if (rdoSquadThree.Checked)
            {
                squad = 3;
            }
            else
            {
                squad = 4;
            }

            try
            {
                int selectedTournamentId = Convert.ToInt32(cbxTourneyDropDown.SelectedValue);
                
                 memScores = (from t in db.Tournaments
                                 join p in db.Participants on t.Id equals p.Tournament.Id
                                 where t.Id == p.Tournament.Id 
                                 && memberID == p.Member.Id 
                                 && selectedTournamentId == t.Id
                                 && p.Squad == squad
                                 select p.Game).Single();
                
            }
            catch(InvalidOperationException ex)
            {
                return null;
            }
            return memScores;
                     
        }
        /// <summary>
        /// clears memberNum, txtScratchScores, and High Game textboxes
        /// </summary>
        private void clear()
        {
            txtMemberNum.Clear();
            richTextBox1.Clear();
            richTextBox2.Clear();
            richTextBox3.Clear();
        }
        
        /// <summary>
        /// increments to the next participant in the tournament
       
        /// </summary>
        
        private void btnRightArrow_Click(object sender, EventArgs e)
        {
            List<Participant> total = TournamentDb.GetTournamentMemberList(GetTournamentById(Convert.ToInt32(cbxTourneyDropDown.SelectedValue)));
            if (currentIndex >= total.Count())
            {
                MessageBox.Show("There are no more players to go to!");
            }
            else
            {
                var temp = total.IndexOf(total[currentIndex]);
                currentIndex++;
                lblRecord.Text = "Record " + (temp + 1) + " / " + total.Count();
                txtMemberNum.Text = Convert.ToString(total[currentIndex - 1].Member.Number);
                if (total[currentIndex - 1].Squad == 1)
                {
                    rdoSquadOne.Checked = true;
                }
                else if (total[currentIndex - 1].Squad == 2)
                {
                    rdoSquadTwo.Checked = true;
                }
                else if (total[currentIndex - 1].Squad == 3)
                {
                    rdoSquadThree.Checked = true;
                }
                else
                {
                    rdoSquadFour.Checked = true;
                }
                FillMember();
            }
        }

        /// <summary>
        /// decrements to the previous participant in the tournament
        /// </summary>
        private void btnLeftArrow_Click(object sender, EventArgs e)
        {
            List<Participant> total = TournamentDb.GetTournamentMemberList(GetTournamentById(Convert.ToInt32(cbxTourneyDropDown.SelectedValue)));
            if (currentIndex <= 0)
            {
                MessageBox.Show("There are no more players to go back to!");
            }
            else
            {
                if (currentIndex <= 1)
                {
                    MessageBox.Show("You can't go back!");
                }
                else
                {
                    currentIndex--;
                    var temp = total.IndexOf(total[currentIndex]);
                    lblRecord.Text = "Record " + temp + " / " + total.Count();
                    txtMemberNum.Text = Convert.ToString(total[currentIndex - 1].Member.Number);
                    if (total[currentIndex - 1].Squad == 1)
                    {
                        rdoSquadOne.Checked = true;
                    }
                    else if (total[currentIndex - 1].Squad == 2)
                    {
                        rdoSquadTwo.Checked = true;
                    }
                    else if (total[currentIndex - 1].Squad == 3)
                    {
                        rdoSquadThree.Checked = true;
                    }
                    else
                    {
                        rdoSquadFour.Checked = true;
                    }
                    FillMember();
                }
            }
        }

        /// <summary>
        /// opens the new tournament form via creating a new from by referencing the form itself
        /// </summary>
        private void btnNewTournament_Click(object sender, EventArgs e)
        {
            var newfrmNewTournament = Application.OpenForms["frmNewTournament"] as frmNewTournament;
            ((FrmMain)MdiParent).OpenOrDisplayForm(ref newfrmNewTournament);
            newfrmNewTournament.Dock = DockStyle.None;
            rdoSquadOne.Checked = true;
        }
        
        /// <summary>
        /// updates record index when tourney is changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxTourneyDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxTourneyDropDown.SelectedIndex <= 0)
            {
                lblRecord.Text = "Record 0" + " / " + "0";
            }
            else
            {
                RecordIndex(TournamentDb.GetTournamentMemberList(GetTournamentById(Convert.ToInt32(cbxTourneyDropDown.SelectedValue))));
                refresh(false);
            }
        }
        /// <summary>
        /// validation method for form fields
        /// </summary>
        /// <returns>boolean</returns>
        public bool IsValid()
        {
            if (cbxTourneyDropDown.SelectedValue == null)
            {
                return false;
            }
            if (txtMemberNum.Text == "" && txtScratchScore1.Text == "" && txtScratchScore2.Text == "" && txtScratchScore3.Text == "" && txtScratchScore4.Text == "")
            {
                return false;
            }
            return true;
        }

        private void txtMemberNum2_TextChanged(object sender, EventArgs e)
        {
            txtMemberNum.Focus();
        }

        /// <summary>
        /// Search for tours by location.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTourSearch_Click(object sender, EventArgs e)
        {
            List<Tournament> tours = new List<Tournament>();
            FrmTourSearch tourSearch = new FrmTourSearch(tours);
            tourSearch.ShowDialog();
#if DEBUG
            foreach (Tournament tour in tours)
            {
                Console.WriteLine(tour.TourneyNameDate);
            }
#endif
            if (tours.Count() > 0) {
                cbxTourneyDropDown.DataSource = tours;
                cbxTourneyDropDown.DisplayMember = "TourneyNameDate";
            }
        }
        /// <summary>
        /// Checks if current member has an existing entry into Squad 4
        /// and clears the scores if the member does NOT
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rdoSquadFour_CheckedChanged(object sender, EventArgs e)
        {
            if (GetScoresById(currentMem.Id) == null)
            {
                ScoreAndTotalClear();
            }
        }
        /// <summary>
        /// Checks if current member has an existing entry into Squad 1
        /// and clears the scores if the member does NOT
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rdoSquadOne_CheckedChanged(object sender, EventArgs e)
        {
            if (GetScoresById(currentMem.Id) == null)
            {
                ScoreAndTotalClear();
            }
        }
        /// <summary>
        /// Checks if current member has an existing entry into Squad 2
        /// and clears the scores if the member does NOT
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rdoSquadTwo_CheckedChanged(object sender, EventArgs e)
        {
            if (GetScoresById(currentMem.Id) == null)
            {
                ScoreAndTotalClear();
            }
        }
        /// <summary>
        /// Clears scratch scores and scratch and handicap totals
        /// </summary>
        private void ScoreAndTotalClear()
        {
            txtScratchScore1.Clear();
            txtScratchScore2.Clear();
            txtScratchScore3.Clear();
            txtScratchScore4.Clear();
            txtScratchTotal.Clear();
            txtHandicapTotal.Clear();
        }
        /// <summary>
        /// Checks if current member has an existing entry into Squad 3
        /// and clears the scores if the member does NOT
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rdoSquadThree_CheckedChanged(object sender, EventArgs e)
        {
            if (GetScoresById(currentMem.Id) == null)
            {
                ScoreAndTotalClear();
            }
        }

        private void btnTournamentsByYear_Click(object sender, EventArgs e)
        {            
            FrmListTournamentsByYear listTournaments = new FrmListTournamentsByYear();
            listTournaments.ShowDialog();
        }

        private void rdoScratchScore_CheckedChanged(object sender, EventArgs e)
        {
            refresh(true);
        }

        private void rdoHandicapScore_CheckedChanged(object sender, EventArgs e)
        {
            refresh(true);
        }

        public void refresh(bool seriesChange)
        {
            try
            {
                // Function scope data
                int nullValues;
                NineTapDb db = new NineTapDb();
                int selectedTourney = Convert.ToInt32(cbxTourneyDropDown.SelectedValue);
                List<MemberScores> scores;
                IComparer<MemberScores> scoreComparer = new MemberScoresComparer();
                var top5 = db.Participants.Include(b => b.Member)
                        .Include(b => b.Game)
                        .Where(b => b.Tournament.Id == selectedTourney);

                // This function combines the former refresh events into a single function, and since they all used the same variable names I just put
                // their old data in a scope block so they could be reused
                if (!seriesChange)
                {
                    richTextBox1.Clear();
                    richTextBox1.Font = new Font(FontFamily.GenericMonospace, richTextBox1.Font.Size);
                    richTextBox1.Text = ("#" + "\t" + "Name" + "\t\t" + "Handicap" + "\n");

                    scores = new List<MemberScores>();

                    var temp = (from g in top5
                                orderby (g.Game.Handicap) descending
                                select g).Take(5).ToList();

                    foreach (var i in temp)
                    {
                        #region conditions for highest handicap scores
                        nullValues = 0;
                        if (i.Game.Game1 == 0)
                        {
                            nullValues += 1;
                        }
                        if (i.Game.Game2 == 0)
                        {
                            nullValues += 1;
                        }
                        if (i.Game.Game3 == 0)
                        {
                            nullValues += 1;
                        }
                        if (i.Game.Game4 == 0)
                        {
                            nullValues += 1;
                        }
                        #endregion
                        scores.Add(new MemberScores { FirstName = i.Member.FirstName, LastName = i.Member.LastName, Score = i.Game.Handicap });
                    }

                    scores.Sort(scoreComparer);
                    scores.Reverse();
                    scores = scores.Take(5).ToList();
                    for (int i = 0; i < scores.Count(); i++)
                    {
                        richTextBox1.AppendText((i+1).ToString() + "\t" + String.Format("{0, -20}", scores[i].FirstName + " " + scores[i].LastName)
                                                + "\t" + String.Format("{0, -5}", scores[i].Score + "\n"));
                    }
                }

                // Do the 2nd box
                if (!seriesChange)
                {
                    richTextBox2.Clear();
                    richTextBox2.Font = new Font(FontFamily.GenericMonospace, richTextBox2.Font.Size);
                    richTextBox2.Text = ("#" + "\t" + "Name" + "\t\t" + "HighScore" + "\n");
                    scores = new List<MemberScores>();

                    var temp = (from g in top5
                                orderby g.Game.Game1
                                select new { g.Game.Game1, g.Member.FirstName, g.Member.LastName }).Take(5);
                    var temp2 = (from g in top5
                                 orderby g.Game.Game2
                                 select new { g.Game.Game2, g.Member.FirstName, g.Member.LastName }).Take(5);
                    var temp3 = (from g in top5
                                 orderby g.Game.Game3
                                 select new { g.Game.Game3, g.Member.FirstName, g.Member.LastName }).Take(5);
                    var temp4 = (from g in top5
                                 orderby g.Game.Game4
                                 select new { g.Game.Game4, g.Member.FirstName, g.Member.LastName }).Take(5);
                    foreach (var s in temp)
                    {
                        scores.Add(new MemberScores { FirstName = s.FirstName, LastName = s.LastName, Score = s.Game1 });
                    }
                    foreach (var s in temp2)
                    {
                        scores.Add(new MemberScores { FirstName = s.FirstName, LastName = s.LastName, Score = s.Game2 });
                    }
                    foreach (var s in temp3)
                    {
                        scores.Add(new MemberScores { FirstName = s.FirstName, LastName = s.LastName, Score = s.Game3 });
                    }
                    foreach (var s in temp4)
                    {
                        scores.Add(new MemberScores { FirstName = s.FirstName, LastName = s.LastName, Score = s.Game4 });
                    }
                    scores.Sort(scoreComparer);
                    scores.Reverse();
                    scores = scores.Take(5).ToList();
                    for (int i = 0; i < scores.Count(); i++)
                    {
                        richTextBox2.AppendText((i + 1).ToString() + "\t" + String.Format("{0, -20}", scores[i].FirstName + " " + scores[i].LastName)
                                                + "\t" + String.Format("{0, -5}", scores[i].Score + "\n"));
                    }
                }

                // Do the third box
                {
                    richTextBox3.Clear();
                    richTextBox3.Font = new Font(FontFamily.GenericMonospace, richTextBox3.Font.Size);
                    richTextBox3.Text = ("#" + "\t" + "Name" + "\t\t" + "High Series" + "\n");
                    scores = new List<MemberScores>();
                    var temp = (from g in top5
                                orderby (g.Game.Game1 + g.Game.Game2 + g.Game.Game3 + g.Game.Game4) descending
                                select g).Take(5).ToList();

                    //populate total score
                    for (int i = 0; i < temp.Count; i++)
                    {
                        temp[i].Game.Game1 = temp[i].Game.Game1 == null ? 0 : temp[i].Game.Game1;
                        temp[i].Game.Game2 = temp[i].Game.Game2 == null ? 0 : temp[i].Game.Game2;
                        temp[i].Game.Game3 = temp[i].Game.Game3 == null ? 0 : temp[i].Game.Game3;
                        temp[i].Game.Game4 = temp[i].Game.Game4 == null ? 0 : temp[i].Game.Game4;
                    }
                    if (rdoScratchScore.Checked)
                    {
                        foreach (var s in temp)
                        {
                            scores.Add(new MemberScores { FirstName = s.Member.FirstName, LastName = s.Member.LastName, Score = s.Game.Game1 + s.Game.Game2 + s.Game.Game3 + s.Game.Game4 });
                        }
                        //IComparer<MemberScores> scoreComparer = new MemberScoresComparer();
                        scores.Sort(scoreComparer);
                        scores.Reverse();
                        scores = scores.Take(5).ToList();
                        for (int i = 0; i < scores.Count(); i++)
                        {
                            richTextBox3.AppendText((i + 1).ToString() + "\t" + String.Format("{0, -20}", scores[i].FirstName + " " + scores[i].LastName)
                                                    + "\t" + String.Format("{0, -5}", scores[i].Score + "\n"));
                        }
                    }
                    else if (rdoHandicapScore.Checked)
                    {
                        foreach (var i in temp)
                        {
                            #region conditions for highest handicap scores
                            nullValues = 0;
                            if (i.Game.Game1 == 0)
                            {
                                nullValues += 1;
                            }
                            if (i.Game.Game2 == 0)
                            {
                                nullValues += 1;
                            }
                            if (i.Game.Game3 == 0)
                            {
                                nullValues += 1;
                            }
                            if (i.Game.Game4 == 0)
                            {
                                nullValues += 1;
                            }
                            #endregion
                            scores.Add(new MemberScores { FirstName = i.Member.FirstName, LastName = i.Member.LastName, Score = (i.Game.Game1) + (i.Game.Game2) + (i.Game.Game3) + (i.Game.Game4) + ((4 - nullValues) * (i.Game.Handicap + i.Game.Bonus)) });
                        }

                        scores.Sort(scoreComparer);
                        scores.Reverse();
                        scores = scores.Take(5).ToList();
                        for (int i = 0; i < scores.Count(); i++)
                        {
                            richTextBox3.AppendText((i + 1).ToString() + "\t" + String.Format("{0, -20}", Convert.ToString(scores[i].FirstName + " " + scores[i].LastName)) + "\t" + String.Format("{0, -5}", scores[i].Score) + "\n");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Cannot refresh when initializing.");
#endif
            if (tours.Count() > 0) {
                cbxTourneyDropDown.DataSource = tours;
                cbxTourneyDropDown.DisplayMember = "TourneyNameDate";
            }
        }
        /// <summary>
        /// Checks if current member has an existing entry into Squad 4
        /// and clears the scores if the member does NOT
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rdoSquadFour_CheckedChanged(object sender, EventArgs e)
        {
            if (GetScoresById(currentMem.Id) == null)
            {
                ScoreAndTotalClear();
            }
        }
        /// <summary>
        /// Checks if current member has an existing entry into Squad 1
        /// and clears the scores if the member does NOT
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rdoSquadOne_CheckedChanged(object sender, EventArgs e)
        {
            if (GetScoresById(currentMem.Id) == null)
            {
                ScoreAndTotalClear();
            }
        }
        /// <summary>
        /// Checks if current member has an existing entry into Squad 2
        /// and clears the scores if the member does NOT
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rdoSquadTwo_CheckedChanged(object sender, EventArgs e)
        {
            if (GetScoresById(currentMem.Id) == null)
            {
                ScoreAndTotalClear();
            }
        }
        /// <summary>
        /// Clears scratch scores and scratch and handicap totals
        /// </summary>
        private void ScoreAndTotalClear()
        {
            txtScratchScore1.Clear();
            txtScratchScore2.Clear();
            txtScratchScore3.Clear();
            txtScratchScore4.Clear();
            txtScratchTotal.Clear();
            txtHandicapTotal.Clear();
        }
        /// <summary>
        /// Checks if current member has an existing entry into Squad 3
        /// and clears the scores if the member does NOT
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rdoSquadThree_CheckedChanged(object sender, EventArgs e)
        {
            if (GetScoresById(currentMem.Id) == null)
            {
                ScoreAndTotalClear();
            }
        }
    }




    class MemberScores
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public int? Score { get; set; }

    }

    class MemberScoresComparer : IComparer<MemberScores>
    {
        int IComparer<MemberScores>.Compare(MemberScores x, MemberScores y)
        {
            int score1 = x.Score.HasValue ? (int)x.Score : 0;
            int score2 = y.Score.HasValue ? (int)y.Score : 0;
            return score1.CompareTo(score2);
        }
    }
}
