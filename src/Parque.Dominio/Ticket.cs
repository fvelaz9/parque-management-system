namespace Parque.Dominio;

public class Ticket
{
    public int Id { get; set; }
    public Guid CuentaId { get; set; }
    public DateTime FechaVisita { get; set; }
    public int? EventoId { get; set; }
    public TipoTicket TipoEntrada { get; set; }
    public Guid Codigo { get; set; } = Guid.NewGuid();
    public DateTime FechaEmision { get; set; }
    public bool EsValido { get; set; } = true;

    public Ticket(Guid cuentaId, DateTime fechaVisita, int eventoId, TipoTicket tipoEntrada, DateTime fechaActual)
    {
        if(fechaVisita.Date < fechaActual.Date)
        {
            throw new ArgumentException("La fecha de visita debe ser futura");
        }

        CuentaId = cuentaId;
        FechaVisita = fechaVisita;
        EventoId = eventoId;
        TipoEntrada = tipoEntrada;
        Codigo = Guid.NewGuid();
        FechaEmision = fechaActual;
        EsValido = true;
    }

    public Ticket()
    {
    }

    public void MarcarComoUsado()
    {
        EsValido = false;
    }

    public bool EstaVigente(DateTime fechaReferencia)
    {
        return EsValido && FechaVisita.Date >= fechaReferencia.Date;
    }
}
