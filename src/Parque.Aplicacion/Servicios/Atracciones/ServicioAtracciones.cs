using Parque.Aplicacion.DTOs;
using Parque.Aplicacion.Servicios.Atracciones;
using Parque.Dominio.Atracciones;
using Parque.Dominio.Excepciones;
using Parque.Infraestructura.Repositorios;

public class ServicioAtracciones(IRepositorio<AtraccionParque> repositorio, IRepositorio<RegistroVisita> repositorio2) : IServicioAtracciones
{
    private readonly IRepositorio<AtraccionParque> _repositorio = repositorio;
    private readonly IRepositorio<RegistroVisita> _repositorioRegistros = repositorio2;

    public AtraccionParque CrearAtraccion(string nombre, TipoAtraccion tipo, int edadMinima, int capacidad, string descripcion)
    {
        ValidarDatosAtraccion(nombre, descripcion, edadMinima, capacidad);

        var atraccion = new AtraccionParque(nombre, tipo, edadMinima, capacidad, descripcion);
        _repositorio.Agregar(atraccion);
        return atraccion;
    }

    public IEnumerable<AtraccionParque> ListarAtracciones() => _repositorio.ObtenerTodos();

    public AtraccionParque? BuscarAtraccion(int id) =>
        _repositorio.Encontrar(a => a.Id == id);

    public IEnumerable<AtraccionParque> ObtenerPorIds(List<int> ids)
    {
        return _repositorio.ObtenerTodos()
            .Where(a => ids.Contains(a.Id))
            .ToList();
    }

    public AtraccionParque ModificarAtraccion(int id, string nombre, TipoAtraccion tipo, int edadMinima, int capacidad, string descripcion)
    {
        var atraccion = _repositorio.Encontrar(a => a.Id == id);
        if(atraccion == null)
        {
            throw new ArgumentException("Atraccion no encontrada");
        }

        ValidarDatosAtraccion(nombre, descripcion, edadMinima, capacidad);

        atraccion.Nombre = nombre;
        atraccion.Tipo = tipo;
        atraccion.EdadMinima = edadMinima;
        atraccion.Capacidad = capacidad;
        atraccion.Descripcion = descripcion;

        _repositorio.Editar(atraccion);
        return atraccion;
    }

    public void EliminarAtraccion(int id) =>
        _repositorio.Eliminar(a => a.Id == id);

    public ReporteAtraccionDto ObtenerReporteUso(int atraccionId, DateTime fechaInicio, DateTime fechaFin)
    {
        var atraccion = _repositorio.Encontrar(a => a.Id == atraccionId);
        if(atraccion == null)
        {
            throw new ExcepcionEntidadNoEncontrada($"Atracción con ID {atraccionId} no encontrada");
        }

        var cantidadVisitas = _repositorioRegistros.ObtenerTodos()
            .Count(r => r.AtraccionId == atraccionId && r.FechaIngreso >= fechaInicio && r.FechaIngreso <= fechaFin);

        return new ReporteAtraccionDto
        {
            AtraccionId = atraccion.Id,
            NombreAtraccion = atraccion.Nombre,
            CantidadVisitas = cantidadVisitas
        };
    }

    public AforoAtraccionDto ObtenerAforoActual(int atraccionId)
    {
        var atraccion = _repositorio.Encontrar(d => d.Id == atraccionId);
        if(atraccion == null)
        {
            throw new ArgumentException("Atraccion no encontrada");
        }

        var aforo = _repositorioRegistros.ObtenerTodos()
            .Count(r => r.AtraccionId == atraccionId && r.FechaEgreso == null);

        return new AforoAtraccionDto
        {
            AtraccionId = atraccion.Id,
            NombreAtraccion = atraccion.Nombre,
            AforoActual = aforo,
            CapacidadMaxima = atraccion.Capacidad,
            Disponible = atraccion.Capacidad - aforo
        };
    }

    private void ValidarDatosAtraccion(string nombre, string descripcion, int edadMinima, int capacidad)
    {
        if(string.IsNullOrWhiteSpace(nombre))
        {
            throw new ArgumentException("El nombre de la atracción es requerido");
        }

        if(string.IsNullOrWhiteSpace(descripcion))
        {
            throw new ArgumentException("La descripción de la atracción es requerida");
        }

        if(edadMinima < 0)
        {
            throw new ArgumentException("La edad mínima no puede ser negativa");
        }

        if(capacidad <= 0)
        {
            throw new ArgumentException("La capacidad debe ser mayor a 0");
        }

        var atraccionRepe = _repositorio.Encontrar(a => a.Nombre == nombre);
        if(atraccionRepe != null)
        {
            throw new ArgumentException("Ese nombre de atraccion ya existe");
        }
    }
}
