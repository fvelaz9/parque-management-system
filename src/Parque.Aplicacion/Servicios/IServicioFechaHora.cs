namespace Parque.Aplicacion.Servicios;

public interface IServicioFechaHora
{
    DateTime ObtenerFechaActual();
    void ConfigurarFecha(DateTime customTime);
    bool UsaFechaPersonalizada();
    void ResetearAFechaSistema();
}
