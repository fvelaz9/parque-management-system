namespace Parque.WebApi.Controllers.Sesiones.Models;
public record class LoginRequest
{
    public required string Email { get; set; }
    public required string Password { get; set; }
}
