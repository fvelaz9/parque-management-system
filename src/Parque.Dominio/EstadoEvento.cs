namespace Parque.Dominio;

/// <summary>
/// Representa los posibles estados de un evento en el parque.
/// </summary>
public enum EstadoEvento
{
    /// <summary>
    /// El evento está programado y activo.
    /// </summary>
    Programado,

    /// <summary>
    /// El evento ha sido cancelado.
    /// </summary>
    Cancelado,

    /// <summary>
    /// El evento ha finalizado.
    /// </summary>
    Finalizado
}
