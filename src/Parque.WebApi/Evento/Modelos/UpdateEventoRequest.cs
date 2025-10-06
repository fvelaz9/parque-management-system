using Parque.Dominio;

namespace Parque.WebApi.Evento.Modelos;

public class UpdateEventoRequest
{
    public string? Titulo { get; set; }
    public string? Descripcion { get; set; }
    public DateTime? Inicio { get; set; }
    public DateTime? Fin { get; set; }
    public int? AforoMaximo { get; set; }
    public float? CostoAdicional { get; set; }
    public EstadoEvento? Estado { get; set; }
}
