using Parque.Aplicacion.DTOs;
using Parque.Dominio.Atracciones;

namespace Parque.Aplicacion.Servicios.Atracciones;

public interface IServicioAtracciones
{
    AtraccionParque CrearAtraccion(string nombre, TipoAtraccion tipo, int edadMinima, int capacidad, string descripcion);
    IEnumerable<AtraccionParque> ListarAtracciones();
    AtraccionParque? BuscarAtraccion(int id);
    AtraccionParque ModificarAtraccion(int id, string nombre, TipoAtraccion tipo, int edadMinima, int capacidad, string descripcion);
    void EliminarAtraccion(int id);
    ReporteAtraccionDto ObtenerReporteUso(int atraccionId, DateTime fechaInicio, DateTime fechaFin);
    AforoAtraccionDto ObtenerAforoActual(int atraccionId);
    IEnumerable<AtraccionParque> ObtenerPorIds(List<int> ids);
}
