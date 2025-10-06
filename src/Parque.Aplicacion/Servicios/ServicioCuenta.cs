using Parque.Aplicacion.DTOs.Usuarios;
using Parque.Aplicacion.Mappers;
using Parque.Dominio.Excepciones;
using Parque.Dominio.Usuarios;
using Parque.Infraestructura.Repositorios;

namespace Parque.Aplicacion.Servicios;
public class ServicioCuenta(IRepositorio<Cuenta> cuentaRepo) : IServicioCuenta
{
    public CuentaDto RegistrarVisitante(RegistrarVisitanteDto dto)
    {
        ValidarEmailUnico(dto.Email);

        var cuenta = CrearCuentaBase(dto.Nombre, dto.Apellido, dto.Email, dto.Password, Rol.Visitante);
        cuenta.AsignarVisitante(dto.FechaNacimiento);

        cuentaRepo.Agregar(cuenta);
        return cuenta.ToDto();
    }

    public CuentaDto CrearCuentaPorAdmin(RegistrarCuentaDto dto)
    {
        ValidarEmailUnico(dto.Email);

        var cuenta = CrearCuentaBase(dto.Nombre, dto.Apellido, dto.Email, dto.Password, dto.Rol);

        if(dto.Rol == Rol.Visitante)
        {
            AsignarPerfilVisitante(cuenta, dto);
        }

        cuentaRepo.Agregar(cuenta);
        return cuenta.ToDto();
    }

    public void ModificarPerfil(Guid cuentaId, ModificarPerfilDto dto)
    {
        var cuenta = ObtenerCuenta(cuentaId);

        ActualizarDatosPersonales(cuenta, dto);
        ActualizarFechaVisitante(cuenta, dto.FechaNacimiento);

        cuentaRepo.Editar(cuenta);
    }

    public void CambiarNivelMembresia(Guid cuentaId, NivelMembresia nuevoNivel)
    {
        var cuenta = ObtenerCuenta(cuentaId);

        if(cuenta.Visitante == null)
        {
            throw new ExcepcionDominio("Solo las cuentas con perfil de visitante tienen nivel de membresía.");
        }

        cuenta.Visitante.AsignarMembresia(nuevoNivel);
        cuentaRepo.Editar(cuenta);
    }

    public CuentaDto ObtenerPorId(Guid id)
    {
        var cuenta = ObtenerCuenta(id);
        return cuenta.ToDto();
    }

    public CuentaDto ObtenerPorEmail(string email)
    {
        var cuenta = cuentaRepo.Encontrar(c => c.Email.Valor == email)
            ?? throw new ExcepcionEntidadNoEncontrada($"Cuenta con email {email} no encontrada.");

        return cuenta.ToDto();
    }

    private void AsignarPerfilVisitante(Cuenta cuenta, RegistrarCuentaDto dto)
    {
        if(!dto.FechaNacimiento.HasValue)
        {
            throw new ExcepcionDominio("La fecha de nacimiento es requerida para visitantes.");
        }

        cuenta.AsignarVisitante(dto.FechaNacimiento.Value);

        if(dto.NivelMembresia.HasValue)
        {
            cuenta.Visitante!.AsignarMembresia(dto.NivelMembresia.Value);
        }
    }

    private void ValidarEmailUnico(string email)
    {
        var cuentaExistente = cuentaRepo.Encontrar(c => c.Email.Valor == email);
        if(cuentaExistente != null)
        {
            throw new ExcepcionDominio("Ya existe una cuenta con este email.");
        }
    }

    private static Cuenta CrearCuentaBase(string nombre, string apellido, string email, string password, Rol rolInicial)
    {
        var emailObj = new Email(email);
        return Cuenta.Crear(nombre, apellido, emailObj, password, rolInicial);
    }

    private Cuenta ObtenerCuenta(Guid cuentaId)
    {
        return cuentaRepo.Encontrar(c => c.Id == cuentaId)
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
