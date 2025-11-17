using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Parque.Infraestructura.Migrations
{
    /// <inheritdoc />
    public partial class Recompensa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HistorialCanjes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VisitanteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RecompensaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PuntosCanjeados = table.Column<int>(type: "int", nullable: false),
                    FechaCanje = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistorialCanjes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Recompensas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CostoEnPuntos = table.Column<int>(type: "int", nullable: false),
                    CantidadDisponible = table.Column<int>(type: "int", nullable: false),
                    NivelMembresiaRequerido = table.Column<int>(type: "int", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recompensas", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HistorialCanjes_FechaCanje",
                table: "HistorialCanjes",
                column: "FechaCanje");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialCanjes_RecompensaId",
                table: "HistorialCanjes",
                column: "RecompensaId");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialCanjes_VisitanteId",
                table: "HistorialCanjes",
                column: "VisitanteId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HistorialCanjes");

            migrationBuilder.DropTable(
                name: "Recompensas");
        }
    }
}
