using Microsoft.AspNetCore.Mvc;
using Parque.Aplicacion.DTOs.Usuarios;
using Parque.Aplicacion.Servicios;
using Parque.Dominio.Usuarios;
using Parque.WebApi.Filtros;

namespace Parque.WebApi.Controllers.Usuarios;
[Route("api/cuentas")]
[ApiController]
public class CuentaController(IServicioCuenta servicioCuenta) : ControllerBase
{
    [HttpGet]
    [AuthorizationFilter("Administrador")]
    public IActionResult ObtenerTodas()
    {
        var cuentas = servicioCuenta.ObtenerTodas();

        return Ok(new ResponseDto
        {
            Content = cuentas,
            ExecutionSuccessful = true,
            Message = "Cuentas obtenidas exitosamente"
        });
    }

    [HttpPost("registro")]
    public IActionResult RegistrarVisitante([FromBody] RegistrarVisitanteDto dto)
    {
        var cuenta = servicioCuenta.RegistrarVisitante(dto);

        return Created($"/api/cuentas/{cuenta.Id}", new ResponseDto
        {
            Content = cuenta,
            ExecutionSuccessful = true,
            Message = "Visitante registrado exitosamente"
        });
    }

    [HttpPost]
    [AuthorizationFilter("Administrador")]
    public IActionResult CrearCuenta([FromBody] RegistrarCuentaDto dto)
    {
        var cuenta = servicioCuenta.CrearCuentaPorAdmin(dto);

        return Created($"/api/cuentas/{cuenta.Id}", new ResponseDto
        {
            Content = cuenta,
            ExecutionSuccessful = true,
            Message = "Cuenta creada exitosamente"
        });
    }

    [HttpPut("perfil")]
    [AuthorizationFilter("any")]
    public IActionResult ModificarPerfil([FromBody] ModificarPerfilDto dto)
    {
        var usuarioAutenticado = HttpContext.Items["user"] as Cuenta;

        if(usuarioAutenticado == null)
        {
            return Unauthorized(new ResponseDto
            {
                Content = null,
                ExecutionSuccessful = false,
                Message = "No se pudo identificar al usuario autenticado"
            });
        }

        servicioCuenta.ModificarPerfil(usuarioAutenticado.Id, dto);

        return Ok(new ResponseDto
        {
            Content = null,
            ExecutionSuccessful = true,
            Message = "Perfil modificado exitosamente"
        });
    }

    [HttpPatch("{id:guid}/membresia")]
    [AuthorizationFilter("Administrador")]
    public IActionResult CambiarNivelMembresia(Guid id, [FromBody] NivelMembresia nuevoNivel)
    {
        servicioCuenta.CambiarNivelMembresia(id, nuevoNivel);

        return Ok(new ResponseDto
        {
            Content = null,
            ExecutionSuccessful = true,
            Message = "Nivel de membresía actualizado exitosamente"
        });
    }
}
