using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infraestrutura.Migrations
{
    /// <inheritdoc />
    public partial class alterandocamposdeinscriçao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Cor",
                table: "Inscricao",
                newName: "QuantidadeKm");

            migrationBuilder.RenameColumn(
                name: "Camisa",
                table: "Inscricao",
                newName: "QuantidadeKit");

            migrationBuilder.AlterColumn<string>(
                name: "TamanhoCamisa",
                table: "Inscricao",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1)",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "QuantidadeKm",
                table: "Inscricao",
                newName: "Cor");

            migrationBuilder.RenameColumn(
                name: "QuantidadeKit",
                table: "Inscricao",
                newName: "Camisa");

            migrationBuilder.AlterColumn<string>(
                name: "TamanhoCamisa",
                table: "Inscricao",
                type: "nvarchar(1)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
