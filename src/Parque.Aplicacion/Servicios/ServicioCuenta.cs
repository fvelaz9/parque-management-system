using Parque.Aplicacion.DTOs.Usuarios;
using Parque.Dominio.Usuarios;
using Parque.Infraestructura.Repositorios;

namespace Parque.Aplicacion.Servicios;
public class ServicioCuenta(IRepositorio<Cuenta> cuentaRepo, IPasswordHashService passwordHashService) : IServicioCuenta
{
    private readonly IRepositorio<Cuenta> _cuentaRepo = cuentaRepo;
    private readonly IPasswordHashService _passwordHashService = passwordHashService;

    public Cuenta RegistrarVisitante(RegistrarVisitanteDto dto)
    {
        throw new NotImplementedException();
    }
}
