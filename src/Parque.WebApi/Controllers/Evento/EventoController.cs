using Microsoft.AspNetCore.Mvc;
using Parque.Aplicacion.Servicios;
using Parque.WebApi.Controllers.Evento.Modelos;

namespace Parque.WebApi.Controllers.Evento;
[ApiController]
[Route("api/evento")]
public class EventoController(IServicioEvento servicioEvento) : ControllerBase
{
    [HttpPost]
    public CreateEventoResponse Crear(CreateEventoRequest request)
    {
        if(request == null)
        {
            throw new Exception("Request no puede ser null");
        }

        if(string.IsNullOrEmpty(request.Titulo))
        {
            throw new Exception("El título no puede ser vacío");
        }

        if(request.AforoMaximo <= 0)
        {
            throw new Exception("El aforo debe ser mayor a cero");
        }

        var evento = new Dominio.Evento(
            request.Titulo,
            request.Descripcion,
            request.Inicio,
            request.Fin,
            request.AforoMaximo,
            request.CostoAdicional,
            request.Estado);
        evento.Atracciones = request.Atracciones;
        evento = servicioEvento.AgregarEvento(evento);

        return new CreateEventoResponse(evento);
    }

    [HttpGet]
    public List<EventoOutDto> Listar()
    {
        return servicioEvento.ListarEventos().Select(e => new EventoOutDto(e)).ToList();
    }

    [HttpGet("{eventoId}")]
    public EventoOutDto ObtenerPorId(int eventoId)
    {
        Dominio.Evento evento = servicioEvento.ObtenerEventoPorId(eventoId);
        return new EventoOutDto(evento);
    }

    [HttpDelete("{eventoId}")]
    public void Eliminar(int eventoId)
    {
        servicioEvento.EliminarEventoPorId(eventoId);
    }

    [HttpPut("{eventoId}")]
    public void Actualizar(int eventoId, UpdateEventoRequest request)
    {
        if(request == null)
        {
            throw new Exception("El request no puede ser null");
        }

        Dominio.Evento eventoExistente = servicioEvento.ObtenerEventoPorId(eventoId);

        if(!string.IsNullOrEmpty(request.Titulo))
        {
            eventoExistente.Titulo = request.Titulo;
        }

        if(!string.IsNullOrEmpty(request.Descripcion))
        {
            eventoExistente.Descripcion = request.Descripcion;
        }

        if(request.Inicio.HasValue)
        {
            eventoExistente.Inicio = request.Inicio.Value;
        }

        if(request.Fin.HasValue)
        {
            eventoExistente.Fin = request.Fin.Value;
        }

        if(request.AforoMaximo.HasValue)
        {
            eventoExistente.AforoMaximo = request.AforoMaximo.Value;
        }

        if(request.CostoAdicional.HasValue)
        {
            eventoExistente.CostoAdicional = request.CostoAdicional.Value;
        }

        if(request.Estado.HasValue)
        {
            eventoExistente.Estado = request.Estado.Value;
        }

        servicioEvento.ActualizarEvento(eventoExistente);
    }
}
