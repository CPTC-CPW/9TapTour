using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NineTapTour.Migrations
{
    /// <inheritdoc />
    public partial class Phase3_RemovePlayerHistoriesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlayerHistories");

            migrationBuilder.DropIndex(
                name: "IX_Participants_GameId",
                table: "Participants");

            migrationBuilder.CreateIndex(
                name: "IX_Participants_GameId",
                table: "Participants",
                column: "GameId",
                unique: true,
                filter: "[GameId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Participants_GameId",
                table: "Participants");

            migrationBuilder.CreateTable(
                name: "PlayerHistories",
                columns: table => new
                {
                    hisID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GameID = table.Column<int>(type: "int", nullable: false),
                    regionID = table.Column<int>(type: "int", nullable: false),
                    AVG = table.Column<int>(type: "int", nullable: false),
                    AverageForEntry = table.Column<double>(type: "float", nullable: false),
                    Bonus = table.Column<int>(type: "int", nullable: false),
                    Game1 = table.Column<int>(type: "int", nullable: true),
                    Game2 = table.Column<int>(type: "int", nullable: true),
                    Game3 = table.Column<int>(type: "int", nullable: true),
                    Game4 = table.Column<int>(type: "int", nullable: true),
                    GamesPlayed = table.Column<int>(type: "int", nullable: false),
                    HandiCap = table.Column<int>(type: "int", nullable: false),
                    MemberNumber = table.Column<int>(type: "int", nullable: false),
                    MoneyWon = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PPHG = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProPot = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TotalScore = table.Column<int>(type: "int", nullable: false),
                    TournamentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    trueAVG = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerHistories", x => x.hisID);
                    table.ForeignKey(
                        name: "FK_PlayerHistories_Games_GameID",
                        column: x => x.GameID,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlayerHistories_NineTapRegions_regionID",
                        column: x => x.regionID,
                        principalTable: "NineTapRegions",
                        principalColumn: "NineTapRegionID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Participants_GameId",
                table: "Participants",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerHistories_GameID",
                table: "PlayerHistories",
                column: "GameID");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerHistories_regionID",
                table: "PlayerHistories",
                column: "regionID");
        }
    }
}
