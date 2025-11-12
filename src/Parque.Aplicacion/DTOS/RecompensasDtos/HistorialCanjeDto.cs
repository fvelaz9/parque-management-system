namespace Parque.Aplicacion.DTOs.RecompensasDtos;

public class HistorialCanjeDto
{
    public Guid Id { get; set; }
    public Guid VisitanteId { get; set; }
    public Guid RecompensaId { get; set; }
    public string NombreRecompensa { get; set; } = string.Empty;
    public int PuntosCanjeados { get; set; }
    public DateTime FechaCanje { get; set; }
}
