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

        ValidarDatosModificacion(dto);

        if(!string.IsNullOrWhiteSpace(dto.Email))
        {
            ValidarEmailUnicoParaActualizacion(dto.Email, cuentaId);
        }

        ActualizarDatosPersonales(cuenta, dto);
        ActualizarFechaVisitante(cuenta, dto.FechaNacimiento);

        cuentaRepo.Editar(cuenta);
    }

    public void CambiarNivelMembresia(Guid cuentaId, NivelMembresia nuevoNivel)
    {
        var cuenta = ObtenerCuenta(cuentaId);

        if(cuenta.Visitante == null)
        {
            throw new ExcepcionDominio("Solo las cuentas con perfil de visitante tienen nivel de membresía");
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
            ?? throw new ExcepcionEntidadNoEncontrada($"Cuenta con email {email} no encontrada");

        return cuenta.ToDto();
    }

    public IEnumerable<CuentaDto> ObtenerTodas()
    {
        var cuentas = cuentaRepo.ObtenerConRelaciones(c => true, "Visitante");
        return (cuentas ?? Enumerable.Empty<Cuenta>()).Select(c => c.ToDto());
    }

    private void AsignarPerfilVisitante(Cuenta cuenta, RegistrarCuentaDto dto)
    {
        if(!dto.FechaNacimiento.HasValue)
        {
            throw new ExcepcionDominio("La fecha de nacimiento es requerida para visitantes");
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
            throw new ExcepcionDominio("Ya existe una cuenta con este email");
        }
    }

    private void ValidarEmailUnicoParaActualizacion(string email, Guid cuentaId)
    {
        var cuentaExistente = cuentaRepo.Encontrar(c => c.Email.Valor == email);

        if(cuentaExistente != null && cuentaExistente.Id != cuentaId)
        {
            throw new ExcepcionDominio("Ya existe una cuenta con este email");
        }
    }

    private static void ValidarDatosModificacion(ModificarPerfilDto dto)
    {
        if(dto.Nombre != null && string.IsNullOrWhiteSpace(dto.Nombre))
        {
            throw new ExcepcionDominio("El nombre no puede estar vacío");
        }

        if(dto.Apellido != null && string.IsNullOrWhiteSpace(dto.Apellido))
        {
            throw new ExcepcionDominio("El apellido no puede estar vacío");
        }

        if(dto.Email != null && string.IsNullOrWhiteSpace(dto.Email))
        {
            throw new ExcepcionDominio("El email no puede estar vacío");
        }
    }

    private static Cuenta CrearCuentaBase(string nombre, string apellido, string email, string password, Rol rolInicial)
    {
        var emailObj = new Email(email);
        return Cuenta.Crear(nombre, apellido, emailObj, password, rolInicial);
    }

    public Cuenta ObtenerCuenta(Guid cuentaId)
    {
        return cuentaRepo.EncontrarConRelaciones(c => c.Id == cuentaId, "Visitante")
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

    public List<CuentaDto> ObtenerCuentasVisitantes()
    {
        var todasLasCuentas = cuentaRepo.ObtenerConRelaciones(
            c => true,
            "Visitante");

        var cuentasVisitantes = todasLasCuentas
            .Where(c => c.Roles.Any(r => r == Rol.Visitante))
            .ToList();

        var resultado = cuentasVisitantes.Select(c => c.ToDto()).ToList();

        return resultado;
    }
}
