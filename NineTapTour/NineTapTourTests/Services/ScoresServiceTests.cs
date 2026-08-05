using Microsoft.VisualStudio.TestTools.UnitTesting;
using NineTapTour.Core.Entities;
using NineTapTour.Core.Models;
using NineTapTour.Core.Services;
using System.Collections.Generic;
using System.Linq;
using static NineTapTour.Core.Calculations.ReportHelper;

namespace NineTapTourTests.Services
{
    /// <summary>
    /// Characterization tests for the pure score-entry hub logic extracted from
    /// FrmMemberScores (M7.3). Expectations were hand-computed from the original
    /// form code and must not change.
    /// </summary>
    [TestClass]
    public class ScoresServiceLeaderboardTests
    {
        // Ann: 4 games 150/200/180/170, member handicap 10, member bonus 2, game handicap 12, game bonus 3
        //   ScratchTotal = 700, Top3 = 200+180+170 = 550
        //   Top3Handi = 550 + 3*10 + 3*3 = 589
        //   HandicapScore = 700 + 4*12 + 4*3 = 760, HighScore = 200
        // Bob: 2 games 210/-/190/-, member handicap 20, member bonus 0, game handicap 25, game bonus 5
        //   ScratchTotal = 400, Top3 = 210+190 = 400
        //   Top3Handi = 400 + 2*20 + 2*5 = 450
        //   HandicapScore = 400 + 2*25 + 2*5 = 460, HighScore = 210
        private static List<Participant> MakeParticipants()
        {
            return
            [
                new Participant
                {
                    Squad = 1,
                    Member = new Member { Number = 101, FirstName = "Ann", LastName = "Ames", Handicap = 10, Bonus = 2 },
                    Game = new Game { Id = 11, Game1 = 150, Game2 = 200, Game3 = 180, Game4 = 170, Handicap = 12, Bonus = 3 }
                },
                new Participant
                {
                    Squad = 2,
                    Member = new Member { Number = 102, FirstName = "Bob", LastName = "Baker", Handicap = 20, Bonus = 0 },
                    Game = new Game { Id = 22, Game1 = 210, Game2 = null, Game3 = 190, Game4 = null, Handicap = 25, Bonus = 5 }
                }
            ];
        }

        [TestMethod]
        public void BuildLeaderboards_ThreeOutOf4HighSeriesScratch_OrdersByTop3ScratchAndDropsLowestGame()
        {
            LeaderboardResult result = ScoresService.BuildLeaderboards(MakeParticipants(), true, ReportType.HighSeriesScratch);

            Assert.HasCount(2, result.Top3Scores);
            Assert.AreEqual(101, result.Top3Scores[0].MemberNo);
            Assert.AreEqual(550, result.Top3Scores[0].Top3ScratchScore);
            Assert.AreEqual(102, result.Top3Scores[1].MemberNo);
            Assert.AreEqual(400, result.Top3Scores[1].Top3ScratchScore);
            Assert.IsTrue(result.Top3Scores[0].ThreeOutOf4);
        }

        [TestMethod]
        public void BuildLeaderboards_RegularHighSeriesScratch_OrdersByFullScratchTotal()
        {
            LeaderboardResult result = ScoresService.BuildLeaderboards(MakeParticipants(), false, ReportType.HighSeriesScratch);

            Assert.AreEqual(101, result.Top3Scores[0].MemberNo);
            Assert.AreEqual(700, result.Top3Scores[0].ScratchTotal);
            Assert.AreEqual(102, result.Top3Scores[1].MemberNo);
            Assert.AreEqual(400, result.Top3Scores[1].ScratchTotal);
            Assert.IsFalse(result.Top3Scores[0].ThreeOutOf4);
        }

        [TestMethod]
        public void BuildLeaderboards_ThreeOutOf4HighSeriesHandicap_OrdersByTop3HandicapTotal()
        {
            LeaderboardResult result = ScoresService.BuildLeaderboards(MakeParticipants(), true, ReportType.HighSeriesHandicap);

            Assert.AreEqual(101, result.Top3Scores[0].MemberNo);
            Assert.AreEqual(589, result.Top3Scores[0].Top3HandiScores);
            Assert.AreEqual(102, result.Top3Scores[1].MemberNo);
            Assert.AreEqual(450, result.Top3Scores[1].Top3HandiScores);
        }

        [TestMethod]
        public void BuildLeaderboards_RegularHighSeriesHandicap_OrdersByGameHandicapTotal()
        {
            LeaderboardResult result = ScoresService.BuildLeaderboards(MakeParticipants(), false, ReportType.HighSeriesHandicap);

            Assert.AreEqual(101, result.Top3Scores[0].MemberNo);
            Assert.AreEqual(760, result.Top3Scores[0].HandicapScore);
            Assert.AreEqual(102, result.Top3Scores[1].MemberNo);
            Assert.AreEqual(460, result.Top3Scores[1].HandicapScore);
        }

        [TestMethod]
        public void BuildLeaderboards_HighGame_OrdersByHighestScratchGame()
        {
            LeaderboardResult result = ScoresService.BuildLeaderboards(MakeParticipants(), false, ReportType.HighGame);

            Assert.HasCount(2, result.ParticipantsGameScores);
            Assert.AreEqual(102, result.ParticipantsGameScores[0].MemberNo);
            Assert.AreEqual(210, result.ParticipantsGameScores[0].HighScore);
            Assert.AreEqual(101, result.ParticipantsGameScores[1].MemberNo);
            Assert.AreEqual(200, result.ParticipantsGameScores[1].HighScore);
        }

        [TestMethod]
        public void BuildLeaderboards_HighGameHandicap_OrdersByHighGamePlusMemberHandicapAndBonus()
        {
            // Ann: 200 + 10 + 2 = 212, Bob: 210 + 20 + 0 = 230
            LeaderboardResult result = ScoresService.BuildLeaderboards(MakeParticipants(), false, ReportType.HighGameHandicapGameSenior);

            Assert.AreEqual(102, result.ParticipantsGameScores[0].MemberNo);
            Assert.AreEqual(101, result.ParticipantsGameScores[1].MemberNo);
        }

        [TestMethod]
        public void FilterParticipantsBySquad_MatchesCurrentBehavior()
        {
            List<Participant> participants = MakeParticipants();

            // Single squad number filters to that squad
            List<Participant> squad2Only = ScoresService.FilterParticipantsBySquad(participants, 2, []);
            Assert.HasCount(1, squad2Only);
            Assert.AreEqual(102, squad2Only[0].Member.Number);

            // 9 applies the multi-squad filter list
            List<Participant> filtered = ScoresService.FilterParticipantsBySquad(participants, 9, [1]);
            Assert.HasCount(1, filtered);
            Assert.AreEqual(101, filtered[0].Member.Number);

            // 0 (all squads) leaves the list unchanged
            Assert.HasCount(2, ScoresService.FilterParticipantsBySquad(participants, 0, []));

            // 9 with an empty filter list leaves the list unchanged
            Assert.HasCount(2, ScoresService.FilterParticipantsBySquad(participants, 9, []));
        }
    }

    [TestClass]
    public class ScoresServiceDoublesTests
    {
        private static List<DoublesTeam> MakeTeams()
        {
            return
            [
                new DoublesTeam
                {
                    Id = 1,
                    Squad = 1,
                    Member1 = new Member { Number = 101, FirstName = "Ann", LastName = "Ames" },
                    Member2 = new Member { Number = 102, FirstName = "Bob", LastName = "Baker" }
                },
                new DoublesTeam
                {
                    Id = 2,
                    Squad = 2,
                    Member1 = new Member { Number = 103, FirstName = "Cam", LastName = "Cole" },
                    Member2 = new Member { Number = 104, FirstName = "Dee", LastName = "Dean" }
                }
            ];
        }

        private static List<MemberScores> MakeIndividualScores()
        {
            return
            [
                new MemberScores { MemberId = 101, Score = 400, Squad = 1, Paid = true, LastPaymentYear = "2025" },
                new MemberScores { MemberId = 102, Score = 350, Squad = 1, Paid = false, LastPaymentYear = "2024" },
                // 103 has a score but partner 104 has none, so team 2 is skipped
                new MemberScores { MemberId = 103, Score = 500, Squad = 2, Paid = true, LastPaymentYear = "2026" }
            ];
        }

        [TestMethod]
        public void CombineDoublesSeriesToTeams_CombinesPartnersAndSkipsIncompleteTeams()
        {
            List<MemberScores> result = ScoresService.CombineDoublesSeriesToTeams(MakeIndividualScores(), MakeTeams(), null);

            Assert.HasCount(1, result);
            TeamMemberScores team = (TeamMemberScores)result[0];
            Assert.AreEqual(750, team.Score);
            Assert.IsFalse(team.Paid);                       // both partners must be paid
            Assert.AreEqual("2025", team.LastPaymentYear);   // first partner's payment year
            Assert.AreEqual(string.Empty, team.FirstName);
            Assert.AreEqual(101, team.MemberId);             // first partner's member number
            Assert.IsTrue(team.IsTeam);
            Assert.AreEqual(101, team.Partner1MemberId);
            Assert.AreEqual("Ann", team.Partner1FirstName);
            Assert.AreEqual("Ames", team.Partner1LastName);
            Assert.AreEqual(400, team.Partner1Score);
            Assert.AreEqual(102, team.Partner2MemberId);
            Assert.AreEqual("Bob", team.Partner2FirstName);
            Assert.AreEqual("Baker", team.Partner2LastName);
            Assert.AreEqual(350, team.Partner2Score);
            Assert.AreEqual("2024", team.Partner2LastPaymentYear);
        }

        [TestMethod]
        public void CombineDoublesSeriesToTeams_SquadFilterExcludesOtherSquads()
        {
            // Squad 2's team is filtered in but partner 104 has no standings row,
            // and squad 1's team is filtered out entirely.
            List<MemberScores> result = ScoresService.CombineDoublesSeriesToTeams(MakeIndividualScores(), MakeTeams(), [2]);

            Assert.IsEmpty(result);
        }

        [TestMethod]
        public void CombineDoublesSeriesToTeams_PartnerMustMatchTeamSquad()
        {
            // Partner 102's score is recorded in a different squad than the pairing
            List<MemberScores> scores =
            [
                new MemberScores { MemberId = 101, Score = 400, Squad = 1 },
                new MemberScores { MemberId = 102, Score = 350, Squad = 2 }
            ];

            List<MemberScores> result = ScoresService.CombineDoublesSeriesToTeams(scores, MakeTeams(), null);

            Assert.IsEmpty(result);
        }
    }

    [TestClass]
    public class ScoresServiceTotalsTests
    {
        [TestMethod]
        public void ComputeThreeOfFourAdjustedTotals_DropsLowestScratchAndLowestHandicapGame()
        {
            // Scratch 150+200+180+170 = 700, minus lowest 150 = 550
            // Handicap 165+215+195+185 = 760, minus lowest 165 = 595
            ScoreTotals totals = ScoresService.ComputeThreeOfFourAdjustedTotals(
                [150, 200, 180, 170],
                [165, 215, 195, 185]);

            Assert.AreEqual(550, totals.ScratchTotal);
            Assert.AreEqual(595, totals.HandicapTotal);
        }

        [DataTestMethod]
        // Regular 4-game tournament: any empty box counts as missing
        [DataRow(false, false, false, false, false, false, false)]
        [DataRow(false, false, true, false, false, false, true)]
        [DataRow(false, false, false, false, false, true, true)]
        // Doubles: only games 1 and 2 are required
        [DataRow(true, false, false, false, true, true, false)]
        [DataRow(true, false, true, false, false, false, true)]
        [DataRow(true, false, false, true, false, false, true)]
        // 3-game tournament: only the first three games are required
        [DataRow(false, true, false, false, false, true, false)]
        [DataRow(false, true, false, true, false, false, true)]
        [DataRow(false, true, false, false, true, false, true)]
        public void AreRequiredScoresMissing_MatchesCurrentBehavior(bool isDoubles, bool isOnlyThreeGames,
            bool game1Empty, bool game2Empty, bool game3Empty, bool game4Empty, bool expected)
        {
            Assert.AreEqual(expected, ScoresService.AreRequiredScoresMissing(
                isDoubles, isOnlyThreeGames, game1Empty, game2Empty, game3Empty, game4Empty));
        }
    }
}
