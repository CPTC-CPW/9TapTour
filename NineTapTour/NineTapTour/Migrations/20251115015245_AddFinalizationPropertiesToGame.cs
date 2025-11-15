using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NineTapTour.Migrations
{
    /// <inheritdoc />
    public partial class AddFinalizationPropertiesToGame : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AdjustedAvg",
                table: "Games",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GameAvg",
                table: "Games",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "HandicapTotal",
                table: "Games",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsFinalized",
                table: "Games",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "KeepAdjustedAvg",
                table: "Games",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<double>(
                name: "LeagueAverage",
                table: "Games",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "TournamentID",
                table: "Games",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdjustedAvg",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "GameAvg",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "HandicapTotal",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "IsFinalized",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "KeepAdjustedAvg",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "LeagueAverage",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "TournamentID",
                table: "Games");
        }
    }
}
