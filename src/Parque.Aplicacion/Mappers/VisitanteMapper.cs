using Parque.Aplicacion.DTOs.Usuarios;
using Parque.Dominio.Usuarios;

namespace Parque.Aplicacion.Mappers;

public static class VisitanteMapper
{
    public static VisitanteDto ToDto(this Visitante visitante)
    {
        var edad = CalcularEdad(visitante.FechaNacimiento);
        return new VisitanteDto(
            visitante.Id,
            visitante.FechaNacimiento,
            edad,
            visitante.NivelMembresia.ToString(),
            0,
            0);
    }

    private static int CalcularEdad(DateTime fechaNacimiento)
    {
        var hoy = DateTime.Today;
        var edad = hoy.Year - fechaNacimiento.Year;
        if (fechaNacimiento.Date > hoy.AddYears(-edad))
        {
            edad--;
        }

        return edad;
    }
}
