using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NineTapTour.Core.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTournamentIsImported : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsImported",
                table: "Tournaments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            // Tournaments created by earlier legacy-import runs are only
            // identifiable by the hard-coded location the importer wrote.
            migrationBuilder.Sql("UPDATE Tournaments SET IsImported = 1 WHERE Location = 'Imported'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsImported",
                table: "Tournaments");
        }
    }
}
