using Microsoft.AspNetCore.Mvc;
using Parque.Aplicacion.Servicios.Atracciones;
using Parque.Dominio.Atracciones;
using Parque.WebApi.Filtros;

namespace Parque.WebApi.Controllers;

[ApiController]
[Route("api/atracciones")]
public class AtraccionesController(IServicioAtracciones servicioAtracciones) : ControllerBase
{
    [HttpGet]
    [AuthorizationFilter("any")]
    public IActionResult GetAll()
    {
        return Ok(servicioAtracciones.ListarAtracciones());
    }

    [HttpGet("{id}")]
    [AuthorizationFilter("any")]
    public IActionResult GetById(int id)
    {
        var atraccion = servicioAtracciones.BuscarAtraccion(id);
        return atraccion == null ? NotFound() : Ok(atraccion);
    }

    [HttpPost]
    [AuthorizationFilter("Administrador")]
    public IActionResult Create([FromBody] AtraccionParque atraccion)
    {
        var creada = servicioAtracciones.CrearAtraccion(
            atraccion.Nombre,
            atraccion.Tipo,
            atraccion.EdadMinima,
            atraccion.Capacidad,
            atraccion.Descripcion);
        return CreatedAtAction(nameof(GetById), new { id = creada.Id }, creada);
    }

    [HttpPut("{id}")]
    [AuthorizationFilter("Administrador")]
    public IActionResult Update(int id, [FromBody] AtraccionParque atraccion)
    {
        var modificada = servicioAtracciones.ModificarAtraccion(
            id,
            atraccion.Nombre,
            atraccion.Tipo,
            atraccion.EdadMinima,
            atraccion.Capacidad,
            atraccion.Descripcion);
        return Ok(modificada);
    }

    [HttpDelete("{id}")]
    [AuthorizationFilter("Administrador")]
    public IActionResult Delete(int id)
    {
        servicioAtracciones.EliminarAtraccion(id);
        return NoContent();
    }

    [HttpGet("reporte-uso")]
    [AuthorizationFilter("Administrador")]
    public IActionResult ReporteUsoAtracciones([FromQuery] DateTime desde, [FromQuery] DateTime hasta)
    {
        if(desde > hasta)
        {
            return BadRequest(new { mensaje = "La fecha 'desde' no puede ser mayor a 'hasta'" });
        }

        var reporte = servicioAtracciones.ObtenerReporteUso(desde, hasta);
        return Ok(reporte);
    }

    [HttpGet("{id}/aforo")]
    [AuthorizationFilter("any")]
    public IActionResult ObtenerAforo(int id)
    {
        var aforo = servicioAtracciones.ObtenerAforoActual(id);
        return Ok(aforo);
    }
}
