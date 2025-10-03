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
        var cuenta = ObtenerCuenta(cuentaId);

        ActualizarDatosPersonales(cuenta, dto);
        ActualizarFechaVisitante(cuenta, dto.FechaNacimiento);

        _cuentaRepo.Editar(cuenta);
    }

    private Cuenta ObtenerCuenta(Guid cuentaId)
    {
        return _cuentaRepo.Encontrar(c => c.Id == cuentaId)
            ?? throw new ExcepcionEntidadNoEncontrada("Cuenta no encontrada");
    }

    private static void ActualizarDatosPersonales(Cuenta cuenta, ModificarPerfilDto dto)
    {
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
    }

    private static void ActualizarFechaVisitante(Cuenta cuenta, DateTime? fecha)
    {
        if(fecha.HasValue && cuenta.Visitante != null)
        {
            cuenta.Visitante.ActualizarFecha(fecha.Value);
        }
    }
}
