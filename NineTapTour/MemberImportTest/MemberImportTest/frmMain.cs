using NineTapTour.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using NineTapTour.Core.Import;
using NineTapTour.Core.Repositories;
using NineTapTour.Core.Entities;
using System.Threading.Tasks;
using System.Linq;
using System.Text.RegularExpressions;

namespace MemberImportTest;

public partial class FrmMain : Form
{
    private Button btnConvertXls;
    private TextBox txtStatus;

    private readonly IMemberRepository memberRepository;
    private readonly IPlayerHistoryRepository playerHistoryRepository;
    private readonly IMemberHistoryImportService memberHistoryImportService;

    public FrmMain(
        IMemberRepository memberRepository,
        IPlayerHistoryRepository playerHistoryRepository,
        IMemberHistoryImportService memberHistoryImportService)
    {
        this.memberRepository = memberRepository;
        this.playerHistoryRepository = playerHistoryRepository;
        this.memberHistoryImportService = memberHistoryImportService;

        InitializeComponent();
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
            if (!memberRepository.MemberExists(validMembers[j]))
            {
                memberRepository.AddOrUpdateMember(validMembers[j]);
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
                memberHistoryImportService.ImportFolder(fbd.SelectedPath, new TextBoxProgress(txtProgress));
            }
        }
        btn_FinalizeData.Enabled = true;
    }

    /// <summary>
    /// Reports import progress synchronously into a multiline text box so the
    /// text appears in the same order the service produces it.
    /// </summary>
    private sealed class TextBoxProgress : IProgress<string>
    {
        private readonly TextBox target;

        public TextBoxProgress(TextBox target)
        {
            this.target = target;
        }

        public void Report(string value)
        {
            target.AppendText(value);
        }
    }

    private void Btn_FinalizeData_Click(object sender, EventArgs e)
    {
        Cursor.Current = Cursors.WaitCursor;

        IncrementFinalizeBar(33, "Step 3: Setting averages and bonus pins from history.");

        // Update validMembers in memory with latest history values
        for (int i = 0; i < validMembers.Count; i++)
        {
            List<PlayerHistoryViewModel> list = playerHistoryRepository.GetLastFiveTournaments(validMembers[i].Number);
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
            memberRepository.AddOrUpdateMember(members[i]);
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
        Text = "Version: 3.2.0";
#if DEBUG
        Text += " DEVELOPMENT ONLY";
#endif
    }
}
