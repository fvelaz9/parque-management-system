namespace Parque.Aplicacion.DTOs;

public class ValidarAccesoRequest
{
    public Guid CodigoTicket { get; set; }
    public int AtraccionId { get; set; }
    public Guid CuentaVisitanteId { get; set; }
}
