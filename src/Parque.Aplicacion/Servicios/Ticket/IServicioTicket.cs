using Parque.Dominio;

namespace Parque.Aplicacion.Servicios.Ticket;

public interface IServicioTicket
{
    Dominio.Ticket CrearTicket(Guid cuentaId, DateTime fechaVisita, int? eventoId, TipoTicket tipoTicket);
    IEnumerable<Dominio.Ticket> ListarTickets();
    Dominio.Ticket CrearTicketGeneral(int cuentaId, DateTime fechaVisita);
    Dominio.Ticket CrearTicketEventoEspecial(int cuentaId, DateTime fechaVisita, int eventoId);
    public IEnumerable<Dominio.Ticket> ListarTickets();
    Dominio.Ticket? BuscarTicket(int id);
    Dominio.Ticket? BuscarTicketPorCodigo(Guid codigo);
    void ModificarTicket(int id, Guid cuentaId, DateTime fechaVisita, int? eventoId, TipoTicket tipoTicket);
    void EliminarTicket(int id);
}
