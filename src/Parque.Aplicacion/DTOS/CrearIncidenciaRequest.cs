namespace Parque.Aplicacion.DTOS;

public class CrearIncidenciaRequest
{
    public required string Descripcion { get; set; }
    public DateTime FechaResolucionEstimada { get; set; }
    public int AtraccionId { get; set; }
}
