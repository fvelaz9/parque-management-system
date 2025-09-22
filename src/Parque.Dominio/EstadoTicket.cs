namespace Parque.Dominio;

/// <summary>
/// Representa los posibles estados de un ticket.
/// </summary>
public enum EstadoTicket
{
    /// <summary>
    /// El ticket está activo y válido.
    /// </summary>
    Activo,

    /// <summary>
    /// El ticket ha sido utilizado.
    /// </summary>
    Utilizado,

    /// <summary>
    /// El ticket ha sido cancelado.
    /// </summary>
    Cancelado
}
