using Parque.Aplicacion.DTOS;
using Parque.Dominio.Atracciones;
using Parque.Dominio.Usuarios;

namespace Parque.Aplicacion.Servicios.Acceso;

public interface IServicioAcceso
{
    ValidarAccesoResponse ValidarAcceso(ValidarAccesoRequest request);
    RegistroVisita RegistrarIngreso(Guid codigoTicket, int atraccionId, Cuenta cuentaVisitante);
    RegistroVisita RegistrarEgreso(Guid codigoTicket, int atraccionId);
}
