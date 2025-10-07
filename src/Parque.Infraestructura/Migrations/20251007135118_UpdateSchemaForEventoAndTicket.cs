using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Parque.Infraestructura.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSchemaForEventoAndTicket : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Atracciones",
                table: "Eventos");

            migrationBuilder.CreateTable(
                name: "EventoAtracciones",
                columns: table => new
                {
                    AtraccionesId = table.Column<int>(type: "int", nullable: false),
                    EventoId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventoAtracciones", x => new { x.AtraccionesId, x.EventoId });
                    table.ForeignKey(
                        name: "FK_EventoAtracciones_Atracciones_AtraccionesId",
                        column: x => x.AtraccionesId,
                        principalTable: "Atracciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventoAtracciones_Eventos_EventoId",
                        column: x => x.EventoId,
                        principalTable: "Eventos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventoAtracciones_EventoId",
                table: "EventoAtracciones",
                column: "EventoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventoAtracciones");

            migrationBuilder.AddColumn<string>(
                name: "Atracciones",
                table: "Eventos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
