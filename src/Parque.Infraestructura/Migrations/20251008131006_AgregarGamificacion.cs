using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Parque.Infraestructura.Migrations
{
    /// <inheritdoc />
    public partial class AgregarGamificacion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "CuentaId",
                table: "Tickets",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateTable(
                name: "ConfiguracionesEstrategia",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EstrategiaActiva = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfiguracionesEstrategia", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PuntuacionesVisitantes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VisitanteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PuntosDiarios = table.Column<int>(type: "int", nullable: false),
                    PuntosTotales = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PuntuacionesVisitantes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PuntuacionesVisitantes_VisitanteId_Fecha",
                table: "PuntuacionesVisitantes",
                columns: new[] { "VisitanteId", "Fecha" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConfiguracionesEstrategia");

            migrationBuilder.DropTable(
                name: "PuntuacionesVisitantes");

            migrationBuilder.AlterColumn<int>(
                name: "CuentaId",
                table: "Tickets",
                type: "int",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");
        }
    }
}
