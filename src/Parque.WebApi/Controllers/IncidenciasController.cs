using Microsoft.AspNetCore.Mvc;
using Parque.Aplicacion.DTOS;
using Parque.Aplicacion.Servicios.Incidencias;

namespace Parque.WebApi.Controllers;

[ApiController]
[Route("api/incidencias")]
public class IncidenciasController(IServicioIncidencia servicio) : ControllerBase
{
    [HttpPost]
    public IActionResult CrearIncidencia([FromBody] CrearIncidenciaRequest request)
    {
        try
        {
            var incidencia = servicio.CrearIncidencia(request);
            return Ok(incidencia);
        }
        catch(ArgumentException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    [HttpGet("atraccion/{atraccionId}/disponible")]
    public IActionResult VerificarDisponibilidad(int atraccionId)
    {
        try
        {
            var disponible = servicio.EstaDisponible(atraccionId);
            return Ok(new { atraccionId, disponible });
        }
        catch(ArgumentException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }
}
