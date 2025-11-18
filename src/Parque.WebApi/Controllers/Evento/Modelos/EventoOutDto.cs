using Parque.Dominio;

namespace Parque.WebApi.Controllers.Evento.Modelos;

public class EventoOutDto(Dominio.Evento evento)
{
    public int Id { get; set; } = evento.Id;
    public string Titulo { get; set; } = evento.Titulo;
    public string Descripcion { get; set; } = evento.Descripcion;
    public DateTime Inicio { get; set; } = evento.Inicio;
    public DateTime Fin { get; set; } = evento.Fin;
    public int AforoMaximo { get; set; } = evento.AforoMaximo;
    public float CostoAdicional { get; set; } = evento.CostoAdicional;
    public EstadoEvento Estado { get; set; } = evento.Estado;
}
