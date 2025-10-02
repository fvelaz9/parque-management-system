using Microsoft.AspNetCore.Mvc;
using Parque.Aplicacion.Servicios;

namespace Parque.WebApi.Controllers;
[Route("api/[controller]")]
[ApiController]
public class FechaHoraController(IServicioFechaHora timeService) : ControllerBase
{
}
