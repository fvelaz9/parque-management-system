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
        var atraccion = repoAtracciones.Encontrar(a => a.Id == request.AtraccionId);
        var fechaHoraInicio = request.FechaProgramada.Add(request.HoraInicio);
        var fechaHoraFin = fechaHoraInicio.Add(request.DuracionEstimada);
        VerificarMantenimiento(request);

        ValidarSolapamientoHorarios(request.AtraccionId, fechaHoraInicio, fechaHoraFin);

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
        atraccion!.Estado = EstadoAtraccion.FueraDeServicio;
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

    public void VerificarMantenimiento(CrearMantenimientoRequest request)
    {
        if(string.IsNullOrWhiteSpace(request.Descripcion))
        {
            throw new ArgumentException("La descripción del mantenimiento es requerida");
        }

        var atraccion = repoAtracciones.Encontrar(a => a.Id == request.AtraccionId);

        if(atraccion == null)
        {
            throw new ArgumentException("Atracción no encontrada");
        }

        var fechaHoraInicio = request.FechaProgramada.Add(request.HoraInicio);
        var fechaActual = servicioFechaHora.ObtenerFechaActual();

        if(fechaHoraInicio <= fechaActual)
        {
            throw new ArgumentException("La fecha y hora de inicio del mantenimiento debe ser futura");
        }
    }

    private void ValidarSolapamientoHorarios(int atraccionId, DateTime inicioNuevo, DateTime finNuevo)
    {
        var mantenimientosExistentes = repoMantenimiento
            .ObtenerTodos()
            .Where(m => m.AtraccionId == atraccionId);

        foreach(var mantenimiento in mantenimientosExistentes)
        {
            var inicioExistente = mantenimiento.FechaProgramada.Add(mantenimiento.HoraInicio);
            var finExistente = inicioExistente.Add(mantenimiento.DuracionEstimada);

            // Verificar solapamiento: dos rangos se solapan si inicio1 < fin2 && inicio2 < fin1
            if(inicioNuevo < finExistente && inicioExistente < finNuevo)
            {
                throw new ArgumentException(
                    $"Ya existe un mantenimiento programado para esta atracción en el horario solicitado. " +
                    $"Mantenimiento existente: {inicioExistente:dd/MM/yyyy HH:mm} - {finExistente:dd/MM/yyyy HH:mm}");
            }
        }
    }

    public MantenimientoPreventivo ActualizarMantenimiento(int id, CrearMantenimientoRequest request)
    {
        VerificarMantenimiento(request);
        var mantenimiento = repoMantenimiento.Encontrar(r => r.Id == id);
        if(mantenimiento == null)
        {
            throw new ArgumentException("Mantenimiento no encontrado");
        }

        var fechaHoraInicio = request.FechaProgramada.Add(request.HoraInicio);
        var fechaHoraFin = fechaHoraInicio.Add(request.DuracionEstimada);

        ValidarSolapamientoHorariosParaActualizacion(id, request.AtraccionId, fechaHoraInicio, fechaHoraFin);
        mantenimiento.AtraccionId = request.AtraccionId;
        mantenimiento.FechaProgramada = request.FechaProgramada;
        mantenimiento.HoraInicio = request.HoraInicio;
        mantenimiento.DuracionEstimada = request.DuracionEstimada;
        mantenimiento.Descripcion = request.Descripcion;
        mantenimiento.IncidenciaId = mantenimiento.IncidenciaId;
        repoMantenimiento.Editar(mantenimiento);
        return mantenimiento;
    }

    private void ValidarSolapamientoHorariosParaActualizacion(int mantenimientoIdActual, int atraccionId, DateTime inicioNuevo, DateTime finNuevo)
    {
        var mantenimientosExistentes = repoMantenimiento
            .ObtenerTodos()
            .Where(m => m.AtraccionId == atraccionId && m.Id != mantenimientoIdActual);

        foreach(var mantenimiento in mantenimientosExistentes)
        {
            var inicioExistente = mantenimiento.FechaProgramada.Add(mantenimiento.HoraInicio);
            var finExistente = inicioExistente.Add(mantenimiento.DuracionEstimada);

            if(inicioNuevo < finExistente && inicioExistente < finNuevo)
            {
                throw new ArgumentException(
                    $"Ya existe un mantenimiento programado para esta atracción en el horario solicitado. " +
                    $"Mantenimiento existente: {inicioExistente:dd/MM/yyyy HH:mm} - {finExistente:dd/MM/yyyy HH:mm}");
            }
        }
    }

    public void EliminarMantenimiento(int id)
    {
        var mantenimiento = repoMantenimiento.Encontrar(m => m.Id == id);
        if(mantenimiento == null)
        {
            throw new ArgumentException("Mantenimiento no encontrado");
        }

        // repoIncidencias.Eliminar(i => i.Id == mantenimiento.IncidenciaId);
        repoMantenimiento.Eliminar(m => m.Id == id);
        var atraccion = repoAtracciones.Encontrar(a => a.Id == mantenimiento.AtraccionId);
        if(atraccion != null)
        {
            var fechaActual = servicioFechaHora.ObtenerFechaActual();
            var tieneIncidenciasActivas = repoIncidencias
                .ObtenerTodos()
                .Any(i => i.AtraccionId == mantenimiento.AtraccionId && i.EstaActiva(fechaActual));
            if(!tieneIncidenciasActivas)
            {
                atraccion.Estado = EstadoAtraccion.Disponible;
                repoAtracciones.Editar(atraccion);
            }
        }
    }

    public IEnumerable<CrearMantenimientoRequest> ListarMantenimientos()
    {
        var mantenimientos = repoMantenimiento.ObtenerTodos();
        var atracciones = repoAtracciones.ObtenerTodos();
        if(mantenimientos == null)
        {
            return Enumerable.Empty<CrearMantenimientoRequest>();
        }

        return mantenimientos.Select(m => new CrearMantenimientoRequest
        {
            Id = m.Id,
            AtraccionId = m.AtraccionId,
            NombreAtraccion = atracciones.FirstOrDefault(a => a.Id == m.AtraccionId)?.Nombre ?? "Atracción no encontrada",
            FechaProgramada = m.FechaProgramada,
            HoraInicio = m.HoraInicio,
            DuracionEstimada = m.DuracionEstimada,
            Descripcion = m.Descripcion ?? string.Empty,
            IncidenciaId = m.IncidenciaId
        });
    }
}
