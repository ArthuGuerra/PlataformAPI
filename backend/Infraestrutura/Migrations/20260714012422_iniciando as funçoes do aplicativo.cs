using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infraestrutura.Migrations
{
    /// <inheritdoc />
    public partial class iniciandoasfunçoesdoaplicativo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Turmas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NomeProfessor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HorarioDeInicioDaAula = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HorarioDeTerminoDaAula = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Esporte = table.Column<int>(type: "int", nullable: false),
                    QuantidadeDeAlunos = table.Column<int>(type: "int", nullable: false),
                    LocalDaAula = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Turmas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InscricoesAulas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UsuarioAppId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    TurmaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InscricoesAulas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InscricoesAulas_AspNetUsers_UsuarioAppId",
                        column: x => x.UsuarioAppId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_InscricoesAulas_Turmas_TurmaId",
                        column: x => x.TurmaId,
                        principalTable: "Turmas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InscricoesAulas_TurmaId",
                table: "InscricoesAulas",
                column: "TurmaId");

            migrationBuilder.CreateIndex(
                name: "IX_InscricoesAulas_UsuarioAppId",
                table: "InscricoesAulas",
                column: "UsuarioAppId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InscricoesAulas");

            migrationBuilder.DropTable(
                name: "Turmas");
        }
    }
}
