using Parque.Aplicacion.Servicios.Ticket;
using Parque.Infraestructura.Repositorios;

namespace Parque.Aplicacion.Servicios.Tickets;

public class ServicioTicket : IServicioTicket
{
    private readonly IRepositorio<Dominio.Ticket> _repositorio;

    public ServicioTicket(IRepositorio<Ticket> repositorio)
    {
        _repositorio = repositorio;
    }

    public Ticket CrearTicket(int cuentaId, DateTime fechaVisita, int eventoId)
    {
        if (fechaVisita <= DateTime.Now)
            throw new ArgumentException("La fecha de visita debe ser futura");

        var ticket = new Ticket
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
}
