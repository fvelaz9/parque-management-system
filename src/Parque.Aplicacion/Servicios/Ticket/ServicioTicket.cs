using Parque.Dominio;
using Parque.Infraestructura.Repositorios;

namespace Parque.Aplicacion.Servicios.Ticket;

public class ServicioTicket(IRepositorio<Dominio.Ticket> repositorio, IRepositorio<Evento> repositorioEvento) : IServicioTicket
{
    private readonly IRepositorio<Dominio.Ticket> _repositorio = repositorio;
    private readonly IRepositorio<Evento> _repositorioEvento = repositorioEvento;
    public Dominio.Ticket CrearTicket(int cuentaId, DateTime fechaVisita, int eventoId, TipoTicket tipoTicket)
    {
        if(fechaVisita <= DateTime.Now)
        {
            throw new ArgumentException("La fecha de visita debe ser futura");
        }

        if(tipoTicket == TipoTicket.EventoEspecial && eventoId == 0)
        {
            throw new ArgumentException("Evento requerido para entradas especiales");
        }

        var ticket = new Dominio.Ticket
        {
            CuentaId = cuentaId,
            FechaVisita = fechaVisita,
            EventoId = eventoId,
            TipoEntrada = tipoTicket,
            Codigo = Guid.NewGuid(),
            FechaEmision = DateTime.Now
        };

        _repositorio.Agregar(ticket);
        return ticket;
    }

    public IEnumerable<Dominio.Ticket> ListarTickets() => _repositorio.ObtenerTodos();

    public Dominio.Ticket BuscarTicket(int id)
    {
        return _repositorio.Encontrar(t => t.Id == id);
    }

    public Dominio.Ticket? BuscarTicketPorCodigo(Guid codigo)
    {
        return _repositorio.Encontrar(t => t.Codigo == codigo);
    }

    public void ModificarTicket(int id, int cuentaId, DateTime fechaVisita, int eventoId, TipoTicket tipoTicket)
    {
        var ticket = _repositorio.Encontrar(t => t.Id == id)
                     ?? throw new ArgumentException("Ticket no encontrado");

        ticket.CuentaId = cuentaId;
        ticket.FechaVisita = fechaVisita;
        ticket.EventoId = eventoId;
        ticket.TipoEntrada = tipoTicket;

        _repositorio.Editar(ticket);
    }

    public void EliminarTicket(int id)
    {
        _repositorio.Eliminar(t => t.Id == id);
    }

    public Dominio.Ticket ComprarTicket(int cuentaId, DateTime fechaVisita, int? eventoId, TipoTicket tipoTicket)
    {
        ValidarFechaFutura(fechaVisita);
        ValidarEventoParaTicketEspecial(tipoTicket, eventoId);

        var ticket = CrearTicket(cuentaId, fechaVisita, eventoId ?? 0, tipoTicket);
        _repositorio.Agregar(ticket);
        return ticket;
    }

    private void ValidarFechaFutura(DateTime fechaVisita)
    {
        if (fechaVisita <= DateTime.Now)
        {
            throw new ArgumentException("La fecha de visita debe ser futura");
        }
    }

    private void ValidarEventoParaTicketEspecial(TipoTicket tipoTicket, int? eventoId)
    {
        if (tipoTicket == TipoTicket.EventoEspecial)
        {
            if (!eventoId.HasValue)
            {
                throw new ArgumentException("Evento requerido para entradas especiales");
            }

            var evento = _repositorioEvento.Encontrar(e => e.Id == eventoId.Value);
            if (evento == null)
            {
                throw new ArgumentException("Evento no encontrado");
            }
        }
    }
}
