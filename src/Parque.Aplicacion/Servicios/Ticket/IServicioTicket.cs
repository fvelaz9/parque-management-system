using Parque.Dominio;

namespace Parque.Aplicacion.Servicios.Ticket;

public interface IServicioTicket
{
    Ticket CrearTicket(int cuentaId, DateTime fechaVisita, int eventoId);
}
