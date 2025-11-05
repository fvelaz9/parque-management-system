using Parque.Aplicacion.DTOs;
using Parque.Dominio;
using Parque.Dominio.Atracciones;
using Parque.Infraestructura.Repositorios;

namespace Parque.Aplicacion.Servicios.Mantenimiento;

public class ServicioMantenimiento(IRepositorio<MantenimientoPreventivo> repoMantenimiento) : IServicioMantenimiento
{
    public MantenimientoPreventivo CrearMantenimiento(CrearMantenimientoRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Descripcion))
        {
            throw new ArgumentException("La descripción del mantenimiento es requerida");
        }

        var mantenimiento = new MantenimientoPreventivo
        {
            AtraccionId = request.AtraccionId,
            FechaProgramada = request.FechaProgramada,
            HoraInicio = request.HoraInicio,
            DuracionEstimada = request.DuracionEstimada,
            Descripcion = request.Descripcion,
            IncidenciaId = 1
        };
        repoMantenimiento.Agregar(mantenimiento);

        return mantenimiento;
    }

    public void EliminarMantenimiento(int id)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<MantenimientoPreventivo> ListarMantenimientos()
    {
        throw new NotImplementedException();
    }
}
