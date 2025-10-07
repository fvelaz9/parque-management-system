namespace Parque.Aplicacion.DTOS;

public class ValidarAccesoRequest
{
    public Guid CodigoTicket { get; set; }
    public int AtraccionId { get; set; }
    public int EdadVisitante { get; set; }
}
