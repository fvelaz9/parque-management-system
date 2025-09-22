using DefaultNamespace;

namespace Parque.Dominio;
public class Evento
{
    public int Id { get; set; }
    public string Titulo { get; set; }
    public string Descripcion { get; set; }
    public DateTime Inicio { get; set; }
    public DateTime Fin { get; set; }
    public int AforoMaximo { get; set; }
    public float CostoAdicional { get; set; }
    public List<Atraccion> Atracciones { get; set; } = [];
    public EstadoEvento Estado { get; set; }

    public Evento(string titulo, string descripcion, DateTime inicio, DateTime fin, int aforoMaximo, float costoAdicional, EstadoEvento estado)
    {
        Titulo = titulo;
        Descripcion = descripcion;
        Inicio = inicio;
        Fin = fin;
        AforoMaximo = aforoMaximo;
        CostoAdicional = costoAdicional;
        Atracciones = [];
        Estado = estado;
    }
}
