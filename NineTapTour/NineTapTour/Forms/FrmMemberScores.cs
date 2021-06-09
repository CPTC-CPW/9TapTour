using Microsoft.EntityFrameworkCore;
using NineTapTour.Database;
using NineTapTour.Models;
using NineTapTour.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using static NineTapTour.Database.ReportHelper;

namespace NineTapTour.Forms
{
    /// <summary>
    /// FrmMemberScores class.
    /// All tournament info and scores are entered here.
    /// </summary>
    public partial class FrmMemberScores : Form
    {
        public int RegionID;
        Member currentMem;

        TextBox[] scratchArray = new TextBox[4];
        TextBox[] handicappArray = new TextBox[4];

        public bool switchingParticipents = false;
        //Count for record counting
        int currentIndex = 0;         
        Participant player = new Participant();
        public static Tournament selectedTournament;
        public static List<Participant> overallListOfParticipants;

        List<int> howManySquadsCanBeFiltered = new List<int>();

        /// <summary>
        /// instantiates all form buttons.
        /// </summary>
        public FrmMemberScores()
        {
            InitializeComponent();
        }
        /// <summary>
        /// initializes all the radio buttons on the form and sets them to their correct default status.
        /// higher squad numbers will be available if the tournament was created for more squads.
        /// </summary>
        private void RadioIntialize()
        {
            rdoSquad1.TabStop = false;
            rdoSquad2.TabStop = false;
            rdoSquad3.TabStop = false;
            rdoSquad4.TabStop = false;
            rdoSquad5.TabStop = false;
            rdoSquad6.TabStop = false;
            rdoSquad7.TabStop = false;
            rdoSquad8.TabStop = false;
            rdoHandicapScore.TabStop = false;
            cbAllSquads.TabStop = false;
            cbFilterSquad4.Visible = false;
            cbFilterSquad5.Visible = false;
            cbFilterSquad6.Visible = false;
            cbFilterSquad7.Visible = false;
            cbFilterSquad8.Visible = false;
            rdoSquad4.Visible = false;
            rdoSquad5.Visible = false;
            rdoSquad6.Visible = false;
            rdoSquad7.Visible = false;
            rdoSquad8.Visible = false;
            cbAllSquads.Checked = true;

            if (cbxTourneyDropDown.SelectedIndex >= 0)
            {
                if(selectedTournament.Squads >= 4)
                {
                    rdoSquad4.Visible = true;
                    cbFilterSquad4.Visible = true;
                }

                if (selectedTournament.Squads >= 5)
                {
                    rdoSquad5.Visible = true;
                    cbFilterSquad5.Visible = true;
                }

                if (selectedTournament.Squads >= 6)
                {
                    rdoSquad6.Visible = true;
                    cbFilterSquad6.Visible = true;
                }

                if (selectedTournament.Squads >= 7)
                {
                    rdoSquad7.Visible = true;
                    cbFilterSquad7.Visible = true;
                }

                if (selectedTournament.Squads == 8)
                {
                    rdoSquad8.Visible = true;
                    cbFilterSquad8.Visible = true;
                }
            }
        }

        /// <summary>
        /// The forms onload method. (fired once when the program is loaded)
        /// Sets variables to there starting state.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmMemberScores_Load(object sender, EventArgs e)
        {
            RegionID = ((FrmMain)MdiParent).RegionID;

            scratchArray = new TextBox[4] { txtScratchScore1, txtScratchScore2, txtScratchScore3, txtScratchScore4 };
            handicappArray = new TextBox[4] { txtHandicapScore1, txtHandicapScore2, txtHandicapScore3, txtHandicapScore4 };

            if (cbxTourneyDropDown.SelectedIndex == -1)
            {
                if (currentIndex <= 1)
                {
                    btnFirstRecord.Enabled = false;
                }

                btnLeftArrow.Enabled = false;
                btnRightArrow.Enabled = false;
                btnDelete.Enabled = false;
            }
            else
            {
                btnRightArrow.Enabled = true;
            }
        }

        /// <summary>
        /// Fires when the form gains focus.
        /// This will set the form to the most recent tournament as well as the most recent bowler entered
        /// in that tournament.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmMemberScores_Activated(object sender, EventArgs e)
        {
           
            RegionID = ((FrmMain)MdiParent).RegionID;

            //added in this line inorder to prevent the reset of the drop down list on memberscores form when switching between forms
            int tempcbx = cbxTourneyDropDown.SelectedIndex;
            rdoHandicapScore.Visible = false;
            rdoScratchScore.Visible = false;
            cbxTourneyDropDown.Visible = false;
            ResetFields();

            MemberStatus("", Color.Black, SystemColors.Control, true);

            List<Tournament> temp2 = TournamentDB.GetTournamentList(RegionID);

            ((FrmMain)MdiParent).TournamentList = temp2;
            cbxTourneyDropDown.DataSource = temp2;
            cbxTourneyDropDown.DisplayMember = "TourneyNameDate";
            cbxTourneyDropDown.ValueMember = "Id";

            if (temp2.Count > 0)
            {
                var item = temp2.Max(x => x.Id);
                cbxTourneyDropDown.SelectedValue = item;
            }

            Clear();
            cbxTourneyDropDown.Visible = true;

            if (cbxTourneyDropDown.SelectedIndex >= 0 && cbxTourneyDropDown.Visible && cbxTourneyDropDown.SelectedIndex.ToString() != null)
            {
                // resets the current index to zero when changing the tournament
                currentIndex = 0;

                overallListOfParticipants = TournamentDB.GetTournamentMemberList(selectedTournament);
                RecordIndex(overallListOfParticipants);
                
                btnDelete.Enabled = true;
               
                Refresh(false);
                // sets focus to member num becuse that is what a user will need next
                rdoHandicapScore.Visible = true;
                rdoScratchScore.Visible = true;
                txtMemberNum.Focus();
            }
            //Clicks LastMemberButton when frm is activated.
            //this will make sure the person entering scores 
            //does not accedently enter a bowler in the wrong squad.
            MoveToLastRecordOfMemberScores();
            
        }

        /// <summary>
        /// Resets the fields to show reset and beginning of records
        /// </summary>
        private void ResetFields()
        {
            txtMemberNum.Clear();
            txtLastName.Clear();
            txtFirstName.Clear();
            txtMiddleInitial.Clear();
            cbCompEntry.Checked = false;
            txtHandicap.Clear();
            txtBonusPins.Clear();
            txtScratchScore1.Clear();
            txtScratchScore2.Clear();
            txtScratchScore3.Clear();
            txtScratchScore4.Clear();
            txtScratchTotal.Clear();
            txtMoney.Clear();
        }

        #region GetMember

        //Get players scores 
        private void GetScores(Game currentGame)
        {
            if (currentGame != null)
            {
                Tournament tourney = TournamentDB.GetTourneyByID(Convert.ToInt32(cbxTourneyDropDown.SelectedValue));
                int total = TournamentDB.GetTotalNumberParticipantsInTournament(tourney);
              
                lblRecord.Text = "Record " + (currentIndex + 1) + " / " + total;

                cbCompEntry.Checked = currentGame.IsComp ? true : false;

                txtScratchScore1.Text = Convert.ToString(currentGame.Game1);
                txtScratchScore2.Text = Convert.ToString(currentGame.Game2);
                txtScratchScore3.Text = Convert.ToString(currentGame.Game3);
                txtScratchScore4.Text = Convert.ToString(currentGame.Game4);
                txtScratchScore1.Focus();
                txtMoney.Text = currentGame.MoneyWon.ToString();
            }
        }
        #endregion
        /// <summary>
        /// fetches bowlers scores for selected tournament by using their id
        /// </summary>
        private void FillMember()
        {
            if (cbxTourneyDropDown.SelectedValue != null)
            {
                Tournament currTourney = TournamentDB.GetTourneyByID(Convert.ToInt32(cbxTourneyDropDown.SelectedValue));
                string searchNumber = txtMemberNum.Text;

                //don't do any further processing if there is no member number
                if (searchNumber.Trim() == string.Empty)
                    return;

                if (!int.TryParse(searchNumber, out int number))
                {
                    MessageBox.Show("Please input numbers only.", "Your attention please.");
                    return;
                }

                int memberNumber = Convert.ToInt32(txtMemberNum.Text);
                currentMem = MemberDB.GetMember(memberNumber, RegionID);
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

                    Game currentGame = GetScoresById(currentMem.Id);

                    txtHandicap.Text = currentMem.Handicap.ToString();          
                    txtBonusPins.Text = currentMem.Bonus.ToString();

                    GetScores(currentGame);

                }
                else
                {
                    MessageBox.Show(string.Format("A member with the number {0} does not exist", txtMemberNum.Text), "Your Attention Please.");
                    txtMemberNum.Clear();
                }
            }
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
        public void ScratchTotal(object sender, EventArgs e)
        {
            TextBox currentTextbox = (TextBox)sender;
            if (currentTextbox.Text.Length != 3)
            {
                return;
            }

            int scratchTotal = 0;
            int cScore = 0;
            string id;
                
            foreach (TextBox score in scratchArray)
            {
                // Get the number of the scratch score textbox. txtScratchScore1 returns 1
                id = Regex.Match(score.Name, @"\d+").Value;

                if (int.TryParse(score.Text, out cScore))
                {
                    if (cScore >= 0 && cScore <= 300)
                    {
                        scratchTotal += cScore;
                        HandicapTotal(id, cScore);
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
                    HandicapTotal(id, cScore);
                }
                txtScratchTotal.Text = scratchTotal.ToString();
            }

            //this code will adjust the scratch and handicap total (textboxes) only if its a 3of4 tournament ( taking out the lowest game) 
            if (txtScratchScore1.Text != "" && txtScratchScore2.Text != "" && txtScratchScore3.Text != "" && txtScratchScore4.Text != "")
            {
                int handicapTotal = 0;

                if (selectedTournament.ThreeOutOf4 == true)
                {
                    int[] scratchAsInt = new int[4];
                    int[] handicapAsInt = new int[4];

                    //put all 4 numbers in an array to find the lowest
                    for (int g = 0; g < scratchArray.Length; g++)
                    {
                        if (scratchArray[g].Text != "")
                        {
                            if(int.TryParse(scratchArray[g].Text, out int result))
                            {
                                scratchAsInt[g] = result;
                            }
                            else
                            {
                                scratchAsInt[g] = 0;
                            }

                            if(int.TryParse(handicappArray[g].Text, out int handicapResult))
                            {
                                handicapAsInt[g] = handicapResult;
                            }
                            else
                            {
                                handicapAsInt[g] = 0;
                            }
                        }
                        handicapTotal += handicapAsInt[g];
                    }

                    scratchTotal -= scratchAsInt.Min();
                    handicapTotal -= handicapAsInt.Min();

                    txtScratchTotal.Text = scratchTotal.ToString();
                    txtHandicapTotal.Text = handicapTotal.ToString();
                }
            }

            // If you enter in the last games score it will automatically
            // click the Add/Update record button 
            if (txtScratchScore4.Focused == true)
            {
                //when last score is entered bowler record will be added
                btnNew.PerformClick();
            }
            else
            {
                SendKeys.Send("{TAB}");
            }
        }

        /// <summary>
        /// finds the handicap score (adds handicap to score)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="score"></param>
        private void HandicapTotal(string id, int score)
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

        #region New Recap
        /// <summary>
        /// Activates when Add New/Update Record is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void NewRecap(object sender, EventArgs e)
        {
            AddNewUpdateRecord();
        }

        /// <summary>
        /// enter a tournamnet participant into a specific tournament
        /// save scores and info in database
        /// </summary>
        private void AddNewUpdateRecord()
        {
            ReEnableNavigation();

            if (AreAllScratchScoreBoxesEmpty())
            {
                MessageBox.Show("A player must play at least one game to be added to the tournament.", "Uh-Oh!");
            }
            else if (IsValid())
            {
                //gets the current tournament from the database 
                Tournament currTourney = TournamentDB.GetTourneyByID(Convert.ToInt32(cbxTourneyDropDown.SelectedValue));

                //get all the current members participating in the current tournament
                overallListOfParticipants = TournamentDB.GetTournamentMemberList(currTourney);

                int squad = GetCurrentSquadNumber();

                //get the member from the database using the number from the memnum textbox
                currentMem = MemberDB.GetMember(Convert.ToInt32(txtMemberNum.Text), RegionID);
                player.Member = currentMem;

                player.Game = new Game();
                player.ParticipantRegionID = RegionID;
                var db = new NineTapDb();

                int gameId = GameDB.GetGameID(db, currentMem.Id, currTourney.Id, squad);

                int parID = ParticipantsDB.GetParticipantID(db, currentMem.Id, currTourney.Id, squad);

                if (parID != 0)
                {
                    player.Id = parID;
                }

                player.Game.Id = gameId;

                //selects the ID of the combobox of tournaments and stores the
                //tournament property within the participants class.
                player.Tournament = currTourney;
                player.Squad = squad;

                //defaults money earned to 0, or enters text box amount
                if (txtMoney.Text == "" || txtMoney.Text == null)
                    player.Game.MoneyWon = 0;
                else
                    player.Game.MoneyWon = Convert.ToDecimal(txtMoney.Text);


                if ((!IsNumeric(txtScratchScore1.Text.Trim()) && !String.IsNullOrWhiteSpace(txtScratchScore1.Text))
                    || (!IsNumeric(txtScratchScore2.Text.Trim()) && !String.IsNullOrWhiteSpace(txtScratchScore2.Text))
                    || (!IsNumeric(txtScratchScore3.Text.Trim()) && !String.IsNullOrWhiteSpace(txtScratchScore3.Text))
                    || (!IsNumeric(txtScratchScore4.Text.Trim()) && !String.IsNullOrWhiteSpace(txtScratchScore4.Text)))
                {
                    MessageBox.Show("Please enter only numbers", "Non-Integer Scores Not Allowed");
                    return;
                }
                else
                {
                    player.Game.Game1 = IsEmpty(txtScratchScore1)
                        ? null
                        : (int?)Convert.ToInt32((scratchArray[0].Text));

                    player.Game.Game2 = IsEmpty(txtScratchScore2)
                        ? null
                        : (int?)Convert.ToInt32((scratchArray[1].Text));

                    player.Game.Game3 = IsEmpty(txtScratchScore3)
                        ? null
                        : (int?)Convert.ToInt32((scratchArray[2].Text));

                    player.Game.Game4 = IsEmpty(txtScratchScore4)
                        ? null
                        : (int?)Convert.ToInt32((scratchArray[3].Text));

                    Game currentGame = GetScoresById(currentMem.Id);

                    if (currentGame == null)
                    {
                        player.Game.Bonus = currentMem.Bonus;
                        player.Game.Handicap = currentMem.Handicap;
                    }
                    else
                    {
                        player.Game.Bonus = currentGame.Bonus;
                        player.Game.Handicap = currentGame.Handicap;
                    }

                    player.Game.gameRegionID = RegionID;

                    // if compEntry checkbox is checked, set IsComp to true in game table
                    if (cbCompEntry.Checked)
                    {
                        player.Game.IsComp = true;
                    }

                    db.SaveChanges();

                    try
                    {
                        TournamentDB.AddMemberToTournament(player);
#if DEBUG
                        MessageBox.Show(@"Bowler Added Successfully to Tournament!");
#endif
                        //if btnNew is being clicked 
                        if (btnNew.ContainsFocus)
                        {
                            //clears score boxes
                            ResetScores();
                        }
                        overallListOfParticipants = TournamentDB.GetTournamentMemberList(currTourney);
                        RecordIndexAfterAddUpdate(overallListOfParticipants);
                    }
                    catch (MemberAccessException ex)
                    {
                        MessageBox.Show(ex.Message);
                    }

                    //UPDATE LASTBOWLED DATE
                    //Sets last bowled to now and updates DB record
                    if (DateTime.Now > currentMem.LastBowled || currentMem.LastBowled == null)
                    {
                        currentMem.LastBowled = DateTime.Now;
                        MemberDB.AddOrUpdateMember(currentMem);
                    }
                }
                Refresh(false);
            }
            else
            {
                MessageBox.Show("Please Fill out the Participants information!");
            }
        }

        /// <summary>
        /// Returns true if all of the scratch score boxes are empty or whitespace
        /// </summary>
        private bool AreAllScratchScoreBoxesEmpty()
        {
            return string.IsNullOrEmpty(txtScratchScore1.Text.Trim())
                            && string.IsNullOrEmpty(txtScratchScore2.Text.Trim())
                            && string.IsNullOrEmpty(txtScratchScore3.Text.Trim())
                            && string.IsNullOrEmpty(txtScratchScore4.Text.Trim());
        }

        /// <summary>
        /// clears game scores of bowlers.
        /// </summary>
        private void ResetScores()
        {
            ResetFields();
            txtMemberNum.Focus();
            Clear();
        }

        #endregion
        /// <summary>
        /// Checks a string for numeric values
        /// true if all are numeric
        /// </summary>
        /// <param name="str"></param>
        /// <returns>isNum</returns>
        public bool IsNumeric(string str)
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
            //sets first index on start up and switching of tournaments
            int temp = 0;

            if (cbxTourneyDropDown.SelectedIndex < 0)
            {
                lblRecord.Text = "Record " + (temp) + " / " + players.Count;
            }
            else if (players.Count == 0)
            {
                lblRecord.Text = "Record " + (temp) + " / " + players.Count;
            }
            else
            {
                currentIndex = 0;
                int playerSquadNumber = players[currentIndex].Squad;
                CheckSquadCheckBoxes(playerSquadNumber);

                lblRecord.Text = "Record " + (currentIndex + 1) + " / " + players.Count;
                txtMemberNum.Text = players[currentIndex].Member.Number.ToString();
                FillMember();
            }
        }

        /// <summary>
        /// Checks the radio button for the corresponding squad
        /// </summary>
        /// <param name="playerSquadNumber">Squad number of player to check</param>
        private void CheckSquadCheckBoxes(int playerSquadNumber)
        {
            if (playerSquadNumber == 1)
            {
                rdoSquad1.Checked = true;
            }
            else if (playerSquadNumber == 2)
            {
                rdoSquad2.Checked = true;
            }
            else if (playerSquadNumber == 3)
            {
                rdoSquad3.Checked = true;
            }
            else if (playerSquadNumber == 4)
            {
                rdoSquad4.Checked = true;
            }
            else if (playerSquadNumber == 5)
            {
                rdoSquad5.Checked = true;
            }
            else if (playerSquadNumber == 6)
            {
                rdoSquad6.Checked = true;
            }
            else if (playerSquadNumber == 7)
            {
                rdoSquad7.Checked = true;
            }
            else if (playerSquadNumber == 8)
            {
                rdoSquad8.Checked = true;
            }
        }

        /// <summary>
        /// updates the record index after the button is clicked, making the record go to the next potential added player
        /// </summary>
        /// <param name="pat"> a list of participant objects </param>
        public void RecordIndexAfterAddUpdate(List<Participant> pat)
        {
            lblRecord.Text = "Record " + (pat.Count) + " / " + pat.Count;
            currentIndex = pat.Count;
        }

        /// <summary>
        /// Find the bowler tournament record number with the currently selected squad
        /// </summary>
        /// <param name="participantList">The list of participants for the current tournament</param>
        private void UpdateRecordNumber(List<Participant> participantList)
        {
            //on enter, find the first index in which the member occurs in the tournament
            if (!selectedTournament.Doubles)
            {
                if (txtMemberNum.Text != "" && txtMemberNum.Text.All(Char.IsDigit))
                {
                    currentMem = MemberDB.GetMember(Convert.ToInt32(txtMemberNum.Text), RegionID);

                    int currentSquadNumber = GetCurrentSquadNumber();

                    for (int i = 0; i < participantList.Count; i++)
                    {
                        if (currentMem.Id == participantList[i].Member.Id && participantList[i].Squad == currentSquadNumber)
                        {
                            lblRecord.Text = "Record " + (i + 1) + " / " + participantList.Count;
                            currentIndex = i;

                            break;
                        }

                        //if no break occurs, set the current index to that of the next potential index
                        lblRecord.Text = "Record " + (participantList.Count) + " / " + participantList.Count;
                        currentIndex = participantList.Count;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the currently selected squad number
        /// </summary>
        /// <returns></returns>
        private int GetCurrentSquadNumber()
        {
            if (rdoSquad1.Checked)
                return 1;
            else if (rdoSquad2.Checked)
                return 2;
            else if (rdoSquad3.Checked)
                return 3;
            else if (rdoSquad4.Checked)
                return 4;
            else if (rdoSquad5.Checked)
                return 5;
            else if (rdoSquad6.Checked)
                return 6;
            else if (rdoSquad7.Checked)
                return 7;
            else if (rdoSquad8.Checked)
                return 8;
            throw new Exception("A squad must be checked!");
        }

        public void RecordIndexOnSquadSwitch()
        {
            int squad = 0;

            if (selectedTournament.Doubles == false && switchingParticipents == false)
            {
                if (txtMemberNum.Text != "")
                {
                    squad = GetCurrentSquadNumber();

                    for (int i = 0; i < overallListOfParticipants.Count; i++)
                    {
                        if (currentMem.Id == overallListOfParticipants[i].Member.Id && overallListOfParticipants[i].Squad == squad)
                        {
                            lblRecord.Text = "Record " + (i + 1) + " / " + overallListOfParticipants.Count;
                            currentIndex = i;
                            break;
                        }
                    }
                }
            }
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
        /// gets the scores from games table by joining participants and tourneys by id 
        /// where member id = participant.member ID and selectedtourney id = tourney id
        /// </summary>
        /// <param name="memberID"></param>
        /// <returns></returns>

        public Game GetScoresById(int memberID)
        {
            int squad = GetCurrentSquadNumber();

            try
            {
                int selectedTournamentId = Convert.ToInt32(cbxTourneyDropDown.SelectedValue);
                return GameDB.GetGameInTournament(memberID, selectedTournamentId, squad);
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine("Error Number : " + ex.Message);
                return null;
            }
        }
        /// <summary>
        /// clears memberNum, txtScratchScores, and High Game textboxes
        /// </summary>
        private void Clear()
        {
            txtMemberNum.Clear();
            //            richTextBox1.Clear();
            //            richTextBox2.Clear();
            //            richTextBox3.Clear();
        }

        /// <summary>
        /// increments to the next participant in the tournament
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnRightArrow_Click(object sender, EventArgs e)
        {
            switchingParticipents = true;
            currentIndex++;

            // Disables buttons and breaks function
            // if already at the last record
            if (currentIndex >= overallListOfParticipants.Count)
            {
                currentIndex--;
                btnRightArrow.Enabled = false;
                btnLastRecord.Enabled = false;
                return;
            }

            ReEnableNavigation();

            // Disables buttons if last record
            // is reached
            if (currentIndex + 1 >= overallListOfParticipants.Count)
            {
                btnRightArrow.Enabled = false;
                btnLastRecord.Enabled = false;
            }

            txtMemberNum.Text = Convert.ToString(overallListOfParticipants[currentIndex].Member.Number);
            int playerSquadNumber = overallListOfParticipants[currentIndex].Squad;
            CheckSquadCheckBoxes(playerSquadNumber);

            lblRecord.Text = "Record " + (currentIndex + 1) + " / " + overallListOfParticipants.Count;

            FillMember();
            switchingParticipents = false;
        }

        /// <summary>
        /// decrements to the previous participant in the tournament
        /// </summary>
        private void BtnLeftArrow_Click(object sender, EventArgs e)
        {
            switchingParticipents = true;

            currentIndex--;
            // Disables buttons and breaks function
            // if already at the first record
            if (currentIndex <= -1)
            {
                currentIndex++;
                btnLeftArrow.Enabled = false;
                btnFirstRecord.Enabled = false;
                return;
            }

            ReEnableNavigation();

            // Disables buttons if first record
            // is reached
            if (currentIndex <= 0)
            {
                btnLeftArrow.Enabled = false;
                btnFirstRecord.Enabled = false;
            }

            txtMemberNum.Text = Convert.ToString(overallListOfParticipants[currentIndex].Member.Number);
            int playerSquadNumber = overallListOfParticipants[currentIndex].Squad;
            CheckSquadCheckBoxes(playerSquadNumber);

            lblRecord.Text = "Record " + (currentIndex + 1) + " / " + overallListOfParticipants.Count;

            FillMember();

            switchingParticipents = false;
        }

        /// <summary>
        /// Goes to the first record.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnFirstRecord_Click(object sender, EventArgs e)
        {

            switchingParticipents = true;

            // Disables buttons and breaks function
            // if already at the 1st record
            if (currentIndex <= -1)
            {
                btnLeftArrow.Enabled = false;
                btnFirstRecord.Enabled = false;
                return;
            }
            if(overallListOfParticipants.Count > 1)
            {
                // Sets currentIndex to 1 in order to get the 1st record
                currentIndex = 0;

                lblRecord.Text = "Record " + (currentIndex + 1) + " / " + overallListOfParticipants.Count;
                ReEnableNavigation();

                // Gets the 1st record in the list
                txtMemberNum.Text = Convert.ToString(overallListOfParticipants[0].Member.Number);

                int playerSquadNumber = overallListOfParticipants[currentIndex].Squad;
                CheckSquadCheckBoxes(playerSquadNumber);

                FillMember();

                // Disables buttons left and first record buttons 
                // if there are no more records go back to.
                btnLeftArrow.Enabled = false;
                btnFirstRecord.Enabled = false;


                switchingParticipents = false;
            }

        }

        /// <summary>
        /// Goes to the last record
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLastRecord_Click(object sender, EventArgs e)
        {
            MoveToLastRecordOfMemberScores();
        }

        private void MoveToLastRecordOfMemberScores()
        {
            //If there are no participants in the current tournament
            if (overallListOfParticipants == null)
                return;

            switchingParticipents = true;

            // Disables buttons and breaks function
            // if already at the last record
            if (currentIndex >= overallListOfParticipants.Count)
            {
                btnRightArrow.Enabled = false;
                btnLastRecord.Enabled = false;
                return;
            }

            // Sets currentIndex to the size of total
            currentIndex = overallListOfParticipants.Count - 1;

            lblRecord.Text = "Record " + (currentIndex + 1) + " / " + overallListOfParticipants.Count;
            ReEnableNavigation();

            // Gets the last record from the list
            txtMemberNum.Text = Convert.ToString(overallListOfParticipants[overallListOfParticipants.Count - 1].Member.Number);
            int lastMemberSquad = overallListOfParticipants[overallListOfParticipants.Count - 1].Squad;
            CheckSquadCheckBoxes(lastMemberSquad);

            FillMember();

            // Disables buttons right and last record buttons 
            // if there are no more records go to.
            btnLastRecord.Enabled = false;
            btnRightArrow.Enabled = false;


            switchingParticipents = false;
        }

        /// <summary>
        /// Re enables navigation buttons
        /// </summary>
        private void ReEnableNavigation()
        {
            btnLeftArrow.Enabled = true;
            btnRightArrow.Enabled = true;
            btnFirstRecord.Enabled = true;
            btnLastRecord.Enabled = true;
        }


        /// <summary>
        /// opens the new tournament form via creating a new from by referencing the form itself
        /// </summary>
        private void BtnNewTournament_Click(object sender, EventArgs e)
        {
            var newfrmNewTournament = Application.OpenForms["frmNewTournament"] as FrmNewTournament;
            ((FrmMain)MdiParent).OpenOrDisplayForm(ref newfrmNewTournament);
            newfrmNewTournament.Dock = DockStyle.None;
            rdoSquad1.Checked = true;
        }

        /// <summary>
        /// updates record index when tourney is changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbxTourneyDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            // resets the fields when a different tournament is selected
            ResetFields();

            // Used to find out if user actually clicked a different tournament instead of just Member Scores loading.
            int prevTourneyId = (selectedTournament == null) ? 0 : selectedTournament.Id;
            
            // assigns the selectedTournament variable as the selected Tournament from the comboBox
            selectedTournament = (Tournament)cbxTourneyDropDown.SelectedItem;
            int currTourneyId;

            // determines whether the tournament is a double tourney or not, then enables or disables the single and/or double textBox selection option
            if (selectedTournament == null)
            {
                rdoScratchScore.Visible = false;
                txtMemberNum.Enabled = false;
                btnRecapByPin.Enabled = false;

                RadioIntialize();
                rdoHandicapScore.Visible = false;
                rdoScratchScore.Visible = false;

                currTourneyId = 0;
            }
            else
            {
                rdoScratchScore.Visible = true;
                txtMemberNum.Enabled = true;
                EnableButtonsWhenValidTournamentSelected();
                RadioIntialize();
                btnDelete.Enabled = true;
                rdoHandicapScore.Visible = true;
                rdoScratchScore.Visible = true;

                currTourneyId = selectedTournament.Id;
            }

            if (cbxTourneyDropDown.SelectedIndex < 0)
            {
                lblRecord.Text = "Record 0" + " / " + "0";
                rdoHandicapScore.Visible = false;
                rdoScratchScore.Visible = false;
                DisableButtonsWhenValidTournamentSelected();
            }

            if (cbxTourneyDropDown.SelectedIndex >= 0 && cbxTourneyDropDown.Visible)
            {
                // resets the current index to zero when changing the tournament
                currentIndex = 0;
                // Gets the record for the selected tournament
                overallListOfParticipants = TournamentDB.GetTournamentMemberList(selectedTournament);
                RecordIndex(overallListOfParticipants);
                Refresh(false);
                rdoHandicapScore.Visible = true;
                rdoScratchScore.Visible = true;

                // sets focus to member num becuse that is what a user will need next
                txtMemberNum.Focus();
            }
            #region Jake's Section
            // clear the temp variables for the money earned for tourn results
            if (TempVariablesForGlobalLevel.MoneyEarnings != null && prevTourneyId != currTourneyId)
            {
                TempVariablesForGlobalLevel.MoneyEarnings.Clear();
            }  
        }

        /// <summary>
        /// Enables buttons to select when valid Tournament is selected
        /// </summary>
        private void EnableButtonsWhenValidTournamentSelected()
        {
            btnStats.Enabled = true;

            if (currentIndex > 1)
            {
                btnLeftArrow.Enabled = true;
            }

            btnRightArrow.Enabled = true;
            btnPlaceStandings.Enabled = true;
            btnRecapByPin.Enabled = true;
        }

        /// <summary>
        /// Disables buttons to select when invalid Tournament is selected
        /// </summary>
        private void DisableButtonsWhenValidTournamentSelected()
        {
            btnStats.Enabled = false;
            btnLeftArrow.Enabled = false;
            btnRightArrow.Enabled = false;
            btnFirstRecord.Enabled = false;
            btnLastRecord.Enabled = false;
        }

        /// <summary>
        /// validation method for form fields
        /// </summary>
        /// <returns>boolean</returns>
        public bool IsValid()
        {
            //Checks if selected tournament is null
            if (cbxTourneyDropDown.SelectedValue == null)
                return false;

            //Checks if member number is blank
            if (txtMemberNum.Text == "")
                return false;

            //Checks all score boxes and asks if you want to enter member without scores
            if (string.IsNullOrEmpty(txtScratchScore1.Text.Trim()) || string.IsNullOrEmpty(txtScratchScore2.Text.Trim()) || string.IsNullOrEmpty(txtScratchScore3.Text.Trim()) || string.IsNullOrEmpty(txtScratchScore4.Text.Trim()))
            {
                DialogResult result = MessageBox.Show("Are you sure you want to continue with a score missing?", "Are you sure?",
                                                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.No)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Search for tours by location.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnTourSearch_Click(object sender, EventArgs e)
        {
            List<Tournament> tours = new List<Tournament>();
            FrmTourSearch tourSearch = new FrmTourSearch(tours, RegionID);
            tourSearch.ShowDialog();
#if DEBUG
            foreach (Tournament tour in tours)
            {
                Console.WriteLine(tour.TourneyNameDate);
            }
#endif
            //Populates dropdown box with tournaments
            if (tours.Count > 0)
            {
                cbxTourneyDropDown.DataSource = tours;
                cbxTourneyDropDown.DisplayMember = "TourneyNameDate";
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
        //Calls refresh method on radiobutton change
        private void RdoScratchScore_CheckedChanged(object sender, EventArgs e)
        {
            Refresh(true);
        }

        private void RdoHandicapScore_CheckedChanged(object sender, EventArgs e)
        {
            Refresh(true);
        }

        // NOTE: This listOfTopScore is never populated/ultilized
        //List<TopScores> listOfTopScore = new List<TopScores>();
        IComparer<MemberScores> scoreComparer = new Calculations.MemberScoresComparer();

        /// <summary>
        /// pass true if you are changing the radio buttons and only want to refresh the bottom box.
        /// </summary>
        /// <param name="seriesChange"></param>
        public void Refresh(bool seriesChange)
        {
            var scores = new List<MemberScores>();
         // NOTE: This listOfTopScore doesn't need to be cleared because there was never 
         //       anything in it
         //   listOfTopScore.Clear();

            try
            {
                int selectedTourney = selectedTournament.Id;

                var listOfParticipants = ParticipantsDB.GetParticipants(selectedTournament.Id);

                var topScores = listOfParticipants.GroupBy(p => p.Member.Id).Select(pg => pg.Max()).ToList();

                int qualifyBySquadNumber = GetSquadResultsNumberChecked();

                //TAKES A TOURNAMENT ID AND SQUAD NUMBER AND FILTERS FOR A LIST OF PARTICIPANTS.
                if (qualifyBySquadNumber > 0 && qualifyBySquadNumber <= 8)
                    listOfParticipants = listOfParticipants.Where(p => p.Squad == qualifyBySquadNumber).ToList();

                else if (howManySquadsCanBeFiltered.Count > 0 && qualifyBySquadNumber == 9)
                    //filters out each squad
                    //take the list of participants where => if the squad number equals to any of the filtered numbers.
                    listOfParticipants = listOfParticipants
                        .Where(p => howManySquadsCanBeFiltered.Any(h => h == p.Squad)).ToList();
                try
                {
                    var participantsGameViewModels = new List<ParticipantsGameViewModel>();

                    var topParticipantGameViewModels = new List<TopParticipantGameViewModel>();

                    // makes list of ParticipantsGameViewModel which will be used to populate scratch game and handicap game
                    // listboxes which only allow 1 top game per person per squad
                    foreach (Participant currParticipant in listOfParticipants)
                    {
                        // creates temp variable for PaticipantsGameViewModel to store necessary info for each person 
                        ParticipantsGameViewModel currTopScoreViewModel =
                            new ParticipantsGameViewModel(
                            /* MemberNo  */ currParticipant.Member.Number,
                            /* FirstName */ currParticipant.Member.FirstName,
                            /* LastName  */ currParticipant.Member.LastName,
                            /* Squad */ currParticipant.Squad,
                            /* HighScore */ currParticipant.Game.AllGameScores().Max(),
                            /* Handicap  */ currParticipant.Member.Handicap,
                            /* Bonus */ currParticipant.Member.Bonus
                            );

                        // adds person to list<ParticipantsGameViewModel>
                        participantsGameViewModels.Add(currTopScoreViewModel);
                    }

                    foreach (Participant currParticipant in listOfParticipants)
                    {
                        //Gets all of the game scores that are valid (that have a value)
                        var allScoresWithOutNullGames = currParticipant.Game.AllGameScores().Where(g => g.HasValue);

                        //totals all games with out nulls/valid score
                        int? totalScore = allScoresWithOutNullGames.Sum();

                        //Sets a collection of all the games to a new variable.
                        var top4Games = allScoresWithOutNullGames;

                        //Sets a collection of all the games using the 3 out of 4 ruleset
                        var top3Games = FrmTournamentStats.GetTop3OutOf4(top4Games.ToList());

                        TopParticipantGameViewModel currTopScoreViewModel =
                            new TopParticipantGameViewModel(
                            /* MemberNo  */ currParticipant.Member.Number,
                            /* FirstName */ currParticipant.Member.FirstName,
                            /* LastName  */ currParticipant.Member.LastName,
                            /* Placeing  */ 0,
                            /* ScratchTotal */ currParticipant.Game.AllGameScores().Sum().Value,
                            /* top3ScratchScore  */ top3Games.Sum(),
                            /* top3HandicapScore */ top3Games.Sum() +
                                                    (3 * currParticipant.Member.Handicap) +
                                                    (3 * currParticipant.Game.Bonus),
                            /* Game1 */ currParticipant.Game.Game1,
                            /* Game2 */ currParticipant.Game.Game2,
                            /* Game3 */ currParticipant.Game.Game3,
                            /* Game4 */ currParticipant.Game.Game4,
                            /* Handicap */ currParticipant.Game.Handicap,
                            /* Bonus  */ currParticipant.Game.Bonus.Value,
                            /* gameID */ currParticipant.Game.Id,
                            /* squad  */ currParticipant.Squad
                            );

                        topParticipantGameViewModels.Add(currTopScoreViewModel);
                    }

                    // variable used to update lblHighSelected appropriately
                    Boolean isGame = true;

                    if (rdoGameHC.Checked)
                    {
                        // display data in the list boxes
                        // orders list by highest handicap score game to lowest
                        participantsGameViewModels = participantsGameViewModels
                            .OrderByDescending(t => t.HighScore + t.Handicap + t.Bonus).ToList();
                        // links handicap score listbox to list
                        lbxHighSelected.DataSource = participantsGameViewModels;
                        // displays specific tostring for displaying info dealing with high handicap score game
                        lbxHighSelected.DisplayMember = "HandicapScoreToString";
                    }

                    else if (rdoGameSC.Checked)
                    {
                        // orders list by highest scratch score game to lowest
                        participantsGameViewModels = participantsGameViewModels.OrderByDescending(t => t.HighScore).ToList();
                        // links scratch score listbox to list
                        lbxHighSelected.DataSource = participantsGameViewModels;
                        // displays specific tostring for displaying info dealing with high scratch score game
                        lbxHighSelected.DisplayMember = "ScratchScoreToString";

                    }

                    // for high games series listbox (third listbox)
                    else if (rdoHighSeries.Checked)
                    {
                        isGame = false;
                        // if scratch score radio button is checked
                        if (rdoScratchScore.Checked)
                        {
                            // orders list by highest scoring scratch score total to lowest
                            if (selectedTournament.ThreeOutOf4)
                            {
                                topParticipantGameViewModels = topParticipantGameViewModels.OrderByDescending(t => t.Top3ScratchScore).ToList();
                            }
                            else
                            {
                                topParticipantGameViewModels = topParticipantGameViewModels.OrderByDescending(t => t.ScratchTotal).ToList();
                            }
                            
                            // links game series listbox to list
                            lbxHighSelected.DataSource = topParticipantGameViewModels;

                            //displays specific tostring for displaying info dealing with scratch score total
                            lbxHighSelected.DisplayMember = nameof(TopParticipantGameViewModel.ScratchTotalToString);
                        }
                        // if handicap score radio button is checked
                        else if (rdoHandicapScore.Checked)
                        {
                            // orders list by highest scoring handicap score total to lowest
                            if (selectedTournament.ThreeOutOf4)
                            {
                                topParticipantGameViewModels = topParticipantGameViewModels.OrderByDescending(t => t.Top3HandiScores).ToList();
                            }
                            else
                            {
                                topParticipantGameViewModels = topParticipantGameViewModels.OrderByDescending(t => t.HandicapScore).ToList();
                            }

                            // links game series listbox to list
                            lbxHighSelected.DataSource = topParticipantGameViewModels;

                            // displays specific tostring for displaying info dealing with handicap score total
                            lbxHighSelected.DisplayMember = nameof(TopParticipantGameViewModel.HandicapTotalToString);
                        }
                    }

                    UpdateHighSelectedLabel(isGame);

                }
                catch (SqlException)
                {
                    //what is the 3rd box?
                    // NOTE: This listOfTopScore doesn't need to be cleared because there was never 
                    //       anything in it
                    // listOfTopScore
                    //   .Clear(); //filter out if there is no one on the squad yet so the 3rd box won't get populated
                }
            }
            catch
            {

            }
        }

        /// <summary>
        /// Used to update label for lbxHighSelected panel, 
        /// pass in true if HighHC or HighSC, false for High Series
        /// </summary>
        /// <param name="isGame"></param>
        public void UpdateHighSelectedLabel(Boolean isGame)
        {
            String firstCol = "Game ";
            if ( !isGame )
            {
                firstCol = "Series ";
            }

            lblHighSelected.Text = firstCol + "[Member No.] --- (Name)";
        }

        /// <summary>
        /// Calculates each bowler's place standing. Accounts for ties.
        /// </summary>
        /// <param name="winners"></param>
        private static void CalculatePlaceStanding(List<TopParticipantGameViewModel> winners, bool scoreToOrganizeBy)
        {
            int place = 1;

            if (scoreToOrganizeBy == false)
            {
                for (int i = 0; i < winners.Count; i++)
                {
                    if (i > 0 && winners[i].ScratchTotal == winners[i - 1].ScratchTotal)
                    {
                        winners[i].Placing = winners[i - 1].Placing;
                    }
                    else
                    {
                        winners[i].Placing = place;
                    }
                    place++;
                }
            }

            if (scoreToOrganizeBy == true)
            {
                for (int i = 0; i < winners.Count; i++)
                {
                    if (i > 0 && winners[i].HandicapScore == winners[i - 1].HandicapScore)
                    {
                        winners[i].Placing = winners[i - 1].Placing;
                    }
                    else
                    {
                        winners[i].Placing = place;
                    }
                    place++;
                }
            }
        }

        //Gets nullable scratchscore
        private int? GetScratchScore(int? gameScore, int? gameHandicap)
        {
            return gameScore + gameHandicap;
        }

        private void BtnTournamentsByYear_Click(object sender, EventArgs e)
        {
            FrmTournamentsByYear listTournaments = new FrmTournamentsByYear(RegionID);
            listTournaments.ShowDialog();
        }
        //Called when stats btn is clicked
        private void BtnStats_Click(object sender, EventArgs e)
        {
            FrmTournamentStats tournamentStats = new FrmTournamentStats();
            tournamentStats.ShowDialog();
        }

        private void BtnRecapByPin_Click(object sender, EventArgs e)
        {
            FrmSelection selectTournament = new FrmSelection(RegionID);
            selectTournament.StartPosition = FormStartPosition.CenterParent;
            DialogResult t = selectTournament.ShowDialog();
            if (t != DialogResult.Cancel)
            {
                DialogResult mboxResult =
                    MessageBox.Show($"are you sure you want to print {selectTournament.selectedTournament.TourneyNameDate}?",
                        "Confirm Tournament", MessageBoxButtons.YesNo);
                if (mboxResult == DialogResult.Yes)
                {
                    Print.PrintByTour(selectTournament.selectedTournament);
                }
            }
        }

        private void BtnPlaceStandings_Click(object sender, EventArgs e)
        {
            MessageBox.Show("The place standings feature will be implemented in the future",
                "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // Saved for future use - Client wants this saved
            //FrmTournamentPlaceStandings form = new FrmTournamentPlaceStandings();
            //form.ShowDialog();
        }

        //runs fill member when enter key is pressed on text box
        private void TxtMemberNum_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                UpdateRecordNumberAndRetrieveParticpant();
            }
        }

        /// <summary>
        /// Updates the tournament record number for the chosen bowler and retrieves and displays the bowler information
        /// </summary>
        private void UpdateRecordNumberAndRetrieveParticpant()
        {
            Tournament currSelectedTournament = TournamentDB.GetTourneyByID(Convert.ToInt32(cbxTourneyDropDown.SelectedValue));
            List<Participant> allParticipantsInCurrTourney = TournamentDB.GetTournamentMemberList(currSelectedTournament);
            UpdateRecordNumber(allParticipantsInCurrTourney);
            FillMember();
        }

        /// <summary>
        /// Populates Tournament dropdown list to most recently modified tournament;
        /// </summary>
        public void PopulateSelectedTournament(Tournament currtourney)
        {
            List<Tournament> temp2 = TournamentDB.GetTournamentList(RegionID);

            for (int i = 0; i < temp2.Count; i++)
            {
                if (temp2[i].Id == currtourney.Id)
                {
                    cbxTourneyDropDown.SelectedIndex = i;
                }
            }
        }

        //opens the FinalizeTourn form, checks to make sure a tourn is selected.
        private void BtnFinalizeTounament_Click(object sender, EventArgs e)
        {
            if (cbxTourneyDropDown.SelectedIndex < 0)
            {
                MessageBox.Show("Please Select a Tournament");
            }
            else
            {
                //Since this takes 20+ seconds to display the DGV this displays a swirling loading indicator.
                Cursor.Current = Cursors.WaitCursor;
                Application.DoEvents();

                var newFrmFinalizeTournament = new FrmFinalizeTournament(selectedTournament, RegionID);
                newFrmFinalizeTournament.Dock = DockStyle.Right;
                newFrmFinalizeTournament.WindowState = FormWindowState.Normal;
                newFrmFinalizeTournament.ShowDialog();
            }

            //This sets it back to default arrow after the DGV is finish loading.
            Cursor.Current = Cursors.Default;
            Application.DoEvents();
        }

        /*******************************************************************************
        When the report section buttons are clicked, it will take them to the FrmMemberScoresReports to ask for how many they want to take for printing
        ********************************************************************************/
        private void BtnSenior_Click(object sender, EventArgs e)
        {
            //Checks if tournament is not selected
            if (cbxTourneyDropDown.SelectedIndex < 0)
            {
                MessageBox.Show("Please Select a Tournament");
            }
            else
            {
                List<MemberScores> temp = ParticipantsDB.GetSeniorMemberScores(selectedTournament.Id);

                //squadList is not used in Senior Report. Passes empty list.
                List<int> squadList = new List<int>();

                if (temp.Count != 0)
                {
                    int currentsNum = GetSquadResultsNumberChecked();

                    FrmMemberScoresReports report = new FrmMemberScoresReports(temp, selectedTournament, 0/*reportTypeNum, 0 for High game handicap/senior, 1 for game/high game, 2 for series/high series*/, currentsNum, squadList);
                    //report.Dock = DockStyle.Fill;
                    report.Show();
                }
                else
                {
                    MessageBox.Show("There are no particpants in this tournament.");
                }
            }
        }

        /// <summary>
        /// Get the squad number for the current Squad Results Radio Button that is checked
        /// Returns the number of the squad of 0 if "All Squads" is selected
        /// </summary>
        /// <returns></returns>
        private int GetSquadResultsNumberChecked()
        {
            int currentsNum = 0;

            if (cbFilterSquad1.Checked)
                currentsNum = 1;
            else if (cbFilterSquad2.Checked)
                currentsNum = 2;
            else if (cbFilterSquad3.Checked)
                currentsNum = 3;
            else if (cbFilterSquad4.Checked)
                currentsNum = 4;
            else if (cbFilterSquad5.Checked)
                currentsNum = 5;
            else if (cbFilterSquad6.Checked)
                currentsNum = 6;
            else if (cbFilterSquad7.Checked)
                currentsNum = 7;
            else if (cbFilterSquad8.Checked)
                currentsNum = 8;
            return currentsNum;
        }
        //called when report game is clicked
        private void BtnGame_Click(object sender, EventArgs e)
        {
            if (cbxTourneyDropDown.SelectedIndex < 0)
            {
                MessageBox.Show("Please Select a Tournament");
            }
            else
            {
                List<MemberScores> temp = ParticipantsDB.GetGameMemberScores(selectedTournament.Id);
                temp.Sort(scoreComparer);

                //seriesCurrentSquad is not used in Game Report. Passes empty
                List<int> squadList = new List<int>();

                //find out what squad is selected At the moment of series button click
                int currentsNum = GetSquadResultsNumberChecked();

                if (temp.Count != 0)

                {
                    FrmMemberScoresReports report = new FrmMemberScoresReports(temp, selectedTournament, ReportType.HighGame, currentsNum, squadList);
                    report.Show();
                }
                else
                {
                    MessageBox.Show("There are no particpants in this tournament.");
                }
            }
        }

        /// <summary>
        /// Called when the report series is clicked
        /// </summary>
        private void BtnSeries_Click(object sender, EventArgs e)
        {
            if (cbxTourneyDropDown.SelectedIndex < 0)
            {
                MessageBox.Show("Please Select a Tournament");
            }
            else
            {
                var temp = new List<MemberScores>();

                int qualifyBySquadNumber = GetSquadResultsNumberChecked();

                //Gets information from Filter Series by Squad checkboxes and gets the latest squad to pass when Series is clicked.
                List<bool> filterSeries = FormHelper.GetFilterSeriesList(GRPQBS1);
                List<int> squadList = FormHelper.SquadNumList(filterSeries);
                    
                #endregion
                // These 2 regions would recreate data that already exists on the page
                #region PRINTING HANDICAP TOURNAMENT RESULTS
                if (rdoHandicapScore.Checked)
                {
                    if (selectedTournament.ThreeOutOf4 && squadList.Contains(0))
                    {
                        temp = ParticipantsDB.GetStandingsForThreeOutOf4ByHandicap(selectedTournament.Id);
                    }
                    else if (selectedTournament.ThreeOutOf4 && !squadList.Contains(0))
                    {
                        temp = ParticipantsDB.GetStandingsForThreeOutOf4ByFilterSeriesByHandicap(squadList, selectedTournament.Id);
                    }
                    else if (!selectedTournament.ThreeOutOf4 && squadList.Contains(0))
                    {
                        temp = ParticipantsDB.GetStandingsForTournamentByHandicap(selectedTournament.Id);
                    }
                    else if (!selectedTournament.ThreeOutOf4 && !squadList.Contains(0))
                    {
                        temp = ParticipantsDB.GetStandingsForTournamentByFilterSeriesByHandicap(squadList, selectedTournament.Id);
                    }
                    #endregion

                    #region PRINTING SCRATCH TOURNAMENT RESULTS
                    else if (rdoScratchScore.Checked)
                    {
                        if (selectedTournament.ThreeOutOf4 && squadList.Contains(0))
                        {
                            temp = ParticipantsDB.GetStandingsForThreeOf4ByScratch(selectedTournament.Id);
                        }
                        else if (selectedTournament.ThreeOutOf4 && !squadList.Contains(0))
                        {
                            temp = ParticipantsDB.GetStandingsForThreeOf4ByFilterSeriesByScratch(squadList, selectedTournament.Id);
                        }
                        else if (!selectedTournament.ThreeOutOf4 && squadList.Contains(0))
                        {
                            temp = ParticipantsDB.GetStandingsForTournamentByScratch(selectedTournament.Id);
                        }
                        else if (!selectedTournament.ThreeOutOf4 && !squadList.Contains(0))
                        {
                            temp = ParticipantsDB.GetStandingsForTournamentByFilterSeriesByScratch(squadList, selectedTournament.Id);
                        }
                    }
                    #endregion

                    temp.Sort(scoreComparer);

                    if (temp.Count() != 0)
                    {
                        FrmMemberScoresReports report = new FrmMemberScoresReports(temp, selectedTournament, ReportType.HighSeries, qualifyBySquadNumber, squadList);
                        report.Show();
                    }
                    else
                    {
                        MessageBox.Show("Error: No Participants in selected Squad.");
                    }
                }
            }
        }

        //these change the value of the QBSnumber, allowing the director to filter the rich text boxes by squad, then calls the refresh method to update the rich textboxes information to 
        //display the tournament information but based on squad'
        #region changing the sqaud number
        private void CheckBoxAllResults_CheckedChanged(object sender, EventArgs e)
        {
            Refresh(false);
        }

        private void CheckBoxSquadResults_CheckChanged(object sender, EventArgs e)
        {
            if (cbxTourneyDropDown.Size != null)
            {
                Refresh(false);
            }
        }
        #endregion  

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            //Grabs the tournament from the selected tournament combobox and casts it to selected Tournament
            selectedTournament = (Tournament)cbxTourneyDropDown.SelectedItem;
            //Repopulates list of participants with the current tournament
            overallListOfParticipants = TournamentDB.GetTournamentMemberList(selectedTournament);


            //Checks to make sure the member Id textbox isn't empty
            if(txtMemberNum.Text == String.Empty)
            {
                MessageBox.Show("You must enter a member number.");
                return;
            }

            //needs to delete current member information from database in all important places
            if (overallListOfParticipants.Count == 0)
            {
                var confirm = MessageBox.Show(@"No players currently in tournament", @"Attention", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                RemoveParticipantFromTournament();
            }
            catch
            {
                MessageBox.Show("Current Stats Not added to Tournament yet.");
            }
            finally
            {
                RefreshMemberScoresForm();
                Cursor.Current = Cursors.Default;
            }
            ReEnableNavigation();
        }

        private void RefreshMemberScoresForm()
        {
            //resets all the fields back to what it would've looked like without such record existing
            ResetFields();
            Refresh(false);
            RecordIndex(overallListOfParticipants);
            overallListOfParticipants = TournamentDB.GetTournamentMemberList(selectedTournament);
            cbxTourneyDropDown.DisplayMember = nameof(Tournament.TourneyNameDate);
            cbxTourneyDropDown.ValueMember = nameof(Tournament.Id);
        }

        private void RemoveParticipantFromTournament()
        {
            Game g = GetScoresById(currentMem.Id);

            if (g != null) 
            {
                //Delete from player history
                PlayerHistory p = PlayerHistoryDB.GetPlayerHistoryByGameID(g.Id);
                if(p != null)
                {
                    PlayerHistoryDB.DeletePlayerHistory(p);
                }

                //Delete from FinalizeTemp
                FinalizeTemp ft = FinalizeTempDB.GetFinalizeID(GameDB.GetGame(g.Id));
                FinalizeTempDB.DeleteFinalizeTemp(ft);

            
                //Delete from Participants list
                Participant par = FinalizeTempDB.GetParticipantByGameId(g.Id);
                FinalizeTempDB.DeleteParticipant(par);
                overallListOfParticipants.Remove(par);
                if(currentIndex + 1 == overallListOfParticipants.Count)
                {
                    currentIndex--;
                }
                //Delete the game itself
                PlayerHistoryDB.DeleteGame(g);


                //corrects any changes to the members stats after finalizing to the last accurate data
                List<PlayerHistory> temp = PlayerHistoryDB.GetLastFiveTournaments(currentMem.Number, RegionID);
                currentMem.Handicap = temp[0].HandiCap;
                currentMem.Bonus = temp[0].Bonus;
                currentMem.StartAvg = temp[0].AVG; // avg will have to be adjusted manually by director if last player history avg was not correct
                currentMem.Average = Convert.ToInt32(temp[0].trueAVG);
                MemberDB.AddOrUpdateMember(currentMem);           
            }
        }

        private void BtnTournamentResults_Click(object sender, EventArgs e)
        {
            FrmTournamentResults form = new FrmTournamentResults();
            form.ShowDialog();
        }

        private void CheckBoxSquadNumber_CheckedChanged(object sender, EventArgs e)
                                        {
            //only run the code the code for the radio button that is checked
            if((sender as RadioButton).Checked)
            {
                ScoreAndTotalClear();
                RecordIndexOnSquadSwitch();
                FillMember();
            }
            
        }

		/// <summary>
		/// The resize event for the form
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void FrmMemberScores_Resize(object sender, EventArgs e)
		{
			FormHelper.SetFlowDirection(this, flpMemberScores, 1100, 766);
		}

        /// <summary>
        /// After the size of the form has been changed, it checks the pixel
        /// width and height to determine whether there needs to be scroll bars
        /// or not.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FlpMemberScores_SizeChanged(object sender, EventArgs e)
        {
            FormHelper.SetFlowControlScrollBars(this, flpMemberScores, 1300, 750);
        }

        //runs fill member when you tab out of text box
        private void TxtMemberNum_Leave(object sender, EventArgs e)
        {
            UpdateRecordNumberAndRetrieveParticpant();
        }

        private void CbAllSquads_CheckedChanged(object sender, EventArgs e)
        {
            cbFilterSquad1.Checked = false;
            cbFilterSquad2.Checked = false;
            cbFilterSquad3.Checked = false;
            cbFilterSquad4.Checked = false;
            cbFilterSquad5.Checked = false;
            cbFilterSquad6.Checked = false;
            cbFilterSquad7.Checked = false;
            cbFilterSquad8.Checked = false;

            //if all squads is selected then uncheck and disable squad selections
            if (cbAllSquads.Checked)
            {
                cbFilterSquad1.Enabled = false;
                cbFilterSquad2.Enabled = false;
                cbFilterSquad3.Enabled = false;
                cbFilterSquad4.Enabled = false;
                cbFilterSquad5.Enabled = false;
                cbFilterSquad6.Enabled = false;
                cbFilterSquad7.Enabled = false;
                cbFilterSquad8.Enabled = false;

                howManySquadsCanBeFiltered.Clear();
                Refresh(false);   
            }
            else
            {
                cbFilterSquad1.Enabled = true;
                cbFilterSquad2.Enabled = true;
                cbFilterSquad3.Enabled = true;
                cbFilterSquad4.Enabled = true;
                cbFilterSquad5.Enabled = true;
                cbFilterSquad6.Enabled = true;
                cbFilterSquad7.Enabled = true;
                cbFilterSquad8.Enabled = true;
            }
        }

        public int FilterCheck()
        {
            CheckBox[] filterSquadCheckBoxes = new CheckBox[]
            {
                cbFilterSquad1, cbFilterSquad2, cbFilterSquad3, cbFilterSquad4,
                cbFilterSquad5, cbFilterSquad6, cbFilterSquad7, cbFilterSquad8
            };

            return filterSquadCheckBoxes
                .Where(filterCheckBox => filterCheckBox.Checked).Count();
        }

        private void CbFilterSquad_CheckedChanged(object sender, EventArgs e)
        {
            object squadNumber = (((CheckBox)sender).Tag);
            SquadFilter(sender as CheckBox, Convert.ToByte(squadNumber));
        }

        private void SquadFilter(CheckBox squadCheckBox, byte squadNum)
        {
            if (FilterCheck() == selectedTournament.Squads)
            {
                howManySquadsCanBeFiltered.Clear();
                cbAllSquads.Checked = true;
            }
            else if (squadCheckBox.Checked == false && howManySquadsCanBeFiltered.Contains(squadNum))
            {
                howManySquadsCanBeFiltered.Remove(squadNum);
                if (FilterCheck() == 0)
                {
                    cbAllSquads.Checked = true;
                }
                else
                {
                    Refresh(false);
                }
            }
            else
            {
                howManySquadsCanBeFiltered.Add(squadNum);
                Refresh(false);
            }
        }

        private void LbxGameLeader_Click(object sender, EventArgs e)
        {
            ChangeToSelectedPerson(sender as ListBox);
        }

        private void ChangeToSelectedPerson(ListBox participantGamesListBox)
        {
            if(participantGamesListBox.SelectedItem is ParticipantsGameViewModel)
            {
                ParticipantsGameViewModel participant = participantGamesListBox.SelectedItem as ParticipantsGameViewModel;
                txtMemberNum.Text = participant.MemberNo.ToString();
                FillMember();
                FormHelper.SelectParticipantSquad(participant.Squad, groupBox1);
            }
            else if(participantGamesListBox.SelectedItem is TopParticipantGameViewModel)
            {
                TopParticipantGameViewModel participant = participantGamesListBox.SelectedItem as TopParticipantGameViewModel;
                txtMemberNum.Text = participant.MemberNo.ToString();
                FillMember();
                FormHelper.SelectParticipantSquad(participant.Squad, groupBox1);
            }
        }

        private void CheckBoxGameSC_CheckedChanged(object sender, EventArgs e)
        {
            Refresh(false);
        }

        private void CheckBoxHighSeries_CheckedChanged(object sender, EventArgs e)
        {
            Refresh(false);
        }
    }
}
