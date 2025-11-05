using Parque.Aplicacion.DTOs;
using Parque.Dominio;

namespace Parque.Aplicacion.Servicios.Mantenimiento;

public interface IServicioMantenimiento
{
    MantenimientoPreventivo CrearMantenimiento(CrearMantenimientoRequest request);
    void EliminarMantenimiento(int id);
    IEnumerable<MantenimientoPreventivo> ListarMantenimientos();
}
