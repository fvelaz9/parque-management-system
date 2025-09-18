using Parque.Dominio.Excepciones;

namespace Parque.Dominio.Usuarios;
public class Cuenta
{
    public Guid Id { get; private set; }
    public string Nombre { get; private set; } = string.Empty;
    public string Apellido { get; private set; } = string.Empty;
    public Email Email { get; private set; } = default!;
    public PasswordHash PasswordHash { get; private set; } = default!;
    public Visitante? Visitante { get; private set; }

    // Se evita duplicación de roles usando un HashSet
    private readonly HashSet<Rol> _roles = [];
    public IReadOnlyCollection<Rol> Roles => _roles;
    private Cuenta()
    {
    }

    public static Cuenta Crear(string nombre, string apellido, Email email, PasswordHash passwordHash)
    {
        ValidarCreacion(nombre, apellido, email, passwordHash);

        var cuenta = new Cuenta();
        cuenta.Id = Guid.NewGuid();
        cuenta.Nombre = nombre;
        cuenta.Apellido = apellido;
        cuenta.Email = email;
        cuenta.PasswordHash = passwordHash;
        cuenta._roles.Add(Rol.Visitante);
        return cuenta;
    }

    private static void ValidarCreacion(string nombre, string apellido, Email email, PasswordHash passwordHash)
    {
        ValidarCampoRequerido(nombre, "Nombre");
        ValidarCampoRequerido(apellido, "Apellido");
        ValidarObjetoRequerido(email, "Email");
        ValidarObjetoRequerido(passwordHash, "Password");
    }

    private static void ValidarCampoRequerido(string valor, string nombreCampo)
    {
        if(string.IsNullOrWhiteSpace(valor))
        {
            throw new ExcepcionDominio($"{nombreCampo} es requerido");
        }
    }

    private static void ValidarObjetoRequerido(object obj, string nombreCampo)
    {
        if(obj == null)
        {
            throw new ExcepcionDominio($"{nombreCampo} es requerido");
        }
    }
}
