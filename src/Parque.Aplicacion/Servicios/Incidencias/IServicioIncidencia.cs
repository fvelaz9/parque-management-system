using Parque.Aplicacion.DTOs;
using Parque.Dominio;

namespace Parque.Aplicacion.Servicios.Incidencias;

public interface IServicioIncidencia
{
    Incidencia CrearIncidencia(CrearIncidenciaRequest request);
    void ResolverIncidencia(int incidenciaId);
    bool EstaDisponible(int atraccionId);
}
