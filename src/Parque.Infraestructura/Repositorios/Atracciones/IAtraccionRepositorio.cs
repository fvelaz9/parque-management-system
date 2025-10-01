using Parque.Dominio.Atracciones;

namespace Parque.Infraestructura.Repositorios.Atracciones;
using Dominio;

public interface IAtraccionRepositorio
{
    AtraccionParque? ObtenerPorId(int id);
    IEnumerable<AtraccionParque> ObtenerTodos();
    void Agregar(AtraccionParque atraccion);
    void Actualizar(AtraccionParque atraccion);
    void Eliminar(int id);
}
