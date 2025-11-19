using Parque.Dominio.Usuarios;

namespace Parque.Aplicacion.DTOs.Usuarios;
public record RegistrarCuentaDto(
    string Nombre,
    string Apellido,
    string Email,
    string Password,
    List<Rol> Roles,
    DateTime? FechaNacimiento,
    NivelMembresia? NivelMembresia
);
