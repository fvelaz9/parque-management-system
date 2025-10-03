using Parque.Aplicacion.DTOs.Usuarios;
using Parque.Aplicacion.Mappers;
using Parque.Dominio.Excepciones;
using Parque.Dominio.Usuarios;
using Parque.Infraestructura.Repositorios;

namespace Parque.Aplicacion.Servicios;
public class ServicioCuenta(IRepositorio<Cuenta> cuentaRepo) : IServicioCuenta
{
    private readonly IRepositorio<Cuenta> _cuentaRepo = cuentaRepo;

    public CuentaDto RegistrarVisitante(RegistrarVisitanteDto dto)
    {
        var cuentaExistente = _cuentaRepo.Encontrar(c => c.Email.Valor == dto.Email);
        if(cuentaExistente != null)
        {
            throw new ExcepcionDominio("Ya existe una cuenta con este email.");
        }

        var email = new Email(dto.Email);

        var cuenta = Cuenta.Crear(dto.Nombre, dto.Apellido, email, dto.Password);
        cuenta.AsignarVisitante(dto.FechaNacimiento);

        _cuentaRepo.Agregar(cuenta);

        return cuenta.ToDto();
    }

    public void ModificarPerfil(Guid cuentaId, ModificarPerfilDto dto)
    {
        var cuenta = _cuentaRepo.Encontrar(c => c.Id == cuentaId);
        cuenta.ActualizarNombre("Carlos");
        cuenta.ActualizarApellido("Gómez");
        cuenta.ActualizarEmail(new Email("carlos@test.com"));
        cuenta.Visitante.ActualizarFecha(new DateTime(1985, 5, 15));

        _cuentaRepo.Editar(cuenta);
    }
}
