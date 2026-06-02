using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infraestrutura.Migrations
{
    /// <inheritdoc />
    public partial class update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Cor",
                table: "Evento");

            migrationBuilder.DropColumn(
                name: "TamanhoCamisa",
                table: "Evento");

            migrationBuilder.AddColumn<int>(
                name: "Camisa",
                table: "Inscricao",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Cor",
                table: "Inscricao",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TamanhoCamisa",
                table: "Inscricao",
                type: "nvarchar(1)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Camisa",
                table: "Inscricao");

            migrationBuilder.DropColumn(
                name: "Cor",
                table: "Inscricao");

            migrationBuilder.DropColumn(
                name: "TamanhoCamisa",
                table: "Inscricao");

            migrationBuilder.AddColumn<string>(
                name: "Cor",
                table: "Evento",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TamanhoCamisa",
                table: "Evento",
                type: "nvarchar(1)",
                nullable: true);
        }
    }
}
