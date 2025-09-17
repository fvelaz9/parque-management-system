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
    public Guid? VisitanteId { get; private set; }

    private Cuenta()
    {
    }

    public static Cuenta Crear(string nombre, string apellido, Email email, PasswordHash pass)
    {
        return new Cuenta { Id = Guid.NewGuid(), Nombre = nombre, Apellido = apellido, Email = email, PasswordHash = pass};
    }
}
