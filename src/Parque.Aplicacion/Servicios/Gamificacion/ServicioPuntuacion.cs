using Parque.Aplicacion.DTOs.Gamificacion;
using Parque.Dominio;
using Parque.Dominio.Atracciones;
using Parque.Dominio.Gamificacion;
using Parque.Dominio.Usuarios;
using Parque.Infraestructura.Repositorios;

namespace Parque.Aplicacion.Servicios.Gamificacion;

public class ServicioPuntuacion(IRepositorio<AtraccionParque> repoAtracciones, IRepositorio<Dominio.Ticket> repoTickets,
    IRepositorio<RegistroVisita> repoRegistros, IRepositorio<PuntuacionVisitante> repoPuntuaciones, IRepositorio<Cuenta> repoCuentas,
    IRepositorio<Evento> repoEventos, IRepositorio<ConfiguracionEstrategia> repoConfiguracion, IEnumerable<IEstrategiaPuntuacion> estrategias,
    IServicioFechaHora servicioFechaHora, IRepositorio<Visitante> repoVisitante) : IServicioPuntuacion
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
        if(cuenta == null || cuenta.Visitante == null)
        {
            throw new InvalidOperationException($"Cuenta o Visitante no encontrado para ticket {registro.Identificador}");
        }

        var visitanteId = cuenta.Visitante.Id;

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

        var nombreEstrategia = estrategiaActiva.Nombre;
        var origenPuntuacion = " ";
        if(eventoActivo != null)
        {
            origenPuntuacion = $"Evento: {eventoActivo.Titulo}";
        }
        else
        {
            origenPuntuacion = $"Atraccion: {atraccion.Nombre}";
        }

        var visitante = repoVisitante.Encontrar(c => c.Id == visitanteId);
        if(visitante == null)
        {
            throw new InvalidOperationException($"Visitante con ID no encontrado");
        }

        AgregarPuntuacionAVisitante(visitante, puntos, origenPuntuacion, nombreEstrategia);
        repoVisitante.Editar(visitante);
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

    public void AgregarPuntuacionAVisitante(Visitante visitante, int puntos, string origenPuntos, string estrategia)
    {
        var fechaActual = servicioFechaHora.ObtenerFechaActual();
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
        var visitante = repoVisitante.Encontrar(v => v.Id == visitanteId);
        if(visitante == null)
        {
            throw new InvalidOperationException($"Visitante con ID {visitanteId} no encontrado");
        }

        return visitante;
    }
}
