using Parque.Aplicacion.DTOs;
using Parque.Dominio;
using Parque.Dominio.Atracciones;
using Parque.Infraestructura.Repositorios;

namespace Parque.Aplicacion.Servicios.Mantenimiento;

public class ServicioMantenimiento(IRepositorio<MantenimientoPreventivo> repoMantenimiento,
    IRepositorio<AtraccionParque> repoAtracciones,
    IServicioFechaHora servicioFechaHora
    ) : IServicioMantenimiento
{
    public MantenimientoPreventivo CrearMantenimiento(CrearMantenimientoRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Descripcion))
        {
            throw new ArgumentException("La descripción del mantenimiento es requerida");
        }

        var atraccion = repoAtracciones.Encontrar(a => a.Id == request.AtraccionId);

        if (atraccion == null)
        {
            throw new ArgumentException("Atracción no encontrada");
        }

        var fechaHoraInicio = request.FechaProgramada.Add(request.HoraInicio);
        var fechaHoraFin = fechaHoraInicio.Add(request.DuracionEstimada);
        var fechaActual = servicioFechaHora.ObtenerFechaActual();

        if (fechaHoraInicio <= fechaActual)
        {
            throw new ArgumentException("La fecha y hora de inicio del mantenimiento debe ser futura");
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
