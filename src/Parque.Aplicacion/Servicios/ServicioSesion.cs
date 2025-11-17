using Parque.Dominio;
using Parque.Dominio.Excepciones;
using Parque.Dominio.Usuarios;
using Parque.Infraestructura.Repositorios;

namespace Parque.Aplicacion.Servicios;

public class ServicioSesion(IRepositorio<Sesion> sesionRepo, IRepositorio<Cuenta> cuentaRepo) : IServicioSesion
{
    public Sesion AgregarSesion(string email, string password)
    {
        var cuenta = cuentaRepo.Encontrar(c => c.Email.Valor == email)
            ?? throw new ExcepcionDominio("Email no encontrado");

        if(!cuenta.Password.Equals(password))
        {
            throw new ExcepcionDominio("Password incorrecto");
        }

        var nuevaSesion = new Sesion
        {
            Token = Guid.NewGuid().ToString(),
            UsuarioId = cuenta.Id
        };

        sesionRepo.Agregar(nuevaSesion);
        return nuevaSesion;
    }

    public Cuenta ObtenerUsuarioSesion(string token)
    {
        var sesion = sesionRepo.Encontrar(s => s.Token == token)
                     ?? throw new ExcepcionDominio("Token inválido");

        var cuenta = cuentaRepo.EncontrarConRelaciones(c => c.Id == sesion.UsuarioId, "Visitante")
            ?? throw new ExcepcionDominio("Usuario no encontrado");

        return cuenta;
    }

    public bool ValidarSesion(string token, string rol)
    {
        var sesion = sesionRepo.Encontrar(s => s.Token == token);

        if(sesion == null)
        {
            throw new ExcepcionDominio("Token inválido");
        }

        var cuenta = cuentaRepo.Encontrar(c => c.Id == sesion.UsuarioId)
            ?? throw new ExcepcionDominio("Usuario no encontrado");

        if(rol.Equals("any", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        if(Enum.TryParse<Rol>(rol, ignoreCase: true, out var rolEnum))
        {
            return cuenta.Roles.Contains(rolEnum);
        }

        return false;
    }

    public void EliminarSesion(string token)
    {
        var sesion = sesionRepo.Encontrar(s => s.Token == token)
            ?? throw new ExcepcionDominio("Token inválido");

        sesionRepo.Eliminar(s => s.Id == sesion.Id);
    }
}
