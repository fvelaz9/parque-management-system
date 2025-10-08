using Parque.Aplicacion.DTOS.Gamificacion;

namespace Parque.Aplicacion.Servicios.Gamificacion;

public interface IServicioPuntuacion
{
    void CalcularYRegistrarPuntos(int registroVisitaId);
    List<RankingVisitanteDto> ObtenerRankingDiario(DateTime? fecha, int top);
    List<EstrategiaDto> ListarEstrategias();
    void CambiarEstrategiaActiva(string nombreEstrategia);
    string ObtenerEstrategiaActiva();
}
