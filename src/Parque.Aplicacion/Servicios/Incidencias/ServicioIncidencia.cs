using Parque.Aplicacion.DTOS;
using Parque.Dominio;
using Parque.Dominio.Atracciones;
using Parque.Dominio.Excepciones;
using Parque.Infraestructura.Repositorios;

namespace Parque.Aplicacion.Servicios.Incidencias;

public class ServicioIncidencia(IRepositorio<Incidencia> repoIncidencias, IRepositorio<AtraccionParque> repoAtracciones,
    IServicioFechaHora servicioFechaHora) : IServicioIncidencia
{
    public Incidencia CrearIncidencia(CrearIncidenciaRequest request)
    {
        if(string.IsNullOrWhiteSpace(request.Descripcion))
        {
            throw new ArgumentException("La descripción de la incidencia es requerida");
        }

        var atraccion = repoAtracciones.Encontrar(a => a.Id == request.AtraccionId);
        if(atraccion == null)
        {
            throw new ArgumentException("Atraccion no encontrada");
        }

        var fechaActual = servicioFechaHora.ObtenerFechaActual();
        if(request.FechaResolucionEstimada <= fechaActual)
        {
            throw new ArgumentException("La fecha de resolucion debe ser futura");
        }

        var incidencia = new Incidencia
        {
            Descripcion = request.Descripcion,
            FechaReporte = fechaActual,
            FechaResolucionEstimada = request.FechaResolucionEstimada,
            AtraccionId = atraccion.Id,
        };
        repoIncidencias.Agregar(incidencia);
        atraccion.Estado = EstadoAtraccion.FueraDeServicio;
        repoAtracciones.Editar(atraccion);
        return incidencia;
    }

    public void ResolverIncidencia(int incidenciaId)
    {
        var incidencia = repoIncidencias.Encontrar(i => i.Id == incidenciaId);
        if(incidencia == null)
        {
            throw new ExcepcionEntidadNoEncontrada("Incidencia no encontrada");
        }

        repoIncidencias.Eliminar(i => i.Id == incidenciaId);

        var atraccion = repoAtracciones.Encontrar(a => a.Id == incidencia.AtraccionId);
        if(atraccion != null)
        {
            var incidenciasActivas = repoIncidencias.ObtenerTodos()
                .Any(i => i.AtraccionId == incidencia.AtraccionId && i.EstaActiva(servicioFechaHora.ObtenerFechaActual()));

            if(!incidenciasActivas)
            {
                atraccion.Estado = EstadoAtraccion.Disponible;
                repoAtracciones.Editar(atraccion);
            }
        }
    }

    public bool EstaDisponible(int atraccionId)
    {
        var atraccion = repoAtracciones.Encontrar(a => a.Id == atraccionId);
        if(atraccion == null)
        {
            throw new ArgumentException("Atraccion no encontrada");
        }

        var incidenciaActiva = repoIncidencias.ObtenerTodos()
            .FirstOrDefault(i => i.AtraccionId == atraccionId && i.EstaActiva(servicioFechaHora.ObtenerFechaActual()));
        if(incidenciaActiva == null)
        {
            atraccion.Estado = EstadoAtraccion.Disponible;
            repoAtracciones.Editar(atraccion);
            return true;
        }

        atraccion.Estado = EstadoAtraccion.FueraDeServicio;
        repoAtracciones.Editar(atraccion);
        return false;
    }
}
