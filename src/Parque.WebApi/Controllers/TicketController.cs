using Microsoft.AspNetCore.Mvc;
using Parque.Aplicacion.DTOS;
using Parque.Aplicacion.Servicios.Ticket;
using Parque.Dominio;
using Parque.Dominio.Usuarios;
using Parque.WebApi.Filtros;

namespace Parque.WebApi.Controllers;

[ApiController]
[Route("api/tickets")]
public class TicketController(IServicioTicket service) : ControllerBase
{
    [HttpPost]
    [AuthorizationFilter("Visitante")]
    public IActionResult Create([FromBody] CrearTicketDto request)
    {
        var usuario = HttpContext.Items["user"] as Cuenta;
        if(usuario == null)
        {
            return Unauthorized(new { mensaje = "Usuario no autenticado" });
        }

        Dominio.Ticket creado;

        if(request.TipoEntrada == TipoTicket.General)
        {
            creado = service.CrearTicketGeneral(usuario.Id, request.FechaVisita);
        }
        else if(request.TipoEntrada == TipoTicket.EventoEspecial)
        {
            if(!request.EventoId.HasValue)
            {
                return BadRequest(new { mensaje = "Debe especificar el eventoId para tickets de evento especial" });
            }

            creado = service.CrearTicketEventoEspecial(usuario.Id, request.FechaVisita, request.EventoId.Value);
        }
        else
        {
            return BadRequest(new { mensaje = "Tipo de ticket no válido" });
        }

        return CreatedAtAction(nameof(GetByCodigo), new { codigo = creado.Codigo }, creado);
    }

    [HttpGet("mis-tickets")]
    [AuthorizationFilter("Visitante")]
    public IActionResult ObtenerMisTickets()
    {
        var usuario = HttpContext.Items["user"] as Cuenta;
        if(usuario == null)
        {
            return Unauthorized(new { mensaje = "Usuario no autenticado" });
        }

        var tickets = service.ListarTickets().Where(t => t.CuentaId == usuario.Id);
        return Ok(tickets);
    }

    [HttpGet("codigo/{codigo:guid}")]
    [AuthorizationFilter("Operador")]
    public IActionResult GetByCodigo(Guid codigo)
    {
        var ticket = service.BuscarTicketPorCodigo(codigo);
        if(ticket == null)
        {
            return NotFound(new { mensaje = "Ticket no encontrado" });
        }

        return Ok(ticket);
    }
}
