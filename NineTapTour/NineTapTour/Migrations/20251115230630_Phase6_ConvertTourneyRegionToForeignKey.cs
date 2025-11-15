using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NineTapTour.Migrations
{
    /// <inheritdoc />
    public partial class Phase6_ConvertTourneyRegionToForeignKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TourneyRegion",
                table: "Tournaments");

            migrationBuilder.AddColumn<int>(
                name: "TourneyRegionNineTapRegionID",
                table: "Tournaments",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tournaments_TourneyRegionNineTapRegionID",
                table: "Tournaments",
                column: "TourneyRegionNineTapRegionID");

            migrationBuilder.AddForeignKey(
                name: "FK_Tournaments_NineTapRegions_TourneyRegionNineTapRegionID",
                table: "Tournaments",
                column: "TourneyRegionNineTapRegionID",
                principalTable: "NineTapRegions",
                principalColumn: "NineTapRegionID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tournaments_NineTapRegions_TourneyRegionNineTapRegionID",
                table: "Tournaments");

            migrationBuilder.DropIndex(
                name: "IX_Tournaments_TourneyRegionNineTapRegionID",
                table: "Tournaments");

            migrationBuilder.DropColumn(
                name: "TourneyRegionNineTapRegionID",
                table: "Tournaments");

            migrationBuilder.AddColumn<int>(
                name: "TourneyRegion",
                table: "Tournaments",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
