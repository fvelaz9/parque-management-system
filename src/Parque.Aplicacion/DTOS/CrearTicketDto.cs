namespace Parque.Aplicacion.DTOS;

public class CrearTicketDto
{
    public int CuentaId { get; set; }
    public DateTime FechaVisita { get; set; }
    public int EventoId { get; set; }
}
