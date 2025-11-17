using Parque.Dominio.Excepciones;
using Parque.Dominio.Usuarios;

namespace Parque.Dominio.Gamificacion;

public class Recompensa
{
    public Guid Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public int CostoEnPuntos { get; set; }
    public int CantidadDisponible { get; set; }
    public NivelMembresia? NivelMembresiaRequerido { get; set; }
    public DateTime FechaCreacion { get; set; }

    public Recompensa()
    {
    }

    public void ReducirStock()
    {
        if(CantidadDisponible <= 0)
        {
            throw new ExcepcionDominio("No hay stock disponible para reducir.");
        }

        CantidadDisponible--;
    }
}
