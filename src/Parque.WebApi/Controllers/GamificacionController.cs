using Microsoft.AspNetCore.Mvc;
using Parque.Aplicacion.Servicios.Gamificacion;
using Parque.WebApi.Filtros;

namespace Parque.WebApi.Controllers;

[ApiController]
[Route("api/gamificacion")]
public class GamificacionController(IServicioPuntuacion servicioPuntuacion) : ControllerBase
{
    private readonly IServicioPuntuacion _servicioPuntuacion = servicioPuntuacion;

    [HttpGet("ranking/diario")]
    [AuthorizationFilter("any")]
    [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status400BadRequest)]
    public IActionResult ObtenerRankingDiario([FromQuery] DateTime? fecha, [FromQuery] int top = 10)
    {
        if(top <= 0)
        {
            return BadRequest(new ResponseDto
            {
                Content = null,
                ExecutionSuccessful = false,
                Message = "El parámetro 'top' debe ser mayor a 0"
            });
        }

        var ranking = _servicioPuntuacion.ObtenerRankingDiario(fecha, top);

        return Ok(new ResponseDto
        {
            Content = new
            {
                fecha = fecha?.Date ?? DateTime.Today,
                totalVisitantes = ranking.Count,
                ranking
            },
            ExecutionSuccessful = true,
            Message = $"Ranking obtenido correctamente ({ranking.Count} visitantes)"
        });
    }

    [HttpGet("historial")]
    [AuthorizationFilter("any")]
    [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status404NotFound)]
    public ActionResult<ResponseDto> ObtenerHistorialPuntuaciones([FromQuery] Guid visitanteId)
    {
        try
        {
            var historial = _servicioPuntuacion.ObtenerHistorialVisitante(visitanteId);
            return Ok(new ResponseDto
            {
                Content = historial,
                ExecutionSuccessful = true,
                Message = $"Historial obtenido ({historial.Count} registros)"
            });
        }
        catch(InvalidOperationException ex)
        {
            return NotFound(new ResponseDto
            {
                Content = null,
                ExecutionSuccessful = false,
                Message = ex.Message
            });
        }
    }
}
