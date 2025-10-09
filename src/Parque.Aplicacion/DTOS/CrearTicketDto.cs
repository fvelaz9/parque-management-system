using Parque.Dominio;

namespace Parque.Aplicacion.DTOs;

public class CrearTicketDto
{
    public DateTime FechaVisita { get; set; }
    public int? EventoId { get; set; }
    public TipoTicket TipoEntrada { get; set; }
}
