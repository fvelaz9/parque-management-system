using Parque.Aplicacion.DTOS;
using Parque.Dominio.Atracciones;

namespace Parque.Aplicacion.Servicios.Atracciones;

public interface IServicioAtracciones
{
    AtraccionParque CrearAtraccion(string nombre, TipoAtraccion tipo, int edadMinima, int capacidad, string descripcion);
    IEnumerable<AtraccionParque> ListarAtracciones();
    AtraccionParque? BuscarAtraccion(int id);
    void ModificarAtraccion(int id, string nombre, TipoAtraccion tipo, int edadMinima, int capacidad, string descripcion);
    void EliminarAtraccion(int id);
    RegistroVisita RegistrarIngreso(Guid identificador, int atraccionId);
    RegistroVisita RegistrarEgreso(Guid identificador, int atraccionId);
    List<ReporteAtraccionDto> ObtenerReporteUso(DateTime fechaInicio, DateTime fechaFin);
    AforoAtraccionDto ObtenerAforoActual(int atraccionId);
}
