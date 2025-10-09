using Parque.Dominio;

namespace Parque.Aplicacion.DTOs;

public class UpdateTicketDto
{
    public Guid CuentaId { get; set; }
    public DateTime FechaVisita { get; set; }
    public int? EventoId { get; set; }
    public TipoTicket TipoEntrada { get; set; }
}
