using Microsoft.AspNetCore.Mvc;
using Parque.Aplicacion.DTOs.Usuarios;
using Parque.Aplicacion.Servicios;
using Parque.WebApi.Filtros;

namespace Parque.WebApi.Controllers.Usuarios;
[Route("api/cuentas")]
[ApiController]
public class CuentaController(IServicioCuenta servicioCuenta) : ControllerBase
{
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
        throw new NotImplementedException();
    }
}
