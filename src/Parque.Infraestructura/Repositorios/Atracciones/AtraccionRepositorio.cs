using Parque.Dominio.Atracciones;

namespace Parque.Infraestructura.Repositorios.Atracciones;

public class AtraccionRepositorio : IAtraccionRepositorio
{
    private readonly AppContexto _context;

    public AtraccionRepositorio(AppContexto contexto)
    {
        _context = contexto;
    }

    public AtraccionParque? ObtenerPorId(int id) =>
        _context.Atracciones.FirstOrDefault(a => a.Id == id);

    public IEnumerable<AtraccionParque> ObtenerTodos() =>
        _context.Atracciones.ToList();

    public void Agregar(AtraccionParque atraccion)
    {
        _context.Atracciones.Add(atraccion);
        _context.SaveChanges();
    }

    public void Actualizar(AtraccionParque atraccion)
    {
        _context.Atracciones.Update(atraccion);
        _context.SaveChanges();
    }

    public void Eliminar(int id)
    {
        var atraccion = ObtenerPorId(id);
        if (atraccion != null)
        {
            _context.Atracciones.Remove(atraccion);
            _context.SaveChanges();
        }
    }
}
