using Parque.Dominio.Atracciones;

namespace Parque.Infraestructura.Repositorios.Atracciones;
using Dominio;

public interface IAtraccionRepositorio
{
    AtraccionParque? GetById(int id);
    IEnumerable<AtraccionParque> GetAll();
    void Add(AtraccionParque atraccion);
    void Update(AtraccionParque atraccion);
    void Delete(int id);
}
