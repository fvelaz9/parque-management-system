using Parque.Aplicacion.DTOs;
using Parque.Dominio;
using Parque.Dominio.Atracciones;
using Parque.Infraestructura.Repositorios;

namespace Parque.Aplicacion.Servicios.Mantenimiento;

public class ServicioMantenimiento(IRepositorio<MantenimientoPreventivo> repoMantenimiento,
    IRepositorio<AtraccionParque> repoAtracciones,
    IServicioFechaHora servicioFechaHora,
    IRepositorio<Incidencia> repoIncidencias
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

        var incidencia = CrearIncidenciaTemporal(request, fechaHoraInicio, fechaHoraFin);
        repoIncidencias.Agregar(incidencia);

        var mantenimiento = new MantenimientoPreventivo
        {
            AtraccionId = request.AtraccionId,
            FechaProgramada = request.FechaProgramada,
            HoraInicio = request.HoraInicio,
            DuracionEstimada = request.DuracionEstimada,
            Descripcion = request.Descripcion,
            IncidenciaId = incidencia.Id
        };
        repoMantenimiento.Agregar(mantenimiento);

        return mantenimiento;
    }

    private Incidencia CrearIncidenciaTemporal(CrearMantenimientoRequest request, DateTime inicio, DateTime fin)
    {
        return new Incidencia
        {
            Descripcion = $"Mantenimiento preventivo: {request.Descripcion}",
            FechaReporte = inicio,
            FechaResolucionEstimada = fin,
            AtraccionId = request.AtraccionId
        };
    }

    public void EliminarMantenimiento(int id)
    {
        var mantenimiento = repoMantenimiento.Encontrar(m => m.Id == id);
        if (mantenimiento == null)
        {
            throw new ArgumentException("Mantenimiento no encontrado");
        }

        repoIncidencias.Eliminar(i => i.Id == mantenimiento.IncidenciaId);
        repoMantenimiento.Eliminar(m => m.Id == id);
        var atraccion = repoAtracciones.Encontrar(a => a.Id == mantenimiento.AtraccionId);
        if (atraccion != null)
        {
            var fechaActual = servicioFechaHora.ObtenerFechaActual();
            var tieneIncidenciasActivas = repoIncidencias
                .ObtenerTodos()
                .Any(i => i.AtraccionId == mantenimiento.AtraccionId && i.EstaActiva(fechaActual));
            if (!tieneIncidenciasActivas)
            {
                atraccion.Estado = EstadoAtraccion.Disponible;
                repoAtracciones.Editar(atraccion);
            }
        }
    }

    public IEnumerable<MantenimientoPreventivo> ListarMantenimientos()
    {
        throw new NotImplementedException();
    }
}
