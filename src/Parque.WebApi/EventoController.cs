using Microsoft.AspNetCore.Mvc;
using Parque.Aplicacion;

namespace Parque.WebApi;
[ApiController]
[Route("[controller]")]
public class EventoController(IServicioEvento _servicioEvento) : ControllerBase
{
}
