using Parque.Aplicacion.DTOs.Usuarios;
using Parque.Dominio.Usuarios;

namespace Parque.Aplicacion.Servicios;
public interface IServicioCuenta
{
    Cuenta RegistrarVisitante(RegistrarVisitanteDto dto);
}
