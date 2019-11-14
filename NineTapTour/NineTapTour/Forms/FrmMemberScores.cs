using NineTapTour.Database;
using NineTapTour.Models;
using NineTapTour.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Linq.Dynamic;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using static NineTapTour.Database.ReportHelper;

namespace NineTapTour.Forms
{
    #region Casey's Comments
    /// <summary>
    /// FrmMemberScores class.
    /// All tournament info and scores are entered here.
    /// </summary>
    public partial class frmMemberScores : Form
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
        public frmMemberScores()
        {
            InitializeComponent();
        }
        /// <summary>
        /// initializes all the radio buttons on the form and sets them to their correct default status.
        /// higher squad numbers will be available if the tournament was created for more squads.
        /// </summary>
        private void RadioIntialize()
        {
            checkSquadOne.TabStop = false;
            checkSquadTwo.TabStop = false;
            checkSquadThree.TabStop = false;
            checkSquadFour.TabStop = false;
            checkSquad5.TabStop = false;
            checkSquad6.TabStop = false;
            checkSquad7.TabStop = false;
            checkSquad8.TabStop = false;
            checkBoxHandicapScore.TabStop = false;
            cbAllSquads.TabStop = false;
            cbFilterSquad5.Visible = false;
            cbFilterSquad6.Visible = false;
            cbFilterSquad7.Visible = false;
            cbFilterSquad8.Visible = false;
            checkSquad5.Visible = false;
            checkSquad6.Visible = false;
            checkSquad7.Visible = false;
            checkSquad8.Visible = false;
            //rdoSquad5Results.Visible = false;
            //rdoSquad6Results.Visible = false;
            //rdoSquad7Results.Visible = false;
            //rdoSquad8Results.Visible = false;
            cbAllSquads.Checked = true;

            if (cbxTourneyDropDown.SelectedIndex >= 0)
            {
                if (selectedTournament.Squads == 5)
                {
                    checkSquad5.Visible = true;
                    //rdoSquad5Results.Visible = true;
                    cbFilterSquad5.Visible = true;


                }

                if (selectedTournament.Squads == 6)
                {
                    checkSquad5.Visible = true;
                    checkSquad6.Visible = true;
                    //rdoSquad5Results.Visible = true;
                    //rdoSquad6Results.Visible = true;
                    cbFilterSquad5.Visible = true;
                    cbFilterSquad6.Visible = true;
                }

                if (selectedTournament.Squads == 7)
                {
                    checkSquad5.Visible = true;
                    checkSquad6.Visible = true;
                    checkSquad7.Visible = true;
                    //rdoSquad5Results.Visible = true;
                    //rdoSquad6Results.Visible = true;
                    //rdoSquad7Results.Visible = true;
                    cbFilterSquad5.Visible = true;
                    cbFilterSquad6.Visible = true;
                    cbFilterSquad7.Visible = true;

                }

                if (selectedTournament.Squads == 8)
                {
                    checkSquad5.Visible = true;
                    checkSquad6.Visible = true;
                    checkSquad7.Visible = true;
                    checkSquad8.Visible = true;
                    //rdoSquad5Results.Visible = true;
                    //rdoSquad6Results.Visible = true;
                    //rdoSquad7Results.Visible = true;
                    //rdoSquad8Results.Visible = true;
                    cbFilterSquad5.Visible = true;
                    cbFilterSquad6.Visible = true;
                    cbFilterSquad7.Visible = true;
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
            checkBoxHandicapScore.Visible = false;
            checkBoxScratchScore.Visible = false;
            cbxTourneyDropDown.Visible = false;
            ResetFields();

            MemberStatus("", Color.Black, SystemColors.Control, true);

            List<Tournament> temp2 = TournamentDB.GetTournamentList(RegionID);

            ((FrmMain)MdiParent)._tournamentList = temp2;
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
                checkBoxHandicapScore.Visible = true;
                checkBoxScratchScore.Visible = true;
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
            chbCompEntry.Checked = false;
            txtHandicap.Clear();
            txtBonusPins.Clear();
            txtScratchScore1.Clear();
            txtScratchScore2.Clear();
            txtScratchScore3.Clear();
            txtScratchScore4.Clear();
            txtScratchTotal.Clear();
            listOfTopScore.Clear();
            txtMoney.Clear();
        }

        #region GetMember

        //Get players scores 
        private void GetScores(Game currentGame)
        {
            if (currentGame != null)
            {
                Tournament tourney = GetTournamentById(Convert.ToInt32(cbxTourneyDropDown.SelectedValue));
                int total = TournamentDB.GetTotalNumberParticipantsInTournament(tourney);
              
                lblRecord.Text = "Record " + (currentIndex + 1) + " / " + total;

                chbCompEntry.Checked = currentGame.IsComp ? true : false;

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
            Tournament currTourney = null;

            if (cbxTourneyDropDown.SelectedValue != null)
            {
                currTourney = GetTournamentById(Convert.ToInt32(cbxTourneyDropDown.SelectedValue));

                string searchNumber = txtMemberNum.Text;

                //don't do any further processing if there is no member number
                if (searchNumber.Trim() == string.Empty)
                    return;

                if (!int.TryParse(searchNumber, out int number))
                {
                    MessageBox.Show("Please input numbers only.", "Your attention please.");
                    return;
                }

                int memberNumber = Convert.ToInt16(txtMemberNum.Text);
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

            //this code will adjust the scratch and handicap total (textboxes) only if its a 3of4 tournament ( taking out the lowest game) 
            if (txtScratchScore1.Text != "" && txtScratchScore2.Text != "" && txtScratchScore3.Text != "" && txtScratchScore4.Text != "")
            {
                int handicapTotal = 0;

                if (selectedTournament.ThreeOutOf4 == true)
                {
                    int[] scratchasInt = new int[4];
                    int[] handicapasInt = new int[4];

                    //put all 4 numbers in an array to find the lowest
                    for (int g = 0; g < scratchArray.Length; g++)
                    {
                        if (scratchArray[g].Text != "")
                        {
                            try
                            {
                                scratchasInt[g] = Convert.ToInt32(scratchArray[g].Text);
                            }
                            catch
                            {
                                scratchasInt[g] = 0;
                            }

                            try
                            {
                                handicapasInt[g] = Convert.ToInt32(handicappArray[g].Text);
                            }
                            catch
                            {
                                handicapasInt[g] = 0;
                            }
                        }
                        handicapTotal += handicapasInt[g];
                    }

                    scratchTotal -= scratchasInt.Min();
                    handicapTotal -= handicapasInt.Min();

                    txtScratchTotal.Text = scratchTotal.ToString();
                    txtHandicapTotal.Text = handicapTotal.ToString();
                }
            }

            ////auto tab to the next textbox when textbox's length is 3.           
            if (tx.Text.Length == 3)
            {
                //if you enter in the last games score it will automatically be recorded with out pressing Add/Update
                if (txtScratchScore4.Text.Length == 3 && txtScratchScore4.Focused == true)
                {
                    //when last score is entered bowler record will be added
                    AddNewUpdateRecord();
                    btnNew.Focus();
                }
                else
                {
                    SendKeys.Send("{TAB}");
                }
            }
        }

        /// <summary>
        /// finds the handicap score (adds handicap to score)
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

        #region New Recap
        /// <summary>
        /// Activates when Add New/Update Record is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void newRecap(object sender, EventArgs e)
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

            if (IsValid())
            {
                //gets the current tournament from the database 
                Tournament currTourney = GetTournamentById(Convert.ToInt32(cbxTourneyDropDown.SelectedValue));

                //get all the current members participating in the current tournament
                overallListOfParticipants = TournamentDB.GetTournamentMemberList(currTourney);

                int squad = GetCurrentSquadNumber();  

                //get the member from the database using the number from the memnum textbox
                currentMem = MemberDB.GetMember(Convert.ToInt32(txtMemberNum.Text), RegionID);
                player.Member = currentMem;

                player.Game = new Game();
                player.ParticipantRegionID = RegionID;
                var db = new NineTapDb();

                var gameId = (from p in db.Participants
                    where p.Member.Id == currentMem.Id
                            && p.Tournament.Id == currTourney.Id
                            && p.Squad == squad
                    select p.Game.Id).FirstOrDefault();

                var parID = (from p in db.Participants
                    where p.Member.Id == currentMem.Id
                            && p.Tournament.Id == currTourney.Id
                            && p.Squad == squad
                    select p.Id).FirstOrDefault();

                var parList = (from p in db.Participants
                    select new
                    {
                        p.Id
                    }).ToList();

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

                if (string.IsNullOrEmpty(txtScratchScore1.Text.Trim()) || string.IsNullOrEmpty(txtScratchScore2.Text
                                                                            .Trim())
                                                                        || string.IsNullOrEmpty(txtScratchScore3.Text
                                                                            .Trim()) || string.IsNullOrEmpty(
                                                                            txtScratchScore4.Text.Trim()))
                {
                    MessageBox.Show("Please enter all scratch scores", "Blank Scores Not Allowed");
                    return;
                }
                else if (!isNumeric(txtScratchScore1.Text.Trim()) || !isNumeric(txtScratchScore2.Text.Trim())
                                                                    || !isNumeric(txtScratchScore3.Text.Trim()) ||
                                                                    !isNumeric(txtScratchScore4.Text.Trim()))
                {
                    MessageBox.Show("Please enter only numbers", "Non-Integer Scores Not Allowed");
                    return;
                }
                else
                {
                    player.Game.Game1 = IsEmpty(txtScratchScore1)
                        ? null
                        : (int?) Convert.ToInt32((scratchArray[0].Text));

                    player.Game.Game2 = IsEmpty(txtScratchScore2)
                        ? null
                        : (int?) Convert.ToInt32((scratchArray[1].Text));

                    player.Game.Game3 = IsEmpty(txtScratchScore3)
                        ? null
                        : (int?) Convert.ToInt32((scratchArray[2].Text));

                    player.Game.Game4 = IsEmpty(txtScratchScore4)
                        ? null
                        : (int?) Convert.ToInt32((scratchArray[3].Text));

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
                    if (chbCompEntry.Checked)
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
        /// clears game scores of bowlers.
        /// </summary>
        private void ResetScores()
        {
            ResetFields();
            txtMemberNum.Focus();
            Clear();
        }

        #endregion
        #endregion
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
                checkSquadOne.Checked = true;
            }
            else if (playerSquadNumber == 2)
            {
                checkSquadTwo.Checked = true;
            }
            else if (playerSquadNumber == 3)
            {
                checkSquadThree.Checked = true;
            }
            else if (playerSquadNumber == 4)
            {
                checkSquadFour.Checked = true;
            }
            else if (playerSquadNumber == 5)
            {
                checkSquad5.Checked = true;
            }
            else if (playerSquadNumber == 6)
            {
                checkSquad6.Checked = true;
            }
            else if (playerSquadNumber == 7)
            {
                checkSquad7.Checked = true;
            }
            else if (playerSquadNumber == 8)
            {
                checkSquad8.Checked = true;
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

        public void RecordIndexOnEnter(List<Participant> part)
        {
            //on enter, find the first index in which the member occurs in the tournament
            if (selectedTournament.Doubles == false)
            {
                if (txtMemberNum.Text != "" && txtMemberNum.Text.All(Char.IsDigit))
                {
                    currentMem = MemberDB.GetMember(Convert.ToInt32(txtMemberNum.Text), RegionID);

                    int currentSquadNumber = GetCurrentSquadNumber();

                    for (int i = 0; i < part.Count; i++)
                    {
                        if (currentMem.Id == part[i].Member.Id && part[i].Squad == currentSquadNumber)
                        {
                            lblRecord.Text = "Record " + (i + 1) + " / " + part.Count;
                            currentIndex = i;

                            break;
                        }

                        //if no break occurs, set the current index to that of the next potential index
                        lblRecord.Text = "Record " + (part.Count) + " / " + part.Count;
                        currentIndex = part.Count;
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
            if (checkSquadOne.Checked)
                return 1;
            else if (checkSquadTwo.Checked)
                return 2;
            else if (checkSquadThree.Checked)
                return 3;
            else if (checkSquadFour.Checked)
                return 4;
            else if (checkSquad5.Checked)
                return 5;
            else if (checkSquad6.Checked)
                return 6;
            else if (checkSquad7.Checked)
                return 7;
            else if (checkSquad8.Checked)
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
        /// get a tournament by selected id
        /// </summary>
        /// <param name="selectedTournamentId"></param>    
        private Tournament GetTournamentById(int selectedTournamentId)
        {
            try
            {
                Tournament selectedTournament = (from t in TournamentDB.GetTournamentList(RegionID)
                                                 where t.Id == selectedTournamentId
                                                 select t).Single();
            }
            catch
            {

            }
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
            Game memScores = new Game();
            int squad = 0;
            squad = GetCurrentSquadNumber();

            try
            {
                int selectedTournamentId = Convert.ToInt32(cbxTourneyDropDown.SelectedValue);

                memScores = (from t in db.Tournaments
                             join p in db.Participants on t.Id equals p.Tournament.Id
                             where t.Id == p.Tournament.Id
                             && memberID == p.Member.Id
                             && selectedTournamentId == t.Id
                             && p.Squad == squad
                             select p.Game).SingleOrDefault();

            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine("Error Number : " + ex.Message);
                return null;
            }
            return memScores;

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
        private void btnRightArrow_Click(object sender, EventArgs e)
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
        private void btnLeftArrow_Click(object sender, EventArgs e)
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
        private void btnFirstRecord_Click(object sender, EventArgs e)
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
        private void btnLastRecord_Click(object sender, EventArgs e)
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
        private void btnNewTournament_Click(object sender, EventArgs e)
        {
            var newfrmNewTournament = Application.OpenForms["frmNewTournament"] as frmNewTournament;
            ((FrmMain)MdiParent).OpenOrDisplayForm(ref newfrmNewTournament);
            newfrmNewTournament.Dock = DockStyle.None;
            checkSquadOne.Checked = true;
        }

        /// <summary>
        /// updates record index when tourney is changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxTourneyDropDown_SelectedIndexChanged(object sender, EventArgs e)
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
                checkBoxScratchScore.Visible = false;
                txtMemberNum.Enabled = false;
                btnRecapByPin.Enabled = false;

                RadioIntialize();
                checkBoxHandicapScore.Visible = false;
                checkBoxScratchScore.Visible = false;

                currTourneyId = 0;
            }
            else
            {
                checkBoxScratchScore.Visible = true;
                txtMemberNum.Enabled = true;
                EnableButtonsWhenValidTournamentSelected();
                RadioIntialize();
                btnDelete.Enabled = true;
                checkBoxHandicapScore.Visible = true;
                checkBoxScratchScore.Visible = true;

                currTourneyId = selectedTournament.Id;
            }

            if (cbxTourneyDropDown.SelectedIndex < 0)
            {
                lblRecord.Text = "Record 0" + " / " + "0";
                checkBoxHandicapScore.Visible = false;
                checkBoxScratchScore.Visible = false;
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
                checkBoxHandicapScore.Visible = true;
                checkBoxScratchScore.Visible = true;

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
            {
                return false;
            }
            //Checks if member number is blank
            if (txtMemberNum.Text == "")
            {
                return false;
            }
            //Checks all score boxes and asks if you want to enter member without scores
            if (string.IsNullOrEmpty(txtScratchScore1.Text.Trim()) || string.IsNullOrEmpty(txtScratchScore2.Text.Trim()) || string.IsNullOrEmpty(txtScratchScore3.Text.Trim()) || string.IsNullOrEmpty(txtScratchScore4.Text.Trim()))
            {
                DialogResult result = MessageBox.Show("Are you sure you want to continue with a score missing?", "Are you sure?",
                                                      MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.No)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            return true;
        }



        /// <summary>
        /// Search for tours by location.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTourSearch_Click(object sender, EventArgs e)
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
        private void rdoScratchScore_CheckedChanged(object sender, EventArgs e)
        {
            Refresh(true);
        }

        private void rdoHandicapScore_CheckedChanged(object sender, EventArgs e)
        {
            Refresh(true);
        }

        List<TopScores> listOfTopScore = new List<TopScores>();
        IComparer<MemberScores> scoreComparer = new Calculations.MemberScoresComparer();

        /// <summary>
        /// pass true if you are changing the radio buttons and only want to refresh the bottom box.
        /// </summary>
        /// <param name="seriesChange"></param>
        public void Refresh(bool seriesChange)
        {
            var scores = new List<MemberScores>();
            listOfTopScore.Clear();

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
                            new ParticipantsGameViewModel(currParticipant.Member.Number, currParticipant.Member.FirstName, currParticipant.Member.LastName, currParticipant.Squad,
                                currParticipant.Game.AllGameScores().Max(), currParticipant.Member.Handicap, currParticipant.Member.Bonus);

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
                        var top3Games = TournamentStats.GetTop3OutOf4(top4Games.ToList());

                        TopParticipantGameViewModel currTopScoreViewModel =
                            new TopParticipantGameViewModel(
                                currParticipant.Member.Number,
                                currParticipant.Member.FirstName,
                                currParticipant.Member.LastName,
                                0, 
                                currParticipant.Game.AllGameScores().Sum().Value,
                                top3Games.Sum(),
                                top3Games.Sum() + (3 * currParticipant.Member.Handicap) + (3 * currParticipant.Game.Bonus),
                                currParticipant.Game.Game1,
                                currParticipant.Game.Game2,
                                currParticipant.Game.Game3,
                                currParticipant.Game.Game4,
                                currParticipant.Game.Handicap, 
                                currParticipant.Game.Bonus.Value,
                                currParticipant.Game.Id,
                                currParticipant.Squad); 

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
                        if (checkBoxScratchScore.Checked)
                        {
                            // orders list by highest scoring scratch score total to lowest
                            topParticipantGameViewModels = topParticipantGameViewModels.OrderByDescending(t => t.ScratchTotal).ToList();

                            // links game series listbox to list
                            lbxHighSelected.DataSource = topParticipantGameViewModels;

                            //displays specific tostring for displaying info dealing with scratch score total
                            lbxHighSelected.DisplayMember = "ScratchTotalToString";
                        }
                        // if handicap score radio button is checked
                        else if (checkBoxHandicapScore.Checked)
                        {
                            // orders list by highest scoring handicap score total to lowest
                            topParticipantGameViewModels = topParticipantGameViewModels.OrderByDescending(t => t.HandicapScore).ToList();

                            // links game series listbox to list
                            lbxHighSelected.DataSource = topParticipantGameViewModels;

                            // displays specific tostring for displaying info dealing with handicap score total
                            lbxHighSelected.DisplayMember = "HandicapTotalToString";
                        }
                    }

                    UpdateHighSelectedLabel(isGame);

                }
                catch (SqlException)
                {
                    //what is the 3rd box?
                    listOfTopScore
                        .Clear(); //filter out if there is no one on the squad yet so the 3rd box won't get populated
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
        private int? getScratchScore(int? gameScore, int? gameHandicap)
        {
            return gameScore + gameHandicap;
        }

        private void btnTournamentsByYear_Click(object sender, EventArgs e)
        {
            TournamentsByYear listTournaments = new TournamentsByYear(RegionID);
            listTournaments.ShowDialog();
        }
        //Called when stats btn is clicked
        private void btnStats_Click(object sender, EventArgs e)
        {
            TournamentStats tournamentStats = new TournamentStats();
            tournamentStats.ShowDialog();
        }

        private void btnRecapByPin_Click(object sender, EventArgs e)
        {
            FrmSelection selectTournament = new FrmSelection(RegionID);
            selectTournament.ShowDialog();
            DialogResult mboxResult = 
                MessageBox.Show($"are you sure you want to print {selectTournament.selectedTournament.TourneyNameDate}?",
                    "Confirm Tournament", MessageBoxButtons.YesNo);
            if (mboxResult == DialogResult.Yes)
            {
                Print.printByTour(selectTournament.selectedTournament);
            }
        }

        private void btnPlaceStandings_Click(object sender, EventArgs e)
        {
            TournamentPlaceStandings form = new TournamentPlaceStandings();
            form.ShowDialog();
        }

        //runs fill member when enter key is pressed on text box
        private void txtMemberNum_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                List<Participant> total = TournamentDB.GetTournamentMemberList(GetTournamentById(Convert.ToInt32(cbxTourneyDropDown.SelectedValue)));
                RecordIndexOnEnter(total);
                FillMember();
            }
        }
        
        /// <summary>
        /// Populates Tournament dropdown list to most recently modified tournament;
        /// </summary>
        public void populateSelectedTournament(Tournament currtourney)
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
        private void btnFinalizeTounament_Click(object sender, EventArgs e)
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
        private void btnSenior_Click(object sender, EventArgs e)
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
        private void btnGame_Click(object sender, EventArgs e)
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
        //Called when the report series is clicked
        private void btnSeries_Click(object sender, EventArgs e)
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
                //these 2 regions would recreate data that already exists on trhe page
                #region PRINTING HANDICAP TOURNAMENT RESULTS
                if (checkBoxHandicapScore.Checked)
                {
                    if (selectedTournament.ThreeOutOf4 && squadList.Contains(0))
                    {
                        temp = ParticipantsDB.GetStandingsForThreeOutOf4ByHandicap(selectedTournament.Id);
                    }
                    else if (selectedTournament.ThreeOutOf4 && !squadList.Contains(0))
                    {
                        temp = ParticipantsDB.GetStandingsForThreeOutOf4ByFilterSeriesByHandicap(squadList, selectedTournament.Id);
                    }
                    /*
                    if (selectedTournament.ThreeOutOf4 && qualifyBySquadNumber == 0) //overall best standings for 3of4 tournament
                    {
                        temp = ParticipantsDB.GetStandingsForThreeOutOf4ByHandicap(db, selectedTournament.Id);
                    }
                    else if (selectedTournament.ThreeOutOf4 && qualifyBySquadNumber > 0) //best standings based on sqaud for  3of4 tournament
                    {
                        temp = ParticipantsDB.GetStandingsForThreeOf4BySquadNumberByHandicap(db, qualifyBySquadNumber, selectedTournament.Id);

                    }*/

                    else if (!selectedTournament.ThreeOutOf4 && squadList.Contains(0))
                    {
                        temp = ParticipantsDB.GetStandingsForTournamentByHandicap(selectedTournament.Id);
                    }
                    else if (!selectedTournament.ThreeOutOf4 && !squadList.Contains(0))
                    {
                        temp = ParticipantsDB.GetStandingsForTournamentByFilterSeriesByHandicap(squadList, selectedTournament.Id);
                    }

                        /*if (!selectedTournament.ThreeOutOf4 && qualifyBySquadNumber == 0) //overall standings for a regular tournament
                        {
                            temp = ParticipantsDB.GetStandingsForTournamentByHandicap(db, selectedTournament.Id);
                        }
                        else if (!selectedTournament.ThreeOutOf4 && qualifyBySquadNumber > 0) //standings based on squad for a regular tournament
                        {
                            temp = ParticipantsDB.GetStandingsForTournamentBySquadByHandicap(db, qualifyBySquadNumber, selectedTournament.Id);
                        }*/
                    #endregion

                    #region PRINTING SCRATCH TOURNAMENT RESULTS
                    else if (checkBoxScratchScore.Checked)
                    {
                        if (selectedTournament.ThreeOutOf4 && squadList.Contains(0))
                        {
                            temp = ParticipantsDB.GetStandingsForThreeOf4ByScratch(selectedTournament.Id);
                        }
                        else if (selectedTournament.ThreeOutOf4 && !squadList.Contains(0))
                        {
                            temp = ParticipantsDB.GetStandingsForThreeOf4ByFilterSeriesByScratch(squadList, selectedTournament.Id);
                        }
                        /*
                        if (selectedTournament.ThreeOutOf4 && qualifyBySquadNumber == 0) //overall best standings for 3of4 tournament
                        {
                            temp = ParticipantsDB.GetStandingsForThreeOf4ByScratch(db, selectedTournament.Id);
                        }
                        else if (selectedTournament.ThreeOutOf4 && qualifyBySquadNumber > 0) //best standings based on sqaud for  3of4 tournament
                        {
                            temp = ParticipantsDB.GetStandingsThreeOfFourBySquadScratch(db, qualifyBySquadNumber, selectedTournament.Id);
                        }
                        */
                        else if (!selectedTournament.ThreeOutOf4 && squadList.Contains(0))
                        {
                            temp = ParticipantsDB.GetStandingsForTournamentByScratch(selectedTournament.Id);
                        }
                        else if (!selectedTournament.ThreeOutOf4 && !squadList.Contains(0))
                        {
                            temp = ParticipantsDB.GetStandingsForTournamentByFilterSeriesByScratch(squadList, selectedTournament.Id);
                        }
                        /*
                        if (!selectedTournament.ThreeOutOf4 && qualifyBySquadNumber == 0) //overall standings for a regular tournament
                        {
                            temp = ParticipantsDB.GetStandingsForTournamentByScratch(db, selectedTournament.Id);
                        }
                        else if (!selectedTournament.ThreeOutOf4 && qualifyBySquadNumber > 0) //standings based on squad for a regular tournament
                        {
                            temp = ParticipantsDB.GetStandingsForTournamentBySquadScratch(db, qualifyBySquadNumber, selectedTournament.Id);
                        }*/
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
        private void checkBoxAllResults_CheckedChanged(object sender, EventArgs e)
        {
            Refresh(false);
        }

        private void checkBoxSquadResults_CheckChanged(object sender, EventArgs e)
        {
            if (cbxTourneyDropDown.Size != null)
            {
                Refresh(false);
            }
        }
        #endregion  

        private void btnDelete_Click(object sender, EventArgs e)
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

                RefreshMemberScoresForm();
            }
            catch
            {
                MessageBox.Show("Current Stats Not added to Tournament yet.");
            }
            finally
            {
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
            cbxTourneyDropDown.DisplayMember = "TourneyNameDate";
            cbxTourneyDropDown.ValueMember = "Id";
        }

        private void RemoveParticipantFromTournament()
        {
            Game g = GetScoresById(currentMem.Id);
            //Delete from player history
            PlayerHistory p = PlayerHistoryDB.GetPlayerHistoryByGameID(g.Id);
            if(p != null)
            {
                PlayerHistoryDB.DeletePlayerHistory(p);
            }

            //Delete from FinalizeTemp
            FinalizeTemp ft = FinalizeTempDB.GetFinalizeID(GameDB.GetGame(g.Id));
            try
            {
                FinalizeTempDB.DeleteFinalizeTemp(ft);
            }
            catch (DbUpdateException)
            {
                //no finalized record to remove
            }
            
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

        private void btnTournamentResults_Click(object sender, EventArgs e)
        {
            FrmTournamentResults form = new FrmTournamentResults();
            form.ShowDialog();
        }

        private void checkBoxSquadNumber_CheckedChanged(object sender, EventArgs e)
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
		private void frmMemberScores_Resize(object sender, EventArgs e)
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
        private void flpMemberScores_SizeChanged(object sender, EventArgs e)
        {
            FormHelper.SetFlowControlScrollBars(this, flpMemberScores, 1300, 750);
        }

        //runs fill member when you tab out of text box
        private void txtMemberNum_Leave(object sender, EventArgs e)
        {
            FillMember();
        }

        private void cbAllSquads_CheckedChanged(object sender, EventArgs e)
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
            int check = 0;

            if (cbFilterSquad1.Checked)
            {
                check++;
            }

            if (cbFilterSquad2.Checked)
            {
                check++;
            }

            if (cbFilterSquad3.Checked)
            {
                check++;
            }

            if (cbFilterSquad4.Checked)
            {
                check++;
            }

            if (cbFilterSquad5.Checked)
            {
                check++;
            }

            if (cbFilterSquad6.Checked)
            {
                check++;
            }

            if (cbFilterSquad7.Checked)
            {
                check++;
            }

            if (cbFilterSquad8.Checked)
            {
                check++;
            }
            return check;
        }

        private void cbFilterSquad1_CheckedChanged(object sender, EventArgs e)
        {
            SquadFilter(sender as CheckBox, 1);
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

        private void cbFilterSquad2_CheckedChanged(object sender, EventArgs e)
        {
            SquadFilter(sender as CheckBox, 2);
        }

        private void cbFilterSquad3_CheckedChanged(object sender, EventArgs e)
        {
            SquadFilter(sender as CheckBox, 3);
        }

        private void cbFilterSquad4_CheckedChanged(object sender, EventArgs e)
        {
            SquadFilter(sender as CheckBox, 4);
        }

        private void cbFilterSquad5_CheckedChanged(object sender, EventArgs e)
        {
            SquadFilter(sender as CheckBox, 5);
        }

        private void cbFilterSquad6_CheckedChanged(object sender, EventArgs e)
        {
            SquadFilter(sender as CheckBox, 6);
        }

        private void cbFilterSquad7_CheckedChanged(object sender, EventArgs e)
        {
            SquadFilter(sender as CheckBox, 7);
        }

        private void cbFilterSquad8_CheckedChanged(object sender, EventArgs e)
        {
            SquadFilter(sender as CheckBox, 8);
        }

        private void lbxGameLeader_Click(object sender, EventArgs e)
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

        private void checkBoxGameSC_CheckedChanged(object sender, EventArgs e)
        {
            Refresh(false);
        }

        private void checkBoxHighSeries_CheckedChanged(object sender, EventArgs e)
        {
            Refresh(false);
        }


    }
}
