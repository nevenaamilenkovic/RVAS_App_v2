using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RvasApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class TabelaKurs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Kursevi",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Naziv = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Opis = table.Column<string>(type: "nvarchar(525)", maxLength: 525, nullable: true),
                    DatumPocetka = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DatumZavrsetka = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MaksimalanBrojPolaznika = table.Column<int>(type: "int", nullable: true),
                    InstruktorId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kursevi", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Kursevi_AspNetUsers_InstruktorId",
                        column: x => x.InstruktorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Kursevi_InstruktorId",
                table: "Kursevi",
                column: "InstruktorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Kursevi");
        }
    }
}
