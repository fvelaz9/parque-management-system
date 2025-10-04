using Parque.Aplicacion.DTOs.Usuarios;
using Parque.Dominio.Usuarios;

namespace Parque.Aplicacion.Servicios;
public interface IServicioCuenta
{
    CuentaDto RegistrarVisitante(RegistrarVisitanteDto dto);
    CuentaDto CrearCuentaPorAdmin(RegistrarCuentaDto dto);
    void ModificarPerfil(Guid cuentaId, ModificarPerfilDto dto);
    void CambiarNivelMembresia(Guid cuentaId, NivelMembresia nuevoNivel);
    CuentaDto ObtenerPorId(Guid id);
    CuentaDto ObtenerPorEmail(string email);
}
