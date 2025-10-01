using Parque.Dominio.Atracciones;

namespace Parque.Infraestructura.Repositorios.Atracciones;

public class AtraccionRepositorio : IAtraccionRepositorio
{
    private readonly AppContexto _context;

    public AtraccionRepositorio(AppContexto contexto)
    {
        _context = contexto;
    }

    public AtraccionParque? GetById(int id) =>
        _context.Atracciones.FirstOrDefault(a => a.Id == id);

    public IEnumerable<AtraccionParque> GetAll() =>
        _context.Atracciones.ToList();

    public void Add(AtraccionParque atraccion)
    {
        _context.Atracciones.Add(atraccion);
        _context.SaveChanges();
    }

    public void Update(AtraccionParque atraccion)
    {
        _context.Atracciones.Update(atraccion);
        _context.SaveChanges();
    }

    public void Delete(int id)
    {
        var atraccion = GetById(id);
        if (atraccion != null)
        {
            _context.Atracciones.Remove(atraccion);
            _context.SaveChanges();
        }
    }
}
