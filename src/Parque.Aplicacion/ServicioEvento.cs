using Parque.Dominio;
using Parque.Infraestructura.Repositorios;

namespace Parque.Aplicacion;

public class ServicioEvento : IServicioEvento
{
    private readonly IRepositorio<Evento> _repositorioEvento;
    public ServicioEvento(IRepositorio<Evento> repositorioEvento)
    {
        _repositorioEvento = repositorioEvento;
    }

    public Evento AgregarEvento(Evento evento)
    {
        throw new NotImplementedException();
    }

    public void EliminarEventoPorId(int eventoId)
    {
        throw new NotImplementedException();
    }

    public Evento ObtenerEventoPorId(int eventoId)
    {
        throw new NotImplementedException();
    }

    public List<Evento> ListarEventos()
    {
        throw new NotImplementedException();
    }

    public void ActualizarEvento(Evento evento)
    {
        throw new NotImplementedException();
    }
}
