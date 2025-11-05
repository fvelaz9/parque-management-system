using Parque.Aplicacion.DTOs.Gamificacion;
using Parque.Dominio.Usuarios;

namespace Parque.Aplicacion.Servicios.Gamificacion;

public interface IServicioPuntuacion
{
    void CalcularYRegistrarPuntos(int registroVisitaId);
    List<RankingVisitanteDto> ObtenerRankingDiario(DateTime? fecha, int top);
    List<EstrategiaDto> ListarEstrategias();
    void CambiarEstrategiaActiva(string nombreEstrategia);
    string ObtenerEstrategiaActiva();
    void AgregarPuntuacionAVisitante(Visitante visitante, int puntos, string origenPuntos, string estrategia);
    List<HistorialPuntuacionDto> ObtenerHistorialPuntuacionesDto(Guid visitanteId);
}
