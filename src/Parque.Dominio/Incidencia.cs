namespace Parque.Dominio;

public class Incidencia
{
    public int Id { get; set; }
    public string? Descripcion { get; set; }
    public DateTime FechaReporte { get; set; }
    public DateTime FechaResolucionEstimada { get; set; }
    public bool Disponible { get; set; }
    public int AtraccionId { get; set; }

    public Incidencia()
    {
    }

    public Incidencia(string descripcion, DateTime fechaIngreso, DateTime fechafin, int atraccionId)
    {
        Descripcion = descripcion;
        FechaReporte = fechaIngreso;
        FechaResolucionEstimada = fechafin;
        AtraccionId = atraccionId;
    }

    public bool EstaActiva(DateTime fechaReferencia) => fechaReferencia < FechaResolucionEstimada;
    public bool EstaDisponible(DateTime fechaReferencia) => fechaReferencia >= FechaResolucionEstimada;
}
