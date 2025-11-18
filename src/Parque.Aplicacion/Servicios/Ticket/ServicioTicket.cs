using Parque.Aplicacion.DTOs;
using Parque.Dominio;
using Parque.Dominio.Excepciones;
using Parque.Infraestructura.Repositorios;

namespace Parque.Aplicacion.Servicios.Ticket;

public class ServicioTicket(IRepositorio<Dominio.Ticket> repositorio, IRepositorio<Evento> repositorioEvento,
    IServicioFechaHora servicioFechaHora) : IServicioTicket
{
    public Dominio.Ticket CrearTicket(Guid cuentaId, CrearTicketDto ticketDto)
    {
        ValidarFechaFutura(ticketDto.FechaVisita);

        if(ticketDto.TipoEntrada == TipoTicket.EventoEspecial)
        {
            if(!ticketDto.EventoId.HasValue)
            {
                throw new ArgumentException("Debe especificar el eventoId para tickets de evento especial");
            }

            ValidarEventoParaTicketEspecial(ticketDto.EventoId!.Value, ticketDto.FechaVisita);
            return CrearTicketEventoEspecial(cuentaId, ticketDto.FechaVisita, ticketDto.EventoId.Value);
        }
        else if(ticketDto.TipoEntrada == TipoTicket.General)
        {
            return CrearTicketGeneral(cuentaId, ticketDto.FechaVisita);
        }
        else
        {
            throw new ArgumentException("Tipo de ticket no válido");
        }
    }

    public Dominio.Ticket CrearTicketGeneral(Guid cuentaId, DateTime fechaVisita)
    {
        ValidarFechaFutura(fechaVisita);
        Dominio.Ticket ticket = ConstruirTicket(cuentaId, fechaVisita, null, TipoTicket.General);

        repositorio.Agregar(ticket);
        return ticket;
    }

    public Dominio.Ticket CrearTicketEventoEspecial(Guid cuentaId, DateTime fechaVisita, int eventoId)
    {
        ValidarFechaFutura(fechaVisita);
        ValidarEventoParaTicketEspecial(eventoId, fechaVisita);
        Dominio.Ticket ticket = ConstruirTicket(cuentaId, fechaVisita, eventoId, TipoTicket.EventoEspecial);
        repositorio.Agregar(ticket);
        return ticket;
    }

    public IEnumerable<Dominio.Ticket> ListarTickets() => repositorio.ObtenerTodos();

    public Dominio.Ticket BuscarTicket(int id)
    {
        var ticket = repositorio.Encontrar(t => t.Id == id);
        if(ticket == null)
        {
            throw new ExcepcionEntidadNoEncontrada("Ticket no encontrado");
        }

        return ticket;
    }

    public Dominio.Ticket? BuscarTicketPorCodigo(Guid codigo)
    {
        return repositorio.Encontrar(t => t.Codigo == codigo);
    }

    public void ModificarTicket(int id, Guid cuentaId, DateTime fechaVisita, int? eventoId, TipoTicket tipoTicket)
    {
        var ticket = repositorio.Encontrar(t => t.Id == id);
        if(ticket == null)
        {
            throw new ExcepcionEntidadNoEncontrada("Ticket no encontrado");
        }

        ValidarFechaFutura(fechaVisita);

        if(tipoTicket == TipoTicket.EventoEspecial && eventoId.HasValue)
        {
            ValidarEventoParaTicketEspecial(eventoId.Value, fechaVisita);
        }

        ticket.CuentaId = cuentaId;
        ticket.FechaVisita = fechaVisita;
        ticket.EventoId = eventoId;
        ticket.TipoEntrada = tipoTicket;

        repositorio.Editar(ticket);
    }

    public void EliminarTicket(int id)
    {
        var ticket = repositorio.Encontrar(t => t.Id == id);
        if(ticket == null)
        {
            throw new ExcepcionEntidadNoEncontrada("Ticket no encontrado");
        }

        repositorio.Eliminar(t => t.Id == id);
    }

    private void ValidarFechaFutura(DateTime fechaVisita)
    {
        var fechaActual = servicioFechaHora.ObtenerFechaActual();
        if(fechaVisita.Date < fechaActual.Date)
        {
            throw new ArgumentException("La fecha de visita debe ser futura");
        }
    }

    private void ValidarEventoParaTicketEspecial(int eventoId, DateTime fechaVisita)
    {
        var evento = repositorioEvento.Encontrar(e => e.Id == eventoId);
        if(evento == null)
        {
            throw new ExcepcionEntidadNoEncontrada("Evento no encontrado");
        }

        if(fechaVisita.Date < evento.Inicio.Date || fechaVisita.Date > evento.Fin.Date)
        {
            throw new ArgumentException("La fecha de visita debe estar dentro del rango del evento especial");
        }

        var ticketsVendidos = repositorio.ObtenerTodos()
            .Count(t => t.EventoId == eventoId && t.EsValido);

        if(ticketsVendidos >= evento.AforoMaximo)
        {
            throw new InvalidOperationException("Aforo completo para este evento");
        }
    }

    private Dominio.Ticket ConstruirTicket(Guid cuentaId, DateTime fechaVisita, int? eventoId, TipoTicket tipo)
    {
        return new Dominio.Ticket
        {
            CuentaId = cuentaId,
            FechaVisita = fechaVisita,
            EventoId = eventoId,
            TipoEntrada = tipo,
            Codigo = Guid.NewGuid(),
            FechaEmision = servicioFechaHora.ObtenerFechaActual(),
            EsValido = true
        };
    }
}
