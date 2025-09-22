namespace Parque.Dominio.Atracciones;

public class AtraccionParque(string nombre, TipoAtraccion tipo, int edadMinima, int capacidadMaxima, string descripcion)
{
    public int Id { get; set; }
    public string Nombre { get; set; } = nombre;
    public TipoAtraccion Tipo { get; set; } = tipo;
    public int Edad_Minima { get; set; } = edadMinima;
    public int Capacidad { get; set; } = capacidadMaxima;
    public string Descripcion { get; set; } = descripcion;
    public EstadoAtraccion Estado { get; set; } = EstadoAtraccion.Disponible;

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
