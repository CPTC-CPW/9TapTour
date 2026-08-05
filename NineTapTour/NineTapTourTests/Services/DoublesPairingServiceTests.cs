using ClosedXML.Excel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NineTapTour.Core.Entities;
using NineTapTour.Core.Models;
using NineTapTour.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NineTapTourTests.Services
{
    /// <summary>
    /// Characterization tests for the squad sheet-name parsing extracted from
    /// FrmDoublesTeamPairing (M7.4). Expectations were hand-computed from the
    /// original form code and must not change.
    /// </summary>
    [TestClass]
    public class DoublesPairingSheetNameTests
    {
        [TestMethod]
        public void TryParseSquadSheetName_SimpleSquadName_ParsesNumber()
        {
            Assert.IsTrue(DoublesPairingService.TryParseSquadSheetName("Squad 3", out int squad));
            Assert.AreEqual(3, squad);
        }

        [TestMethod]
        public void TryParseSquadSheetName_IsCaseInsensitive()
        {
            Assert.IsTrue(DoublesPairingService.TryParseSquadSheetName("squad 7", out int squad));
            Assert.AreEqual(7, squad);
        }

        [TestMethod]
        public void TryParseSquadSheetName_TrimsOuterAndInnerWhitespace()
        {
            Assert.IsTrue(DoublesPairingService.TryParseSquadSheetName("  Squad 2  ", out int squad));
            Assert.AreEqual(2, squad);

            Assert.IsTrue(DoublesPairingService.TryParseSquadSheetName("Squad  10", out int twoDigit));
            Assert.AreEqual(10, twoDigit);
        }

        [TestMethod]
        public void TryParseSquadSheetName_RejectsNonSquadNames()
        {
            Assert.IsFalse(DoublesPairingService.TryParseSquadSheetName("Roster", out _));
            Assert.IsFalse(DoublesPairingService.TryParseSquadSheetName("Squad", out _));
            Assert.IsFalse(DoublesPairingService.TryParseSquadSheetName("Squad X", out _));
            Assert.IsFalse(DoublesPairingService.TryParseSquadSheetName("", out _));
            Assert.IsFalse(DoublesPairingService.TryParseSquadSheetName(null, out _));
        }
    }

    /// <summary>
    /// Characterization tests for the Excel cell reading and row-level import
    /// decision chain, run against in-memory ClosedXML workbooks with
    /// dictionary-backed member/participant lookups.
    /// </summary>
    [TestClass]
    public class DoublesPairingImportTests
    {
        [TestMethod]
        public void TryReadIntCell_ReadsIntDoubleAndNumericText()
        {
            using XLWorkbook wb = new();
            IXLWorksheet ws = wb.AddWorksheet("Squad 1");
            ws.Cell(1, 1).Value = 42;
            ws.Cell(2, 1).Value = 17.6;
            ws.Cell(3, 1).Value = 3.2;
            ws.Cell(4, 1).Value = "29";
            ws.Cell(5, 1).Value = "abc";
            // Cell (6,1) left empty

            Assert.IsTrue(DoublesPairingService.TryReadIntCell(ws, 1, 1, out int intValue));
            Assert.AreEqual(42, intValue);

            Assert.IsTrue(DoublesPairingService.TryReadIntCell(ws, 2, 1, out int roundedUp));
            Assert.AreEqual(18, roundedUp);

            Assert.IsTrue(DoublesPairingService.TryReadIntCell(ws, 3, 1, out int roundedDown));
            Assert.AreEqual(3, roundedDown);

            Assert.IsTrue(DoublesPairingService.TryReadIntCell(ws, 4, 1, out int textValue));
            Assert.AreEqual(29, textValue);

            Assert.IsFalse(DoublesPairingService.TryReadIntCell(ws, 5, 1, out _));
            Assert.IsFalse(DoublesPairingService.TryReadIntCell(ws, 6, 1, out _));
        }

        [TestMethod]
        public void RunImport_ProcessesRowsAndRecordsSkipsErrorsAndUpserts()
        {
            using XLWorkbook wb = new();

            // Non-squad sheet is ignored entirely, even with data in the row range
            IXLWorksheet roster = wb.AddWorksheet("Roster");
            roster.Cell(9, 2).Value = "Ignored";
            roster.Cell(9, 3).Value = 101;
            roster.Cell(9, 10).Value = 1;

            // Squad number beyond the tournament's squad count: error, rows untouched
            IXLWorksheet squad5 = wb.AddWorksheet("Squad 5");
            squad5.Cell(9, 2).Value = "Ignored";
            squad5.Cell(9, 3).Value = 101;
            squad5.Cell(9, 10).Value = 1;

            IXLWorksheet ws = wb.AddWorksheet("Squad 1");
            ws.Cell(9, 2).Value = "Alice Smith";    ws.Cell(9, 3).Value = 101;    ws.Cell(9, 10).Value = 2;
            ws.Cell(10, 2).Value = "No Number";     /* column C empty */
            ws.Cell(11, 2).Value = "Bad Number";    ws.Cell(11, 3).Value = "abc"; ws.Cell(11, 10).Value = 1;
            ws.Cell(12, 2).Value = "Bad Count";     ws.Cell(12, 3).Value = 102;   ws.Cell(12, 10).Value = "bad";
            ws.Cell(13, 2).Value = "Negative";      ws.Cell(13, 3).Value = 103;   ws.Cell(13, 10).Value = -1;
            ws.Cell(14, 2).Value = "Unknown";       ws.Cell(14, 3).Value = 999;   ws.Cell(14, 10).Value = 1;
            ws.Cell(15, 2).Value = "Existing";      ws.Cell(15, 3).Value = 104;   ws.Cell(15, 10).Value = 0;
            ws.Cell(16, 2).Value = "Ensure Fails";  ws.Cell(16, 3).Value = 105;   ws.Cell(16, 10).Value = 1;
            // Row 17 column B empty: loop stops

            // Member number -> member id; unknown numbers resolve to 0
            Dictionary<int, int> members = new() { { 101, 11 }, { 102, 12 }, { 103, 13 }, { 104, 14 }, { 105, 15 } };
            // Member id 14 is already a participant in squad 1
            HashSet<(int MemberId, int Squad)> existingParticipants = [(14, 1)];
            List<(int MemberId, int Squad, int ExpectedCount)> upserts = [];

            DoublesImportSummary summary = DoublesPairingService.RunImport(
                wb,
                squadCount: 2,
                getMemberIdByNumber: n => members.TryGetValue(n, out int id) ? id : 0,
                participantExists: (memberId, squad) => existingParticipants.Contains((memberId, squad)),
                ensureParticipantExists: (memberId, squad) => memberId != 15,
                upsertPlan: (memberId, squad, expectedCount) => upserts.Add((memberId, squad, expectedCount)));

            Assert.AreEqual(8, summary.RowsProcessed);
            Assert.AreEqual(6, summary.RowsSkipped);
            Assert.AreEqual(2, summary.PlansUpserted);
            Assert.AreEqual(1, summary.ParticipantsCreated);

            Assert.AreEqual(6, summary.Errors.Count);
            Assert.AreEqual("Sheet 'Squad 5': squad is out of range for this tournament.", summary.Errors[0]);
            Assert.AreEqual("Sheet 'Squad 1', row 11: invalid member number in column C.", summary.Errors[1]);
            Assert.AreEqual("Sheet 'Squad 1', row 12: invalid partner count in column J.", summary.Errors[2]);
            Assert.AreEqual("Sheet 'Squad 1', row 13: partner count cannot be negative.", summary.Errors[3]);
            Assert.AreEqual("Sheet 'Squad 1', row 14: member #999 not found.", summary.Errors[4]);
            Assert.AreEqual("Sheet 'Squad 1', row 16: could not create participant for #105.", summary.Errors[5]);

            Assert.AreEqual(2, summary.ProcessedEntries.Count);
            Assert.IsTrue(summary.ProcessedEntries.Contains((101, 1)));
            Assert.IsTrue(summary.ProcessedEntries.Contains((104, 1)));

            Assert.AreEqual(2, upserts.Count);
            Assert.AreEqual((11, 1, 2), upserts[0]);
            Assert.AreEqual((14, 1, 0), upserts[1]);
        }

        [TestMethod]
        public void ApplyReimportDiff_ReportsRemovalsAndCountChanges()
        {
            DoublesImportSummary summary = new();
            summary.ProcessedEntries.Add((101, 1));
            summary.ProcessedEntries.Add((103, 1));

            // 102 vanished entirely; 103 moved from squad 2 to squad 1
            List<(int MemberNumber, int Squad)> prevParticipants = [(101, 1), (102, 1), (103, 2)];
            // Plan for 104 was deleted (not in updated): reported nowhere, matching the original
            Dictionary<(int MemberNumber, int Squad), int> prevPlans = new() { { (101, 1), 2 }, { (104, 1), 1 } };
            Dictionary<(int MemberNumber, int Squad), int> updatedPlans = new() { { (101, 1), 3 } };

            DoublesPairingService.ApplyReimportDiff(summary, prevParticipants, prevPlans, updatedPlans);

            Assert.AreEqual(1, summary.RemovedFromTournament.Count);
            Assert.AreEqual("#102 removed from tournament entirely", summary.RemovedFromTournament[0]);

            Assert.AreEqual(1, summary.RemovedFromSquad.Count);
            Assert.AreEqual("#103 no longer in Squad 2", summary.RemovedFromSquad[0]);

            Assert.AreEqual(1, summary.PartnerCountChanged.Count);
            Assert.AreEqual("#101 (Squad 1): 2 → 3 partners", summary.PartnerCountChanged[0]);
        }
    }

    /// <summary>
    /// Characterization tests for the discrepancy set math extracted from
    /// FrmDoublesDiscrepancies.LoadDiscrepancies, driven by in-memory
    /// plan/claim lists.
    /// </summary>
    [TestClass]
    public class DoublesPairingDiscrepancyTests
    {
        private static Member MakeMember(int id, int number, string first, string last)
        {
            return new Member { Id = id, Number = number, FirstName = first, LastName = last };
        }

        private static DoublesPartnerClaim MakeClaim(int squad, Member source, Member partner)
        {
            return new DoublesPartnerClaim { Squad = squad, SourceMember = source, PartnerMember = partner };
        }

        private static DoublesPartnerPlan MakePlan(int squad, Member member, int expectedCount)
        {
            return new DoublesPartnerPlan { Squad = squad, Member = member, ExpectedPartnerCount = expectedCount };
        }

        [TestMethod]
        public void ComputeDiscrepancies_FindsMissingReciprocalsThenCountMismatches()
        {
            Member ann = MakeMember(1, 201, "Ann", "Ames");
            Member bob = MakeMember(2, 202, "Bob", "Baker");
            Member cal = MakeMember(3, 203, "Cal", "Cole");
            Member dee = MakeMember(4, 204, "Dee", "Dunn");
            Member eve = MakeMember(5, 205, "Eve", "Eads");

            // Ann<->Bob is reciprocal; Cal->Dee has no reciprocal
            List<DoublesPartnerClaim> claims =
            [
                MakeClaim(1, ann, bob),
                MakeClaim(1, bob, ann),
                MakeClaim(1, cal, dee)
            ];

            // Ann planned 1, entered 1 (ok); Cal planned 2, entered 1; Eve planned 1, entered 0
            List<DoublesPartnerPlan> plans =
            [
                MakePlan(1, ann, 1),
                MakePlan(1, cal, 2),
                MakePlan(1, eve, 1)
            ];

            List<DoublesDiscrepancy> result = DoublesPairingService.ComputeDiscrepancies(plans, claims);

            Assert.AreEqual(3, result.Count);

            Assert.AreEqual(DoublesDiscrepancyType.MissingReciprocal, result[0].Type);
            Assert.AreEqual(1, result[0].Squad);
            Assert.AreEqual(3, result[0].SourceMemberId);
            Assert.AreEqual(203, result[0].SourceMemberNumber);
            Assert.AreEqual("Cal Cole", result[0].SourceMemberName);
            Assert.AreEqual(4, result[0].PartnerMemberId);
            Assert.AreEqual(204, result[0].PartnerMemberNumber);
            Assert.AreEqual("Dee Dunn", result[0].PartnerMemberName);

            Assert.AreEqual(DoublesDiscrepancyType.CountMismatch, result[1].Type);
            Assert.AreEqual(3, result[1].SourceMemberId);
            Assert.AreEqual(2, result[1].PlannedCount);
            Assert.AreEqual(1, result[1].ActualCount);

            Assert.AreEqual(DoublesDiscrepancyType.CountMismatch, result[2].Type);
            Assert.AreEqual(5, result[2].SourceMemberId);
            Assert.AreEqual(1, result[2].PlannedCount);
            Assert.AreEqual(0, result[2].ActualCount);
        }

        [TestMethod]
        public void ComputeDiscrepancies_ReciprocalMustBeInSameSquad()
        {
            Member ann = MakeMember(1, 201, "Ann", "Ames");
            Member bob = MakeMember(2, 202, "Bob", "Baker");

            // A->B in squad 1 and B->A in squad 2 do not satisfy each other
            List<DoublesPartnerClaim> claims =
            [
                MakeClaim(1, ann, bob),
                MakeClaim(2, bob, ann)
            ];

            List<DoublesDiscrepancy> result = DoublesPairingService.ComputeDiscrepancies([], claims);

            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.All(d => d.Type == DoublesDiscrepancyType.MissingReciprocal));
            Assert.AreEqual(1, result[0].SourceMemberId);
            Assert.AreEqual(1, result[0].Squad);
            Assert.AreEqual(2, result[1].SourceMemberId);
            Assert.AreEqual(2, result[1].Squad);
        }

        [TestMethod]
        public void ComputeDiscrepancies_NoIssues_ReturnsEmpty()
        {
            Member ann = MakeMember(1, 201, "Ann", "Ames");
            Member bob = MakeMember(2, 202, "Bob", "Baker");

            List<DoublesPartnerClaim> claims =
            [
                MakeClaim(1, ann, bob),
                MakeClaim(1, bob, ann)
            ];
            List<DoublesPartnerPlan> plans =
            [
                MakePlan(1, ann, 1),
                MakePlan(1, bob, 1)
            ];

            Assert.AreEqual(0, DoublesPairingService.ComputeDiscrepancies(plans, claims).Count);
        }
    }

    /// <summary>
    /// Characterization tests for the pairing-grid reconciliation and bowler
    /// roster math extracted from FrmDoublesTeamPairing.LoadPairings and
    /// PopulateBowlersList.
    /// </summary>
    [TestClass]
    public class DoublesPairingReconciliationTests
    {
        private static DoublesTeam MakeTeam(int id, int squad, int member1Number)
        {
            return new DoublesTeam
            {
                Id = id,
                Squad = squad,
                Member1 = new Member { Id = id * 10, Number = member1Number },
                Member2 = new Member { Id = id * 10 + 1, Number = member1Number + 1 }
            };
        }

        [TestMethod]
        public void FilterAndSortTeams_AllSquads_SortsBySquadThenBowler1Number()
        {
            List<DoublesTeam> teams =
            [
                MakeTeam(1, 2, 110),
                MakeTeam(2, 1, 300),
                MakeTeam(3, 1, 100),
                MakeTeam(4, 2, 50)
            ];

            List<DoublesTeam> result = DoublesPairingService.FilterAndSortTeams(teams, 0);

            Assert.AreEqual(4, result.Count);
            Assert.AreEqual(3, result[0].Id);   // S1 #100
            Assert.AreEqual(2, result[1].Id);   // S1 #300
            Assert.AreEqual(4, result[2].Id);   // S2 #50
            Assert.AreEqual(1, result[3].Id);   // S2 #110
        }

        [TestMethod]
        public void FilterAndSortTeams_SpecificSquad_FiltersBeforeSorting()
        {
            List<DoublesTeam> teams =
            [
                MakeTeam(1, 2, 110),
                MakeTeam(2, 1, 300),
                MakeTeam(3, 2, 50)
            ];

            List<DoublesTeam> result = DoublesPairingService.FilterAndSortTeams(teams, 2);

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(3, result[0].Id);   // S2 #50
            Assert.AreEqual(1, result[1].Id);   // S2 #110
        }

        [TestMethod]
        public void ComputeSquadBreakdown_GroupsAndOrdersBySquad()
        {
            List<DoublesTeam> teams =
            [
                MakeTeam(1, 2, 110),
                MakeTeam(2, 1, 300),
                MakeTeam(3, 1, 100)
            ];

            List<DoublesSquadTeamCount> result = DoublesPairingService.ComputeSquadBreakdown(teams);

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(new DoublesSquadTeamCount(1, 2), result[0]);
            Assert.AreEqual(new DoublesSquadTeamCount(2, 1), result[1]);
        }

        [TestMethod]
        public void BuildBowlerRoster_ReconcilesPlannedAndEnteredCountsPerSquad()
        {
            Member ann = new() { Id = 1, Number = 101, FirstName = "Ann", LastName = "Ames" };
            Member bob = new() { Id = 2, Number = 102, FirstName = "Bob", LastName = "Baker" };

            // Ann bowls in both squads; Bob only in squad 1
            List<DoublesParticipantInfo> participants =
            [
                new DoublesParticipantInfo(2, 1, 101, "Ann", "Ames"),
                new DoublesParticipantInfo(1, 2, 102, "Bob", "Baker"),
                new DoublesParticipantInfo(1, 1, 101, "Ann", "Ames")
            ];

            List<DoublesPartnerPlan> plans =
            [
                new DoublesPartnerPlan { Squad = 1, Member = ann, ExpectedPartnerCount = 2 },
                new DoublesPartnerPlan { Squad = 2, Member = ann, ExpectedPartnerCount = 1 }
            ];

            List<DoublesPartnerClaim> claims =
            [
                new DoublesPartnerClaim { Squad = 1, SourceMember = ann, PartnerMember = bob },
                new DoublesPartnerClaim { Squad = 1, SourceMember = bob, PartnerMember = ann }
            ];

            List<DoublesBowlerRosterEntry> result = DoublesPairingService.BuildBowlerRoster(participants, 0, plans, claims);

            Assert.AreEqual(3, result.Count);
            Assert.AreEqual(new DoublesBowlerRosterEntry(1, 1, 101, "Ann", "Ames", 2, 1), result[0]);
            Assert.AreEqual(new DoublesBowlerRosterEntry(1, 2, 102, "Bob", "Baker", 0, 1), result[1]);
            Assert.AreEqual(new DoublesBowlerRosterEntry(2, 1, 101, "Ann", "Ames", 1, 0), result[2]);
        }

        [TestMethod]
        public void BuildBowlerRoster_SpecificSquad_FiltersParticipants()
        {
            List<DoublesParticipantInfo> participants =
            [
                new DoublesParticipantInfo(1, 1, 101, "Ann", "Ames"),
                new DoublesParticipantInfo(2, 1, 101, "Ann", "Ames")
            ];

            List<DoublesBowlerRosterEntry> result = DoublesPairingService.BuildBowlerRoster(participants, 2, [], []);

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(2, result[0].Squad);
        }
    }
}
