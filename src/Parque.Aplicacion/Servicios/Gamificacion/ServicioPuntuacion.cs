using Parque.Aplicacion.DTOS.Gamificacion;
using Parque.Dominio;
using Parque.Dominio.Atracciones;
using Parque.Dominio.Gamificacion;
using Parque.Dominio.Usuarios;
using Parque.Infraestructura.Repositorios;

namespace Parque.Aplicacion.Servicios.Gamificacion;

public class ServicioPuntuacion(IRepositorio<AtraccionParque> repoAtracciones, IRepositorio<Dominio.Ticket> repoTickets,
    IRepositorio<RegistroVisita> repoRegistros, IRepositorio<PuntuacionVisitante> repoPuntuaciones, IRepositorio<Cuenta> repoCuentas,
    IRepositorio<Evento> repoEventos, IRepositorio<ConfiguracionEstrategia> repoConfiguracion, IEnumerable<IEstrategiaPuntuacion> estrategias) : IServicioPuntuacion
{
    public void CalcularYRegistrarPuntos(int registroVisitaId)
    {
        // 1. Obtener el registro de visita
        var registro = repoRegistros.Encontrar(r => r.Id == registroVisitaId);
        if(registro == null)
        {
            throw new InvalidOperationException($"Registro de visita con ID {registroVisitaId} no encontrado");
        }

        // 2. Obtener la atracción
        var atraccion = repoAtracciones.Encontrar(a => a.Id == registro.AtraccionId);
        if(atraccion == null)
        {
            throw new InvalidOperationException($"Atracción con ID {registro.AtraccionId} no encontrada");
        }

        // 3. Resolver el visitante a través de Ticket -> Cuenta
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

        var visitanteId = cuenta.Visitante.Id;

        // 4. Obtener historial diario del visitante (registros del mismo día)
        var fechaRegistro = registro.FechaIngreso.Date;
        var historialDiario = repoRegistros
            .ObtenerTodos()
            .Where(r => r.Identificador == registro.Identificador
                        && r.FechaIngreso.Date == fechaRegistro
                        && r.Id != registroVisitaId) // Excluir el registro actual
            .OrderBy(r => r.FechaIngreso)
            .ToList();

        // 5. Obtener evento activo (si existe)
        var eventoActivo = repoEventos
            .ObtenerTodos()
            .FirstOrDefault(e => e.Estado == EstadoEvento.Activo
                                 && e.Inicio <= DateTime.Now
                                 && e.Fin >= DateTime.Now);

        // 6. Obtener la estrategia activa
        var estrategiaActiva = ObtenerEstrategiaActivaInterno();

        // 7. Calcular puntos usando la estrategia
        var puntos = estrategiaActiva.CalcularPuntos(registro, atraccion, historialDiario, eventoActivo);

        // 8. Registrar o actualizar puntuación del visitante
        var puntuacion = repoPuntuaciones
            .ObtenerTodos()
            .FirstOrDefault(p => p.VisitanteId == visitanteId && p.Fecha == fechaRegistro);

        if(puntuacion == null)
        {
            // Crear nueva puntuación
            puntuacion = new PuntuacionVisitante(visitanteId, fechaRegistro, puntos);
            repoPuntuaciones.Agregar(puntuacion);
        }
        else
        {
            // Actualizar puntuación existente
            puntuacion.AgregarPuntos(puntos);

            repoPuntuaciones.Editar(puntuacion);
        }
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

    public List<RankingVisitanteDto> ObtenerRankingDiario(DateTime? fecha, int top)
    {
    }

    public List<EstrategiaDto> ListarEstrategias()
    {
    }

    public void CambiarEstrategiaActiva(string nombreEstrategia)
    {
    }

    public string ObtenerEstrategiaActiva()
    {
    }
}
