namespace Parque.Dominio;

public class Ticket
{
    public int Id { get; set; }
    public int CuentaId { get; set; }
    public DateTime FechaVisita { get; set; }
    public int EventoId { get; set; }
    public Guid Codigo { get; set; } = Guid.NewGuid();
    public DateTime FechaEmision { get; set; } = DateTime.Now;
    public bool EsValido { get; set; } = true;

    public Ticket(int cuentaId, DateTime fechaVisita, int eventoId)
    {
        if(fechaVisita <= DateTime.Now)
        {
            throw new ArgumentException("La fecha de visita debe ser futura");
        }

        CuentaId = cuentaId;
        FechaVisita = fechaVisita;
        EventoId = eventoId;
        Codigo = Guid.NewGuid();
        FechaEmision = DateTime.Now;
        EsValido = true;
    }

    public Ticket() { }
    
    public void MarcarComoUsado()
    {
        EsValido = false;
    }
    
    public bool EstaVigente()
    {
        return EsValido && FechaVisita.Date >= DateTime.Now.Date;
    }
}
