using Parque.Dominio.Usuarios;
namespace Parque.Aplicacion.DTOS;

public class ValidarAccesoRequest
{
    public Guid CodigoTicket { get; set; }
    public int AtraccionId { get; set; }
    public Cuenta? CuentaVisitante { get; set; }
}
