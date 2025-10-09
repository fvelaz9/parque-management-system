using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Parque.Infraestructura.Migrations
{
    /// <inheritdoc />
    public partial class CambiarPasswordHashAPlanText : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "Cuentas");

            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "Cuentas",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Password",
                table: "Cuentas");

            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                table: "Cuentas",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
