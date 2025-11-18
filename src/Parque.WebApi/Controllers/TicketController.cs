using Microsoft.AspNetCore.Mvc;
using Parque.Aplicacion.DTOs;
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

        if(request.TipoEntrada == TipoTicket.EventoEspecial && !request.EventoId.HasValue)
        {
            return BadRequest(new { mensaje = "Debe especificar el eventoId para tickets de evento especial" });
        }

        var creado = service.CrearTicket(usuario!.Id, request);

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

    [HttpGet("por-usuario/{usuarioId:guid}")]
    [AuthorizationFilter("Operador")]
    public IActionResult GetTicketsPorUsuario(Guid usuarioId)
    {
        var tickets = service.ListarTicketsValidosGeneral(usuarioId);
        return Ok(tickets);
    }

    [HttpGet("por-usuario/{usuarioId:guid}/evento/{eventoId:int}")]
    [AuthorizationFilter("Operador")]
    public IActionResult GetTicketsPorUsuarioYEvento(Guid usuarioId, int eventoId)
    {
        var tickets = service.ObtenerTicketsPorUsuarioYEvento(usuarioId, eventoId);
        return Ok(tickets);
    }
}
