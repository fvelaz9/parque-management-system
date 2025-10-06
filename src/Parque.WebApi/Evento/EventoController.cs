using Microsoft.AspNetCore.Mvc;
using Parque.Aplicacion;
using Parque.WebApi.Evento.Modelos;

namespace Parque.WebApi.Evento;
[ApiController]
[Route("api/evento")]
public class EventoController(IServicioEvento servicioEvento) : ControllerBase
{
    [HttpPost]
    public CreateEventoResponse Crear(CreateEventoRequest request)
    {
        if (request == null)
        {
            throw new Exception("Request no puede ser null");
        }

        if (string.IsNullOrEmpty(request.Titulo))
        {
            throw new Exception("El título no puede ser vacío");
        }

        if (request.AforoMaximo <= 0)
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
            request.Estado
        );

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
}
