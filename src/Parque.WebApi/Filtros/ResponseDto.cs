using System.Diagnostics.CodeAnalysis;

namespace Parque.WebApi.Filtros;
[ExcludeFromCodeCoverage]
public record ResponseDto
{
    public object? Content { get; set; }
    public bool ExecutionSuccessful { get; set; }
    public string? Message { get; set; }
}
