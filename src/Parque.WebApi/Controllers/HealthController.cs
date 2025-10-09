using Microsoft.AspNetCore.Mvc;

namespace Parque.WebApi.Controllers;

[ApiController]
[Route("/", Name = "Ping")]
[Route("health", Name = "Health Check")]
public sealed class HealthController
  : ControllerBase
{
    [HttpGet]
    public object Get()
    {
        return new
        {
            v = "1.0",
            alive = true
        };
    }
}
