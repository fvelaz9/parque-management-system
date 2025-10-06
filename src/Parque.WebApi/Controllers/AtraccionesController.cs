using Microsoft.AspNetCore.Mvc;
using Parque.Aplicacion.DTOS;
using Parque.Aplicacion.Servicios.Atracciones;
using Parque.Dominio.Atracciones;

namespace Parque.WebApi.Controllers;

[ApiController]
[Route("api/atracciones")]
public class AtraccionesController(IServicioAtracciones service) : ControllerBase
{
    private readonly IServicioAtracciones _service = service;

    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(_service.ListarAtracciones());
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var atraccion = _service.BuscarAtraccion(id);
        return atraccion == null ? NotFound() : Ok(atraccion);
    }

    [HttpPost]
    public IActionResult Create([FromBody] AtraccionParque atraccion)
    {
        var creada = _service.CrearAtraccion(
            atraccion.Nombre,
            atraccion.Tipo,
            atraccion.EdadMinima,
            atraccion.Capacidad,
            atraccion.Descripcion);
        return CreatedAtAction(nameof(GetById), new { id = creada.Id }, creada);
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] AtraccionParque atraccion)
    {
        _service.ModificarAtraccion(
            id,
            atraccion.Nombre,
            atraccion.Tipo,
            atraccion.EdadMinima,
            atraccion.Capacidad,
            atraccion.Descripcion);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        _service.EliminarAtraccion(id);
        return NoContent();
    }

    [HttpPost("{id}/ingresos")]
    public IActionResult RegistrarIngresos(int id, [FromBody] RegistroIngresoDto dto)
    {
        try
        {
            var registro = _service.RegistrarIngreso(dto.Identificador, id);
            return Ok(registro);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }
    
    [HttpPost("{id}/egresos")]
    public IActionResult RegistrarEgreso(int id, [FromBody] RegistroIngresoDto dto)
    {
        try
        {
            var registro = _service.RegistrarEgreso(dto.Identificador, id);
            return Ok(registro);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }
    
    [HttpGet("atracciones")]
    public IActionResult ReporteUsoAtracciones([FromQuery] DateTime desde, [FromQuery] DateTime hasta)
    {
        if (desde > hasta)
        {
            return BadRequest(new { mensaje = "La fecha 'desde' no puede ser mayor a 'hasta'" });
        }

        var reporte = _service.ObtenerReporteUso(desde, hasta);
        return Ok(reporte);
    }
}
