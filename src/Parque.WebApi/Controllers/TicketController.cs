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
        if (request == null)
        {
            return BadRequest("El request no puede ser nulo.");
        }

        try
        {
            Dominio.Ticket creado;

            if (request.TipoEntrada == TipoTicket.General)
            {
                creado = _service.CrearTicketGeneral(request.CuentaId, request.FechaVisita);
            }
            else if (request.TipoEntrada == TipoTicket.EventoEspecial)
            {
                if (!request.EventoId.HasValue)
                {
                    return BadRequest("Debe especificar el eventoId para tickets de evento especial.");
                }

                creado = _service.CrearTicketEventoEspecial(request.CuentaId, request.FechaVisita, request.EventoId.Value);
            }
            else
            {
                return BadRequest("Tipo de ticket no válido.");
            }

            return CreatedAtAction(nameof(GetById), new { id = creado.Id }, creado);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { mensaje = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { mensaje = $"Error interno: {ex.Message}" });
        }
    }

    [HttpPut("{id:int}")]
    public IActionResult Update(int id, [FromBody] UpdateTicketDto request)
    {
        if(request == null)
        {
            return BadRequest("El cuerpo de la solicitud no puede ser nulo.");
        }

        try
        {
            _service.ModificarTicket(
                id,
                request.CuentaId,
                request.FechaVisita,
                request.EventoId,
                request.TipoEntrada);

            return NoContent();
        }
        catch(ArgumentException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        _service.EliminarTicket(id);
        return NoContent();
    }
}
