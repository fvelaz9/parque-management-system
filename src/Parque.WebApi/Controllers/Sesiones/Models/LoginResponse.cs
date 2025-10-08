using Parque.Aplicacion.DTOs.Usuarios;

namespace Parque.WebApi.Controllers.Sesiones.Models;
public record class LoginResponse
{
    public required string Token { get; set; }
    public required CuentaDto Cuenta { get; set; }
}
