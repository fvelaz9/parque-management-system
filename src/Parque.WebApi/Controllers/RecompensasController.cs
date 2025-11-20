using Microsoft.AspNetCore.Mvc;
using Parque.Aplicacion.DTOs.RecompensasDtos;
using Parque.Aplicacion.Servicios.Recompensas;
using Parque.WebApi.Filtros;

namespace Parque.WebApi.Controllers;

[ApiController]
[Route("api/recompensas")]
public class RecompensasController(IServicioRecompensa servicioRecompensa) : ControllerBase
{
    private readonly IServicioRecompensa _servicioRecompensa = servicioRecompensa ?? throw new ArgumentNullException(nameof(servicioRecompensa));

    [HttpPost]
    [AuthorizationFilter("Administrador")]
    public IActionResult CrearRecompensa([FromBody] RecompensaDto dto)
    {
        var recompensa = _servicioRecompensa.CrearRecompensa(dto);
        return CreatedAtAction(
            nameof(ObtenerRecompensaPorId),
            new { id = recompensa.Id },
            new
            {
                mensaje = "Recompensa creada exitosamente",
                recompensa = new
                {
                    recompensa.Id,
                    recompensa.Nombre,
                    recompensa.Descripcion,
                    recompensa.CostoEnPuntos,
                    recompensa.CantidadDisponible,
                    recompensa.NivelMembresiaRequerido,
                    recompensa.FechaCreacion
                }
            });
    }

    [HttpPut("{id}")]
    [AuthorizationFilter("Administrador")]
    public IActionResult ActualizarRecompensa(Guid id, [FromBody] RecompensaDto dto)
    {
        var recompensa = _servicioRecompensa.ActualizarRecompensa(id, dto);
        return Ok(new
        {
            mensaje = "Recompensa actualizada exitosamente",
            recompensa = new
            {
                recompensa.Id,
                recompensa.Nombre,
                recompensa.Descripcion,
                recompensa.CostoEnPuntos,
                recompensa.CantidadDisponible,
                recompensa.NivelMembresiaRequerido
            }
        });
    }

    [HttpGet]
    [AuthorizationFilter("any")]
    public IActionResult ObtenerRecompensas()
    {
        var recompensas = _servicioRecompensa.ObtenerRecompensas();
        return Ok(new
        {
            total = recompensas.Count,
            recompensas = recompensas.Select(r => new
            {
                r.Id,
                r.Nombre,
                r.Descripcion,
                r.CostoEnPuntos,
                r.CantidadDisponible,
                r.NivelMembresiaRequerido,
                r.FechaCreacion
            })
        });
    }

    [HttpGet("{id}")]
    [AuthorizationFilter("any")]
    public IActionResult ObtenerRecompensaPorId(Guid id)
    {
        var recompensa = _servicioRecompensa.ObtenerRecompensaPorId(id);
        return Ok(new
        {
            recompensa.Id,
            recompensa.Nombre,
            recompensa.Descripcion,
            recompensa.CostoEnPuntos,
            recompensa.CantidadDisponible,
            recompensa.NivelMembresiaRequerido,
            recompensa.FechaCreacion
        });
    }

    [HttpPost("canjear")]
    [AuthorizationFilter("Visitante")]
    public IActionResult CanjearRecompensa([FromBody] CanjearRecompensaRequest request)
    {
        var historial = _servicioRecompensa.CanjearRecompensa(request);
        return Ok(new
        {
            mensaje = "Recompensa canjeada exitosamente",
            canje = new
            {
                historial.Id,
                historial.VisitanteId,
                historial.RecompensaId,
                historial.PuntosCanjeados,
                historial.FechaCanje
            }
        });
    }

    [HttpGet("historial/{visitanteId}")]
    [AuthorizationFilter("any")]
    public IActionResult ObtenerHistorialCanjes(Guid visitanteId)
    {
        var historial = _servicioRecompensa.ObtenerHistorialCanjes(visitanteId);
        return Ok(new
        {
            visitanteId,
            totalCanjes = historial.Count,
            historial = historial.Select(h => new
            {
                h.Id,
                h.VisitanteId,
                h.RecompensaId,
                h.NombreRecompensa,
                h.PuntosCanjeados,
                h.FechaCanje
            })
        });
    }

    [HttpDelete("{id}")]
    [AuthorizationFilter("Administrador")]
    public IActionResult EliminarRecompensa(Guid id)
    {
        try
        {
            _servicioRecompensa.EliminarRecompensa(id);
            return Ok(new { mensaje = "Recompensa eliminada exitosamente" });
        }
        catch(InvalidOperationException ex)
        {
            return NotFound(new { mensaje = ex.Message });
        }
    }

    [HttpGet("puntos/{visitanteId}")]
    public IActionResult ObtenerPuntos(Guid visitanteId)
    {
        var puntos = _servicioRecompensa.ObtenerPuntosTotalesVisitante(visitanteId);
        return Ok(new { puntos });
    }
}
