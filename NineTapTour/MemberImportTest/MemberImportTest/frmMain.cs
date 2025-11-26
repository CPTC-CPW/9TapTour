using MemberImportTest.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using ClosedXML.Excel;
using NineTapTour.Database;
using System.Text.RegularExpressions;
using System.Globalization;
using NineTapTour.Forms;
using NineTapTour.Models;
using System.Drawing;
using System.Threading.Tasks;
using System.Linq;

namespace MemberImportTest;

public partial class FrmMain : Form
{
    private Button btnConvertXls;
    private TextBox txtStatus;

    public FrmMain()
    {
        InitializeComponent();
        InitializeConvertXlsControls();
        List<NineTapRegion> r = NineTapRegionDB.GetRegionList();

        // Create a default local region if there are no regions in the database
        if (r.Count == 0)
        {
            NineTapRegion defaultRegion = new()
            {
                NineTapRegionName = "Local",
            };

            NineTapRegionDB.AddRegion(defaultRegion);
            r.Add(defaultRegion);
        }
        cbxRegionSelect.DataSource = r;
        cbxRegionSelect.DisplayMember = nameof(NineTapRegion.NineTapRegionName);
        RegionID = r[cbxRegionSelect.SelectedIndex].NineTapRegionID;
    }

    private void InitializeConvertXlsControls()
    {
        // Button
        btnConvertXls = new Button();
        btnConvertXls.Text = "Convert .xls to .xlsx (only need to do this once)";
        btnConvertXls.Width = 150;
        btnConvertXls.Height = 60;
        btnConvertXls.Top = 75;
        btnConvertXls.Left = 10;
        btnConvertXls.Click += btnConvertXls_Click;
        this.Controls.Add(btnConvertXls);

        // TextBox
        txtStatus = new TextBox();
        txtStatus.Multiline = true;
        txtStatus.ReadOnly = true;
        txtStatus.ScrollBars = ScrollBars.Vertical;
        txtStatus.Width = 500;
        txtStatus.Height = 200;
        txtStatus.Top = btnConvertXls.Bottom + 10;
        txtStatus.Left = 10;
        this.Controls.Add(txtStatus);
    }
    
    public int RegionID;
    public int allGames;

    public List<Member> validMembers = [];      // Makes list of valid members
    public List<PlayerHistoryViewModel> PlayerHistoryList = [];

    private static readonly List<ExcelRow> ALLEXCELDATAFROMALLPLAYERS = [];

    /// <summary>
    /// When the user clicks on the open button file it will open a file selection window
    /// allowing the user to select the file they wish to choose for importation
    /// </summary>

    private void BtnOpenFile_Click(object sender, EventArgs e)
    {
        #region Member Info Static Ints
        const int MemNumSpace = 6;     // Member Number
        const int DJoinedSpace = 8;    // Date Joined
        const int LNameSpace = 20;     // Last Name
        const int FNameSpace = 20;     // First Name
        const int MISpace = 2;         // Middle Initial
        const int EPhoneSpace = 15;    // Evening Phone
        const int DPhoneSpace = 15;    // Day Phone
        const int CPhoneSpace = 15;    // Cell Phone
        const int StreetSpace = 40;    // Street Address
        const int EmailSpace = 40;     // Email Address
        const int CitySpace = 20;      // City
        const int StateSpace = 2;      // State
        const int ZipSpace = 10;       // Zip
        const int NotesSpace = 200;    // Notes
        const int AVGSpace = 3;        // Average
        const int HCSpace = 2;         // Handicap
        const int BSpace = 2;          // Bonus
        const int LastBSpace = 8;      // Last Bowled
        const int YearEndTSpace = 2;   // Year End Tournaments
        const int MoneyESpace = 10;    // Money Earned
        const int RejoinDSpace = 8;    // Rejoin Date;
        const int ReferalSpace = 2;    // ReferalSpace;
        const int SSSpace = 11;        // Social Security
        const int CBSpace = 5;         // Check Box Spaceing, there are 7 total, only 5 are actually checked for information, repeated 7 times in Spaces array.
        const int DOBSpace = 8;        // Date Of Birth.
        #endregion

        int[] Spaces = [ MemNumSpace, DJoinedSpace, LNameSpace, FNameSpace, MISpace, EPhoneSpace, DPhoneSpace, CPhoneSpace,
                        StreetSpace, EmailSpace, CitySpace, StateSpace, ZipSpace, NotesSpace, AVGSpace, HCSpace, BSpace,
                        LastBSpace, YearEndTSpace, MoneyESpace, RejoinDSpace, ReferalSpace, SSSpace, CBSpace, CBSpace, CBSpace, CBSpace, CBSpace, CBSpace, DOBSpace];


        // Configure OpenFileDialog
        ofdOpen.Filter = "Data Files (*.dat)|*.dat|Text Files (*.txt)|*.txt";
        ofdOpen.Title = "Please Select a member file to open";
        if (ofdOpen.ShowDialog() != DialogResult.OK)
            return;

        // Read full file into memory (legacy format; file sizes here are expected to be small)
        string file;
        try
        {
            file = System.IO.File.ReadAllText(ofdOpen.FileName);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to read file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        // Prepare parsing
        int currentIndex = 0;
        int memberCount = 1;
        int addedMembers = 0;
        int recordLength = Spaces.Sum();

        // Defensive: if Spaces is invalid, abort
        if (recordLength <= 0)
        {
            MessageBox.Show("Import configuration is invalid (record length <= 0).", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        // Parse records by searching for the next member marker (memberCount)
        while (true)
        {
            string marker = memberCount.ToString();
            int idx = file.IndexOf(marker, currentIndex, StringComparison.Ordinal);

            // No more records
            if (idx == -1)
                break;

            // Ensure enough characters remain; if not, stop
            if (idx + recordLength > file.Length)
                break;

            var m = new Member { NineTapRegionID = RegionID };

            int pos = idx;
            for (int s = 0; s < Spaces.Length; s++)
            {
                int len = Spaces[s];

                // If the segment would run past EOF, clamp to available chars
                if (pos + len > file.Length)
                    len = Math.Max(0, file.Length - pos);

                string seg = len > 0 ? file.Substring(pos, len).Trim() : string.Empty;
                pos += len;

                // Map fixed-width segment to Member fields (trimmed)
                switch (s)
                {
                    case 0: // Member Number
                        if (int.TryParse(seg, out var num)) m.Number = num;
                        break;
                    case 1: // Date Joined
                        if (DateTime.TryParse(seg, out var jd)) m.JoinDate = jd;
                        break;
                    case 2: // Last Name
                        if (!string.IsNullOrWhiteSpace(seg)) m.LastName = seg;
                        break;
                    case 3: // First Name
                        if (!string.IsNullOrWhiteSpace(seg))
                        {
                            var split = seg.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                            if (split.Length > 0) m.FirstName = split[0];
                        }
                        break;
                    case 4: // Middle Initial
                        if (!string.IsNullOrWhiteSpace(seg)) m.MiddleInitial = seg;
                        break;
                    case 5: // Primary Phone
                        if (!string.IsNullOrWhiteSpace(seg)) m.PrimaryPhone = seg;
                        break;
                    case 7: // Cell Phone
                        if (!string.IsNullOrWhiteSpace(seg)) m.SecondaryPhone = seg;
                        break;
                    case 8: // Street
                        if (!string.IsNullOrWhiteSpace(seg)) m.Street = seg;
                        break;
                    case 9: // Email
                        if (!string.IsNullOrWhiteSpace(seg)) m.Email = seg;
                        break;
                    case 10: // City
                        if (!string.IsNullOrWhiteSpace(seg)) m.City = seg;
                        break;
                    case 11: // State
                        if (!string.IsNullOrWhiteSpace(seg)) m.State = seg;
                        break;
                    case 12: // Zip
                        if (!string.IsNullOrWhiteSpace(seg)) m.PostalCode = seg;
                        break;
                    case 13: // Notes
                        if (!string.IsNullOrWhiteSpace(seg)) m.Notes = seg;
                        break;
                    case 14: // Average
                        if (int.TryParse(seg, out var avg)) { m.StartAvg = avg; m.Average = avg; }
                        break;
                    case 15: // Handicap
                        if (int.TryParse(seg, out var hc)) m.Handicap = hc;
                        break;
                    case 16: // Bonus
                        if (int.TryParse(seg, out var b)) m.Bonus = b;
                        break;
                    case 17: // Last Bowled
                        if (DateTime.TryParse(seg, out var lb)) m.LastBowled = lb;
                        break;
                    case 19: // Money Earned
                        if (decimal.TryParse(seg, out var me)) m.MoneyEarned = me;
                        break;
                    case 20: // Rejoin Date
                        if (DateTime.TryParse(seg, out var rj)) m.RejoinDate = rj;
                        break;
                    case 21: // Referrals
                        if (short.TryParse(seg, out var rf)) m.Referrals = rf;
                        break;
                    case 22: // SSN
                        if (!string.IsNullOrWhiteSpace(seg)) m.SSN = seg;
                        break;
                    case 24: // Active flag
                        if (int.TryParse(seg, out var act) && act == 1) m.IsActive = true;
                        break;
                    case 26: // Inactive flag
                        if (int.TryParse(seg, out var inact) && inact == 1) m.IsActive = false;
                        break;
                    case 27: // Senior
                        if (int.TryParse(seg, out var sr) && sr == 1) m.IsSenior = true;
                        break;
                    case 28: // Female
                        if (int.TryParse(seg, out var gf) && gf == 1) m.Gender = MemberGenders.Female;
                        break;
                    case 29: // Male
                        if (int.TryParse(seg, out var gm) && gm == 1) m.Gender = MemberGenders.Male;
                        break;
                    case 30: // Birth Date
                        if (DateTime.TryParse(seg, out var dob))
                        {
                            // If date appears to be in the future, adjust by -100 years (legacy data quirk)
                            if (dob > DateTime.Today) dob = dob.AddYears(-100);
                            m.DateOfBirth = dob;
                        }
                        break;
                    default:
                        // intentionally ignore other indices
                        break;
                }
        } // end segments loop

        // Basic validation: require numeric member number and last name
        if (m.Number > 0 && !string.IsNullOrWhiteSpace(m.LastName))
        {
            // Normalize dates within SQL safe range
            if (m.DateOfBirth.HasValue && m.DateOfBirth.Value < new DateTime(1753, 1, 1))
                m.DateOfBirth = new DateTime(1753, 1, 1);

            if (m.JoinDate < new DateTime(1753, 1, 1))
                m.JoinDate = new DateTime(1753, 1, 1);

            validMembers.Add(m);
            addedMembers++;
        }

        // Advance to next candidate record
        memberCount++;
        currentIndex = idx + recordLength;
    } // end while

    // Persist parsed members (skip ones that already exist)
    int persisted = 0;
    for (int j = 0; j < validMembers.Count; j++)
    {
        if (!NineTapTour.Database.MemberDB.MemberExists(validMembers[j]))
        {
            NineTapTour.Database.MemberDB.AddOrUpdateMember(validMembers[j]);
            persisted++;
        }
    }

    MessageBox.Show($"{persisted} new members added. {validMembers.Count - persisted} were already present.", "Import Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
    CheckSpaces();
}

    /// <summary>
    /// Checks that has members (valid or not) and allows the btnSelectExcel to be enabled
    /// Checks that has data from excel files then when does allows the finalze data button to become enabled.
    /// </summary>
    private void CheckSpaces()
    {
        if (validMembers.Count > 0)
        {
            btnSelectExcelFolder.Enabled = true;
        }
        if (ALLEXCELDATAFROMALLPLAYERS.Count > 0)
        {
            btn_FinalizeData.Enabled = true;
        }
    }

    /// <summary>
    /// Verifies if you would like to get more files form a folder to import.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Button1_Click(object sender, EventArgs e)
    {
        DialogResult proceed = MessageBox.Show("Are you ready to import your Excel files? We will only convert the newer .xlsx files, not the older .xls files.\r\n" +
            "If you have not converted yet, please do so using the button on the left side", "Import Excel Files", MessageBoxButtons.YesNo);
        if (proceed == DialogResult.No)
        {
            return;
        }

        txtProgress.Clear();
        GetAndProcessFolderWithExcelFiles();
        while (MessageBox.Show("Do You have more Excel Files to import?", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
        {
            GetAndProcessFolderWithExcelFiles();
        }
        // Show completion
        txtProgress.AppendText("Complete\r\n");
    }

    /// <summary>
    /// This will open the explorer to find all the excel files in the folder to allow user to choose the file they want to import
    /// </summary>
    private void GetAndProcessFolderWithExcelFiles()
    {
        using (var fbd = new FolderBrowserDialog())
        {
            allGames = PlayerHistoryDB.GetNumberOfAllGames();
            DialogResult result = fbd.ShowDialog();
            if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
            {
                string[] files = Directory.GetFiles(fbd.SelectedPath);
                List<ExcelRow> participantHistory = GetAllExcelData(files);
            }
        }
        CheckSpaces();
    }

    private List<ExcelRow> GetAllExcelData(string[] files)
    {
        for (int i = 0; i < files.Length; i++)
        {
            // If the file is not an excel file, skip it
            if (!FileHelper.IsValidExcelExtension(Path.GetExtension(files[i])))
            {
                continue;
            }
            txtProgress.AppendText($"Processing: {Path.GetFileName(files[i])}\r\n");
            List<ExcelRow> rows = ProcessExcelFile(files[i]);
            foreach (ExcelRow r in rows)
            {
                ALLEXCELDATAFROMALLPLAYERS.Add(r);
            }
        }
        txtProgress.AppendText("Complete\r\n");
        return ALLEXCELDATAFROMALLPLAYERS;
    }

    /// <summary>
    /// This will process the actual excel files and impport the info needed from the files to the program
    /// /// NOTE: This is currently set up for the old format. New format has not yet been implemented.
    /// </summary>
    /// <param name="PathAndFileName"></param>
    /// <returns></returns>
    private List<ExcelRow> ProcessExcelFile(string PathAndFileName)
    {
        txtProgress.AppendText($"Current File Being Processed: {Path.GetFileName(PathAndFileName)}\r\n");

        List<ExcelRow> returnMe = new List<ExcelRow>();
        char[] splitters = new[] { '/', '-' };

        using (var workbook = new XLWorkbook(PathAndFileName))
        {
            // Iterate all worksheets in the workbook
            foreach (var ws in workbook.Worksheets)
            {
                txtProgress.AppendText($" Processing Worksheet: {ws.Name}\r\n");

                string[] PlayerFinalFirstAndMiddle = new[] { "", "" };
                string playerLastName = "";
                string firstAndMiddle = "";

                // Parse header for player name
                string playerFullName = ws.Cell(1, 2).GetString();
                if (!string.IsNullOrWhiteSpace(playerFullName))
                {
                    if (playerFullName.Contains(','))
                    {
                        playerLastName = playerFullName[..playerFullName.IndexOf(',')];
                        firstAndMiddle = playerFullName[(playerFullName.IndexOf(',') + 2)..];
                    }
                    else if (playerFullName.Contains('.'))
                    {
                        playerLastName = playerFullName[..playerFullName.IndexOf('.')];
                        try
                        {
                            firstAndMiddle = playerFullName[(playerFullName.IndexOf('.') + 2)..];
                        }
                        catch (ArgumentOutOfRangeException)
                        {
                            int firstSpaceIndex = playerFullName.IndexOf(' ');
                            firstAndMiddle = firstSpaceIndex > -1 ? playerFullName[..firstSpaceIndex] : playerFullName;
                        }
                    }
                }

                string[] first0middle1 = firstAndMiddle.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < Math.Min(first0middle1.Length, PlayerFinalFirstAndMiddle.Length); i++)
                {
                    PlayerFinalFirstAndMiddle[i] = first0middle1[i];
                }

                int playerOrgAVG;
                try
                {
                    playerOrgAVG = ws.Cell(1, 10).GetValue<int>();
                }
                catch (Exception)
                {
                    string orgString = ws.Cell(1, 10).GetString();
                    string[] afterSplit = orgString.Split('-', '*', 'L');
                    if (afterSplit.Length > 0 && int.TryParse(afterSplit[0], out int val))
                        playerOrgAVG = val;
                    else
                        playerOrgAVG = -1;
                }

                string playerNumber = ws.Cell(1, 14).GetString();
                if (playerNumber == null)
                {
                    MessageBox.Show($"Player number could not be read in excel file {PathAndFileName}. Program is unable to continue.");
                    throw new ArgumentException($"While reading {PathAndFileName} a player number was not found in the file.");
                }

                // Some regions have letters in their player numbers, so strip non-numeric characters
                playerNumber = RegexHelpers.StripNonNumericRegex().Replace(playerNumber, string.Empty);

                string[] playerNumberAfterSplit;
                int.TryParse(playerNumber, out int playerNumberAsInt);
                if (playerNumberAsInt != 0)
                {
                    playerNumberAsInt = Convert.ToInt32(RegexHelpers.StripNonNumericRegex().Replace(playerNumber, string.Empty));
                }
                else if (playerNumberAsInt == 0)
                {
                    for (int i = 0; i < splitters.Length; i++)
                    {
                        try
                        {
                            playerNumberAfterSplit = playerNumber.Split(splitters[i]);
                            playerNumberAsInt = Convert.ToInt32(RegexHelpers.StripNonNumericRegex().Replace(playerNumberAfterSplit[^1], string.Empty));
                        }
                        catch { }
                    }
                }

                int lastRow = ws.LastRowUsed().RowNumber();
                const int GameDataStartRow = 3;

                // Load existing tournaments for this region to reuse by date
                List<Tournament> existingTournaments = TournamentDB.GetTournamentList(RegionID);

                for (int row = GameDataStartRow; row <= lastRow; row++)
                {
                    ExcelRow temp = new ExcelRow();

                    string game1 = ws.Cell(row, 3).GetString();
                    string game2 = ws.Cell(row, 4).GetString();
                    string game3 = ws.Cell(row, 5).GetString();
                    string game4 = ws.Cell(row, 6).GetString();
                    string testFin = ws.Cell(row, 14).GetString();

                    if (!string.IsNullOrWhiteSpace(ws.Cell(row, 1).GetString()))
                    {
                        if (ws.Cell(row, 1).GetValue<int>() == 0 && string.IsNullOrWhiteSpace(ws.Cell(row, 15).GetString()))
                        {
                            continue;
                        }
                    }

                    if (string.IsNullOrWhiteSpace(ws.Cell(row, 2).GetString()) && string.IsNullOrWhiteSpace(ws.Cell(row, 15).GetString()))
                    {
                        continue;
                    }

                    if (string.IsNullOrWhiteSpace(game1) && string.IsNullOrWhiteSpace(game2) && string.IsNullOrWhiteSpace(game3) && string.IsNullOrWhiteSpace(game4) && !string.IsNullOrWhiteSpace(testFin))
                    {
                        continue;
                    }

                    // Populate excel row and game data
                    temp.PlayerFirstName = PlayerFinalFirstAndMiddle[0];
                    temp.PlayerMiddleName = PlayerFinalFirstAndMiddle[1];
                    temp.PlayerLastName = playerLastName;
                    temp.PlayerOrginalAVG = playerOrgAVG;
                    temp.PlayerNumber = playerNumberAsInt;

                    var member = MemberDB.GetMember(temp.PlayerNumber, RegionID);
                    if (member == null || member.IsActive != true)
                        continue;

                    PlayerHistoryViewModel playerH = new PlayerHistoryViewModel();

                    try { temp.GameTotal = ws.Cell(row, 1).GetValue<int>(); playerH.GamesPlayed = temp.GameTotal; } catch { temp.GameTotal = -1; }
                    try { temp.Date = ws.Cell(row, 2).GetDateTime(); playerH.TournamentDate = temp.Date; } catch { temp.Date = new DateTime(); }
                    try { temp.Game1 = ws.Cell(row, 3).GetValue<int>(); } catch { temp.Game1 = -1; }
                    try { temp.Game2 = ws.Cell(row, 4).GetValue<int>(); } catch { temp.Game2 = -1; }
                    try { temp.Game3 = ws.Cell(row, 5).GetValue<int>(); } catch { temp.Game3 = -1; }
                    try { temp.Game4 = ws.Cell(row, 6).GetValue<int>(); } catch { temp.Game4 = -1; }
                    try { temp.Total = ws.Cell(row, 7).GetValue<int>(); } catch { temp.Total = -1; }
                    try { temp.AverageOfRow = ws.Cell(row, 8).GetValue<double>(); } catch { temp.AverageOfRow = -1; }
                    try { temp.TrueAverage = ws.Cell(row, 9).GetValue<double>(); } catch { temp.TrueAverage = -1; }
                    try { temp.AVG = ws.Cell(row, 10).GetValue<int>(); } catch { temp.AVG = -1; }
                    try { temp.HandyCap = ws.Cell(row, 11).GetValue<int>(); } catch { temp.HandyCap = -1; }
                    try { temp.Bonus = ws.Cell(row, 12).GetValue<int>(); } catch { temp.Bonus = -1; }
                    temp.PotPro = ws.Cell(row, 13).GetString();
                    temp.FinPPHG = ws.Cell(row, 14).GetString();
                    try { if (!string.IsNullOrEmpty(temp.FinPPHG)) { temp.Cash = ws.Cell(row, 15).GetValue<double>(); } else { temp.Cash = 0; } } catch { temp.Cash = 0; }
                    temp.Notes = ws.Cell(row, 16).GetString();

                    // Determine tournament for this row by date; create if it doesn't exist
                    DateTime rowDate = temp.Date != default(DateTime) ? temp.Date.Date : DateTime.Now.Date;
                    Tournament tourn = existingTournaments.FirstOrDefault(t => t.Date.Date == rowDate);
                    if (tourn == null)
                    {
                        tourn = new Tournament()
                        {
                            Date = rowDate,
                            Location = "Imported",
                            Event = $"Imported Tourney - {rowDate}",
                            Notes = string.Empty,
                            Sponsors = string.Empty,
                            Squads = 1,
                            Doubles = false,
                            ThreeOutOf4 = false,
                            IsOnlyThreeGames = false,
                        };

                        // Ensure region is set for new or existing tournament records
                        using (var db = new NineTapDb())
                        {
                            tourn.TourneyRegion = db.NineTapRegion.Find(RegionID);
                        }

                        TournamentDB.AddTournament(tourn);
                        existingTournaments.Add(tourn);
                    }

                    // Ensure region is set for new or existing tournament records
                    using (var db = new NineTapDb())
                    {
                        tourn.TourneyRegion = db.NineTapRegion.Find(RegionID);
                    }

                    // Build Game entity
                    Game game = new Game()
                    {
                        Game1 = temp.Game1 > -1 ? temp.Game1 : null,
                        Game2 = temp.Game2 > -1 ? temp.Game2 : null,
                        Game3 = temp.Game3 > -1 ? temp.Game3 : null,
                        Game4 = temp.Game4 > -1 ? temp.Game4 : null,
                        TotalScore = temp.Total > -1 ? temp.Total : null,
                        Handicap = temp.HandyCap > -1 ? temp.HandyCap : 0,
                        Bonus = temp.Bonus > -1 ? temp.Bonus : 0,
                        MoneyWon = Convert.ToDecimal(temp.Cash),
                        Notes = temp.Notes,
                        IsComp = !string.IsNullOrWhiteSpace(temp.FinPPHG),
                        // Mark imported games as finalized
                        IsFinalized = true,
                        // Ensure use flags are set only when a game value exists
                        UseGame1 = temp.Game1 > -1 ? true : false,
                        UseGame2 = temp.Game2 > -1 ? true : false,
                        UseGame3 = temp.Game3 > -1 ? true : false,
                        UseGame4 = temp.Game4 > -1 ? true : false
                    };

                    // Persist game to database to get Id
                    GameDB.AddOrUpdateGame(game);

                    // Create participant linking member, game and tournament
                    Participant participant = new Participant()
                    {
                        Squad = 1,
                        Member = member,
                        Game = game,
                        Tournament = tourn
                    };

                    // Persist participant (will avoid duplicates)
                    TournamentDB.AddMemberToTournament(participant);

                    // Track progress and returned rows
                    allGames++;
                    PlayerHistoryList.Add(playerH);
                    returnMe.Add(temp);
                }
            }

            return returnMe;
        }
    }

    private void Btn_FinalizeData_Click(object sender, EventArgs e)
    {
        Cursor.Current = Cursors.WaitCursor;

        IncrementFinalizeBar(33, "Step 3: Setting averages and bonus pins from history.");

        // Update validMembers in memory with latest history values
        for (int i = 0; i < validMembers.Count; i++)
        {
            List<PlayerHistoryViewModel> list = PlayerHistoryDB.GetLastFiveTournaments(validMembers[i].Number, RegionID);
            if (list.Count > 0)
            {
                validMembers[i].StartAvg = list[0].AVG; // set new avg to last bowled adjusted avg
                validMembers[i].Average = Convert.ToInt32(list[0].trueAVG); // last 30 game avg
                validMembers[i].Bonus = list[0].Bonus; // last adjusted bonus pin
            }
        }

        // Persist member updates to the DB (automatic)
        IncrementFinalizeBar(33, "Saving member averages and bonus pins to database...");
        UpdateMembers(validMembers);
        IncrementFinalizeBar(34, "Members updated");

        Cursor.Current = Cursors.Default;
        MessageBox.Show($"Import complete. {validMembers.Count} members processed; games and participants were added to the database.", "Import Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
        this.Close();
    }
    /// <summary>
    /// progress bar code the status of completion
    /// </summary>
    /// <param name="increment"></param>
    /// <param name="msg"></param>
    private void IncrementFinalizeBar(int increment, string msg)
    {
        progressBarFinalize.Increment(increment);
        lblFinalizeStatus.Text = msg;
        progressBarFinalize.Refresh();
        lblFinalizeStatus.Refresh();
    }

    /// <summary>
    /// Checks members list if member does not exist it updates the list with adding or updating member
    /// </summary>
    /// <param name="members"></param>
    private static void UpdateMembers(List<Member> members)
    {
        for (int i = 0; i < members.Count; i++)
        {
            // Use AddOrUpdate to ensure existing members get their averages/bonus updated
            MemberDB.AddOrUpdateMember(members[i]);
        }
    }

    /// <summary>
    /// Allows user to change region for where they would like to import the member data to.
    /// </summary>
    private void CbxRegionSelect_SelectedIndexChanged(object sender, EventArgs e)
    {
        List<NineTapRegion> r = NineTapRegionDB.GetRegionList();
        RegionID = r[cbxRegionSelect.SelectedIndex].NineTapRegionID;
    }

    private void FrmMain_Paint(object sender, PaintEventArgs e)
    {
        Graphics g = e.Graphics;
        Font drawFont = new("Arial", 12);
        SolidBrush drawBrush = new(Color.Black);
        PointF drawPoint = new(20, 2);
        g.DrawString("Version: 2.5.2", drawFont, drawBrush, drawPoint);
#if DEBUG
        drawBrush.Color = Color.Red;
        drawPoint.Y += 16;
        g.DrawString("DEVELOPMENT VERSION NOT FOR PRODUCTION", drawFont, drawBrush, drawPoint);
#endif
    }

    private async void btnConvertXls_Click(object sender, EventArgs e)
    {
        DialogResult accept = MessageBox.Show("Do you want to convert your old .xls files into the newer .xlsx format? This will create a copy and your original files will not be deleted. You only need to do this one time." +
            "The computer you run this on must have Excel installed on it.", "Convert to new Excel format", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

        if (accept == DialogResult.No)
        {
            return;
        }

        using var fbd = new FolderBrowserDialog();
        fbd.Description = "Select the folder containing .xls files to convert. This will create a copy in the .xlsx format. Your old files will not be deleted. You only need to do this one time";
        if (fbd.ShowDialog() == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
        {
            txtStatus.Clear();
            btnConvertXls.Enabled = false;
            string folderPath = fbd.SelectedPath;
            await Task.Run(() => RunPowerShellScript(folderPath));
            btnConvertXls.Enabled = true;
        }
    }

    private void RunPowerShellScript(string folderPath)
    {
        var psi = new System.Diagnostics.ProcessStartInfo
        {
            FileName = "powershell.exe",
            Arguments = $"-ExecutionPolicy Bypass -File \"Convert-XlsToXlsx.ps1\" -folder \"{folderPath}\"",
            WorkingDirectory = Application.StartupPath,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using (var process = System.Diagnostics.Process.Start(psi))
        {
            while (!process.StandardOutput.EndOfStream)
            {
                string line = process.StandardOutput.ReadLine();
                this.Invoke(new Action(() => txtStatus.AppendText(line + Environment.NewLine)));
            }
            while (!process.StandardError.EndOfStream)
            {
                string line = process.StandardError.ReadLine();
                this.Invoke(new Action(() => txtStatus.AppendText("ERROR: " + line + Environment.NewLine)));
            }
            process.WaitForExit();
        }
    }
}