namespace Parque.Aplicacion.Servicios;

public interface IServicioFechaHora
{
    DateTime ObtenerFechaActual();
    void AsignarFechaPersonalizada(DateTime customTime);
    bool UsaFechaPersonalizada();
    void ResetearAFechaSistema();
}
