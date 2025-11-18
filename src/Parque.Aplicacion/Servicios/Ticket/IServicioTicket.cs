using Parque.Dominio;

namespace Parque.Aplicacion.Servicios.Ticket;

public interface IServicioTicket
{
    IEnumerable<Dominio.Ticket> ListarTickets();
    Dominio.Ticket CrearTicketGeneral(Guid cuentaId, DateTime fechaVisita);
    Dominio.Ticket CrearTicketEventoEspecial(Guid cuentaId, DateTime fechaVisita, int eventoId);
    Dominio.Ticket? BuscarTicket(int id);
    Dominio.Ticket? BuscarTicketPorCodigo(Guid codigo);
    void ModificarTicket(int id, Guid cuentaId, DateTime fechaVisita, int? eventoId, TipoTicket tipoTicket);
    void EliminarTicket(int id);
    List<Dominio.Ticket> ObtenerTicketsPorUsuarioYEvento(Guid usuarioId, int eventoId);
    List<Dominio.Ticket> ListarTicketsValidosGeneral(Guid usuarioId);
}
