using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NineTapTour.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InputtedAvg = table.Column<int>(type: "int", nullable: true),
                    Game1 = table.Column<int>(type: "int", nullable: true),
                    Game2 = table.Column<int>(type: "int", nullable: true),
                    Game3 = table.Column<int>(type: "int", nullable: true),
                    Game4 = table.Column<int>(type: "int", nullable: true),
                    UseGame1 = table.Column<bool>(type: "bit", nullable: true),
                    UseGame2 = table.Column<bool>(type: "bit", nullable: true),
                    UseGame3 = table.Column<bool>(type: "bit", nullable: true),
                    UseGame4 = table.Column<bool>(type: "bit", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Handicap = table.Column<int>(type: "int", nullable: true),
                    Bonus = table.Column<int>(type: "int", nullable: true),
                    MoneyWon = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    SidePot = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PlaceStanding = table.Column<int>(type: "int", nullable: true),
                    IsComp = table.Column<bool>(type: "bit", nullable: false),
                    IsFinalized = table.Column<bool>(type: "bit", nullable: false),
                    LeagueAverage = table.Column<double>(type: "float", nullable: false),
                    AdjustedAvg = table.Column<int>(type: "int", nullable: false),
                    KeepAdjustedAvg = table.Column<bool>(type: "bit", nullable: false),
                    GameAvg = table.Column<int>(type: "int", nullable: false),
                    HandicapTotal = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NineTapRegions",
                columns: table => new
                {
                    NineTapRegionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NineTapRegionName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NineTapRegions", x => x.NineTapRegionID);
                });

            migrationBuilder.CreateTable(
                name: "Members",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Number = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MiddleInitial = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SSN = table.Column<string>(type: "char(11)", maxLength: 11, nullable: true),
                    Gender = table.Column<int>(type: "int", nullable: false),
                    Street = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    State = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PostalCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PrimaryPhone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecondaryPhone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Average = table.Column<int>(type: "int", nullable: true),
                    StartAvg = table.Column<int>(type: "int", nullable: true),
                    Handicap = table.Column<int>(type: "int", nullable: true),
                    Bonus = table.Column<int>(type: "int", nullable: false),
                    JoinDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejoinDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastBowled = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastPayment = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsLifetimeMember = table.Column<bool>(type: "bit", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Referrals = table.Column<int>(type: "int", nullable: true),
                    IsSenior = table.Column<bool>(type: "bit", nullable: false),
                    MoneyEarned = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NineTapRegionID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Members", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Members_NineTapRegions_NineTapRegionID",
                        column: x => x.NineTapRegionID,
                        principalTable: "NineTapRegions",
                        principalColumn: "NineTapRegionID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tournaments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Event = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Sponsors = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Squads = table.Column<int>(type: "int", nullable: false),
                    Doubles = table.Column<bool>(type: "bit", nullable: false),
                    ThreeOutOf4 = table.Column<bool>(type: "bit", nullable: false),
                    IsOnlyThreeGames = table.Column<bool>(type: "bit", nullable: false),
                    TourneyRegionNineTapRegionID = table.Column<int>(type: "int", nullable: true),
                    IsTournamentFinalized = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tournaments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tournaments_NineTapRegions_TourneyRegionNineTapRegionID",
                        column: x => x.TourneyRegionNineTapRegionID,
                        principalTable: "NineTapRegions",
                        principalColumn: "NineTapRegionID");
                });

            migrationBuilder.CreateTable(
                name: "Participants",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SquadNumber = table.Column<int>(type: "int", nullable: false),
                    MemberId = table.Column<int>(type: "int", nullable: false),
                    GameId = table.Column<int>(type: "int", nullable: true),
                    TournamentId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Participants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Participants_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Participants_Members_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Participants_Tournaments_TournamentId",
                        column: x => x.TournamentId,
                        principalTable: "Tournaments",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Members_NineTapRegionID",
                table: "Members",
                column: "NineTapRegionID");

            migrationBuilder.CreateIndex(
                name: "IX_Participants_GameId",
                table: "Participants",
                column: "GameId",
                unique: true,
                filter: "[GameId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Participants_MemberId",
                table: "Participants",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_Participants_TournamentId",
                table: "Participants",
                column: "TournamentId");

            migrationBuilder.CreateIndex(
                name: "IX_Tournaments_TourneyRegionNineTapRegionID",
                table: "Tournaments",
                column: "TourneyRegionNineTapRegionID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Participants");

            migrationBuilder.DropTable(
                name: "Games");

            migrationBuilder.DropTable(
                name: "Members");

            migrationBuilder.DropTable(
                name: "Tournaments");

            migrationBuilder.DropTable(
                name: "NineTapRegions");
        }
    }
}
