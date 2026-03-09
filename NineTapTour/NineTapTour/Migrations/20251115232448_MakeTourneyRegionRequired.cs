using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NineTapTour.Migrations
{
    /// <inheritdoc />
    public partial class MakeTourneyRegionRequired : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tournaments_NineTapRegions_TourneyRegionNineTapRegionID",
                table: "Tournaments");

            migrationBuilder.AlterColumn<int>(
                name: "TourneyRegionNineTapRegionID",
                table: "Tournaments",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Tournaments_NineTapRegions_TourneyRegionNineTapRegionID",
                table: "Tournaments",
                column: "TourneyRegionNineTapRegionID",
                principalTable: "NineTapRegions",
                principalColumn: "NineTapRegionID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tournaments_NineTapRegions_TourneyRegionNineTapRegionID",
                table: "Tournaments");

            migrationBuilder.AlterColumn<int>(
                name: "TourneyRegionNineTapRegionID",
                table: "Tournaments",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Tournaments_NineTapRegions_TourneyRegionNineTapRegionID",
                table: "Tournaments",
                column: "TourneyRegionNineTapRegionID",
                principalTable: "NineTapRegions",
                principalColumn: "NineTapRegionID");
        }
    }
}
