using Parque.Dominio;
using Parque.Infraestructura.Repositorios;

namespace Parque.Aplicacion.Servicios;

public class ServicioFechaHora(IRepositorio<ConfiguracionFechaHora> repositorio) : IServicioFechaHora
{
    public DateTime ObtenerFechaActual()
    {
        var configuracion = repositorio.ObtenerTodos().FirstOrDefault();
        return configuracion?.FechaHoraConfigurada ?? DateTime.Now;
    }

    public void ConfigurarFecha(DateTime customTime)
    {
        var fechaActual = ObtenerFechaActual();

        // Validar que solo avance hacia adelante
        if(customTime < fechaActual)
        {
            throw new InvalidOperationException(
                $"No se puede configurar una fecha anterior. Fecha actual: {fechaActual:yyyy-MM-ddTHH:mm}, Fecha solicitada: {customTime:yyyy-MM-ddTHH:mm}");
        }

        var configuracion = repositorio.ObtenerTodos().FirstOrDefault();

        if(configuracion == null)
        {
            configuracion = new ConfiguracionFechaHora(customTime);
            repositorio.Agregar(configuracion);
        }
        else
        {
            configuracion.FechaHoraConfigurada = customTime;
            repositorio.Editar(configuracion);
        }
    }

    public bool UsaFechaPersonalizada()
    {
        return repositorio.ObtenerTodos().Any();
    }

    public void ResetearAFechaSistema()
    {
        repositorio.Eliminar(c => true);
    }
}
