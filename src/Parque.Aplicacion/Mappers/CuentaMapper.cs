using Parque.Aplicacion.DTOs.Usuarios;
using Parque.Dominio.Usuarios;

namespace Parque.Aplicacion.Mappers;

public static class CuentaMapper
{
    public static CuentaDto ToDto(this Cuenta cuenta)
    {
        return new CuentaDto(
            cuenta.Id,
            cuenta.Nombre,
            cuenta.Apellido,
            cuenta.Email.Valor,
            cuenta.Roles.Select(r => r.ToString()),
            cuenta.Visitante?.ToDto());
    }
}
