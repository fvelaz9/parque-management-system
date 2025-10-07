namespace Parque.Aplicacion.DTOS;

public class RespuestaIncidencia
{
    public int Id { get; set; }
    public string? Descripcion { get; set; }
    public DateTime FechaReporte { get; set; }
    public DateTime FechaResolucionEstimada { get; set; }
    public int AtraccionId { get; set; }
    public bool EstaActiva { get; set; }
    public string? NombreAtraccion { get; set; }
}
