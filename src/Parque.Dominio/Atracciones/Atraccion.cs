namespace Parque.Dominio.Atracciones;

public class Atraccion
{
    public int Id { get; set; }
    public required string Nombre { get; set; }
    public TipoAtraccion Tipo { get; set; }
    public int EdadMinima { get; set; }
    public int CapacidadMaxima { get; set; }
    public required string Descripcion { get; set; }
}
