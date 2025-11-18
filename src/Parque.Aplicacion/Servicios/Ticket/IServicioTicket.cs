using Parque.Aplicacion.DTOs;
using Parque.Dominio;

namespace Parque.Aplicacion.Servicios.Ticket;

public interface IServicioTicket
{
    IEnumerable<Dominio.Ticket> ListarTickets();
    Dominio.Ticket CrearTicket(Guid cuentaId, CrearTicketDto ticketDto);
    Dominio.Ticket? BuscarTicket(int id);
    Dominio.Ticket? BuscarTicketPorCodigo(Guid codigo);
    void ModificarTicket(int id, Guid cuentaId, DateTime fechaVisita, int? eventoId, TipoTicket tipoTicket);
    void EliminarTicket(int id);
}
