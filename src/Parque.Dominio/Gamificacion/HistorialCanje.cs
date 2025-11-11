using Parque.Dominio.Excepciones;
using Parque.Dominio.Usuarios;

namespace Parque.Dominio.Gamificacion.Recompensa;

public class HistorialCanje
{
    public Guid Id { get; set; }
    public Guid VisitanteId { get; set; }
    public Guid RecompensaId { get; set; }
    public int PuntosCanjeados { get; set; }
    public DateTime FechaCanje { get; set; }

    public HistorialCanje() { }
}
