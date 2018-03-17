using NineTapTour.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Drawing.Printing;
using System.Configuration;
using System.Data.Entity.Core.Objects;

namespace NineTapTour.Forms
{
    public partial class frmMemberScores : Form
    {

        //IOrderedEnumerable<Member> _membersList;
        public int RegionID;
        Member currentMem;
        Member currentMem2;
        TextBox[] scratchArray = new TextBox[4];
        TextBox[] handicappArray = new TextBox[4];
        int currentIndex = 0;         //Count for record counting
        bool buttonCheck; // boolean value used to determine which record index button was clicked
        Participant player = new Participant();
        Participant player2 = new Participant();
        //bool doubles = true;
        public static Tournament selectedTournament;
        public static List<TopScores> overallListOfTopScores;
        public static List<Participant> overallListOfParticipants;
        int QBSNumber = 0;
        frmNewTournament currentTourneyPage;

       

       


        public frmMemberScores()
        {
            InitializeComponent();
            DoubleInitialize(false);

        }

        private void RadioIntialize()
        {
            rdoSquadOne.TabStop = false;
            rdoHandicapScore.TabStop = false;
            rdoAllResults.TabStop = false;
            rdoSquad5.Visible = false;
            rdoSquad6.Visible = false;
            rdoSquad7.Visible = false;
            rdoSquad8.Visible = false;
            rdoSquad5Results.Visible = false;
            rdoSquad6Results.Visible = false;
            rdoSquad7Results.Visible = false;
            rdoSquad8Resualts.Visible = false;
            if (cbxTourneyDropDown.SelectedIndex >= 0)
            {
                if (selectedTournament.Squads == 5)
                {
                    rdoSquad5.Visible = true;
                    rdoSquad5Results.Visible = true;
                }
                if (selectedTournament.Squads == 6)
                {
                    rdoSquad5.Visible = true;
                    rdoSquad6.Visible = true;
                    rdoSquad5Results.Visible = true;
                    rdoSquad6Results.Visible = true;
                }
                if (selectedTournament.Squads == 7)
                {
                    rdoSquad5.Visible = true;
                    rdoSquad6.Visible = true;
                    rdoSquad7.Visible = true;
                    rdoSquad5Results.Visible = true;
                    rdoSquad6Results.Visible = true;
                    rdoSquad7Results.Visible = true;

                }
                if (selectedTournament.Squads == 8)
                {
                    rdoSquad5.Visible = true;
                    rdoSquad6.Visible = true;
                    rdoSquad7.Visible = true;
                    rdoSquad8.Visible = true;
                    rdoSquad5Results.Visible = true;
                    rdoSquad6Results.Visible = true;
                    rdoSquad7Results.Visible = true;
                    rdoSquad8Resualts.Visible = true;
                }
            }
        }

        private void FrmMemberScores_Load(object sender, EventArgs e)
        {
            RegionID = ((FrmMain)MdiParent).RegionID;

            scratchArray = new TextBox[4] { txtScratchScore1, txtScratchScore2, txtScratchScore3, txtScratchScore4 };
            handicappArray = new TextBox[4] { txtHandicapScore1, txtHandicapScore2, txtHandicapScore3, txtHandicapScore4 };
           
            if (cbxTourneyDropDown.SelectedIndex == -1)
            {
                btnLeftArrow.Enabled = false;
                btnRightArrow.Enabled = false;
                btnDelete.Enabled = false;
            }
            else
            {
                btnLeftArrow.Enabled = true;
                btnRightArrow.Enabled = true;
            }

        
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
            //addedd in this line inorder to prevent the reset of the drop down list on memberscores form when switching between forms
            int tempcbx = cbxTourneyDropDown.SelectedIndex;
            rdoHandicapScore.Visible = false;
            rdoScratchScore.Visible = false;
            cbxTourneyDropDown.Visible = false;
            ResetFields();

            MemberStatus("", Color.Black, SystemColors.Control, true);
            cbxTourneyDropDown.DataSource = ((FrmMain)MdiParent)._tournamentList;
            cbxTourneyDropDown.DisplayMember = "TourneyNameDate";
            cbxTourneyDropDown.ValueMember = "Id";


            List<Tournament> temp2 = TournamentDb.GetTournamentList(RegionID);

            if (temp2.Count() > 0)
            {
                var item = temp2.Max(x => x.Id);
                cbxTourneyDropDown.SelectedValue = item;
            }
            clear();
            cbxTourneyDropDown.Visible = true;
            if (cbxTourneyDropDown.SelectedIndex >= 0 && cbxTourneyDropDown.Visible && cbxTourneyDropDown.SelectedIndex.ToString() != null)
            {
                // resets the current index to zero when changing the tournament
                currentIndex = 0;
                // Gets the record for the selected tournament
                RecordIndex(TournamentDb.GetTournamentMemberList(GetTournamentById(selectedTournament.Id)));
                overallListOfParticipants = TournamentDb.GetTournamentMemberList(selectedTournament);
                btnDelete.Enabled = true;

                refresh(false,QBSNumber);
                // sets focus to member num becuse that is what a user will need next
                rdoHandicapScore.Visible = true;
                rdoScratchScore.Visible = true;
                txtMemberNum.Focus();
            }

        }

        /// <summary>
        /// Resets the fields to show reset and beginning of records
        /// </summary>
        private void ResetFields()
        {
            txtMemberNum.Clear();
            txtMemberNum2.Clear();
            txtLastName.Clear();
            txtLastName2.Clear();
            txtFirstName.Clear();
            txtFirstName2.Clear();
            txtMiddleInitial.Clear();
            txtMiddleInitial2.Clear();
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

        //Hides/Shows the 2nd player information for doubles tourneys
        private void DoubleInitialize(bool set)
        {
            txtFirstName2.Visible = set;
            txtLastName2.Visible = set;
            txtMiddleInitial2.Visible = set;
            lbLastName2.Visible = set;
            lblFirstName2.Visible = set;
            lblMiddleInitial2.Visible = set;
        }
        #region GetMember


        //Get players scores
        private void GetScores(Game currentGame)
        {
            if (currentGame != null)
            {
                // The game bonus & handicap shouldn't be changed to current member bonus/handicap
                currentGame.Bonus = currentMem.Bonus;
                currentGame.Handicap = currentMem.Handicap;

                //////////////////////////////////////////////////////////////// PAGINATION HAPPENS RIGHT HERE!!!! ////////////////////////////////////////////////////
                List<Participant> total = TournamentDb.GetTournamentMemberListInOrder(GetTournamentById(Convert.ToInt32(cbxTourneyDropDown.SelectedValue))); //gets list in order so forloops itterate better

                //if (buttonCheck == true) // if the right button was clicked
                //{
                //    for (int i = currentIndex; i < total.Count(); i++)
                //    {
                //        if (currentMem.Id == total[i].Member.Id)
                //        {
                //            currentIndex++;
                //        }
                //    }
                //}
                //else // if left button was clicked
                //{
                //    for (int i = 0; i < currentIndex; i++)
                //    {
                //        if (currentMem.Id == total[i].Member.Id)
                //        {
                //            currentIndex--;
                //        }
                //    }
                //}
                lblRecord.Text = "Record " + (currentIndex)  + " / " + total.Count();

                // if IsComp true then check CompEntry checkbox
                chbCompEntry.Checked = false;
                if (currentGame.IsComp)
                {
                    chbCompEntry.Checked = true;
                }



                txtScratchScore1.Text = Convert.ToString(currentGame.Game1);
                    txtScratchScore2.Text = Convert.ToString(currentGame.Game2);
                txtScratchScore3.Text = Convert.ToString(currentGame.Game3);
                txtScratchScore4.Text = Convert.ToString(currentGame.Game4);
                txtScratchScore1.Focus();
                txtMoney.Text = currentGame.MoneyWon.ToString();
                // put game.handicap in Handicap field for that tournament game
                //txtHandicap.Text = currentGame.Handicap.ToString();



            }
        }
        #endregion
        private void FillMember()
        {
            Tournament currTourney = null;
            if (cbxTourneyDropDown.SelectedValue != null)
            {
                currTourney = GetTournamentById(Convert.ToInt32(cbxTourneyDropDown.SelectedValue));

                string searchNumber = txtMemberNum.Text;
                string searchNumber2 = txtMemberNum2.Text;
                if (!currTourney.Doubles)
                {
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
                        currentMem = MemberDb.GetMember(memberNumber,RegionID);
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


                            #region Incorrect code
                            // This code was setting the member.handicap and txtHandicap to the last ph.handicap
                            // which was the original member.handicap, however, this handicap then was saved as the 
                            // game.handicap, finalizeTemp.handicap and ph.handicap, so it never changed
                            // It never reflected the correct current member.handicap

                            //check to make sure the right numbers are being brought over from the members information page
                            List<PlayerHistory> last5 = PlayerHistoryDB.getLastFiveFromPlayerhistory(currentMem.Number, RegionID);
                            if (last5.Count > 0)
                            {
                                if (last5[0].HandiCap != currentMem.Handicap || last5[0].Bonus != currentMem.Bonus)
                                {
                                    currentMem.Bonus = last5[0].Bonus;
                                    currentMem.Handicap = last5[0].HandiCap;
                                    txtHandicap.Text = last5[0].HandiCap.ToString();
                                    txtBonusPins.Text = last5[0].Bonus.ToString();
                                }
                                else
                                {

                                    txtHandicap.Text = currentMem.Handicap.ToString();
                                    txtBonusPins.Text = currentMem.Bonus.ToString();
                                }


                            }
                            else
                            {
                                currentMem.Bonus = 0;
                                txtHandicap.Text = currentMem.Bonus.ToString();
                                txtBonusPins.Text = currentMem.Bonus.ToString();
                            }

                            #endregion


                            txtHandicap.Text = currentMem.Handicap.ToString();
                                txtBonusPins.Text = currentMem.Bonus.ToString();

              


                            Game currentGame = GetScoresById(currentMem.Id);

                            GetScores(currentGame);

                        }
                        else
                        {
                            MessageBox.Show(string.Format("A member with the number {0} does not exist", txtMemberNum.Text), "Your Attention Please.");
                            txtMemberNum.Clear();
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < searchNumber2.Length; i++)
                    {
                        if (!char.IsNumber(searchNumber2[i]))
                        {
                            MessageBox.Show("Please input numbers only.", "Your Attention Please.");
                            txtMemberNum2.Clear();
                            return;
                        }
                    }
                    for (int i = 0; i < searchNumber.Length; i++)
                    {
                        if (!char.IsNumber(searchNumber[i]))
                        {
                            MessageBox.Show("Please input numbers only.", "Your Attention Please.");
                            txtMemberNum.Clear();
                            return;
                        }
                    }
                    if (searchNumber2.Trim() != "" && searchNumber.Trim() != "")
                    {
                        int memberNumber2 = Convert.ToInt16(txtMemberNum2.Text);
                        int memberNumber = Convert.ToInt16(txtMemberNum.Text);
                        currentMem = ((FrmMain)MdiParent)._membersList.FirstOrDefault(m => m.Number == memberNumber);
                        currentMem2 = ((FrmMain)MdiParent)._membersList.FirstOrDefault(m => m.Number == memberNumber2);
                        if (currentMem2 != null && currentMem != null)
                        {
                            if (currentMem.IsActive && currentMem2.IsActive)
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
                            txtLastName2.Text = currentMem2.LastName;
                            txtFirstName2.Text = currentMem2.FirstName;
                            txtMiddleInitial2.Text = currentMem2.MiddleInitial;
                            Game currentGame = GetScoresById(currentMem.Id);
                            Game currentGame2 = GetScoresById(currentMem2.Id);
                            if (currentGame != null || currentGame2 != null)
                            {
                                List<Member> total = TournamentDb.GetUniqueTourMembers(GetTournamentById(Convert.ToInt32(cbxTourneyDropDown.SelectedValue)));
                                foreach (Member mem in total)
                                {
                                    if (currentMem.Id == mem.Id)
                                    {
                                        txtScratchScore1.Text = Convert.ToString(currentGame.Game1);
                                        txtScratchScore2.Text = Convert.ToString(currentGame.Game2);
                                    }
                                    if (currentMem2.Id == mem.Id)
                                    {
                                        txtScratchScore3.Text = Convert.ToString(currentGame2.Game1);
                                        txtScratchScore4.Text = Convert.ToString(currentGame2.Game2);
                                    }
                                }
                            }

                        }
                    }
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
            //auto tab to the next textbox when textbox1's length is 3.
            if (tx.Text.Length == 3)
            {
                SendKeys.Send("{TAB}");
            }

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
        #region New Recap
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
                    player.ParticipantRegionID = RegionID;
                    player2.ParticipantRegionID = RegionID;

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
                    player.Game.Game3 = 0;
                    player.Game.Game4 = 0;
                    player.Game.Bonus = currentMem.Bonus;
                    player.Game.Handicap = currentMem.Handicap;
                    player2.Game.Id = gameId2;

                    player2.Tournament = currTourney;
                    player2.Game.Game1 = IsEmpty(txtScratchScore1) ? null : (int?)Convert.ToInt32((scratchArray[2].Text));
                    player2.Game.Game2 = IsEmpty(txtScratchScore2) ? null : (int?)Convert.ToInt32((scratchArray[3].Text));
                    player2.Game.Game3 = 0;
                    player2.Game.Game4 = 0;
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
                        ResetFields();
                        txtMemberNum.Focus();
                        RecordIndex(TournamentDb.GetTournamentMemberList(currTourney));
                        clear();

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
                    player.ParticipantRegionID = RegionID;
                    var db = new NineTapDb();
                    var gameId = (from p in db.Participants
                                  where p.Member.Id == currentMem.Id
                                  && p.Tournament.Id == currTourney.Id
                                  select p.Game.Id).FirstOrDefault();

                    player.Game.Id = gameId;
                    //selects the ID of the combobox of tournaments and stores the
                    //tournament property within the participants class.
                    player.Tournament = currTourney;

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
                    //defaults money earned to 0, or enters text box amount
                    if (txtMoney.Text == "" || txtMoney.Text == null)
                        player.Game.MoneyWon = 0;
                    else
                        player.Game.MoneyWon = Convert.ToDecimal(txtMoney.Text);

                    if (string.IsNullOrEmpty(txtScratchScore1.Text.Trim()) || string.IsNullOrEmpty(txtScratchScore2.Text.Trim())
                        || string.IsNullOrEmpty(txtScratchScore3.Text.Trim()) || string.IsNullOrEmpty(txtScratchScore4.Text.Trim()))
                    {
                        MessageBox.Show("Please enter all scratch scores", "Blank Scores Not Allowed");
                        return;
                    }
                    else if (!isNumeric(txtScratchScore1.Text.Trim()) || !isNumeric(txtScratchScore2.Text.Trim())
                        || !isNumeric(txtScratchScore3.Text.Trim()) || !isNumeric(txtScratchScore4.Text.Trim()))
                    {
                        MessageBox.Show("Please enter only numbers", "Non-Integer Scores Not Allowed");
                        return;
                    }
                    else
                    {
                        player.Game.Game1 = IsEmpty(txtScratchScore1) ? null : (int?)Convert.ToInt32((scratchArray[0].Text));
                        player.Game.Game2 = IsEmpty(txtScratchScore2) ? null : (int?)Convert.ToInt32((scratchArray[1].Text));
                        player.Game.Game3 = IsEmpty(txtScratchScore3) ? null : (int?)Convert.ToInt32((scratchArray[2].Text));
                        player.Game.Game4 = IsEmpty(txtScratchScore4) ? null : (int?)Convert.ToInt32((scratchArray[3].Text));
                        player.Game.Bonus = currentMem.Bonus;
                        player.Game.Handicap = currentMem.Handicap;
                        player.Game.gameRegionID = RegionID;
                        player.Member = currentMem;

                        // if compEntry checkbox is checked, set IsComp to true in game table
                        if(chbCompEntry.Checked)
                        {
                            player.Game.IsComp = true;
                            db.SaveChanges();
                        }

                        try
                        {
                            TournamentDb.AddMemberToTournament(player);
#if DEBUG
                            MessageBox.Show(@"Bowler Added Successfully to Tournament!");
#endif
                            ResetFields();
                            txtMemberNum.Focus();
                            RecordIndex(TournamentDb.GetTournamentMemberList(currTourney));
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
                            db.Entry(currentMem).State = EntityState.Modified;
                            db.SaveChanges();

                        }

                    }

                }
                refresh(false,QBSNumber);
            }
            else
            {
                MessageBox.Show("Please Fill out the Participants information!");
            }
        }

        private string GetConnection()
        {
            return ConfigurationManager.ConnectionStrings["NineTapDbConnection"].ConnectionString;
        }
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
            int temp;
            if (cbxTourneyDropDown.SelectedIndex <= 0)
            {
                temp = 0;
            }
            else if (players.Count() <= 0)
            {
                temp = 0;
            }
            else
            {
                try
                {
                    temp = players.IndexOf(players[currentIndex - 1]);
                }
                catch
                {
                    temp = players.IndexOf(players[currentIndex]);
                }
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
        private  Tournament GetTournamentById(int selectedTournamentId)
        {
            try
            {
                Tournament selectedTournament = (from t in TournamentDb.GetTournamentList(RegionID)
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
            else if (rdoSquadFour.Checked)
            {
                squad = 4;
            }
            else if (rdoSquad5.Checked)
            {
                squad = 5;
            }
            else if (rdoSquad6.Checked)
            {
                squad = 6;
            }
            else if (rdoSquad7.Checked)
            {
                squad = 7;
            }
            else if (rdoSquad8.Checked)
            {
                squad = 8;
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
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRightArrow_Click(object sender, EventArgs e)
        {

            List<Participant> total = TournamentDb.GetTournamentMemberList(GetTournamentById(Convert.ToInt32(cbxTourneyDropDown.SelectedValue)));
            if (currentIndex >= total.Count())
            {
                MessageBox.Show("There are no more players to go to!");
            }
            else
            {
                buttonCheck = true; // right button clicked
                currentIndex++;
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
                else if (total[currentIndex - 1].Squad == 4)
                {
                    rdoSquadFour.Checked = true;
                }
                else if (total[currentIndex - 1].Squad == 5)
                {
                    rdoSquad5.Checked = true;
                }
                else if (total[currentIndex - 1].Squad == 6)
                {
                    rdoSquad6.Checked = true;
                }
                else if (total[currentIndex - 1].Squad == 7)
                {
                    rdoSquad7.Checked = true;
                }
                else if (total[currentIndex - 1].Squad == 8)
                {
                    rdoSquad8.Checked = true;
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
                //if (currentIndex <= 1)
                if (currentIndex <= 0) // should be 0, must be able to read the member at currentIndex = 0;
                {
                    MessageBox.Show("You can't go back!");
                }
                else
                {
                    buttonCheck = false;  //Left button clicked
                    currentIndex--;
                    txtMemberNum.Text = Convert.ToString(total[currentIndex].Member.Number);
                    if (total[currentIndex].Squad == 1)
                    {
                        rdoSquadOne.Checked = true;
                    }
                    else if (total[currentIndex].Squad == 2)
                    {
                        rdoSquadTwo.Checked = true;
                    }
                    else if (total[currentIndex].Squad == 3)
                    {
                        rdoSquadThree.Checked = true;
                    }
                    else if (total[currentIndex].Squad == 4)
                    {
                        rdoSquadFour.Checked = true;
                    }
                    else if (total[currentIndex].Squad == 5)
                    {
                        rdoSquad5.Checked = true;
                    }
                    else if (total[currentIndex].Squad == 6)
                    {
                        rdoSquad6.Checked = true;
                    }
                    else if (total[currentIndex].Squad == 7)
                    {
                        rdoSquad7.Checked = true;
                    }
                    else if (total[currentIndex].Squad == 8)
                    {
                        rdoSquad8.Checked = true;
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

            // resets the fields when a different tournament is selected
            ResetFields();
            // assigns the selectedTournament variable as the selected Tournament from the comboBox
            selectedTournament = (Tournament)cbxTourneyDropDown.SelectedItem;

            // determines whether the tournament is a double tourney or not, then enables or disables the single and/or double textBox selection option
            if (selectedTournament == null)
            {
                rdoScratchScore.Visible = false;
                txtMemberNum.Enabled = false;
                txtMemberNum2.Visible = false;
                btnRecapByPin.Enabled = false;
                DoubleInitialize(false);

                RadioIntialize();
                rdoHandicapScore.Visible = false;
                rdoScratchScore.Visible = false;
            }
            else if (selectedTournament.Doubles)
            {
                txtMemberNum.Enabled = true;
                txtMemberNum2.Visible = true;
                txtMemberNum2.Enabled = true;
                DoubleInitialize(true);
                EnableButtonsWhenValidTournamentSelected();
                rdoHandicapScore.Visible = true;
                rdoScratchScore.Visible = true;
                RadioIntialize();
            }
            else
            {
                rdoScratchScore.Visible = true;
                txtMemberNum.Enabled = true;
                txtMemberNum2.Visible = false;
                DoubleInitialize(false);
                EnableButtonsWhenValidTournamentSelected();
                RadioIntialize();
                rdoHandicapScore.Visible = true;
                rdoScratchScore.Visible = true;
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
                RecordIndex(TournamentDb.GetTournamentMemberList(GetTournamentById(selectedTournament.Id)));
                refresh(false,QBSNumber);
                rdoHandicapScore.Visible = true;
                rdoScratchScore.Visible = true;
                // sets focus to member num becuse that is what a user will need next
                txtMemberNum.Focus();
            }


        }

        /// <summary>
        /// Enables buttons to select when valid Tournament is selected
        /// </summary>
        private void EnableButtonsWhenValidTournamentSelected()
        {
            btnStats.Enabled = true;
            btnLeftArrow.Enabled = true;
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



        /// <summary>
        /// Search for tours by location.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTourSearch_Click(object sender, EventArgs e)
        {
            List<Tournament> tours = new List<Tournament>();
            FrmTourSearch tourSearch = new FrmTourSearch(tours,RegionID);
            tourSearch.ShowDialog();
#if DEBUG
            foreach (Tournament tour in tours)
            {
                Console.WriteLine(tour.TourneyNameDate);
            }
#endif
            if (tours.Count() > 0)
            {
                cbxTourneyDropDown.DataSource = tours;
                cbxTourneyDropDown.DisplayMember = "TourneyNameDate";
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
            ScoreAndTotalClear();
            FillMember();

        }
        /// <summary>
        /// Checks if current member has an existing entry into Squad 2
        /// and clears the scores if the member does NOT
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rdoSquadTwo_CheckedChanged(object sender, EventArgs e)
        {
            ScoreAndTotalClear();
            FillMember();
        }

        private void rdoSquadThree_CheckedChanged(object sender, EventArgs e)
        {
            ScoreAndTotalClear();
            FillMember();
        }
        /// <summary>
        /// Checks if current member has an existing entry into Squad 4
        /// and clears the scores if the member does NOT
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rdoSquadFour_CheckedChanged(object sender, EventArgs e)
        {
            ScoreAndTotalClear();
            FillMember();
        }


        private void rdoSquad5_CheckedChanged_1(object sender, EventArgs e)
        {
            ScoreAndTotalClear();
            FillMember();
        }


        private void rdoSquad6_CheckedChanged_1(object sender, EventArgs e)
        {
            ScoreAndTotalClear();
            FillMember();
        }

        private void rdoSquad7_CheckedChanged(object sender, EventArgs e)
        {
            ScoreAndTotalClear();
            FillMember();
        }

        private void rdoSquad8_CheckedChanged(object sender, EventArgs e)
        {
            ScoreAndTotalClear();
            FillMember();
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

        /* TODO Error
        private void btnTournamentsByYear_Click(object sender, EventArgs e)
        {            
            FrmListTournamentsByYear listTournaments = new FrmListTournamentsByYear();
            listTournaments.ShowDialog();
        }
        */
        private void rdoScratchScore_CheckedChanged(object sender, EventArgs e)
        {
            refresh(true, QBSNumber);
        }

        private void rdoHandicapScore_CheckedChanged(object sender, EventArgs e)
        {
            refresh(true, QBSNumber);
        }

        /// <summary>
        /// pass true if you are changing the radio buttons and only want to refresh the bottom box.
        /// </summary>
        /// <param name="seriesChange"></param>

        List<TopScores> listOfTopScore = new List<TopScores>();
        IComparer<MemberScores> scoreComparer = new MemberScoresComparer();
        public void refresh(bool seriesChange, int qbsNumber)
        {
            // DEV NOTE: The text generated for the boxes in this is strange and has tabs that the 
            // code doesn't seem to be writing as far as I can tell.
            // I think a bug fixer should look at this some time and try to see why it's happening
            try
            {
                List<TopScores> listOfTopScore = new List<TopScores>();
                // Function scope data
                int nullValues;
                NineTapDb db = new NineTapDb();
                int selectedTourney = selectedTournament.Id;
                List<MemberScores> scores;


                /// Seperate top scores so that only top score from each participant shows up for each tournament,
                /// no matter how many squads they rolled in for the tournament.
                SqlConnection con = new SqlConnection(TournamentStats.GetConnection());
                SqlCommand getList = new SqlCommand();
                getList.Connection = con;
                if (QBSNumber == 0)
                {
                    getList.CommandText = @"SELECT Participants.Member_Id, Members.FirstName, Members.LastName, Game1, Game2, Game3, Game4, Games.Id,  Members.Handicap, Members.Bonus, SUM(Game1 + Game2 + Game3 + Game4) AS Total
                                    FROM Tournaments JOIN Participants ON Tournaments.Id = Participants.Tournament_Id
                                    JOIN Games ON Games.Id = Participants.Game_Id
                                    JOIN Members ON Members.Id = Participants.Member_Id 
                                    WHERE Tournaments.Id = @TID
                                    GROUP BY Game1, Game2, Game3, Game4, Participants.Member_Id, Tournaments.Location, Participants.SquadNumber, Members.FirstName, Members.LastName, Members.Handicap, Members.Bonus,  Games.Id
                                    ORDER BY Participants.Member_Id";
                    getList.Parameters.AddWithValue("@TID", selectedTourney);
                }
                else
                {
                    getList.CommandText = @"SELECT Participants.Member_Id, Members.FirstName, Members.LastName, Game1, Game2, Game3, Game4, Games.Id, Members.Handicap, Members.Bonus, SUM(Game1 + Game2 + Game3 + Game4) AS Total
                                    FROM Tournaments JOIN Participants ON Tournaments.Id = Participants.Tournament_Id
                                    JOIN Games ON Games.Id = Participants.Game_Id
                                    JOIN Members ON Members.Id = Participants.Member_Id 
                                    WHERE Tournaments.Id = @TID AND Participants.SquadNumber = @SN
                                    GROUP BY Game1, Game2, Game3, Game4, Participants.Member_Id, Tournaments.Location, Participants.SquadNumber, Members.FirstName, Members.LastName, Members.Handicap, Members.Bonus,  Games.Id
                                    ORDER BY Participants.Member_Id";
                    getList.Parameters.AddWithValue("@TID", selectedTourney);
                    getList.Parameters.AddWithValue("@SN", QBSNumber);

                }

                try
                {
                    // open connection
                    con.Open();

                    // execute command(query)
                    SqlDataReader reader = getList.ExecuteReader();

                    int id = 0;
                    int count = 0;
                    int num = 0;

                    // view results
                    foreach (var i in reader)
                    {
                        num = listOfTopScore.Count();
                        int score = Convert.ToInt32(reader["Total"]);
                        List<int?> top4Games = new List<int?> { Convert.ToInt32(reader["Game1"]), Convert.ToInt32(reader["Game2"]), Convert.ToInt32(reader["Game3"]), Convert.ToInt32(reader["Game4"]) };
                        List<int> top3Games = TournamentStats.GetTop3OutOf4(top4Games);
                        if (Convert.ToInt32(reader["Member_ID"]) == id)
                        {
                            if (score > listOfTopScore[count - 1].ScratchTotal)
                            {
                                listOfTopScore[count - 1].ScratchTotal = score;
                                listOfTopScore[count - 1].HandicapScore = score + (listOfTopScore[count - 1].Handicap * 4) + (listOfTopScore[count - 1].Bonus * 4);
                                listOfTopScore[count - 1].Top3ScratchScore = top3Games[0] + top3Games[1] + top3Games[2];
                                listOfTopScore[count - 1].Top3HandiScores = top3Games[0] + top3Games[1] + top3Games[2] + (3 * Convert.ToInt32(reader["Handicap"])) + (3 * listOfTopScore[count-1].Bonus);
                                listOfTopScore[count - 1].Game1 = Convert.ToInt32(reader["Game1"]);
                                listOfTopScore[count - 1].Game2 = Convert.ToInt32(reader["Game2"]);
                                listOfTopScore[count - 1].Game3 = Convert.ToInt32(reader["Game3"]);
                                listOfTopScore[count - 1].Game4 = Convert.ToInt32(reader["Game4"]);
                                listOfTopScore[count - 1].GameID = Convert.ToInt32(reader["Id"]);
                            }
                        }
                        else
                        {
                            if (count == num)
                            {
                                TopScores temp = new TopScores();
                                listOfTopScore.Add(temp);
                            }


                            id = Convert.ToInt32(reader["Member_ID"]);
                            /// Populates info                         
                            listOfTopScore[count].FirstName = reader["FirstName"].ToString();
                            listOfTopScore[count].LastName = reader["LastName"].ToString();
                            listOfTopScore[count].Game1 = Convert.ToInt32(reader["Game1"]);
                            listOfTopScore[count].Game2 = Convert.ToInt32(reader["Game2"]);
                            listOfTopScore[count].Game3 = Convert.ToInt32(reader["Game3"]);
                            listOfTopScore[count].Game4 = Convert.ToInt32(reader["Game4"]);
                            listOfTopScore[count].GameID = Convert.ToInt32(reader["Id"]);
                            listOfTopScore[count].Handicap = Convert.ToInt32(reader["Handicap"]);
                            listOfTopScore[count].memberID = id;
                            try
                            {
                                listOfTopScore[count].Bonus = Convert.ToInt32(reader["Bonus"]);
                            }
                            catch
                            {
                                listOfTopScore[count].Bonus = 0;
                            }
                            listOfTopScore[count].memberID = id;
                            listOfTopScore[count].ScratchTotal = Convert.ToInt32(reader["Total"]);
                            listOfTopScore[count].HandicapScore = score + (listOfTopScore[count].Handicap * 4) + (listOfTopScore[count].Bonus * 4);
                            listOfTopScore[count].Top3ScratchScore = top3Games[0] + top3Games[1] + top3Games[2];
                            listOfTopScore[count].Top3HandiScores = top3Games[0] + top3Games[1] + top3Games[2] + (3 * Convert.ToInt32(reader["Handicap"])) + (3 * listOfTopScore[count].Bonus);
                            count++;
                        }
                    }
                }
                catch (SqlException)
                {

                }

                overallListOfTopScores = listOfTopScore;
                /// Top 5 LINQ query
                var top5 = db.Participants.Include(b => b.Member)
                .Include(b => b.Game)
                .Where(b => b.Tournament.Id == selectedTourney);

                #region Populates 1st Box
                // This function combines the former refresh events into a single function, and since they all used the same variable names I just put
                // their old data in a scope block so they could be reused
                if (!seriesChange)
                {

                    richTextBox1.Clear();
                    richTextBox1.Font = new Font(FontFamily.GenericMonospace, richTextBox1.Font.Size);
                    richTextBox1.Text = ("#" + "\t" + "Name" + "\t\t\t" + "HighScore" + "\n");
                    scores = new List<MemberScores>();

                    if (QBSNumber == 0)
                    {

                        var temp = (from g in top5
                                    orderby g.Game.Game1
                                    select new { g.Game.Game1, g.Game.Handicap, g.Member.FirstName, g.Member.LastName });
                        var temp2 = (from g in top5
                                     orderby g.Game.Game2
                                     select new { g.Game.Game2, g.Game.Handicap, g.Member.FirstName, g.Member.LastName });
                        var temp3 = (from g in top5
                                     orderby g.Game.Game3
                                     select new { g.Game.Game3, g.Game.Handicap, g.Member.FirstName, g.Member.LastName });
                        var temp4 = (from g in top5
                                     orderby g.Game.Game4
                                     select new { g.Game.Game4, g.Game.Handicap, g.Member.FirstName, g.Member.LastName });
                        foreach (var s in temp)
                        {
                            scores.Add(new MemberScores { FirstName = s.FirstName, LastName = s.LastName, Score = getScratchScore(s.Game1, s.Handicap) });
                        }
                        foreach (var s in temp2)
                        {
                            scores.Add(new MemberScores { FirstName = s.FirstName, LastName = s.LastName, Score = getScratchScore(s.Game2, s.Handicap) });
                        }
                        foreach (var s in temp3)
                        {
                            scores.Add(new MemberScores { FirstName = s.FirstName, LastName = s.LastName, Score = getScratchScore(s.Game3, s.Handicap) });
                        }
                        foreach (var s in temp4)
                        {
                            scores.Add(new MemberScores { FirstName = s.FirstName, LastName = s.LastName, Score = getScratchScore(s.Game4, s.Handicap) });
                        }
                        scores.Sort(scoreComparer);
                        scores.Reverse();
                        scores = scores.ToList();
                        for (int i = 0; i < scores.Count(); i++)
                        {
                            int FirstNameLength = 0;
                            int LastNameLength = 0;
                            if (scores[i].FirstName.Length < 6)
                            {
                                FirstNameLength = scores[i].FirstName.Length;

                            }
                            else
                            {
                                FirstNameLength = 6;
                            }
                            if (scores[i].LastName.Length < 6)
                            {
                                LastNameLength = scores[i].LastName.Length;
                            }
                            else
                            {
                                LastNameLength = 6;
                            }
                            //richTextBox1.AppendText((i + 1).ToString() + "\t" + String.Format("{0, -20}", scores[i].FirstName + " " + scores[i].LastName)
                            //                        + "\t" + String.Format("{0, -5}", scores[i].Score + " " + "\n"));
                            richTextBox1.AppendText($"{i + 1}\t{scores[i].FirstName.Substring(0, FirstNameLength)}\t{scores[i].LastName.Substring(0, LastNameLength)}\t\t\t{scores[i].Score}\n");
                        }
                    }
                    else
                    {
                        var temp = (from g in top5
                                    orderby g.Game.Game1
                                    where g.Squad == QBSNumber
                                    select new { g.Game.Game1, g.Game.Handicap, g.Member.FirstName, g.Member.LastName });
                        var temp2 = (from g in top5
                                     orderby g.Game.Game2
                                     where g.Squad == QBSNumber
                                     select new { g.Game.Game2, g.Game.Handicap, g.Member.FirstName, g.Member.LastName });
                        var temp3 = (from g in top5
                                     orderby g.Game.Game3
                                     where g.Squad == QBSNumber
                                     select new { g.Game.Game3, g.Game.Handicap, g.Member.FirstName, g.Member.LastName });
                        var temp4 = (from g in top5
                                     orderby g.Game.Game4
                                     where g.Squad == QBSNumber
                                     select new { g.Game.Game4, g.Game.Handicap, g.Member.FirstName, g.Member.LastName });
                        foreach (var s in temp)
                        {
                            scores.Add(new MemberScores { FirstName = s.FirstName, LastName = s.LastName, Score = getScratchScore(s.Game1, s.Handicap) });
                        }
                        foreach (var s in temp2)
                        {
                            scores.Add(new MemberScores { FirstName = s.FirstName, LastName = s.LastName, Score = getScratchScore(s.Game2, s.Handicap) });
                        }
                        foreach (var s in temp3)
                        {
                            scores.Add(new MemberScores { FirstName = s.FirstName, LastName = s.LastName, Score = getScratchScore(s.Game3, s.Handicap) });
                        }
                        foreach (var s in temp4)
                        {
                            scores.Add(new MemberScores { FirstName = s.FirstName, LastName = s.LastName, Score = getScratchScore(s.Game4, s.Handicap) });
                        }
                        scores.Sort(scoreComparer);
                        scores.Reverse();
                        scores = scores.ToList();
                        for (int i = 0; i < scores.Count(); i++)
                        {
                            int FirstNameLength = 0;
                            int LastNameLength = 0;
                            if (scores[i].FirstName.Length < 6)
                            {
                                FirstNameLength = scores[i].FirstName.Length;

                            }
                            else
                            {
                                FirstNameLength = 6;
                            }
                            if (scores[i].LastName.Length < 6)
                            {
                                LastNameLength = scores[i].LastName.Length;
                            }
                            else
                            {
                                LastNameLength = 6;
                            }
                            //richTextBox1.AppendText((i + 1).ToString() + "\t" + String.Format("{0, -20}", scores[i].FirstName + " " + scores[i].LastName)
                            //                        + "\t" + String.Format("{0, -5}", scores[i].Score + " " + "\n"));
                            richTextBox1.AppendText($"{i + 1}\t{scores[i].FirstName.Substring(0, FirstNameLength)}\t{scores[i].LastName.Substring(0, LastNameLength)}\t\t\t{scores[i].Score}\n");

                        }
                    }
                }
                #endregion

                #region Populates 2nd Box
                // Do the 2nd box
                if (!seriesChange)
                {

                    richTextBox2.Clear();
                    richTextBox2.Font = new Font(FontFamily.GenericMonospace, richTextBox2.Font.Size);
                    richTextBox2.Text = ("#" + "\t" + "Name" + "\t\t\t" + "HighScore" + "\n");
                    scores = new List<MemberScores>();

                    if (QBSNumber == 0)
                    {

                        var temp = (from g in top5
                                    orderby g.Game.Game1
                                    select new { g.Game.Game1, g.Member.FirstName, g.Member.LastName });
                        var temp2 = (from g in top5
                                     orderby g.Game.Game2
                                     select new { g.Game.Game2, g.Member.FirstName, g.Member.LastName });
                        var temp3 = (from g in top5
                                     orderby g.Game.Game3
                                     select new { g.Game.Game3, g.Member.FirstName, g.Member.LastName });
                        var temp4 = (from g in top5
                                     orderby g.Game.Game4
                                     select new { g.Game.Game4, g.Member.FirstName, g.Member.LastName });
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
                        scores = scores.ToList();
                        for (int i = 0; i < scores.Count(); i++)
                        {
                            int FirstNameLength = 0;
                            int LastNameLength = 0;
                            if (scores[i].FirstName.Length < 6)
                            {
                                FirstNameLength = scores[i].FirstName.Length;

                            }
                            else
                            {
                                FirstNameLength = 6;
                            }
                            if (scores[i].LastName.Length < 6)
                            {
                                LastNameLength = scores[i].LastName.Length;
                            }
                            else
                            {
                                LastNameLength = 6;
                            }
                            //richTextBox2.AppendText((i + 1).ToString() + "\t" + String.Format("{0, -20}", scores[i].FirstName + " " + scores[i].LastName)
                            //                        + "\t" + String.Format("{0, -5}", scores[i].Score + " " + "\n"));
                            richTextBox2.AppendText($"{i + 1}\t{scores[i].FirstName.Substring(0, FirstNameLength)}\t{scores[i].LastName.Substring(0, LastNameLength)}\t\t\t{scores[i].Score}\n");
                        }
                    }
                    else
                    {
                        var temp = (from g in top5
                                    orderby g.Game.Game1
                                    where g.Squad == QBSNumber
                                    select new { g.Game.Game1, g.Member.FirstName, g.Member.LastName });
                        var temp2 = (from g in top5
                                     orderby g.Game.Game2
                                     where g.Squad == QBSNumber
                                     select new { g.Game.Game2, g.Member.FirstName, g.Member.LastName });
                        var temp3 = (from g in top5
                                     orderby g.Game.Game3
                                     where g.Squad == QBSNumber
                                     select new { g.Game.Game3, g.Member.FirstName, g.Member.LastName });
                        var temp4 = (from g in top5
                                     orderby g.Game.Game4
                                     where g.Squad == QBSNumber
                                     select new { g.Game.Game4, g.Member.FirstName, g.Member.LastName });
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
                        scores = scores.ToList();
                        for (int i = 0; i < scores.Count(); i++)
                        {
                            int FirstNameLength = 0;
                            int LastNameLength = 0;
                            if (scores[i].FirstName.Length < 6)
                            {
                                FirstNameLength = scores[i].FirstName.Length;

                            }
                            else
                            {
                                FirstNameLength = 6;
                            }
                            if (scores[i].LastName.Length < 6)
                            {
                                LastNameLength = scores[i].LastName.Length;
                            }
                            else
                            {
                                LastNameLength = 6;
                            }
                            //richTextBox2.AppendText((i + 1).ToString() + "\t" + String.Format("{0, -20}", scores[i].FirstName + " " + scores[i].LastName)
                            //                        + "\t" + String.Format("{0, -5}", scores[i].Score + " " + "\n"));
                            richTextBox2.AppendText($"{i + 1}\t{scores[i].FirstName.Substring(0, FirstNameLength)}\t{scores[i].LastName.Substring(0, LastNameLength)}\t\t\t{scores[i].Score}\n");

                        }
                    }
                }
                #endregion

                #region If not Three out of 4
                // Do the third box

                /////////////////////////////////
                if (!selectedTournament.ThreeOutOf4)
                {
                    /////////////////////////////////
                    richTextBox3.Clear();
                    richTextBox3.Font = new Font(FontFamily.GenericMonospace, richTextBox3.Font.Size);
                    richTextBox3.Text = ("#" + "\t" + "Name" + "\t\t" + "High Series" + "\n");
                    int TotalToCompare = 0;
                    int currentPlace = 1;
                    scores = new List<MemberScores>();

                    //populate total score
                    if (rdoScratchScore.Checked)
                    {
                        foreach (var s in listOfTopScore)
                        {
                            scores.Add(new MemberScores { FirstName = s.FirstName, LastName = s.LastName, Score = s.Game1 + s.Game2 + s.Game3 + s.Game4});
                        }
                        //IComparer<MemberScores> scoreComparer = new MemberScoresComparer();
                        scores.Sort(scoreComparer);
                        scores.Reverse();
                        scores = scores.ToList();
                        for (int i = 0; i < scores.Count(); i++)
                        {
                            int FirstNameLength = 0;
                            int LastNameLength = 0;
                            if (scores[i].FirstName.Length < 6)
                            {
                                FirstNameLength = scores[i].FirstName.Length;

                            }
                            else
                            {
                                FirstNameLength = 6;
                            }
                            if (scores[i].LastName.Length < 6)
                            {
                                LastNameLength = scores[i].LastName.Length;
                            }
                            else
                            {
                                LastNameLength = 6;
                            }
                            if(i >= 1) // checks to see if the top score is equal to the last score looked at, to figure out of they had tied or not 
                            {
                                if(Convert.ToInt32(scores[i].Score) == TotalToCompare)
                                {
                                    scores[i].placing = currentPlace;
                                    TotalToCompare = Convert.ToInt32(scores[i].Score);
                                    listOfTopScore[i].Placing = currentPlace;
                                    
                                }
                                else
                                {
                                    currentPlace = i+1;
                                    scores[i].placing = currentPlace; //only increment placing up if the score is less than the current total
                                    listOfTopScore[i].Placing = currentPlace;
                                    TotalToCompare = Convert.ToInt32(scores[i].Score);
                                }
                            }

                            else //set first place 
                            {
                                scores[i].placing = currentPlace;
                                TotalToCompare = Convert.ToInt32(scores[i].Score);
                            }
                            
                            //richTextBox3.AppendText((i + 1).ToString() + "\t" + String.Format("{0,0}", scores[i].FirstName.Substring(0, FirstNameLength) + " " + scores[i].LastName.Substring(0, LastNameLength)
                            //                        + "\t" + String.Format("{0, -5}", scores[i].Score + "\n")));

                            richTextBox3.AppendText($"{scores[i].placing}\t{scores[i].FirstName.Substring(0, FirstNameLength)}\t{scores[i].LastName.Substring(0, LastNameLength)}\t\t\t{scores[i].Score}\n");
                        }
                    }
                    else if (rdoHandicapScore.Checked)
                    {
                        foreach (var i in listOfTopScore)
                        {
                            #region conditions for highest handicap scores
                            nullValues = 0;
                            if (i.Game1 == 0)
                            {
                                nullValues += 1;
                            }
                            if (i.Game2 == 0)
                            {
                                nullValues += 1;
                            }
                            if (i.Game3 == 0)
                            {
                                nullValues += 1;
                            }
                            if (i.Game4 == 0)
                            {
                                nullValues += 1;
                            }
                            #endregion
                            scores.Add(new MemberScores {MemberNo = i.memberID, FirstName = i.FirstName, LastName = i.LastName, Score = ((i.Game1 + i.Bonus + i.Handicap) + (i.Game2 + i.Bonus + i.Handicap) + (i.Game3 + i.Bonus + i.Handicap) + (i.Game4 + i.Handicap + i.Bonus)) });
                        }
                        scores.Sort(scoreComparer);
                        scores.Reverse();
                        scores = scores.ToList();
                        for (int i = 0; i < scores.Count(); i++)
                        {
                            int FirstNameLength = 0;
                            int LastNameLength = 0;
                            if (scores[i].FirstName.Length < 6)
                            {
                                FirstNameLength = scores[i].FirstName.Length;

                            }
                            else
                            {
                                FirstNameLength = 6;
                            }
                            if (scores[i].LastName.Length < 6)
                            {
                                LastNameLength = scores[i].LastName.Length;
                            }
                            else
                            {
                                LastNameLength = 6;
                            }
                            //richTextBox3.AppendText((i + 1).ToString() + "\t" + String.Format("{0,0}", scores[i].FirstName.Substring(0, FirstNameLength) + " " + scores[i].LastName.Substring(0, LastNameLength)
                            //                        + "\t" + String.Format("{0, -5}", scores[i].Score + "\n")));

                            if (i >= 1) // checks to see if the top score is equal to the last score looked at, to figure out of they had tied or not 
                            {
                                if (Convert.ToInt32(scores[i].Score) == TotalToCompare)
                                {
                                    scores[i].placing = currentPlace;
                                    TotalToCompare = Convert.ToInt32(scores[i].Score);                               
                                }
                                else
                                {
                                    currentPlace = i + 1;
                                    scores[i].placing = currentPlace;
                                    TotalToCompare = Convert.ToInt32(scores[i].Score);
                                }
                            }

                            else //set first place 
                            {
                                scores[i].placing = currentPlace;
                                listOfTopScore[i].Placing = currentPlace;
                                TotalToCompare = Convert.ToInt32(scores[i].Score);
                            }

                            richTextBox3.AppendText($"{scores[i].placing}\t{scores[i].FirstName.Substring(0, FirstNameLength)}\t{scores[i].LastName.Substring(0, LastNameLength)}\t\t\t{scores[i].Score}\n");
                        }
                    }
                }
                #endregion
                #region Three Out Of 4
                /////////////////////////////////////////////////////
                /// Executes if tournament selected is 3 Out of 4 ///
                /////////////////////////////////////////////////////
                if (selectedTournament.ThreeOutOf4)
                {
                    /////////////////////////////////
                    richTextBox3.Clear();
                    richTextBox3.Font = new Font(FontFamily.GenericMonospace, richTextBox3.Font.Size);
                    richTextBox3.Text = ("#" + "\t" + "Name" + "\t\t\t" + "High Series" + "\n");
                    scores = new List<MemberScores>();
                    int TotalToCompare = 0;
                    int currentPlace = 1;

                    // List to get top 3 scores   
                    List<int> listOfScores = new List<int>();

                    if (rdoScratchScore.Checked)
                    {
                        foreach (var s in listOfTopScore)
                        {
                            int one = Convert.ToInt32(s.Game1);
                            int two = Convert.ToInt32(s.Game2);
                            int three = Convert.ToInt32(s.Game3);
                            int four = Convert.ToInt32(s.Game4);
                            listOfScores.Add(one);
                            listOfScores.Add(two);
                            listOfScores.Add(three);
                            listOfScores.Add(four);
                            listOfScores.Sort();
                            listOfScores.Reverse();

                            ///*************************
                            scores.Add(new MemberScores { FirstName = s.FirstName, LastName = s.LastName, Score = listOfScores[0] + listOfScores[1] + listOfScores[2] });
                            listOfScores.Clear();
                        }
                        //IComparer<MemberScores> scoreComparer = new MemberScoresComparer();
                        scores.Sort(scoreComparer);
                        scores.Reverse();
                        scores = scores.ToList();
                        for (int i = 0; i < scores.Count(); i++)
                        {
                            int FirstNameLength = 0;
                            int LastNameLength = 0;
                            if (scores[i].FirstName.Length < 6)
                            {
                                FirstNameLength = scores[i].FirstName.Length;

                            }
                            else
                            {
                                FirstNameLength = 6;
                            }
                            if (scores[i].LastName.Length < 6)
                            {
                                LastNameLength = scores[i].LastName.Length;
                            }
                            else
                            {
                                LastNameLength = 6;
                            }

                            if (i >= 1) // checks to see if the top score is equal to the last score looked at, to figure out of they had tied or not 
                            {
                                if (Convert.ToInt32(scores[i].Score) == TotalToCompare)
                                {
                                    scores[i].placing = currentPlace;
                                    TotalToCompare = Convert.ToInt32(scores[i].Score);
                                }
                                else
                                {
                                    currentPlace = i + 1;
                                    scores[i].placing = currentPlace;
                                    TotalToCompare = Convert.ToInt32(scores[i].Score);
                                }
                            }

                            else //set first place 
                            {
                                scores[i].placing = currentPlace;
                                TotalToCompare = Convert.ToInt32(scores[i].Score);
                            }
                            //richTextBox3.AppendText((i + 1).ToString() + "\t" + String.Format("{0,0}", scores[i].FirstName.Substring(0, FirstNameLength) + " " + scores[i].LastName.Substring(0, LastNameLength)
                            //                        + "\t" + String.Format("{0, -5}", scores[i].Score + "\n")));

                            richTextBox3.AppendText($"{scores[i].placing}\t{scores[i].FirstName.Substring(0,FirstNameLength)}\t{scores[i].LastName.Substring(0,LastNameLength)}\t\t\t{scores[i].Score}\n"); 
                        }
                    }
                    else if (rdoHandicapScore.Checked)
                    {
                        foreach (var i in listOfTopScore)
                        {
                            #region conditions for highest handicap scores
                            nullValues = 0;
                            if (i.Game1 == 0)
                            {
                                nullValues += 1;
                            }
                            if (i.Game2 == 0)
                            {
                                nullValues += 1;
                            }
                            if (i.Game3 == 0)
                            {
                                nullValues += 1;
                            }
                            if (i.Game4 == 0)
                            {
                                nullValues += 1;
                            }
                            #endregion
                            ///***********************
                            int one = Convert.ToInt32(i.Game1 + i.Handicap + i.Bonus);
                            int two = Convert.ToInt32(i.Game2 + i.Handicap + i.Bonus);
                            int three = Convert.ToInt32(i.Game3 + i.Handicap + i.Bonus);
                            int four = Convert.ToInt32(i.Game4 + i.Handicap + i.Bonus);
                            listOfScores.Add(one);
                            listOfScores.Add(two);
                            listOfScores.Add(three);
                            listOfScores.Add(four);
                            listOfScores.Sort();
                            listOfScores.Reverse();

                            ///*************************
                            scores.Add(new MemberScores { FirstName = i.FirstName, LastName = i.LastName, Score = listOfScores[0] + listOfScores[1] + listOfScores[2] });
                            listOfScores.Clear();
                        }
                        scores.Sort(scoreComparer);
                        scores.Reverse();
                        scores = scores.ToList();
                        for (int i = 0; i < scores.Count(); i++)
                        {
                            int FirstNameLength = 0;
                            int LastNameLength = 0;
                            if (scores[i].FirstName.Length < 6)
                            {
                                FirstNameLength = scores[i].FirstName.Length;

                            }
                            else
                            {
                                FirstNameLength = 6;
                            }
                            if (scores[i].LastName.Length < 6)
                            {
                                LastNameLength = scores[i].LastName.Length;
                            }
                            else
                            {
                                LastNameLength = 6;
                            }
                            //richTextBox3.AppendText((i + 1).ToString() + "\t" + String.Format("{0, -20}", scores[i].FirstName.Substring(0, FirstNameLength) + " " + scores[i].LastName.Substring(0, LastNameLength)
                            //                        + "\t" + String.Format("{0, -5}", scores[i].Score + "\n")));

                            if (i >= 1) // checks to see if the top score is equal to the last score looked at, to figure out of they had tied or not 
                            {
                                if (Convert.ToInt32(scores[i].Score) == TotalToCompare)
                                {
                                    scores[i].placing = currentPlace;
                                    TotalToCompare = Convert.ToInt32(scores[i].Score);
                                }
                                else
                                {
                                    currentPlace = i + 1;
                                    scores[i].placing = currentPlace;
                                    TotalToCompare = Convert.ToInt32(scores[i].Score);
                                }
                            }

                            else //set first place 
                            {
                                scores[i].placing = currentPlace;
                                TotalToCompare = Convert.ToInt32(scores[i].Score);
                            }
                            richTextBox3.AppendText($"{scores[i].placing}\t{scores[i].FirstName.Substring(0, FirstNameLength)}\t{scores[i].LastName.Substring(0, LastNameLength)}\t\t\t{scores[i].Score}\n");
                        }
                    }
                }
                #endregion            
            }
            finally
            {

            }//TODO ADDED FOR ERRORS REMOVE WHEN FIXED
        }

        private int? getScratchScore(int? gameScore, int? gameHandicap)
        {
            return gameScore + gameHandicap;
        }

        public class MemberScores
        {
            public string FirstName { get; set; }

            public string LastName { get; set; }

            public int placing { get; set; }

            public int? Score { get; set; }

            public int MemberNo { get; set; }

            public string LastPaymentYear { get; set; }

            public bool Paid { get; set; }
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

        private void btnTournamentsByYear_Click(object sender, EventArgs e)
        {
            TournamentsByYear listTournaments = new TournamentsByYear(RegionID);
            listTournaments.ShowDialog();
        }

        private void btnStats_Click(object sender, EventArgs e)
        {
            TournamentStats tournamentStats = new TournamentStats();
            tournamentStats.ShowDialog();
        }

        private void btnRecapByPin_Click(object sender, EventArgs e)
        {
            NineTapTour.Database.Print.printByTour((Tournament)cbxTourneyDropDown.SelectedItem);
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
                FillMember();
        }
        //runs fill member when enter key is pressed on text box
        private void txtMemberNum2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
                FillMember();
        }
        //runs fill member when you tab out of text box
        private void txtMemberNum_Leave(object sender, EventArgs e)
        {
            FillMember();
        }
        //runs fill member when you tab out of text box
        private void txtMemberNum2_Leave(object sender, EventArgs e)
        {
            FillMember();
        }

        /// <summary>
        /// Populates Tournament dropdown list to most recently modified tournament;
        /// </summary>
        public void populateSelectedTournament(Tournament currtourney)
        {
            List<Tournament> temp2 = TournamentDb.GetTournamentList(RegionID);

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
                
                var newFrmFinalizeTournament = new FrmFinalizeTournament(selectedTournament, overallListOfTopScores, RegionID);
                newFrmFinalizeTournament.Dock = DockStyle.Fill;
                newFrmFinalizeTournament.WindowState = FormWindowState.Maximized;
                newFrmFinalizeTournament.Show();
            }
            
           
        }

       

        /*******************************************************************************
        When the report section buttons are clicked, it will take them to the FrmMemberScoresReports to ask for how many they want to take for printing
        ********************************************************************************/
        private void btnSenior_Click(object sender, EventArgs e)
        {
            if (cbxTourneyDropDown.SelectedIndex < 0)
            {
                MessageBox.Show("Please Select a Tournament");
            }
            else
            {
                //handicap is being sent of a members game
                //using (NineTapDb db = new NineTapDb())
                //{
                //    var temp = (from g in (db.Participants.Include(b => b.Member)
                //                            .Include(b => b.Game)
                //                            .Where(b => b.Tournament.Id == selectedTournament.Id))
                //                orderby (g.Game.Handicap) descending
                //                select new MemberScores { MemberNo = g.Member.Id, FirstName = g.Member.FirstName, LastName = g.Member.LastName, Score = g.Game.Handicap }).ToList();
                //    temp.Sort(scoreComparer);
                //    temp.Reverse();

                using (NineTapDb db = new NineTapDb())
                {
                    var temp = (from g in (db.Participants.Include(b => b.Member)
                                           .Include(b => b.Game)
                                           .Where(b => b.Tournament.Id == selectedTournament.Id)
                                           .Where(b => b.Member.IsSenior))

                                select new MemberScores { MemberNo = g.Member.Id, FirstName = g.Member.FirstName, LastName = g.Member.LastName, Score = g.Game.Game1.Value, LastPaymentYear = (g.Member.IsLifetimeMember) ? "life ": g.Member.LastPayment.Value.Year.ToString(), Paid = (g.Member.IsLifetimeMember == true || !( g.Member.LastPayment != null && (g.Member.LastPayment.Value <= EntityFunctions.AddYears(DateTime.Now, -1)/* DateTime.Now.AddYears(-1)*/))) }).Concat(
                       (from g in (db.Participants.Include(b => b.Member)
                                           .Include(b => b.Game)
                                           .Where(b => b.Tournament.Id == selectedTournament.Id)
                                           .Where(b => b.Member.IsSenior))
                        select new MemberScores { MemberNo = g.Member.Id, FirstName = g.Member.FirstName, LastName = g.Member.LastName, Score = g.Game.Game2.Value, LastPaymentYear = (g.Member.IsLifetimeMember) ? "life " : g.Member.LastPayment.Value.Year.ToString(), Paid = (g.Member.IsLifetimeMember == true || !(g.Member.LastPayment != null && (g.Member.LastPayment.Value <= EntityFunctions.AddYears(DateTime.Now, -1)))) })).Concat(
                       (from g in (db.Participants.Include(b => b.Member)
                                           .Include(b => b.Game)
                                           .Where(b => b.Tournament.Id == selectedTournament.Id)
                                           .Where(b => b.Member.IsSenior))
                        select new MemberScores { MemberNo = g.Member.Id, FirstName = g.Member.FirstName, LastName = g.Member.LastName, Score = g.Game.Game3.Value, LastPaymentYear = (g.Member.IsLifetimeMember) ? "life " : g.Member.LastPayment.Value.Year.ToString(), Paid = (g.Member.IsLifetimeMember == true || !(g.Member.LastPayment != null && (g.Member.LastPayment.Value <= EntityFunctions.AddYears(DateTime.Now, -1)))) })).Concat(
                       (from g in (db.Participants.Include(b => b.Member)
                                           .Include(b => b.Game)
                                           .Where(b => b.Tournament.Id == selectedTournament.Id)
                                           .Where(b => b.Member.IsSenior))
                        select new MemberScores { MemberNo = g.Member.Id, FirstName = g.Member.FirstName, LastName = g.Member.LastName, Score = g.Game.Game4.Value, LastPaymentYear = (g.Member.IsLifetimeMember) ? "life " : g.Member.LastPayment.Value.Year.ToString(), Paid = (g.Member.IsLifetimeMember == true || !(g.Member.LastPayment != null && (g.Member.LastPayment.Value <= EntityFunctions.AddYears(DateTime.Now, -1)))) })).ToList();
                    temp.Sort(scoreComparer);
                    temp.Reverse();

                    if (temp.Count() != 0)
                    {
                        FrmMemberScoresReports report = new FrmMemberScoresReports(temp, selectedTournament, 0/*reportTypeNum, 0 for High game handicap/senior, 1 for game/high game, 2 for series/high series*/);
                        //report.Dock = DockStyle.Fill;
                        report.Show();
                    }
                    else
                    {
                        MessageBox.Show("There are no particpants in this tournament.");
                    }
                }
            }
        }

        private void btnGame_Click(object sender, EventArgs e)
        {
            if (cbxTourneyDropDown.SelectedIndex < 0)
            {
                MessageBox.Show("Please Select a Tournament");
            }
            else
            {
                using (NineTapDb db = new NineTapDb())
                {
                    var temp = (from g in (db.Participants.Include(b => b.Member)
                                            .Include(b => b.Game)
                                            .Where(b => b.Tournament.Id == selectedTournament.Id))

                                select new MemberScores { MemberNo = g.Member.Id, FirstName = g.Member.FirstName, LastName = g.Member.LastName, Score = g.Game.Game1.Value, LastPaymentYear = (g.Member.IsLifetimeMember) ? "life " : g.Member.LastPayment.Value.Year.ToString(), Paid = (g.Member.IsLifetimeMember == true || !(g.Member.LastPayment != null && (g.Member.LastPayment.Value <= EntityFunctions.AddYears(DateTime.Now, -1)))) }).Concat(
                        (from g in (db.Participants.Include(b => b.Member)
                                            .Include(b => b.Game)
                                            .Where(b => b.Tournament.Id == selectedTournament.Id))
                         select new MemberScores { MemberNo = g.Member.Id, FirstName = g.Member.FirstName, LastName = g.Member.LastName, Score = g.Game.Game2.Value, LastPaymentYear = (g.Member.IsLifetimeMember) ? "life " : g.Member.LastPayment.Value.Year.ToString(), Paid = (g.Member.IsLifetimeMember == true || !(g.Member.LastPayment != null && (g.Member.LastPayment.Value <= EntityFunctions.AddYears(DateTime.Now, -1)))) })).Concat(
                        (from g in (db.Participants.Include(b => b.Member)
                                            .Include(b => b.Game)
                                            .Where(b => b.Tournament.Id == selectedTournament.Id))
                         select new MemberScores { MemberNo = g.Member.Id, FirstName = g.Member.FirstName, LastName = g.Member.LastName, Score = g.Game.Game3.Value, LastPaymentYear = (g.Member.IsLifetimeMember) ? "life " : g.Member.LastPayment.Value.Year.ToString(), Paid = (g.Member.IsLifetimeMember == true || !(g.Member.LastPayment != null && (g.Member.LastPayment.Value <= EntityFunctions.AddYears(DateTime.Now, -1)))) })).Concat(
                        (from g in (db.Participants.Include(b => b.Member)
                                            .Include(b => b.Game)
                                            .Where(b => b.Tournament.Id == selectedTournament.Id))
                         select new MemberScores { MemberNo = g.Member.Id, FirstName = g.Member.FirstName, LastName = g.Member.LastName, Score = g.Game.Game4.Value, LastPaymentYear = (g.Member.IsLifetimeMember) ? "life " : g.Member.LastPayment.Value.Year.ToString(), Paid = (g.Member.IsLifetimeMember == true || !(g.Member.LastPayment != null && (g.Member.LastPayment.Value <= EntityFunctions.AddYears(DateTime.Now, -1)))) })).ToList();
                    temp.Sort(scoreComparer);
                    temp.Reverse();

                    if (temp.Count() != 0)
                    {
                        FrmMemberScoresReports report = new FrmMemberScoresReports(temp, selectedTournament, 1/*reportTypeNum, 0 for High game handicap/senior, 1 for game/high game, 2 for series/high series*/);
                        report.Show();
                    }
                    else
                    {
                        MessageBox.Show("There are no particpants in this tournament.");
                    }
                }
            }
        }

        private void btnSeries_Click(object sender, EventArgs e)
        {
            if (cbxTourneyDropDown.SelectedIndex < 0)
            {
                MessageBox.Show("Please Select a Tournament");
            }
            else
            {
                using (NineTapDb db = new NineTapDb())
                {
                    var temp = new List<MemberScores>();
                    #region PRINTING HANDICAP TOURNAMENT RESULTS
                    if (rdoHandicapScore.Checked)
                    {
                        if (selectedTournament.ThreeOutOf4 && QBSNumber == 0) //overall best standings for 3of4 tournament
                        {
                            temp = (from g in (db.Participants.Include(b => b.Member)
                                                    .Include(b => b.Game)
                                                    .Where(b => b.Tournament.Id == selectedTournament.Id))
                                    orderby (g.Game.Game1 + g.Game.Game2 + g.Game.Game3 + g.Game.Game4 + (g.Game.Handicap * 3 + g.Game.Bonus * 3) - (new List<int> { g.Game.Game1.Value, g.Game.Game2.Value, g.Game.Game3.Value, g.Game.Game4.Value }.Min())) descending
                                    select new MemberScores { MemberNo = g.Member.Id, FirstName = g.Member.FirstName, LastName = g.Member.LastName, Score = g.Game.Game1 + g.Game.Game2 + g.Game.Game3 + g.Game.Game4 + (g.Game.Handicap * 3) - (new List<int> { g.Game.Game1.Value, g.Game.Game2.Value, g.Game.Game3.Value, g.Game.Game4.Value }.Min()), LastPaymentYear = (g.Member.IsLifetimeMember) ? "life " : g.Member.LastPayment.Value.Year.ToString(), Paid = (g.Member.IsLifetimeMember == true || !(g.Member.LastPayment != null && (g.Member.LastPayment.Value <= EntityFunctions.AddYears(DateTime.Now, -1)))) }).ToList();
                        }
                        else if (selectedTournament.ThreeOutOf4 && QBSNumber > 0) //best standings based on sqaud for  3of4 tournament
                        {
                            temp = (from g in (db.Participants.Include(b => b.Member)
                                          .Include(b => b.Game)
                                          .Where(b => b.Tournament.Id == selectedTournament.Id).Where(b => b.Squad == QBSNumber))
                                    orderby (g.Game.Game1 + g.Game.Game2 + g.Game.Game3 + g.Game.Game4 + (g.Game.Handicap * 3 + g.Game.Bonus * 3) - (new List<int> { g.Game.Game1.Value, g.Game.Game2.Value, g.Game.Game3.Value, g.Game.Game4.Value }.Min())) descending
                                    select new MemberScores { MemberNo = g.Member.Id, FirstName = g.Member.FirstName, LastName = g.Member.LastName, Score = g.Game.Game1 + g.Game.Game2 + g.Game.Game3 + g.Game.Game4 + (g.Game.Handicap * 3) - (new List<int> { g.Game.Game1.Value, g.Game.Game2.Value, g.Game.Game3.Value, g.Game.Game4.Value }.Min()), LastPaymentYear = (g.Member.IsLifetimeMember) ? "life " : g.Member.LastPayment.Value.Year.ToString(), Paid = (g.Member.IsLifetimeMember == true || !(g.Member.LastPayment != null && (g.Member.LastPayment.Value <= EntityFunctions.AddYears(DateTime.Now, -1)))) }).ToList();

                        }
                        else if (!selectedTournament.ThreeOutOf4 && QBSNumber == 0) //overall standings for a regular tournament
                        {
                            temp = (from g in (db.Participants.Include(b => b.Member)
                                                    .Include(b => b.Game)
                                                    .Where(b => b.Tournament.Id == selectedTournament.Id))
                                    orderby ((g.Game.Game1 + g.Game.Bonus + g.Game.Handicap) + (g.Game.Game2 + g.Game.Bonus + g.Game.Handicap) + (g.Game.Game3 + g.Game.Bonus + g.Game.Handicap) + (g.Game.Game4 + g.Game.Bonus + g.Game.Handicap)) descending
                                    select new MemberScores { MemberNo = g.Member.Id, FirstName = g.Member.FirstName, LastName = g.Member.LastName, Score = (g.Game.Game1 + g.Game.Bonus + g.Game.Handicap) + (g.Game.Game2 + g.Game.Bonus + g.Game.Handicap) + (g.Game.Game3 + g.Game.Bonus + g.Game.Handicap) + (g.Game.Game4 + g.Game.Bonus + g.Game.Handicap), LastPaymentYear = (g.Member.IsLifetimeMember) ? "life " : g.Member.LastPayment.Value.Year.ToString(), Paid = (g.Member.IsLifetimeMember == true || !(g.Member.LastPayment != null && (g.Member.LastPayment.Value <= EntityFunctions.AddYears(DateTime.Now, -1)))) }).ToList();
                        }
                        else if (!selectedTournament.ThreeOutOf4 && QBSNumber > 0) //standings based on squad for a regular tournament
                        {
                            temp = (from g in (db.Participants.Include(b => b.Member)
                                                    .Include(b => b.Game)
                                                    .Where(b => b.Tournament.Id == selectedTournament.Id).Where(b => b.Squad == QBSNumber))
                                    orderby (g.Game.Game1 + g.Game.Game2 + g.Game.Game3 + g.Game.Game4 + (g.Game.Handicap * 4 + g.Game.Bonus * 4)) descending
                                    select new MemberScores { MemberNo = g.Member.Id, FirstName = g.Member.FirstName, LastName = g.Member.LastName, Score = g.Game.Game1 + g.Game.Game2 + g.Game.Game3 + g.Game.Game4 + (g.Game.Handicap * 4), LastPaymentYear = (g.Member.IsLifetimeMember) ? "life " : g.Member.LastPayment.Value.Year.ToString(), Paid = (g.Member.IsLifetimeMember == true || !(g.Member.LastPayment != null && (g.Member.LastPayment.Value <= EntityFunctions.AddYears(DateTime.Now, -1)))) }).ToList();
                        }
                    }
                    #endregion
                    #region PRINTING SCRATCH TOURNAMENT RESULTS
                    else if (rdoScratchScore.Checked)
                    {
                        if (selectedTournament.ThreeOutOf4 && QBSNumber == 0) //overall best standings for 3of4 tournament
                        {
                            temp = (from g in (db.Participants.Include(b => b.Member)
                                                    .Include(b => b.Game)
                                                    .Where(b => b.Tournament.Id == selectedTournament.Id))
                                    orderby (g.Game.Game1 + g.Game.Game2 + g.Game.Game3 + g.Game.Game4 - (new List<int> { g.Game.Game1.Value, g.Game.Game2.Value, g.Game.Game3.Value, g.Game.Game4.Value }.Min())) descending
                                    select new MemberScores { MemberNo = g.Member.Id, FirstName = g.Member.FirstName, LastName = g.Member.LastName, Score = g.Game.Game1 + g.Game.Game2 + g.Game.Game3 + g.Game.Game4  - (new List<int> { g.Game.Game1.Value, g.Game.Game2.Value, g.Game.Game3.Value, g.Game.Game4.Value }.Min()), LastPaymentYear = (g.Member.IsLifetimeMember) ? "life " : g.Member.LastPayment.Value.Year.ToString(), Paid = (g.Member.IsLifetimeMember == true || !(g.Member.LastPayment != null && (g.Member.LastPayment.Value <= EntityFunctions.AddYears(DateTime.Now, -1)))) }).ToList();
                        }
                        else if (selectedTournament.ThreeOutOf4 && QBSNumber > 0) //best standings based on sqaud for  3of4 tournament
                        {
                            temp = (from g in (db.Participants.Include(b => b.Member)
                                          .Include(b => b.Game)
                                          .Where(b => b.Tournament.Id == selectedTournament.Id).Where(b => b.Squad == QBSNumber))
                                    orderby (g.Game.Game1 + g.Game.Game2 + g.Game.Game3 + g.Game.Game4 - (new List<int> { g.Game.Game1.Value, g.Game.Game2.Value, g.Game.Game3.Value, g.Game.Game4.Value }.Min())) descending
                                    select new MemberScores { MemberNo = g.Member.Id, FirstName = g.Member.FirstName, LastName = g.Member.LastName, Score = g.Game.Game1 + g.Game.Game2 + g.Game.Game3 + g.Game.Game4 - (new List<int> { g.Game.Game1.Value, g.Game.Game2.Value, g.Game.Game3.Value, g.Game.Game4.Value }.Min()), LastPaymentYear = (g.Member.IsLifetimeMember) ? "life " : g.Member.LastPayment.Value.Year.ToString(), Paid = (g.Member.IsLifetimeMember == true || !(g.Member.LastPayment != null && (g.Member.LastPayment.Value <= EntityFunctions.AddYears(DateTime.Now, -1)))) }).ToList();

                        }
                        else if (!selectedTournament.ThreeOutOf4 && QBSNumber == 0) //overall standings for a regular tournament
                        {
                            temp = (from g in (db.Participants.Include(b => b.Member)
                                                    .Include(b => b.Game)
                                                    .Where(b => b.Tournament.Id == selectedTournament.Id))
                                    orderby ((g.Game.Game1) + (g.Game.Game2) + (g.Game.Game3) + (g.Game.Game4)) descending
                                    select new MemberScores { MemberNo = g.Member.Id, FirstName = g.Member.FirstName, LastName = g.Member.LastName, Score = (g.Game.Game1) + (g.Game.Game2) + (g.Game.Game3) + (g.Game.Game4), LastPaymentYear = (g.Member.IsLifetimeMember) ? "life " : g.Member.LastPayment.Value.Year.ToString(), Paid = (g.Member.IsLifetimeMember == true || !(g.Member.LastPayment != null && (g.Member.LastPayment.Value <= EntityFunctions.AddYears(DateTime.Now, -1)))) }).ToList();
                        }
                        else if (!selectedTournament.ThreeOutOf4 && QBSNumber > 0) //standings based on squad for a regular tournament
                        {
                            temp = (from g in (db.Participants.Include(b => b.Member)
                                                    .Include(b => b.Game)
                                                    .Where(b => b.Tournament.Id == selectedTournament.Id).Where(b => b.Squad == QBSNumber))
                                    orderby (g.Game.Game1 + g.Game.Game2 + g.Game.Game3 + g.Game.Game4) descending
                                    select new MemberScores { MemberNo = g.Member.Id, FirstName = g.Member.FirstName, LastName = g.Member.LastName, Score = g.Game.Game1 + g.Game.Game2 + g.Game.Game3 + g.Game.Game4 , LastPaymentYear = (g.Member.IsLifetimeMember) ? "life " : g.Member.LastPayment.Value.Year.ToString(), Paid = (g.Member.IsLifetimeMember == true || !(g.Member.LastPayment != null && (g.Member.LastPayment.Value <= EntityFunctions.AddYears(DateTime.Now, -1)))) }).ToList();
                        }
                    }
                    #endregion



                    temp.Sort(scoreComparer);
                    temp.Reverse();

                    if (temp.Count() != 0)
                    {
                        FrmMemberScoresReports report = new FrmMemberScoresReports(temp, selectedTournament, 2/*reportTypeNum, 0 for High game handicap/senior, 1 for game/high game, 2 for series/high series*/);
                        report.Show();
                    }
                    else
                    {
                        MessageBox.Show("Error: No Participants in selected Squad.");
                    }
                }
            }
        }


        //these change the value of the QBSnumber, allowing the director to filter the rich text boxes by sqaud, then calls the refresh method to update the rich textboxes information to 
        //display the tournament information but based on squad'
        #region changing the sqaud number
        private void rdoAllResults_CheckedChanged(object sender, EventArgs e)
        {
            QBSNumber = 0;
            refresh(false, QBSNumber);

        }

        private void rdoSquad1Results_CheckedChanged(object sender, EventArgs e)
        {
            if (cbxTourneyDropDown.Size != null)
            {
                QBSNumber = 1;
                refresh(false, QBSNumber);
            }
        }

        private void rdoSquad2Results_CheckedChanged(object sender, EventArgs e)
        {
            if (cbxTourneyDropDown.Size != null)
            {
                QBSNumber = 2;
                refresh(false, QBSNumber);
            }
        }

        private void rdoSquad3Results_CheckedChanged(object sender, EventArgs e)
        {
            if (cbxTourneyDropDown.Size != null)
            {
                QBSNumber = 3;
                refresh(false, QBSNumber);
            }
        }

        private void rdoSquad4Results_CheckedChanged(object sender, EventArgs e)
        {
            if (cbxTourneyDropDown.Size != null)
            {
                QBSNumber = 4;
                refresh(false, QBSNumber);
            }

        }

        private void rdoSquad5Results_CheckedChanged(object sender, EventArgs e)
        {
            if (cbxTourneyDropDown.Size != null)
            {
                QBSNumber = 5;
                refresh(false, QBSNumber);
            }

        }

        private void rdoSquad6Results_CheckedChanged(object sender, EventArgs e)
        {

            if (cbxTourneyDropDown.Size != null)
            {
                QBSNumber = 6;
                refresh(false, QBSNumber);
            }

        }

        private void rdoSquad7Results_CheckedChanged(object sender, EventArgs e)
        {
            if (cbxTourneyDropDown.Size != null)
            {
                QBSNumber = 7;
                refresh(false, QBSNumber);
            };
        }

        private void rdoSquad8Resualts_CheckedChanged(object sender, EventArgs e)
        {
            if (cbxTourneyDropDown.Size != null)
            {
                QBSNumber = 8;
                refresh(false, QBSNumber);
            }

        }
        #endregion  

        private void btnDelete_Click(object sender, EventArgs e)
        {
            //needs to delete current member information from datbase in all important places
            if (selectedTournament.Doubles == false)
            {
                if (overallListOfParticipants.Count == 0)
                {
                    var confirm = MessageBox.Show(@"No players currently in tournament, would you like to delete the Tournament?", @"Confirm Save", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (confirm == DialogResult.No)
                        return;
                    else
                    {
                        //delete tournament if there are no participants.
                        Tournament t = TournamentDb.getTourneyByID(selectedTournament.Id);
                        TournamentDb.deleteTournament(t);
                        ResetFields();
                        refresh(false, QBSNumber);
                        currentIndex = 0;
                        RecordIndex(overallListOfParticipants);
                        cbxTourneyDropDown.DataSource = TournamentDb.GetTournamentList(RegionID);
                        cbxTourneyDropDown.DisplayMember = "TourneyNameDate";
                        cbxTourneyDropDown.ValueMember = "Id";
                        if (TournamentDb.GetTournamentList(RegionID).Count <= 0)
                        {
                            btnDelete.Enabled = false;
                            btnLeftArrow.Enabled = false;
                            btnRightArrow.Enabled = false;
                        }

                        return;
                    }
                }

                try
                {

                    Game g = GetScoresById(currentMem.Id);
                    //Delete from player history
                    PlayerHistory p = PlayerHistoryDB.getPlayerHistoryByGameID(g.Id);
                    PlayerHistoryDB.DeletePlayerHistory(p);
                    //Delete from FinalizeTemp
                    FinalizeTemp ft = FinalizeTempDB.getFinalizeID(FinalizeTempDB.getGame(g.Id));
                    FinalizeTempDB.DeleteFinilizeTemp(ft);
                    //Delete from Participants list
                    Participant par = FinalizeTempDB.getParticipantbyGameID(g.Id);
                    FinalizeTempDB.deleteParticipant(par);
                    //Delete the game itself
                    PlayerHistoryDB.DeleteGame(g);

                    //resets all the feilds back to what it wouldve looked like withought such record existing
                    ResetFields();
                    refresh(false, QBSNumber);
                    RecordIndex(overallListOfParticipants);
                    cbxTourneyDropDown.DataSource = TournamentDb.GetTournamentList(RegionID);
                    overallListOfParticipants = TournamentDb.GetTournamentMemberList(selectedTournament);
                    cbxTourneyDropDown.DisplayMember = "TourneyNameDate";
                    cbxTourneyDropDown.ValueMember = "Id";
                    //corrects any changes to the members stats after finalizing to the last accurate data
                    List<PlayerHistory> temp = PlayerHistoryDB.getLastFiveFromPlayerhistory(currentMem.Number, RegionID);
                    currentMem.Handicap = temp[0].HandiCap;
                    currentMem.Bonus = temp[0].Bonus;
                    currentMem.StartAvg = temp[0].AVG; // avg will have to be adjusted manually by director if last player history avg was not correct
                    currentMem.Average = Convert.ToInt32(temp[0].trueAVG);
                    MemberDb.AddMember(currentMem);
                }
                catch
                {
                    MessageBox.Show("Current Stats Not added to Tournament yet.");
                }
            }
        }

        public void UpdateTourneyComboBox()
        {
            RegionID = ((FrmMain)MdiParent).RegionID;
            List<Tournament> t = TournamentDb.GetTournamentList(RegionID);
            ((FrmMain)MdiParent)._tournamentList = t;
            cbxTourneyDropDown.DataSource = t;
            cbxTourneyDropDown.DisplayMember = "TourneyNameDate";
        }

        private void btnTournamentResults_Click(object sender, EventArgs e)
        {
            FrmTournamentResults form = new FrmTournamentResults();
            form.ShowDialog();
        }

        private void frmMemberScores_Resize(object sender, EventArgs e)
        {
            SetFlowDirection();
        }

        private void SetFlowDirection()
        {
            if (Convert.ToInt32(Form.ActiveForm.Size.Width) > 1100 && Convert.ToInt32(Form.ActiveForm.Size.Height) < 766)
            {
                flpMemberScores.FlowDirection = FlowDirection.TopDown;
            }
            else
            {
                flpMemberScores.FlowDirection = FlowDirection.LeftToRight;
            }
        }

        private void flpMemberScores_SizeChanged(object sender, EventArgs e)
        {
            SetFlowControlScrollBars();
        }

        private void SetFlowControlScrollBars()
        {
            if (Convert.ToInt32(Form.ActiveForm.Size.Width) < 1300)
            {
                flpMemberScores.HorizontalScroll.Visible = true;
                flpMemberScores.HorizontalScroll.Enabled = true;
            }
            if (Convert.ToInt32(Form.ActiveForm.Size.Height) < 750)
            {
                flpMemberScores.VerticalScroll.Visible = true;
                flpMemberScores.VerticalScroll.Enabled = true;
            }
            if (Convert.ToInt32(Form.ActiveForm.Size.Width) > 1300)
            {
                flpMemberScores.HorizontalScroll.Visible = false;
                flpMemberScores.HorizontalScroll.Enabled = false;
            }
            if (Convert.ToInt32(Form.ActiveForm.Size.Height) > 750)
            {
                flpMemberScores.VerticalScroll.Visible = false;
                flpMemberScores.VerticalScroll.Enabled = false;
            }
        }
    }

    /************************************************************************/

    }
    /// <summary>
    /// Class used to populate 3rd RichTextBox
    /// </summary>
    public partial class TopScores
    {
        public TopScores()
        {

        }
        #region Properties

        public int memberID{get; set; } 
        public string FirstName { get; set; }
        public string LastName { get; set; }       
        public int Placing { get; set; }
        public int? ScratchTotal { get; set; }
        public int HandicapScore { get; set; }
        public int? Top3ScratchScore { get; set; }
        public int? Top3HandiScores { get; set; }
        public int? Game1 { get; set; }
        public int? Game2 { get; set; }
        public int? Game3 { get; set; }
        public int? Game4 { get; set; }
        public int Handicap { get; set; }
        public int Bonus { get; set; }
        public int GameID { get; set; }
        #endregion
    }

