using Microsoft.AspNetCore.Mvc;
using Parque.Aplicacion.DTOS.Gamificacion;
using Parque.Aplicacion.Servicios.Gamificacion;

namespace Parque.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class GamificacionController(IServicioPuntuacion servicioPuntuacion) : ControllerBase
{
    [HttpPost("calcular-puntos/{registroVisitaId}")]
    public IActionResult CalcularPuntos(int registroVisitaId)
    {
        try
        {
            servicioPuntuacion.CalcularYRegistrarPuntos(registroVisitaId);
            return Ok(new { mensaje = "Puntos calculados y registrados exitosamente" });
        }
        catch(InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch(Exception ex)
        {
            return StatusCode(500, new { error = "Error interno del servidor", detalle = ex.Message });
        }
    }

    [HttpGet("ranking/diario")]
    public IActionResult ObtenerRankingDiario([FromQuery] DateTime? fecha, [FromQuery] int top = 10)
    {
        try
        {
            if(top <= 0)
            {
                return BadRequest(new { error = "El parámetro 'top' debe ser mayor a 0" });
            }

            var ranking = servicioPuntuacion.ObtenerRankingDiario(fecha, top);
            return Ok(new
            {
                fecha = fecha?.Date ?? DateTime.Today,
                totalVisitantes = ranking.Count,
                ranking = ranking
            });
        }
        catch(Exception ex)
        {
            return StatusCode(500, new { error = "Error interno del servidor", detalle = ex.Message });
        }
    }

    [HttpGet("estrategias")]
    public IActionResult ListarEstrategias()
    {
        try
        {
            var estrategias = servicioPuntuacion.ListarEstrategias();
            return Ok(new { estrategias = estrategias });
        }
        catch(Exception ex)
        {
            return StatusCode(500, new { error = "Error interno del servidor", detalle = ex.Message });
        }
    }

    [HttpPut("estrategias/cambiar")]
    public IActionResult CambiarEstrategiaActiva([FromBody] CambiarEstrategiaRequest request)
    {
        try
        {
            if(request == null || string.IsNullOrWhiteSpace(request.NombreEstrategia))
            {
                return BadRequest(new { error = "El request o el nombre de la estrategia es requerido" });
            }

            servicioPuntuacion.CambiarEstrategiaActiva(request.NombreEstrategia);
            return Ok(new { mensaje = "Estrategia cambiada exitosamente", nuevaEstrategia = request.NombreEstrategia });
        }
        catch(ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch(Exception ex)
        {
            return StatusCode(500, new { error = "Error interno del servidor", detalle = ex.Message });
        }
    }

    [HttpGet("estrategias/activa")]
    public IActionResult ObtenerEstrategiaActiva()
    {
        try
        {
            var estrategiaActiva = servicioPuntuacion.ObtenerEstrategiaActiva();
            return Ok(new { estrategiaActiva = estrategiaActiva });
        }
        catch(Exception ex)
        {
            return StatusCode(500, new { error = "Error interno del servidor", detalle = ex.Message });
        }
    }
}
