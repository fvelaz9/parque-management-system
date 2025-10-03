namespace Parque.Aplicacion.DTOs.Usuarios;
public record VisitanteDto(
    Guid Id,
    DateTime FechaNacimiento,
    int Edad,
    string NivelMembresia,
    int PuntosDiarios,
    int PuntosTotales
);
