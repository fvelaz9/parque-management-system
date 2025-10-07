using Parque.Aplicacion.DTOS;
using Parque.Dominio;
using Parque.Dominio.Atracciones;
using Parque.Infraestructura.Repositorios;

namespace Parque.Aplicacion.Servicios.Incidencias;

public class ServicioIncidencia(IRepositorio<Incidencia> repoIncidencias, IRepositorio<AtraccionParque> repoAtracciones) : IServicioIncidencia
{
    public Incidencia CrearIncidencia(CrearIncidenciaRequest request)
    {
        var atraccion = repoAtracciones.Encontrar(a => a.Id == request.AtraccionId);
        if(atraccion == null)
        {
            throw new ArgumentException("Atraccion no encontrada");
        }

        if(request.FechaResolucionEstimada <= DateTime.Now)
        {
            throw new ArgumentException("La fecha de resolucion debe ser futura");
        }

        var incidencia = new Incidencia
        {
            Descripcion = request.Descripcion,
            FechaReporte = DateTime.Now,
            FechaResolucionEstimada = request.FechaResolucionEstimada,
            AtraccionId = atraccion.Id,
        };
        repoIncidencias.Agregar(incidencia);
        atraccion.Estado = EstadoAtraccion.FueraDeServicio;
        repoAtracciones.Editar(atraccion);
        return incidencia;
    }

    public bool EstaDisponible(int atraccionId)
    {
        var atraccion = repoAtracciones.Encontrar(a => a.Id == atraccionId);
        if(atraccion == null)
        {
            throw new ArgumentException("Atraccion no encontrada");
        }

        var incidenciaActiva = repoIncidencias.ObtenerTodos()
            .FirstOrDefault(i => i.AtraccionId == atraccionId && i.EstaActiva());
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
