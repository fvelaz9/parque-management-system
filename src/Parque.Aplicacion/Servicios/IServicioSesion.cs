using Parque.Dominio;
using Parque.Dominio.Usuarios;

namespace Parque.Aplicacion.Servicios;
public interface IServicioSesion
{
    Sesion AgregarSesion(string email, string password);
    Cuenta ObtenerUsuarioSesion(string token);
    bool ValidarSesion(string token, string role);
    void EliminarSesion(string token);
}
