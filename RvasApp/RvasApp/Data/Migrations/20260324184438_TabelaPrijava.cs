using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RvasApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class TabelaPrijava : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Prijave",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KursId = table.Column<int>(type: "int", nullable: false),
                    PolaznikId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DatumPrijave = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prijave", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Prijave_AspNetUsers_PolaznikId",
                        column: x => x.PolaznikId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Prijave_Kursevi_KursId",
                        column: x => x.KursId,
                        principalTable: "Kursevi",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Prijave_KursId_PolaznikId",
                table: "Prijave",
                columns: new[] { "KursId", "PolaznikId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Prijave_PolaznikId",
                table: "Prijave",
                column: "PolaznikId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Prijave");
        }
    }
}
