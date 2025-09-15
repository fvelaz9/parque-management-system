namespace Parque.Dominio;

public class Atracciones
{
    public Guid Id { get; private set; }
    public string Nombre { get; private set; }
    
    public TipoAtraccion Tipo { get; private set; }
    public int Edad_Minima { get; private set; }
    public int Capacidad { get; private set; }
    
    public string Descripcion { get; private set; }
    public EstadoAtraccion Estado { get; private set; }
    
}
