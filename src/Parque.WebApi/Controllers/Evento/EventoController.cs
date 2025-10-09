using Microsoft.AspNetCore.Mvc;
using Parque.Aplicacion.Servicios;
using Parque.Aplicacion.Servicios.Atracciones;
using Parque.WebApi.Controllers.Evento.Modelos;
using Parque.WebApi.Filtros;

namespace Parque.WebApi.Controllers.Evento;

[ApiController]
[Route("api/eventos")]
public class EventoController(IServicioEvento servicioEvento, IServicioAtracciones servicioAtraccion) : ControllerBase
{
    [HttpPost]
    [AuthorizationFilter("Administrador")]
    public IActionResult Crear([FromBody] CreateEventoRequest request)
    {
        var evento = new Dominio.Evento(
            request.Titulo,
            request.Descripcion,
            request.Inicio,
            request.Fin,
            request.AforoMaximo,
            request.CostoAdicional,
            request.Estado);
        var atracciones = servicioAtraccion.ObtenerPorIds(request.AtraccionIds);
        evento.Atracciones = atracciones.ToList();
        evento = servicioEvento.AgregarEvento(evento);

        return CreatedAtAction(nameof(ObtenerPorId), new { eventoId = evento.Id }, new EventoOutDto(evento));
    }

    [HttpGet]
    [AuthorizationFilter("any")]
    public IActionResult Listar()
    {
        var eventos = servicioEvento.ListarEventos().Select(e => new EventoOutDto(e)).ToList();
        return Ok(eventos);
    }

    [HttpGet("{eventoId}")]
    [AuthorizationFilter("any")]
    public IActionResult ObtenerPorId(int eventoId)
    {
        var evento = servicioEvento.ObtenerEventoPorId(eventoId);
        return Ok(new EventoOutDto(evento));
    }

    [HttpDelete("{eventoId}")]
    [AuthorizationFilter("Administrador")]
    public IActionResult Eliminar(int eventoId)
    {
        servicioEvento.EliminarEventoPorId(eventoId);
        return NoContent();
    }
}
