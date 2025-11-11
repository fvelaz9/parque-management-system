using Parque.Dominio.Usuarios;

namespace Parque.Dominio.Gamificacion.Recompensa;

public class Recompensa
{
    public Guid Id { get; private set; }
    public string Nombre { get; private set; } = string.Empty;
    public string Descripcion { get; private set; } = string.Empty;
    public int CostoEnPuntos { get; private set; }
    public int CantidadDisponible { get; private set; }
    public NivelMembresia? NivelMembresiaRequerido { get; private set; }
    public DateTime FechaCreacion { get; private set; }
}
