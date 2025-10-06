using Parque.Dominio.Usuarios;

namespace Parque.Aplicacion.DTOs.Usuarios;

public record RegistrarCuentaDto(
    string Nombre,
    string Apellido,
    string Email,
    string Password,
    Rol Rol,
    DateTime? FechaNacimiento,
    NivelMembresia? NivelMembresia
);
