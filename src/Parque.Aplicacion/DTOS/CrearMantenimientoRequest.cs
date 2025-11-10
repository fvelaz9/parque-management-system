namespace Parque.Aplicacion.DTOs;

public class CrearMantenimientoRequest
{
    public int AtraccionId { get; set; }
    public DateTime FechaProgramada { get; set; }
    public TimeSpan HoraInicio { get; set; }
    public TimeSpan DuracionEstimada { get; set; }
    public required string Descripcion { get; set; }
}
