using Parque.Aplicacion.DTOs.Usuarios;
using Parque.Aplicacion.Mappers;
using Parque.Dominio.Excepciones;
using Parque.Dominio.Usuarios;
using Parque.Infraestructura.Repositorios;

namespace Parque.Aplicacion.Servicios;
public class ServicioCuenta(IRepositorio<Cuenta> cuentaRepo, IPasswordHashService passwordHashService) : IServicioCuenta
{
    private readonly IRepositorio<Cuenta> _cuentaRepo = cuentaRepo;
    private readonly IPasswordHashService _passwordHashService = passwordHashService;

    public CuentaDto RegistrarVisitante(RegistrarVisitanteDto dto)
    {
        var cuentaExistente = _cuentaRepo.Encontrar(c => c.Email.Valor == dto.Email);
        if(cuentaExistente != null)
        {
            throw new ExcepcionDominio("Ya existe una cuenta con este email.");
        }

        var email = new Email(dto.Email);
        var passwordHash = _passwordHashService.HashPassword(dto.Password);

        var cuenta = Cuenta.Crear(dto.Nombre, dto.Apellido, email, passwordHash);
        cuenta.AsignarVisitante(dto.FechaNacimiento);

        _cuentaRepo.Agregar(cuenta);

        return cuenta.ToDto();
    }
}
