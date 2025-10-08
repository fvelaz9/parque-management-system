namespace Parque.Dominio.Usuarios;

public class PuntuacionVisitante
{
    public int Id { get; set; }
    public Guid VisitanteId { get; set; }
    public DateTime Fecha { get; set; }
    public int PuntosDiarios { get; set; }
    public int PuntosTotales { get; set; }

    public PuntuacionVisitante(Guid visitanteId, DateTime fecha, int puntos)
    {
        VisitanteId = visitanteId;
        Fecha = fecha.Date;
        PuntosDiarios = puntos;
        PuntosTotales = puntos;
    }

    public PuntuacionVisitante() { }

    public void AgregarPuntos(int puntos)
    {
        PuntosDiarios += puntos;
        PuntosTotales += puntos;
    }

    public void RestablecerPuntosDiarios()
    {
        PuntosDiarios = 0;
    }
}
