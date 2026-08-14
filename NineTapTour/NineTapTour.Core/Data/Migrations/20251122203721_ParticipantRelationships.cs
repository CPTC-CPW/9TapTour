using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NineTapTour.Core.Data.Migrations
{
    /// <inheritdoc />
    public partial class ParticipantRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Participants_Tournaments_TournamentId",
                table: "Participants");

            migrationBuilder.AlterColumn<int>(
                name: "TournamentId",
                table: "Participants",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Participants_Tournaments_TournamentId",
                table: "Participants",
                column: "TournamentId",
                principalTable: "Tournaments",
                principalColumn: "Id",
                // Avoid multiple cascade paths on SQL Server by not cascading deletes from Tournament
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Participants_Tournaments_TournamentId",
                table: "Participants");

            migrationBuilder.AlterColumn<int>(
                name: "TournamentId",
                table: "Participants",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Participants_Tournaments_TournamentId",
                table: "Participants",
                column: "TournamentId",
                principalTable: "Tournaments",
                principalColumn: "Id");
        }
    }
}
