namespace Parque.WebApi.Controllers.Acceso.Models;

public class RegistrarIngresoRequest
{
    public Guid CodigoTicket { get; set; }
    public Guid CuentaVisitanteId { get; set; }
}
