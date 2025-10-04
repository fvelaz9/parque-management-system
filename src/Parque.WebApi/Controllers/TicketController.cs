using Microsoft.AspNetCore.Mvc;
using Parque.Aplicacion.DTOS;
using Parque.Aplicacion.Servicios.Ticket;
using Parque.Dominio;

namespace Parque.WebApi.Controllers;

[ApiController]
[Route("api/tickets")]
public class TicketController(IServicioTicket service) : ControllerBase
{
    private readonly IServicioTicket _service = service;

    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(_service.ListarTickets());
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var ticket = _service.BuscarTicket(id);
        return ticket == null ? NotFound() : Ok(ticket);
    }

    [HttpGet("codigo/{codigo}")]
    public IActionResult GetByCodigo(Guid codigo)
    {
        var ticket = _service.BuscarTicketPorCodigo(codigo);
        return ticket == null ? NotFound() : Ok(ticket);
    }

    [HttpPost]
    public IActionResult Create([FromBody] CrearTicketDto request)
    {
        var creado = _service.CrearTicket(
            request.CuentaId,
            request.FechaVisita,
            request.EventoId);
        return CreatedAtAction(nameof(GetById), new { id = creado.Id }, creado);
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] UpdateTicketDto request)
    {
        _service.ModificarTicket(
            id,
            request.CuentaId,
            request.FechaVisita,
            request.EventoId);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        _service.EliminarTicket(id);
        return NoContent();
    }
}
