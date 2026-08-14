using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NineTapTour.Core.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPlaceStandingLabelForTwoDayGroups : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PlaceStandingLabel",
                table: "Games",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PlaceStandingLabel",
                table: "Games");
        }
    }
}
