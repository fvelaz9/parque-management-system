using Microsoft.AspNetCore.Mvc;
using Parque.Aplicacion.DTOs.Gamificacion;
using Parque.Aplicacion.Servicios.Gamificacion;
using Parque.WebApi.Filtros;

namespace Parque.WebApi.Controllers.Estrategias;

[Route("api/estrategias")]
[ApiController]
public class EstrategiasController(IServicioPuntuacion servicioPuntuacion) : ControllerBase
{
    private readonly IServicioPuntuacion _servicioPuntuacion = servicioPuntuacion;

    [HttpGet("disponibles")]
    [AuthorizationFilter("Administrador")]
    public IActionResult ObtenerEstrategiasDisponibles()
    {
        var estrategias = _servicioPuntuacion.ListarEstrategias();

        return Ok(new ResponseDto
        {
            Content = estrategias,
            ExecutionSuccessful = true,
            Message = $"Se encontraron {estrategias.Count} estrategias"
        });
    }

    [HttpGet("activa")]
    [AuthorizationFilter("Administrador")]
    public IActionResult ObtenerEstrategiaActiva()
    {
        var estrategiaActiva = _servicioPuntuacion.ObtenerEstrategiaActiva();

        return Ok(new ResponseDto
        {
            Content = new { estrategiaActiva },
            ExecutionSuccessful = true,
            Message = "Estrategia activa obtenida correctamente"
        });
    }

    [HttpPut("activa")]
    [AuthorizationFilter("Administrador")]
    public IActionResult CambiarEstrategiaActiva([FromBody] CambiarEstrategiaRequest request)
    {
        if(string.IsNullOrWhiteSpace(request?.NombreEstrategia))
        {
            return BadRequest(new ResponseDto
            {
                Content = null,
                ExecutionSuccessful = false,
                Message = "El nombre de la estrategia es requerido"
            });
        }

        try
        {
            _servicioPuntuacion.CambiarEstrategiaActiva(request.NombreEstrategia);

            return Ok(new ResponseDto
            {
                Content = new { nuevaEstrategia = request.NombreEstrategia },
                ExecutionSuccessful = true,
                Message = $"Estrategia cambiada a: {request.NombreEstrategia}"
            });
        }
        catch(ArgumentException ex)
        {
            return BadRequest(new ResponseDto
            {
                Content = null,
                ExecutionSuccessful = false,
                Message = ex.Message
            });
        }
        catch(InvalidOperationException ex)
        {
            return BadRequest(new ResponseDto
            {
                Content = null,
                ExecutionSuccessful = false,
                Message = ex.Message
            });
        }
    }

    [HttpPost("plugins/recargar")]
    [AuthorizationFilter("Administrador")]
    public IActionResult RecargarPlugins()
    {
        try
        {
            _servicioPuntuacion.RecargarPlugins();
            var estrategias = _servicioPuntuacion.ListarEstrategias();

            return Ok(new ResponseDto
            {
                Content = estrategias,
                ExecutionSuccessful = true,
                Message = $"Plugins recargados. Total de estrategias: {estrategias.Count}"
            });
        }
        catch(Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new ResponseDto
            {
                Content = null,
                ExecutionSuccessful = false,
                Message = $"Error al recargar plugins: {ex.Message}"
            });
        }
    }
}
