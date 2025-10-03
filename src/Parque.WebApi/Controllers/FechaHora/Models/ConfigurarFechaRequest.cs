namespace Parque.WebApi.Controllers.FechaHora.Models;

public sealed record class ConfigurarFechaRequest()
{
    public string? FechaHora { get; init; }
}
