using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NineTapTour.Migrations
{
    /// <inheritdoc />
    public partial class AddTwoDayAndDoublesPairing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsTwoDay",
                table: "Tournaments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDay2",
                table: "Participants",
                type: "bit",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DoublesTeams",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TournamentId = table.Column<int>(type: "int", nullable: false),
                    Member1Id = table.Column<int>(type: "int", nullable: false),
                    Member2Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DoublesTeams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DoublesTeams_Members_Member1Id",
                        column: x => x.Member1Id,
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DoublesTeams_Members_Member2Id",
                        column: x => x.Member2Id,
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DoublesTeams_Tournaments_TournamentId",
                        column: x => x.TournamentId,
                        principalTable: "Tournaments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DoublesTeams_Member1Id",
                table: "DoublesTeams",
                column: "Member1Id");

            migrationBuilder.CreateIndex(
                name: "IX_DoublesTeams_Member2Id",
                table: "DoublesTeams",
                column: "Member2Id");

            migrationBuilder.CreateIndex(
                name: "IX_DoublesTeams_TournamentId",
                table: "DoublesTeams",
                column: "TournamentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DoublesTeams");

            migrationBuilder.DropColumn(
                name: "IsTwoDay",
                table: "Tournaments");

            migrationBuilder.DropColumn(
                name: "IsDay2",
                table: "Participants");
        }
    }
}
