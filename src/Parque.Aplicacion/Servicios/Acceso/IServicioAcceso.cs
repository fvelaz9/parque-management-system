using Parque.Aplicacion.DTOS;
using Parque.Dominio.Atracciones;

namespace Parque.Aplicacion.Servicios.Acceso;

public interface IServicioAcceso
{
    ValidarAccesoRespuesta ValidarAcceso(ValidarAccesoRequest request);
    RegistroVisita RegistrarIngreso(Guid codigoTicket, int atraccionId);
}
