using Parque.Aplicacion.DTOS;
using Parque.Aplicacion.Servicios.Atracciones;
using Parque.Dominio.Atracciones;
using Parque.Infraestructura.Repositorios;

public class ServicioAtracciones(IRepositorio<AtraccionParque> repositorio, IRepositorio<RegistroVisita> repositorio2) : IServicioAtracciones
{
    private readonly IRepositorio<AtraccionParque> _repositorio = repositorio;
    private readonly IRepositorio<RegistroVisita> _repositorioRegistros = repositorio2;

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
        var atraccion = _repositorio.Encontrar(a => a.Id == id);
        if(atraccion == null)
        {
            throw new ArgumentException("Atraccion no encontrada");
        }

        atraccion.Nombre = nombre;
        atraccion.Tipo = tipo;
        atraccion.EdadMinima = edadMinima;
        atraccion.Capacidad = capacidad;
        atraccion.Descripcion = descripcion;

        _repositorio.Editar(atraccion);
    }

    public void EliminarAtraccion(int id) =>
        _repositorio.Eliminar(a => a.Id == id);
    public RegistroVisita RegistrarIngreso(Guid identificador, int idAtraccion)
    {
        var atraccion = _repositorio.Encontrar(a => a.Id == idAtraccion);
        if(atraccion == null)
        {
            throw new ArgumentException("Atraccion no encontrada");
        }

        var registro = new RegistroVisita
        {
            AtraccionId = idAtraccion, Identificador = identificador, FechaIngreso = DateTime.Now
        };
        _repositorioRegistros.Agregar(registro);
        return registro;
    }

    public RegistroVisita RegistrarEgreso(Guid identificador, int idAtraccion)
    {
        var registro = _repositorioRegistros.Encontrar(a => a.Identificador == identificador && a.AtraccionId == idAtraccion && a.FechaEgreso == null);

        if(registro == null)
        {
            throw new ArgumentException("No hay ingreso registrado o ya se registró el egreso");
        }

        registro.FechaEgreso = DateTime.Now;
        _repositorioRegistros.Editar(registro);
        return registro;
    }

    public List<ReporteAtraccionDto> ObtenerReporteUso(DateTime fechaInicio, DateTime fechaFin)
    {
        var registros = _repositorioRegistros.ObtenerTodos()
            .Where(r => r.FechaIngreso >= fechaInicio && r.FechaEgreso <= fechaFin)
            .GroupBy(r => r.AtraccionId)
            .Select(g => new ReporteAtraccionDto { AtraccionId = g.Key, })
            .ToList();
        foreach (var reporte in registros)
        {
            var atraccion = _repositorio.Encontrar(a => a.Id == reporte.AtraccionId);
            if (atraccion == null)
            {
                reporte.NombreAtraccion = "Desconocida";
            }
            else
            {
                reporte.NombreAtraccion = atraccion.Nombre;
            }
        }

        return registros.OrderByDescending(r => r.CantidadVisitas).ToList();
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
}
