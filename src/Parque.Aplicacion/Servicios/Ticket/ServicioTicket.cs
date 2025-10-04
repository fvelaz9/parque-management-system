using Parque.Dominio;
using Parque.Infraestructura.Repositorios;

namespace Parque.Aplicacion.Servicios.Ticket;

public class ServicioTicket : IServicioTicket
{
    private readonly IRepositorio<Dominio.Ticket> _repositorio;

    public ServicioTicket(IRepositorio<Dominio.Ticket> repositorio)
    {
        _repositorio = repositorio;
    }

    public Dominio.Ticket CrearTicket(int cuentaId, DateTime fechaVisita, int eventoId)
    {
        if(fechaVisita <= DateTime.Now)
        {
            throw new ArgumentException("La fecha de visita debe ser futura");
        }

        var ticket = new Dominio.Ticket
        {
            CuentaId = cuentaId,
            FechaVisita = fechaVisita,
            EventoId = eventoId,
            Codigo = Guid.NewGuid(),
            FechaEmision = DateTime.Now
        };

        _repositorio.Agregar(ticket);
        return ticket;
    }

    public IEnumerable<Dominio.Ticket> ListarTickets() => _repositorio.ObtenerTodos();
}
