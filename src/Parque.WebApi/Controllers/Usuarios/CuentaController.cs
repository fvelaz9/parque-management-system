using Microsoft.AspNetCore.Mvc;
using Parque.Aplicacion.DTOs.Usuarios;
using Parque.Aplicacion.Servicios;

namespace Parque.WebApi.Controllers.Usuarios;
[Route("api/cuentas")]
[ApiController]
public class CuentaController(IServicioCuenta servicioCuenta) : ControllerBase
{
    [HttpPost("registro")]
    public IActionResult RegistrarVisitante([FromBody] RegistrarVisitanteDto dto)
    {
        throw new NotImplementedException();
    }
}
