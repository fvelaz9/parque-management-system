namespace Parque.WebApi.Evento.Modelos;

public class CreateEventoResponse(Evento evento)
{
    public int Id { get; set; } = evento.Id;
}
