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
using System.Data.Entity;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;

namespace NineTapTour.Forms
{
    public partial class frmMemberScores : Form
    {

        //IOrderedEnumerable<Member> _membersList;
        Member currentMem;
        TextBox[] scratchArray = new TextBox[4];
        TextBox[] handicappArray = new TextBox[4];
        //Count for record counting
        int currentIndex = 0;
        Participant player = new Participant();


        public frmMemberScores()
        {
            InitializeComponent();
        }

        private void FrmMemberScores_Load(object sender, EventArgs e)
        {
            txtMemberNum.Focus();
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
                List<Participant> total = TournamentDb.GetTournamentMemberList(GetTournamentById(Convert.ToInt32(cbxTourneyDropDown.SelectedValue)));
                player.Game = new Game();
                int selectedTournamentId = Convert.ToInt32(cbxTourneyDropDown.SelectedValue);
                Tournament selectedTourney = GetTournamentById(selectedTournamentId);
                var db = new NineTapDb();
                var gameId = (from p in db.Participants
                                where p.Member.Id == currentMem.Id 
                                && p.Tournament.Id == selectedTourney.Id
                                select p.Game.Id).FirstOrDefault();

                player.Game.Id = gameId;
                //selects the ID of the combobox of tournaments and stores the
                //tournament property within the participants class.
                player.Tournament = selectedTourney;
                player.Game.Game1 = IsEmpty(txtScratchScore1) ? null : (int?)Convert.ToInt32((scratchArray[0].Text));
                player.Game.Game2 = IsEmpty(txtScratchScore2) ? null : (int?)Convert.ToInt32((scratchArray[1].Text));
                player.Game.Game3 = IsEmpty(txtScratchScore3) ? null : (int?)Convert.ToInt32((scratchArray[2].Text));
                player.Game.Game4 = IsEmpty(txtScratchScore4) ? null : (int?)Convert.ToInt32((scratchArray[3].Text));
                player.Game.Bonus = currentMem.Bonus;
                player.Game.Handicap = currentMem.Handicap;

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
                if(total.Count() == 0)
                {
                    player.Id = 1;
                }
                else
                {
                    player.Id = total[currentIndex - 1].Id;
                }
                try
                {
                    int temp;
                    TournamentDb.AddMemberToTournament(player);
    #if DEBUG
                    MessageBox.Show(@"Bowler Added Successfully to Tournament!");
    #endif
                    if (total.Count() <= 0)
                    {
                        temp = total.IndexOf(total[currentIndex]);
                    }
                    else
                    {
                        temp = total.IndexOf(total[currentIndex - 1]);
                    }
                    lblRecord.Text = "Record " + (temp + 1) + " / " + total.Count();
                }
                catch (MemberAccessException ex)
                {
                    MessageBox.Show(ex.Message);

                }
                clear();
                txtMemberNum.Focus();
            }
            else 
            {
                MessageBox.Show("Please Fill out the Participants information!");
            }
        }

        private bool IsEmpty(TextBox box)
        {
            if (box.Text == "")
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
            var db = new NineTapDb();
            var memScores =  new Game();
            try
            {
                int selectedTournamentId = Convert.ToInt32(cbxTourneyDropDown.SelectedValue);
                
                 memScores = (from t in db.Tournaments
                                 join p in db.Participants on t.Id equals p.Tournament.Id
                                 where t.Id == p.Tournament.Id && memberID == p.Member.Id && selectedTournamentId == t.Id
                                 select p.Game).Single();
                
            }
            catch(InvalidOperationException ex)
            {
                return null;
            }
            return memScores;
                     
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

        private void btnRefresh2_Click(object sender, EventArgs e)
        {
            richTextBox2.Clear();
            richTextBox2.Font = new Font(FontFamily.GenericMonospace, richTextBox2.Font.Size);
            richTextBox2.Text = ("#" + "\t" + "Name" + "\t\t" + "HighScore" + "\n");
            int counter = 1;
            int index = 0;
            var db = new NineTapDb();
            int selectedTourney = Convert.ToInt32(cbxTourneyDropDown.SelectedValue);

            var top5 = db.Participants.Include(b => b.Member)
                .Include(b => b.Game)
                .Where(b => b.Tournament.Id == selectedTourney);
            List<MemberScores> scores = new List<MemberScores>();
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
            IComparer<MemberScores> scoreComparer = new MemberScoresComparer();
            scores.Sort(scoreComparer);
            scores.Reverse();
            scores = scores.Take(5).ToList();
            for (int i = 0; i < scores.Count(); i++)
            {
                richTextBox2.AppendText(counter + "\t" + String.Format("{0, -20}", scores[i].FirstName + " " + scores[i].LastName)
                                        + "\t" + String.Format("{0, -5}", scores[i].Score + "\n"));
                counter++;
                index++;
            }
            
        }

        private void btnRefresh3_Click(object sender, EventArgs e)
        {
            richTextBox3.Clear();
            int counter = 1;
            int index = 0;
            int nullValues = 0;
            richTextBox3.Font = new Font(FontFamily.GenericMonospace, richTextBox3.Font.Size);
            richTextBox3.Text = ("#" + "\t" + "Name" + "\t\t" + "High Series" + "\n");
            var db = new NineTapDb();
            int selectedTourney = Convert.ToInt32(cbxTourneyDropDown.SelectedValue);

            var top5 = db.Participants.Include(b => b.Member)
                        .Include(b => b.Game)
                        .Where(b => b.Tournament.Id == selectedTourney);
            List<MemberScores> scores = new List<MemberScores>();
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
            if (rdoScratchScore.Checked) {
                foreach (var s in temp)
                {
                    scores.Add(new MemberScores { FirstName = s.Member.FirstName, LastName = s.Member.LastName, Score = s.Game.Game1 + s.Game.Game2 + s.Game.Game3 + s.Game.Game4 });
                }
                IComparer<MemberScores> scoreComparer = new MemberScoresComparer();
                scores.Sort(scoreComparer);
                scores.Reverse();
                scores = scores.Take(5).ToList();
                for (int i = 0; i < scores.Count(); i++)
                {
                    richTextBox3.AppendText(counter + "\t" + String.Format("{0, -20}", scores[i].FirstName + " " + scores[i].LastName)
                                            + "\t" + String.Format("{0, -5}", scores[i].Score + "\n"));
                    counter++;
                    index++;
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


                IComparer<MemberScores> scoreComparer = new MemberScoresComparer();
                scores.Sort(scoreComparer);
                scores.Reverse();
                scores = scores.Take(5).ToList();
                for (int i = 0; i < scores.Count(); i++) {
                    richTextBox3.AppendText(counter + "\t" + String.Format("{0, -20}", Convert.ToString(scores[i].FirstName + " " + scores[i].LastName)) + "\t" + String.Format("{0, -5}", scores[i].Score) + "\n");
                    counter++;
                    index++;

                }
            }
        }

        private void cbxTourneyDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblRecord.Text = "Record 0" + " / " + "0";
            currentIndex = 1;
        }

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
