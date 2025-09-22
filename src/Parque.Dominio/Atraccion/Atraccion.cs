using Parque.Dominio;
using Parque.Dominio.Usuarios;

namespace DefaultNamespace;

public class Atraccion
{
    public int Id { get; set; }
    public string Nombre { get; set; }
    public TipoAtraccion Tipo { get; set; }
    public int Edad_Minima { get; set; }
    public int Capacidad { get; set; }
    public string Descripcion { get; set; }
    public EstadoAtraccion Estado { get; set; }
    public Atraccion(string nombre, TipoAtraccion tipo, int edadMinima, int capacidadMaxima, string descripcion)
    {
        // Id = Guid.NewGuid();
        Nombre = nombre;
        Tipo = tipo;
        Edad_Minima = edadMinima;
        Capacidad = capacidadMaxima;
        Descripcion = descripcion;
        Estado = EstadoAtraccion.Disponible;
    }

    public int CalcularAforoDisponible(int aforoActual)
    {
        return Capacidad - aforoActual;
    }

    /*public bool PuedeIngresar(Visitante visitante, DateTime fechaActual, int aforoActual)
    {
        if(Estado == EstadoAtraccion.FueraDeServicio)
        {
            return false;
        }

        if (visitante.Edad < EdadMinima)
        {
            return false;
        }

        if (aforoActual >= Capacidad)
        {
            return false;
        }

        return true;
    }*/
}
