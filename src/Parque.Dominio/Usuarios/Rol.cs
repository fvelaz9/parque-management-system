namespace Parque.Dominio.Usuarios;

/// <summary>
/// Defines the roles available in the system.
/// </summary>
public enum Rol
{
    /// <summary>
    /// User with full administrative privileges.
    /// </summary>
    Administrador = 1,

    /// <summary>
    /// User with operational permissions.
    /// </summary>
    Operador = 2,

    /// <summary>
    /// User with visitor-level access.
    /// </summary>
    Visitante = 3
}
