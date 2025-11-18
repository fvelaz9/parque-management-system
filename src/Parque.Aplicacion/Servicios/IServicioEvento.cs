using Parque.Dominio;
using Parque.Dominio.Atracciones;

namespace Parque.Aplicacion.Servicios;

public interface IServicioEvento
{
    Evento AgregarEvento(Evento evento);
    void EliminarEventoPorId(int eventoId);
    Evento ObtenerEventoPorId(int eventoId);
    List<Evento> ListarEventos();
    void ActualizarEvento(Evento evento);
    List<AtraccionParque> ObtenerAtraccionesPorEvento(int eventoId);
}
