using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infraestrutura.Migrations
{
    /// <inheritdoc />
    public partial class updateeventorepo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QuantidadeKit",
                table: "Inscricao");

            migrationBuilder.RenameColumn(
                name: "DataDeInscricaoDousuario",
                table: "Inscricao",
                newName: "DataDeInscricao");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DataDeInscricao",
                table: "Inscricao",
                newName: "DataDeInscricaoDousuario");

            migrationBuilder.AddColumn<int>(
                name: "QuantidadeKit",
                table: "Inscricao",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
