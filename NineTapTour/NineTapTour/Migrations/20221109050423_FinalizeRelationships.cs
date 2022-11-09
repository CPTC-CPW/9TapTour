using Microsoft.EntityFrameworkCore.Migrations;

namespace NineTapTour.Migrations
{
    public partial class FinalizeRelationships : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_FinalizeTemps_FinalizeRegionID",
                table: "FinalizeTemps",
                column: "FinalizeRegionID");

            migrationBuilder.CreateIndex(
                name: "IX_FinalizeTemps_GameId",
                table: "FinalizeTemps",
                column: "GameId");

            migrationBuilder.AddForeignKey(
                name: "FK_FinalizeTemps_Games_GameId",
                table: "FinalizeTemps",
                column: "GameId",
                principalTable: "Games",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FinalizeTemps_NineTapRegions_FinalizeRegionID",
                table: "FinalizeTemps",
                column: "FinalizeRegionID",
                principalTable: "NineTapRegions",
                principalColumn: "NineTapRegionID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FinalizeTemps_Games_GameId",
                table: "FinalizeTemps");

            migrationBuilder.DropForeignKey(
                name: "FK_FinalizeTemps_NineTapRegions_FinalizeRegionID",
                table: "FinalizeTemps");

            migrationBuilder.DropIndex(
                name: "IX_FinalizeTemps_FinalizeRegionID",
                table: "FinalizeTemps");

            migrationBuilder.DropIndex(
                name: "IX_FinalizeTemps_GameId",
                table: "FinalizeTemps");
        }
    }
}
