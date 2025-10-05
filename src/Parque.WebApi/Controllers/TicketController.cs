using Microsoft.AspNetCore.Mvc;
using Parque.Aplicacion.DTOS;
using Parque.Aplicacion.Servicios.Ticket;
using Parque.Aplicacion.Servicios.Ticket; // 👈 namespace plural para evitar confusión
using Parque.Dominio;

namespace Parque.WebApi.Controllers;

[ApiController]
[Route("api/tickets")]
public class TicketController : ControllerBase
{
    private readonly IServicioTicket _service;

    public TicketController(IServicioTicket service)
    {
        _service = service;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(_service.ListarTickets());
    }

    [HttpGet("{id:int}")]
    public IActionResult GetById(int id)
    {
        var ticket = _service.BuscarTicket(id);
        return ticket == null ? NotFound() : Ok(ticket);
    }

    [HttpGet("codigo/{codigo:guid}")]
    public IActionResult GetByCodigo(Guid codigo)
    {
        var ticket = _service.BuscarTicketPorCodigo(codigo);
        return ticket == null ? NotFound() : Ok(ticket);
    }

    [HttpPost]
    public IActionResult Create([FromBody] CrearTicketDto request)
    {
        if(request == null)
        {
            return BadRequest("El cuerpo de la solicitud no puede ser nulo.");
        }

        var creado = _service.CrearTicket(
            request.CuentaId,
            request.FechaVisita,
            request.EventoId,
            request.TipoEntrada 
        );

        return CreatedAtAction(nameof(GetById), new { id = creado.Id }, creado);
    }

    [HttpPut("{id:int}")]
    public IActionResult Update(int id, [FromBody] UpdateTicketDto request)
    {
        if(request == null)
        {
            return BadRequest("El cuerpo de la solicitud no puede ser nulo.");
        }

        _service.ModificarTicket(
            id,
            request.CuentaId,
            request.FechaVisita,
            request.EventoId,
            request.TipoEntrada 
        );

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        _service.EliminarTicket(id);
        return NoContent();
    }
}
