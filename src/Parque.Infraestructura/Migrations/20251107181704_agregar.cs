using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Parque.Infraestructura.Migrations
{
    /// <inheritdoc />
    public partial class agregar : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MantenimientosPreventivos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AtraccionId = table.Column<int>(type: "int", nullable: false),
                    FechaProgramada = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HoraInicio = table.Column<TimeSpan>(type: "time", nullable: false),
                    DuracionEstimada = table.Column<TimeSpan>(type: "time", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IncidenciaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MantenimientosPreventivos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MantenimientosPreventivos_Incidencias_IncidenciaId",
                        column: x => x.IncidenciaId,
                        principalTable: "Incidencias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MantenimientosPreventivos_IncidenciaId",
                table: "MantenimientosPreventivos",
                column: "IncidenciaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MantenimientosPreventivos");
        }
    }
}
