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
        ArgumentNullException.ThrowIfNull(evento);

        if (evento.Inicio >= evento.Fin)
        {
            throw new ArgumentException("La fecha de inicio puede ser anterior a la fecha de fin.");
        }

        if (evento.AforoMaximo <= 0)
        {
            throw new ArgumentException("El aforo debe ser mayor a cero.");
        }

        if (evento.CostoAdicional < 0)
        {
            throw new ArgumentException("El costo adicional no puede ser negativo.");
        }

        _repositorioEvento.Agregar(evento);
        return evento;
    }

    public void EliminarEventoPorId(int eventoId)
    {
        if (eventoId <= 0)
        {
            throw new Exception("El ID debe ser mayor a cero.");
        }

        Evento evento = _repositorioEvento.Encontrar(e => e.Id == eventoId);

        if (evento == null)
        {
            throw new Exception($"No se encontró un evento con ID {eventoId}.");
        }

        _repositorioEvento.Eliminar(e => e.Id == eventoId);
    }

    public Evento ObtenerEventoPorId(int eventoId)
    {
        if (eventoId <= 0)
        {
            throw new Exception("El ID debe ser mayor a cero.");
        }

        Evento evento = _repositorioEvento.Encontrar(e => e.Id == eventoId);
        if (evento == null)
        {
            throw new Exception($"No se encontró un evento con ID {eventoId}.");
        }

        return evento;
    }

    public List<Evento> ListarEventos()
    {
        return _repositorioEvento.ObtenerTodos();
    }

    public void ActualizarEvento(Evento evento)
    {
        ArgumentNullException.ThrowIfNull(evento);

        if (evento.Id <= 0)
        {
            throw new Exception("El ID del evento debe ser mayor a cero.");
        }

        if (evento.Inicio >= evento.Fin)
        {
            throw new ArgumentException("La fecha de inicio puede ser anterior a la fecha de fin.");
        }

        if (evento.AforoMaximo <= 0)
        {
            throw new ArgumentException("El aforo debe ser mayor a cero.");
        }

        if (evento.CostoAdicional < 0)
        {
            throw new ArgumentException("El costo adicional no puede ser negativo.");
        }

        _repositorioEvento.Editar(evento);
    }
}
