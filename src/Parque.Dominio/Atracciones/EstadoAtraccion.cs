namespace Parque.Dominio.Atracciones;

/// <summary>
/// Representa los posibles estados de una atracción del parque.
/// </summary>
public enum EstadoAtraccion
{
    /// <summary>
    /// La atracción está disponible para su uso.
    /// </summary>
    Disponible,

    /// <summary>
    /// La atracción está fuera de servicio temporal o permanentemente.
    /// </summary>
    FueraDeServicio
}
