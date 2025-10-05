using Parque.Dominio;
using Parque.Infraestructura.Repositorios;

namespace Parque.Aplicacion;

public class ServicioEvento(IRepositorio<Evento> repositorioEvento) : IServicioEvento
{
    private readonly IRepositorio<Evento> _repositorioEvento = repositorioEvento;
   public Evento AgregarEvento(Evento evento)
        {
            if (evento == null)
            {
                throw new Exception("El evento no puede ser nulo.");
            }

            if (string.IsNullOrEmpty(evento.Titulo))
            {
                throw new Exception("El evento debe tener un título.");
            }

            if (evento.Inicio >= evento.Fin)
            {
                throw new Exception("La fecha de inicio debe ser anterior a la de fin.");
            }

            if (evento.AforoMaximo <= 0)
            {
                throw new Exception("El aforo máximo debe ser mayor a cero.");
            }

            if (evento.CostoAdicional < 0)
            {
                throw new Exception("El costo adicional no puede ser negativo.");
            }

            var existe = _repositorioEvento
                .Obtener(e => e.Titulo.ToLower() == evento.Titulo.ToLower())
                .Any();

            if (existe)
            {
                throw new Exception($"Ya existe un evento con el título '{evento.Titulo}'.");
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

            var evento = _repositorioEvento.Encontrar(e => e.Id == eventoId);
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

            var evento = _repositorioEvento.Encontrar(e => e.Id == eventoId);
            if (evento == null)
            {
                throw new Exception($"No se encontró un evento con ID {eventoId}.");
            }

            return evento;
        }

        public List<Evento> ListarEventos()
        {
            var eventos = _repositorioEvento.ObtenerTodos();
            if (eventos == null || eventos.Count == 0)
            {
                throw new Exception("No existen eventos registrados.");
            }

            return eventos;
        }

        public void ActualizarEvento(Evento evento)
        {
            if (evento == null)
            {
                throw new Exception("El evento no puede ser nulo.");
            }

            var existente = _repositorioEvento.Encontrar(e => e.Id == evento.Id);
            if (existente == null)
            {
                throw new Exception($"No existe un evento con ID {evento.Id}.");
            }

            if (evento.Inicio >= evento.Fin)
            {
                throw new Exception("La fecha de inicio debe ser anterior a la de fin.");
            }

            if (evento.AforoMaximo <= 0)
            {
                throw new Exception("El aforo máximo debe ser mayor a cero.");
            }

            if (evento.CostoAdicional < 0)
            {
                throw new Exception("El costo adicional no puede ser negativo.");
            }

            existente.Titulo = evento.Titulo;
            existente.Descripcion = evento.Descripcion;
            existente.Inicio = evento.Inicio;
            existente.Fin = evento.Fin;
            existente.AforoMaximo = evento.AforoMaximo;
            existente.CostoAdicional = evento.CostoAdicional;
            existente.Estado = evento.Estado;

            _repositorioEvento.Editar(existente);
        }
}
