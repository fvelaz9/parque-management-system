using Microsoft.AspNetCore.Mvc;
using Parque.Aplicacion.DTOs.RecompensasDtos;
using Parque.Aplicacion.Servicios.Recompensas;
using Parque.WebApi.Filtros;

namespace Parque.WebApi.Controllers;

[ApiController]
[Route("api/recompensas")]
public class RecompensasController(IServicioRecompensa servicioRecompensa) : ControllerBase
{
    [HttpPost]
    [AuthorizationFilter("Administrador")]
    public IActionResult CrearRecompensa([FromBody] RecompensaDto dto)
    {
        var recompensa = servicioRecompensa.CrearRecompensa(dto);
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
        var recompensa = servicioRecompensa.ActualizarRecompensa(id, dto);
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
        var recompensas = servicioRecompensa.ObtenerRecompensas();
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
        var recompensa = servicioRecompensa.ObtenerRecompensaPorId(id);
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
        var historial = servicioRecompensa.CanjearRecompensa(request);
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
        var historial = servicioRecompensa.ObtenerHistorialCanjes(visitanteId);
        return Ok(new
        {
            visitanteId,
            totalCanjes = historial.Count,
            historial = historial.Select(h => new
            {
                h.Id,
                h.VisitanteId,
                h.RecompensaId,
                h.PuntosCanjeados,
                h.FechaCanje
            })
        });
    }
}
