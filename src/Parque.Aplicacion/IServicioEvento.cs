using Parque.Dominio;
namespace Parque.Aplicacion;

public interface IServicioEvento
{
    Evento AgregarEvento(Evento evento);
    void EliminarEventoPorId(int eventoId);
    Evento ObtenerEventoPorId(int eventoId);
    List<Evento> ListarEventos();
    void ActualizarEvento(Evento evento);
}
