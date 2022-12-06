using Microsoft.EntityFrameworkCore.Migrations;

namespace NineTapTour.Migrations
{
    public partial class DoubleTypeForLeagueAvg : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "LeagueAverage",
                table: "FinalizeTemps",
                type: "float",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "LeagueAverage",
                table: "FinalizeTemps",
                type: "int",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");
        }
    }
}
