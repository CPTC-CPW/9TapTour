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
using System.Configuration;
using System.Data.Entity.Core.Objects;
using Bogus.Extensions;
using NineTapTour.Models;
using NineTapTour.Models.ViewModels;

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

        //QBS number is set by the radio buttons on the right side of the form "QUALIFY BY SQUAD" depending on which radio button is selected
        //it will change it to the corresponding value.
        int QBSNumber = 0;
        frmNewTournament currentTourneyPage;
        List<int> howManySquadsCanBeFiltered = new List<int>();


        public frmMemberScores()
        {
            InitializeComponent();
            DoubleInitialize(false);

        }

        private void RadioIntialize()
        {
            rdoSquadOne.TabStop = false;
            rdoSquadTwo.TabStop = false;
            rdoSquadThree.TabStop = false;
            rdoSquadFour.TabStop = false;
            rdoSquad5.TabStop = false;
            rdoSquad6.TabStop = false;
            rdoSquad7.TabStop = false;
            rdoSquad8.TabStop = false;
            rdoHandicapScore.TabStop = false;
            rdoAllResults.TabStop = false;
            cbFilterSquad5.Visible = false;
            cbFilterSquad6.Visible = false;
            cbFilterSquad7.Visible = false;
            cbFilterSquad8.Visible = false;
            rdoSquad5.Visible = false;
            rdoSquad6.Visible = false;
            rdoSquad7.Visible = false;
            rdoSquad8.Visible = false;
            rdoSquad5Results.Visible = false;
            rdoSquad6Results.Visible = false;
            rdoSquad7Results.Visible = false;
            rdoSquad8Results.Visible = false;
            cbAllSquads.Checked = true;
            if (cbxTourneyDropDown.SelectedIndex >= 0)
            {
                if (selectedTournament.Squads == 5)
                {
                    rdoSquad5.Visible = true;
                    rdoSquad5Results.Visible = true;
                    cbFilterSquad5.Visible = true;
                   

                }
                if (selectedTournament.Squads == 6)
                {
                    rdoSquad5.Visible = true;
                    rdoSquad6.Visible = true;
                    rdoSquad5Results.Visible = true;
                    rdoSquad6Results.Visible = true;
                    cbFilterSquad5.Visible = true;
                    cbFilterSquad6.Visible = true;
                }
                if (selectedTournament.Squads == 7)
                {
                    rdoSquad5.Visible = true;
                    rdoSquad6.Visible = true;
                    rdoSquad7.Visible = true;
                    rdoSquad5Results.Visible = true;
                    rdoSquad6Results.Visible = true;
                    rdoSquad7Results.Visible = true;
                    cbFilterSquad5.Visible = true;
                    cbFilterSquad6.Visible = true;
                    cbFilterSquad7.Visible = true;

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
                    rdoSquad8Results.Visible = true;
                    cbFilterSquad5.Visible = true;
                    cbFilterSquad6.Visible = true;
                    cbFilterSquad7.Visible = true;
                    cbFilterSquad8.Visible = true;
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
        /// clears the forms member scores
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmMemberScores_Activated(object sender, EventArgs e)
        {
            RegionID = ((FrmMain)MdiParent).RegionID;

            //addedd in this line inorder to prevent the reset of the drop down list on memberscores form when switching between forms
            int tempcbx = cbxTourneyDropDown.SelectedIndex;
            rdoHandicapScore.Visible = false;
            rdoScratchScore.Visible = false;
            cbxTourneyDropDown.Visible = false;
            ResetFields();

            MemberStatus("", Color.Black, SystemColors.Control, true);
            //cbxTourneyDropDown.DataSource = ((FrmMain)MdiParent)._tournamentList;
            


            List<Tournament> temp2 = TournamentDb.GetTournamentList(RegionID);

            ((FrmMain)MdiParent)._tournamentList = temp2;
            cbxTourneyDropDown.DataSource = temp2;
            cbxTourneyDropDown.DisplayMember = "TourneyNameDate";
            cbxTourneyDropDown.ValueMember = "Id";

            if (temp2.Count() > 0)
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
                // Gets the record for the selected tournament
                RecordIndex(TournamentDb.GetTournamentMemberList(GetTournamentById(selectedTournament.Id)));
                overallListOfParticipants = TournamentDb.GetTournamentMemberList(selectedTournament);
                btnDelete.Enabled = true;
               
                Refresh(false, QBSNumber);
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
                //currentGame.Bonus = currentMem.Bonus;
                //currentGame.Handicap = currentMem.Handicap;

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
                        currentMem = MemberDb.GetMember(memberNumber, RegionID);
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
                            //// This code was setting the member.handicap and txtHandicap to the last ph.handicap
                            //// which was the original member.handicap, however, this handicap then was saved as the 
                            //// game.handicap, finalizeTemp.handicap and ph.handicap, so it never changed
                            //// It never reflected the correct current member.handicap

                            ////check to make sure the right numbers are being brought over from the members information page
                            //List<PlayerHistory> last5 = PlayerHistoryDB.getLastFiveFromPlayerhistory(currentMem.Number, RegionID);
                            //if (last5.Count > 0)
                            //{
                            //    if (last5[0].HandiCap != currentMem.Handicap || last5[0].Bonus != currentMem.Bonus)
                            //    {
                            //        currentMem.Bonus = last5[0].Bonus;
                            //        currentMem.Handicap = last5[0].HandiCap;
                            //        txtHandicap.Text = last5[0].HandiCap.ToString();
                            //        txtBonusPins.Text = last5[0].Bonus.ToString();
                            //    }
                            //    else
                            //    {

                            //        txtHandicap.Text = currentMem.Handicap.ToString();
                            //        txtBonusPins.Text = currentMem.Bonus.ToString();
                            //    }


                            //}
                            //else
                            //{
                            //    currentMem.Bonus = 0;
                            //    txtHandicap.Text = currentMem.Bonus.ToString();
                            //    txtBonusPins.Text = currentMem.Bonus.ToString();
                            //}

                            #endregion


                            Game currentGame = GetScoresById(currentMem.Id);

                            //set the handicap and bonus pins to their most recent if they were not added to the tournament yet
                            if (currentGame == null)
                            {
                                txtHandicap.Text = currentMem.Handicap.ToString();
                                txtBonusPins.Text = currentMem.Bonus.ToString();
                            }
                            else //sets the right historic bowler handicap and bonus pins during this tournament
                            {
                                txtHandicap.Text = currentGame.Handicap.ToString();
                                txtBonusPins.Text = currentGame.Bonus.ToString();
                            }






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


            //
            if (IsValid())
            {
                //gets the current tournament from the database 
                Tournament currTourney = GetTournamentById(Convert.ToInt32(cbxTourneyDropDown.SelectedValue));

                //get all the current members participating in the current tournament
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

                    player.Member = MemberDb.GetMember(Convert.ToInt32(txtMemberNum.Text), RegionID);
                    player.Id = total.Count;
                    player2.Member = MemberDb.GetMember(Convert.ToInt32(txtMemberNum2.Text), RegionID);
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
                        Clear();


                    }
                    catch (MemberAccessException ex)
                    {
                        MessageBox.Show(ex.Message);

                    }
                    Clear();
                    txtMemberNum.Focus();
                }
                //IF the tournament type is NOT a DOUBLES tournament
                else
                {
                    int squad = 0;
                    #region get Squad
                    if (rdoSquadOne.Checked == true)
                    {
                        squad = 1;
                    }
                    else if (rdoSquadTwo.Checked == true)
                    {
                        squad = 2;
                    }
                    else if (rdoSquadThree.Checked == true)
                    {
                        squad = 3;
                    }
                    else if (rdoSquadFour.Checked == true)
                    {
                        squad = 4;
                    }
                    else if (rdoSquad5.Checked == true)
                    {
                        squad = 5;
                    }
                    else if (rdoSquad6.Checked == true)
                    {
                        squad = 6;
                    }
                    else if (rdoSquad7.Checked == true)
                    {
                        squad = 7;
                    }
                    else if (rdoSquad8.Checked == true)
                    {
                        squad = 8;
                    }
                    #endregion


                    //get the member from the database using the number from the memnum textbox
                    currentMem = MemberDb.GetMember(Convert.ToInt32(txtMemberNum.Text), RegionID);
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

                    if (parID == 0) //if participant doesnt exist yet give them a participantID
                    {
                        player.Id = parList.Count + 1;
                    }
                    else
                    {
                        player.Id = parID;
                    }

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
                    else if (rdoSquadFour.Checked)
                    {
                        player.Squad = 4;
                    }
                    else if (rdoSquad5.Checked)
                    {
                        player.Squad = 5;
                    }
                    else if (rdoSquad6.Checked)
                    {
                        player.Squad = 6;
                    }
                    else if (rdoSquad7.Checked)
                    {
                        player.Squad = 7;
                    }
                    else
                    {
                        player.Squad = 8;
                    }
                    #endregion
                    //defaults money earned to 0, or enters text box amount
                    if (txtMoney.Text == "" || txtMoney.Text == null)
                        player.Game.MoneyWon = 0;

                    else
                        player.Game.MoneyWon = Convert.ToDecimal(txtMoney.Text);
                    
                    

                    // TODO: fix issue with not letting less than 4 scores inputted

                    //if (string.IsNullOrEmpty(txtScratchScore1.Text.Trim()) || string.IsNullOrEmpty(txtScratchScore2.Text.Trim())
                    //    || string.IsNullOrEmpty(txtScratchScore3.Text.Trim()) || string.IsNullOrEmpty(txtScratchScore4.Text.Trim()))
                    //{
                    //    MessageBox.Show("Please enter all scratch scores", "Blank Scores Not Allowed");
                    //    return;
                    //}

                    //else if (!isNumeric(txtScratchScore1.Text.Trim()) || !isNumeric(txtScratchScore2.Text.Trim())
                    //    || !isNumeric(txtScratchScore3.Text.Trim()) || !isNumeric(txtScratchScore4.Text.Trim()))
                    //{

                    //    MessageBox.Show("Please enter only numbers", "Non-Integer Scores Not Allowed");
                    //    return;
                    //}

                    //for (int i = 0; i < scratchArray.Length; i++)
                    //{
                    //    if (!isNumeric(scratchArray[i].Text.Trim()))
                    //    {
                    //        if (string.IsNullOrWhiteSpace(scratchArray[i].Text))
                    //        {

                    //        }
                    //        MessageBox.Show("Please enter only numbers", "Non-Integer Scores Not Allowed");
                    //        return;
                    //    }
                    //}

                    
                    //where else would start
                        player.Game.Game1 = IsEmpty(txtScratchScore1) ? null : (int?)Convert.ToInt32((scratchArray[0].Text));
                        player.Game.Game2 = IsEmpty(txtScratchScore2) ? null : (int?)Convert.ToInt32((scratchArray[1].Text));
                        player.Game.Game3 = IsEmpty(txtScratchScore3) ? null : (int?)Convert.ToInt32((scratchArray[2].Text));
                        player.Game.Game4 = IsEmpty(txtScratchScore4) ? null : (int?)Convert.ToInt32((scratchArray[3].Text));

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
                            TournamentDb.AddMemberToTournament(player);
#if DEBUG
                            MessageBox.Show(@"Bowler Added Successfully to Tournament!");
#endif
                            ResetFields();
                            txtMemberNum.Focus();
                            Clear();
                            List<Participant> utotal = TournamentDb.GetTournamentMemberList(currTourney);
                            RecordIndexAfterAddUpdate(utotal);
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
                            MemberDb.AddMember(currentMem);
                        }
                }
                Refresh(false, QBSNumber);
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
            //sets first index on start up and switching of tournaments
            int temp = 0;
            if (cbxTourneyDropDown.SelectedIndex < 0)
            {
                lblRecord.Text = "Record " + (temp) + " / " + players.Count();
            }
            else if (players.Count == 0)
            {
                lblRecord.Text = "Record " + (temp) + " / " + players.Count();
            }
            else
            {
                currentIndex = 1;
                if (players[currentIndex - 1].Squad == 1)
                {
                    rdoSquadOne.Checked = true;
                }
                else if (players[currentIndex - 1].Squad == 2)
                {
                    rdoSquadTwo.Checked = true;
                }
                else if (players[currentIndex - 1].Squad == 3)
                {
                    rdoSquadThree.Checked = true;
                }
                else if (players[currentIndex - 1].Squad == 4)
                {
                    rdoSquadFour.Checked = true;
                }
                else if (players[currentIndex - 1].Squad == 5)
                {
                    rdoSquad5.Checked = true;
                }
                else if (players[currentIndex - 1].Squad == 6)
                {
                    rdoSquad6.Checked = true;
                }
                else if (players[currentIndex - 1].Squad == 7)
                {
                    rdoSquad7.Checked = true;
                }
                else if (players[currentIndex - 1].Squad == 8)
                {
                    rdoSquad8.Checked = true;
                }


                lblRecord.Text = "Record " + (currentIndex) + " / " + players.Count();
                txtMemberNum.Text = players[currentIndex - 1].Member.Number.ToString();
                FillMember();

            }


        }

        /// <summary>
        /// updates the record index after the button is clicked, making the record go to the next potential added player
        /// </summary>
        /// <param name="pat"> a list of participant objects </param>
        public void RecordIndexAfterAddUpdate(List<Participant> pat)
        {
            lblRecord.Text = "Record " + (pat.Count + 1) + " / " + pat.Count();
            currentIndex = pat.Count + 1;
        }


        public void RecordIndexOnEnter(List<Participant> part)
        {

            //on enter, find the first index in which the member occurs in the tournament
            if (selectedTournament.Doubles == false)
            {
                if (txtMemberNum.Text != "" && txtMemberNum.Text.All(Char.IsDigit))
                {
                    currentMem = MemberDb.GetMember(Convert.ToInt32(txtMemberNum.Text), RegionID);

                    int currentsNum = 0;
                    if (rdoSquadOne.Checked)
                        currentsNum = 1;
                    else if (rdoSquadTwo.Checked)
                        currentsNum = 2;
                    else if (rdoSquadThree.Checked)
                        currentsNum = 3;
                    else if (rdoSquadFour.Checked)
                        currentsNum = 4;
                    else if (rdoSquad5.Checked)
                        currentsNum = 5;
                    else if (rdoSquad6.Checked)
                        currentsNum = 6;
                    else if (rdoSquad7.Checked)
                        currentsNum = 7;
                    else if (rdoSquad8.Checked)
                        currentsNum = 8;

                    for (int i = 0; i < part.Count; i++)
                    {
                        if (currentMem.Id == part[i].Member.Id && part[i].Squad == currentsNum)
                        {
                            lblRecord.Text = "Record " + (i + 1) + " / " + part.Count();
                            currentIndex = i + 1;

                            break;
                        }
                        //if no break occurs, set the current index to that of the next potential index
                        lblRecord.Text = "Record " + (part.Count + 1) + " / " + part.Count();
                        currentIndex = part.Count + 1;

                    }


                }
            }

        }

        public void RecordIndexOnSquadSwitch(List<Participant> part)
        {
            int squad = 0;
            if (selectedTournament.Doubles == false)
            {
                if (txtMemberNum.Text != "")
                {
                    if (rdoSquadOne.Checked == true)
                    {
                        squad = 1;
                    }
                    else if (rdoSquadTwo.Checked == true)
                    {
                        squad = 2;
                    }
                    else if (rdoSquadThree.Checked == true)
                    {
                        squad = 3;
                    }
                    else if (rdoSquadFour.Checked == true)
                    {
                        squad = 4;
                    }
                    else if (rdoSquad5.Checked == true)
                    {
                        squad = 5;
                    }
                    else if (rdoSquad6.Checked == true)
                    {
                        squad = 6;
                    }
                    else if (rdoSquad7.Checked == true)
                    {
                        squad = 7;
                    }
                    else if (rdoSquad8.Checked == true)
                    {
                        squad = 8;
                    }

                    for (int i = 0; i < part.Count; i++)
                    {
                        if (currentMem.Id == part[i].Member.Id && part[i].Squad == squad)
                        {
                            lblRecord.Text = "Record " + (i + 1) + " / " + part.Count();
                            currentIndex = i + 1;
                            break;
                        }
                        //if no break occurs, set the current index to that of the next potential index
                        lblRecord.Text = "Record " + (part.Count + 1) + " / " + part.Count();
                        currentIndex = part.Count + 1;
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
        private void Clear()
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
            currentIndex++;
            if (currentIndex > total.Count())
            {
                MessageBox.Show("There are no more players to go to!");
                currentIndex--; // if it cant go up more then reset the index back to right index
            }
            else
            {
                buttonCheck = true; // right button clicked
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

                lblRecord.Text = "Record " + (currentIndex) + " / " + total.Count();

                FillMember();
            }
        }

        /// <summary>
        /// decrements to the previous participant in the tournament
        /// </summary>
        private void btnLeftArrow_Click(object sender, EventArgs e)
        {
            List<Participant> total = TournamentDb.GetTournamentMemberList(GetTournamentById(Convert.ToInt32(cbxTourneyDropDown.SelectedValue)));
            currentIndex--;
            if (currentIndex <= 0)
            {
                MessageBox.Show("There are no more players to go back to!");
                currentIndex++; //if it cant go down anymore, set the number back to the correct index
            }
            else
            {

                if (currentIndex <= 0)
                {
                    MessageBox.Show("You can't go back!");
                }
                else
                {
                    buttonCheck = false;  //Left button clicked


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


                    lblRecord.Text = "Record " + (currentIndex) + " / " + total.Count();

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
                btnDelete.Enabled = true;
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
                overallListOfParticipants = TournamentDb.GetTournamentMemberList(selectedTournament);
                Refresh(false, QBSNumber);
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
            if (txtMemberNum.Text == "")
            {
                return false;
            }
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
            if (tours.Count() > 0)
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
            Refresh(true, QBSNumber);
        }

        private void rdoHandicapScore_CheckedChanged(object sender, EventArgs e)
        {
            Refresh(true, QBSNumber);
        }

        /// <summary>
        /// pass true if you are changing the radio buttons and only want to refresh the bottom box.
        /// </summary>
        /// <param name="seriesChange"></param>


        /* Todo:       ********************************************************************************************************************************************
        ********************************************************************************************************************************************
        ********************************************************************************************************************************************
        ********************************************************************************************************************************************
        ********************************************************************************************************************************************
         * ********************************************************************************************************************************************
         * ********************************************************************************************************************************************
         * ********************************************************************************************************************************************
         * ********************************************************************************************************************************************
         *
             */

        List<TopScores> listOfTopScore = new List<TopScores>();
        IComparer<MemberScores> scoreComparer = new MemberScoresComparer();
        public void Refresh(bool seriesChange, int qbsNumber)
        {
            var scores = new List<MemberScores>();
            listOfTopScore.Clear();
            // DEV NOTE: The text generated for the boxes in this is strange and has tabs that the 
            // code doesn't seem to be writing as far as I can tell.
            // I think a bug fixer should look at this some time and try to see why it's happening
            try
            {
                // Function scope data
                NineTapDb db = new NineTapDb();

                int selectedTourney = selectedTournament.Id;

                var listOfParticipants = ParticipantsDB.GetParticipants(selectedTournament.Id);

                var topScores = listOfParticipants.GroupBy(p => p.Member.Id).Select(pg => pg.Max()).ToList();

                //TAKES A TOURNAMENT ID AND SQUAD NUMBER AND FILTERS FOR A LIST OF PARTICIPANTS.
                if (qbsNumber > 0 && qbsNumber <= 8)
                    listOfParticipants = listOfParticipants.Where(p => p.Squad == qbsNumber).ToList();

                else if(howManySquadsCanBeFiltered.Count > 0 && QBSNumber == 9)
                    //filters out each squad
                    //take the list of participants where => if the squad number equals to any of the filtered numbers.
                    listOfParticipants = listOfParticipants.Where(p => howManySquadsCanBeFiltered.Any(h => h == p.Squad)).ToList();
                try
                {
                    int id = 0;
                    int count = 0;
                    var testScores = new List<TopParticipantGameViewModel>();
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
                            new TopParticipantGameViewModel(currParticipant.Member.Id, currParticipant.Member.FirstName,
                                currParticipant.Member.LastName, 0, currParticipant.Game.AllGameScores().Sum().Value, 
                                top3Games.Sum(), top3Games.Sum() + (3 * currParticipant.Member.Handicap) + (3 * currParticipant.Game.Bonus), 
                                currParticipant.Game.Game1, currParticipant.Game.Game2, currParticipant.Game.Game3, currParticipant.Game.Game4, 
                                currParticipant.Game.Handicap, currParticipant.Game.Bonus.Value, currParticipant.Game.Id);
                        testScores.Add(currTopScoreViewModel);
                        


                        TopScores temp = new TopScores();
                        listOfTopScore.Add(temp);

                        // set id to current member
                        id = currParticipant.Member.Id;

                        // Populates info                         
                        listOfTopScore[count].FirstName = currParticipant.Member.FirstName;
                        listOfTopScore[count].LastName = currParticipant.Member.LastName;
                        listOfTopScore[count].Game1 = currParticipant.Game.Game1;
                        listOfTopScore[count].Game2 = currParticipant.Game.Game2;
                        listOfTopScore[count].Game3 = currParticipant.Game.Game3;
                        listOfTopScore[count].Game4 = currParticipant.Game.Game4;
                        listOfTopScore[count].GameID = currParticipant.Game.Id;
                        listOfTopScore[count].Handicap = currParticipant.Member.Handicap;
                        listOfTopScore[count].memberID = id;
                        //todo: change this as this is uneedeed
                        try
                        {
                            listOfTopScore[count].Bonus = currParticipant.Member.Bonus;
                        }
                        catch
                        {
                            listOfTopScore[count].Bonus = 0;
                        }

                        topScores[count].Game.TotalScore;
                        listOfTopScore[count].ScratchTotal = totalScore;
                        listOfTopScore[count].HandicapScore = totalScore + (listOfTopScore[count].Handicap * 4) + (listOfTopScore[count].Bonus * 4);//TODO: make "game count flexible"
                        listOfTopScore[count].Top3ScratchScore = top3Games[0] + top3Games[1] + top3Games[2];
                        listOfTopScore[count].Top3HandiScores = top3Games[0] + top3Games[1] + top3Games[2] + (3 * currParticipant.Member.Handicap) + (3 * listOfTopScore[count].Bonus);
                        count++;
                    }
                }
                catch (SqlException)
                {
                    //what is the 3rd box?
                    listOfTopScore.Clear(); //filter out if there is no one on the squad yet so the 3rd box won't get populated
                }

                overallListOfTopScores = listOfTopScore;
                // Top 5 LINQ query
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
                            int firstNameLength = 0;
                            int lastNameLength = 0;
                            firstNameLength = scores[i].FirstName.Length < 6 ? scores[i].FirstName.Length : 6;
                            lastNameLength = scores[i].LastName.Length < 6 ? scores[i].LastName.Length : 6;
                            
                            richTextBox1.AppendText($"{i + 1}\t{scores[i].FirstName.Substring(0, firstNameLength)}\t{scores[i].LastName.Substring(0, lastNameLength)}\t\t\t{scores[i].Score}\n");
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
                            int firstNameLength = 0;
                            int lastNameLength = 0;
                            firstNameLength = scores[i].FirstName.Length < 6 ? scores[i].FirstName.Length : 6;
                            lastNameLength = scores[i].LastName.Length < 6 ? scores[i].LastName.Length : 6;

                            richTextBox1.AppendText($"{i + 1}\t{scores[i].FirstName.Substring(0, firstNameLength)}\t{scores[i].LastName.Substring(0, lastNameLength)}\t\t\t{scores[i].Score}\n");
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
                    scores.Clear();

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
                            int firstNameLength = 0;
                            int lastNameLength = 0;
                            firstNameLength = scores[i].FirstName.Length < 6 ? scores[i].FirstName.Length : 6;
                            lastNameLength = scores[i].LastName.Length < 6 ? scores[i].LastName.Length : 6;
                            richTextBox2.AppendText($"{i + 1}\t{scores[i].FirstName.Substring(0, firstNameLength)}\t{scores[i].LastName.Substring(0, lastNameLength)}\t\t\t{scores[i].Score}\n");
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
                            int firstNameLength = 0;
                            int lastNameLength = 0;
                            firstNameLength = scores[i].FirstName.Length < 6 ? scores[i].FirstName.Length : 6;
                            lastNameLength = scores[i].LastName.Length < 6 ? scores[i].LastName.Length : 6;
                            //richTextBox2.AppendText((i + 1).ToString() + "\t" + String.Format("{0, -20}", scores[i].FirstName + " " + scores[i].LastName)
                            //                        + "\t" + String.Format("{0, -5}", scores[i].Score + " " + "\n"));
                            richTextBox2.AppendText($"{i + 1}\t{scores[i].FirstName.Substring(0, firstNameLength)}\t{scores[i].LastName.Substring(0, lastNameLength)}\t\t\t{scores[i].Score}\n");

                        }
                    }
                }
                #endregion

                #region Populates 3rd Box
                if (!selectedTournament.ThreeOutOf4)
                {
                    /////////////////////////////////
                    richTextBox3.Clear();
                    richTextBox3.Font = new Font(FontFamily.GenericMonospace, richTextBox3.Font.Size);
                    richTextBox3.Text = ("#" + "\t" + "Name" + "\t\t" + "High Series" + "\n");
                    scores.Clear();

                    //populate total score
                    if (rdoScratchScore.Checked)
                    {
                        foreach (var s in listOfTopScore)
                        {
                            scores.Add(new MemberScores { FirstName = s.FirstName, LastName = s.LastName, Score = s.allGameScores().Where(sc => sc.HasValue).Sum(), MemberId = s.memberID });
                        }

                        scores.Sort(scoreComparer);
                        scores.Reverse();
                        scores = scores.ToList();
                        for (int i = 0; i < scores.Count(); i++)
                        {
                            int firstNameLength = 0;
                            int lastNameLength = 0;
                            firstNameLength = scores[i].FirstName.Length < 6 ? scores[i].FirstName.Length : 6;
                            lastNameLength = scores[i].LastName.Length < 6 ? scores[i].LastName.Length : 6;

                            CalculatePlaceStanding(scores);

                            richTextBox3.AppendText($"{scores[i].placing}\t{scores[i].FirstName.Substring(0, firstNameLength)}\t{scores[i].LastName.Substring(0, lastNameLength)}\t\t\t{scores[i].Score}\n");
                        }
                    }
                    else if (rdoHandicapScore.Checked)
                    {
                        foreach (var s in listOfTopScore)
                        {
                            #region conditions for highest handicap scores
                            
                            #endregion
                            scores.Add(new MemberScores { FirstName = s.FirstName, LastName = s.LastName, Score = s.allGameScores().Sum() + (s.allGameScores().Count * s.Handicap) + (s.allGameScores().Count * s.Bonus), MemberId = s.memberID });
                        }
                        scores.Sort(scoreComparer);
                        scores.Reverse();
                        scores = scores.ToList();
                        for (int i = 0; i < scores.Count(); i++)
                        {
                            int firstNameLength = 0;
                            int lastNameLength = 0;
                            firstNameLength = scores[i].FirstName.Length < 6 ? scores[i].FirstName.Length : 6;
                            lastNameLength = scores[i].LastName.Length < 6 ? scores[i].LastName.Length : 6;

                            CalculatePlaceStanding(scores);

                            richTextBox3.AppendText($"{scores[i].placing}\t{scores[i].FirstName.Substring(0, firstNameLength)}\t{scores[i].LastName.Substring(0, lastNameLength)}\t\t\t{scores[i].Score}\n");
                        }
                    }
                }
                #endregion

                #region Three Out Of 4
                /////////////////////////////////////////////////////
                // Executes if tournament selected is 3 Out of 4 ///
                /////////////////////////////////////////////////////
                if (selectedTournament.ThreeOutOf4)
                {
                    /////////////////////////////////
                    richTextBox3.Clear();
                    richTextBox3.Font = new Font(FontFamily.GenericMonospace, richTextBox3.Font.Size);
                    richTextBox3.Text = ("#" + "\t" + "Name" + "\t\t\t" + "High Series" + "\n");
                    scores.Clear();

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

                            //*************************
                            scores.Add(new MemberScores { FirstName = s.FirstName, LastName = s.LastName, Score = listOfScores[0] + listOfScores[1] + listOfScores[2] });
                            listOfScores.Clear();
                        }

                        scores.Sort(scoreComparer);
                        scores.Reverse();
                        scores = scores.ToList();
                        for (int i = 0; i < scores.Count(); i++)
                        {
                            int firstNameLength = 0;
                            int lastNameLength = 0;
                            firstNameLength = scores[i].FirstName.Length < 6 ? scores[i].FirstName.Length : 6;
                            lastNameLength = scores[i].LastName.Length < 6 ? scores[i].LastName.Length : 6;

                            CalculatePlaceStanding(scores);

                            richTextBox3.AppendText($"{scores[i].placing}\t{scores[i].FirstName.Substring(0, firstNameLength)}\t{scores[i].LastName.Substring(0, lastNameLength)}\t\t\t{scores[i].Score}\n");
                        }
                    }
                    else if (rdoHandicapScore.Checked)
                    {
                        foreach (var i in listOfTopScore)
                        {
                            #region conditions for highest handicap scores

                            #endregion
                            //***********************
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

                            //*************************
                            scores.Add(new MemberScores { FirstName = i.FirstName, LastName = i.LastName, Score = listOfScores[0] + listOfScores[1] + listOfScores[2] });
                            listOfScores.Clear();
                        }
                        scores.Sort(scoreComparer);
                        scores.Reverse();
                        scores = scores.ToList();
                        for (int i = 0; i < scores.Count(); i++)
                        {
                            int firstNameLength = 0;
                            int lastNameLength = 0;
                            firstNameLength = scores[i].FirstName.Length < 6 ? scores[i].FirstName.Length : 6;
                            lastNameLength = scores[i].LastName.Length < 6 ? scores[i].LastName.Length : 6;

                            CalculatePlaceStanding(scores);

                            richTextBox3.AppendText($"{scores[i].placing}\t{scores[i].FirstName.Substring(0, firstNameLength)}\t{scores[i].LastName.Substring(0, lastNameLength)}\t\t\t{scores[i].Score}\n");
                        }
                    }
                }
                #endregion            
            }
            finally
            {

            }//TODO ADDED FOR ERRORS REMOVE WHEN FIXED

            // Assign Place Standing from scores to overallListOfTopScores
            for (int i = 0; i < overallListOfTopScores.Count; i++)
            {
                foreach (var item in scores)
                {
                    if (overallListOfTopScores[i].memberID == item.MemberId)
                    {
                        overallListOfTopScores[i].Placing = item.placing;
                    }
                }
            }
        }


        /* Todo:       ********************************************************************************************************************************************
        ********************************************************************************************************************************************
        ********************************************************************************************************************************************
        ********************************************************************************************************************************************
        ********************************************************************************************************************************************
         * ********************************************************************************************************************************************
         * ********************************************************************************************************************************************
         * ********************************************************************************************************************************************
         * ********************************************************************************************************************************************
         *
             */





        /// <summary>
        /// Calculates each bowler's place standing. Accounts for ties.
        /// </summary>
        /// <param name="winners"></param>
        private static void CalculatePlaceStanding(List<MemberScores> winners)
        {
            int place = 1;
            for (int i = 0; i < winners.Count; i++)
            {
                if (i > 0 && winners[i].Score == winners[i - 1].Score)
                {
                    winners[i].placing = winners[i - 1].placing;
                }
                else
                {
                    winners[i].placing = place;
                }
                place++;
            }
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

            public int MemberId { get; set; } // Renamed MemberNo to MemberId because that is the actual info being assigned to this property

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
            {
                List<Participant> total = TournamentDb.GetTournamentMemberList(GetTournamentById(Convert.ToInt32(cbxTourneyDropDown.SelectedValue)));
                RecordIndexOnEnter(total);
                FillMember();
            }

        }
        //runs fill member when enter key is pressed on text box
        private void txtMemberNum2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {

                List<Participant> total = TournamentDb.GetTournamentMemberList(GetTournamentById(Convert.ToInt32(cbxTourneyDropDown.SelectedValue)));
                RecordIndexOnEnter(total);
                FillMember();
            }

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
                //Since this takes 20+ seconds to display the DGV this displays a swirling loading indicator.
                Cursor.Current = Cursors.WaitCursor;
                Application.DoEvents();

                var newFrmFinalizeTournament = new FrmFinalizeTournament(selectedTournament, overallListOfTopScores, RegionID);
                newFrmFinalizeTournament.Dock = DockStyle.Right;
                newFrmFinalizeTournament.WindowState = FormWindowState.Normal;
                newFrmFinalizeTournament.Show();
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

                                select new MemberScores { MemberId = g.Member.Number, FirstName = g.Member.FirstName, LastName = g.Member.LastName, Score = g.Game.Game1.Value, LastPaymentYear = (g.Member.IsLifetimeMember) ? "life " : g.Member.LastPayment.Value.Year.ToString(), Paid = (g.Member.IsLifetimeMember == true || !(g.Member.LastPayment != null && (g.Member.LastPayment.Value <= EntityFunctions.AddYears(DateTime.Now, -1)/* DateTime.Now.AddYears(-1)*/))) }).Concat(
                       (from g in (db.Participants.Include(b => b.Member)
                                           .Include(b => b.Game)
                                           .Where(b => b.Tournament.Id == selectedTournament.Id)
                                           .Where(b => b.Member.IsSenior))
                        select new MemberScores { MemberId = g.Member.Number, FirstName = g.Member.FirstName, LastName = g.Member.LastName, Score = g.Game.Game2.Value, LastPaymentYear = (g.Member.IsLifetimeMember) ? "life " : g.Member.LastPayment.Value.Year.ToString(), Paid = (g.Member.IsLifetimeMember == true || !(g.Member.LastPayment != null && (g.Member.LastPayment.Value <= EntityFunctions.AddYears(DateTime.Now, -1)))) })).Concat(
                       (from g in (db.Participants.Include(b => b.Member)
                                           .Include(b => b.Game)
                                           .Where(b => b.Tournament.Id == selectedTournament.Id)
                                           .Where(b => b.Member.IsSenior))
                        select new MemberScores { MemberId = g.Member.Number, FirstName = g.Member.FirstName, LastName = g.Member.LastName, Score = g.Game.Game3.Value, LastPaymentYear = (g.Member.IsLifetimeMember) ? "life " : g.Member.LastPayment.Value.Year.ToString(), Paid = (g.Member.IsLifetimeMember == true || !(g.Member.LastPayment != null && (g.Member.LastPayment.Value <= EntityFunctions.AddYears(DateTime.Now, -1)))) })).Concat(
                       (from g in (db.Participants.Include(b => b.Member)
                                           .Include(b => b.Game)
                                           .Where(b => b.Tournament.Id == selectedTournament.Id)
                                           .Where(b => b.Member.IsSenior))
                        select new MemberScores { MemberId = g.Member.Number, FirstName = g.Member.FirstName, LastName = g.Member.LastName, Score = g.Game.Game4.Value, LastPaymentYear = (g.Member.IsLifetimeMember) ? "life " : g.Member.LastPayment.Value.Year.ToString(), Paid = (g.Member.IsLifetimeMember == true || !(g.Member.LastPayment != null && (g.Member.LastPayment.Value <= EntityFunctions.AddYears(DateTime.Now, -1)))) })).ToList();
                    temp.Sort(scoreComparer);
                    temp.Reverse();

                    if (temp.Count() != 0)
                    {
                        //find out what squad is selected At the moment of series button click
                        int currentsNum = 0;
                        if (rdoSquad1Results.Checked)
                            currentsNum = 1;
                        else if (rdoSquad2Results.Checked)
                            currentsNum = 2;
                        else if (rdoSquad3Results.Checked)
                            currentsNum = 3;
                        else if (rdoSquad4Results.Checked)
                            currentsNum = 4;
                        else if (rdoSquad5Results.Checked)
                            currentsNum = 5;
                        else if (rdoSquad6Results.Checked)
                            currentsNum = 6;
                        else if (rdoSquad7Results.Checked)
                            currentsNum = 7;
                        else if (rdoSquad8Results.Checked)
                            currentsNum = 8;

                        FrmMemberScoresReports report = new FrmMemberScoresReports(temp, selectedTournament, 0/*reportTypeNum, 0 for High game handicap/senior, 1 for game/high game, 2 for series/high series*/,currentsNum);
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

                                select new MemberScores { MemberId = g.Member.Number, FirstName = g.Member.FirstName, LastName = g.Member.LastName, Score = g.Game.Game1.Value, LastPaymentYear = (g.Member.IsLifetimeMember) ? "life " : g.Member.LastPayment.Value.Year.ToString(), Paid = (g.Member.IsLifetimeMember == true || !(g.Member.LastPayment != null && (g.Member.LastPayment.Value <= EntityFunctions.AddYears(DateTime.Now, -1)))) }).Concat(
                        (from g in (db.Participants.Include(b => b.Member)
                                            .Include(b => b.Game)
                                            .Where(b => b.Tournament.Id == selectedTournament.Id))
                         select new MemberScores { MemberId = g.Member.Number, FirstName = g.Member.FirstName, LastName = g.Member.LastName, Score = g.Game.Game2.Value, LastPaymentYear = (g.Member.IsLifetimeMember) ? "life " : g.Member.LastPayment.Value.Year.ToString(), Paid = (g.Member.IsLifetimeMember == true || !(g.Member.LastPayment != null && (g.Member.LastPayment.Value <= EntityFunctions.AddYears(DateTime.Now, -1)))) })).Concat(
                        (from g in (db.Participants.Include(b => b.Member)
                                            .Include(b => b.Game)
                                            .Where(b => b.Tournament.Id == selectedTournament.Id))
                         select new MemberScores { MemberId = g.Member.Number, FirstName = g.Member.FirstName, LastName = g.Member.LastName, Score = g.Game.Game3.Value, LastPaymentYear = (g.Member.IsLifetimeMember) ? "life " : g.Member.LastPayment.Value.Year.ToString(), Paid = (g.Member.IsLifetimeMember == true || !(g.Member.LastPayment != null && (g.Member.LastPayment.Value <= EntityFunctions.AddYears(DateTime.Now, -1)))) })).Concat(
                        (from g in (db.Participants.Include(b => b.Member)
                                            .Include(b => b.Game)
                                            .Where(b => b.Tournament.Id == selectedTournament.Id))
                         select new MemberScores { MemberId = g.Member.Number, FirstName = g.Member.FirstName, LastName = g.Member.LastName, Score = g.Game.Game4.Value, LastPaymentYear = (g.Member.IsLifetimeMember) ? "life " : g.Member.LastPayment.Value.Year.ToString(), Paid = (g.Member.IsLifetimeMember == true || !(g.Member.LastPayment != null && (g.Member.LastPayment.Value <= EntityFunctions.AddYears(DateTime.Now, -1)))) })).ToList();
                    temp.Sort(scoreComparer);
                    temp.Reverse();

                    //find out what squad is selected At the moment of series button click
                    int currentsNum = 0;
                    if (rdoSquad1Results.Checked)
                        currentsNum = 1;
                    else if (rdoSquad2Results.Checked)
                        currentsNum = 2;
                    else if (rdoSquad3Results.Checked)
                        currentsNum = 3;
                    else if (rdoSquad4Results.Checked)
                        currentsNum = 4;
                    else if (rdoSquad5Results.Checked)
                        currentsNum = 5;
                    else if (rdoSquad6Results.Checked)
                        currentsNum = 6;
                    else if (rdoSquad7Results.Checked)
                        currentsNum = 7;
                    else if (rdoSquad8Results.Checked)
                        currentsNum = 8;


                    if (temp.Count() != 0)

                    {
                        FrmMemberScoresReports report = new FrmMemberScoresReports(temp, selectedTournament, 1/*reportTypeNum, 0 for High game handicap/senior, 1 for game/high game, 2 for series/high series*/, currentsNum);
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

                    //instead of recreating existing data, just set temp to whats populated in the third rich textbox at the time of pressing the "series" printing.
                    foreach (var s in overallListOfTopScores)
                    {
                        Member mem  = MemberDb.GetMember(MemberDb.GetMemberNumberbyID(s.memberID), RegionID);
                        if (selectedTournament.ThreeOutOf4 == false)
                        {
                            {
                                if (rdoScratchScore.Checked == true)
                                {                                                                                                                                 //technically we want the member Number here
                                    temp.Add(new MemberScores { FirstName = s.FirstName, LastName = s.LastName, Score = s.ScratchTotal, MemberId = mem.Number, placing = s.Placing, Paid = (mem.IsLifetimeMember == true || (mem.LastPayment != null && (mem.LastPayment.Value <= DateTime.Today.AddHours(-1)))) });
                                }
                                else
                                {
                                    temp.Add(new MemberScores { FirstName = s.FirstName, LastName = s.LastName, Score = s.HandicapScore, MemberId = mem.Number, placing = s.Placing, Paid = (mem.IsLifetimeMember == true || (mem.LastPayment != null && (mem.LastPayment.Value <= DateTime.Today.AddHours(-1)))) });
                                }
                            }
                        }
                        else
                        {
                            if (rdoScratchScore.Checked == true)
                            {                                                                                                                                 //technically we want the member Number here
                                temp.Add(new MemberScores { FirstName = s.FirstName, LastName = s.LastName, Score = s.Top3ScratchScore, MemberId = mem.Number, placing = s.Placing, Paid = (mem.IsLifetimeMember == true || (mem.LastPayment != null && (mem.LastPayment.Value <= DateTime.Today.AddHours(-1)))) });
                            }
                            else
                            {
                                temp.Add(new MemberScores { FirstName = s.FirstName, LastName = s.LastName, Score = s.Top3HandiScores, MemberId = mem.Number, placing = s.Placing, Paid = (mem.IsLifetimeMember == true || (mem.LastPayment != null && (mem.LastPayment.Value <= DateTime.Today.AddHours(-1)))) });
                            }


                        }
                    }




                    //these 2 regions would recreate data that already exists on trhe page
                    #region PRINTING HANDICAP TOURNAMENT RESULTS
                    //if (rdoHandicapScore.Checked)
                    //{
                    //    if (selectedTournament.ThreeOutOf4 && QBSNumber == 0) //overall best standings for 3of4 tournament
                    //    {
                    //        temp = (from g in (db.Participants.Include(b => b.Member)
                    //                                .Include(b => b.Game)
                    //                                .Where(b => b.Tournament.Id == selectedTournament.Id))
                    //                orderby (g.Game.Game1 + g.Game.Game2 + g.Game.Game3 + g.Game.Game4 + (g.Game.Handicap * 3 + g.Game.Bonus * 3) - (new List<int> { g.Game.Game1.Value, g.Game.Game2.Value, g.Game.Game3.Value, g.Game.Game4.Value }.Min())) descending
                    //                select new MemberScores { MemberId = g.Member.Number, FirstName = g.Member.FirstName, LastName = g.Member.LastName, Score = g.Game.Game1 + g.Game.Game2 + g.Game.Game3 + g.Game.Game4 + (g.Game.Handicap * 3) + (g.Game.Bonus * 3) - (new List<int> { g.Game.Game1.Value, g.Game.Game2.Value, g.Game.Game3.Value, g.Game.Game4.Value }.Min()), LastPaymentYear = (g.Member.IsLifetimeMember) ? "life " : g.Member.LastPayment.Value.Year.ToString(), Paid = (g.Member.IsLifetimeMember == true || !(g.Member.LastPayment != null && (g.Member.LastPayment.Value <= EntityFunctions.AddYears(DateTime.Now, -1)))) }).ToList();
                    //    }
                    //    else if (selectedTournament.ThreeOutOf4 && QBSNumber > 0) //best standings based on sqaud for  3of4 tournament
                    //    {
                    //        temp = (from g in (db.Participants.Include(b => b.Member)
                    //                      .Include(b => b.Game)
                    //                      .Where(b => b.Tournament.Id == selectedTournament.Id).Where(b => b.Squad == QBSNumber))
                    //                orderby (g.Game.Game1 + g.Game.Game2 + g.Game.Game3 + g.Game.Game4 + (g.Game.Handicap * 3 + g.Game.Bonus * 3) - (new List<int> { g.Game.Game1.Value, g.Game.Game2.Value, g.Game.Game3.Value, g.Game.Game4.Value }.Min())) descending
                    //                select new MemberScores { MemberId = g.Member.Number, FirstName = g.Member.FirstName, LastName = g.Member.LastName, Score = g.Game.Game1 + g.Game.Game2 + g.Game.Game3 + g.Game.Game4 + (g.Game.Handicap * 3) + (g.Game.Bonus * 3) - (new List<int> { g.Game.Game1.Value, g.Game.Game2.Value, g.Game.Game3.Value, g.Game.Game4.Value }.Min()), LastPaymentYear = (g.Member.IsLifetimeMember) ? "life " : g.Member.LastPayment.Value.Year.ToString(), Paid = (g.Member.IsLifetimeMember == true || !(g.Member.LastPayment != null && (g.Member.LastPayment.Value <= EntityFunctions.AddYears(DateTime.Now, -1)))) }).ToList();

                    //    }
                    //    else if (!selectedTournament.ThreeOutOf4 && QBSNumber == 0) //overall standings for a regular tournament
                    //    {
                    //        temp = (from g in (db.Participants.Include(b => b.Member)
                    //                                .Include(b => b.Game)
                    //                                .Where(b => b.Tournament.Id == selectedTournament.Id))
                    //                orderby ((g.Game.Game1 + g.Game.Bonus + g.Game.Handicap) + (g.Game.Game2 + g.Game.Bonus + g.Game.Handicap) + (g.Game.Game3 + g.Game.Bonus + g.Game.Handicap) + (g.Game.Game4 + g.Game.Bonus + g.Game.Handicap)) descending
                    //                select new MemberScores { MemberId = g.Member.Number, FirstName = g.Member.FirstName, LastName = g.Member.LastName, Score = (g.Game.Game1 + g.Game.Bonus + g.Game.Handicap) + (g.Game.Game2 + g.Game.Bonus + g.Game.Handicap) + (g.Game.Game3 + g.Game.Bonus + g.Game.Handicap) + (g.Game.Game4 + g.Game.Bonus + g.Game.Handicap), LastPaymentYear = (g.Member.IsLifetimeMember) ? "life " : g.Member.LastPayment.Value.Year.ToString(), Paid = (g.Member.IsLifetimeMember == true || !(g.Member.LastPayment != null && (g.Member.LastPayment.Value <= EntityFunctions.AddYears(DateTime.Now, -1)))) }).ToList();
                    //    }
                    //    else if (!selectedTournament.ThreeOutOf4 && QBSNumber > 0) //standings based on squad for a regular tournament
                    //    {
                    //        temp = (from g in (db.Participants.Include(b => b.Member)
                    //                                .Include(b => b.Game)
                    //                                .Where(b => b.Tournament.Id == selectedTournament.Id).Where(b => b.Squad == QBSNumber))
                    //                orderby (g.Game.Game1 + g.Game.Game2 + g.Game.Game3 + g.Game.Game4 + (g.Game.Handicap * 4 + g.Game.Bonus * 4)) descending
                    //                select new MemberScores { MemberId = g.Member.Number, FirstName = g.Member.FirstName, LastName = g.Member.LastName, Score = g.Game.Game1 + g.Game.Game2 + g.Game.Game3 + g.Game.Game4 + (g.Game.Handicap * 4) + (g.Game.Bonus * 4), LastPaymentYear = (g.Member.IsLifetimeMember) ? "life " : g.Member.LastPayment.Value.Year.ToString(), Paid = (g.Member.IsLifetimeMember == true || !(g.Member.LastPayment != null && (g.Member.LastPayment.Value <= EntityFunctions.AddYears(DateTime.Now, -1)))) }).ToList();
                    //    }
                    //}
                    #endregion
                    #region PRINTING SCRATCH TOURNAMENT RESULTS
                    //else if (rdoScratchScore.Checked)
                    //{
                    //    if (selectedTournament.ThreeOutOf4 && QBSNumber == 0) //overall best standings for 3of4 tournament
                    //    {
                    //        temp = (from g in (db.Participants.Include(b => b.Member)
                    //                                .Include(b => b.Game)
                    //                                .Where(b => b.Tournament.Id == selectedTournament.Id))
                    //                orderby (g.Game.Game1 + g.Game.Game2 + g.Game.Game3 + g.Game.Game4 - (new List<int> { g.Game.Game1.Value, g.Game.Game2.Value, g.Game.Game3.Value, g.Game.Game4.Value }.Min())) descending
                    //                select new MemberScores { MemberId = g.Member.Number, FirstName = g.Member.FirstName, LastName = g.Member.LastName, Score = g.Game.Game1 + g.Game.Game2 + g.Game.Game3 + g.Game.Game4 - (new List<int> { g.Game.Game1.Value, g.Game.Game2.Value, g.Game.Game3.Value, g.Game.Game4.Value }.Min()), LastPaymentYear = (g.Member.IsLifetimeMember) ? "life " : g.Member.LastPayment.Value.Year.ToString(), Paid = (g.Member.IsLifetimeMember == true || !(g.Member.LastPayment != null && (g.Member.LastPayment.Value <= EntityFunctions.AddYears(DateTime.Now, -1)))) }).ToList();
                    //    }
                    //    else if (selectedTournament.ThreeOutOf4 && QBSNumber > 0) //best standings based on sqaud for  3of4 tournament
                    //    {
                    //        temp = (from g in (db.Participants.Include(b => b.Member)
                    //                      .Include(b => b.Game)
                    //                      .Where(b => b.Tournament.Id == selectedTournament.Id).Where(b => b.Squad == QBSNumber))
                    //                orderby (g.Game.Game1 + g.Game.Game2 + g.Game.Game3 + g.Game.Game4 - (new List<int> { g.Game.Game1.Value, g.Game.Game2.Value, g.Game.Game3.Value, g.Game.Game4.Value }.Min())) descending
                    //                select new MemberScores { MemberId = g.Member.Number, FirstName = g.Member.FirstName, LastName = g.Member.LastName, Score = g.Game.Game1 + g.Game.Game2 + g.Game.Game3 + g.Game.Game4 - (new List<int> { g.Game.Game1.Value, g.Game.Game2.Value, g.Game.Game3.Value, g.Game.Game4.Value }.Min()), LastPaymentYear = (g.Member.IsLifetimeMember) ? "life " : g.Member.LastPayment.Value.Year.ToString(), Paid = (g.Member.IsLifetimeMember == true || !(g.Member.LastPayment != null && (g.Member.LastPayment.Value <= EntityFunctions.AddYears(DateTime.Now, -1)))) }).ToList();

                    //    }
                    //    else if (!selectedTournament.ThreeOutOf4 && QBSNumber == 0) //overall standings for a regular tournament
                    //    {
                    //        temp = (from g in (db.Participants.Include(b => b.Member)
                    //                                .Include(b => b.Game)
                    //                                .Where(b => b.Tournament.Id == selectedTournament.Id))
                    //                orderby ((g.Game.Game1) + (g.Game.Game2) + (g.Game.Game3) + (g.Game.Game4)) descending
                    //                select new MemberScores { MemberId = g.Member.Number, FirstName = g.Member.FirstName, LastName = g.Member.LastName, Score = (g.Game.Game1) + (g.Game.Game2) + (g.Game.Game3) + (g.Game.Game4), LastPaymentYear = (g.Member.IsLifetimeMember) ? "life " : g.Member.LastPayment.Value.Year.ToString(), Paid = (g.Member.IsLifetimeMember == true || !(g.Member.LastPayment != null && (g.Member.LastPayment.Value <= EntityFunctions.AddYears(DateTime.Now, -1)))) }).ToList();
                    //    }
                    //    else if (!selectedTournament.ThreeOutOf4 && QBSNumber > 0) //standings based on squad for a regular tournament
                    //    {
                    //        temp = (from g in (db.Participants.Include(b => b.Member)
                    //                                .Include(b => b.Game)
                    //                                .Where(b => b.Tournament.Id == selectedTournament.Id).Where(b => b.Squad == QBSNumber))
                    //                orderby (g.Game.Game1 + g.Game.Game2 + g.Game.Game3 + g.Game.Game4) descending
                    //                select new MemberScores { MemberId = g.Member.Number, FirstName = g.Member.FirstName, LastName = g.Member.LastName, Score = g.Game.Game1 + g.Game.Game2 + g.Game.Game3 + g.Game.Game4, LastPaymentYear = (g.Member.IsLifetimeMember) ? "life " : g.Member.LastPayment.Value.Year.ToString(), Paid = (g.Member.IsLifetimeMember == true || !(g.Member.LastPayment != null && (g.Member.LastPayment.Value <= EntityFunctions.AddYears(DateTime.Now, -1)))) }).ToList();
                    //    }
                    //}
                    #endregion

                    //find out what squad is selected At the moment of series button click
                    int currentsNum = 0;
                    if (rdoSquad1Results.Checked)
                        currentsNum = 1;
                    else if (rdoSquad2Results.Checked)
                        currentsNum = 2;
                    else if (rdoSquad3Results.Checked)
                        currentsNum = 3;
                    else if (rdoSquad4Results.Checked)
                        currentsNum = 4;
                    else if (rdoSquad5Results.Checked)
                        currentsNum = 5;
                    else if (rdoSquad6Results.Checked)
                        currentsNum = 6;
                    else if (rdoSquad7Results.Checked)
                        currentsNum = 7;
                    else if (rdoSquad8Results.Checked)
                        currentsNum = 8;





                    temp.Sort(scoreComparer);
                    temp.Reverse();

                    CalculatePlaceStanding(temp);

                    if (temp.Count() != 0)
                    {
                        FrmMemberScoresReports report = new FrmMemberScoresReports(temp, selectedTournament, 2/*reportTypeNum, 0 for High game handicap/senior, 1 for game/high game, 2 for series/high series*/, currentsNum);
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
            Refresh(false, QBSNumber);

        }

        private void rdoSquad1Results_CheckedChanged(object sender, EventArgs e)
        {
            if (cbxTourneyDropDown.Size != null)
            {
                QBSNumber = 1;
                Refresh(false, QBSNumber);
            }
        }

        private void rdoSquad2Results_CheckedChanged(object sender, EventArgs e)
        {
            if (cbxTourneyDropDown.Size != null)
            {
                QBSNumber = 2;
                Refresh(false, QBSNumber);
            }
        }

        private void rdoSquad3Results_CheckedChanged(object sender, EventArgs e)
        {
            if (cbxTourneyDropDown.Size != null)
            {
                QBSNumber = 3;
                Refresh(false, QBSNumber);
            }
        }

        private void rdoSquad4Results_CheckedChanged(object sender, EventArgs e)
        {
            if (cbxTourneyDropDown.Size != null)
            {
                QBSNumber = 4;
                Refresh(false, QBSNumber);
            }

        }

        private void rdoSquad5Results_CheckedChanged(object sender, EventArgs e)
        {
            if (cbxTourneyDropDown.Size != null)
            {
                QBSNumber = 5;
                Refresh(false, QBSNumber);
            }

        }

        private void rdoSquad6Results_CheckedChanged(object sender, EventArgs e)
        {

            if (cbxTourneyDropDown.Size != null)
            {
                QBSNumber = 6;
                Refresh(false, QBSNumber);
            }

        }

        private void rdoSquad7Results_CheckedChanged(object sender, EventArgs e)
        {
            if (cbxTourneyDropDown.Size != null)
            {
                QBSNumber = 7;
                Refresh(false, QBSNumber);
            };
        }

        private void rdoSquad8Resualts_CheckedChanged(object sender, EventArgs e)
        {
            if (cbxTourneyDropDown.Size != null)
            {
                QBSNumber = 8;
                Refresh(false, QBSNumber);
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
                        Refresh(false, QBSNumber);
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
                    Refresh(false, QBSNumber);
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

        private void btnTournamentResults_Click(object sender, EventArgs e)
        {
            FrmTournamentResults form = new FrmTournamentResults();
            form.ShowDialog();
        }

        private void rdoSquadNumber_CheckedChanged(object sender, EventArgs e)
        {
            ScoreAndTotalClear();
            List<Participant> total = TournamentDb.GetTournamentMemberList(GetTournamentById(Convert.ToInt32(cbxTourneyDropDown.SelectedValue)));
            RecordIndexOnSquadSwitch(total);
            FillMember();
        }

		private void txtMemberNum2_Leave(object sender, EventArgs e)
		{
			FillMember();
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
            //if all squads is selected then uncheck and disable squad selections
            if(cbAllSquads.Checked)
            {
                cbFilterSquad1.Checked = false;
                cbFilterSquad2.Checked = false;
                cbFilterSquad3.Checked = false;
                cbFilterSquad4.Checked = false;
                cbFilterSquad5.Checked = false;
                cbFilterSquad6.Checked = false;
                cbFilterSquad7.Checked = false;
                cbFilterSquad8.Checked = false;

                cbFilterSquad1.Enabled = false;
                cbFilterSquad2.Enabled = false;
                cbFilterSquad3.Enabled = false;
                cbFilterSquad4.Enabled = false;
                cbFilterSquad5.Enabled = false;
                cbFilterSquad6.Enabled = false;
                cbFilterSquad7.Enabled = false;
                cbFilterSquad8.Enabled = false;


                howManySquadsCanBeFiltered.Clear();
                QBSNumber = 0;
                Refresh(false, QBSNumber);
               
            }
            else
            {
                cbFilterSquad1.Checked = false;
                cbFilterSquad2.Checked = false;
                cbFilterSquad3.Checked = false;
                cbFilterSquad4.Checked = false;
                cbFilterSquad5.Checked = false;
                cbFilterSquad6.Checked = false;
                cbFilterSquad7.Checked = false;
                cbFilterSquad8.Checked = false;


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
            if(cbFilterSquad1.Checked)
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
            //If all check boxes on filtered are checked then uncheck all and check allSquads
            if(FilterCheck() == selectedTournament.Squads)
            {
                howManySquadsCanBeFiltered.Clear();
                cbAllSquads.Checked = true;
            }
            else if(cbFilterSquad1.Checked == false && howManySquadsCanBeFiltered.Contains(1))
            {
                howManySquadsCanBeFiltered.Remove(1);
                if (FilterCheck() == 0)
                {
                    QBSNumber = 0;
                    cbAllSquads.Checked = true;
                }
                else
                {
                    QBSNumber = 9;
                    Refresh(false, QBSNumber);
                }
                
            }
            else if(cbFilterSquad1.Checked == true && !(howManySquadsCanBeFiltered.Contains(1)))
            {
                howManySquadsCanBeFiltered.Add(1);
                QBSNumber = 9;
                Refresh(false, QBSNumber);
            }
            
        }

        private void cbFilterSquad2_CheckedChanged(object sender, EventArgs e)
        {
            if (FilterCheck() == selectedTournament.Squads)
            {
                howManySquadsCanBeFiltered.Clear();
                cbAllSquads.Checked = true;
            }
            else if (cbFilterSquad2.Checked == false && howManySquadsCanBeFiltered.Contains(2))
            {
                howManySquadsCanBeFiltered.Remove(2);
                if (FilterCheck() == 0)
                {
                    QBSNumber = 0;
                    cbAllSquads.Checked = true;
                }
                else
                {
                    QBSNumber = 9;
                    Refresh(false, QBSNumber);
                }
             
            }
            else
            {
                howManySquadsCanBeFiltered.Add(2);
                QBSNumber = 9;
                Refresh(false, QBSNumber);
            }
        }

        private void cbFilterSquad3_CheckedChanged(object sender, EventArgs e)
        {
            if (FilterCheck() == selectedTournament.Squads)
            {
                howManySquadsCanBeFiltered.Clear();
                cbAllSquads.Checked = true;
            }
            else if (cbFilterSquad3.Checked == false && howManySquadsCanBeFiltered.Contains(3))
            {
                howManySquadsCanBeFiltered.Remove(3);
                if (FilterCheck() == 0)
                {
                    QBSNumber = 0;
                    cbAllSquads.Checked = true;
                }
                else
                {
                    QBSNumber = 9;
                    Refresh(false, QBSNumber);
                }
              
            }
            else
            {
                howManySquadsCanBeFiltered.Add(3);
                QBSNumber = 9;
                Refresh(false, QBSNumber);
            }

        }

        private void cbFilterSquad4_CheckedChanged(object sender, EventArgs e)
        {
            if (FilterCheck() == selectedTournament.Squads)
            {
                howManySquadsCanBeFiltered.Clear();
                cbAllSquads.Checked = true;
            }
            else if (cbFilterSquad4.Checked == false && howManySquadsCanBeFiltered.Contains(4))
            {
                howManySquadsCanBeFiltered.Remove(4);
                if (FilterCheck() == 0)
                {
                    QBSNumber = 0;
                    cbAllSquads.Checked = true;
                }
                else
                {
                    QBSNumber = 9;
                    Refresh(false, QBSNumber);
                }
              
            }
            else
            {
                howManySquadsCanBeFiltered.Add(4);
                QBSNumber = 9;
                Refresh(false, QBSNumber);
            }

        }

        private void cbFilterSquad5_CheckedChanged(object sender, EventArgs e)
        {
            if (FilterCheck() == selectedTournament.Squads)
            {
                howManySquadsCanBeFiltered.Clear();
                cbAllSquads.Checked = true;
            }
            else if (cbFilterSquad5.Checked == false && howManySquadsCanBeFiltered.Contains(5))
            {
                howManySquadsCanBeFiltered.Remove(5);
                if (FilterCheck() == 0)
                {
                    QBSNumber = 0;
                    cbAllSquads.Checked = true;
                }
                else
                {
                    QBSNumber = 9;
                    Refresh(false, QBSNumber);
                }
        
            }
            else
            {
                howManySquadsCanBeFiltered.Add(5);
                QBSNumber = 9;
                Refresh(false, QBSNumber);
            }
        }

        private void cbFilterSquad6_CheckedChanged(object sender, EventArgs e)
        {
            if (FilterCheck() == selectedTournament.Squads)
            {
                howManySquadsCanBeFiltered.Clear();
                cbAllSquads.Checked = true;
            }
            else if (cbFilterSquad6.Checked == false && howManySquadsCanBeFiltered.Contains(6))
            {
                howManySquadsCanBeFiltered.Remove(6);
                if (FilterCheck() == 0)
                {
                    QBSNumber = 0;
                    cbAllSquads.Checked = true;
                }
                else
                {
                    QBSNumber = 9;
                    Refresh(false, QBSNumber);
                }
        
            }
            else
            {
                howManySquadsCanBeFiltered.Add(6);
                QBSNumber = 9;
                Refresh(false, QBSNumber);
            }
        }

        private void cbFilterSquad7_CheckedChanged(object sender, EventArgs e)
        {
            if (FilterCheck() == selectedTournament.Squads)
            {
                howManySquadsCanBeFiltered.Clear();
                cbAllSquads.Checked = true;
            }
            else if (cbFilterSquad7.Checked == false && howManySquadsCanBeFiltered.Contains(7))
            {
                howManySquadsCanBeFiltered.Remove(7);
                if (FilterCheck() == 0)
                {
                    QBSNumber = 0;
                    cbAllSquads.Checked = true;
                }
                else
                {
                    QBSNumber = 9;
                    Refresh(false, QBSNumber);
                }
               
            }
            else
            {
                howManySquadsCanBeFiltered.Add(7);
                QBSNumber = 9;
                Refresh(false, QBSNumber);
            }
        }

        private void cbFilterSquad8_CheckedChanged(object sender, EventArgs e)
        {
            if (FilterCheck() == selectedTournament.Squads)
            {
                howManySquadsCanBeFiltered.Clear();
                cbAllSquads.Checked = true;
            }
            else if (cbFilterSquad8.Checked == false && howManySquadsCanBeFiltered.Contains(8))
            {
                howManySquadsCanBeFiltered.Remove(8);
                if (FilterCheck() == 0)
                {
                    QBSNumber = 0;
                    cbAllSquads.Checked = true;
                }
                else
                {
                    QBSNumber = 9;
                    Refresh(false, QBSNumber);
                }
             
            }
            else
            {
                howManySquadsCanBeFiltered.Add(8);
                QBSNumber = 9;
                Refresh(false, QBSNumber);
            }
        }
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

        public int memberID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Placing { get; set; }
        public int? ScratchTotal { get; set; }
        public int? HandicapScore { get; set; }
        public int? Top3ScratchScore { get; set; }
        public int? Top3HandiScores { get; set; }
        public int? Game1 { get; set; }
        public int? Game2 { get; set; }
        public int? Game3 { get; set; }
        public int? Game4 { get; set; }
        public int? Handicap { get; set; }
        public int Bonus { get; set; }
        public int GameID { get; set; }
        #endregion
        public List<int?> allGameScores()
        {
            var newList = new List<int?>();
            newList.Add(Game1);
            newList.Add(Game2);
            newList.Add(Game3);
            newList.Add(Game4);
            return newList.Where(sc => sc.HasValue).ToList();
        }
    }
}
