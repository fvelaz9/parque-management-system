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
        var cuenta = new Cuenta();

        cuenta.Id = Guid.NewGuid();
        cuenta.Nombre = nombre;
        cuenta.Apellido = apellido;
        cuenta.Email = email;
        cuenta.PasswordHash = passwordHash;
        cuenta._roles.Add(Rol.Visitante);

        return cuenta;
    }
}
