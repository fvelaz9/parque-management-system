namespace Parque.Aplicacion.DTOs.Usuarios;
public record CuentaDto(
    Guid Id,
    string Nombre,
    string Apellido,
    string Email,
    IEnumerable<string> Roles,
    VisitanteDto? Visitante
);
