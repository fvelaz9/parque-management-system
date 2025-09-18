using Parque.Dominio.Excepciones;

namespace Parque.Dominio.Usuarios;
public class Cuenta
{
    public Guid Id { get; private set; }
    public string Nombre { get; private set; } = string.Empty;
    public string Apellido { get; private set; } = string.Empty;
    public Email Email { get; private set; } = default!;
    public PasswordHash PasswordHash { get; private set; } = default!;

    // Se evita duplicación de roles usando un HashSet
    private readonly HashSet<Rol> _roles = [];
    public IReadOnlyCollection<Rol> Roles => _roles;
    private Cuenta()
    {
    }

    public static Cuenta Crear(string nombre, string apellido, Email email, PasswordHash passwordHash)
    {
        ValidarDatos(nombre, apellido, email, passwordHash);

        var cuenta = new Cuenta();
        cuenta.Id = Guid.NewGuid();
        cuenta.Nombre = nombre;
        cuenta.Apellido = apellido;
        cuenta.Email = email;
        cuenta.PasswordHash = passwordHash;
        cuenta._roles.Add(Rol.Visitante);
        return cuenta;
    }

    private static void ValidarDatos(string nombre, string apellido, Email email, PasswordHash passwordHash)
    {
        if(string.IsNullOrWhiteSpace(nombre))
        {
            throw new ExcepcionDominio("Nombre es requerido");
        }

        if(string.IsNullOrWhiteSpace(apellido))
        {
            throw new ExcepcionDominio("Apellido es requerido");
        }

        if(email == null)
        {
            throw new ExcepcionDominio("Email es requerido");
        }

        if(passwordHash == null)
        {
            throw new ExcepcionDominio("Password es requerido");
        }
    }
}
