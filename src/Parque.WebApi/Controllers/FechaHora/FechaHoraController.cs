using Microsoft.AspNetCore.Mvc;
using Parque.Aplicacion.Servicios;
using Parque.WebApi.Controllers.FechaHora.Models;

namespace Parque.WebApi.Controllers.FechaHora;
[Route("api/fecha-hora")]
[ApiController]
public class FechaHoraController(IServicioFechaHora servicioFecha) : ControllerBase
{
    [HttpGet]
    public IActionResult ObtenerFechaActual()
    {
        var currentTime = servicioFecha.ObtenerFechaActual();
        return Ok(new { datetime = currentTime });
    }

    [HttpPut]
    public IActionResult ConfigurarFecha([FromBody] ConfigurarFechaRequest request)
    {
        if(!DateTime.TryParse(request.FechaHora, out var fechaPersonalizada))
        {
            return BadRequest("Formato de fecha inválido. Use 'YYYY-MM-DDTHH:MM'.");
        }

        servicioFecha.ConfigurarFecha(fechaPersonalizada);
        return Ok(new { mensaje = "Fecha configurada exitosamente" });
    }

    [HttpDelete]
    public IActionResult ResetearAFechaSistema()
    {
        servicioFecha.ResetearAFechaSistema();
        return Ok(new { mensaje = "Fecha reseteada al sistema exitosamente" });
    }
}
