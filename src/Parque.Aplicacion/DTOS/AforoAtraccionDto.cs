namespace Parque.Aplicacion.DTOS;

public class AforoAtraccionDto
{
    public int AtraccionId { get; set; }
    public string? NombreAtraccion { get; set; }
    public int AforoActual { get; set; }
    public int CapacidadMaxima { get; set; }
    public int Disponible { get; set; }
}
