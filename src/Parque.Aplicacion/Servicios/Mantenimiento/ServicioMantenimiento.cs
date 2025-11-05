using Parque.Aplicacion.DTOs;
using Parque.Dominio;
using Parque.Dominio.Atracciones;
using Parque.Infraestructura.Repositorios;

namespace Parque.Aplicacion.Servicios.Mantenimiento;

public class ServicioMantenimiento(IRepositorio<MantenimientoPreventivo> repoIncidencias,
    IRepositorio<Incidencia> repoMantenimiento,
    IRepositorio<AtraccionParque> repoAtracciones,
    IServicioFechaHora servicioFechaHora) : IServicioMantenimiento
{
    public MantenimientoPreventivo CrearMantenimiento(CrearMantenimientoRequest request)
    {
        // TODO: Implementar según el test
        throw new NotImplementedException();
    }

    public void EliminarMantenimiento(int id)
    {
        // TODO: Implementar según el test
        throw new NotImplementedException();
    }

    public IEnumerable<MantenimientoPreventivo> ListarMantenimientos()
    {
        // TODO: Implementar según el test
        throw new NotImplementedException();
    }
}
