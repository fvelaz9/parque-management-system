using Microsoft.AspNetCore.Mvc;
using Parque.Aplicacion.DTOs.Gamificacion;
using Parque.Aplicacion.Servicios.Gamificacion;
using Parque.WebApi.Filtros;

namespace Parque.WebApi.Controllers;

[ApiController]
[Route("api/gamificacion")]
public class GamificacionController(IServicioPuntuacion servicioPuntuacion) : ControllerBase
{
    [HttpGet("ranking/diario")]
    [AuthorizationFilter("any")]
    public IActionResult ObtenerRankingDiario([FromQuery] DateTime? fecha, [FromQuery] int top = 10)
    {
        if(top <= 0)
        {
            return BadRequest(new { mensaje = "El parámetro 'top' debe ser mayor a 0" });
        }

        var ranking = servicioPuntuacion.ObtenerRankingDiario(fecha, top);
        return Ok(new
        {
            fecha = fecha?.Date ?? DateTime.Today,
            totalVisitantes = ranking.Count,
            ranking
        });
    }

    [HttpGet("estrategias")]
    [AuthorizationFilter("Administrador")]
    public IActionResult ListarEstrategias()
    {
        var estrategias = servicioPuntuacion.ListarEstrategias();
        return Ok(new { estrategias });
    }

    [HttpPut("estrategias/activa")]
    [AuthorizationFilter("Administrador")]
    public IActionResult CambiarEstrategiaActiva([FromBody] CambiarEstrategiaRequest request)
    {
        servicioPuntuacion.CambiarEstrategiaActiva(request.NombreEstrategia);
        return Ok(new { mensaje = "Estrategia cambiada exitosamente", nuevaEstrategia = request.NombreEstrategia });
    }

    [HttpGet("estrategias/activa")]
    [AuthorizationFilter("Administrador")]
    public IActionResult ObtenerEstrategiaActiva()
    {
        var estrategiaActiva = servicioPuntuacion.ObtenerEstrategiaActiva();
        return Ok(new { estrategiaActiva });
    }
}
