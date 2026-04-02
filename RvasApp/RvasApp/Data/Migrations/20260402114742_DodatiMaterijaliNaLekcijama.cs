using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RvasApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class DodatiMaterijaliNaLekcijama : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Materijali",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LekcijaId = table.Column<int>(type: "int", nullable: false),
                    Naziv = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Putanja = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Tip = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Velicina = table.Column<long>(type: "bigint", nullable: false),
                    DatumOtpremanja = table.Column<DateTime>(type: "datetime2", nullable: false),
                    InstruktorId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Materijali", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Materijali_AspNetUsers_InstruktorId",
                        column: x => x.InstruktorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Materijali_Lekcije_LekcijaId",
                        column: x => x.LekcijaId,
                        principalTable: "Lekcije",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Materijali_InstruktorId",
                table: "Materijali",
                column: "InstruktorId");

            migrationBuilder.CreateIndex(
                name: "IX_Materijali_LekcijaId",
                table: "Materijali",
                column: "LekcijaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Materijali");
        }
    }
}
