using Parque.Dominio;
using Parque.Dominio.Excepciones;
using Parque.Infraestructura.Repositorios;

namespace Parque.Aplicacion.Servicios.Ticket;

public class ServicioTicket(IRepositorio<Dominio.Ticket> repositorio, IRepositorio<Evento> repositorioEvento,
    IServicioFechaHora servicioFechaHora) : IServicioTicket
{
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
        ValidarEventoParaTicketEspecial(eventoId);
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
            ValidarEventoParaTicketEspecial(eventoId.Value);
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

    private void ValidarEventoParaTicketEspecial(int eventoId)
    {
        var evento = repositorioEvento.Encontrar(e => e.Id == eventoId);
        if(evento == null)
        {
            throw new ExcepcionEntidadNoEncontrada("Evento no encontrado");
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

    public List<Dominio.Ticket> ObtenerTicketsPorUsuarioYEvento(Guid usuarioId, int eventoId)
    {
        var evento = repositorioEvento.Encontrar(e => e.Id == eventoId);
        if(evento == null)
        {
            throw new ExcepcionEntidadNoEncontrada("Evento no encontrado");
        }

        var tickets = repositorio.ObtenerTodos()
            .Where(t => t.CuentaId == usuarioId &&
                        t.EventoId == eventoId &&
                        t.EsValido &&
                        t.TipoEntrada == TipoTicket.EventoEspecial &&
                        t.FechaVisita.Date >= servicioFechaHora.ObtenerFechaActual().Date)
            .ToList();

        return tickets;
    }

    public List<Dominio.Ticket> ListarTicketsValidosGeneral(Guid usuarioId)
    {
        var fechaActual = servicioFechaHora.ObtenerFechaActual();

        return repositorio.ObtenerTodos()
            .Where(t => t.CuentaId == usuarioId &&
                        t.EsValido &&
                        t.TipoEntrada == TipoTicket.General &&
                        t.FechaVisita.Date >= fechaActual.Date)
            .ToList();
    }
}
