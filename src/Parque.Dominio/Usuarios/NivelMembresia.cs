namespace Parque.Dominio.Usuarios;

/// <summary>
/// Representa los niveles de membresía disponibles para los usuarios.
/// </summary>
public enum NivelMembresia
{
    /// <summary>
    /// Membresía estándar con acceso básico.
    /// </summary>
    Estandar = 1,

    /// <summary>
    /// Membresía premium con beneficios adicionales.
    /// </summary>
    Premium = 2,

    /// <summary>
    /// Membresía VIP con todos los beneficios disponibles.
    /// </summary>
    VIP = 3
}
