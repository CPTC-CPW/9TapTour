using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NineTapTour.Core.Data.Migrations
{
    /// <summary>
    /// Re-introduces regions (dropped by RemoveRegions in 2026-03) with clean
    /// names. Existing databases already hold members and tournaments, so the
    /// scaffolded "add NOT NULL column with default 0 then add FK" would fail;
    /// instead the migration seeds one "Default" region, adds the columns as
    /// nullable, backfills every row to the default, and only then tightens
    /// the columns and adds the foreign keys.
    /// </summary>
    public partial class AddRegions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Regions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Regions", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Regions_Name",
                table: "Regions",
                column: "Name",
                unique: true);

            migrationBuilder.InsertData(
                table: "Regions",
                column: "Name",
                value: "Default");

            migrationBuilder.AddColumn<int>(
                name: "RegionId",
                table: "Members",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RegionId",
                table: "Tournaments",
                type: "int",
                nullable: true);

            // Every existing member and tournament belongs to the default region.
            migrationBuilder.Sql("UPDATE Members SET RegionId = (SELECT MIN(Id) FROM Regions) WHERE RegionId IS NULL");
            migrationBuilder.Sql("UPDATE Tournaments SET RegionId = (SELECT MIN(Id) FROM Regions) WHERE RegionId IS NULL");

            migrationBuilder.AlterColumn<int>(
                name: "RegionId",
                table: "Members",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "RegionId",
                table: "Tournaments",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Members_RegionId",
                table: "Members",
                column: "RegionId");

            migrationBuilder.CreateIndex(
                name: "IX_Tournaments_RegionId",
                table: "Tournaments",
                column: "RegionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Members_Regions_RegionId",
                table: "Members",
                column: "RegionId",
                principalTable: "Regions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Tournaments_Regions_RegionId",
                table: "Tournaments",
                column: "RegionId",
                principalTable: "Regions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Members_Regions_RegionId",
                table: "Members");

            migrationBuilder.DropForeignKey(
                name: "FK_Tournaments_Regions_RegionId",
                table: "Tournaments");

            migrationBuilder.DropIndex(
                name: "IX_Tournaments_RegionId",
                table: "Tournaments");

            migrationBuilder.DropIndex(
                name: "IX_Members_RegionId",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "RegionId",
                table: "Tournaments");

            migrationBuilder.DropColumn(
                name: "RegionId",
                table: "Members");

            migrationBuilder.DropTable(
                name: "Regions");
        }
    }
}
