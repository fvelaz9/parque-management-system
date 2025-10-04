namespace Parque.Aplicacion.Servicios.Ticket;

public interface IServicioTicket
{
    Dominio.Ticket CrearTicket(int cuentaId, DateTime fechaVisita, int eventoId);
}
