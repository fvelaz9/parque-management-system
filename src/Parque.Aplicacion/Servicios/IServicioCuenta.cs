using Parque.Aplicacion.DTOs.Usuarios;

namespace Parque.Aplicacion.Servicios;
public interface IServicioCuenta
{
    CuentaDto RegistrarVisitante(RegistrarVisitanteDto dto);
    void ModificarPerfil(Guid cuentaId, string nombre, string apellido);
}
