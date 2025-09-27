namespace Parque.Dominio;

public class Ticket
{
    public int Id { get; set; }
    public int CuentaId { get; set; }
    public DateTime FechaVisita { get; set; }
    public int EventoId { get; set; }
    public Guid Codigo { get; set; }
    public DateTime FechaEmision { get; set; }
}
