using Parque.Dominio;
using Parque.Infraestructura.Repositorios;

namespace Parque.Aplicacion;

public class ServicioEvento(IRepositorio<Evento> repositorioEvento) : IServicioEvento
{
    private readonly IRepositorio<Evento> _repositorioEvento = repositorioEvento;
    public Evento AgregarEvento(Evento evento)
    {
        _repositorioEvento.Agregar(evento);
        return evento;
    }
}

