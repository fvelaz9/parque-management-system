using Microsoft.AspNetCore.Mvc;
using Parque.Aplicacion.Servicios;
using Parque.WebApi.Controllers.FechaHora.Models;

namespace Parque.WebApi.Controllers.FechaHora;
[Route("api/[controller]")]
[ApiController]
public class FechaHoraController(IServicioFechaHora timeService) : ControllerBase
{
    private readonly IServicioFechaHora _servicioFecha = timeService;

    [HttpGet("actual")]
    public IActionResult ObtenerFechaActual()
    {
        var currentTime = _servicioFecha.ObtenerFechaActual();
        return Ok(new { datetime = currentTime });
    }

    [HttpPost("configurar")]
    public IActionResult ConfigurarFecha([FromBody] ConfigurarFechaRequest request)
    {
        if(!DateTime.TryParse(request.FechaHora, out var fechaPersonalizada))
        {
            return BadRequest("Formato de fecha inválido. Use 'YYYY-MM-DDTHH:MM'.");
        }

        _servicioFecha.ConfigurarFecha(fechaPersonalizada);
        return Ok(new { mensaje = "Fecha configurada exitosamente" });
    }

    [HttpPost("resetear")]
    public IActionResult ResetearAFechaSistema()
    {
        _servicioFecha.ResetearAFechaSistema();
        return Ok(new { mensaje = "Fecha reseteada al sistema exitosamente" });
    }
}
