namespace Parque.Aplicacion.DTOs;

public class AforoResponse
{
    public int AtraccionId { get; set; }
    public string NombreAtraccion { get; set; } = string.Empty;
    public int CapacidadTotal { get; set; }
    public int VisitantesActuales { get; set; }
    public int CapacidadRestante { get; set; }
    public double PorcentajeOcupacion { get; set; }
    public bool AforoCompleto { get; set; }
}
