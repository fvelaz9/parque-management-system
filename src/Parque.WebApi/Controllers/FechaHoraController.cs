using Microsoft.AspNetCore.Mvc;
using Parque.Aplicacion.Servicios;

namespace Parque.WebApi.Controllers;
[Route("api/[controller]")]
[ApiController]
public class FechaHoraController(IServicioFechaHora timeService) : ControllerBase
{
    private readonly IServicioFechaHora _servicioFecha = timeService;

    [HttpGet("current")]
    public IActionResult ObtenerFechaActual()
    {
        // Usas la instancia inyectada
        var currentTime = _servicioFecha.ObtenerFechaActual();
        return Ok(new { datetime = currentTime });
    }
}
