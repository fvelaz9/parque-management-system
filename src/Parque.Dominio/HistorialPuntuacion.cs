namespace Parque.Dominio;

public class HistorialPuntuacion
{
    public int Id { get; set; }
    public DateTime FechaHora { get; set; }
    public string OrigenPuntos { get; set; } = string.Empty;
    public string EstrategiaActiva { get; set; } = string.Empty;
    public int Puntos { get; set; }

    public HistorialPuntuacion()
    {
    }

    public HistorialPuntuacion(DateTime fechaHora, string origenPuntos, string estrategiaActiva, int puntos)
    {
        FechaHora = fechaHora;
        OrigenPuntos = origenPuntos;
        EstrategiaActiva = estrategiaActiva;
        Puntos = puntos;
    }
}
