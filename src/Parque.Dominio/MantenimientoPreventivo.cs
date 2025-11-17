namespace Parque.Dominio;

public class MantenimientoPreventivo
{
    public int Id { get; set; }
    public int AtraccionId { get; set; }
    public DateTime FechaProgramada { get; set; }
    public TimeSpan HoraInicio { get; set; }
    public TimeSpan DuracionEstimada { get; set; }
    public string? Descripcion { get; set; }
    public int IncidenciaId { get; set; }
    public Incidencia IncidenciaAsociada { get; set; } = null!;
    public MantenimientoPreventivo()
    {
    }

    public DateTime FechaHoraInicio() => FechaProgramada.Add(HoraInicio);
    public DateTime FechaHoraFin() => FechaHoraInicio().Add(DuracionEstimada);
}
