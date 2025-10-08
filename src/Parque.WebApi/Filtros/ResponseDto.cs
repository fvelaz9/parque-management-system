namespace Parque.WebApi.Filtros;

public record ResponseDto
{
    public object? Content { get; set; }
    public bool ExecutionSuccessful { get; set; }
    public string? Message { get; set; }
}
