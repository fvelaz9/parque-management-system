using Parque.Dominio.Excepciones;

namespace Parque.Dominio.Usuarios;
public class Cuenta
{
    public Guid Id { get; private set; }
    public string Nombre { get; private set; } = string.Empty;
    public string Apellido { get; private set; } = string.Empty;
    public Email Email { get; private set; } = default!;
    public string Password { get; private set; } = string.Empty;
    public Visitante? Visitante { get; private set; }

    // Se evita duplicación de roles usando un HashSet
    private readonly HashSet<Rol> _roles = [];
    public IReadOnlyCollection<Rol> Roles => _roles;
    private Cuenta()
    {
    }

    public static Cuenta Crear(string nombre, string apellido, Email email, string password)
    {
        ValidarCreacion(nombre, apellido, email, password);

        var cuenta = new Cuenta();
        cuenta.Id = Guid.NewGuid();
        cuenta.Nombre = nombre;
        cuenta.Apellido = apellido;
        cuenta.Email = email;
        cuenta.Password = password;
        cuenta._roles.Add(Rol.Visitante);
        return cuenta;
    }

    public void AsignarVisitante(DateTime fechaNacimiento)
    {
        if (Visitante != null)
        {
            throw new ExcepcionDominio("Esta cuenta ya tiene un visitante asignado");
        }

        Visitante = Visitante.Crear(fechaNacimiento);
    }

    private static void ValidarCreacion(string nombre, string apellido, Email email, string password)
    {
        ValidarCampoRequerido(nombre, "Nombre");
        ValidarMaximoCaracteres(nombre, 100, "Nombre");

        ValidarCampoRequerido(apellido, "Apellido");
        ValidarMaximoCaracteres(apellido, 100, "Apellido");

        ValidarCampoRequerido(password, "Password");
        ValidarMaximoCaracteres(password, 100, "Password");

        ValidarObjetoRequerido(email, "Email");
    }

    private static void ValidarCampoRequerido(string valor, string nombreCampo)
    {
        if(string.IsNullOrWhiteSpace(valor))
        {
            throw new ExcepcionDominio($"{nombreCampo} es requerido");
        }
    }

    private static void ValidarMaximoCaracteres(string valor, int maxCaracteres, string nombreCampo)
    {
        if(valor.Length > maxCaracteres)
        {
            throw new ExcepcionDominio($"{nombreCampo} no puede tener más de {maxCaracteres} caracteres");
        }
    }

    private static void ValidarObjetoRequerido(object obj, string nombreCampo)
    {
        if(obj == null)
        {
            throw new ExcepcionDominio($"{nombreCampo} es requerido");
        }
    }

    public void ActualizarNombre(string nombre)
    {
        ValidarCampoRequerido(nombre, "Nombre");
        ValidarMaximoCaracteres(nombre, 100, "Nombre");
        Nombre = nombre;
    }

    public void ActualizarApellido(string apellido)
    {
        ValidarCampoRequerido(apellido, "Apellido");
        ValidarMaximoCaracteres(apellido, 100, "Apellido");
        Apellido = apellido;
    }

    public void ActualizarPassword(string password)
    {
        ValidarCampoRequerido(password, "Password");
        ValidarMaximoCaracteres(password, 100, "Password");
        Password = password;
    }

    public void ActualizarEmail(Email email)
    {
        ValidarObjetoRequerido(email, "Email");
        Email = email;
    }

    public void AgregarRol(Rol rol)
    {
        _roles.Add(rol);
    }
}
