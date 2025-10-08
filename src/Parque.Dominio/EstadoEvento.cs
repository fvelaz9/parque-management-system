namespace Parque.Dominio;

/// <summary>
/// Representa los posibles estados de un evento en el parque.
/// </summary>
public enum EstadoEvento
{
    /// <summary>
    /// El evento est� programado y activo.
    /// </summary>
    Programado = 1,

    /// <summary>
    /// El evento ha sido cancelado.
    /// </summary>
    Activo = 2,

    /// <summary>
    /// El evento ha finalizado.
    /// </summary>
    Finalizado = 3,
    
    /// <summary>
    /// El evento ha finalizado.
    /// </summary>
    Cancelado = 4
}
