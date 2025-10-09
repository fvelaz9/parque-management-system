using Parque.Dominio;
using Parque.Dominio.Excepciones;
using Parque.Infraestructura.Repositorios;

namespace Parque.Aplicacion.Servicios;

public class ServicioEvento(IRepositorio<Evento> repositorioEvento) : IServicioEvento
{
    public Evento AgregarEvento(Evento evento)
    {
        ArgumentNullException.ThrowIfNull(evento);
        ValidarDatosEvento(evento);

        repositorioEvento.Agregar(evento);
        return evento;
    }

    public void EliminarEventoPorId(int eventoId)
    {
        if(eventoId <= 0)
        {
            throw new ArgumentException("El ID debe ser mayor a cero");
        }

        Evento? evento = repositorioEvento.Encontrar(e => e.Id == eventoId);

        if(evento == null)
        {
            throw new ExcepcionEntidadNoEncontrada($"No se encontró un evento con ID {eventoId}");
        }

        repositorioEvento.Eliminar(e => e.Id == eventoId);
    }

    public Evento ObtenerEventoPorId(int eventoId)
    {
        if(eventoId <= 0)
        {
            throw new ArgumentException("El ID debe ser mayor a cero");
        }

        Evento? evento = repositorioEvento.Encontrar(e => e.Id == eventoId);
        if(evento == null)
        {
            throw new ExcepcionEntidadNoEncontrada($"No se encontró un evento con ID {eventoId}");
        }

        return evento;
    }

    public List<Evento> ListarEventos()
    {
        return repositorioEvento.ObtenerTodos();
    }

    public void ActualizarEvento(Evento evento)
    {
        ArgumentNullException.ThrowIfNull(evento);

        if(evento.Id <= 0)
        {
            throw new ArgumentException("El ID del evento debe ser mayor a cero");
        }

        ValidarDatosEvento(evento);

        repositorioEvento.Editar(evento);
    }

    private static void ValidarDatosEvento(Evento evento)
    {
        if(string.IsNullOrWhiteSpace(evento.Titulo))
        {
            throw new ArgumentException("El título del evento es requerido");
        }

        if(string.IsNullOrWhiteSpace(evento.Descripcion))
        {
            throw new ArgumentException("La descripción del evento es requerida");
        }

        if(evento.Inicio >= evento.Fin)
        {
            throw new ArgumentException("La fecha de inicio debe ser anterior a la fecha de fin");
        }

        if(evento.AforoMaximo <= 0)
        {
            throw new ArgumentException("El aforo debe ser mayor a cero");
        }

        if(evento.CostoAdicional < 0)
        {
            throw new ArgumentException("El costo adicional no puede ser negativo");
        }
    }
}
