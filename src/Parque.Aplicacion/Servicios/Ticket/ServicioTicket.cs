using Parque.Dominio;
using Parque.Infraestructura.Repositorios;

namespace Parque.Aplicacion.Servicios.Ticket;

public class ServicioTicket(IRepositorio<Dominio.Ticket> repositorio, IRepositorio<Evento> repositorioEvento) : IServicioTicket
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
        ValidarEventoParaTicketEspecial(TipoTicket.EventoEspecial, eventoId);
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
            throw new ArgumentException("Ticket no encontrado");
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
            throw new ArgumentException("Ticket no encontrado");
        }

        ticket.CuentaId = cuentaId;
        ticket.FechaVisita = fechaVisita;
        ticket.EventoId = eventoId;
        ticket.TipoEntrada = tipoTicket;

        repositorio.Editar(ticket);
    }

    public void EliminarTicket(int id)
    {
        repositorio.Eliminar(t => t.Id == id);
    }

    private void ValidarFechaFutura(DateTime fechaVisita)
    {
        if(fechaVisita <= DateTime.Now)
        {
            throw new ArgumentException("La fecha de visita debe ser futura");
        }
    }

    private void ValidarEventoParaTicketEspecial(TipoTicket tipoTicket, int? eventoId)
    {
        if(tipoTicket == TipoTicket.EventoEspecial)
        {
            if(!eventoId.HasValue)
            {
                throw new ArgumentException("Evento requerido para entradas especiales");
            }

            var evento = repositorioEvento.Encontrar(e => e.Id == eventoId.Value);
            if(evento == null)
            {
                throw new ArgumentException("Evento no encontrado");
            }

            var ticketsVendidos = repositorio.ObtenerTodos()
                .Count(t => t.EventoId == eventoId.Value && t.EsValido);

            if(ticketsVendidos >= evento.AforoMaximo)
            {
                throw new InvalidOperationException("Aforo completo para este evento");
            }
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
            FechaEmision = DateTime.Now,
            EsValido = true
        };
    }
}
