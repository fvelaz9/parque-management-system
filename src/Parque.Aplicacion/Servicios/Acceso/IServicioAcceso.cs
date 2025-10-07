using Parque.Aplicacion.DTOS;

namespace Parque.Aplicacion.Servicios.Acceso;

public interface IServicioAcceso
{
    ValidarAccesoRespuesta ValidarAcceso(ValidarAccesoRequest request);
    void RegistrarIngreso(Guid codigoTicket, int atraccionId);
}
