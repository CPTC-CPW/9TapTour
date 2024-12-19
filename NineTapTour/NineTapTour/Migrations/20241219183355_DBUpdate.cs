using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NineTapTour.Migrations
{
    /// <inheritdoc />
    public partial class DBUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Participants_Members_MemberId",
                table: "Participants");

            migrationBuilder.AlterColumn<int>(
                name: "MemberId",
                table: "Participants",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Participants_Members_MemberId",
                table: "Participants",
                column: "MemberId",
                principalTable: "Members",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Participants_Members_MemberId",
                table: "Participants");

            migrationBuilder.AlterColumn<int>(
                name: "MemberId",
                table: "Participants",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Participants_Members_MemberId",
                table: "Participants",
                column: "MemberId",
                principalTable: "Members",
                principalColumn: "Id");
        }
    }
}
