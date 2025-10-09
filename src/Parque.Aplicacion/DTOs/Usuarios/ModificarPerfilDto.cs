namespace Parque.Aplicacion.DTOs.Usuarios;
public record ModificarPerfilDto(
    string? Nombre,
    string? Apellido,
    string? Email,
    DateTime? FechaNacimiento
);
