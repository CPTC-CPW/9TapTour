using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NineTapTour.Migrations
{
    /// <inheritdoc />
    public partial class Phase5_RemoveFinalizeTempsDbSet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FinalizeTemps");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FinalizeTemps",
                columns: table => new
                {
                    FinalizeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FinalizeRegionID = table.Column<int>(type: "int", nullable: false),
                    GameId = table.Column<int>(type: "int", nullable: false),
                    AdjustedAvg = table.Column<int>(type: "int", nullable: false),
                    Bonus = table.Column<int>(type: "int", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Game1 = table.Column<int>(type: "int", nullable: true),
                    Game2 = table.Column<int>(type: "int", nullable: true),
                    Game3 = table.Column<int>(type: "int", nullable: true),
                    Game4 = table.Column<int>(type: "int", nullable: true),
                    GameAvg = table.Column<int>(type: "int", nullable: false),
                    Handicap = table.Column<int>(type: "int", nullable: false),
                    HandicapTotal = table.Column<int>(type: "int", nullable: false),
                    KeepAdjustedAvg = table.Column<bool>(type: "bit", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LeagueAverage = table.Column<double>(type: "float", nullable: false),
                    MemberId = table.Column<int>(type: "int", nullable: false),
                    MemberNumber = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ScratchTotal = table.Column<int>(type: "int", nullable: false),
                    Squad = table.Column<int>(type: "int", nullable: false),
                    TournamentID = table.Column<int>(type: "int", nullable: false),
                    UseGame1 = table.Column<bool>(type: "bit", nullable: false),
                    UseGame2 = table.Column<bool>(type: "bit", nullable: false),
                    UseGame3 = table.Column<bool>(type: "bit", nullable: false),
                    UseGame4 = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinalizeTemps", x => x.FinalizeID);
                    table.ForeignKey(
                        name: "FK_FinalizeTemps_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FinalizeTemps_NineTapRegions_FinalizeRegionID",
                        column: x => x.FinalizeRegionID,
                        principalTable: "NineTapRegions",
                        principalColumn: "NineTapRegionID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FinalizeTemps_FinalizeRegionID",
                table: "FinalizeTemps",
                column: "FinalizeRegionID");

            migrationBuilder.CreateIndex(
                name: "IX_FinalizeTemps_GameId",
                table: "FinalizeTemps",
                column: "GameId");
        }
    }
}
