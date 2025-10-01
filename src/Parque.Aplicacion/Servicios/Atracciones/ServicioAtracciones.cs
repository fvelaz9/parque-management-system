using Parque.Dominio.Atracciones;
using Parque.Infraestructura.Repositorios.Atracciones;

namespace Parque.Aplicacion.Servicios.Atracciones;

public class ServicioAtracciones : IServicioAtracciones
{
    private readonly IAtraccionRepositorio _repositorio;

    public ServicioAtracciones(IAtraccionRepositorio repositorio)
    {
        _repositorio = repositorio;
    }

    public AtraccionParque CrearAtraccion(string nombre, TipoAtraccion tipo, int edadMinima, int capacidad,
        string descripcion)
    {
        if(capacidad <= 0)
        {
            throw new ArgumentException("La capacidad debe ser mayor a 0");
        }

        var atraccion = new AtraccionParque(nombre, tipo, edadMinima, capacidad, descripcion);
        _repositorio.Agregar(atraccion);
        return atraccion;
    }

    public IEnumerable<AtraccionParque> ListarAtracciones() => _repositorio.ObtenerTodos();

    public AtraccionParque? BuscarAtraccion(int id) => _repositorio.ObtenerPorId(id);

    public void ModificarAtraccion(int id, string nombre, TipoAtraccion tipo, int edadMinima, int capacidad, string descripcion)
    {
        var atraccion = _repositorio.ObtenerPorId(id) ?? throw new ArgumentException("Atracción no encontrada");

        atraccion.Nombre = nombre;
        atraccion.Tipo = tipo;
        atraccion.EdadMinima = edadMinima;
        atraccion.Capacidad = capacidad;
        atraccion.Descripcion = descripcion;

        _repositorio.Actualizar(atraccion);
    }

    public void EliminarAtraccion(int id) => _repositorio.Eliminar(id);
}
