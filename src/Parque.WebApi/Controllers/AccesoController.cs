using Microsoft.AspNetCore.Mvc;
using Parque.Aplicacion.DTOS;
using Parque.Aplicacion.Servicios.Acceso;

namespace Parque.WebApi.Controllers;

/// <summary>
/// Controlador para validar acceso a atracciones.
/// </summary>
[ApiController]
[Route("api/acceso")]
public class AccesoController : ControllerBase
{
    private readonly IServicioAcceso _servicio;

    public AccesoController(IServicioAcceso servicio)
    {
        _servicio = servicio;
    }

    [HttpPost("validar")]
    public IActionResult ValidarAcceso([FromBody] ValidarAccesoRequest request)
    {
        var resultado = _servicio.ValidarAcceso(request);

        if (resultado.AccesoPermitido)
        {
            return Ok(resultado);
        }
        else
        {
            return BadRequest(resultado);
        }
    }

    [HttpPost("{id}/ingresos")]
    public IActionResult RegistrarIngresos(int id, [FromBody] ValidarAccesoRequest dto, int edad)
    {
        try
        {
            var registro = _servicio.RegistrarIngreso(dto.CodigoTicket, id, edad);
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
            var registro = _servicio.RegistrarEgreso(codigoTicket, atraccionId);
            return Ok(new { mensaje = "Egreso registrado exitosamente", registro });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }
}
