namespace Parque.Dominio.Atracciones;

public class AtraccionParque(string nombre, TipoAtraccion tipo, int edadMinima, int capacidad, string descripcion)
{
    public int Id { get; set; }
    public string Nombre { get; set; } = nombre;
    public TipoAtraccion Tipo { get; set; } = tipo;
    public int EdadMinima { get; set; } = edadMinima;
    public int Capacidad { get; set; } = capacidad;
    public string Descripcion { get; set; } = descripcion;
    public EstadoAtraccion Estado { get; set; } = EstadoAtraccion.Disponible;

    public int CalcularAforoDisponible(int aforoActual)
    {
        return Capacidad - aforoActual;
    }
}
