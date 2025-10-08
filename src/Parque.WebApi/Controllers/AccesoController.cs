using Microsoft.AspNetCore.Mvc;
using Parque.Aplicacion.DTOS;
using Parque.Aplicacion.Servicios.Acceso;
using Parque.Dominio.Usuarios;

namespace Parque.WebApi.Controllers;

[ApiController]
[Route("api/acceso")]
public class AccesoController(IServicioAcceso servicio) : ControllerBase
{
    [HttpPost("validar")]
    public IActionResult ValidarAcceso([FromBody] ValidarAccesoRequest request)
    {
        var resultado = servicio.ValidarAcceso(request);

        if(resultado.AccesoPermitido)
        {
            return Ok(resultado);
        }
        else
        {
            return BadRequest(resultado);
        }
    }

    [HttpPost("{id}/ingresos")]
    public IActionResult RegistrarIngresos(int id, [FromBody] ValidarAccesoRequest dto, Cuenta cuentaVisitante)
    {
        try
        {
            var registro = servicio.RegistrarIngreso(dto.CodigoTicket, id, cuentaVisitante);
            return Ok(registro);
        }
        catch(ArgumentException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    [HttpPost("atraccion/{atraccionId}/egreso/{codigoTicket}")]
    public IActionResult RegistrarEgreso(int atraccionId, Guid codigoTicket)
    {
        try
        {
            var registro = servicio.RegistrarEgreso(codigoTicket, atraccionId);
            return Ok(new { mensaje = "Egreso registrado exitosamente", registro });
        }
        catch(ArgumentException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }
}
