using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infraestrutura.Migrations
{
    /// <inheritdoc />
    public partial class alterandoquantidadedekitsemeventosedevolvendoumkitsecancelar : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "QuantidadeDeCamisasDisponiveis",
                table: "Evento",
                newName: "QuantidadeDeKitsDisponiveis");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "QuantidadeDeKitsDisponiveis",
                table: "Evento",
                newName: "QuantidadeDeCamisasDisponiveis");
        }
    }
}
