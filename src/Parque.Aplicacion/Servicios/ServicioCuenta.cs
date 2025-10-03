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
        if(cuenta == null)
        {
            throw new ExcepcionEntidadNoEncontrada("Cuenta no encontrada");
        }

        if(!string.IsNullOrWhiteSpace(dto.Nombre))
        {
            cuenta.ActualizarNombre(dto.Nombre);
        }

        if(!string.IsNullOrWhiteSpace(dto.Apellido))
        {
            cuenta.ActualizarApellido(dto.Apellido);
        }

        if(!string.IsNullOrWhiteSpace(dto.Email))
        {
            cuenta.ActualizarEmail(new Email(dto.Email));
        }

        if(dto.FechaNacimiento.HasValue && cuenta.Visitante != null)
        {
            cuenta.Visitante.ActualizarFecha(dto.FechaNacimiento.Value);
        }

        _cuentaRepo.Editar(cuenta);
    }
}
