using Parque.Dominio.Atracciones;

namespace Parque.Aplicacion.Servicios.Atracciones;

public interface IServicioAtracciones
{
    AtraccionParque CrearAtraccion(string nombre, TipoAtraccion tipo, int edadMinima, int capacidad, string descripcion);
    IEnumerable<AtraccionParque> ListarAtracciones();
    AtraccionParque? BuscarAtraccion(int id);
    void ModificarAtraccion(int id, string nombre, TipoAtraccion tipo, int edadMinima, int capacidad, string descripcion);
    void EliminarAtraccion(int id);
}
