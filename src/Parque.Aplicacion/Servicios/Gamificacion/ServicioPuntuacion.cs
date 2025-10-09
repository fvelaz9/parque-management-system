using Parque.Aplicacion.DTOS.Gamificacion;
using Parque.Dominio;
using Parque.Dominio.Atracciones;
using Parque.Dominio.Gamificacion;
using Parque.Dominio.Usuarios;
using Parque.Infraestructura.Repositorios;

namespace Parque.Aplicacion.Servicios.Gamificacion;

public class ServicioPuntuacion(IRepositorio<AtraccionParque> repoAtracciones, IRepositorio<Dominio.Ticket> repoTickets,
    IRepositorio<RegistroVisita> repoRegistros, IRepositorio<PuntuacionVisitante> repoPuntuaciones, IRepositorio<Cuenta> repoCuentas,
    IRepositorio<Evento> repoEventos, IRepositorio<ConfiguracionEstrategia> repoConfiguracion, IEnumerable<IEstrategiaPuntuacion> estrategias,
    IServicioFechaHora servicioFechaHora) : IServicioPuntuacion
{
    public void CalcularYRegistrarPuntos(int registroVisitaId)
    {
        var registro = repoRegistros.Encontrar(r => r.Id == registroVisitaId);
        if(registro == null)
        {
            throw new InvalidOperationException($"Registro de visita con ID {registroVisitaId} no encontrado");
        }

        var atraccion = repoAtracciones.Encontrar(a => a.Id == registro.AtraccionId);
        if(atraccion == null)
        {
            throw new InvalidOperationException($"Atracción con ID {registro.AtraccionId} no encontrada");
        }

        var ticket = repoTickets.Encontrar(t => t.Codigo == registro.Identificador);
        if(ticket == null)
        {
            throw new InvalidOperationException($"Ticket con código {registro.Identificador} no encontrado");
        }

        var cuenta = repoCuentas.Encontrar(c => c.Id == ticket.CuentaId);
        if(cuenta == null)
        {
            throw new InvalidOperationException($"Cuenta con ID {ticket.CuentaId} no encontrada");
        }

        var visitanteId = cuenta.Visitante!.Id;

        var fechaRegistro = registro.FechaIngreso.Date;
        var historialDiario = repoRegistros
            .ObtenerTodos()
            .Where(r => r.Identificador == registro.Identificador
                        && r.FechaIngreso.Date == fechaRegistro
                        && r.Id != registroVisitaId)
            .OrderBy(r => r.FechaIngreso)
            .ToList();

        var fechaActual = servicioFechaHora.ObtenerFechaActual();
        var eventoActivo = repoEventos
            .ObtenerTodos()
            .FirstOrDefault(e => e.Estado == EstadoEvento.Activo
                                 && e.Inicio <= fechaActual
                                 && e.Fin >= fechaActual);

        var estrategiaActiva = ObtenerEstrategiaActivaInterno();

        var puntos = estrategiaActiva.CalcularPuntos(registro, atraccion, historialDiario, eventoActivo);

        var puntuacion = repoPuntuaciones
            .ObtenerTodos()
            .FirstOrDefault(p => p.VisitanteId == visitanteId && p.Fecha == fechaRegistro);

        if(puntuacion == null)
        {
            puntuacion = new PuntuacionVisitante(visitanteId, fechaRegistro, puntos);
            repoPuntuaciones.Agregar(puntuacion);
        }
        else
        {
            puntuacion.AgregarPuntos(puntos);

            repoPuntuaciones.Editar(puntuacion);
        }
    }

    public List<RankingVisitanteDto> ObtenerRankingDiario(DateTime? fecha, int top)
    {
        if(top <= 0)
        {
            throw new ArgumentException("El parámetro 'top' debe ser mayor a 0", nameof(top));
        }

        var fechaConsulta = fecha?.Date ?? servicioFechaHora.ObtenerFechaActual().Date;

        var ranking = repoPuntuaciones
            .ObtenerTodos()
            .Where(p => p.Fecha == fechaConsulta)
            .OrderByDescending(p => p.PuntosDiarios)
            .Take(top)
            .Select((p, index) => new RankingVisitanteDto
            {
                VisitanteId = p.VisitanteId,
                PuntosDiarios = p.PuntosDiarios,
                PuntosTotales = p.PuntosTotales,
                Posicion = index + 1
            })
            .ToList();

        return ranking;
    }

    public List<EstrategiaDto> ListarEstrategias()
    {
        var estrategiaActivaNombre = ObtenerEstrategiaActiva();

        var estrategiasDto = estrategias.Select(e => new EstrategiaDto
        {
            Nombre = e.Nombre,
            EsActiva = e.Nombre == estrategiaActivaNombre
        }).ToList();

        return estrategiasDto;
    }

    public void CambiarEstrategiaActiva(string nombreEstrategia)
    {
        var estrategia = estrategias.FirstOrDefault(e => e.Nombre == nombreEstrategia);
        if(estrategia == null)
        {
            throw new ArgumentException($"Estrategia '{nombreEstrategia}' no encontrada", nameof(nombreEstrategia));
        }

        var configuracion = repoConfiguracion.ObtenerTodos().FirstOrDefault();
        if(configuracion == null)
        {
            configuracion = new ConfiguracionEstrategia(nombreEstrategia);
            repoConfiguracion.Agregar(configuracion);
        }
        else
        {
            configuracion.CambiarEstrategia(nombreEstrategia);
            repoConfiguracion.Editar(configuracion);
        }
    }

    public string ObtenerEstrategiaActiva()
    {
        var configuracion = repoConfiguracion.ObtenerTodos().FirstOrDefault();
        if(configuracion == null)
        {
            var estrategiaPorDefecto = estrategias.FirstOrDefault();
            if(estrategiaPorDefecto == null)
            {
                throw new InvalidOperationException("No hay estrategias de puntuación registradas");
            }

            configuracion = new ConfiguracionEstrategia(estrategiaPorDefecto.Nombre);
            repoConfiguracion.Agregar(configuracion);
            return estrategiaPorDefecto.Nombre;
        }

        return configuracion.EstrategiaActiva;
    }

    private IEstrategiaPuntuacion ObtenerEstrategiaActivaInterno()
    {
        var nombreEstrategia = ObtenerEstrategiaActiva();

        var estrategia = estrategias.FirstOrDefault(e => e.Nombre == nombreEstrategia);
        if(estrategia == null)
        {
            throw new InvalidOperationException($"Estrategia '{nombreEstrategia}' configurada pero no registrada en DI");
        }

        return estrategia;
    }
}
