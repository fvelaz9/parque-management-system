namespace Parque.Aplicacion.DTOs.Gamificacion;

public class RankingVisitanteDto
{
    public Guid VisitanteId { get; set; }
    public int PuntosDiarios { get; set; }
    public int PuntosTotales { get; set; }
    public int Posicion { get; set; }
}
