using Parque.Dominio.Atraccion;

namespace Parque.Dominio;
public class Evento(string titulo, string descripcion, DateTime inicio, DateTime fin, int aforoMaximo, float costoAdicional, EstadoEvento estado)
{
    public int Id { get; set; }
    public string Titulo { get; set; } = titulo;
    public string Descripcion { get; set; } = descripcion;
    public DateTime Inicio { get; set; } = inicio;
    public DateTime Fin { get; set; } = fin;
    public int AforoMaximo { get; set; } = aforoMaximo;
    public float CostoAdicional { get; set; } = costoAdicional;
    public List<TipoAtraccion> Atracciones { get; set; } = [];
    public EstadoEvento Estado { get; set; } = estado;
}
