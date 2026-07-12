using NineTapTour.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using ClosedXML.Excel;
using NineTapTour.Abstractions;
using NineTapTour.Database;
using NineTapTour.Forms;
using NineTapTour.Models;
using System.Drawing;
using System.Threading.Tasks;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace MemberImportTest;

public partial class FrmMain : Form
{
    private Button btnConvertXls;
    private TextBox txtStatus;

    private readonly IDbContextFactory<NineTapDb> _dbFactory;
    private readonly IMemberRepository _memberRepo;
    private readonly IPlayerHistoryRepository _playerHistoryRepo;

    /// <summary>Designer constructor. Do not use at runtime.</summary>
    public FrmMain()
    {
        InitializeComponent();
        InitializeConvertXlsControls();
    }

    [ActivatorUtilitiesConstructor]
    public FrmMain(IDbContextFactory<NineTapDb> dbFactory, IMemberRepository memberRepo, IPlayerHistoryRepository playerHistoryRepo)
    {
        InitializeComponent();
        _dbFactory = dbFactory;
        _memberRepo = memberRepo;
        _playerHistoryRepo = playerHistoryRepo;
        InitializeConvertXlsControls();
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
    public List<Member> validMembers = [];      // Makes list of valid members

    private static bool TryParsePaidThroughYear(string token, out int paidThroughYear)
    {
        paidThroughYear = 0;
        if (!int.TryParse(token, out int numeric))
            return false;

        if (token.Length == 2)
        {
            paidThroughYear = 2000 + numeric;
            return true;
        }

        if (token.Length == 4 && numeric >= 2000 && numeric <= 2099)
        {
            paidThroughYear = numeric;
            return true;
        }

        return false;
    }

    private static void ParseFirstNameAndMembershipInfo(Member member, string seg)
    {
        if (string.IsNullOrWhiteSpace(seg))
            return;

        string[] tokens = seg.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        string parsedFirstName = string.Empty;

        foreach (string raw in tokens)
        {
            string normalized = Regex.Replace(raw, "[^A-Za-z0-9]", string.Empty);
            if (string.IsNullOrWhiteSpace(normalized))
                continue;

            if (normalized.Equals("life", StringComparison.OrdinalIgnoreCase)
                || normalized.Equals("hof", StringComparison.OrdinalIgnoreCase))
            {
                member.IsLifetimeMember = true;
                member.LastPayment = new DateTime(9999, 12, 31);
                continue;
            }

            if (TryParsePaidThroughYear(normalized, out int paidThroughYear))
            {
                // Paid through YYYY means membership expires at the start of YYYY+1.
                // Store LastPayment as YYYY-1 so existing report logic (+1 display) remains consistent.
                int lastPaymentYear = Math.Max(1753, paidThroughYear - 1);
                if (!member.IsLifetimeMember)
                    member.LastPayment = new DateTime(lastPaymentYear, 12, 31);
                continue;
            }

            if (string.IsNullOrWhiteSpace(parsedFirstName))
                parsedFirstName = normalized;
        }

        if (string.IsNullOrWhiteSpace(parsedFirstName) && tokens.Length > 0)
            parsedFirstName = tokens[0];

        if (!string.IsNullOrWhiteSpace(parsedFirstName))
            member.FirstName = parsedFirstName;
    }

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
                        LastBSpace, YearEndTSpace, MoneyESpace, RejoinDSpace, ReferalSpace, SSSpace, CBSpace, CBSpace, CBSpace, CBSpace, CBSpace, CBSpace, CBSpace, DOBSpace];


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

            var m = new Member();

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
                        ParseFirstNameAndMembershipInfo(m, seg);
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
                        if (int.TryParse(seg, out var avg)) { m.Average = avg; }
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
            if (!_memberRepo.MemberExists(validMembers[j]))
            {
                _memberRepo.AddOrUpdateMember(validMembers[j]);
                persisted++;
            }
        }

        MessageBox.Show($"{persisted} new members added. {validMembers.Count - persisted} were already present.", "Import Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
        btnSelectExcelFolder.Enabled = true;
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
            DialogResult result = fbd.ShowDialog();
            if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
            {
                string[] files = Directory.GetFiles(fbd.SelectedPath);
                List<ExcelRow> participantHistory = GetAllExcelData(files);
            }
        }
        btn_FinalizeData.Enabled = true;
    }

    private List<ExcelRow> GetAllExcelData(string[] files)
    {
        List<ExcelRow> rows = [];
        for (int i = 0; i < files.Length; i++)
        {
            // If the file is not an excel file, skip it
            if (!FileHelper.IsValidExcelExtension(Path.GetExtension(files[i])))
            {
                continue;
            }
            txtProgress.AppendText($"Processing: {Path.GetFileName(files[i])}\r\n");
            // PERFORMANCE FIX: Accumulate results instead of overwriting
            rows.AddRange(ProcessExcelFile(files[i]));
        }
        return rows;
    }

    /// <summary>
    /// This will process the actual excel files and impport the info needed from the files to the program
    /// /// NOTE: This is currently set up for the old format. New format has not yet been implemented.
    /// </summary>
    /// <param name="PathAndFileName"></param>
    /// <returns></returns>
    /// <summary>
    /// Adds (or updates) a participant on an already-open import context so the whole file imports
    /// under a single transaction. Mirrors the former TournamentDB.AddMemberToTournament(player, db).
    /// </summary>
    private static void AddParticipantToContext(NineTapDb db, Participant player)
    {
        bool isMemberInTournament = db.Participants
            .AsNoTracking()
            .Any(p => p.Member.Id == player.Member.Id
                   && p.Tournament.Id == player.Tournament.Id
                   && p.Squad == player.Squad);

        if (!isMemberInTournament)
        {
            player.Id = 0;

            if (db.Entry(player.Member).State == EntityState.Detached)
                db.Attach(player.Member);
            if (db.Entry(player.Tournament).State == EntityState.Detached)
                db.Attach(player.Tournament);
            if (db.Entry(player.Game).State == EntityState.Detached)
                db.Attach(player.Game);

            db.Participants.Add(player);
        }
        else
        {
            Game result = db.Games.SingleOrDefault(g => g.Id == player.Game.Id);
            Participant squadResult = db.Participants.SingleOrDefault(p => p.Id == player.Id);
            Participant memberQuery = db.Participants.Include(m => m.Member)
                .Where(m => m.Member.Id == player.Member.Id).FirstOrDefault();
            result.Game1 = player.Game.Game1;
            result.Game2 = player.Game.Game2;
            result.Game3 = player.Game.Game3;
            result.Game4 = player.Game.Game4;
            result.MoneyWon = player.Game.MoneyWon;
            result.IsComp = player.Game.IsComp;

            if (squadResult == null)
            {
                squadResult = new Participant();
            }
            squadResult.Squad = player.Squad;
            squadResult.Member = memberQuery.Member;
        }
    }

    private List<ExcelRow> ProcessExcelFile(string PathAndFileName)
    {
        txtProgress.AppendText($"Current File Being Processed: {Path.GetFileName(PathAndFileName)}\r\n");

        List<ExcelRow> returnMe = new List<ExcelRow>();
        char[] splitters = new[] { '/', '-' };

        // Create a single DbContext for the entire file import
        using (var db = _dbFactory.CreateDbContext())
        {
            using (var workbook = new XLWorkbook(PathAndFileName))
            {
                // Extract player information ONCE from the first worksheet that has it
                string[] PlayerFinalFirstAndMiddle = new[] { "", "" };
                string playerLastName = "";
                int playerOrgAVG = -1;
                int playerNumberAsInt = 0;

                bool playerInfoExtracted = false;

                // Find and extract player information from first worksheet with data
                foreach (var ws in workbook.Worksheets)
                {
                    if (!playerInfoExtracted)
                    {
                        ExtractPlayerInfoFromWorksheet(ws, ref PlayerFinalFirstAndMiddle, ref playerLastName,
                            ref playerOrgAVG, ref playerNumberAsInt, PathAndFileName, splitters);

                        if (playerNumberAsInt > 0)
                        {
                            playerInfoExtracted = true;
                        }
                    }
                }

                // If we couldn't extract player info, abort
                if (!playerInfoExtracted || playerNumberAsInt <= 0)
                {
                    throw new ArgumentException($"  ERROR: Could not extract valid player information from {Path.GetFileName(PathAndFileName)}\r\n");
                }

                // Load existing tournaments once for the entire workbook
                List<Tournament> existingTournaments = [.. db.Tournaments.OrderByDescending(t => t.Date)];

                // PERFORMANCE: Look up member once per file instead of per row
                var member = db.Members.SingleOrDefault(m => m.Number == playerNumberAsInt) ?? new Member();
                if (member == null || member.IsActive != true)
                {
                    txtProgress.AppendText($"  WARNING: Member #{playerNumberAsInt} not found or inactive. Skipping file.\r\n");
                    return returnMe;
                }

                // Now process each worksheet with the extracted player info
                foreach (var ws in workbook.Worksheets)
                {
                    const int GameDataLastRow = 46;
                    const int GameDataStartRow = 3;

                    // PERFORMANCE: Track participant counts per tournament to avoid repeated DB queries
                    Dictionary<int, int> tournamentSquadCounts = new Dictionary<int, int>();

                    for (int row = GameDataStartRow; row <= GameDataLastRow; row++)
                    {
                        ExcelRow temp = new();

                        // Populate excel row with reused player data
                        temp.PlayerFirstName = PlayerFinalFirstAndMiddle[0];
                        temp.PlayerMiddleName = PlayerFinalFirstAndMiddle[1];
                        temp.PlayerLastName = playerLastName;
                        temp.PlayerOrginalAVG = playerOrgAVG;
                        temp.PlayerNumber = playerNumberAsInt;

                        try { temp.GameTotal = ws.Cell(row, 1).GetValue<int>(); } catch { temp.GameTotal = -1; }
                        try { temp.Date = ws.Cell(row, 2).GetDateTime(); } catch { temp.Date = new DateTime(); }
                        try { temp.Game1 = ws.Cell(row, 3).GetValue<int>(); } catch { temp.Game1 = -1; }
                        try { temp.Game2 = ws.Cell(row, 4).GetValue<int>(); } catch { temp.Game2 = -1; }
                        try { temp.Game3 = ws.Cell(row, 5).GetValue<int>(); } catch { temp.Game3 = -1; }
                        try { temp.Game4 = ws.Cell(row, 6).GetValue<int>(); } catch { temp.Game4 = -1; }
                        try { temp.Total = ws.Cell(row, 7).GetValue<int>(); } catch { temp.Total = -1; }

                        if (temp.GameTotal == -1)
                        {
                            // No game data in this row; skip it
                            continue;
                        }

                        try { temp.AverageOfRow = ws.Cell(row, 8).GetValue<double>(); } catch { temp.AverageOfRow = -1; }
                        try { temp.TrueAverage = ws.Cell(row, 9).GetValue<double>(); } catch { temp.TrueAverage = -1; }
                        try { temp.AVG = ws.Cell(row, 10).GetValue<int>(); } catch { temp.AVG = -1; }
                        try { temp.HandyCap = ws.Cell(row, 11).GetValue<int>(); } catch { temp.HandyCap = -1; }
                        try { temp.Bonus = ws.Cell(row, 12).GetValue<int>(); } catch { temp.Bonus = -1; }
                        temp.FinPPHG = ws.Cell(row, 14).GetString();
                        try { if (!string.IsNullOrEmpty(temp.FinPPHG)) { temp.Cash = ws.Cell(row, 15).GetValue<double>(); } else { temp.Cash = 0; } } catch { temp.Cash = 0; }
                        temp.Notes = ws.Cell(row, 16).GetString();

                        DateTime rowDate = temp.Date.Date;
                        if (rowDate == DateTime.MinValue)
                        {
                            // Invalid date; skip this row
                            continue;
                        }

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
                                Squads = 4,
                                Doubles = false,
                                ThreeOutOf4 = false,
                                IsOnlyThreeGames = false,
                            };

                            db.Entry(tourn).State = db.Tournaments.Any(t => t.Id == tourn.Id)
                                ? EntityState.Modified : EntityState.Added;
                            existingTournaments.Add(tourn);
                        }

                        // There are some cases where an entire entry will be all null games
                        // this is due to tournament conditions such as invalid lane oilings.
                        // The tournament is valid but none of the scores are counted due to inflated numbers.
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
                            IsFinalized = true,
                            UseGame1 = temp.Game1 > -1 ? true : false,
                            UseGame2 = temp.Game2 > -1 ? true : false,
                            UseGame3 = temp.Game3 > -1 ? true : false,
                            UseGame4 = temp.Game4 > -1 ? true : false,

                            AdjustedAvg = temp.AVG,
                            KeepAdjustedAvg = true,
                            LeagueAverage = temp.TrueAverage,
                            HandicapTotal = temp.HandyCap,
                            // Place standing has many variations like (4th, 17th tie, 9thHM, and more)
                            // PlaceStanding = Convert.ToInt32(temp.FinPPHG),
                        };

                        db.Entry(game).State = db.Games.Any(g => g.Id == game.Id)
                            ? EntityState.Modified : EntityState.Added;

                        // Squad numbering is 1-based per player per tournament within this import run.
                        // Always start from 1 for the first entry read, regardless of any existing DB records.
                        int squadNumber;
                        if (!tournamentSquadCounts.ContainsKey(tourn.Id))
                        {
                            tournamentSquadCounts[tourn.Id] = 0;
                        }
                        tournamentSquadCounts[tourn.Id]++;
                        squadNumber = tournamentSquadCounts[tourn.Id];

                        Participant participant = new Participant()
                        {
                            Squad = squadNumber,
                            Member = member,
                            Game = game,
                            Tournament = tourn
                        };

                        AddParticipantToContext(db, participant);

                        returnMe.Add(temp);
                    }
                }

                db.SaveChanges();
                txtProgress.AppendText($"  File complete: {returnMe.Count} records saved.\r\n");
                return returnMe;
            }
        }
    }

    /// <summary>
    /// Extracts player information (name, number, average) from the first worksheet with valid data.
    /// Used once per workbook and reused for all sheets.
    /// </summary>
    private void ExtractPlayerInfoFromWorksheet(IXLWorksheet ws, ref string[] PlayerFinalFirstAndMiddle,
        ref string playerLastName, ref int playerOrgAVG, ref int playerNumberAsInt,
        string PathAndFileName, char[] splitters)
    {
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
            playerNumberAsInt = 0;
            return;
        }

        playerNumber = RegexHelpers.StripNonNumericRegex().Replace(playerNumber, string.Empty);

        string[] playerNumberAfterSplit;
        int.TryParse(playerNumber, out playerNumberAsInt);
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
    }

    private void Btn_FinalizeData_Click(object sender, EventArgs e)
    {
        Cursor.Current = Cursors.WaitCursor;

        IncrementFinalizeBar(33, "Step 3: Setting averages and bonus pins from history.");

        // Update validMembers in memory with latest history values
        for (int i = 0; i < validMembers.Count; i++)
        {
            List<PlayerHistoryViewModel> list = _playerHistoryRepo.GetLastFiveTournaments(validMembers[i].Number);
            if (list.Count > 0)
            {
                validMembers[i].Average = list[0].AVG; // set new avg to last bowled adjusted avg
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

    private void IncrementFinalizeBar(int increment, string msg)
    {
        progressBarFinalize.Increment(increment);
        lblFinalizeStatus.Text = msg;
        progressBarFinalize.Refresh();
        lblFinalizeStatus.Refresh();
    }

    private void UpdateMembers(List<Member> members)
    {
        for (int i = 0; i < members.Count; i++)
        {
            // Use AddOrUpdate to ensure existing members get their averages/bonus updated
            _memberRepo.AddOrUpdateMember(members[i]);
        }
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

    private void FrmMain_Load(object sender, EventArgs e)
    {
        Text = "Version: 3.1.11";
#if DEBUG
        Text += " DEVELOPMENT ONLY";
#endif
    }
}