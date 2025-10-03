using Parque.Dominio.Usuarios;

namespace Parque.Aplicacion.Servicios;
public interface IPasswordHashService
{
    PasswordHash HashPassword(string plainPassword);
    bool VerifyPassword(string plainPassword, PasswordHash passwordHash);
}
