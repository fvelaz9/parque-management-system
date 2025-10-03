using Parque.Aplicacion.DTOs.Usuarios;
using Parque.Dominio.Usuarios;
using Parque.Infraestructura.Repositorios;

namespace Parque.Aplicacion.Servicios;
public class ServicioCuenta(IRepositorio<Cuenta> repositorioCuenta) : IServicioCuenta
{
    private readonly IRepositorio<Cuenta> _repositorioCuenta = repositorioCuenta;

    public Cuenta RegistrarVisitante(RegistrarVisitanteDto dto)
    {
        throw new NotImplementedException();
    }
}
