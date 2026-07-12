using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NineTapTour.Migrations
{
    /// <inheritdoc />
    public partial class Pairings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DoublesPartnerClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TournamentId = table.Column<int>(type: "int", nullable: false),
                    SourceMemberId = table.Column<int>(type: "int", nullable: false),
                    PartnerMemberId = table.Column<int>(type: "int", nullable: false),
                    Squad = table.Column<int>(type: "int", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DoublesPartnerClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DoublesPartnerClaims_Members_PartnerMemberId",
                        column: x => x.PartnerMemberId,
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DoublesPartnerClaims_Members_SourceMemberId",
                        column: x => x.SourceMemberId,
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DoublesPartnerClaims_Tournaments_TournamentId",
                        column: x => x.TournamentId,
                        principalTable: "Tournaments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DoublesPartnerPlans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TournamentId = table.Column<int>(type: "int", nullable: false),
                    MemberId = table.Column<int>(type: "int", nullable: false),
                    Squad = table.Column<int>(type: "int", nullable: false),
                    ExpectedPartnerCount = table.Column<int>(type: "int", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DoublesPartnerPlans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DoublesPartnerPlans_Members_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DoublesPartnerPlans_Tournaments_TournamentId",
                        column: x => x.TournamentId,
                        principalTable: "Tournaments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DoublesPartnerClaims_PartnerMemberId",
                table: "DoublesPartnerClaims",
                column: "PartnerMemberId");

            migrationBuilder.CreateIndex(
                name: "IX_DoublesPartnerClaims_SourceMemberId",
                table: "DoublesPartnerClaims",
                column: "SourceMemberId");

            migrationBuilder.CreateIndex(
                name: "IX_DoublesPartnerClaims_TournamentId_SourceMemberId_PartnerMemberId_Squad",
                table: "DoublesPartnerClaims",
                columns: new[] { "TournamentId", "SourceMemberId", "PartnerMemberId", "Squad" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DoublesPartnerPlans_MemberId",
                table: "DoublesPartnerPlans",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_DoublesPartnerPlans_TournamentId_MemberId_Squad",
                table: "DoublesPartnerPlans",
                columns: new[] { "TournamentId", "MemberId", "Squad" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DoublesPartnerClaims");

            migrationBuilder.DropTable(
                name: "DoublesPartnerPlans");
        }
    }
}
