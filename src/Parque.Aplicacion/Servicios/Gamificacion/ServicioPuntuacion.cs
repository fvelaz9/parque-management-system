using Microsoft.Extensions.Logging;
using Parque.Aplicacion.DTOs.Gamificacion;
using Parque.Dominio;
using Parque.Dominio.Atracciones;
using Parque.Dominio.Gamificacion;
using Parque.Dominio.Usuarios;
using Parque.Infraestructura.Repositorios;

namespace Parque.Aplicacion.Servicios.Gamificacion;

public class ServicioPuntuacion : IServicioPuntuacion
{
    private readonly IRepositorio<AtraccionParque> _repoAtracciones;
    private readonly IRepositorio<Dominio.Ticket> _repoTickets;
    private readonly IRepositorio<RegistroVisita> _repoRegistros;
    private readonly IRepositorio<PuntuacionVisitante> _repoPuntuaciones;
    private readonly IRepositorio<Cuenta> _repoCuentas;
    private readonly IRepositorio<Evento> _repoEventos;
    private readonly IRepositorio<ConfiguracionEstrategia> _repoConfiguracion;
    private readonly IEnumerable<IEstrategiaPuntuacion> _estrategiasBase;
    private readonly IServicioFechaHora _servicioFechaHora;
    private readonly IRepositorio<Visitante> _repoVisitante;
    private readonly PluginLoader _pluginLoader;
    private readonly ILogger<ServicioPuntuacion> _logger;
    private readonly object _lockPlugins = new object();
    private List<IEstrategiaPuntuacion> _estrategiasPlugins;

    public ServicioPuntuacion(
        IRepositorio<AtraccionParque> repoAtracciones,
        IRepositorio<Dominio.Ticket> repoTickets,
        IRepositorio<RegistroVisita> repoRegistros,
        IRepositorio<PuntuacionVisitante> repoPuntuaciones,
        IRepositorio<Cuenta> repoCuentas,
        IRepositorio<Evento> repoEventos,
        IRepositorio<ConfiguracionEstrategia> repoConfiguracion,
        IEnumerable<IEstrategiaPuntuacion> estrategias,
        IServicioFechaHora servicioFechaHora,
        IRepositorio<Visitante> repoVisitante,
        ILogger<ServicioPuntuacion> logger,
        ILogger<PluginLoader> pluginLogger)
    {
        _repoAtracciones = repoAtracciones;
        _repoTickets = repoTickets;
        _repoRegistros = repoRegistros;
        _repoPuntuaciones = repoPuntuaciones;
        _repoCuentas = repoCuentas;
        _repoEventos = repoEventos;
        _repoConfiguracion = repoConfiguracion;
        _estrategiasBase = estrategias;
        _servicioFechaHora = servicioFechaHora;
        _repoVisitante = repoVisitante;
        _logger = logger;

        _pluginLoader = new PluginLoader(pluginLogger);
        _estrategiasPlugins = [];

        CargarPlugins();
    }

    private void CargarPlugins()
    {
        try
        {
            _estrategiasPlugins = _pluginLoader.CargarEstrategiasDesdePlugins();
            _logger.LogInformation("Cargadas {Count} estrategias desde plugins", _estrategiasPlugins.Count);
        }
        catch(Exception ex)
        {
            _logger.LogWarning(ex, "Error al cargar plugins de estrategias");
            _estrategiasPlugins = [];
        }
    }

    public void RecargarPlugins()
    {
        lock(_lockPlugins)
        {
            _logger.LogInformation("Recargando plugins de estrategias");
            CargarPlugins();
        }
    }

    private List<IEstrategiaPuntuacion> ObtenerTodasLasEstrategias()
    {
        var todas = new List<IEstrategiaPuntuacion>(_estrategiasBase);
        todas.AddRange(_estrategiasPlugins);
        return todas;
    }

    public List<EstrategiaDto> ListarEstrategias()
    {
        var estrategiaActivaNombre = ObtenerEstrategiaActiva();
        var todasLasEstrategias = ObtenerTodasLasEstrategias();

        var estrategiasDto = todasLasEstrategias.Select(e => new EstrategiaDto
        {
            Nombre = e.Nombre,
            EsActiva = e.Nombre == estrategiaActivaNombre,
            Origen = _estrategiasBase.Contains(e) ? "Base" : "Plugin"
        }).ToList();

        return estrategiasDto;
    }

    public void CambiarEstrategiaActiva(string nombreEstrategia)
    {
        var todasLasEstrategias = ObtenerTodasLasEstrategias();
        var estrategia = todasLasEstrategias.FirstOrDefault(e => e.Nombre == nombreEstrategia);

        if(estrategia == null)
        {
            throw new ArgumentException($"Estrategia '{nombreEstrategia}' no encontrada", nameof(nombreEstrategia));
        }

        var configuracion = _repoConfiguracion.ObtenerTodos().FirstOrDefault();
        if(configuracion == null)
        {
            configuracion = new ConfiguracionEstrategia(nombreEstrategia);
            _repoConfiguracion.Agregar(configuracion);
        }
        else
        {
            configuracion.CambiarEstrategia(nombreEstrategia);
            _repoConfiguracion.Editar(configuracion);
        }

        _logger.LogInformation("Estrategia activa cambiada a: {Estrategia}", nombreEstrategia);
    }

    public string ObtenerEstrategiaActiva()
    {
        var configuracion = _repoConfiguracion.ObtenerTodos().FirstOrDefault();
        if(configuracion == null)
        {
            var todasLasEstrategias = ObtenerTodasLasEstrategias();
            var estrategiaPorDefecto = todasLasEstrategias.FirstOrDefault();

            if(estrategiaPorDefecto == null)
            {
                throw new InvalidOperationException("No hay estrategias de puntuación registradas");
            }

            configuracion = new ConfiguracionEstrategia(estrategiaPorDefecto.Nombre);
            _repoConfiguracion.Agregar(configuracion);
            return estrategiaPorDefecto.Nombre;
        }

        return configuracion.EstrategiaActiva;
    }

    private IEstrategiaPuntuacion ObtenerEstrategiaActivaInterno()
    {
        var nombreEstrategia = ObtenerEstrategiaActiva();
        var todasLasEstrategias = ObtenerTodasLasEstrategias();

        var estrategia = todasLasEstrategias.FirstOrDefault(e => e.Nombre == nombreEstrategia);
        if(estrategia == null)
        {
            throw new InvalidOperationException(
                $"Estrategia '{nombreEstrategia}' configurada pero no encontrada");
        }

        return estrategia;
    }

    public void CalcularYRegistrarPuntos(int registroVisitaId)
    {
        var registro = _repoRegistros.Encontrar(r => r.Id == registroVisitaId);
        if(registro == null)
        {
            throw new InvalidOperationException($"Registro de visita con ID {registroVisitaId} no encontrado");
        }

        var atraccion = _repoAtracciones.Encontrar(a => a.Id == registro.AtraccionId);
        if(atraccion == null)
        {
            throw new InvalidOperationException($"Atracción con ID {registro.AtraccionId} no encontrada");
        }

        var ticket = _repoTickets.Encontrar(t => t.Codigo == registro.Identificador);
        if(ticket == null)
        {
            throw new InvalidOperationException($"Ticket con código {registro.Identificador} no encontrado");
        }

        var cuenta = _repoCuentas.Encontrar(c => c.Id == ticket.CuentaId);
        if(cuenta == null || cuenta.Visitante == null)
        {
            throw new InvalidOperationException($"Cuenta o Visitante no encontrado para ticket {registro.Identificador}");
        }

        var visitanteId = cuenta.Visitante!.Id;

        var fechaRegistro = registro.FechaIngreso.Date;
        var historialDiario = _repoRegistros
            .ObtenerTodos()
            .Where(r => r.Identificador == registro.Identificador
                        && r.FechaIngreso.Date == fechaRegistro
                        && r.Id != registroVisitaId)
            .OrderBy(r => r.FechaIngreso)
            .ToList();

        var fechaActual = _servicioFechaHora.ObtenerFechaActual();
        var eventoActivo = _repoEventos
            .ObtenerTodos()
            .FirstOrDefault(e => e.Estado == EstadoEvento.Activo
                                 && e.Inicio <= fechaActual
                                 && e.Fin >= fechaActual);

        var estrategiaActiva = ObtenerEstrategiaActivaInterno();

        var puntos = estrategiaActiva.CalcularPuntos(registro, atraccion, historialDiario, eventoActivo);

        var puntuacion = _repoPuntuaciones
            .ObtenerTodos()
            .FirstOrDefault(p => p.VisitanteId == visitanteId && p.Fecha == fechaRegistro);

        if(puntuacion == null)
        {
            puntuacion = new PuntuacionVisitante(visitanteId, fechaRegistro, puntos);
            _repoPuntuaciones.Agregar(puntuacion);
        }
        else
        {
            puntuacion.AgregarPuntos(puntos);
            _repoPuntuaciones.Editar(puntuacion);
        }

        var nombreEstrategia = estrategiaActiva.Nombre;
        var origenPuntuacion = eventoActivo != null
            ? $"Evento: {eventoActivo.Titulo}"
            : $"Atraccion: {atraccion.Nombre}";

        var visitante = _repoVisitante.Encontrar(c => c.Id == visitanteId);
        if(visitante == null)
        {
            throw new InvalidOperationException($"Visitante con ID no encontrado");
        }

        AgregarPuntuacionAVisitante(visitante, puntos, origenPuntuacion, nombreEstrategia);
        _repoVisitante.Editar(visitante);
    }

    public List<RankingVisitanteDto> ObtenerRankingDiario(DateTime? fecha, int top)
    {
        if(top <= 0)
        {
            throw new ArgumentException("El parámetro 'top' debe ser mayor a 0", nameof(top));
        }

        var fechaConsulta = fecha?.Date ?? _servicioFechaHora.ObtenerFechaActual().Date;
        var ranking = _repoPuntuaciones
            .ObtenerTodos()
            .Where(p => p.Fecha == fechaConsulta)
            .OrderByDescending(p => p.PuntosDiarios)
            .Take(top)
            .Select(p => new
            {
                Puntuacion = p,
                Visitante = _repoVisitante.Encontrar(v => v.Id == p.VisitanteId),
                Cuenta = _repoCuentas.ObtenerTodos().FirstOrDefault(c => c.Visitante != null && c.Visitante.Id == p.VisitanteId)
            })
            .Select((x, index) => new RankingVisitanteDto
            {
                VisitanteId = x.Puntuacion.VisitanteId,
                Nombre = x.Cuenta?.Nombre,
                PuntosDiarios = x.Puntuacion.PuntosDiarios,
                PuntosTotales = x.Puntuacion.PuntosTotales,
                Posicion = index + 1
            })
            .ToList();

        return ranking;
    }

    public void AgregarPuntuacionAVisitante(Visitante visitante, int puntos, string origenPuntos, string estrategia)
    {
        var fechaActual = _servicioFechaHora.ObtenerFechaActual();
        var puntuacion = new HistorialPuntuacion(fechaActual, origenPuntos, estrategia, puntos);
        visitante.AgregarPuntuacionAHistorial(puntuacion);
    }

    public List<HistorialPuntuacionDto> ObtenerHistorialVisitante(Guid visitanteId)
    {
        var visitante = ObtenerVisitante(visitanteId);

        return visitante.HistorialPuntuaciones
            .OrderByDescending(x => x.FechaHora)
            .Select(x => new HistorialPuntuacionDto
            {
                FechaHora = x.FechaHora,
                EstrategiaActiva = x.EstrategiaActiva,
                OrigenPuntos = x.OrigenPuntos,
                Puntos = x.Puntos
            }).ToList();
    }

    public Visitante ObtenerVisitante(Guid visitanteId)
    {
        var visitante = _repoVisitante.Encontrar(v => v.Id == visitanteId);
        if(visitante == null)
        {
            throw new InvalidOperationException($"Visitante con ID {visitanteId} no encontrado");
        }

        return visitante;
    }
}
