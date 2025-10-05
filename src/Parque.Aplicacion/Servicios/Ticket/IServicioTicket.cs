using Parque.Dominio;

namespace Parque.Aplicacion.Servicios.Ticket;

public interface IServicioTicket
{
    Dominio.Ticket CrearTicket(int cuentaId, DateTime fechaVisita, int? eventoId, TipoTicket tipoTicket);
    public IEnumerable<Dominio.Ticket> ListarTickets();
    Dominio.Ticket? BuscarTicket(int id);
    Dominio.Ticket? BuscarTicketPorCodigo(Guid codigo);
    void ModificarTicket(int id, int cuentaId, DateTime fechaVisita, int eventoId, TipoTicket tipoTicket);
    void EliminarTicket(int id);
    Dominio.Ticket ComprarTicket(int cuentaId, DateTime fechaVisita, int? eventoId, TipoTicket tipoTicket);
}
