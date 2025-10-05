using Parque.Dominio;

namespace Parque.Aplicacion.DTOS;

public class UpdateTicketDto
{
    public int CuentaId { get; set; }
    public DateTime FechaVisita { get; set; }
    public int EventoId { get; set; }
    public TipoTicket TipoEntrada { get; set; }
}
