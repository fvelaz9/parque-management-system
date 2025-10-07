using Parque.Aplicacion.DTOS;
using Parque.Dominio;

namespace Parque.Aplicacion.Servicios.Incidencias;

public interface IServicioIncidencia
{
    Incidencia CrearIncidencia(CrearIncidenciaRequest request);
    bool EstaDisponible(int atraccionId);
}
