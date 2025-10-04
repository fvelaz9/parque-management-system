namespace Parque.Aplicacion.Servicios.Ticket;

public interface IServicioTicket
{
    Dominio.Ticket CrearTicket(int cuentaId, DateTime fechaVisita, int eventoId);
    public IEnumerable<Dominio.Ticket> ListarTickets();
    Dominio.Ticket? BuscarTicket(int id);
    Dominio.Ticket? BuscarTicketPorCodigo(Guid codigo);
    void ModificarTicket(int id, int cuentaId, DateTime fechaVisita, int eventoId);
    void EliminarTicket(int id);
}
