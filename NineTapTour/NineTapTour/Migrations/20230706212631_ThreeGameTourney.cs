using Microsoft.EntityFrameworkCore.Migrations;

namespace NineTapTour.Migrations
{
    public partial class ThreeGameTourney : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsOnlyThreeGames",
                table: "Tournaments",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsOnlyThreeGames",
                table: "Tournaments");
        }
    }
}
