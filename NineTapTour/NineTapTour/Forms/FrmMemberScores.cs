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
            txtMemberNum.Focus();
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
            var temp2 = TournamentDb.GetTournamentList();
            var item = temp2.Max(x => x.Id);
            cbxTourneyDropDown.SelectedValue = item;
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
                    Game temp = GetScoresById(currentMem.Id);
                    if(temp != null)
                    {
                        txtScratchScore1.Text = Convert.ToString(temp.Game1);
                        txtScratchScore2.Text = Convert.ToString(temp.Game2);
                        txtScratchScore3.Text = Convert.ToString(temp.Game3);
                        txtScratchScore4.Text = Convert.ToString(temp.Game4);
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

                    Game temp = GetScoresById(currentMem.Id);
                    txtScratchScore1.Text = Convert.ToString(temp.Game1);
                    txtScratchScore2.Text = Convert.ToString(temp.Game2);
                    txtScratchScore3.Text = Convert.ToString(temp.Game3);
                    txtScratchScore4.Text = Convert.ToString(temp.Game4);
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
                List<Participant> total = TournamentDb.GetTournamentMemberList(GetTournamentById(Convert.ToInt32(cbxTourneyDropDown.SelectedValue)));
                int temp = total.IndexOf(total[count]);
                count++;
                lblRecord.Text = "Record " + (temp + 1) + " / " + total.Count();
            }
            catch (MemberAccessException ex)
            {
                MessageBox.Show(ex.Message);

            }
            clear();
            txtMemberNum.Focus();
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
            catch(SystemException ex)
            {
                MessageBox.Show(ex.Message);
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
            if (count >= total.Count())
            {
                MessageBox.Show("There are no more players to go to!");
            }
            else
            {
                var temp = total.IndexOf(total[count]);
                count++;
                lblRecord.Text = "Record " + (temp + 1) + " / " + total.Count();
                txtMemberNum.Text = Convert.ToString(temp + 1);
                FillMember();
            }

        }

        /// <summary>
        /// decrements to the previous participant in the tournament
        /// </summary>
        private void btnLeftArrow_Click(object sender, EventArgs e)
        {
            List<Participant> total = TournamentDb.GetTournamentMemberList(GetTournamentById(Convert.ToInt32(cbxTourneyDropDown.SelectedValue)));
            if (count < 0)
            {
                MessageBox.Show("There are no more players to go back to!");
            }
            else
            {
                if (count == 0)
                {
                    MessageBox.Show("You can't go back!");
                }
                else
                {
                    count--;
                    var temp = total.IndexOf(total[count]);
                    lblRecord.Text = "Record " + temp + " / " + total.Count();
                    txtMemberNum.Text = Convert.ToString(temp);
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
            richTextBox2.Text = ("#" + "\t" + "Name" + "\t\t" + "HighScore" + "\n");
            int counter = 1;
            int index = 0;
            var db = new NineTapDb();
            int selectedTourney = Convert.ToInt32(cbxTourneyDropDown.SelectedValue);
            var top5 = (from t in db.Tournaments
                        join p in db.Participants on t.Id equals p.Tournament.Id
                        join g in db.Participants on p.Game.Id equals g.Id
                        where p.Tournament.Id == selectedTourney
                        orderby p.Game.Game1 descending
                        select p.Game).Take(5).ToList();
            var top5Names = (from t in db.Tournaments
                             join p in db.Participants on t.Id equals p.Tournament.Id
                             join g in db.Participants on p.Game.Id equals g.Id
                             where p.Tournament.Id == selectedTourney
                             orderby p.Game.Game1 descending
                             select p.Member).Take(5).ToList();

            foreach (var i in top5)
            {
                int highestGame = i.Game1;
                var mem = i.Member;
                if(i.Game2 > highestGame)
                {
                    highestGame = i.Game2;
                }
                else if(i.Game3 > highestGame) 
                {
                    highestGame = i.Game3;
                }
                else if(i.Game4 > highestGame)
                {
                    highestGame = i.Game4;
                }
                richTextBox2.AppendText(counter + "\t" + Convert.ToString(top5Names[index].FirstName + " " + top5Names[index].LastName) + "\t" + Convert.ToString(highestGame) + "\n");
                counter++;
                index++;
            }
        }

        private void btnRefresh3_Click(object sender, EventArgs e)
        {
            richTextBox3.Clear();
            int scratch = 0;
            int counter = 1;
            int index = 0;
            richTextBox3.Text = ("#" + "\t" + "Name" + "\t\t" + "High Series" + "\n");
            var db = new NineTapDb();
            int selectedTourney = Convert.ToInt32(cbxTourneyDropDown.SelectedValue);
            var top5 = (from t in db.Tournaments
                        join p in db.Participants on t.Id equals p.Tournament.Id
                        join g in db.Participants on p.Game.Id equals g.Id
                        where p.Tournament.Id == selectedTourney
                        orderby (p.Game.Game1 + p.Game.Game2 + p.Game.Game3 + p.Game.Game4) descending
                        select p.Game).Take(5).ToList();
            var top5Names = (from t in db.Tournaments
                                join p in db.Participants on t.Id equals p.Tournament.Id
                                join g in db.Participants on p.Game.Id equals g.Id
                                where p.Tournament.Id == selectedTourney
                                orderby (p.Game.Game1 + p.Game.Game2 + p.Game.Game3 + p.Game.Game4) descending
                                select p.Member).Take(5).ToList();
            if (rdoScratchScore.Checked)
            {
                foreach (var i in top5)
                {
                    scratch = i.Game1 + i.Game2 + i.Game3 + i.Game4;
                    richTextBox3.AppendText(counter + "\t" + Convert.ToString(top5Names[index].FirstName + " " + top5Names[index].LastName) + "\t" + Convert.ToString(scratch) + "\n");
                    counter++;
                    index++;
                }

            }
            else if(rdoHandicapScore.Checked) 
            {
                foreach (var i in top5)
                {
                    scratch = (i.Game1 + Convert.ToInt32(top5Names[index].Handicap) + Convert.ToInt32(top5Names[index].Bonus)) 
                        + (i.Game2 + Convert.ToInt32(top5Names[index].Handicap) + Convert.ToInt32(top5Names[index].Bonus)) 
                        + (i.Game3 + Convert.ToInt32(top5Names[index].Handicap) + Convert.ToInt32(top5Names[index].Bonus))
                        + (i.Game4 + Convert.ToInt32(top5Names[index].Handicap) + Convert.ToInt32(top5Names[index].Bonus));
                    richTextBox3.AppendText(counter + "\t" + Convert.ToString(top5Names[index].FirstName + " " + top5Names[index].LastName) + "\t" + Convert.ToString(scratch) + "\n");
                    counter++;
                    index++;
                }
            }
        }
    }
}
