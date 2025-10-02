namespace Parque.Aplicacion.Servicios;
public class ServicioFechaHora : IServicioFechaHora
{
    private DateTime? _fechaPersonalizada;
    private readonly object _lock = new();

    public ServicioFechaHora()
    {
    }

    public DateTime ObtenerFechaActual()
    {
        lock(_lock)
        {
            return _fechaPersonalizada ?? DateTime.Now;
        }
    }

    public void AsignarFechaPersonalizada(DateTime customTime)
    {
        lock(_lock)
        {
            _fechaPersonalizada = customTime;
        }
    }

    public bool UsaFechaPersonalizada()
    {
        lock(_lock)
        {
            return _fechaPersonalizada.HasValue;
        }
    }

    public void ResetearAFechaSistema()
    {
        lock(_lock)
        {
            _fechaPersonalizada = null;
        }
    }
}
