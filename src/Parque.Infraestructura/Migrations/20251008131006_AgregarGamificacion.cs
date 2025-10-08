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
            // Add a temporary column for the new type
            migrationBuilder.AddColumn<Guid>(
                name: "CuentaId_Temp",
                table: "Tickets",
                type: "uniqueidentifier",
                nullable: true);

            // Drop the old column
            migrationBuilder.DropColumn(
                name: "CuentaId",
                table: "Tickets");

            // Rename the temporary column to CuentaId
            migrationBuilder.RenameColumn(
                name: "CuentaId_Temp",
                table: "Tickets",
                newName: "CuentaId");

            // Make the column non-nullable with a default value
            migrationBuilder.AlterColumn<Guid>(
                name: "CuentaId",
                table: "Tickets",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

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

            // Add a temporary column for the old type
            migrationBuilder.AddColumn<int>(
                name: "CuentaId_Temp",
                table: "Tickets",
                type: "int",
                nullable: true);

            // Drop the GUID column
            migrationBuilder.DropColumn(
                name: "CuentaId",
                table: "Tickets");

            // Rename the temporary column back
            migrationBuilder.RenameColumn(
                name: "CuentaId_Temp",
                table: "Tickets",
                newName: "CuentaId");

            // Make the column non-nullable
            migrationBuilder.AlterColumn<int>(
                name: "CuentaId",
                table: "Tickets",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
