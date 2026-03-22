using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NineTapTour.Migrations
{
    /// <inheritdoc />
    public partial class RemoveRegions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Members_NineTapRegions_NineTapRegionID",
                table: "Members");

            migrationBuilder.DropForeignKey(
                name: "FK_Tournaments_NineTapRegions_TourneyRegionNineTapRegionID",
                table: "Tournaments");

            migrationBuilder.DropTable(
                name: "NineTapRegions");

            migrationBuilder.DropIndex(
                name: "IX_Tournaments_TourneyRegionNineTapRegionID",
                table: "Tournaments");

            migrationBuilder.DropIndex(
                name: "IX_Members_NineTapRegionID",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "TourneyRegionNineTapRegionID",
                table: "Tournaments");

            migrationBuilder.DropColumn(
                name: "NineTapRegionID",
                table: "Members");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TourneyRegionNineTapRegionID",
                table: "Tournaments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NineTapRegionID",
                table: "Members",
                type: "int",
                nullable: false,
                defaultValue: 0);

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

            migrationBuilder.CreateIndex(
                name: "IX_Tournaments_TourneyRegionNineTapRegionID",
                table: "Tournaments",
                column: "TourneyRegionNineTapRegionID");

            migrationBuilder.CreateIndex(
                name: "IX_Members_NineTapRegionID",
                table: "Members",
                column: "NineTapRegionID");

            migrationBuilder.AddForeignKey(
                name: "FK_Members_NineTapRegions_NineTapRegionID",
                table: "Members",
                column: "NineTapRegionID",
                principalTable: "NineTapRegions",
                principalColumn: "NineTapRegionID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tournaments_NineTapRegions_TourneyRegionNineTapRegionID",
                table: "Tournaments",
                column: "TourneyRegionNineTapRegionID",
                principalTable: "NineTapRegions",
                principalColumn: "NineTapRegionID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
