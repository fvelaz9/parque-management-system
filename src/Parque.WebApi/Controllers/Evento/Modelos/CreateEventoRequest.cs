using Parque.Dominio;

namespace Parque.WebApi.Controllers.Evento.Modelos;

public class CreateEventoRequest
{
    public string Titulo { get; set; } = null!;
    public string Descripcion { get; set; } = null!;
    public DateTime Inicio { get; set; }
    public DateTime Fin { get; set; }
    public int AforoMaximo { get; set; }
    public float CostoAdicional { get; set; }
    public List<int> AtraccionIds { get; set; } = [];
    public EstadoEvento Estado { get; set; }
}
