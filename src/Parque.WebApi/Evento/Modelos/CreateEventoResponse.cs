namespace Parque.WebApi.Evento.Modelos;

public class CreateEventoResponse(Dominio.Evento evento)
{
    public int Id { get; set; } = evento.Id;
}
