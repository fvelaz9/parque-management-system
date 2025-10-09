using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Parque.Infraestructura.Migrations
{
    /// <inheritdoc />
    public partial class AgregarConfiguracionFechaHora : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Cuentas_VisitanteId",
                table: "Cuentas");

            migrationBuilder.CreateTable(
                name: "ConfiguracionFechaHora",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FechaHoraConfigurada = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfiguracionFechaHora", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cuentas_VisitanteId",
                table: "Cuentas",
                column: "VisitanteId",
                unique: true,
                filter: "[VisitanteId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConfiguracionFechaHora");

            migrationBuilder.DropIndex(
                name: "IX_Cuentas_VisitanteId",
                table: "Cuentas");

            migrationBuilder.CreateIndex(
                name: "IX_Cuentas_VisitanteId",
                table: "Cuentas",
                column: "VisitanteId");
        }
    }
}
