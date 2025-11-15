namespace Parque.Aplicacion.DTOs.Gamificacion;

public class HistorialPuntuacionDto
{
    public DateTime FechaHora { get; set; }
    public string OrigenPuntos { get; set; } = string.Empty;
    public string EstrategiaActiva { get; set; } = string.Empty;
    public int Puntos { get; set; }
}
