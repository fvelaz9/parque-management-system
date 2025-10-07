using Microsoft.AspNetCore.Mvc;
using Parque.Aplicacion.Mappers;
using Parque.Aplicacion.Servicios;
using Parque.Dominio.Usuarios;
using Parque.WebApi.Controllers.Sesiones.Models;
using Parque.WebApi.Filtros;

namespace Parque.WebApi.Controllers.Sesiones;

[Route("api/sesiones")]
[ApiController]
public class SesionController(IServicioSesion servicioSesion) : ControllerBase
{
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest login)
    {
        var sesion = servicioSesion.AgregarSesion(login.Email, login.Password);
        var cuenta = servicioSesion.ObtenerUsuarioSesion(sesion.Token);

        var response = new LoginResponse
        {
            Token = sesion.Token,
            Cuenta = cuenta.ToDto()
        };

        return Ok(new ResponseDto
        {
            Content = response,
            ExecutionSuccessful = true,
            Message = "Login exitoso"
        });
    }

    [HttpGet("perfil")]
    [AuthorizationFilter("any")]
    public IActionResult ObtenerPerfil()
    {
        // El usuario fue agregado al HttpContext.Items por el AuthorizationFilter
        var cuenta = HttpContext.Items["user"] as Cuenta
            ?? throw new InvalidOperationException("Usuario no encontrado");

        return Ok(new ResponseDto
        {
            Content = cuenta.ToDto(),
            ExecutionSuccessful = true,
            Message = "Perfil obtenido correctamente"
        });
    }

    [HttpPost("logout")]
    [AuthorizationFilter("any")]
    public IActionResult Logout()
    {
        return Ok(new ResponseDto
        {
            ExecutionSuccessful = true,
            Message = "Sesión cerrada correctamente"
        });
    }
}
