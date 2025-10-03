using Parque.Aplicacion.Servicios.Atracciones;
using Parque.Dominio.Atracciones;
using Parque.Infraestructura.Repositorios;

public class ServicioAtracciones(IRepositorio<AtraccionParque> repositorio) : IServicioAtracciones
{
    private readonly IRepositorio<AtraccionParque> _repositorio = repositorio;

    public AtraccionParque CrearAtraccion(string nombre, TipoAtraccion tipo, int edadMinima, int capacidad, string descripcion)
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

    public AtraccionParque? BuscarAtraccion(int id) =>
        _repositorio.Encontrar(a => a.Id == id);

    public void ModificarAtraccion(int id, string nombre, TipoAtraccion tipo, int edadMinima, int capacidad, string descripcion)
    {
        var atraccion = _repositorio.Encontrar(a => a.Id == id)
                        ?? throw new ArgumentException("Atracción no encontrada");

        atraccion.Nombre = nombre;
        atraccion.Tipo = tipo;
        atraccion.EdadMinima = edadMinima;
        atraccion.Capacidad = capacidad;
        atraccion.Descripcion = descripcion;

        _repositorio.Editar(atraccion);
    }

    public void EliminarAtraccion(int id) =>
        _repositorio.Eliminar(a => a.Id == id);
}
