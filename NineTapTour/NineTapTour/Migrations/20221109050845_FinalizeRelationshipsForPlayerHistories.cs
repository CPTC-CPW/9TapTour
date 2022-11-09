using Microsoft.EntityFrameworkCore.Migrations;

namespace NineTapTour.Migrations
{
    public partial class FinalizeRelationshipsForPlayerHistories : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_PlayerHistories_GameID",
                table: "PlayerHistories",
                column: "GameID");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerHistories_regionID",
                table: "PlayerHistories",
                column: "regionID");

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerHistories_Games_GameID",
                table: "PlayerHistories",
                column: "GameID",
                principalTable: "Games",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerHistories_NineTapRegions_regionID",
                table: "PlayerHistories",
                column: "regionID",
                principalTable: "NineTapRegions",
                principalColumn: "NineTapRegionID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlayerHistories_Games_GameID",
                table: "PlayerHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerHistories_NineTapRegions_regionID",
                table: "PlayerHistories");

            migrationBuilder.DropIndex(
                name: "IX_PlayerHistories_GameID",
                table: "PlayerHistories");

            migrationBuilder.DropIndex(
                name: "IX_PlayerHistories_regionID",
                table: "PlayerHistories");
        }
    }
}
