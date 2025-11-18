using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Parque.Infraestructura.Migrations
{
    /// <inheritdoc />
    public partial class AddHistorialPuntuacionRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HistorialPuntuaciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FechaHora = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OrigenPuntos = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    EstrategiaActiva = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Puntos = table.Column<int>(type: "int", nullable: false),
                    VisitanteId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistorialPuntuaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HistorialPuntuaciones_Visitantes_VisitanteId",
                        column: x => x.VisitanteId,
                        principalTable: "Visitantes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HistorialPuntuaciones_VisitanteId",
                table: "HistorialPuntuaciones",
                column: "VisitanteId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HistorialPuntuaciones");
        }
    }
}
