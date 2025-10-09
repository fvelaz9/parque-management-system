using Microsoft.AspNetCore.Mvc;
using Parque.Aplicacion.DTOs;
using Parque.Aplicacion.Servicios.Incidencias;
using Parque.WebApi.Filtros;

namespace Parque.WebApi.Controllers;

[ApiController]
[Route("api/incidencias")]
public class IncidenciasController(IServicioIncidencia servicio) : ControllerBase
{
    [HttpPost]
    [AuthorizationFilter("Operador")]
    public IActionResult CrearIncidencia([FromBody] CrearIncidenciaRequest request)
    {
        var incidencia = servicio.CrearIncidencia(request);
        return CreatedAtAction(nameof(VerificarDisponibilidad),
            new { atraccionId = incidencia.AtraccionId },
            incidencia);
    }

    [HttpDelete("{incidenciaId}")]
    [AuthorizationFilter("Operador")]
    public IActionResult ResolverIncidencia(int incidenciaId)
    {
        servicio.ResolverIncidencia(incidenciaId);
        return NoContent();
    }

    [HttpGet("atraccion/{atraccionId}/disponible")]
    [AuthorizationFilter("Operador")]
    public IActionResult VerificarDisponibilidad(int atraccionId)
    {
        var disponible = servicio.EstaDisponible(atraccionId);
        return Ok(new { atraccionId, disponible });
    }
}
