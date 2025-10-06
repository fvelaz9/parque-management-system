namespace Parque.WebApi.Controllers.Evento.Modelos;

public class EventoOutDto(Dominio.Evento evento)
{
    public string Titulo { get; set; } = evento.Titulo;
}
