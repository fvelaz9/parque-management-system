namespace Parque.Aplicacion.DTOs.Usuarios;
public record RegistrarVisitanteDto(
    string Nombre,
    string Apellido,
    string Email,
    string Password,
    DateTime FechaNacimiento
);
