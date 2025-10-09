using Parque.Dominio.Usuarios;

namespace Parque.WebApi.Controllers.Acceso.Models;

public class RegistrarIngresoRequest
{
    public Guid CodigoTicket { get; set; }
    public Cuenta CuentaVisitante { get; set; } = null!;
}
